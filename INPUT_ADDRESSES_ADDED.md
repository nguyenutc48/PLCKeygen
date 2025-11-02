# ✅ Đã Thêm Input Addresses Vào PLCAddresses.Generated.cs

## 📋 Tổng Quan

Tôi đã thành công thêm **78 địa chỉ I/O sensors** vào file **PLCAddresses.Generated.cs** trong phần `Input` class.

## ✅ Những Gì Đã Thêm

### Port 1 I/O Sensors (MR21, MR22) - 26 địa chỉ

**MR21 Register (16 bits):**
- `PLCAddresses.Input.P1_Ss_VIn1` → MR2100 (Sensor đầu vào)
- `PLCAddresses.Input.P1_Ss_VOt1` → MR2101 (Sensor đầu ra)
- `PLCAddresses.Input.P1_Ss_VCa21_Port1` → MR2102 (Camera vacuum Port 1)
- `PLCAddresses.Input.P1_Ss_VCa21_Port2` → MR2103 (Camera vacuum Port 2)
- `PLCAddresses.Input.P1_Ss_Fix1_Open` → MR2104 (Fixture mở)
- `PLCAddresses.Input.P1_Ss_Fix1_Close` → MR2105 (Fixture đóng)
- `PLCAddresses.Input.P1_Stt_lca_Start1` → MR2106 (Đèn Start)
- `PLCAddresses.Input.P1_Stt_lca_Stop1` → MR2107 (Đèn Stop)
- `PLCAddresses.Input.P1_Stt_lca_Gre1` → MR2108 (Đèn xanh)
- `PLCAddresses.Input.P1_Stt_lca_Yel1` → MR2109 (Đèn vàng)
- `PLCAddresses.Input.P1_Stt_lca_Red1` → MR2110 (Đèn đỏ)
- `PLCAddresses.Input.P1_Ss_Door1` → MR2111 (Sensor cửa)
- `PLCAddresses.Input.P1_Ss_Jig_OK` → MR2112 (Jig OK)
- `PLCAddresses.Input.P1_Ss_Tray_OK` → MR2113 (Tray OK)
- `PLCAddresses.Input.P1_Ss_Jig_NG` → MR2114 (Jig NG)
- `PLCAddresses.Input.P1_Ss_Tray_NG` → MR2115 (Tray NG)

**MR22 Register (10 bits):**
- `PLCAddresses.Input.P1_Ss_Jig_NG4` → MR2200
- `PLCAddresses.Input.P1_Ss_Tray_NG4` → MR2201
- `PLCAddresses.Input.P1_SW_EMS1` → MR2202 (Emergency Stop)
- `PLCAddresses.Input.P1_SW_Start1` → MR2203 (Nút Start)
- `PLCAddresses.Input.P1_SW_Stop1` → MR2204 (Nút Stop)
- `PLCAddresses.Input.P1_SW_Reset1` → MR2205 (Nút Reset)
- `PLCAddresses.Input.P1_Ss_AirPlus1` → MR2206 (Khí nén +)
- `PLCAddresses.Input.P1_Ss_AirMinus1` → MR2207 (Khí nén -)
- `PLCAddresses.Input.P1_Ss_VSk1` → MR2208 (Vision Seek)
- `PLCAddresses.Input.P1_SW_Init1` → MR2209 (Nút Init)

### Port 2 I/O Sensors (MR1, MR2) - 26 địa chỉ

**MR1 Register (16 bits):**
- `PLCAddresses.Input.P2_Ss_VIn2` → MR100
- `PLCAddresses.Input.P2_Ss_VOt2` → MR101
- `PLCAddresses.Input.P2_Ss_VCa12_Port2` → MR102
- `PLCAddresses.Input.P2_Ss_VCa12_Port1` → MR103
- `PLCAddresses.Input.P2_Ss_Fix2_Open` → MR104
- `PLCAddresses.Input.P2_Ss_Fix2_Close` → MR105
- `PLCAddresses.Input.P2_Stt_lca_Start2_StatusTester` → MR106
- `PLCAddresses.Input.P2_Stt_lca_Stop2_StatusTester` → MR107
- `PLCAddresses.Input.P2_Stt_lca_Gre2` → MR108
- `PLCAddresses.Input.P2_Stt_lca_Yel2` → MR109
- `PLCAddresses.Input.P2_Stt_lca_Red2` → MR110
- `PLCAddresses.Input.P2_Ss_Door2` → MR111
- `PLCAddresses.Input.P2_Ss_Jig_OK` → MR112
- `PLCAddresses.Input.P2_Ss_Tray_OK` → MR113
- `PLCAddresses.Input.P2_Ss_Jig_NG` → MR114
- `PLCAddresses.Input.P2_Ss_Tray_NG` → MR115

**MR2 Register (10 bits):**
- `PLCAddresses.Input.P2_Ss_Jig_NG4` → MR200
- `PLCAddresses.Input.P2_Ss_Tray_NG4` → MR201
- `PLCAddresses.Input.P2_SW_EMS2` → MR202
- `PLCAddresses.Input.P2_SW_Start2` → MR203
- `PLCAddresses.Input.P2_SW_Stop2` → MR204
- `PLCAddresses.Input.P2_SW_Reset2` → MR205
- `PLCAddresses.Input.P2_Ss_AirPlus2` → MR206
- `PLCAddresses.Input.P2_Ss_AirMinus2` → MR207
- `PLCAddresses.Input.P2_Ss_VSk2` → MR208
- `PLCAddresses.Input.P2_SW_Init2` → MR209

### Port 3 I/O Sensors (MR31, MR32) - 26 địa chỉ

**MR31 Register (16 bits):**
- `PLCAddresses.Input.P3_Ss_VIn3` → MR3100
- `PLCAddresses.Input.P3_Ss_VOt3` → MR3101
- `PLCAddresses.Input.P3_Ss_VCa34_Port3` → MR3102
- `PLCAddresses.Input.P3_Ss_VCa34_Port4` → MR3103
- `PLCAddresses.Input.P3_Ss_Fix3_Open` → MR3104
- `PLCAddresses.Input.P3_Ss_Fix3_Close` → MR3105
- `PLCAddresses.Input.P3_Stt_lca_Start3` → MR3106
- `PLCAddresses.Input.P3_Stt_lca_Stop3` → MR3107
- `PLCAddresses.Input.P3_Stt_lca_Gre3` → MR3108
- `PLCAddresses.Input.P3_Stt_lca_Yel3` → MR3109
- `PLCAddresses.Input.P3_Stt_lca_Red3` → MR3110
- `PLCAddresses.Input.P3_Ss_Door3` → MR3111
- `PLCAddresses.Input.P3_Ss_Jig_OK` → MR3112
- `PLCAddresses.Input.P3_Ss_Tray_OK` → MR3113
- `PLCAddresses.Input.P3_Ss_Jig_NG` → MR3114
- `PLCAddresses.Input.P3_Ss_Tray_NG` → MR3115

**MR32 Register (10 bits):**
- `PLCAddresses.Input.P3_Ss_Jig_NG4` → MR3200
- `PLCAddresses.Input.P3_Ss_Tray_NG4` → MR3201
- `PLCAddresses.Input.P3_SW_EMS3` → MR3202
- `PLCAddresses.Input.P3_SW_Start3` → MR3203
- `PLCAddresses.Input.P3_SW_Stop3` → MR3204
- `PLCAddresses.Input.P3_SW_Reset3` → MR3205
- `PLCAddresses.Input.P3_Ss_AirPlus3` → MR3206
- `PLCAddresses.Input.P3_Ss_AirMinus3` → MR3207
- `PLCAddresses.Input.P3_Ss_VSk3` → MR3208
- `PLCAddresses.Input.P3_SW_Init3` → MR3209

---

## 🔄 Đã Cập Nhật Form1.IO.Extension.cs

File **Form1.IO.Extension.cs** đã được cập nhật để sử dụng địa chỉ mới từ `PLCAddresses.Input`:

### Trước (Old):
```csharp
bool ems1 = PLCKey.ReadBit(PLCAddresses.Port1_IO.SW_EMS1);
bool jigOK = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_Jig_OK);
```

### Sau (New):
```csharp
bool ems1 = PLCKey.ReadBit(PLCAddresses.Input.P1_SW_EMS1);
bool jigOK = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Jig_OK);
```

---

## 🚀 Cách Sử Dụng

### Ví Dụ 1: Đọc Sensor Đầu Vào

```csharp
// Đọc sensor đầu vào Port 1
bool inputSensor = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_VIn1);

if (inputSensor)
{
    Console.WriteLine("Có sản phẩm tại Port 1");
}
```

### Ví Dụ 2: Kiểm Tra Emergency Stop

```csharp
// Kiểm tra EMS tất cả các port
bool ems1 = PLCKey.ReadBit(PLCAddresses.Input.P1_SW_EMS1);
bool ems2 = PLCKey.ReadBit(PLCAddresses.Input.P2_SW_EMS2);
bool ems3 = PLCKey.ReadBit(PLCAddresses.Input.P3_SW_EMS3);

if (ems1 || ems2 || ems3)
{
    MessageBox.Show("EMERGENCY STOP ACTIVATED!");
}
```

### Ví Dụ 3: Kiểm Tra Chất Lượng

```csharp
// Kiểm tra chất lượng Port 1
bool jigOK = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Jig_OK);
bool jigNG = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Jig_NG);

if (jigOK)
{
    PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Green);
    Console.WriteLine("Sản phẩm OK!");
}
else if (jigNG)
{
    PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Red);
    Console.WriteLine("Sản phẩm NG!");
}
```

### Ví Dụ 4: Kiểm Tra Fixture

```csharp
// Kiểm tra trạng thái fixture
bool fixtureOpen = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Fix1_Open);
bool fixtureClosed = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Fix1_Close);

if (fixtureOpen)
{
    Console.WriteLine("Fixture đang mở");
}
else if (fixtureClosed)
{
    Console.WriteLine("Fixture đã đóng - Sẵn sàng xử lý");
}
else
{
    Console.WriteLine("Fixture đang chuyển động");
}
```

### Ví Dụ 5: Đọc Tất Cả Switches

```csharp
// Đọc tất cả nút bấm Port 1
bool start = PLCKey.ReadBit(PLCAddresses.Input.P1_SW_Start1);
bool stop = PLCKey.ReadBit(PLCAddresses.Input.P1_SW_Stop1);
bool reset = PLCKey.ReadBit(PLCAddresses.Input.P1_SW_Reset1);
bool init = PLCKey.ReadBit(PLCAddresses.Input.P1_SW_Init1);

Console.WriteLine($"Switches: Start={start}, Stop={stop}, Reset={reset}, Init={init}");
```

---

## 📊 Bảng So Sánh Naming Convention

| Loại | Tên Cũ (PLCAddresses.IO.cs) | Tên Mới (PLCAddresses.Generated.cs) |
|------|------------------------------|--------------------------------------|
| Sensor Input | `Port1_IO.Ss_VIn1` | `Input.P1_Ss_VIn1` |
| Sensor Output | `Port1_IO.Ss_VOt1` | `Input.P1_Ss_VOt1` |
| Emergency Stop | `Port1_IO.SW_EMS1` | `Input.P1_SW_EMS1` |
| Jig OK | `Port1_IO.Ss_Jig_OK` | `Input.P1_Ss_Jig_OK` |
| Fixture Open | `Port1_IO.Ss_Fix1_Open` | `Input.P1_Ss_Fix1_Open` |
| Door Sensor | `Port1_IO.Ss_Door1` | `Input.P1_Ss_Door1` |

---

## ✅ Tổng Kết

### Đã Hoàn Thành:
✅ **78 địa chỉ Input** đã được thêm vào `PLCAddresses.Generated.cs`
✅ **3 Ports** (Port 1, 2, 3) với đầy đủ sensors và switches
✅ **Form1.IO.Extension.cs** đã được cập nhật sử dụng địa chỉ mới
✅ **Naming convention nhất quán**: `PLCAddresses.Input.P{port}_{tên}`

### Cấu Trúc File:
```
PLCAddresses.Generated.cs
├── Input
│   ├── Port 1-4 Jog (đã có)
│   ├── Port 1 I/O Sensors (MR21, MR22) ← MỚI
│   ├── Port 2 I/O Sensors (MR1, MR2) ← MỚI
│   └── Port 3 I/O Sensors (MR31, MR32) ← MỚI
├── Output
│   ├── Camera Cylinders
│   └── Tower Lights (Port 1-3)
└── Data
    └── Position data (đã có)
```

### Files Liên Quan:
1. ✅ **PLCAddresses.Generated.cs** - Đã thêm Input addresses
2. ✅ **Form1.IO.Extension.cs** - Đã cập nhật sử dụng Input addresses
3. ✅ **PLCAddresses.IO.cs** - File gốc (giữ lại để tham khảo)

---

## 🎯 Sử Dụng Ngay

Bây giờ bạn có thể sử dụng:

```csharp
// Đọc sensors
bool sensor = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_VIn1);

// Điều khiển output
PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Green);

// Đọc position data
int xPos = PLCKey.ReadInt32(PLCAddresses.Data.P1_X_Pos_Cur);
```

Tất cả địa chỉ I/O đã được tích hợp vào **một file duy nhất** `PLCAddresses.Generated.cs` để dễ quản lý và bảo trì! 🎉
