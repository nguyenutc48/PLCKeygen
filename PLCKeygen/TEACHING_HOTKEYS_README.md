# Hướng Dẫn Sử Dụng Phím Tắt Teaching Mode

## Tổng Quan

Hệ thống phím tắt cho phép bạn lưu (Save) và di chuyển đến (Go) các điểm teaching một cách nhanh chóng mà không cần nhìn màn hình hoặc click chuột.

## Kích Hoạt Teaching Mode

1. Chuyển sang tab **Motion**
2. Chọn **Port** muốn teaching (Port 1-4)
3. Chọn radio button **Teaching Mode**
4. Nhập mật khẩu: `1234`
5. Các nhóm teaching (Tray và Socket) sẽ được kích hoạt

## Bảng Phím Tắt

### LƯU ĐIỂM TEACHING (SAVE)

Các phím tắt lưu vị trí hiện tại của robot vào điểm teaching:

#### Tray Input (OK)
| Phím | Chức năng |
|------|-----------|
| **Ctrl+1** | Lưu Tray Input - XY Start |
| **Ctrl+2** | Lưu Tray Input - X End |
| **Ctrl+3** | Lưu Tray Input - Y End |
| **Ctrl+4** | Lưu Tray Input - Z Position |

#### Tray NG1
| Phím | Chức năng |
|------|-----------|
| **Ctrl+Alt+1** | Lưu Tray NG1 - XY Start |
| **Ctrl+Alt+2** | Lưu Tray NG1 - X End |
| **Ctrl+Alt+3** | Lưu Tray NG1 - Y End |
| **Ctrl+Alt+4** | Lưu Tray NG1 - Z Position |

#### Tray NG2
| Phím | Chức năng |
|------|-----------|
| **Alt+1** | Lưu Tray NG2 - XY Start |
| **Alt+2** | Lưu Tray NG2 - X End |
| **Alt+3** | Lưu Tray NG2 - Y End |
| **Alt+4** | Lưu Tray NG2 - Z Position |

#### Socket
| Phím | Chức năng |
|------|-----------|
| **F5** | Lưu Socket - XY Position |
| **F6** | Lưu Socket - Z Load |
| **F7** | Lưu Socket - Z Unload |
| **F8** | Lưu Socket - Z Ready |
| **F9** | Lưu Socket - F Opened |
| **F10** | Lưu Socket - F Closed |

#### Camera
| Phím | Chức năng |
|------|-----------|
| **F11** | Lưu Camera - XY Position |
| **F12** | Lưu Camera - Z Position |

---

### DI CHUYỂN ĐẾN ĐIỂM TEACHING (GO)

Các phím tắt di chuyển robot đến điểm teaching đã lưu:

#### Tray Input (OK)
| Phím | Chức năng |
|------|-----------|
| **Ctrl+Shift+1** | Di chuyển đến Tray Input - XY Start |
| **Ctrl+Shift+2** | Di chuyển đến Tray Input - X End |
| **Ctrl+Shift+3** | Di chuyển đến Tray Input - Y End |
| **Ctrl+Shift+4** | Di chuyển đến Tray Input - Z Position |

#### Tray NG1
| Phím | Chức năng |
|------|-----------|
| **Ctrl+Alt+Shift+1** | Di chuyển đến Tray NG1 - XY Start |
| **Ctrl+Alt+Shift+2** | Di chuyển đến Tray NG1 - X End |
| **Ctrl+Alt+Shift+3** | Di chuyển đến Tray NG1 - Y End |
| **Ctrl+Alt+Shift+4** | Di chuyển đến Tray NG1 - Z Position |

#### Tray NG2
| Phím | Chức năng |
|------|-----------|
| **Alt+Shift+1** | Di chuyển đến Tray NG2 - XY Start |
| **Alt+Shift+2** | Di chuyển đến Tray NG2 - X End |
| **Alt+Shift+3** | Di chuyển đến Tray NG2 - Y End |
| **Alt+Shift+4** | Di chuyển đến Tray NG2 - Z Position |

#### Socket
| Phím | Chức năng |
|------|-----------|
| **Shift+F5** | Di chuyển đến Socket - XY |
| **Shift+F6** | Di chuyển đến Socket - Z Load |
| **Shift+F7** | Di chuyển đến Socket - Z Unload |
| **Shift+F8** | Di chuyển đến Socket - Z Ready |
| **Shift+F9** | Di chuyển đến Socket - F Opened |
| **Shift+F10** | Di chuyển đến Socket - F Closed |

#### Camera
| Phím | Chức năng |
|------|-----------|
| **Shift+F11** | Di chuyển đến Camera - XY |
| **Shift+F12** | Di chuyển đến Camera - Z |

---

## Phím Tắt Khác

### Điều Khiển Cơ Bản
| Phím | Chức năng |
|------|-----------|
| **F1-F4** | Chọn Port 1-4 |
| **Q** | Jog Plus (giữ để di chuyển +) |
| **A** | Jog Minus (giữ để di chuyển -) |
| **X** | Chọn trục X |
| **Y** | Chọn trục Y |
| **Z** | Chọn trục Z |
| **I** | Chọn trục RI (Rotation Inner) |
| **O** | Chọn trục RO (Rotation Outer) |
| **F** | Chọn trục F (Focus) |
| **Space** | Chuyển đổi Jog/Step mode |
| **Ctrl+H** | Hiển thị hướng dẫn phím tắt (Help) |

---

## Quy Trình Teaching Thông Thường

### Bước 1: Lưu Điểm Teaching
1. Sử dụng phím **Q/A** để Jog robot đến vị trí mong muốn
2. Nhấn phím tắt **Save** tương ứng (VD: **F1** cho Tray Input XY Start)
3. Button Save sẽ chuyển sang màu **xanh lá** để xác nhận đã lưu
4. Tiêu đề window sẽ hiển thị thông báo trong 2 giây

### Bước 2: Kiểm Tra Điểm Teaching
1. Di chuyển robot ra khỏi vị trí đã teaching
2. Nhấn phím tắt **Go** tương ứng (VD: **Shift+F1**)
3. Robot sẽ tự động di chuyển đến điểm đã lưu
4. Kiểm tra độ chính xác của vị trí

### Bước 3: Điều Chỉnh (Nếu Cần)
1. Nếu vị trí chưa chính xác, dùng Jog để điều chỉnh
2. Nhấn lại phím **Save** để cập nhật vị trí mới

---

## Visual Feedback

### Hiệu Ứng Khi Nhấn Phím Tắt
- **Màu vàng nhấp nháy**: Button tương ứng sẽ nhấp nháy màu vàng trong 150ms
- **Màu xanh lá cây**: Button Save sẽ giữ màu xanh sau khi lưu thành công
- **Thông báo title bar**: Thông tin về hành động sẽ hiển thị trên thanh tiêu đề trong 2 giây

### Màu Button
- **Xám (mặc định)**: Chưa lưu teaching point
- **Xanh lá**: Đã lưu teaching point
- **Vàng nhấp nháy**: Đang thực hiện hành động

---

## Lưu Ý Quan Trọng

1. **Phím tắt chỉ hoạt động khi ở chế độ Teaching Mode**
   - F1-F4 dùng để chọn Port cả ở Jog Mode và Teaching Mode
   - Teaching points dùng Ctrl/Alt+số để tránh xung đột với việc chọn Port

2. **Port hiện tại ảnh hưởng đến teaching point**
   - Mỗi Port có bộ teaching points riêng
   - Chuyển Port sẽ load các teaching points của Port đó

3. **Reset màu button khi thoát Teaching Mode**
   - Tất cả button Save sẽ về màu xám khi chuyển sang Jog Mode
   - Dữ liệu teaching vẫn được lưu trong PLC

4. **Phím Jog vẫn hoạt động trong Teaching Mode**
   - Q: Jog Plus
   - A: Jog Minus
   - Space: Chuyển đổi Jog/Step mode
   - X, Y, Z, I, O, F: Chọn trục

---

## Troubleshooting

### Phím tắt không hoạt động?
- ✓ Kiểm tra đã chuyển sang Teaching Mode chưa
- ✓ Kiểm tra đã nhập đúng mật khẩu chưa (1234)
- ✓ Kiểm tra các nhóm Teaching có được enable không

### Button không đổi màu?
- ✓ Kiểm tra kết nối PLC
- ✓ Kiểm tra log/error messages
- ✓ Thử Save lại

### Robot không di chuyển khi Go?
- ✓ Kiểm tra đã Save teaching point chưa
- ✓ Kiểm tra PLC connection
- ✓ Kiểm tra điểm teaching có hợp lệ không

---

## Technical Details

### Files Added
- `TeachingPointHotkey.cs`: Định nghĩa class cho hotkey và manager
- `TeachingHotkeyHelp.cs`: Form hiển thị help
- `TEACHING_HOTKEYS_README.md`: Tài liệu hướng dẫn

### Code Changes in Form1.cs
- Added `TeachingHotkeyManager hotkeyManager` field
- Modified `Form1_KeyDown()` to handle teaching hotkeys
- Added methods:
  - `ExecuteTeachingHotkey()`
  - `FindButtonByName()`
  - `FindButtonInControl()`
  - `FlashButton()`
  - `ShowHotkeyNotification()`
  - `ShowTeachingHotkeyHelp()`
  - `CheckHotkeyHelpShortcut()`

### PLC Addresses Used
Teaching points được lưu vào các địa chỉ PLC được định nghĩa trong `PLCAddresses.Generated.cs`:
- Port 1: DM1800-1910
- Port 2: DM1000-1110
- Port 3: DM2200-2310
- Port 4: DM1400-1510

---

## Changelog

### Version 1.0 (2025-11-05)
- ✓ Triển khai hệ thống phím tắt cho 18 teaching points
- ✓ Hỗ trợ Save và Go với function keys + modifiers
- ✓ Visual feedback (button flash, color change)
- ✓ Help form với Ctrl+H
- ✓ Notification trên title bar
- ✓ Tương thích với 4 Ports

---

## Contact & Support

Nếu có vấn đề hoặc câu hỏi, vui lòng liên hệ team phát triển.

Version: 2025.11.2+
