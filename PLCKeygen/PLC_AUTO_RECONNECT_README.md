# PLC Auto-Reconnect & Status Bar - Hướng Dẫn

## Tổng Quan

PLCKeygen được tích hợp tính năng **tự động kết nối lại** (auto-reconnect) với PLC Keyence và **hiển thị trạng thái kết nối** real-time trên thanh trạng thái (status bar).

Tính năng này đảm bảo:
- ✅ Tự động kết nối khi khởi động ứng dụng
- ✅ Tự động kết nối lại khi mất kết nối
- ✅ Hiển thị trạng thái kết nối rõ ràng cho operator
- ✅ Visual feedback với progress bar và màu sắc

## Vị Trí Trên Giao Diện

### Status Bar (Thanh Trạng Thái)

Nằm ở **góc dưới cùng** của cửa sổ:

```
┌────────────────────────────────────────────────────────┐
│                                                        │
│                   Main Application                     │
│                                                        │
└────────────────────────────────────────────────────────┘
│ [Progress Bar] PLC: Đã kết nối (192.168.0.10:8501)    │
└────────────────────────────────────────────────────────┘
   ↑                ↑
   │                └── toolStripStatusLabel2 (Text + Color)
   └── toolStripProgressBar1 (Marquee animation)
```

### Các Thành Phần

1. **toolStripProgressBar1**: Progress bar dạng marquee (chạy qua lại)
   - Hiển thị: Khi đang kết nối
   - Ẩn: Khi đã kết nối hoặc ngắt kết nối

2. **toolStripStatusLabel2**: Label hiển thị text và màu sắc
   - 🟢 **Xanh lá**: PLC đã kết nối thành công
   - 🟠 **Cam**: Đang kết nối...
   - 🔴 **Đỏ**: Mất kết nối, đang thử lại
   - ⚪ **Xám**: Đã ngắt kết nối (manual)

## Cách Hoạt Động

### 1. Khi Khởi Động Application

```
┌─────────────────────────────────────┐
│  PLCKeygen.exe starts               │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  Form1 Constructor                  │
│  - InitializePLCConnection()        │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  Create PLCKeyence instance         │
│  - IP: 192.168.0.10                 │
│  - Port: 8501                       │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  Subscribe to PropertyChanged       │
│  PLCKey.PropertyChanged += ...      │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  Create Timer                       │
│  - Interval: 3000ms (3 seconds)     │
│  - Tick event: PlcReconnectTimer    │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  ConnectToPLC()                     │
│  - Show "Đang kết nối..."           │
│  - PLCKey.Open()                    │
│  - PLCKey.StartCommunication()      │
└────────────┬────────────────────────┘
             │
      ┌──────┴──────┐
      │             │
      ▼             ▼
┌──────────┐   ┌──────────────┐
│ Success  │   │   Failed     │
└────┬─────┘   └─────┬────────┘
     │               │
     ▼               ▼
┌──────────┐   ┌──────────────┐
│ 🟢 Green │   │ 🔴 Red       │
│ Connected│   │ Start Timer  │
└──────────┘   └──────────────┘
```

### 2. Khi Mất Kết Nối (Tự Động)

```
┌─────────────────────────────────────┐
│  PLC Connection Lost                │
│  (Cable unplugged, PLC powered off) │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  PLCKeyence raises PropertyChanged  │
│  - Property: "disconnected"         │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  PLCKey_PropertyChanged handler     │
│  - Detects "disconnected"           │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  UpdatePLCConnectionStatus(false)   │
│  - Show red text                    │
│  - Show progress bar                │
│  - Text: "Mất kết nối..."           │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  Start Reconnect Timer              │
│  - Tick every 3 seconds             │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  Every 3 seconds:                   │
│  PlcReconnectTimer_Tick()           │
│  → ConnectToPLC()                   │
└────────────┬────────────────────────┘
             │
      ┌──────┴──────┐
      │             │
      ▼             ▼
┌──────────┐   ┌──────────────┐
│ Success  │   │   Failed     │
│ 🟢 Green │   │ 🔴 Try again │
│ Stop     │   │ in 3 sec     │
│ Timer    │   │              │
└──────────┘   └──────────────┘
```

### 3. Khi Kết Nối Lại Thành Công

```
┌─────────────────────────────────────┐
│  ConnectToPLC() succeeds            │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  PLCKeyence raises PropertyChanged  │
│  - Property: "connected"            │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│  UpdatePLCConnectionStatus(true)    │
│  - Hide progress bar                │
│  - Show green text                  │
│  - Stop reconnect timer             │
│  - Text: "PLC: Đã kết nối (...)"    │
└─────────────────────────────────────┘
```

## Trạng Thái Kết Nối

### 🟢 Đã Kết Nối (Connected)

**Text**: `PLC: Đã kết nối (192.168.0.10:8501)`
**Màu**: Green
**Progress Bar**: Hidden
**Timer**: Stopped

**Ý nghĩa**:
- Kết nối PLC thành công
- Application có thể điều khiển PLC bình thường
- Có thể Jog, Go, Teaching, etc.

### 🟠 Đang Kết Nối (Connecting)

**Text**: `PLC: Đang kết nối...`
**Màu**: Orange
**Progress Bar**: Visible (Marquee)
**Timer**: N/A

**Ý nghĩa**:
- Đang trong quá trình kết nối
- Chờ response từ PLC
- Thường xuất hiện khi:
  - Khởi động application
  - Đang retry sau khi mất kết nối

### 🔴 Mất Kết Nối (Disconnected - Auto Retry)

**Text**: `PLC: Mất kết nối - Đang thử kết nối lại...`
**Màu**: Red
**Progress Bar**: Visible (Marquee)
**Timer**: Running (every 3s)

**Ý nghĩa**:
- Mất kết nối với PLC
- Hệ thống đang tự động retry
- Application không thể điều khiển PLC
- Cần kiểm tra:
  - ✓ Cáp mạng
  - ✓ PLC có nguồn không
  - ✓ IP address đúng không
  - ✓ Firewall

### ⚪ Đã Ngắt Kết Nối (Manually Disconnected)

**Text**: `PLC: Đã ngắt kết nối`
**Màu**: Gray
**Progress Bar**: Hidden
**Timer**: Stopped

**Ý nghĩa**:
- Ngắt kết nối thủ công (nếu có chức năng)
- Auto-reconnect đã bị disable
- Cần kết nối lại thủ công

## Cấu Hình

### Constants (Form1.cs)

```csharp
private const int RECONNECT_INTERVAL = 3000; // 3 seconds
private const string PLC_IP = "192.168.0.10";
private const int PLC_PORT = 8501;
```

### Thay Đổi IP/Port

Nếu muốn thay đổi IP hoặc Port của PLC:

1. Mở file `Form1.cs`
2. Tìm dòng:
   ```csharp
   private const string PLC_IP = "192.168.0.10";
   private const int PLC_PORT = 8501;
   ```
3. Sửa thành IP/Port mong muốn
4. Rebuild application

### Thay Đổi Reconnect Interval

Nếu muốn thay đổi thời gian retry (mặc định 3 giây):

1. Mở file `Form1.cs`
2. Tìm dòng:
   ```csharp
   private const int RECONNECT_INTERVAL = 3000; // 3 seconds
   ```
3. Sửa thành giá trị mong muốn (milliseconds)
   - 1000 = 1 giây
   - 5000 = 5 giây
   - 10000 = 10 giây
4. Rebuild application

## Technical Details

### Thread Safety

Tất cả UI updates đều được kiểm tra thread-safe:

```csharp
if (InvokeRequired)
{
    Invoke(new Action(() => PLCKey_PropertyChanged(sender, e)));
    return;
}
```

Điều này đảm bảo:
- PropertyChanged events từ background thread được marshal về UI thread
- Không có cross-thread exceptions
- UI updates an toàn

### Event Flow

```
PLCKeyence
    │
    │ PropertyChanged event
    │ (from background thread)
    ▼
PLCKey_PropertyChanged
    │
    │ InvokeRequired check
    │ → Invoke to UI thread
    ▼
UpdatePLCConnectionStatus
    │
    │ Update UI controls
    ▼
Status Bar + Progress Bar updated
```

### Timer vs PLCKeyence Internal Timer

**Form1 Reconnect Timer**:
- UI-level timer (`System.Windows.Forms.Timer`)
- Runs on UI thread
- Handles UI updates
- 3-second interval

**PLCKeyence Internal Timer**:
- From `KeyenceHostLinkTcpClient.cs`
- Background timer (`System.Timers.Timer`)
- Handles TCP reconnection
- Also 3-second interval

Both work together để đảm bảo:
- PLC connection được retry
- UI được update kịp thời

## Methods

### InitializePLCConnection()

**Mục đích**: Khởi tạo PLC connection và setup auto-reconnect

**Called**: Constructor của Form1

**Thực hiện**:
1. Initialize status bar (progress bar + label)
2. Create PLCKeyence instance
3. Subscribe to PropertyChanged event
4. Create reconnect timer
5. Call ConnectToPLC() lần đầu

### PLCKey_PropertyChanged()

**Mục đích**: Handle connection status changes từ PLCKeyence

**Triggered**: Khi PLCKeyence raises PropertyChanged event

**Parameters**:
- `e.PropertyName`: "connected" hoặc "disconnected"

**Thread Safety**: ✅ Uses InvokeRequired

### UpdatePLCConnectionStatus()

**Mục đích**: Update UI controls với connection status

**Parameters**:
- `isConnected` (bool): true = connected, false = disconnected

**UI Changes**:
- Progress bar visibility
- Status label text và color
- Timer start/stop

### ConnectToPLC()

**Mục đích**: Thực hiện connection đến PLC

**Called by**:
- InitializePLCConnection() (lần đầu)
- PlcReconnectTimer_Tick() (retry)

**Error Handling**: ✅ Try-catch với fallback

### DisconnectPLC()

**Mục đích**: Ngắt kết nối thủ công (manual disconnect)

**Actions**:
1. Set `isManualDisconnect = true`
2. Stop reconnect timer
3. Close PLC connection
4. Update status bar to gray

## Troubleshooting

### Progress Bar Luôn Chạy (Không Kết Nối Được)

**Nguyên nhân**:
- PLC không có nguồn
- Cáp mạng chưa cắm
- IP/Port sai
- Firewall chặn port 8501

**Giải pháp**:
1. Kiểm tra PLC có nguồn
2. Kiểm tra cáp mạng (LED nhấp nháy?)
3. Ping PLC: `ping 192.168.0.10`
4. Telnet test: `telnet 192.168.0.10 8501`
5. Tắt firewall tạm thời để test

### Status Bar Không Cập Nhật

**Nguyên nhân**:
- PropertyChanged event không được raise
- Thread issue

**Giải pháp**:
1. Check Debug output window trong Visual Studio
2. Xem có exception không
3. Kiểm tra `PLCKey.PropertyChanged += ...` đã subscribe chưa

### Kết Nối Thành Công Nhưng Status Bar Vẫn Đỏ

**Nguyên nhân**:
- PropertyChanged event không được raise đúng
- String parsing issue ("connected" vs "disconnected")

**Giải pháp**:
1. Kiểm tra `KeyenceHostLinkTcpClient.cs`
2. Đảm bảo `PropertyChangedEvent($"{Tcpstatus.connected}")` được gọi
3. Check enum `Tcpstatus` có đúng không

### Auto-Reconnect Không Hoạt Động

**Nguyên nhân**:
- Timer không start
- `isManualDisconnect` = true

**Giải pháp**:
1. Kiểm tra timer có được khởi tạo không
2. Set breakpoint ở `PlcReconnectTimer_Tick`
3. Reset `isManualDisconnect = false`

## Best Practices

### ✅ Khuyến Nghị

1. **Không tắt Auto-Reconnect**
   - Để hệ thống tự động kết nối lại
   - Tiết kiệm thời gian troubleshooting

2. **Quan sát Status Bar**
   - Luôn kiểm tra trạng thái trước khi vận hành
   - Nếu đỏ → không điều khiển được PLC

3. **Kiểm tra kết nối định kỳ**
   - Test Jog vài trục trước khi chạy auto
   - Đảm bảo PLC response

4. **Ghi chú lại IP/Port**
   - Document lại nếu có thay đổi
   - Tránh mất thông tin cấu hình

### ⚠️ Lưu Ý

1. **Reconnect mỗi 3 giây**
   - Không spam quá nhiều
   - Tránh làm chậm hệ thống

2. **Thread Safety**
   - Không gọi PLC commands từ UI thread
   - Dùng async/await nếu cần

3. **Network Latency**
   - Connection có thể mất vài giây
   - Không panic nếu thấy orange/red trong lúc khởi động

## Summary

Tính năng PLC Auto-Reconnect đảm bảo:
- ✅ Tự động kết nối khi khởi động
- ✅ Tự động retry khi mất kết nối (3s interval)
- ✅ Visual feedback rõ ràng (progress + colors)
- ✅ Thread-safe UI updates
- ✅ Không cần can thiệp thủ công

**Status Bar Colors**:
- 🟢 Green = Good to go
- 🟠 Orange = Wait a moment
- 🔴 Red = Problem, checking...
- ⚪ Gray = Manual disconnect

Version: PLCKeygen 2025.11.2+
