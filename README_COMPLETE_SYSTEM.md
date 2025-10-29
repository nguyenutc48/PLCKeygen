# HỆ THỐNG QUẢN LÝ PLC - HƯỚNG DẪN ĐẦY ĐỦ

## Tổng quan hệ thống

Hệ thống này cung cấp một giải pháp hoàn chỉnh để quản lý và làm việc với PLC Keyence thông qua:
- ✅ File config JSON tập trung
- ✅ Enum type-safe cho địa chỉ PLC
- ✅ Singleton Manager để quản lý kết nối
- ✅ Auto-reconnect khi mất kết nối
- ✅ Hỗ trợ tiếng Việt có dấu

---

## Cấu trúc file trong hệ thống

### 📁 Core Files (Không sửa)

| File | Mô tả | Vai trò |
|------|-------|---------|
| `PLCConfigModels.cs` | Định nghĩa models cho config | Cấu trúc dữ liệu |
| `PLCConfigManager.cs` | Quản lý load/generate config | Load & parse JSON |
| `PLCManager.cs` | **Singleton Manager** ⭐ | Quản lý config & connection |
| `PLCAddressEnums.cs` | Định nghĩa Enum addresses | Type-safe addresses |
| `PLCAddresses.Generated.cs` | Constants (auto-generated) | Static constants |

### 📁 Configuration Files (Có thể sửa)

| File | Mô tả | Khi nào sửa |
|------|-------|-------------|
| `PLCConfig.json` | **Config chính** ⭐ | Khi thay đổi địa chỉ PLC |
| `PLCConfig.Sample.json` | Config mẫu đơn giản | Tham khảo |

### 📁 Documentation Files

| File | Mô tả | Nội dung |
|------|-------|----------|
| `README_COMPLETE_SYSTEM.md` | Tài liệu tổng hợp này | Overview toàn hệ thống |
| `HUONG_DAN_PLC_MANAGER.md` | **Hướng dẫn PLCManager** ⭐ | Cách dùng PLCManager |
| `HUONG_DAN_SU_DUNG.md` | Hướng dẫn chi tiết | Cách dùng Enum & Config |
| `QUICKSTART.md` | Bắt đầu nhanh | Quick start 3 bước |

### 📁 Example Files

| File | Mô tả | Số ví dụ |
|------|-------|----------|
| `PLCUsageExample.cs` | Ví dụ cơ bản | 5 ví dụ |
| `PLCEnumUsageExample.cs` | **Ví dụ dùng Enum** ⭐ | 6 ví dụ |
| `PLCManagerUsageExample.cs` | **Ví dụ dùng PLCManager** ⭐ | 7 ví dụ |
| `ProgramExamples.cs` | Ví dụ Program.cs | Console & WinForms |

---

## Workflow làm việc

### 🚀 Lần đầu khởi động

```
1. Chuẩn bị PLCConfig.json
   └─> Định nghĩa IP, Port, Addresses

2. Khởi tạo PLCManager trong Program.cs
   └─> PLCManager.Instance.Initialize("PLCConfig.json")
   └─> PLCManager.Instance.Connect()

3. Sử dụng PLC trong code
   └─> PLCKeyence plc = PLCManager.Instance.PLC
   └─> Đọc/Ghi với Enum addresses

4. Ngắt kết nối khi thoát
   └─> PLCManager.Instance.Disconnect()
```

### 🔄 Workflow hàng ngày

```
┌─────────────────────────────────────────┐
│  1. Mở ứng dụng                         │
│     - PLCManager tự động load config    │
│     - Tự động kết nối PLC               │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│  2. Làm việc với PLC                    │
│     - Đọc Input: plc.ReadBit(...)       │
│     - Ghi Output: plc.SetBit(...)       │
│     - Đọc Data: plc.ReadInt32(...)      │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│  3. Xử lý lỗi (nếu có)                  │
│     - AutoReconnect() tự động           │
│     - Hiển thị thông báo lỗi            │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│  4. Đóng ứng dụng                       │
│     - Disconnect() tự động              │
└─────────────────────────────────────────┘
```

---

## 3 cách sử dụng (từ cơ bản đến nâng cao)

### ❌ Cách 1: Hard-code (KHÔNG khuyến nghị)

```csharp
// Không type-safe, dễ gõ sai
PLCKeyence plc = new PLCKeyence("192.168.1.100", 8501);
plc.Open();
bool sensor = plc.ReadBit("R0");  // Dễ gõ sai "R00"
plc.Close();
```

**Nhược điểm:**
- Không có IntelliSense
- Dễ gõ sai địa chỉ
- Khó bảo trì khi thay đổi

### ⚠️ Cách 2: Dùng Constants (Tạm ổn)

```csharp
PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
plc.Open();
bool sensor = plc.ReadBit(PLCAddresses.Input.Cam_bien_va_cham);
plc.Close();
```

**Ưu điểm:**
- Có IntelliSense
- Ít gõ sai hơn

**Nhược điểm:**
- Phải tạo nhiều PLC instance
- Không quản lý connection tập trung

### ✅ Cách 3: Dùng PLCManager + Enum (KHUYẾN NGHỊ) ⭐

```csharp
// Khởi tạo 1 lần trong Program.cs
PLCManager.Instance.Initialize("PLCConfig.json");
PLCManager.Instance.Connect();

// Sử dụng ở BẤT KỲ ĐÂU trong code
PLCKeyence plc = PLCManager.Instance.PLC;
bool sensor = plc.ReadBit(InputAddress.Cam_bien_va_cham.GetAddress());
string displayName = InputAddress.Cam_bien_va_cham.GetDisplayName(); // "Cảm biến va chạm"
```

**Ưu điểm:**
- ✅ Type-safe với Enum
- ✅ IntelliSense đầy đủ
- ✅ Quản lý tập trung
- ✅ Auto-reconnect
- ✅ Singleton pattern
- ✅ Thread-safe

---

## Quick Start - 3 bước đơn giản

### Bước 1: Chuẩn bị PLCConfig.json

Tạo file `PLCConfig.json` trong thư mục project:

```json
{
  "PLCName": "My_PLC",
  "IPAddress": "192.168.1.100",
  "Port": 8501,
  "Addresses": {
    "Input": [
      {
        "Name": "Sensor_Start",
        "DisplayName": "Cảm biến Start",
        "DataType": "Bool",
        "Address": "R0"
      }
    ],
    "Output": [
      {
        "Name": "Green_Light",
        "DisplayName": "Đèn xanh",
        "DataType": "Bool",
        "Address": "R500"
      }
    ],
    "Data": [
      {
        "Name": "Product_Count",
        "DisplayName": "Số lượng sản phẩm",
        "DataType": "Int32",
        "Address": "DM1000"
      }
    ]
  }
}
```

### Bước 2: Khởi tạo trong Program.cs

**Console Application:**

```csharp
using System;
using PLCKeygen;

class Program
{
    static void Main()
    {
        // Khởi tạo
        if (!PLCManager.Instance.Initialize("PLCConfig.json"))
        {
            Console.WriteLine("Lỗi load config!");
            return;
        }

        // Kết nối
        if (!PLCManager.Instance.Connect())
        {
            Console.WriteLine("Lỗi kết nối PLC!");
            return;
        }

        // Sử dụng
        PLCKeyence plc = PLCManager.Instance.PLC;
        bool sensor = plc.ReadBit(InputAddress.Sensor_Start.GetAddress());
        Console.WriteLine($"Sensor: {sensor}");

        // Ngắt kết nối
        PLCManager.Instance.Disconnect();
    }
}
```

**Windows Forms Application:**

```csharp
using System;
using System.Windows.Forms;
using PLCKeygen;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Khởi tạo
        if (!PLCManager.Instance.Initialize("PLCConfig.json"))
        {
            MessageBox.Show("Lỗi load config!");
            return;
        }

        // Kết nối
        if (!PLCManager.Instance.Connect())
        {
            MessageBox.Show("Lỗi kết nối PLC!");
        }

        // Chạy form
        Application.Run(new Form1());

        // Ngắt kết nối
        PLCManager.Instance.Disconnect();
    }
}
```

### Bước 3: Sử dụng trong code

**Đọc Input:**
```csharp
PLCKeyence plc = PLCManager.Instance.PLC;

// Đọc Bool
bool sensor = plc.ReadBit(InputAddress.Sensor_Start.GetAddress());

// Đọc UInt16
ushort speed = plc.ReadUInt16(InputAddress.Current_Speed.GetAddress());

// Đọc Int32
int position = plc.ReadInt32(InputAddress.Current_Position.GetAddress());
```

**Ghi Output:**
```csharp
PLCKeyence plc = PLCManager.Instance.PLC;

// Ghi Bool
plc.SetBit(OutputAddress.Green_Light.GetAddress());
plc.ResetBit(OutputAddress.Red_Light.GetAddress());

// Ghi UInt16
plc.WriteUInt16(OutputAddress.Target_Speed.GetAddress(), 1500);

// Ghi Int32
plc.WriteInt32(OutputAddress.Target_Position.GetAddress(), 1000);
```

**Đọc/Ghi Data:**
```csharp
PLCKeyence plc = PLCManager.Instance.PLC;

// Đọc
int count = plc.ReadInt32(DataAddress.Product_Count.GetAddress());
ushort status = plc.ReadUInt16(DataAddress.System_Status.GetAddress());

// Ghi
plc.WriteInt32(DataAddress.Product_Count.GetAddress(), count + 1);
plc.WriteUInt16(DataAddress.System_Status.GetAddress(), 1);
```

---

## Các Enum có sẵn

### 1. InputAddress
Enum cho địa chỉ đầu vào (sensors, buttons, ...)

```csharp
InputAddress.Cam_bien_va_cham
InputAddress.Cam_bien_quang_dau_vao
InputAddress.Nut_start
InputAddress.Nut_stop
InputAddress.Toc_do_dong_co
// ... 25 địa chỉ khác
```

### 2. OutputAddress
Enum cho địa chỉ đầu ra (motors, lights, valves, ...)

```csharp
OutputAddress.Den_bao_xanh
OutputAddress.Den_bao_do
OutputAddress.Dong_co_chinh
OutputAddress.Van_khi_1
OutputAddress.Xi_lanh_nang
// ... 27 địa chỉ khác
```

### 3. DataAddress
Enum cho địa chỉ dữ liệu (counters, status, ...)

```csharp
DataAddress.So_luong_san_pham_OK
DataAddress.So_luong_san_pham_NG
DataAddress.Trang_thai_he_thong
DataAddress.Che_do_van_hanh
DataAddress.OEE
// ... 23 địa chỉ khác
```

### 4. RunMode
Enum cho chế độ vận hành

```csharp
RunMode.Stop    // 0
RunMode.Manual  // 1
RunMode.Auto    // 2
```

### Extension Methods

Mỗi enum có 2 extension methods:

```csharp
// Lấy địa chỉ PLC
string addr = InputAddress.Cam_bien_va_cham.GetAddress();  // "R0"

// Lấy tên tiếng Việt
string name = InputAddress.Cam_bien_va_cham.GetDisplayName();  // "Cảm biến va chạm"
```

---

## Các ví dụ có sẵn

### File PLCEnumUsageExample.cs

1. **Example1_UseEnumWithExtensions** - Sử dụng Enum cơ bản
2. **Example2_CheckSystemStatus** - Kiểm tra trạng thái hệ thống
3. **Example3_ControlMotorAndValve** - Điều khiển động cơ & van
4. **Example4_MonitorProductionData** - Giám sát sản xuất
5. **Example5_ScanAllSensors** - Quét tất cả sensor
6. **Example6_AutoSequence** - Sequence tự động

### File PLCManagerUsageExample.cs

1. **Example1_ConsoleApplication** - Console app
2. **Example2_WindowsFormsStartup** - Windows Forms startup
3. **UseWithPLCManager** - Sử dụng PLCManager
4. **Example4_ViewConfigInfo** - Xem thông tin config
5. **Example5_AutoReconnect** - Auto reconnect
6. **Example6_GetAddressInfo** - Lấy thông tin địa chỉ
7. **Example7_ReloadConfig** - Reload config runtime

### File ProgramExamples.cs

- **ConsoleApplicationMain** - Program.cs đầy đủ cho Console
- **WindowsFormsApplicationMain** - Program.cs đầy đủ cho WinForms

---

## Chạy ví dụ

### Console Application

```csharp
using PLCKeygen;

class Program
{
    static void Main()
    {
        // Chạy ví dụ 1
        PLCEnumUsageExample.Example1_UseEnumWithExtensions();

        // Hoặc ví dụ 6
        PLCEnumUsageExample.Example6_AutoSequence();

        // Hoặc ví dụ PLCManager
        PLCManagerUsageExample.Example1_ConsoleApplication();
    }
}
```

### Copy code mẫu

Xem file `ProgramExamples.cs` để copy code mẫu hoàn chỉnh cho:
- Console Application
- Windows Forms Application

---

## Thêm địa chỉ PLC mới

### Bước 1: Sửa PLCConfig.json

Thêm địa chỉ mới vào mục tương ứng:

```json
{
  "Input": [
    {
      "Name": "New_Sensor",
      "DisplayName": "Cảm biến mới",
      "DataType": "Bool",
      "Address": "R10"
    }
  ]
}
```

### Bước 2: Cập nhật Enum (Manual)

Mở file `PLCAddressEnums.cs` và thêm:

```csharp
public enum InputAddress
{
    // ... các địa chỉ cũ

    [Description("R10")]
    [DisplayName("Cảm biến mới")]
    New_Sensor,
}
```

### Bước 3: Rebuild project

Rebuild project để áp dụng thay đổi.

### Bước 4: Sử dụng

```csharp
bool newSensor = plc.ReadBit(InputAddress.New_Sensor.GetAddress());
```

---

## Troubleshooting

### ❌ Lỗi: "Config chưa được load!"

**Giải pháp:**
```csharp
PLCManager.Instance.Initialize("PLCConfig.json");
```

### ❌ Lỗi: "PLC chưa kết nối!"

**Giải pháp:**
```csharp
PLCManager.Instance.Connect();
```

### ❌ Lỗi: "File config không tồn tại"

**Giải pháp:**
- Kiểm tra file `PLCConfig.json` có trong thư mục project không
- Đảm bảo file được copy vào output directory
- Hoặc dùng đường dẫn tuyệt đối

### ❌ Lỗi: Mất kết nối giữa chừng

**Giải pháp:**
```csharp
if (!PLCManager.Instance.AutoReconnect())
{
    MessageBox.Show("Không thể kết nối lại!");
}
```

---

## Best Practices

### ✅ NÊN:

1. Sử dụng **PLCManager** thay vì tạo nhiều PLC instance
2. Sử dụng **Enum** thay vì hard-code địa chỉ
3. Kiểm tra **IsConnected** trước khi đọc/ghi
4. Sử dụng **AutoReconnect()** trong loop/timer
5. **Disconnect()** khi thoát ứng dụng

### ❌ KHÔNG NÊN:

1. Hard-code địa chỉ PLC trong code
2. Tạo nhiều instance PLCKeyence
3. Quên Disconnect khi thoát
4. Bỏ qua xử lý exception
5. Không kiểm tra IsConnected

---

## Tài liệu tham khảo

| Tài liệu | Nội dung | Độ ưu tiên |
|----------|----------|-----------|
| `QUICKSTART.md` | Bắt đầu nhanh 3 bước | ⭐⭐⭐ |
| `HUONG_DAN_PLC_MANAGER.md` | Hướng dẫn PLCManager chi tiết | ⭐⭐⭐ |
| `HUONG_DAN_SU_DUNG.md` | Hướng dẫn toàn bộ hệ thống | ⭐⭐ |
| `README_COMPLETE_SYSTEM.md` | Tài liệu này | ⭐ |

---

## Tóm tắt

```
┌──────────────────────────────────────────────┐
│  PLCConfig.json                              │
│  - Định nghĩa IP, Port, Addresses            │
└────────────┬─────────────────────────────────┘
             │ Load
             ▼
┌──────────────────────────────────────────────┐
│  PLCManager (Singleton)                      │
│  - Initialize(): Load config                 │
│  - Connect(): Kết nối PLC                    │
│  - AutoReconnect(): Tự động reconnect        │
└────────────┬─────────────────────────────────┘
             │ Sử dụng
             ▼
┌──────────────────────────────────────────────┐
│  Your Application                            │
│  - PLCKeyence plc = Instance.PLC             │
│  - plc.ReadBit(Enum.GetAddress())            │
│  - plc.SetBit(Enum.GetAddress())             │
└──────────────────────────────────────────────┘
```

**Workflow:**
1. Chuẩn bị `PLCConfig.json`
2. Khởi tạo `PLCManager` trong `Program.cs`
3. Sử dụng `Enum` để truy cập địa chỉ PLC
4. Tận hưởng **Type-safe** + **IntelliSense**! 🎉

---

Chúc bạn thành công! 🚀

Nếu cần hỗ trợ, xem các file ví dụ hoặc tài liệu chi tiết.
