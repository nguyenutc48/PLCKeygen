# Hộp thoại xác nhận khi lưu giá trị xuống PLC

## Tổng quan
Khi người dùng nhập giá trị mới vào TextBox và nhấn Enter, hệ thống sẽ hiển thị hộp thoại xác nhận trước khi ghi xuống PLC để tránh thay đổi nhầm.

## Quy trình xử lý

### Bước 1: Nhập giá trị
Người dùng nhập giá trị mới vào TextBox

### Bước 2: Nhấn Enter
Trigger event KeyDown

### Bước 3: Validation
- Kiểm tra giá trị hợp lệ
- Nếu không hợp lệ → Hiển thị MessageBox lỗi và dừng

### Bước 4: Hiển thị hộp thoại xác nhận (MỚI)
```
┌─────────────────────────────────────────┐
│ Xác nhận lưu giá trị              [?]   │
├─────────────────────────────────────────┤
│                                         │
│ Bạn có chắc chắn muốn lưu giá trị      │
│ '67.05' xuống PLC?                      │
│                                         │
│ Địa chỉ PLC: DM2056                     │
│ Port: 1                                 │
│                                         │
│                                         │
│           [ OK ]    [ Cancel ]          │
└─────────────────────────────────────────┘
```

### Bước 5a: Người dùng chọn OK
- Ghi giá trị xuống PLC
- Đọc lại giá trị từ PLC
- Hiển thị giá trị đã đọc lại
- Hiển thị MessageBox thành công

### Bước 5b: Người dùng chọn Cancel
- Không ghi xuống PLC
- Giữ nguyên giá trị hiện tại trong TextBox
- Kết thúc xử lý

### Bước 6: Thông báo thành công (nếu chọn OK)
```
┌─────────────────────────────────────────┐
│ Thành công                        [i]   │
├─────────────────────────────────────────┤
│                                         │
│ Đã lưu giá trị thành công!             │
│                                         │
│ Giá trị đã ghi: 67.05                  │
│                                         │
│                                         │
│                 [ OK ]                  │
└─────────────────────────────────────────┘
```

## Code implementation

### SaveTextBoxValueToPLC method
```csharp
public bool SaveTextBoxValueToPLC(TextBox textBox)
{
    string plcAddress = GetPLCAddressForTextBox(textBox.Name);

    if (string.IsNullOrEmpty(plcAddress))
        return false;

    try
    {
        int valueToWrite;
        string displayValue = textBox.Text;

        // Validation & parse value
        // ... (validation code)

        // Hiển thị hộp thoại xác nhận
        string confirmMessage = $"Bạn có chắc chắn muốn lưu giá trị '{displayValue}' xuống PLC?\n\n" +
                               $"Địa chỉ PLC: {plcAddress}\n" +
                               $"Port: {currentPort}";

        DialogResult result = MessageBox.Show(
            confirmMessage,
            "Xác nhận lưu giá trị",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Question
        );

        // Nếu người dùng chọn Cancel, không lưu
        if (result != DialogResult.OK)
        {
            return false;
        }

        // Write to PLC
        plcKeyence.WriteInt32(plcAddress, valueToWrite);

        // Read back to confirm
        int readValue = plcKeyence.ReadInt32(plcAddress);

        // Display read value with proper formatting
        // ... (display code)

        // Thông báo thành công
        MessageBox.Show(
            $"Đã lưu giá trị thành công!\n\nGiá trị đã ghi: {textBox.Text}",
            "Thành công",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );

        return true;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Lỗi khi ghi giá trị xuống PLC: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
    }
}
```

## Ví dụ sử dụng

### Ví dụ 1: Lưu giá trị thành công
```
User: Nhập "100" vào txtTray_Col_Number
User: Nhấn Enter
System: Hiển thị "Bạn có chắc chắn muốn lưu giá trị '100' xuống PLC?
         Địa chỉ PLC: DM2024
         Port: 1"
User: Click OK
System: Ghi 100 xuống DM2024
System: Đọc lại → 100
System: Hiển thị "Đã lưu giá trị thành công! Giá trị đã ghi: 100"
```

### Ví dụ 2: Hủy lưu giá trị
```
User: Nhập "200" vào txtTray_Row_Number
User: Nhấn Enter
System: Hiển thị "Bạn có chắc chắn muốn lưu giá trị '200' xuống PLC?
         Địa chỉ PLC: DM2026
         Port: 1"
User: Click Cancel
System: Không ghi xuống PLC
System: Giữ nguyên giá trị cũ trong TextBox
```

### Ví dụ 3: Lưu giá trị RORI Distance (đặc biệt)
```
User: Nhập "67.055" vào txtRORI_Distance_X
User: Nhấn Enter
System: Làm tròn → 67.06
System: Hiển thị "Bạn có chắc chắn muốn lưu giá trị '67.06' xuống PLC?
         Địa chỉ PLC: DM2056
         Port: 1"
User: Click OK
System: Ghi 6706 xuống DM2056
System: Đọc lại → 6706 (hiển thị 67.06)
System: Hiển thị "Đã lưu giá trị thành công! Giá trị đã ghi: 67.06"
```

### Ví dụ 4: Giá trị không hợp lệ
```
User: Nhập "abc" vào txtTray_Col_Number
User: Nhấn Enter
System: Hiển thị "Giá trị không hợp lệ. Vui lòng nhập số nguyên."
System: Không hiển thị hộp thoại xác nhận
```

## Lợi ích của tính năng

### 1. An toàn
- Tránh ghi nhầm giá trị xuống PLC
- Cho phép người dùng kiểm tra lại trước khi lưu
- Giảm thiểu lỗi do thao tác nhầm

### 2. Thông tin rõ ràng
- Hiển thị giá trị sẽ được ghi
- Hiển thị địa chỉ PLC
- Hiển thị Port hiện tại

### 3. Xác nhận thành công
- MessageBox thông báo sau khi ghi thành công
- Hiển thị giá trị đã ghi (đã đọc lại từ PLC)
- Tạo sự tin tưởng cho người dùng

### 4. Khả năng hủy bỏ
- Người dùng có thể Cancel nếu phát hiện sai
- Không làm thay đổi giá trị PLC khi Cancel

## Các trường hợp đặc biệt

### 1. Giá trị giống với giá trị hiện tại
Vẫn hiển thị hộp thoại xác nhận, vì người dùng có thể muốn:
- Làm mới giá trị
- Đồng bộ với PLC
- Kiểm tra kết nối

### 2. PLC mất kết nối
- Validation pass
- Hiển thị hộp thoại xác nhận
- User click OK
- Khi ghi → Exception
- Hiển thị MessageBox lỗi với thông tin chi tiết

### 3. Giá trị read-back khác với giá trị ghi
Có thể xảy ra nếu:
- PLC có giới hạn giá trị
- PLC có logic xử lý
- Vẫn hiển thị giá trị thật sự từ PLC

## MessageBox Types

### 1. Xác nhận (Confirmation)
```csharp
MessageBox.Show(
    message,
    "Xác nhận lưu giá trị",
    MessageBoxButtons.OKCancel,
    MessageBoxIcon.Question
);
```
- Icon: Question (?)
- Buttons: OK, Cancel
- Return: DialogResult.OK hoặc DialogResult.Cancel

### 2. Thành công (Success)
```csharp
MessageBox.Show(
    message,
    "Thành công",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information
);
```
- Icon: Information (i)
- Buttons: OK
- Return: DialogResult.OK

### 3. Lỗi (Error)
```csharp
MessageBox.Show(
    message,
    "Lỗi",
    MessageBoxButtons.OK,
    MessageBoxIcon.Error
);
```
- Icon: Error (X)
- Buttons: OK
- Return: DialogResult.OK

## Customization options (tùy chọn mở rộng)

### 1. Thêm nút "Lưu không hỏi lại"
```csharp
// Có thể thêm checkbox "Không hỏi lại cho lần sau"
// Lưu setting vào Properties.Settings
```

### 2. Timeout tự động
```csharp
// Có thể thêm timer để tự động Cancel sau 30s
```

### 3. Ghi log
```csharp
// Log mọi thao tác ghi xuống PLC
// Bao gồm: timestamp, user, port, address, old value, new value
```

### 4. Batch update
```csharp
// Cho phép lưu nhiều giá trị cùng lúc
// Chỉ hiển thị 1 hộp thoại xác nhận
```

## Testing checklist

- [ ] Nhập giá trị hợp lệ → Enter → OK → Ghi thành công
- [ ] Nhập giá trị hợp lệ → Enter → Cancel → Không ghi
- [ ] Nhập giá trị không hợp lệ → Enter → Lỗi validation (không có confirmation dialog)
- [ ] Nhập giá trị RORI Distance → Enter → OK → Ghi đúng (đã nhân 100)
- [ ] PLC disconnect → Enter → OK → Lỗi khi ghi
- [ ] Nhập giá trị → ESC (không trigger confirmation)
- [ ] Nhập giá trị → Tab (không trigger confirmation)
- [ ] Multiple TextBox → Enter liên tiếp → Mỗi lần đều có confirmation

## User feedback

Sau khi triển khai, thu thập feedback về:
1. Có quá nhiều click không? (validation → confirmation → success = 3 clicks)
2. Thông báo có rõ ràng không?
3. Có cần option "Không hỏi lại" không?
4. Có cần hiển thị giá trị cũ trong confirmation dialog không?

## Kết luận

Đã implement thành công hộp thoại xác nhận với:
- ✅ Validation trước khi hiển thị confirmation
- ✅ Confirmation dialog với thông tin đầy đủ
- ✅ Cho phép Cancel
- ✅ Success message sau khi ghi
- ✅ Error handling đầy đủ
- ✅ Build thành công: 0 errors

Tính năng này giúp tăng tính an toàn và độ tin cậy khi thao tác với PLC.
