# Tóm tắt triển khai tính năng Tab Data

## Tổng quan
Đã hoàn thành việc triển khai các tính năng cho tab Data với khả năng:
- Đọc giá trị Speed và Teaching từ PLC lên các TextBox
- Ghi giá trị từ TextBox xuống PLC khi nhấn Enter
- Hỗ trợ chuyển đổi giữa 4 Port khác nhau

## Các file đã tạo/sửa đổi

### 1. DataTabManager.cs (MỚI)
**Mô tả**: Class quản lý việc đọc/ghi dữ liệu Speed và Teaching cho tab Data

**Chức năng chính**:
- `InitializeAddressMaps()`: Khởi tạo mapping giữa tên TextBox và địa chỉ PLC cho cả 4 Port
- `LoadSpeedDataToTextBoxes()`: Đọc tất cả giá trị Speed từ PLC lên TextBox
- `LoadTeachingDataToTextBoxes()`: Đọc tất cả giá trị Teaching từ PLC lên TextBox
- `SaveTextBoxValueToPLC()`: Lưu giá trị từ TextBox xuống PLC
- `RegisterTextBoxEvents()`: Đăng ký event KeyDown cho tất cả TextBox
- `DataTextBox_KeyDown()`: Xử lý sự kiện nhấn Enter để lưu giá trị

**Địa chỉ PLC được map**:

#### Speed Settings (cho mỗi Port 1-4):
- **Unload Tray Speed**: ZReady, XY, Z, XReady, YReady, XYReady, RO
- **Load Tray Speed**: XY, Z, XReady, YReady, XYReady
- **Camera Speed**: XY, RI, XReady, YReady, XYReady, Z
- **Unload Socket Speed**: XY, Z, XReady, YReady, XYReady
- **Load Socket Speed**: XY, Z, XReady, YReady, XYReady
- **Socket Speed**: Close, Open

#### Teaching Data (cho mỗi Port 1-4):
- Tray_Col_Number (Số cột sản phẩm)
- Tray_Row_Number (Số hàng sản phẩm)
- Tray_Row_NG1, NG2, NG3, NG4 (Số hàng chứa NG)
- RORI_Distance_X (Khoảng cách 2 đầu hút)
- Socket_Angle (Góc xoay đầu hút)

### 2. Form1.cs (CẬP NHẬT)
**Các thay đổi**:

#### Thêm fields mới:
```csharp
private DataTabManager dataTabManager;
private int selectedDataPort = 1;
```

#### Khởi tạo trong constructor:
```csharp
// Trong Form1_Load():
dataTabManager = new DataTabManager(PLCKey);
InitializeDataTab();
```

#### Thêm methods mới:
- `InitializeDataTab()`: Khởi tạo tab Data, đăng ký events cho TextBox
- `LoadDataTabValues()`: Load dữ liệu từ PLC cho Port hiện tại
- `DataPortRadioButton_CheckedChanged()`: Xử lý sự kiện thay đổi Port

### 3. PLCKeygen.csproj (CẬP NHẬT)
Đã thêm DataTabManager.cs vào danh sách compile:
```xml
<Compile Include="DataTabManager.cs" />
```

### 4. DATA_TAB_SETUP_GUIDE.md (MỚI)
Hướng dẫn chi tiết cách thêm Radio buttons trong Designer và hoàn thiện tính năng.

## Cách sử dụng

### Bước 1: Thêm Radio Buttons trong Designer (BẮT BUỘC)
Mở Visual Studio Designer và thêm:
1. GroupBox: `grpDataPortSelection` với text "PORT SELECTION"
2. 4 Radio buttons:
   - `rbtDataPort1` (Text: "PORT 1", Checked: true)
   - `rbtDataPort2` (Text: "PORT 2")
   - `rbtDataPort3` (Text: "PORT 3")
   - `rbtDataPort4` (Text: "PORT 4")

### Bước 2: Bỏ comment code trong InitializeDataTab()
Trong file Form1.cs, method InitializeDataTab(), bỏ comment các dòng:
```csharp
rbtDataPort1.CheckedChanged += DataPortRadioButton_CheckedChanged;
rbtDataPort2.CheckedChanged += DataPortRadioButton_CheckedChanged;
rbtDataPort3.CheckedChanged += DataPortRadioButton_CheckedChanged;
rbtDataPort4.CheckedChanged += DataPortRadioButton_CheckedChanged;
rbtDataPort1.Checked = true;
```

### Bước 3: Build và chạy
```bash
Build Solution (Ctrl+Shift+B)
Run (F5)
```

## Cách hoạt động

### 1. Đọc giá trị từ PLC
- Khi mở tab Data hoặc chuyển Port, tự động load giá trị từ PLC
- Method `LoadDataTabValues()` được gọi:
  1. Set port hiện tại trong DataTabManager
  2. Gọi `LoadSpeedDataToTextBoxes()` để load speed
  3. Gọi `LoadTeachingDataToTextBoxes()` để load teaching data
- Duyệt qua tất cả TextBox, tìm các TextBox có name khớp với mapping
- Đọc giá trị từ địa chỉ PLC tương ứng bằng `ReadInt32()`
- Hiển thị giá trị lên TextBox

### 2. Ghi giá trị xuống PLC
- User nhập giá trị mới vào TextBox
- Nhấn Enter
- Event `DataTextBox_KeyDown()` được trigger:
  1. Validate giá trị nhập vào (phải là số)
  2. Gọi `SaveTextBoxValueToPLC()`
  3. Ghi giá trị xuống PLC bằng `WriteInt32()`
  4. Đọc lại giá trị từ PLC để xác nhận
  5. Cập nhật TextBox với giá trị đã đọc lại

### 3. Chuyển đổi Port
- User click vào Radio button Port khác
- Event `DataPortRadioButton_CheckedChanged()` được trigger:
  1. Xác định Port được chọn (1-4)
  2. Update `selectedDataPort`
  3. Gọi `LoadDataTabValues()` để load dữ liệu Port mới

## Ưu điểm của thiết kế

### 1. Tách biệt logic
- DataTabManager độc lập, không phụ thuộc vào Form
- Dễ dàng test và maintain
- Có thể tái sử dụng cho các tab khác

### 2. Tự động mapping
- Không cần viết code riêng cho từng TextBox
- Chỉ cần đặt tên TextBox đúng convention
- Tự động đăng ký events

### 3. Validation và Error handling
- Kiểm tra giá trị nhập vào
- MessageBox thông báo lỗi rõ ràng
- Read back để xác nhận giá trị đã ghi

### 4. Dễ mở rộng
- Muốn thêm TextBox mới:
  1. Thêm TextBox trong Designer
  2. Thêm mapping trong `InitializeAddressMaps()`
  3. Không cần code thêm gì khác

## Kiểm tra kết quả build

Build thành công với:
- **0 Errors**
- **12 Warnings** (warnings này đã có từ trước, không liên quan đến code mới)

```
Build succeeded.
    12 Warning(s)
    0 Error(s)
Time Elapsed 00:00:00.88
```

## Lưu ý quan trọng

### 1. Tên TextBox
Tên TextBox trong Designer phải khớp chính xác với tên trong mapping. Ví dụ:
- `txtUnload_Speed_ZReady` (đúng)
- `txtUnloadSpeedZReady` (sai - không khớp)

### 2. PLC Connection
Đảm bảo PLC đã kết nối trước khi load giá trị.

### 3. Read/Write permissions
Đảm bảo PLC cho phép đọc/ghi vào các địa chỉ DM.

### 4. Data type
Hiện tại chỉ hỗ trợ Int32. Nếu cần kiểu dữ liệu khác (float, string), cần cập nhật code.

## Các bước tiếp theo (tùy chọn)

### 1. Thêm nút Refresh
Thêm button để người dùng có thể refresh dữ liệu từ PLC bất kỳ lúc nào.

### 2. Thêm indicator trạng thái
Hiển thị trạng thái đang đọc/ghi PLC (loading spinner).

### 3. Log lịch sử thay đổi
Ghi log các thay đổi giá trị để audit trail.

### 4. Undo/Redo
Cho phép hoàn tác các thay đổi.

### 5. Batch update
Cho phép update nhiều giá trị cùng lúc.

## Troubleshooting

### Lỗi: TextBox không hoạt động
**Nguyên nhân**: Tên TextBox không khớp với mapping
**Giải pháp**: Kiểm tra tên TextBox trong Designer

### Lỗi: Không đọc được giá trị
**Nguyên nhân**: PLC chưa kết nối hoặc địa chỉ sai
**Giải pháp**:
- Kiểm tra connection PLC
- Kiểm tra địa chỉ trong PLCAddresses.Generated.cs

### Lỗi: Không ghi được giá trị
**Nguyên nhân**: PLC ở chế độ PROGRAM hoặc không có quyền ghi
**Giải pháp**:
- Chuyển PLC sang chế độ RUN
- Kiểm tra quyền write

## Kết luận

Đã hoàn thành triển khai đầy đủ tính năng cho tab Data với:
- ✅ Class DataTabManager quản lý logic
- ✅ Tích hợp vào Form1.cs
- ✅ Mapping đầy đủ cho 4 Port
- ✅ Event handling cho Enter key
- ✅ Build thành công không lỗi
- ✅ Hướng dẫn chi tiết

Chỉ còn cần thêm Radio buttons trong Designer là có thể sử dụng được ngay!
