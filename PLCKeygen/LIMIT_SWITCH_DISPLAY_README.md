# Limit Switch Display - Hướng Dẫn Sử Dụng

## Tổng Quan

PLCKeygen hiển thị trạng thái **limit switches** (công tắc hành trình) real-time từ PLC lên giao diện, giúp operator giám sát vị trí và trạng thái của các trục máy.

## Vị Trí Trên Giao Diện

### Jog Panel - Motion Tab

Các button indicator nằm trong **grpJogPanel** (Jog Control Panel):

```
┌─────────────────────────────────┐
│  Jog Control                    │
├─────────────────────────────────┤
│                                 │
│  X:  [L-] [O] [L+]             │
│  Y:  [L-] [O] [L+]             │
│  Z:  [L-] [O] [L+]             │
│  F:  [L-] [O] [L+]             │
│                                 │
└─────────────────────────────────┘

L- = Limit Minus (giới hạn âm)
O  = Home (điểm gốc)
L+ = Limit Plus (giới hạn dương)
```

## Màu Sắc Indicator

### 🟢 Xanh Lá (LimeGreen)
**Ý nghĩa**: Limit switch **ĐANG ACTIVE** (bit = 1)

**Ví dụ**:
- Trục X đang chạm vào limit switch âm → `lampXLimitMinus` màu xanh
- Trục Y đang ở vị trí home → `lampYHome` màu xanh
- Trục Z đang chạm limit switch dương → `lampZLimitPlus` màu xanh

### ⚪ Xám (Gray)
**Ý nghĩa**: Limit switch **KHÔNG ACTIVE** (bit = 0)

**Ví dụ**:
- Trục X không chạm limit switch → `lampXLimitMinus`, `lampXHome`, `lampXLimitPlus` màu xám
- Trục đang di chuyển giữa các limit → tất cả đều xám

## Mapping PLC Addresses

### Port 1

| Trục | Limit Minus | Home | Limit Plus |
|------|-------------|------|------------|
| **X** | DM388.00 | DM388.01 | DM388.02 |
| **Y** | DM418.00 | DM418.01 | DM418.02 |
| **Z** | DM448.00 | DM448.01 | DM448.02 |
| **F** | DM538.00 | DM538.01 | DM538.02 |

### Port 2

| Trục | Limit Minus | Home | Limit Plus |
|------|-------------|------|------------|
| **X** | DM28.00 | DM28.01 | DM28.02 |
| **Y** | DM58.00 | DM58.01 | DM58.02 |
| **Z** | DM88.00 | DM88.01 | DM88.02 |
| **F** | DM178.00 | DM178.01 | DM178.02 |

### Port 3

| Trục | Limit Minus | Home | Limit Plus |
|------|-------------|------|------------|
| **X** | DM568.00 | DM568.01 | DM568.02 |
| **Y** | DM598.00 | DM598.01 | DM598.02 |
| **Z** | DM628.00 | DM628.01 | DM628.02 |
| **F** | DM718.00 | DM718.01 | DM718.02 |

### Port 4

| Trục | Limit Minus | Home | Limit Plus |
|------|-------------|------|------------|
| **X** | DM208.00 | DM208.01 | DM208.02 |
| **Y** | DM238.00 | DM238.01 | DM238.02 |
| **Z** | DM268.00 | DM268.01 | DM268.02 |
| **F** | DM358.00 | DM358.01 | DM358.02 |

## Cách Hoạt Động

### 1. Đọc Bit từ DM Register

Địa chỉ dạng `DM388.00` được parse thành:
- **Word address**: `DM388`
- **Bit index**: `00` (bit 0)

Code:
```csharp
// Ví dụ: Đọc P1_X_LimitMinus = "DM388.00"
bool isActive = PLCKey.ReadBitFromWord("DM388", 0);
```

### 2. Cập Nhật Màu Sắc

```csharp
if (isActive)
{
    lampXLimitMinus.BackColor = Color.LimeGreen;  // Xanh
    lampXLimitMinus.ForeColor = Color.Black;
}
else
{
    lampXLimitMinus.BackColor = Color.Gray;  // Xám
    lampXLimitMinus.ForeColor = Color.White;
}
```

### 3. Update Timing

**Tần suất cập nhật**: Theo `timer1` interval (default: ~500ms - 1s)

**Trigger events**:
1. `timer1_Tick()` - Cập nhật định kỳ tự động
2. `PortRadioButton_CheckedChanged()` - Khi thay đổi Port
3. `IOPortRadioButton_CheckedChanged()` - Khi thay đổi IO Port

## Technical Implementation

### Method: UpdateLimitSwitchDisplays()

**Mục đích**: Đọc và cập nhật trạng thái limit switches cho port hiện tại

**Location**: [Form1.cs:1913-1997](d:\3. Program\C#\PLCKeygen\PLCKeygen\Form1.cs#L1913)

**Flow**:
```
UpdateLimitSwitchDisplays()
    │
    ▼
Check selectedPort (1, 2, 3, or 4)
    │
    ▼
For each axis (X, Y, Z, F):
    │
    ├─ UpdateLimitLamp(lampXLimitMinus, PLCAddresses.Input.P1_X_LimitMinus)
    ├─ UpdateLimitLamp(lampXHome, PLCAddresses.Input.P1_X_Home)
    └─ UpdateLimitLamp(lampXLimitPlus, PLCAddresses.Input.P1_X_LimitPlus)
```

### Method: UpdateLimitLamp()

**Mục đích**: Cập nhật màu sắc cho một button dựa trên trạng thái bit

**Location**: [Form1.cs:2003-2026](d:\3. Program\C#\PLCKeygen\PLCKeygen\Form1.cs#L2003)

**Parameters**:
- `lamp`: Button control cần update
- `address`: PLC address (ví dụ: "DM388.00")

**Code**:
```csharp
private void UpdateLimitLamp(System.Windows.Forms.Button lamp, string address)
{
    try
    {
        bool isActive = PLCKey.ReadBitFromWord(address, GetBitIndexFromAddress(address));

        if (isActive)
        {
            lamp.BackColor = Color.LimeGreen;  // Active
            lamp.ForeColor = Color.Black;
        }
        else
        {
            lamp.BackColor = Color.Gray;  // Inactive
            lamp.ForeColor = Color.White;
        }
    }
    catch (Exception ex)
    {
        // On error, set to gray
        lamp.BackColor = Color.Gray;
        lamp.ForeColor = Color.White;
    }
}
```

### Method: GetBitIndexFromAddress()

**Mục đích**: Parse bit index từ address string

**Location**: [Form1.cs:2031-2045](d:\3. Program\C#\PLCKeygen\PLCKeygen\Form1.cs#L2031)

**Examples**:
- `"DM388.00"` → `0`
- `"DM388.01"` → `1`
- `"DM388.02"` → `2`

**Code**:
```csharp
private int GetBitIndexFromAddress(string address)
{
    if (address.Contains("."))
    {
        string[] parts = address.Split('.');
        if (parts.Length == 2)
        {
            if (int.TryParse(parts[1], out int bitIndex))
            {
                return bitIndex;
            }
        }
    }
    return 0;
}
```

## Scenarios

### Scenario 1: Trục Đang Ở Vị Trí Home

**Tình huống**:
- Operator bấm "Home All" hoặc "Home X"
- Trục X di chuyển về vị trí home
- Chạm sensor home

**Kết quả**:
- `lampXHome` chuyển sang **màu xanh** 🟢
- `lampXLimitMinus` và `lampXLimitPlus` vẫn màu xám
- Operator biết trục đã về home an toàn

### Scenario 2: Trục Chạm Limit Switch

**Tình huống**:
- Operator jog trục Y về phía âm quá xa
- Trục chạm vào limit switch âm
- PLC set bit `DM418.00` = 1

**Kết quả**:
- `lampYLimitMinus` chuyển sang **màu xanh** 🟢
- Warning cho operator: Trục đã đạt giới hạn
- Cần jog về hướng ngược lại

### Scenario 3: Trục Di Chuyển Giữa Các Limit

**Tình huống**:
- Trục Z đang di chuyển bình thường
- Không chạm vào limit nào

**Kết quả**:
- Tất cả `lampZLimitMinus`, `lampZHome`, `lampZLimitPlus` đều **màu xám** ⚪
- Cho biết trục đang ở vị trí an toàn, chưa chạm giới hạn

### Scenario 4: Thay Đổi Port

**Tình huống**:
- User đang xem Port 1
- Click radio button Port 2

**Kết quả**:
- `UpdateLimitSwitchDisplays()` được gọi ngay lập tức
- Tất cả lamp được cập nhật với trạng thái của Port 2
- User thấy trạng thái limit switches của port mới

## Debugging

### Kiểm Tra Limit Switch Status

**Method 1: Visual Inspection**
- Nhìn vào Jog Panel
- Quan sát màu sắc của buttons
- Xanh = Active, Xám = Inactive

**Method 2: Console Log**
```csharp
// Thêm vào UpdateLimitLamp() để debug
Console.WriteLine($"Lamp: {lamp.Name}, Address: {address}, Active: {isActive}");
```

**Method 3: Breakpoint**
- Set breakpoint tại `UpdateLimitLamp()` line 2007
- Run debug mode
- Xem giá trị `isActive`, `address`, `lamp.Name`

### Common Issues

#### Issue 1: Limit Lamp Luôn Màu Xám

**Nguyên nhân**:
1. PLC không kết nối
2. Address PLC sai
3. Sensor limit switch hỏng

**Giải pháp**:
1. Kiểm tra PLC connection (status bar màu xanh)
2. Verify address trong [PLCAddresses.Generated.cs](d:\3. Program\C#\PLCKeygen\PLCKeygen\PLCAddresses.Generated.cs)
3. Test sensor bằng cách chạm tay vào limit switch

#### Issue 2: Limit Lamp Không Đổi Màu Khi Chạm

**Nguyên nhân**:
1. Timer không chạy
2. Exception trong `UpdateLimitLamp()`
3. Địa chỉ bit index sai

**Giải pháp**:
1. Kiểm tra `timer1.Enabled = true`
2. Xem Debug Output window cho exceptions
3. Verify bit index (00, 01, 02) trong PLCAddresses

#### Issue 3: Chỉ Một Số Lamp Hoạt Động

**Nguyên nhân**:
1. Thiếu mapping cho một số trục
2. PLC chưa config hết các sensors

**Giải pháp**:
1. Kiểm tra `UpdateLimitSwitchDisplays()` có gọi tất cả UpdateLimitLamp()
2. Verify PLC ladder có đọc sensors chưa

## Performance

### Update Frequency

**Timer Interval**: ~500ms - 1s (tùy thuộc timer1 configuration)

**Number of Reads per Update**:
- 4 axes × 3 limit switches = 12 bit reads per port
- Example: Port 1 → đọc 12 bits từ PLC

**Network Traffic**:
- Minimal - chỉ đọc bits, không ghi
- Sử dụng `ReadBitFromWord()` - efficient

### Optimization

**Tip 1**: Tăng timer interval nếu cần giảm network load
```csharp
timer1.Interval = 1000;  // 1 second thay vì 500ms
```

**Tip 2**: Chỉ update khi tab Motion được focus
```csharp
if (tabControl1.SelectedTab == tabPageMotion)
{
    UpdateLimitSwitchDisplays();
}
```

## Safety

### Warning Colors

Nếu muốn thêm cảnh báo màu đỏ khi chạm limit:

```csharp
// Trong UpdateLimitLamp()
if (isActive && (address.Contains("LimitMinus") || address.Contains("LimitPlus")))
{
    lamp.BackColor = Color.Red;  // Warning - chạm limit!
    lamp.ForeColor = Color.White;
}
else if (isActive && address.Contains("Home"))
{
    lamp.BackColor = Color.LimeGreen;  // OK - ở home
    lamp.ForeColor = Color.Black;
}
```

### Stop Motion on Limit

Nếu muốn tự động dừng khi chạm limit:

```csharp
// Trong UpdateLimitSwitchDisplays()
// Kiểm tra nếu đang jog và chạm limit
if (isJogging && (isLimitMinusActive || isLimitPlusActive))
{
    StopAllMotion();
    MessageBox.Show("Chạm giới hạn! Đã dừng chuyển động.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
```

## Best Practices

### ✅ DO

1. **Quan sát limit lamps trước khi jog**
   - Đảm bảo không có lamp nào xanh trước khi di chuyển
   - Nếu có lamp xanh → trục đang ở limit

2. **Sử dụng Home function thường xuyên**
   - Home All trước khi bắt đầu ca làm việc
   - Đảm bảo trục ở vị trí chuẩn

3. **Không ignore limit warnings**
   - Nếu limit lamp xanh → dừng ngay
   - Kiểm tra nguyên nhân

### ❌ DON'T

1. **Đừng force jog khi đang ở limit**
   - Có thể làm hỏng sensor hoặc trục
   - Jog về hướng ngược lại

2. **Đừng disable limit switches**
   - Rất nguy hiểm
   - Có thể gây hỏng máy

3. **Đừng modify PLC addresses**
   - File `PLCAddresses.Generated.cs` là auto-generated
   - Chỉ modify nếu biết rõ đang làm gì

## Summary

### Key Features

✅ **Real-time Display**
- Cập nhật limit switches mỗi ~500ms-1s
- Tự động theo dõi khi thay đổi port

✅ **Visual Feedback**
- 🟢 Xanh = Active (đang chạm)
- ⚪ Xám = Inactive (không chạm)

✅ **All Axes Supported**
- X, Y, Z, F axes
- Limit-, Home, Limit+ cho mỗi trục

✅ **All Ports Supported**
- Port 1, 2, 3, 4
- Tự động switch khi đổi port

### Control List

| Control Name | Function | Location |
|-------------|----------|----------|
| `lampXLimitMinus` | X axis limit minus | grpJogPanel |
| `lampXHome` | X axis home | grpJogPanel |
| `lampXLimitPlus` | X axis limit plus | grpJogPanel |
| `lampYLimitMinus` | Y axis limit minus | grpJogPanel |
| `lampYHome` | Y axis home | grpJogPanel |
| `lampYLimitPlus` | Y axis limit plus | grpJogPanel |
| `lampZLimitMinus` | Z axis limit minus | grpJogPanel |
| `lampZHome` | Z axis home | grpJogPanel |
| `lampZLimitPlus` | Z axis limit plus | grpJogPanel |
| `lampFLimitMinus` | F axis limit minus | grpJogPanel |
| `lampFHome` | F axis home | grpJogPanel |
| `lampFLimitPlus` | F axis limit plus | grpJogPanel |

Version: PLCKeygen 2025.11.9+
