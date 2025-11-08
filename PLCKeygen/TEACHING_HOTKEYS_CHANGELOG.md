# Teaching Hotkeys - Changelog

## Version 1.4 (2025-11-06) - PLC Auto-Reconnect & Status Bar

### 🔌 PLC Connection Management
- **Auto-Reconnect**: Tự động kết nối lại khi mất kết nối với PLC
  - Reconnect timer với interval 3 giây
  - Subscribe to PLCKeyence PropertyChanged events
  - Retry logic với error handling

- **Status Bar Integration**: Hiển thị trạng thái kết nối real-time
  - `toolStripProgressBar1`: Hiển thị marquee khi đang kết nối
  - `toolStripStatusLabel2`: Hiển thị trạng thái với màu sắc
    - 🟢 Xanh: "PLC: Đã kết nối (192.168.0.10:8501)"
    - 🟠 Cam: "PLC: Đang kết nối..."
    - 🔴 Đỏ: "PLC: Mất kết nối - Đang thử kết nối lại..."
    - ⚪ Xám: "PLC: Đã ngắt kết nối"

### 🎯 Features
- Tự động kết nối khi khởi động application
- Tự động reconnect khi mất kết nối (mỗi 3 giây)
- Visual feedback rõ ràng với progress bar và status label
- Thread-safe UI updates với InvokeRequired
- Manual disconnect support (dừng auto-reconnect)
- **Connection Verification**: Đọc thử DM register để verify kết nối thực sự
- **⚡ Fast Connection Timeout**: 2 giây timeout (thay vì 20-30 giây mặc định)
- **⚡ Response Time Monitoring**: Theo dõi tốc độ response từ PLC
- **⚡ Auto-Disconnect On Slow Response**: Tự động ngắt khi PLC response chậm

### 🐛 Bug Fixes
- Fixed: Progress bar hiển thị ngay khi khởi động (thay vì ẩn)
- Fixed: Status bar báo "Đã kết nối" khi chưa cắm PLC
  - Thêm verification bằng ReadUInt16("DM0")
  - Kiểm tra IsSessionStarted trước khi báo success
- Fixed: `IsSessionStarted` được set true trước khi connection thành công
  - Di chuyển `IsSessionStarted = true` xuống sau `GetStream()`
  - Set `IsSessionStarted = false` trong catch block
- Fixed: Connection timeout quá lâu (20-30s)
  - Thêm connection timeout: 2 giây
  - Thêm read/write timeout: 1 giây
- Fixed: PLC response chậm không được phát hiện
  - Monitor response time với Stopwatch
  - Disconnect sau 3 slow responses liên tiếp

### ⚙️ Timeout Configuration
- **CONNECTION_TIMEOUT_MS**: 2000ms (2 giây) - Timeout khi kết nối
- **READ_WRITE_TIMEOUT_MS**: 1000ms (1 giây) - Timeout khi đọc/ghi
- **SLOW_RESPONSE_THRESHOLD_MS**: 500ms - Coi là chậm nếu > 500ms
- **MAX_SLOW_RESPONSES**: 3 - Disconnect sau 3 lần chậm

### 📁 Files Modified
1. ✅ `Form1.cs` - PLC Connection Management region
   - `InitializePLCConnection()`: Show progress bar khi khởi động
   - `PLCKey_PropertyChanged()`: Handler cho status changes
   - `UpdatePLCConnectionStatus()`: Cập nhật status bar
   - `ConnectToPLC()`: Thêm verification với ReadUInt16("DM0")
   - `DisconnectPLC()`: Manual disconnect
   - Constants: PLC_IP, PLC_PORT, RECONNECT_INTERVAL

2. ✅ `KeyenceHostLinkTcpClient.cs` - Connection reliability & performance
   - **Timeout Settings**: CONNECTION_TIMEOUT_MS, READ_WRITE_TIMEOUT_MS, SLOW_RESPONSE_THRESHOLD_MS
   - **Response Monitoring**: _responseTimer, _slowResponseCount, MAX_SLOW_RESPONSES
   - `Open()`:
     - BeginConnect/EndConnect với timeout
     - Set stream read/write timeout
     - Reset slow response counter
   - `SendCommand()`:
     - Measure response time với Stopwatch
     - Track slow responses và auto-disconnect
     - Improved exception handling (IOException, SocketException)

---

## Version 1.3 (2025-11-06) - Single Instance Control

### 🔒 Single Instance Protection
- **Chỉ cho phép chạy 1 instance**: Ngăn chặn mở nhiều phần mềm cùng lúc
  - Sử dụng Mutex để kiểm tra instance đang chạy
  - Hiển thị thông báo khi cố mở phần mềm lần thứ 2
  - Tự động focus vào cửa sổ đang mở
  - Restore window nếu đang minimize

### 🎯 User Experience
- MessageBox thông báo rõ ràng: "PLCKeygen đã đang chạy!"
- Tự động đưa cửa sổ đang chạy lên foreground
- Tránh nhầm lẫn khi chạy nhiều instance

### 📁 Files Modified
1. ✅ `Program.cs` - Thêm single instance check với Mutex
   - Win32 API: SetForegroundWindow, ShowWindow, IsIconic
   - BringExistingInstanceToFront() method

---

## Version 1.2 (2025-11-06) - Model Management

### 🗂️ Model Management System
- **Lưu/Load Teaching Models**: Quản lý teaching points cho nhiều model khác nhau
  - ComboBox `cbbModel`: Chọn model để load
  - Button "Add": Lưu tất cả teaching points hiện tại vào model mới
  - Button "Del": Xóa model đã chọn
  - Storage: JSON file tại `TeachingModels/teaching_models.json`

### 📚 Technical Implementation
- **TeachingModel.cs**: Data structures (TeachingPoint, PortTeachingPoints, TeachingModel)
- **ModelManager.cs**: JSON serialization và file management
- **Form1.cs**: UI controls và PLC integration
  - `ReadAllTeachingPointsFromPLC()`: Đọc 72 teaching points từ PLC (4 ports × 18 points)
  - `WriteAllTeachingPointsToPLC()`: Ghi teaching points vào PLC
  - `UpdateTeachingButtonColors()`: Cập nhật màu button sau khi load

### 🎯 Features
- Lưu tất cả teaching points (4 ports) vào 1 model
- Load model và ghi vào PLC với confirmation dialog
- Auto-refresh ComboBox sau khi thêm/xóa model
- Validation: Kiểm tra tên model trùng lặp

### 📁 Files Modified/Added
1. ✅ `TeachingModel.cs` - Data model classes (NEW)
2. ✅ `ModelManager.cs` - JSON manager (NEW)
3. ✅ `Form1.cs` - Model management UI và logic
4. ✅ `PLCKeygen.csproj` - Thêm new files

---

## Version 1.1 (2025-11-06)

### 🔒 Security Improvements
- **Ẩn mật khẩu khi nhập**: Thay thế `Interaction.InputBox` bằng custom `PasswordDialog`
  - Mật khẩu hiển thị dưới dạng ký tự `●` khi nhập
  - Không thể nhìn thấy mật khẩu khi typing
  - Hỗ trợ nút Cancel để thoát mà không báo lỗi
  - File: `PasswordDialog.cs` (mới)

### 📚 Documentation Updates
- **Thêm phím tắt Jog vào hướng dẫn**:
  - Q: Jog Plus (giữ để di chuyển +)
  - A: Jog Minus (giữ để di chuyển -)
  - X, Y, Z, I, O, F: Chọn trục
  - Space: Toggle Jog/Step mode
  - Ctrl+H: Hiển thị Help

### 📁 Files Modified
1. ✅ `PasswordDialog.cs` - Custom password dialog (NEW)
2. ✅ `Form1.cs` - Sử dụng PasswordDialog thay vì InputBox
3. ✅ `TeachingHotkeyHelp.cs` - Thêm section "PHÍM TẮT JOG & ĐIỀU KHIỂN"
4. ✅ `TEACHING_HOTKEYS_README.md` - Thêm bảng "Điều Khiển Cơ Bản"
5. ✅ `TEACHING_HOTKEYS_QUICK_REF.txt` - Cập nhật quick reference
6. ✅ `PLCKeygen.csproj` - Thêm PasswordDialog.cs

---

## Version 1.0 (2025-11-05) - Initial Release

### ✨ New Features
- **Hệ thống phím tắt Teaching Mode**:
  - 18 teaching points × 2 actions (Save + Go) = 36 hotkeys
  - Tránh xung đột với F1-F4 (chọn Port)
  - Pattern dễ nhớ: Ctrl/Alt+số cho Tray, F5-F12 cho Socket/Camera

### ⌨️ Hotkey Mapping

#### Save Teaching Points
- **Ctrl+1-4**: Tray Input
- **Ctrl+Alt+1-4**: Tray NG1
- **Alt+1-4**: Tray NG2
- **F5-F10**: Socket (6 points)
- **F11-F12**: Camera (2 points)

#### Go to Teaching Points
- **Ctrl+Shift+1-4**: Go to Tray Input
- **Ctrl+Alt+Shift+1-4**: Go to Tray NG1
- **Alt+Shift+1-4**: Go to Tray NG2
- **Shift+F5-F10**: Go to Socket
- **Shift+F11-F12**: Go to Camera

### 🎨 Visual Feedback
- Button nhấp nháy màu vàng khi thực hiện
- Button Save chuyển màu xanh sau khi lưu
- Notification trên title bar (2 giây)

### 📖 Documentation
- `TEACHING_HOTKEYS_README.md` - Chi tiết đầy đủ
- `TEACHING_HOTKEYS_QUICK_REF.txt` - Quick reference card
- Help Form (Ctrl+H) - Hiển thị trong app

### 🔧 Technical Details
- **TeachingPointHotkey.cs**: Class định nghĩa hotkey và manager
- **TeachingHotkeyHelp.cs**: Form hiển thị help
- **Form1.cs**: Integration với existing code
  - ExecuteTeachingHotkey()
  - FindButtonByName()
  - FlashButton()
  - ShowHotkeyNotification()

### 🎯 Benefits
- Tăng tốc độ teaching (không cần click chuột)
- Không cần nhìn màn hình khi teaching
- Pattern logic dễ nhớ
- Multi-port support (4 ports)
- Error handling đầy đủ

---

## Design Decisions

### Tại sao dùng Ctrl+số thay vì F1-F4?
- F1-F4 đã được dùng để chọn Port (cả Jog và Teaching Mode)
- Ctrl+số tránh xung đột và vẫn dễ nhấn
- Pattern nhất quán: Ctrl = Input, Alt = NG2, Ctrl+Alt = NG1

### Tại sao dùng PasswordChar '●'?
- Ẩn mật khẩu khi nhập (bảo mật)
- Dễ nhìn hơn ký tự `*`
- Unicode character đẹp hơn

### Tại sao tạo PasswordDialog riêng?
- `Interaction.InputBox` không hỗ trợ PasswordChar
- Custom dialog cho phép control tốt hơn
- Hỗ trợ Cancel mà không báo lỗi
- UI/UX tốt hơn

---

## Future Enhancements (Optional)

### Có thể thêm:
1. ✨ Button "Help" trên form để dễ access
2. 📊 Status bar thay vì title bar notification
3. 🔊 Sound effect khi save thành công
4. ⚙️ Cho phép customize hotkeys
5. 💾 Export/Import teaching points ra file
6. 🌍 Multi-language support
7. 🎨 Theme customization
8. 📝 Teaching point notes/comments

---

## Known Issues
Không có issue nào được báo cáo.

---

## Compatibility
- .NET Framework 4.7.2+
- Windows Forms
- PLC: Keyence (Host Link protocol)
- Tested on: Windows 10/11

---

## Credits
- Developer: Claude Code Assistant
- Version: PLCKeygen 2025.11.2+
- Date: November 2025

---

## License
Internal use only. Proprietary software.
