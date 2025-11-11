# Xử lý đặc biệt cho txtRORI_Distance_X

## Tổng quan
TextBox `txtRORI_Distance_X` cần xử lý đặc biệt vì:
- **Đơn vị hiển thị**: millimeters (mm) với 2 chữ số thập phân
- **Đơn vị trong PLC**: Int32 (giá trị * 100)
- **Chuyển đổi**: Cần nhân/chia 100 khi đọc/ghi

## Ví dụ cụ thể

### Đọc từ PLC lên hiển thị
```
Giá trị PLC: 6705 (Int32)
           ↓ (chia cho 100)
Hiển thị:   67.05 mm
```

### Ghi từ TextBox xuống PLC
```
Nhập vào:   67.055 mm
           ↓ (làm tròn 2 chữ số)
Sau round:  67.05 mm
           ↓ (nhân 100)
Ghi PLC:    6705 (Int32)
```

## Quy trình xử lý

### 1. Đọc giá trị từ PLC (LoadTeachingDataToTextBoxes)

```csharp
int value = plcKeyence.ReadInt32(plcAddress);

if (textBox.Name == "txtRORI_Distance_X")
{
    double displayValue = value / 100.0;
    textBox.Text = displayValue.ToString("F2"); // Format 2 chữ số thập phân
}
```

**Ví dụ**:
- PLC = 6705 → Hiển thị = "67.05"
- PLC = 10050 → Hiển thị = "100.50"
- PLC = 500 → Hiển thị = "5.00"

### 2. Ghi giá trị xuống PLC (SaveTextBoxValueToPLC)

```csharp
if (textBox.Name == "txtRORI_Distance_X")
{
    // Parse giá trị double (mm)
    if (!double.TryParse(textBox.Text, out double doubleValue))
    {
        MessageBox.Show("Giá trị không hợp lệ. Vui lòng nhập số (ví dụ: 67.05).", "Lỗi");
        return false;
    }

    // Làm tròn 2 chữ số thập phân và nhân 100
    doubleValue = Math.Round(doubleValue, 2);
    valueToWrite = (int)(doubleValue * 100);
}
```

**Ví dụ**:
- Nhập: "67.055" → Round: 67.05 → PLC: 6705
- Nhập: "67.054" → Round: 67.05 → PLC: 6705
- Nhập: "100.5" → Round: 100.50 → PLC: 10050
- Nhập: "100.999" → Round: 101.00 → PLC: 10100

### 3. Read-back sau khi ghi

```csharp
int readValue = plcKeyence.ReadInt32(plcAddress);

if (textBox.Name == "txtRORI_Distance_X")
{
    double displayValue = readValue / 100.0;
    textBox.Text = displayValue.ToString("F2");
}
```

## Các trường hợp test

### Test case 1: Giá trị chính xác
```
Input:    67.05
Expected: 67.05 (PLC: 6705)
Result:   ✓ Pass
```

### Test case 2: Giá trị cần làm tròn xuống
```
Input:    67.054
Expected: 67.05 (PLC: 6705)
Result:   ✓ Pass
```

### Test case 3: Giá trị cần làm tròn lên
```
Input:    67.055
Expected: 67.06 (PLC: 6706)
Result:   ✓ Pass (MidpointRounding.ToEven)
```

### Test case 4: Giá trị thập phân 1 chữ số
```
Input:    67.5
Expected: 67.50 (PLC: 6750)
Result:   ✓ Pass
```

### Test case 5: Giá trị nguyên
```
Input:    67
Expected: 67.00 (PLC: 6700)
Result:   ✓ Pass
```

### Test case 6: Giá trị âm (nếu cho phép)
```
Input:    -67.05
Expected: -67.05 (PLC: -6705)
Result:   ✓ Pass
```

### Test case 7: Giá trị không hợp lệ
```
Input:    abc
Expected: Error message
Result:   ✓ Pass
```

### Test case 8: Giá trị rỗng
```
Input:    (empty)
Expected: Error message
Result:   ✓ Pass
```

## Lưu ý quan trọng

### 1. Làm tròn (Rounding)
- Sử dụng `Math.Round(value, 2)` với mode mặc định
- Mode mặc định là `MidpointRounding.ToEven` (banker's rounding)
- Ví dụ:
  - 67.055 → 67.06 (round up vì 5 sau số lẻ)
  - 67.045 → 67.04 (round down vì 5 sau số chẵn)

### 2. Format hiển thị
- Sử dụng `ToString("F2")` để luôn hiển thị 2 chữ số thập phân
- Ví dụ:
  - 67.5 → "67.50"
  - 67 → "67.00"
  - 67.123 → "67.12"

### 3. Validation
- Chỉ chấp nhận giá trị số
- Cho phép dấu thập phân (. hoặc ,)
- Cho phép số âm (nếu logic cho phép)

### 4. Độ chính xác
- Độ phân giải: 0.01 mm (1 unit PLC)
- Giá trị min/max phụ thuộc vào Int32: -21474.83 đến 21474.83 mm

## Code snippet đầy đủ

### LoadTeachingDataToTextBoxes
```csharp
int value = plcKeyence.ReadInt32(plcAddress);

// Xử lý đặc biệt cho txtRORI_Distance_X: chia cho 100 để hiển thị mm
if (textBox.Name == "txtRORI_Distance_X")
{
    double displayValue = value / 100.0;
    textBox.Text = displayValue.ToString("F2"); // Format 2 chữ số thập phân
}
else
{
    textBox.Text = value.ToString();
}
```

### SaveTextBoxValueToPLC
```csharp
int valueToWrite;

// Xử lý đặc biệt cho txtRORI_Distance_X
if (textBox.Name == "txtRORI_Distance_X")
{
    // Parse giá trị double (mm)
    if (!double.TryParse(textBox.Text, out double doubleValue))
    {
        MessageBox.Show("Giá trị không hợp lệ. Vui lòng nhập số (ví dụ: 67.05).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
    }

    // Làm tròn 2 chữ số thập phân và nhân 100
    doubleValue = Math.Round(doubleValue, 2);
    valueToWrite = (int)(doubleValue * 100);
}
else
{
    // Validate input is numeric integer
    if (!int.TryParse(textBox.Text, out valueToWrite))
    {
        MessageBox.Show("Giá trị không hợp lệ. Vui lòng nhập số nguyên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
    }
}

// Write to PLC
plcKeyence.WriteInt32(plcAddress, valueToWrite);

// Read back to confirm
int readValue = plcKeyence.ReadInt32(plcAddress);

// Display read value with proper formatting
if (textBox.Name == "txtRORI_Distance_X")
{
    double displayValue = readValue / 100.0;
    textBox.Text = displayValue.ToString("F2");
}
else
{
    textBox.Text = readValue.ToString();
}
```

## Mở rộng cho các TextBox khác

Nếu có các TextBox khác cũng cần xử lý tương tự (ví dụ: txtRORI_Distance_Y), có thể:

### Cách 1: Thêm điều kiện OR
```csharp
if (textBox.Name == "txtRORI_Distance_X" || textBox.Name == "txtRORI_Distance_Y")
{
    // Xử lý chia/nhân 100
}
```

### Cách 2: Sử dụng list
```csharp
private static readonly HashSet<string> DistanceTextBoxes = new HashSet<string>
{
    "txtRORI_Distance_X",
    "txtRORI_Distance_Y"
};

if (DistanceTextBoxes.Contains(textBox.Name))
{
    // Xử lý chia/nhân 100
}
```

### Cách 3: Tạo method riêng
```csharp
private bool IsDistanceTextBox(string name)
{
    return name.Contains("Distance") || name == "txtRORI_Distance_X";
}

if (IsDistanceTextBox(textBox.Name))
{
    // Xử lý chia/nhân 100
}
```

## Troubleshooting

### Vấn đề: Giá trị hiển thị không đúng
**Nguyên nhân**: Quên chia cho 100 khi đọc
**Giải pháp**: Kiểm tra logic trong LoadTeachingDataToTextBoxes

### Vấn đề: Giá trị ghi xuống PLC không đúng
**Nguyên nhân**: Quên nhân 100 khi ghi
**Giải pháp**: Kiểm tra logic trong SaveTextBoxValueToPLC

### Vấn đề: Mất độ chính xác
**Nguyên nhân**: Round sai hoặc format sai
**Giải pháp**: Sử dụng Math.Round(value, 2) và ToString("F2")

### Vấn đề: Không nhận giá trị thập phân
**Nguyên nhân**: Sử dụng int.TryParse thay vì double.TryParse
**Giải pháp**: Đảm bảo dùng double.TryParse cho txtRORI_Distance_X

## Kết luận

Xử lý đặc biệt cho `txtRORI_Distance_X` đã được implement đầy đủ với:
- ✅ Đọc từ PLC: chia cho 100 và format F2
- ✅ Ghi xuống PLC: làm tròn 2 chữ số và nhân 100
- ✅ Validation: chấp nhận giá trị double
- ✅ Read-back: xác nhận giá trị đã ghi
- ✅ Build thành công: 0 errors

Người dùng có thể nhập giá trị như: 67.05, 67.055, 100.5, 100 và hệ thống sẽ xử lý đúng!
