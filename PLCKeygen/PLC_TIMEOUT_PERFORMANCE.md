# PLC Timeout & Performance Monitoring - Technical Guide

## Tổng Quan

PLCKeygen được tích hợp hệ thống **Timeout & Performance Monitoring** để:
- ✅ Phát hiện nhanh khi PLC không kết nối được (2 giây thay vì 20-30 giây)
- ✅ Theo dõi tốc độ response từ PLC
- ✅ Tự động ngắt kết nối khi PLC response chậm
- ✅ Cải thiện user experience (không bị treo UI)

## Vấn Đề Cũ

### Trước Khi Có Timeout

**Scenario 1: PLC Chưa Cắm**
```
User khởi động app
    ↓
TcpClient.Connect(192.168.0.10, 8501)
    ↓
Đợi... đợi... đợi... (20-30 giây)
    ↓
Timeout exception
    ↓
Status bar mới chuyển đỏ
```

❌ **User experience tệ**: Phải đợi 20-30 giây mới biết không kết nối được

**Scenario 2: PLC Chậm/Lag**
```
User gửi command
    ↓
PLC response sau 2-3 giây
    ↓
App vẫn nghĩ là OK
    ↓
Tiếp tục giao tiếp chậm...
```

❌ **Không phát hiện**: Giao tiếp chậm nhưng không có cảnh báo

---

## Giải Pháp Mới

### 1. Connection Timeout (2 giây)

**Code**:
```csharp
private const int CONNECTION_TIMEOUT_MS = 2000;  // 2 seconds

public void Open()
{
    _client = new TcpClient();
    var result = _client.BeginConnect(_ip, _port, null, null);
    var success = result.AsyncWaitHandle.WaitOne(
        TimeSpan.FromMilliseconds(CONNECTION_TIMEOUT_MS)
    );

    if (!success)
    {
        _client.Close();
        throw new Exception($"Connection timeout after {CONNECTION_TIMEOUT_MS}ms");
    }

    _client.EndConnect(result);
}
```

**Flow**:
```
User khởi động app
    ↓
BeginConnect (async)
    ↓
WaitOne(2000ms)
    ↓
┌─────────┬──────────┐
│ Success │ Timeout  │
│ < 2s    │ >= 2s    │
└────┬────┴────┬─────┘
     │         │
     ▼         ▼
Connected   Disconnect
   🟢         🔴
```

✅ **Improvement**: 2 giây thay vì 20-30 giây

---

### 2. Read/Write Timeout (1 giây)

**Code**:
```csharp
private const int READ_WRITE_TIMEOUT_MS = 1000;  // 1 second

_stream.ReadTimeout = READ_WRITE_TIMEOUT_MS;
_stream.WriteTimeout = READ_WRITE_TIMEOUT_MS;
```

**Purpose**:
- Mỗi lệnh đọc/ghi PLC phải hoàn thành trong 1 giây
- Nếu timeout → IOException
- Catch IOException → Track slow response

**Example**:
```csharp
try
{
    _stream.Read(buffer, 0, buffer.Length);  // Must complete in 1s
}
catch (IOException ioEx)
{
    // Read timeout
    _slowResponseCount++;
}
```

---

### 3. Response Time Monitoring

**Code**:
```csharp
private const int SLOW_RESPONSE_THRESHOLD_MS = 500;  // 500ms
private Stopwatch _responseTimer = new Stopwatch();
private int _slowResponseCount = 0;
private const int MAX_SLOW_RESPONSES = 3;

public string SendCommand(string command)
{
    _responseTimer.Restart();

    // Send command and read response
    // ...

    _responseTimer.Stop();
    long responseTimeMs = _responseTimer.ElapsedMilliseconds;

    if (responseTimeMs > SLOW_RESPONSE_THRESHOLD_MS)
    {
        _slowResponseCount++;
        Console.WriteLine($"Slow response: {responseTimeMs}ms (count: {_slowResponseCount}/3)");

        if (_slowResponseCount >= MAX_SLOW_RESPONSES)
        {
            // Disconnect
            IsSessionStarted = false;
            PropertyChangedEvent($"{Tcpstatus.disconnected}");
            Close();
            return "EX:Connection too slow";
        }
    }
    else
    {
        // Good response, decrement counter
        if (_slowResponseCount > 0)
        {
            _slowResponseCount--;
        }
    }
}
```

**Flow**:
```
SendCommand()
    ↓
Start Stopwatch
    ↓
Write + Read
    ↓
Stop Stopwatch
    ↓
Check response time
    ↓
┌──────────┬──────────┐
│ < 500ms  │ >= 500ms │
│ (Good)   │ (Slow)   │
└────┬─────┴────┬─────┘
     │          │
     ▼          ▼
Decrement   Increment
 counter     counter
     │          │
     │          ▼
     │     >= 3 times?
     │          │
     │      ┌───┴───┐
     │      │ Yes   │ No
     │      ▼       │
     │   Disconnect │
     │      🔴      │
     └──────────────┘
           🟢
```

**Why 500ms threshold?**
- Normal PLC response: 10-50ms
- 500ms = 10x slower than normal
- Clearly indicates network/PLC issue

**Why 3 strikes?**
- 1 slow response = might be temporary
- 2 slow responses = suspicious
- 3 slow responses = definitely problem → disconnect

**Why decrement counter?**
- Gradual recovery
- If PLC speed improves, counter goes back to 0
- More forgiving than immediate disconnect

---

## Timeout Values

### Summary Table

| Setting | Value | Purpose |
|---------|-------|---------|
| **CONNECTION_TIMEOUT_MS** | 2000ms (2s) | Timeout khi BeginConnect |
| **READ_WRITE_TIMEOUT_MS** | 1000ms (1s) | Timeout cho mỗi Read/Write |
| **SLOW_RESPONSE_THRESHOLD_MS** | 500ms | Coi là "chậm" nếu >= 500ms |
| **MAX_SLOW_RESPONSES** | 3 | Disconnect sau 3 lần chậm |
| **RECONNECT_INTERVAL** | 3000ms (3s) | Retry interval (Form1.cs) |

### Tuning Guidelines

**Nếu mạng chậm (WiFi, VPN)**:
```csharp
// Increase timeouts
private const int CONNECTION_TIMEOUT_MS = 5000;  // 5 seconds
private const int READ_WRITE_TIMEOUT_MS = 2000;  // 2 seconds
private const int SLOW_RESPONSE_THRESHOLD_MS = 1000;  // 1 second
```

**Nếu mạng nhanh (Ethernet, local)**:
```csharp
// Decrease timeouts for faster detection
private const int CONNECTION_TIMEOUT_MS = 1000;  // 1 second
private const int READ_WRITE_TIMEOUT_MS = 500;   // 500ms
private const int SLOW_RESPONSE_THRESHOLD_MS = 200;  // 200ms
```

**Nếu PLC cũ/chậm**:
```csharp
// More forgiving
private const int SLOW_RESPONSE_THRESHOLD_MS = 1000;  // 1 second
private const int MAX_SLOW_RESPONSES = 5;  // 5 strikes before disconnect
```

---

## Exception Handling

### Exception Types

**1. SocketException** (Network error)
```csharp
catch (IOException ioEx) when (ioEx.InnerException is SocketException)
{
    // Network cable unplugged, switch powered off, etc.
    Console.WriteLine($"[HostLinkTCP] Network error: {ioEx.Message}");
    IsSessionStarted = false;
    PropertyChangedEvent($"{Tcpstatus.disconnected}");
}
```

**2. IOException** (Timeout)
```csharp
catch (IOException ioEx)
{
    // Read/Write timeout (> 1 second)
    Console.WriteLine($"[HostLinkTCP] Timeout: {ioEx.Message}");
    _slowResponseCount++;

    if (_slowResponseCount >= MAX_SLOW_RESPONSES)
    {
        // Too many timeouts, disconnect
        IsSessionStarted = false;
        PropertyChangedEvent($"{Tcpstatus.disconnected}");
        Close();
    }
}
```

**3. Generic Exception**
```csharp
catch (Exception ex)
{
    // Unknown error
    Console.WriteLine($"[HostLinkTCP] Error: {ex.Message}");
    IsSessionStarted = false;
    PropertyChangedEvent($"{Tcpstatus.disconnected}");
}
```

### Why Separate IOException?

**IOException with SocketException**:
- Hard failure (cable unplugged, device off)
- Immediate disconnect

**IOException without SocketException**:
- Soft failure (timeout, slow response)
- Track count before disconnect
- Might recover

---

## Performance Impact

### Overhead Analysis

**Stopwatch Overhead**: ~0.01ms per measurement
- Negligible compared to network latency
- Does not affect PLC communication speed

**Memory Overhead**: ~100 bytes
- 1 Stopwatch object
- 1 int counter
- Minimal impact

**CPU Overhead**: < 0.1%
- Stopwatch.Start/Stop very lightweight
- No background threads

### Before vs After

| Metric | Before | After | Improvement |
|--------|--------|-------|-------------|
| **Connection timeout** | 20-30s | 2s | **10-15x faster** |
| **Slow PLC detection** | Never | 1.5s (3×500ms) | **∞ improvement** |
| **Read/Write timeout** | 20-30s | 1s | **20-30x faster** |
| **User wait time** | Long | Short | ✅ Much better UX |

---

## Test Scenarios

### Test Case 1: PLC Chưa Cắm

**Steps**:
1. Đảm bảo PLC chưa cắm
2. Khởi động PLCKeygen
3. Quan sát status bar

**Expected**:
- ⏱️ Sau **2 giây**: Status chuyển đỏ "Mất kết nối"
- ✅ Progress bar hiển thị
- ✅ Timer retry mỗi 3 giây

**Old Behavior**:
- ❌ Sau **20-30 giây** mới chuyển đỏ

---

### Test Case 2: PLC Chậm (Simulated)

**Simulate**:
```csharp
// In KeyenceHostLinkTcpClient.cs - SendCommand()
// Add delay for testing
System.Threading.Thread.Sleep(600);  // Simulate 600ms response
```

**Expected**:
1. Lần 1: Log "Slow response: 600ms (count: 1/3)"
2. Lần 2: Log "Slow response: 600ms (count: 2/3)"
3. Lần 3: Log "Slow response: 600ms (count: 3/3)"
4. Lần 3: Log "Too many slow responses, disconnecting..."
5. Status bar chuyển đỏ

---

### Test Case 3: Network Congestion

**Scenario**: Mạng bị lag, response time tăng lên 400-600ms

**Expected**:
- Slow response counter tăng
- Nếu response time quay về < 500ms → counter giảm
- Chỉ disconnect nếu liên tục chậm

**Example Log**:
```
[HostLinkTCP] Slow response: 550ms (count: 1/3)
[HostLinkTCP] Slow response: 480ms (count: 0/3)  // Decremented (480 < 500)
[HostLinkTCP] Slow response: 600ms (count: 1/3)
[HostLinkTCP] Response: 50ms  // Good, decrement
```

---

### Test Case 4: PLC Recovery

**Scenario**: PLC chậm → recovery → chậm lại

**Timeline**:
```
t=0s:   Slow 600ms → count=1
t=1s:   Slow 700ms → count=2
t=2s:   Good 50ms  → count=1 (decremented)
t=3s:   Good 40ms  → count=0 (decremented)
t=4s:   Slow 550ms → count=1
t=5s:   Good 30ms  → count=0
```

**Result**: ✅ No disconnect (counter never reached 3)

---

## Debugging

### Enable Debug Logging

Already enabled via `Console.WriteLine`:

```csharp
Console.WriteLine($"[HostLinkTCP] Slow response: {responseTimeMs}ms (count: {_slowResponseCount}/{MAX_SLOW_RESPONSES})");
Console.WriteLine($"[HostLinkTCP] Too many slow responses, disconnecting...");
Console.WriteLine($"[HostLinkTCP] Timeout: {ioEx.Message}");
Console.WriteLine($"[HostLinkTCP] Network error: {ioEx.Message}");
```

### View Debug Output

**Visual Studio**:
1. Run in Debug mode
2. View → Output Window
3. Show output from: Debug

**Example Output**:
```
[HostLinkTCP] Connection failed: Connection timeout after 2000ms
[HostLinkTCP] Slow response: 550ms (count: 1/3)
[HostLinkTCP] Slow response: 620ms (count: 2/3)
[HostLinkTCP] Slow response: 580ms (count: 3/3)
[HostLinkTCP] Too many slow responses, disconnecting...
```

### Manual Testing

**Test Connection Timeout**:
```csharp
// Change IP to non-existent
private const string PLC_IP = "192.168.0.99";  // Does not exist

// Run app → should timeout in 2 seconds
```

**Test Read Timeout**:
```csharp
// In SendCommand(), before Read()
_stream.ReadTimeout = 100;  // Very short timeout

// Any command will timeout
```

---

## Configuration

### Change Timeout Values

**Location**: `KeyenceHostLinkTcpClient.cs` lines 37-40

```csharp
// Current settings
private const int CONNECTION_TIMEOUT_MS = 2000;
private const int READ_WRITE_TIMEOUT_MS = 1000;
private const int SLOW_RESPONSE_THRESHOLD_MS = 500;
private const int MAX_SLOW_RESPONSES = 3;
```

**How to change**:
1. Open `KeyenceHostLinkTcpClient.cs`
2. Modify const values
3. Rebuild solution

**Recommended values**:
- LAN/Ethernet: Current values OK
- WiFi: Increase by 50% (3000, 1500, 750, 3)
- VPN/Remote: Increase by 100% (4000, 2000, 1000, 5)

---

## Best Practices

### ✅ DO

1. **Monitor Debug Output**: Check for slow response logs
2. **Test on Real Network**: Simulate real conditions
3. **Tune for Environment**: Adjust timeouts based on network
4. **Keep Logs**: Save debug output for troubleshooting

### ❌ DON'T

1. **Set Too Low**: Don't set < 1s connection timeout (might false alarm)
2. **Set Too High**: Don't set > 10s (defeats purpose)
3. **Ignore Warnings**: Check logs if seeing frequent slow responses
4. **Disable Monitoring**: Don't remove timeout/monitoring code

---

## Summary

### Key Improvements

1. **⚡ Fast Connection Detection**
   - 2s timeout vs 20-30s
   - 10-15x faster failure detection

2. **⚡ Response Time Monitoring**
   - Track every PLC command
   - Detect degrading performance

3. **⚡ Auto-Disconnect on Slow**
   - 3 strikes rule
   - Prevent UI freeze

4. **⚡ Better Exception Handling**
   - Separate SocketException vs IOException
   - Appropriate response for each type

### User Benefits

- ✅ No more long waits
- ✅ Clear status indication
- ✅ Auto-recovery with reconnect
- ✅ Smooth user experience

Version: PLCKeygen 2025.11.2+ (Performance Update)
