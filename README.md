# HỆ THỐNG QUẢN LÝ PLC - THAY ĐỔI JSON KHÔNG CẦN REBUILD! 🚀

## Tính năng chính

### ✅ THAY ĐỔI ĐỊA CHỈ KHÔNG CẦN REBUILD CODE

**Thay đổi file `PLCConfig.json` và restart app là đủ!**

```
1. Sửa PLCConfig.json
2. Restart ứng dụng
3. ✅ Địa chỉ mới được áp dụng ngay!
```

**KHÔNG CẦN:**
- ❌ Rebuild code
- ❌ Generate lại file
- ❌ Release lại chương trình

---

## Quick Start - 3 bước

### Bước 1: Khởi tạo trong Program.cs

```csharp
using PLCKeygen;

class Program
{
    static void Main()
    {
        // Load config từ JSON
        PLCManager.Instance.Initialize("PLCConfig.json");
        PLCManager.Instance.Connect();

        // Sử dụng
        UsePLC();

        PLCManager.Instance.Disconnect();
    }
}
```

### Bước 2: Sử dụng địa chỉ động

```csharp
void UsePLC()
{
    PLCKeyence plc = PLCManager.Instance.PLC;
    PLCAddressProvider addr = PLCManager.Instance.Addresses;

    // ✅ Địa chỉ load từ JSON - THAY ĐỔI JSON = THAY ĐỔI ĐỊA CHỈ
    string sensorAddr = addr.GetInputAddress(
        PLCAddressProvider.InputNames.Cam_bien_va_cham);

    bool sensor = plc.ReadBit(sensorAddr);
    Console.WriteLine($"Sensor ({sensorAddr}): {sensor}");
}
```

### Bước 3: Thay đổi địa chỉ

**Mở `PLCConfig.json`:**
```json
{
  "Addresses": {
    "Input": [
      {
        "Name": "Cam_bien_va_cham",
        "DisplayName": "Cảm biến va chạm",
        "DataType": "Bool",
        "Address": "R0"    ← Thay đổi thành "R10"
      }
    ]
  }
}
```

**Restart app → Địa chỉ mới áp dụng ngay!** ✅

---

## Tài liệu

| File | Mô tả | Ưu tiên |
|------|-------|---------|
| **[HUONG_DAN_DYNAMIC_ADDRESS.md](HUONG_DAN_DYNAMIC_ADDRESS.md)** | **Hướng dẫn địa chỉ động** ⭐ | ⭐⭐⭐ |
| [QUICKSTART.md](QUICKSTART.md) | Bắt đầu nhanh 3 bước | ⭐⭐⭐ |
| [HUONG_DAN_PLC_MANAGER.md](HUONG_DAN_PLC_MANAGER.md) | Hướng dẫn PLCManager | ⭐⭐ |
| [HUONG_DAN_SU_DUNG.md](HUONG_DAN_SU_DUNG.md) | Hướng dẫn tổng quan | ⭐⭐ |

---

## Ví dụ code

| File | Số ví dụ | Nội dung |
|------|----------|----------|
| **[DynamicAddressExample.cs](PLCKeygen/DynamicAddressExample.cs)** | **9 ví dụ** | **Địa chỉ động** ⭐ |
| [PLCEnumUsageExample.cs](PLCKeygen/PLCEnumUsageExample.cs) | 6 ví dụ | Sử dụng Enum |
| [PLCManagerUsageExample.cs](PLCKeygen/PLCManagerUsageExample.cs) | 7 ví dụ | PLCManager |
| [ProgramExamples.cs](PLCKeygen/ProgramExamples.cs) | 2 ví dụ | Program.cs mẫu |

---

## So sánh 3 cách

### ❌ Cách 1: Hard-code

```csharp
bool sensor = plc.ReadBit("R0");
```
- ❌ Thay đổi địa chỉ → Sửa code → Rebuild
- ❌ Dễ gõ sai

### ⚠️ Cách 2: Dùng const

```csharp
// PLCAddresses.Generated.cs
public const string Cam_bien_va_cham = "R0";  // HARD-CODE!

// Sử dụng
bool sensor = plc.ReadBit(PLCAddresses.Input.Cam_bien_va_cham);
```
- ❌ Thay đổi địa chỉ → Generate → Rebuild
- ✅ Có IntelliSense

### ✅ Cách 3: Dynamic Address (KHUYẾN NGHỊ)

```csharp
PLCAddressProvider addr = PLCManager.Instance.Addresses;

string address = addr.GetInputAddress(
    PLCAddressProvider.InputNames.Cam_bien_va_cham);

bool sensor = plc.ReadBit(address);
```
- ✅ Thay đổi JSON → **Restart app** → Xong!
- ✅ **KHÔNG CẦN REBUILD**
- ✅ Có IntelliSense
- ✅ Lấy được DisplayName tiếng Việt

---

## File cấu trúc

### Core Files

| File | Mô tả |
|------|-------|
| `PLCManager.cs` | Singleton Manager quản lý config & connection |
| `PLCAddressProvider.cs` | **Provider địa chỉ động từ JSON** ⭐ |
| `PLCConfigManager.cs` | Quản lý load/parse JSON |
| `PLCConfigModels.cs` | Models cho config |

### Config Files

| File | Mô tả | Sửa được? |
|------|-------|-----------|
| `PLCConfig.json` | **Config chính** | ✅ **CÓ** |
| `PLCConfig.Sample.json` | Config mẫu | Tham khảo |

---

## API chính

### PLCManager

```csharp
// Singleton instance
PLCManager.Instance

// Khởi tạo & load config
PLCManager.Instance.Initialize("PLCConfig.json")

// Kết nối PLC
PLCManager.Instance.Connect()

// Lấy PLC instance
PLCKeyence plc = PLCManager.Instance.PLC

// Lấy Address Provider
PLCAddressProvider addr = PLCManager.Instance.Addresses

// Ngắt kết nối
PLCManager.Instance.Disconnect()
```

### PLCAddressProvider

```csharp
PLCAddressProvider addr = PLCManager.Instance.Addresses;

// Lấy địa chỉ Input
string addr = addr.GetInputAddress("Cam_bien_va_cham");

// Lấy địa chỉ Output
string addr = addr.GetOutputAddress("Den_bao_xanh");

// Lấy địa chỉ Data
string addr = addr.GetDataAddress("So_luong_san_pham_OK");

// Lấy thông tin đầy đủ
PLCAddressInfo info = addr.GetInputInfo("Cam_bien_va_cham");
Console.WriteLine(info.DisplayName);  // "Cảm biến va chạm"

// Tìm kiếm
var results = addr.SearchByDisplayName("động cơ");
```

### Constant Names (tránh magic string)

```csharp
// Input
PLCAddressProvider.InputNames.Cam_bien_va_cham
PLCAddressProvider.InputNames.Nut_start
PLCAddressProvider.InputNames.Toc_do_dong_co

// Output
PLCAddressProvider.OutputNames.Den_bao_xanh
PLCAddressProvider.OutputNames.Dong_co_chinh
PLCAddressProvider.OutputNames.Van_khi_1

// Data
PLCAddressProvider.DataNames.So_luong_san_pham_OK
PLCAddressProvider.DataNames.Trang_thai_he_thong
```

---

## Ví dụ hoàn chỉnh

### Console Application

```csharp
using System;
using PLCKeygen;

class Program
{
    static void Main()
    {
        // Khởi tạo
        PLCManager.Instance.Initialize("PLCConfig.json");
        PLCManager.Instance.Connect();

        PLCKeyence plc = PLCManager.Instance.PLC;
        PLCAddressProvider addr = PLCManager.Instance.Addresses;

        // Đọc sensor (địa chỉ từ JSON)
        bool sensor = plc.ReadBit(
            addr.GetInputAddress(PLCAddressProvider.InputNames.Cam_bien_va_cham));

        Console.WriteLine($"Sensor: {sensor}");

        // Bật đèn (địa chỉ từ JSON)
        plc.SetBit(
            addr.GetOutputAddress(PLCAddressProvider.OutputNames.Den_bao_xanh));

        // Đọc số lượng (địa chỉ từ JSON)
        int count = plc.ReadInt32(
            addr.GetDataAddress(PLCAddressProvider.DataNames.So_luong_san_pham_OK));

        Console.WriteLine($"Count: {count}");

        PLCManager.Instance.Disconnect();
    }
}
```

### Windows Forms

```csharp
// Program.cs
[STAThread]
static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);

    // Khởi tạo
    PLCManager.Instance.Initialize("PLCConfig.json");
    PLCManager.Instance.Connect();

    Application.Run(new Form1());

    PLCManager.Instance.Disconnect();
}

// Form1.cs
private void Timer_Tick(object sender, EventArgs e)
{
    PLCKeyence plc = PLCManager.Instance.PLC;
    PLCAddressProvider addr = PLCManager.Instance.Addresses;

    // Đọc và hiển thị (địa chỉ từ JSON)
    bool motor = plc.ReadBit(
        addr.GetOutputAddress(PLCAddressProvider.OutputNames.Dong_co_chinh));

    lblMotor.Text = motor ? "CHẠY" : "DỪNG";
}

private void btnStart_Click(object sender, EventArgs e)
{
    PLCKeyence plc = PLCManager.Instance.PLC;
    PLCAddressProvider addr = PLCManager.Instance.Addresses;

    // Điều khiển (địa chỉ từ JSON)
    plc.SetBit(
        addr.GetOutputAddress(PLCAddressProvider.OutputNames.Dong_co_chinh));
}
```

---

## Workflow thay đổi địa chỉ

```
┌─────────────────────────────────────────┐
│ 1. Mở PLCConfig.json                    │
│    Sửa: "Address": "R0" -> "R10"        │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│ 2. Lưu file JSON                        │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│ 3. Restart ứng dụng                     │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│ 4. ✅ Địa chỉ mới được áp dụng!         │
│    KHÔNG CẦN REBUILD!                   │
└─────────────────────────────────────────┘
```

---

## Chạy ví dụ

### Chạy tất cả ví dụ Dynamic Address

```csharp
using PLCKeygen;

class Program
{
    static void Main()
    {
        // Chạy tất cả 9 ví dụ
        DynamicAddressExample.RunAllExamples();
    }
}
```

### Chạy từng ví dụ

```csharp
// Ví dụ 1: Basic
DynamicAddressExample.Example1_BasicDynamicAddress();

// Ví dụ 2: Dùng constant names
DynamicAddressExample.Example2_UseConstantNames();

// Ví dụ 3: Lấy thông tin đầy đủ
DynamicAddressExample.Example3_GetFullAddressInfo();

// Ví dụ 4: Tìm kiếm
DynamicAddressExample.Example4_SearchByDisplayName();

// ... và 5 ví dụ khác
```

---

## Troubleshooting

### ❌ Lỗi: "Không tìm thấy địa chỉ"

**Nguyên nhân:** Tên địa chỉ sai hoặc không có trong JSON

**Giải pháp:**
```csharp
// Kiểm tra trước
if (addr.HasInput("Cam_bien_va_cham"))
{
    string address = addr.GetInputAddress("Cam_bien_va_cham");
}
```

### ❌ Lỗi: "Config chưa được load"

**Giải pháp:**
```csharp
PLCManager.Instance.Initialize("PLCConfig.json");
```

### ❌ Thay đổi JSON nhưng địa chỉ vẫn cũ

**Giải pháp:** Restart ứng dụng hoặc:
```csharp
PLCManager.Instance.ReloadConfig("PLCConfig.json");
```

---

## Lợi ích

### 🎯 Cho Developer

- ✅ Code sạch, dễ bảo trì
- ✅ Không cần rebuild khi thay đổi địa chỉ
- ✅ IntelliSense đầy đủ
- ✅ Type-safe với constant names

### 🏭 Cho Production

- ✅ Deploy một lần, config nhiều lần
- ✅ Khách hàng tự config địa chỉ PLC
- ✅ Linh hoạt với nhiều PLC khác nhau
- ✅ Không cần source code để thay đổi

### 💰 Tiết kiệm

- ✅ Không cần rebuild/release lại
- ✅ Giảm thời gian deployment
- ✅ Giảm risk khi thay đổi
- ✅ Dễ maintain

---

## Best Practices

### ✅ NÊN

1. Dùng `PLCAddressProvider` thay vì hard-code
2. Dùng constant names (`PLCAddressProvider.InputNames.xxx`)
3. Kiểm tra `HasInput()` trước khi dùng
4. Load config trong `Program.cs` (1 lần)
5. Sử dụng `PLCManager.Instance` ở mọi nơi

### ❌ KHÔNG NÊN

1. Hard-code địa chỉ trong code
2. Tạo nhiều instance PLCKeyence
3. Bỏ qua kiểm tra `IsConnected`
4. Quên `Disconnect()` khi thoát
5. Dùng magic string thay vì constant names

---

## Tóm tắt

| Tính năng | Mô tả |
|-----------|-------|
| **Dynamic Address** | Thay đổi JSON → Restart → Xong! |
| **PLCManager** | Singleton quản lý config & connection |
| **PLCAddressProvider** | Load địa chỉ động từ JSON |
| **Constant Names** | Tránh magic string, có IntelliSense |
| **No Rebuild** | **KHÔNG CẦN REBUILD KHI ĐỔI ĐỊA CHỈ** ⭐ |

---

## Bắt đầu ngay

1. Đọc [QUICKSTART.md](QUICKSTART.md) - Bắt đầu nhanh 3 bước
2. Đọc [HUONG_DAN_DYNAMIC_ADDRESS.md](HUONG_DAN_DYNAMIC_ADDRESS.md) - Hướng dẫn chi tiết
3. Chạy [DynamicAddressExample.cs](PLCKeygen/DynamicAddressExample.cs) - 9 ví dụ

**Thay đổi địa chỉ PLC chưa bao giờ dễ dàng đến thế!** 🚀

---

Made with ❤️ for flexible PLC configuration
