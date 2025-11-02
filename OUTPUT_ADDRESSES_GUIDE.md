# 📘 Hướng Dẫn Sử Dụng Output Addresses

## ✅ Đã Thêm Output Addresses

Tôi đã thêm các địa chỉ output vào **PLCAddresses.Generated.cs** bao gồm:

### 1. Đèn Tháp (Tower Lights) - 3 Ports

**Port 1:**
- `PLCAddresses.Output.P1_Tower_Green` → MR2108 (Đèn xanh)
- `PLCAddresses.Output.P1_Tower_Yellow` → MR2109 (Đèn vàng)
- `PLCAddresses.Output.P1_Tower_Red` → MR2110 (Đèn đỏ)
- `PLCAddresses.Output.P1_Tower_Start` → MR2106 (Đèn Start)
- `PLCAddresses.Output.P1_Tower_Stop` → MR2107 (Đèn Stop)

**Port 2:**
- `PLCAddresses.Output.P2_Tower_Green` → MR108 (Đèn xanh)
- `PLCAddresses.Output.P2_Tower_Yellow` → MR109 (Đèn vàng)
- `PLCAddresses.Output.P2_Tower_Red` → MR110 (Đèn đỏ)
- `PLCAddresses.Output.P2_Tower_Start` → MR106 (Đèn Start)
- `PLCAddresses.Output.P2_Tower_Stop` → MR107 (Đèn Stop)

**Port 3:**
- `PLCAddresses.Output.P3_Tower_Green` → MR3108 (Đèn xanh)
- `PLCAddresses.Output.P3_Tower_Yellow` → MR3109 (Đèn vàng)
- `PLCAddresses.Output.P3_Tower_Red` → MR3110 (Đèn đỏ)
- `PLCAddresses.Output.P3_Tower_Start` → MR3106 (Đèn Start)
- `PLCAddresses.Output.P3_Tower_Stop` → MR3107 (Đèn Stop)

### 2. Xi Lanh Camera (Camera Cylinders)

- `PLCAddresses.Output.P12_Cam_cylinder` → MR5002 (Xi lanh camera Port 1-2)
- `PLCAddresses.Output.P34_Cam_cylinder` → MR6002 (Xi lanh camera Port 3-4)

### 3. Helper Method

- `PLCAddresses.Output.GetTowerLight(port, color)` - Lấy địa chỉ đèn động

---

## 🚀 Cách Sử Dụng

### Phương Pháp 1: Truy Cập Trực Tiếp

```csharp
// Bật đèn xanh Port 1
PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Green);

// Bật đèn đỏ Port 2
PLCKey.SetBit(PLCAddresses.Output.P2_Tower_Red);

// Bật đèn vàng Port 3
PLCKey.SetBit(PLCAddresses.Output.P3_Tower_Yellow);

// Tắt đèn
PLCKey.ResetBit(PLCAddresses.Output.P1_Tower_Green);
```

### Phương Pháp 2: Sử Dụng Helper Method

```csharp
// Lấy địa chỉ đèn xanh Port 1
string greenAddr = PLCAddresses.Output.GetTowerLight(1, "GREEN");
PLCKey.SetBit(greenAddr);

// Lấy địa chỉ đèn đỏ Port 2
string redAddr = PLCAddresses.Output.GetTowerLight(2, "RED");
PLCKey.SetBit(redAddr);

// Lấy địa chỉ đèn vàng Port 3
string yellowAddr = PLCAddresses.Output.GetTowerLight(3, "YELLOW");
PLCKey.SetBit(yellowAddr);
```

### Phương Pháp 3: Sử Dụng Method Trong Form1

```csharp
// Nếu bạn đã thêm Form1.IO.Extension.cs
SetTowerLight(1, "GREEN");   // Đèn xanh = thành công
SetTowerLight(2, "YELLOW");  // Đèn vàng = đang xử lý
SetTowerLight(3, "RED");     // Đèn đỏ = lỗi
SetTowerLight(1, "OFF");     // Tắt tất cả đèn
```

---

## 📋 Ví Dụ Thực Tế

### Ví Dụ 1: Kiểm Tra Chất Lượng và Báo Hiệu

```csharp
private void btnCheckQuality_Click(object sender, EventArgs e)
{
    // Đọc sensor chất lượng
    bool jigOK = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_Jig_OK);
    bool jigNG = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_Jig_NG);

    if (jigOK)
    {
        // Sản phẩm OK - Bật đèn xanh
        PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Green);
        MessageBox.Show("Sản phẩm OK!", "Chất Lượng");
    }
    else if (jigNG)
    {
        // Sản phẩm NG - Bật đèn đỏ
        PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Red);
        MessageBox.Show("Sản phẩm NG!", "Chất Lượng");
    }
    else
    {
        // Chưa có kết quả - Bật đèn vàng
        PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Yellow);
        MessageBox.Show("Đang kiểm tra...", "Chất Lượng");
    }
}
```

### Ví Dụ 2: Điều Khiển Xi Lanh Camera

```csharp
private void btnCameraCylinder_Click(object sender, EventArgs e)
{
    // Đọc trạng thái hiện tại
    bool currentState = PLCKey.ReadBit(PLCAddresses.Output.P12_Cam_cylinder);

    if (currentState)
    {
        // Đang ở bên phải -> Chuyển sang trái
        PLCKey.ResetBit(PLCAddresses.Output.P12_Cam_cylinder);
        btnCameraCylinder.Text = "Sang Trái";
    }
    else
    {
        // Đang ở bên trái -> Chuyển sang phải
        PLCKey.SetBit(PLCAddresses.Output.P12_Cam_cylinder);
        btnCameraCylinder.Text = "Sang Phải";
    }
}
```

### Ví Dụ 3: Chuỗi Sản Xuất Hoàn Chỉnh

```csharp
private async void btnStartProduction_Click(object sender, EventArgs e)
{
    int port = 1; // Chọn Port 1

    // Bước 1: Đèn vàng - Đang chờ
    PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Yellow);
    Console.WriteLine("Đang chờ sản phẩm...");
    await Task.Delay(2000);

    // Bước 2: Kiểm tra có sản phẩm không
    bool haspart = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_VIn1);
    if (!haspart)
    {
        PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Red);
        MessageBox.Show("Không có sản phẩm!");
        return;
    }

    // Bước 3: Đang xử lý
    Console.WriteLine("Đang xử lý...");
    await Task.Delay(3000);

    // Bước 4: Kiểm tra chất lượng
    bool qualityOK = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_Jig_OK);

    if (qualityOK)
    {
        // Thành công - Đèn xanh
        PLCKey.ResetBit(PLCAddresses.Output.P1_Tower_Yellow);
        PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Green);
        MessageBox.Show("Sản phẩm OK!");
    }
    else
    {
        // Lỗi - Đèn đỏ
        PLCKey.ResetBit(PLCAddresses.Output.P1_Tower_Yellow);
        PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Red);
        MessageBox.Show("Sản phẩm NG!");
    }
}
```

### Ví Dụ 4: Nhấp Nháy Đèn Cảnh Báo

```csharp
private async Task BlinkRedLight(int port, int times = 5)
{
    string redLightAddr = PLCAddresses.Output.GetTowerLight(port, "RED");

    for (int i = 0; i < times; i++)
    {
        PLCKey.SetBit(redLightAddr);
        await Task.Delay(300);

        PLCKey.ResetBit(redLightAddr);
        await Task.Delay(300);
    }

    // Giữ đèn đỏ sáng
    PLCKey.SetBit(redLightAddr);
}

// Sử dụng
private async void btnError_Click(object sender, EventArgs e)
{
    await BlinkRedLight(1, 5); // Nhấp nháy 5 lần
    MessageBox.Show("Lỗi nghiêm trọng!");
}
```

### Ví Dụ 5: Reset Tất Cả Output

```csharp
private void btnResetAll_Click(object sender, EventArgs e)
{
    // Tắt tất cả đèn tháp
    for (int port = 1; port <= 3; port++)
    {
        PLCKey.ResetBit(PLCAddresses.Output.GetTowerLight(port, "GREEN"));
        PLCKey.ResetBit(PLCAddresses.Output.GetTowerLight(port, "YELLOW"));
        PLCKey.ResetBit(PLCAddresses.Output.GetTowerLight(port, "RED"));
        PLCKey.ResetBit(PLCAddresses.Output.GetTowerLight(port, "START"));
        PLCKey.ResetBit(PLCAddresses.Output.GetTowerLight(port, "STOP"));
    }

    // Tắt xi lanh camera
    PLCKey.ResetBit(PLCAddresses.Output.P12_Cam_cylinder);
    PLCKey.ResetBit(PLCAddresses.Output.P34_Cam_cylinder);

    MessageBox.Show("Đã reset tất cả output!");
}
```

---

## 📊 Bảng Tổng Hợp Output

| Output | Port 1 | Port 2 | Port 3 |
|--------|--------|--------|--------|
| Đèn Xanh | `P1_Tower_Green` (MR2108) | `P2_Tower_Green` (MR108) | `P3_Tower_Green` (MR3108) |
| Đèn Vàng | `P1_Tower_Yellow` (MR2109) | `P2_Tower_Yellow` (MR109) | `P3_Tower_Yellow` (MR3109) |
| Đèn Đỏ | `P1_Tower_Red` (MR2110) | `P2_Tower_Red` (MR110) | `P3_Tower_Red` (MR3110) |
| Đèn Start | `P1_Tower_Start` (MR2106) | `P2_Tower_Start` (MR106) | `P3_Tower_Start` (MR3106) |
| Đèn Stop | `P1_Tower_Stop` (MR2107) | `P2_Tower_Stop` (MR107) | `P3_Tower_Stop` (MR3107) |

| Xi Lanh | Địa Chỉ | Ghi Chú |
|---------|---------|---------|
| Camera P1-2 | `P12_Cam_cylinder` (MR5002) | OFF=Trái, ON=Phải |
| Camera P3-4 | `P34_Cam_cylinder` (MR6002) | OFF=Trái, ON=Phải |

---

## 🎯 Ý Nghĩa Màu Đèn Tháp

| Màu | Ý Nghĩa | Khi Nào Sử Dụng |
|-----|---------|------------------|
| 🟢 XANH | Thành công / OK / Sẵn sàng | Sản phẩm OK, quy trình hoàn thành |
| 🟡 VÀNG | Cảnh báo / Đang chờ / Đang xử lý | Đang kiểm tra, đang chờ sản phẩm |
| 🔴 ĐỎ | Lỗi / NG / Dừng | Sản phẩm NG, lỗi hệ thống, EMS |
| ⚪ START | Bắt đầu | Quy trình đang chạy |
| ⚪ STOP | Dừng | Quy trình đã dừng |

---

## 💡 Lưu Ý Quan Trọng

### 1. Tắt Đèn Trước Khi Bật Đèn Mới
```csharp
// ✅ ĐÚNG
PLCKey.ResetBit(PLCAddresses.Output.P1_Tower_Yellow); // Tắt vàng
PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Green);    // Bật xanh

// ❌ SAI - Có thể có nhiều đèn sáng cùng lúc
PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Green);
```

### 2. Sử Dụng Method Có Sẵn
```csharp
// ✅ TỐT - Tự động tắt các đèn khác
SetTowerLight(1, "GREEN");

// ❌ DÀI - Phải tự tắt thủ công
PLCKey.ResetBit(PLCAddresses.Output.P1_Tower_Yellow);
PLCKey.ResetBit(PLCAddresses.Output.P1_Tower_Red);
PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Green);
```

### 3. Kiểm Tra Kết Nối PLC
```csharp
if (PLCKey != null)
{
    PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Green);
}
else
{
    MessageBox.Show("PLC chưa kết nối!");
}
```

---

## 📁 Files Liên Quan

1. **PLCAddresses.Generated.cs** - Chứa tất cả địa chỉ output
2. **PLCAddresses.Output.UsageExample.cs** - 10 ví dụ chi tiết
3. **Form1.IO.Extension.cs** - Methods hỗ trợ (đã cập nhật)
4. **OUTPUT_ADDRESSES_GUIDE.md** - Tài liệu này

---

## ✅ Tóm Tắt

✅ **Đã thêm:** 15 địa chỉ đèn tháp + 2 địa chỉ xi lanh
✅ **Helper method:** `GetTowerLight(port, color)`
✅ **Đã cập nhật:** Form1.IO.Extension.cs sử dụng Output addresses
✅ **Ví dụ:** 10 ví dụ sử dụng trong PLCAddresses.Output.UsageExample.cs

**Sẵn sàng sử dụng ngay!** 🎉
