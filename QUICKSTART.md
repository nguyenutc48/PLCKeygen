# QUICK START - PLC Config System

## Bắt đầu nhanh trong 3 bước

### Bước 1: Chuẩn bị file PLCConfig.json

Tạo file `PLCConfig.json` hoặc copy từ `PLCConfig.Sample.json`:

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

### Bước 2: Sử dụng Enum trong code

```csharp
using PLCKeygen;

// Kết nối PLC
PLCKeyence plc = new PLCKeyence("192.168.1.100", 8501);
plc.Open();
plc.StartCommunication();

// Đọc Input - Sử dụng Enum
bool sensor = plc.ReadBit(InputAddress.Sensor_Start.GetAddress());
Console.WriteLine($"Sensor: {sensor}");

// Ghi Output - Sử dụng Enum
plc.SetBit(OutputAddress.Green_Light.GetAddress());
Console.WriteLine("Đã bật đèn xanh");

// Đọc/Ghi Data - Sử dụng Enum
int count = plc.ReadInt32(DataAddress.Product_Count.GetAddress());
plc.WriteInt32(DataAddress.Product_Count.GetAddress(), count + 1);
Console.WriteLine($"Số sản phẩm: {count + 1}");

plc.Close();
```

### Bước 3: Chạy thử

Chạy một trong các ví dụ sau:

```csharp
// Ví dụ cơ bản
PLCEnumUsageExample.Example1_UseEnumWithExtensions();

// Kiểm tra trạng thái
PLCEnumUsageExample.Example2_CheckSystemStatus();

// Điều khiển động cơ
PLCEnumUsageExample.Example3_ControlMotorAndValve();

// Giám sát sản xuất
PLCEnumUsageExample.Example4_MonitorProductionData();

// Quét tất cả sensor
PLCEnumUsageExample.Example5_ScanAllSensors();

// Sequence tự động
PLCEnumUsageExample.Example6_AutoSequence();
```

---

## So sánh 3 cách sử dụng

### ❌ Cách 1: Hard-code (KHÔNG khuyến nghị)

```csharp
bool sensor = plc.ReadBit("R0");  // Dễ gõ sai, không có IntelliSense
```

### ✅ Cách 2: Sử dụng Constants (Tốt)

```csharp
bool sensor = plc.ReadBit(PLCAddresses.Input.Sensor_Start);  // Có IntelliSense
```

### ⭐ Cách 3: Sử dụng Enum (KHUYẾN NGHỊ)

```csharp
bool sensor = plc.ReadBit(InputAddress.Sensor_Start.GetAddress());  // Type-safe + IntelliSense
string displayName = InputAddress.Sensor_Start.GetDisplayName();    // Lấy tên tiếng Việt
```

---

## Các Enum có sẵn

| Enum | Mô tả | Extension Methods |
|------|-------|-------------------|
| `InputAddress` | Địa chỉ đầu vào | `.GetAddress()`, `.GetDisplayName()` |
| `OutputAddress` | Địa chỉ đầu ra | `.GetAddress()`, `.GetDisplayName()` |
| `DataAddress` | Địa chỉ dữ liệu | `.GetAddress()`, `.GetDisplayName()` |
| `RunMode` | Chế độ vận hành | `Stop`, `Manual`, `Auto` |

---

## Ví dụ đầy đủ

```csharp
using System;
using PLCKeygen;

public class Program
{
    static void Main()
    {
        // Kết nối PLC
        PLCKeyence plc = new PLCKeyence("192.168.1.100", 8501);
        plc.Open();
        plc.StartCommunication();

        try
        {
            // Đọc sensor
            bool startSensor = plc.ReadBit(InputAddress.Sensor_Start.GetAddress());
            Console.WriteLine($"{InputAddress.Sensor_Start.GetDisplayName()}: {startSensor}");

            if (startSensor)
            {
                // Bật động cơ
                plc.SetBit(OutputAddress.Motor_Main.GetAddress());

                // Bật đèn xanh
                plc.SetBit(OutputAddress.Green_Light.GetAddress());
                plc.ResetBit(OutputAddress.Red_Light.GetAddress());

                Console.WriteLine("Hệ thống đã khởi động!");

                // Tăng đếm sản phẩm
                int count = plc.ReadInt32(DataAddress.Product_Count.GetAddress());
                plc.WriteInt32(DataAddress.Product_Count.GetAddress(), count + 1);
                Console.WriteLine($"Số lượng sản phẩm: {count + 1}");
            }
            else
            {
                // Tắt động cơ
                plc.ResetBit(OutputAddress.Motor_Main.GetAddress());

                // Bật đèn đỏ
                plc.SetBit(OutputAddress.Red_Light.GetAddress());
                plc.ResetBit(OutputAddress.Green_Light.GetAddress());

                Console.WriteLine("Hệ thống đang dừng");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi: {ex.Message}");
        }
        finally
        {
            plc.Close();
        }
    }
}
```

---

## Các kiểu dữ liệu hỗ trợ

| DataType | Phương thức đọc | Phương thức ghi | Kích thước |
|----------|-----------------|-----------------|------------|
| `Bool` | `ReadBit()` | `SetBit()` / `ResetBit()` | 1 bit |
| `UInt16` | `ReadUInt16()` | `WriteUInt16()` | 2 bytes |
| `Int16` | `ReadInt16()` | `WriteInt16()` | 2 bytes |
| `Int32` | `ReadInt32()` | `WriteInt32()` | 4 bytes (2 words) |
| `UInt32` | `ReadUInt32()` | `WriteUInt32()` | 4 bytes (2 words) |

---

## Thêm địa chỉ mới

1. Mở file `PLCConfig.json`
2. Thêm địa chỉ vào mục tương ứng:

```json
{
  "Name": "New_Sensor",
  "DisplayName": "Cảm biến mới",
  "DataType": "Bool",
  "Address": "R10"
}
```

3. **Quan trọng:** File enum `PLCAddressEnums.cs` cần được cập nhật thủ công hoặc regenerate
4. Rebuild project
5. Sử dụng: `InputAddress.New_Sensor.GetAddress()`

---

## Tài liệu đầy đủ

Xem file `HUONG_DAN_SU_DUNG.md` để biết thêm chi tiết và ví dụ nâng cao.

---

## Files quan trọng

- `PLCConfig.json` - File cấu hình chính (CÓ THỂ SỬA)
- `PLCConfig.Sample.json` - File mẫu để tham khảo
- `PLCAddressEnums.cs` - Định nghĩa các Enum
- `PLCEnumUsageExample.cs` - Ví dụ sử dụng Enum (6 ví dụ)
- `HUONG_DAN_SU_DUNG.md` - Hướng dẫn chi tiết

Chúc bạn thành công! 🎉
