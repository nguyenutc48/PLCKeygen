# Teaching Hotkeys - Changelog

## Version 1.1 (2025-11-06) - Latest Update

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
