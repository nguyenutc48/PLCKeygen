# Hướng Dẫn Sử Dụng PropertyChanged Event - PLCKeygen

## Tổng Quan

`PropertyChanged` event là một cơ chế trong C# cho phép một object thông báo khi có thay đổi về trạng thái. Trong PLCKeygen, chúng ta sử dụng event này để theo dõi trạng thái kết nối PLC.

## Cách Hoạt Động

### 1. Interface `INotifyPropertyChanged`

Class `PLCKeyence` implement interface `INotifyPropertyChanged`:

```csharp
public class PLCKeyence : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void PropertyChangedEvent(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

### 2. Raise Event Khi Có Thay Đổi

Trong `KeyenceHostLinkTcpClient.cs`, khi có thay đổi trạng thái kết nối:

```csharp
// Khi kết nối thành công
PropertyChangedEvent($"{Tcpstatus.connected}");

// Khi mất kết nối
PropertyChangedEvent($"{Tcpstatus.disconnected}");
```

Enum `Tcpstatus`:
```csharp
public enum Tcpstatus
{
    connected,
    disconnected,
}
```

### 3. Subscribe Event Trong Form1

Trong Form1 constructor, subscribe vào event:

```csharp
PLCKey = new PLCKeyence("192.168.0.10", 8501);

// Subscribe to PropertyChanged event to monitor connection status
PLCKey.PropertyChanged += PLCKey_PropertyChanged;

PLCKey.Open();
PLCKey.StartCommunication();
```

### 4. Event Handler Method

```csharp
/// <summary>
/// Handle PLC connection status changes
/// </summary>
private void PLCKey_PropertyChanged(object sender, PropertyChangedEventArgs e)
{
    // Thread-safe UI update
    if (InvokeRequired)
    {
        Invoke(new Action(() => PLCKey_PropertyChanged(sender, e)));
        return;
    }

    // Parse the property name to get connection status
    string status = e.PropertyName;

    if (status.Contains("connected"))
    {
        // PLC is connected
        toolStripStatusLabel2.Text = "PLC: Đã kết nối (192.168.0.10:8501)";
        toolStripStatusLabel2.ForeColor = Color.Green;
        toolStripProgressBar1.Visible = false;

        Console.WriteLine("[Form1] PLC connected successfully");
    }
    else if (status.Contains("disconnected"))
    {
        // PLC is disconnected
        toolStripStatusLabel2.Text = "PLC: Mất kết nối - Kiểm tra cáp mạng";
        toolStripStatusLabel2.ForeColor = Color.Red;
        toolStripProgressBar1.Visible = true;
        toolStripProgressBar1.Style = ProgressBarStyle.Marquee;

        Console.WriteLine("[Form1] PLC disconnected");
    }
}
```

## Thread Safety

### Tại Sao Cần InvokeRequired?

PLC communication thường xảy ra trên **background thread**, nhưng UI controls chỉ có thể được cập nhật từ **UI thread** (main thread).

```csharp
if (InvokeRequired)
{
    Invoke(new Action(() => PLCKey_PropertyChanged(sender, e)));
    return;
}
```

**Giải thích**:
- `InvokeRequired`: Kiểm tra xem method có đang chạy trên UI thread không
- Nếu `false`: Đang chạy trên UI thread → Cập nhật UI trực tiếp
- Nếu `true`: Đang chạy trên background thread → Dùng `Invoke()` để marshal về UI thread

### Flow Chart

```
Background Thread
    │
    ▼
PropertyChangedEvent("disconnected")
    │
    ▼
PropertyChanged event fired
    │
    ▼
PLCKey_PropertyChanged() called
    │
    ▼
InvokeRequired check
    │
┌───┴────┐
│        │
▼        ▼
true    false
│        │
▼        │
Invoke() │
  │      │
  └──┬───┘
     │
     ▼
UI Thread
     │
     ▼
Update toolStripStatusLabel2
Update toolStripProgressBar1
```

## Khi Nào Event Được Raise?

### 1. Trong `Open()` Method

**Khi connection thất bại**:
```csharp
catch (Exception ex)
{
    PropertyChangedEvent($"{Tcpstatus.disconnected}");
    Console.WriteLine($"[HostLinkTCP] Connection failed: {ex.Message}");
}
```

### 2. Trong `SendCommand()` Method

**Khi có lỗi gửi/nhận**:
```csharp
catch (Exception ex)
{
    Console.WriteLine($"[HostLinkTCP] Error sending command: {ex.Message}");
    IsSessionStarted = false;
    PropertyChangedEvent($"{Tcpstatus.disconnected}");
    return $"EX:{ex.Message}";
}
```

### 3. Trong `ReadUInt16()`, `ReadInt16()`, `ReadInt32()`

**Khi đọc lỗi**:
```csharp
if (response.Contains("not") || response.Contains("EX"))
{
    Console.WriteLine($"Lỗi kết nối đến PLC tại địa chỉ {address}: {response}");
    PropertyChangedEvent($"{Tcpstatus.disconnected}");
    // ...
}
```

**Khi catch exception**:
```csharp
catch (Exception ex)
{
    Console.WriteLine($"Lỗi kết nối đến PLC tại địa chỉ {address}: {ex.Message}");
    IsSessionStarted = false;
    PropertyChangedEvent($"{Tcpstatus.disconnected}");
    return 0;
}
```

### 4. Trong `WriteUInt16()`, `WriteInt16()`, `WriteInt32()`

**Khi kết nối thành công**:
```csharp
else
{
    _isConnected = true;
    _timer.Elapsed -= Reconnect;
    _timer.Stop();
    PropertyChangedEvent($"{Tcpstatus.connected}");
    IsSessionStarted = true;
}
```

**Khi mất kết nối**:
```csharp
if (response.Contains("not") || response.Contains("EX"))
{
    PropertyChangedEvent($"{Tcpstatus.disconnected}");
    IsSessionStarted = false;
    // ...
}
```

## Ví Dụ Sử Dụng

### Ví Dụ 1: Hiển Thị Thông Báo

```csharp
private void PLCKey_PropertyChanged(object sender, PropertyChangedEventArgs e)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => PLCKey_PropertyChanged(sender, e)));
        return;
    }

    string status = e.PropertyName;

    if (status.Contains("disconnected"))
    {
        MessageBox.Show(
            "Mất kết nối PLC!\nVui lòng kiểm tra cáp mạng.",
            "Cảnh Báo",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }
}
```

### Ví Dụ 2: Disable Controls Khi Mất Kết Nối

```csharp
private void PLCKey_PropertyChanged(object sender, PropertyChangedEventArgs e)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => PLCKey_PropertyChanged(sender, e)));
        return;
    }

    string status = e.PropertyName;

    if (status.Contains("connected"))
    {
        // Enable all controls
        btnJogX.Enabled = true;
        btnJogY.Enabled = true;
        btnTeaching.Enabled = true;
    }
    else if (status.Contains("disconnected"))
    {
        // Disable all controls
        btnJogX.Enabled = false;
        btnJogY.Enabled = false;
        btnTeaching.Enabled = false;
    }
}
```

### Ví Dụ 3: Log Connection History

```csharp
private List<string> connectionLog = new List<string>();

private void PLCKey_PropertyChanged(object sender, PropertyChangedEventArgs e)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => PLCKey_PropertyChanged(sender, e)));
        return;
    }

    string status = e.PropertyName;
    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    string logEntry = $"[{timestamp}] PLC Status: {status}";

    connectionLog.Add(logEntry);
    Console.WriteLine(logEntry);

    // Update UI
    if (status.Contains("connected"))
    {
        toolStripStatusLabel2.Text = "PLC: Connected";
        toolStripStatusLabel2.ForeColor = Color.Green;
    }
    else if (status.Contains("disconnected"))
    {
        toolStripStatusLabel2.Text = "PLC: Disconnected";
        toolStripStatusLabel2.ForeColor = Color.Red;
    }
}
```

## Debugging

### 1. Kiểm Tra Event Có Được Subscribe Không

Thêm breakpoint tại:
- Line 52: `PLCKey.PropertyChanged += PLCKey_PropertyChanged;`

Chạy debug và kiểm tra:
- Event handler có được gán không?
- `PLCKey` có null không?

### 2. Kiểm Tra Event Có Được Raise Không

Thêm breakpoint tại:
- `KeyenceHostLinkTcpClient.cs`: Tất cả các dòng `PropertyChangedEvent(...)`

Chạy debug và quan sát:
- Event có được raise không?
- Property name là gì? ("connected" hay "disconnected"?)

### 3. Kiểm Tra Event Handler Có Được Gọi Không

Thêm breakpoint tại:
- `Form1.cs` line 460: `private void PLCKey_PropertyChanged(...)`

Chạy debug và kiểm tra:
- Handler có được gọi không?
- `e.PropertyName` có giá trị gì?

### 4. Kiểm Tra Thread Issue

Thêm log để xem thread ID:

```csharp
private void PLCKey_PropertyChanged(object sender, PropertyChangedEventArgs e)
{
    int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
    Console.WriteLine($"[Form1] PropertyChanged on thread {threadId}");

    if (InvokeRequired)
    {
        Console.WriteLine($"[Form1] InvokeRequired = true, marshalling to UI thread");
        Invoke(new Action(() => PLCKey_PropertyChanged(sender, e)));
        return;
    }

    Console.WriteLine($"[Form1] Updating UI on thread {threadId}");
    // ... update UI ...
}
```

## Common Issues

### Issue 1: Event Handler Không Được Gọi

**Nguyên nhân**: Quên subscribe event

**Giải pháp**:
```csharp
PLCKey = new PLCKeyence("192.168.0.10", 8501);
PLCKey.PropertyChanged += PLCKey_PropertyChanged;  // ← Thêm dòng này
```

### Issue 2: UI Không Update

**Nguyên nhân**: Cross-thread exception

**Giải pháp**: Sử dụng `InvokeRequired` check
```csharp
if (InvokeRequired)
{
    Invoke(new Action(() => PLCKey_PropertyChanged(sender, e)));
    return;
}
```

### Issue 3: Event Raise Quá Nhiều Lần

**Nguyên nhân**: PropertyChangedEvent được gọi trong loop

**Giải pháp**: Debounce hoặc kiểm tra state trước khi raise
```csharp
private bool _lastConnectionState = false;

if (isConnected != _lastConnectionState)
{
    _lastConnectionState = isConnected;
    PropertyChangedEvent(isConnected ? "connected" : "disconnected");
}
```

### Issue 4: Status Bar Không Hiển Thị Màu

**Nguyên nhân**: Quên set `ForeColor`

**Giải pháp**:
```csharp
toolStripStatusLabel2.ForeColor = Color.Green;  // Connected
toolStripStatusLabel2.ForeColor = Color.Red;    // Disconnected
```

## Best Practices

### ✅ DO

1. **Always use InvokeRequired for UI updates**
   ```csharp
   if (InvokeRequired)
   {
       Invoke(new Action(() => PLCKey_PropertyChanged(sender, e)));
       return;
   }
   ```

2. **Check status string properly**
   ```csharp
   if (status.Contains("connected"))  // ✅ Flexible
   // instead of
   if (status == "connected")  // ❌ Might fail if string is "Tcpstatus.connected"
   ```

3. **Add console logging for debugging**
   ```csharp
   Console.WriteLine($"[Form1] PLC status changed to: {status}");
   ```

4. **Handle both connected and disconnected**
   ```csharp
   if (status.Contains("connected")) { /* ... */ }
   else if (status.Contains("disconnected")) { /* ... */ }
   ```

### ❌ DON'T

1. **Don't forget to subscribe**
   ```csharp
   PLCKey = new PLCKeyence("192.168.0.10", 8501);
   // ❌ Missing: PLCKey.PropertyChanged += PLCKey_PropertyChanged;
   ```

2. **Don't update UI without InvokeRequired check**
   ```csharp
   // ❌ Wrong - might cause cross-thread exception
   toolStripStatusLabel2.Text = "Connected";
   ```

3. **Don't subscribe multiple times**
   ```csharp
   // ❌ Wrong - will call handler multiple times
   PLCKey.PropertyChanged += PLCKey_PropertyChanged;
   PLCKey.PropertyChanged += PLCKey_PropertyChanged;  // Duplicate!
   ```

4. **Don't do heavy operations in event handler**
   ```csharp
   // ❌ Wrong - blocks UI thread
   private void PLCKey_PropertyChanged(...)
   {
       Thread.Sleep(5000);  // Don't do this!
       // ... heavy computation ...
   }
   ```

## Summary

### PropertyChanged Event Flow

```
PLC Connection Change
        │
        ▼
KeyenceHostLinkTcpClient
        │
        ▼
PropertyChangedEvent($"{Tcpstatus.connected/disconnected}")
        │
        ▼
PropertyChanged event raised
        │
        ▼
Form1.PLCKey_PropertyChanged() called
        │
        ▼
InvokeRequired check
        │
        ▼
Marshal to UI thread (if needed)
        │
        ▼
Update toolStripStatusLabel2
Update toolStripProgressBar1
        │
        ▼
User sees connection status
```

### Key Points

1. ✅ **Subscribe**: `PLCKey.PropertyChanged += PLCKey_PropertyChanged;`
2. ✅ **Thread-safe**: Use `InvokeRequired` and `Invoke()`
3. ✅ **Check status**: `e.PropertyName.Contains("connected")`
4. ✅ **Update UI**: Status label, progress bar, colors

Version: PLCKeygen 2025.11.8
