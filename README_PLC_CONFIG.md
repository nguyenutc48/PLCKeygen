# Hệ thống quản lý cấu hình PLC

## Tổng quan

Hệ thống này cho phép bạn quản lý cấu hình PLC thông qua file JSON và tự động generate code C# để truy cập các địa chỉ PLC một cách type-safe.

### Lợi ích:
- ✅ Quản lý tập trung tất cả địa chỉ PLC trong file JSON
- ✅ Tự động generate constants để sử dụng như enum: `PLCAddresses.Input.Cam_bien_va_cham`
- ✅ Hỗ trợ IntelliSense và auto-complete trong Visual Studio
- ✅ Phân loại rõ ràng: Input, Output, Data
- ✅ Có tên hiển thị tiếng Việt kèm theo
- ✅ Dễ dàng maintain và update

## Cấu trúc File

### 1. PLCConfig.json
File cấu hình chính chứa thông tin PLC và các địa chỉ:

```json
{
  "PLCName": "Keyence_PLC_Main",
  "IPAddress": "192.168.1.100",
  "Port": 8501,
  "Addresses": {
    "Input": [
      {
        "Name": "Cam_bien_va_cham",
        "DisplayName": "Cảm biến va chạm",
        "DataType": "Bool",
        "Address": "R0"
      }
    ],
    "Output": [...],
    "Data": [...]
  }
}
```

**Giải thích các trường:**
- `PLCName`: Tên PLC (dùng để nhận biết)
- `IPAddress`: Địa chỉ IP của PLC
- `Port`: Cổng kết nối
- `Addresses`: Danh sách các địa chỉ
  - `Input`: Địa chỉ đầu vào
  - `Output`: Địa chỉ đầu ra
  - `Data`: Địa chỉ dữ liệu
- Mỗi địa chỉ có:
  - `Name`: Tên biến (không dấu cách, dùng để generate code)
  - `DisplayName`: Tên hiển thị có dấu tiếng Việt
  - `DataType`: Kiểu dữ liệu (Bool, UInt16, Int16, Int32, UInt32)
  - `Address`: Địa chỉ trong PLC (ví dụ: R0, DM100)

### 2. PLCAddresses.Generated.cs
File được tạo tự động chứa các constants:

```csharp
public static class PLCAddresses
{
    public const string PLCName = "Keyence_PLC_Main";
    public const string IPAddress = "192.168.1.100";
    public const int Port = 8501;

    public static class Input
    {
        public const string Cam_bien_va_cham = "R0";
        public const string Cam_bien_quang = "R1";
        // ...
    }

    public static class Output
    {
        public const string Den_bao = "R500";
        // ...
    }

    public static class Data
    {
        public const string So_luong_san_pham = "DM1000";
        // ...
    }
}
```

## Hướng dẫn sử dụng

### Bước 1: Cài đặt Newtonsoft.Json

**Cách 1: Qua Visual Studio Package Manager Console**
```powershell
Install-Package Newtonsoft.Json
```

**Cách 2: Qua NuGet Package Manager GUI**
1. Click chuột phải vào Project → Manage NuGet Packages
2. Tìm "Newtonsoft.Json"
3. Click Install

**Cách 3: Restore packages (đã có packages.config)**
```bash
nuget restore
```

### Bước 2: Chỉnh sửa PLCConfig.json

Mở file `PLCConfig.json` và chỉnh sửa theo cấu hình PLC của bạn:

```json
{
  "PLCName": "Ten_PLC_Cua_Ban",
  "IPAddress": "192.168.1.XXX",
  "Port": 8501,
  "Addresses": {
    "Input": [
      {
        "Name": "Ten_khong_dau",
        "DisplayName": "Tên có dấu",
        "DataType": "Bool",
        "Address": "R0"
      }
    ]
  }
}
```

### Bước 3: Generate file PLCAddresses.Generated.cs

**Cách 1: Chạy code trong Program.cs hoặc Form_Load:**

```csharp
PLCConfigManager configManager = new PLCConfigManager();
configManager.LoadConfig("PLCConfig.json");
configManager.GenerateAddressesFile("PLCAddresses.Generated.cs");
```

**Cách 2: Tạo button trong Form để generate:**

```csharp
private void btnGenerateAddresses_Click(object sender, EventArgs e)
{
    PLCConfigManager configManager = new PLCConfigManager();

    if (configManager.LoadConfig("PLCConfig.json"))
    {
        if (configManager.GenerateAddressesFile("PLCAddresses.Generated.cs"))
        {
            MessageBox.Show("Generate thành công! Hãy rebuild project.", "Success");
        }
    }
}
```

### Bước 4: Rebuild Project

Sau khi generate file, bạn cần **Rebuild** project để Visual Studio nhận biết các constants mới.

### Bước 5: Sử dụng trong code

```csharp
// Tạo kết nối PLC
PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);

// Mở kết nối
plc.Open();
plc.StartCommunication();

// Đọc Input - Sử dụng IntelliSense
bool camBien = plc.ReadBit(PLCAddresses.Input.Cam_bien_va_cham);
ushort tocDo = plc.ReadUInt16(PLCAddresses.Input.Toc_do_dong_co);

// Ghi Output
plc.SetBit(PLCAddresses.Output.Den_bao);
plc.WriteInt16(PLCAddresses.Output.Gia_tri_dat, 1500);

// Đọc/Ghi Data
int soLuong = plc.ReadInt32(PLCAddresses.Data.So_luong_san_pham);
plc.WriteInt32(PLCAddresses.Data.So_luong_san_pham, soLuong + 1);

// Đóng kết nối
plc.Close();
```

## Ví dụ đầy đủ

Xem file `PLCUsageExample.cs` để có các ví dụ chi tiết:
- Example 1: Sử dụng địa chỉ đã generate
- Example 2: Load config và tạo PLC instance
- Example 3: Regenerate file khi thay đổi JSON
- Example 4: Lấy thông tin chi tiết của địa chỉ
- Example 5: Sử dụng trong Form

## Workflow khi thêm địa chỉ mới

1. Mở file `PLCConfig.json`
2. Thêm địa chỉ mới vào mục tương ứng (Input/Output/Data)
3. Chạy code generate để tạo lại file `PLCAddresses.Generated.cs`:
   ```csharp
   PLCConfigManager configManager = new PLCConfigManager();
   configManager.LoadConfig("PLCConfig.json");
   configManager.GenerateAddressesFile("PLCAddresses.Generated.cs");
   ```
4. Rebuild project (Ctrl + Shift + B)
5. Sử dụng địa chỉ mới: `PLCAddresses.Input.Dia_chi_moi`

## Các kiểu dữ liệu hỗ trợ

| DataType | Mô tả | Hàm đọc | Hàm ghi |
|----------|-------|---------|---------|
| Bool | Boolean (true/false) | `ReadBit()` | `SetBit()` / `ResetBit()` |
| UInt16 | Unsigned 16-bit (0-65535) | `ReadUInt16()` | `WriteUInt16()` |
| Int16 | Signed 16-bit (-32768 to 32767) | `ReadInt16()` | `WriteInt16()` |
| Int32 | Signed 32-bit | `ReadInt32()` | `WriteInt32()` |

## Tips

1. **Đặt tên địa chỉ:** Sử dụng tiếng Việt không dấu, gạch dưới thay khoảng trắng
   - ✅ Tốt: `Cam_bien_va_cham`, `Toc_do_dong_co`
   - ❌ Tránh: `camBienVaCham`, `cam-bien`

2. **Phân loại rõ ràng:**
   - Input: Các tín hiệu đầu vào từ cảm biến, nút nhấn
   - Output: Các tín hiệu điều khiển đầu ra (động cơ, đèn, van)
   - Data: Dữ liệu xử lý, counter, timer

3. **Backup config:** Luôn backup file `PLCConfig.json` trước khi thay đổi lớn

4. **Version control:** Commit cả `PLCConfig.json` và `PLCAddresses.Generated.cs` vào Git

## Troubleshooting

### Lỗi: "Type or namespace name 'JsonConvert' could not be found"
**Giải pháp:** Cài đặt Newtonsoft.Json package (xem Bước 1)

### Lỗi: "The name 'PLCAddresses' does not exist"
**Giải pháp:**
1. Kiểm tra file `PLCAddresses.Generated.cs` đã được tạo chưa
2. Rebuild project (Ctrl + Shift + B)

### File JSON không load được
**Giải pháp:**
1. Kiểm tra file `PLCConfig.json` có trong thư mục output (bin/Debug hoặc bin/Release)
2. Kiểm tra property của file JSON: `Copy to Output Directory` = `Copy if newer`

### IntelliSense không hiển thị địa chỉ mới
**Giải pháp:**
1. Rebuild project
2. Close và mở lại file code
3. Restart Visual Studio nếu vẫn không được

## Tác giả

Hệ thống được tạo cho project PLCKeygen.

## License

Free to use trong project nội bộ.
