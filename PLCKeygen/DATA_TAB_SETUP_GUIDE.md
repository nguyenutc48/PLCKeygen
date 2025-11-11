# Hướng dẫn hoàn thiện Tab Data

## Tổng quan
Tab Data đã được code logic xử lý, nhưng cần thêm Radio buttons trong Designer để chọn Port (1-4).

## Tính năng đã hoàn thành

### 1. DataTabManager.cs
- Class quản lý đọc/ghi dữ liệu Speed và Teaching cho từng Port
- Tự động map tên TextBox với địa chỉ PLC tương ứng
- Hỗ trợ nhấn Enter để lưu giá trị xuống PLC và đọc lại để xác nhận

### 2. Form1.cs
- Đã tích hợp DataTabManager
- Đã có methods: InitializeDataTab(), LoadDataTabValues(), DataPortRadioButton_CheckedChanged()
- Event handlers đã được đăng ký cho tất cả TextBox trong tab Data

## Các bước cần thực hiện trong Visual Studio Designer

### Bước 1: Thêm Radio Buttons cho chọn Port

1. Mở Form1.cs trong Designer (double click Form1.cs hoặc click phải -> View Designer)
2. Chọn tab "Data" (tabPage4)
3. Thêm một GroupBox mới:
   - Name: `grpDataPortSelection`
   - Text: `PORT SELECTION`
   - Location: Đặt ở vị trí phù hợp (ví dụ: góc trên bên phải)
   - Size: (200, 60)

4. Thêm 4 Radio buttons vào GroupBox vừa tạo:

   **Radio Button 1:**
   - Name: `rbtDataPort1`
   - Text: `PORT 1`
   - Location: (10, 20)
   - Checked: `true` (mặc định chọn Port 1)

   **Radio Button 2:**
   - Name: `rbtDataPort2`
   - Text: `PORT 2`
   - Location: (60, 20)

   **Radio Button 3:**
   - Name: `rbtDataPort3`
   - Text: `PORT 3`
   - Location: (110, 20)

   **Radio Button 4:**
   - Name: `rbtDataPort4`
   - Text: `PORT 4`
   - Location: (160, 20)

### Bước 2: Kết nối Event Handlers

Sau khi thêm Radio buttons, cần bỏ comment các dòng code trong file Form1.cs, method `InitializeDataTab()`:

```csharp
// Tìm method InitializeDataTab() trong Form1.cs
// Bỏ comment các dòng sau:

rbtDataPort1.CheckedChanged += DataPortRadioButton_CheckedChanged;
rbtDataPort2.CheckedChanged += DataPortRadioButton_CheckedChanged;
rbtDataPort3.CheckedChanged += DataPortRadioButton_CheckedChanged;
rbtDataPort4.CheckedChanged += DataPortRadioButton_CheckedChanged;
rbtDataPort1.Checked = true;  // Set default to Port 1
```

### Bước 3: Build và Test

1. Build project (Ctrl+Shift+B)
2. Chạy ứng dụng (F5)
3. Chuyển sang tab Data
4. Kiểm tra:
   - Các TextBox hiển thị giá trị từ PLC cho Port 1
   - Chuyển sang Port 2, 3, 4 -> Giá trị TextBox thay đổi theo
   - Thay đổi giá trị trong TextBox và nhấn Enter -> Giá trị được lưu xuống PLC và đọc lại

## Cách sử dụng

### Đọc giá trị từ PLC
- Chọn Port muốn xem (1-4) bằng Radio buttons
- Các TextBox sẽ tự động load giá trị từ PLC

### Ghi giá trị xuống PLC
1. Chọn Port muốn cập nhật
2. Click vào TextBox cần thay đổi
3. Nhập giá trị mới (chỉ nhập số)
4. Nhấn Enter
5. Giá trị sẽ được ghi xuống PLC và đọc lại để xác nhận

## Các TextBox được hỗ trợ

### Speed Settings (trong groupBox47 - RUN SPEED)
- **Unload Tray Speed:**
  - txtUnload_Speed_ZReady
  - txtUnload_Speed_XY
  - txtUnload_Speed_Z
  - txtUnload_Speed_XReady
  - txtUnload_Speed_YReady
  - txtUnload_Speed_XYReady
  - txtUnload_Speed_RO

- **Load Tray Speed:**
  - txtLoad_Speed_XY
  - txtLoad_Speed_Z
  - txtLoad_Speed_XReady
  - txtLoad_Speed_YReady
  - txtLoad_Speed_XYReady

- **Camera Speed:**
  - txtCamera_Speed_XY
  - txtCamera_Speed_RI
  - txtCamera_Speed_XReady
  - txtCamera_Speed_YReady
  - txtCamera_Speed_XYReady
  - txtCamera_Speed_Z

- **Unload Socket Speed:**
  - txtUnload_Socket_Speed_XY
  - txtUnload_Socket_Speed_Z
  - txtUnload_Socket_Speed_XReady
  - txtUnload_Socket_Speed_YReady
  - txtUnload_Socket_Speed_XYReady

- **Load Socket Speed:**
  - txtLoad_Socket_Speed_XY
  - txtLoad_Socket_Speed_Z
  - txtLoad_Socket_Speed_XReady
  - txtLoad_Socket_Speed_YReady
  - txtLoad_Socket_Speed_XYReady

- **Socket Speed Close/Open:**
  - txtSocket_Speed_Close
  - txtSocket_Speed_Open

### Teaching Data (trong groupBox39 - DATA TEACHING)
- txtTray_Col_Number (Số cột sản phẩm)
- txtTray_Row_Number (Số hàng sản phẩm)
- txtTray_Row_NG1 (Số hàng chứa NG1)
- txtTray_Row_NG2 (Số hàng chứa NG2)
- txtTray_Row_NG3 (Số hàng chứa NG3)
- txtTray_Row_NG4 (Số hàng chứa NG4)
- txtRORI_Distance_X (Khoảng cách 2 đầu hút)
- txtSocket_Angle (Góc xoay đầu hút)

## Lưu ý kỹ thuật

1. **Validation**: Chỉ chấp nhận giá trị số
2. **Auto-read back**: Sau khi ghi xuống PLC, giá trị sẽ được đọc lại để xác nhận
3. **Error handling**: Nếu có lỗi khi đọc/ghi, sẽ hiển thị MessageBox thông báo
4. **Port-specific**: Mỗi port có bộ địa chỉ PLC riêng, được map tự động

## Troubleshooting

### Nếu TextBox không hoạt động
1. Kiểm tra tên TextBox có đúng không (phân biệt hoa/thường)
2. Kiểm tra PLC có kết nối không
3. Kiểm tra địa chỉ PLC trong PLCAddresses.Generated.cs

### Nếu không lưu được giá trị
1. Kiểm tra nhập đúng định dạng số
2. Kiểm tra PLC có ở chế độ RUN không
3. Kiểm tra quyền ghi vào PLC

## Mở rộng tương lai

Nếu cần thêm TextBox mới:
1. Thêm TextBox trong Designer với tên thích hợp
2. Thêm mapping trong DataTabManager.cs (trong InitializeAddressMaps())
3. Không cần code thêm - Event handler sẽ tự động được đăng ký
