# HƯỚNG DẪN SỬ DỤNG HỆ THỐNG PLC CONFIG

## Mục lục
1. [Giới thiệu](#giới-thiệu)
2. [File PLCConfig.json](#file-plcconfigjson)
3. [Các Enum được sinh ra](#các-enum-được-sinh-ra)
4. [Cách sử dụng](#cách-sử-dụng)
5. [Ví dụ thực tế](#ví-dụ-thực-tế)

---

## Giới thiệu

Hệ thống PLC Config giúp bạn quản lý các địa chỉ PLC một cách có tổ chức, dễ bảo trì và an toàn về mặt kiểu dữ liệu (type-safe).

### Lợi ích:
- **Tập trung quản lý**: Tất cả địa chỉ PLC được định nghĩa trong một file JSON duy nhất
- **Type-safe**: Sử dụng Enum để tránh lỗi gõ sai địa chỉ
- **IntelliSense**: Hỗ trợ gợi ý code tự động trong Visual Studio
- **Dễ bảo trì**: Khi thay đổi địa chỉ, chỉ cần sửa file JSON và regenerate
- **Tên tiếng Việt**: Hỗ trợ tên hiển thị tiếng Việt có dấu

---

## File PLCConfig.json

### Cấu trúc cơ bản

```json
{
  "PLCName": "Tên PLC của bạn",
  "IPAddress": "192.168.1.100",
  "Port": 8501,
  "Addresses": {
    "Input": [ /* Danh sách địa chỉ Input */ ],
    "Output": [ /* Danh sách địa chỉ Output */ ],
    "Data": [ /* Danh sách địa chỉ Data */ ]
  }
}
```

### Thông tin kết nối PLC

```json
"PLCName": "Keyence_PLC_Main",
"IPAddress": "192.168.1.100",
"Port": 8501
```

- **PLCName**: Tên nhận dạng PLC (dùng trong code và comments)
- **IPAddress**: Địa chỉ IP của PLC
- **Port**: Cổng kết nối (mặc định Keyence là 8501)

### Định nghĩa địa chỉ

Mỗi địa chỉ PLC có 4 thuộc tính:

```json
{
  "Name": "Cam_bien_va_cham",
  "DisplayName": "Cảm biến va chạm",
  "DataType": "Bool",
  "Address": "R0"
}
```

| Thuộc tính | Mô tả | Ví dụ |
|------------|-------|-------|
| **Name** | Tên biến không dấu (dùng làm tên Enum) | `Cam_bien_va_cham` |
| **DisplayName** | Tên hiển thị tiếng Việt có dấu | `Cảm biến va chạm` |
| **DataType** | Kiểu dữ liệu | `Bool`, `UInt16`, `Int16`, `Int32`, `UInt32` |
| **Address** | Địa chỉ trong PLC | `R0`, `DM100`, ... |

### Phân loại địa chỉ

#### 1. Input (Đầu vào)
Các tín hiệu từ sensor, nút nhấn, công tắc đọc vào PLC.

```json
"Input": [
  {
    "Name": "Cam_bien_quang",
    "DisplayName": "Cảm biến quang",
    "DataType": "Bool",
    "Address": "R1"
  },
  {
    "Name": "Toc_do_dong_co",
    "DisplayName": "Tốc độ động cơ",
    "DataType": "UInt16",
    "Address": "DM100"
  }
]
```

#### 2. Output (Đầu ra)
Các tín hiệu điều khiển từ PLC ra thiết bị (đèn, động cơ, van...).

```json
"Output": [
  {
    "Name": "Den_bao_xanh",
    "DisplayName": "Đèn báo xanh",
    "DataType": "Bool",
    "Address": "R500"
  },
  {
    "Name": "Toc_do_dat_dong_co",
    "DisplayName": "Tốc độ đặt động cơ",
    "DataType": "UInt16",
    "Address": "DM500"
  }
]
```

#### 3. Data (Dữ liệu)
Các vùng nhớ dùng để lưu trữ dữ liệu, trạng thái, đếm số lượng...

```json
"Data": [
  {
    "Name": "So_luong_san_pham_OK",
    "DisplayName": "Số lượng sản phẩm OK",
    "DataType": "Int32",
    "Address": "DM1000"
  },
  {
    "Name": "Trang_thai_he_thong",
    "DisplayName": "Trạng thái hệ thống",
    "DataType": "UInt16",
    "Address": "DM2000"
  }
]
```

---

## Các Enum được sinh ra

### 1. InputAddress Enum

```csharp
public enum InputAddress
{
    [Description("R0")]
    [DisplayName("Cảm biến va chạm")]
    Cam_bien_va_cham,

    [Description("R1")]
    [DisplayName("Cảm biến quang đầu vào")]
    Cam_bien_quang_dau_vao,

    // ... các địa chỉ khác
}
```

### 2. OutputAddress Enum

```csharp
public enum OutputAddress
{
    [Description("R500")]
    [DisplayName("Đèn báo xanh")]
    Den_bao_xanh,

    [Description("R510")]
    [DisplayName("Động cơ chính")]
    Dong_co_chinh,

    // ... các địa chỉ khác
}
```

### 3. DataAddress Enum

```csharp
public enum DataAddress
{
    [Description("DM1000")]
    [DisplayName("Số lượng sản phẩm OK")]
    So_luong_san_pham_OK,

    [Description("DM2000")]
    [DisplayName("Trạng thái hệ thống")]
    Trang_thai_he_thong,

    // ... các địa chỉ khác
}
```

### 4. RunMode Enum

```csharp
public enum RunMode : ushort
{
    Stop = 0,
    Manual = 1,
    Auto = 2
}
```

### Extension Methods

Mỗi enum có 2 extension methods:

```csharp
// Lấy địa chỉ PLC thực tế
string address = InputAddress.Cam_bien_va_cham.GetAddress();  // Trả về "R0"

// Lấy tên hiển thị tiếng Việt
string displayName = InputAddress.Cam_bien_va_cham.GetDisplayName();  // Trả về "Cảm biến va chạm"
```

---

## Cách sử dụng

### Phương pháp 1: Sử dụng Enum (Khuyến nghị)

```csharp
using PLCKeygen;

// Kết nối PLC
PLCKeyence plc = new PLCKeyence("192.168.1.100", 8501);
plc.Open();
plc.StartCommunication();

// Đọc Input sử dụng Enum
bool sensor = plc.ReadBit(InputAddress.Cam_bien_va_cham.GetAddress());
ushort speed = plc.ReadUInt16(InputAddress.Toc_do_dong_co.GetAddress());

// Ghi Output sử dụng Enum
plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());
plc.WriteUInt16(OutputAddress.Toc_do_dat_dong_co.GetAddress(), 1500);

// Đọc/Ghi Data
int productCount = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
plc.WriteInt32(DataAddress.So_luong_san_pham_OK.GetAddress(), productCount + 1);

plc.Close();
```

**Ưu điểm:**
- IntelliSense hỗ trợ gợi ý
- Tránh lỗi gõ sai địa chỉ
- Code dễ đọc, dễ hiểu
- Type-safe

### Phương pháp 2: Sử dụng PLCAddresses (Generated class)

```csharp
using PLCKeygen;

// Kết nối PLC sử dụng thông tin từ config
PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
plc.Open();
plc.StartCommunication();

// Đọc Input
bool sensor = plc.ReadBit(PLCAddresses.Input.Cam_bien_va_cham);

// Ghi Output
plc.SetBit(PLCAddresses.Output.Den_bao_xanh);

// Đọc Data
int productCount = plc.ReadInt32(PLCAddresses.Data.So_luong_san_pham_OK);

plc.Close();
```

**Ưu điểm:**
- Code ngắn gọn hơn
- Vẫn có IntelliSense support
- Không cần gọi GetAddress()

### Phương pháp 3: Sử dụng PLCConfigManager

```csharp
using PLCKeygen;

// Load config từ file
PLCConfigManager configManager = new PLCConfigManager();
if (configManager.LoadConfig("PLCConfig.json"))
{
    // Tạo PLC instance từ config
    PLCKeyence plc = configManager.CreatePLCInstance();

    plc.Open();
    plc.StartCommunication();

    // Lấy thông tin địa chỉ
    PLCAddressInfo addrInfo = configManager.GetAddressInfo("Input", "Cam_bien_va_cham");
    Console.WriteLine($"{addrInfo.DisplayName} - {addrInfo.Address}");

    plc.Close();
}
```

---

## Ví dụ thực tế

### Ví dụ 1: Đọc trạng thái tất cả cảm biến

```csharp
public void ReadAllSensors()
{
    PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
    plc.Open();
    plc.StartCommunication();

    try
    {
        // Danh sách các sensor cần đọc
        InputAddress[] sensors = new InputAddress[]
        {
            InputAddress.Cam_bien_va_cham,
            InputAddress.Cam_bien_quang_dau_vao,
            InputAddress.Cam_bien_quang_dau_ra,
            InputAddress.Cam_bien_tu
        };

        Console.WriteLine("Trạng thái cảm biến:");
        foreach (var sensor in sensors)
        {
            bool value = plc.ReadBit(sensor.GetAddress());
            string status = value ? "BẬT" : "TẮT";
            Console.WriteLine($"[{status}] {sensor.GetDisplayName()}");
        }
    }
    finally
    {
        plc.Close();
    }
}
```

### Ví dụ 2: Điều khiển động cơ

```csharp
public void ControlMotor(bool start, ushort speed)
{
    PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
    plc.Open();
    plc.StartCommunication();

    try
    {
        if (start)
        {
            // Đặt tốc độ
            plc.WriteUInt16(OutputAddress.Toc_do_dat_dong_co.GetAddress(), speed);

            // Bật động cơ
            plc.SetBit(OutputAddress.Dong_co_chinh.GetAddress());

            // Bật đèn xanh
            plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());
            plc.ResetBit(OutputAddress.Den_bao_do.GetAddress());

            Console.WriteLine($"Động cơ đã BẬT với tốc độ {speed} RPM");
        }
        else
        {
            // Tắt động cơ
            plc.ResetBit(OutputAddress.Dong_co_chinh.GetAddress());

            // Bật đèn vàng
            plc.SetBit(OutputAddress.Den_bao_vang.GetAddress());
            plc.ResetBit(OutputAddress.Den_bao_xanh.GetAddress());

            Console.WriteLine("Động cơ đã TẮT");
        }
    }
    finally
    {
        plc.Close();
    }
}
```

### Ví dụ 3: Giám sát sản xuất

```csharp
public void MonitorProduction()
{
    PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
    plc.Open();
    plc.StartCommunication();

    try
    {
        // Đọc dữ liệu
        int productOK = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
        int productNG = plc.ReadInt32(DataAddress.So_luong_san_pham_NG.GetAddress());
        int totalProduct = plc.ReadInt32(DataAddress.Tong_san_pham.GetAddress());
        ushort efficiency = plc.ReadUInt16(DataAddress.Hieu_suat.GetAddress());
        ushort oee = plc.ReadUInt16(DataAddress.OEE.GetAddress());

        // Hiển thị
        Console.WriteLine("=== BÁO CÁO SẢN XUẤT ===");
        Console.WriteLine($"Sản phẩm OK: {productOK}");
        Console.WriteLine($"Sản phẩm NG: {productNG}");
        Console.WriteLine($"Tổng: {totalProduct}");
        Console.WriteLine($"Hiệu suất: {efficiency}%");
        Console.WriteLine($"OEE: {oee}%");

        // Tính tỷ lệ NG
        if (totalProduct > 0)
        {
            double ngRate = (double)productNG / totalProduct * 100;
            Console.WriteLine($"Tỷ lệ NG: {ngRate:F2}%");

            // Cảnh báo nếu tỷ lệ NG cao
            if (ngRate > 5.0)
            {
                plc.SetBit(OutputAddress.Den_bao_do.GetAddress());
                plc.SetBit(OutputAddress.Coi_bao_dong.GetAddress());
                Console.WriteLine("CẢNH BÁO: Tỷ lệ NG cao!");
            }
        }
    }
    finally
    {
        plc.Close();
    }
}
```

### Ví dụ 4: Sequence tự động

```csharp
public void AutoSequence()
{
    PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
    plc.Open();
    plc.StartCommunication();

    try
    {
        // Đặt chế độ Auto
        plc.WriteUInt16(DataAddress.Che_do_van_hanh.GetAddress(), (ushort)RunMode.Auto);

        // Bước 1: Xi lanh hạ
        plc.SetBit(OutputAddress.Xi_lanh_ha.GetAddress());
        System.Threading.Thread.Sleep(500);

        // Bước 2: Kẹp
        plc.SetBit(OutputAddress.Xi_lanh_kep.GetAddress());
        System.Threading.Thread.Sleep(300);

        // Bước 3: Nâng
        plc.ResetBit(OutputAddress.Xi_lanh_ha.GetAddress());
        plc.SetBit(OutputAddress.Xi_lanh_nang.GetAddress());
        System.Threading.Thread.Sleep(500);

        // Bước 4: Đẩy
        plc.SetBit(OutputAddress.Xi_lanh_day.GetAddress());
        System.Threading.Thread.Sleep(500);

        // Bước 5: Thả
        plc.ResetBit(OutputAddress.Xi_lanh_kep.GetAddress());
        System.Threading.Thread.Sleep(200);

        // Bước 6: Kéo về
        plc.ResetBit(OutputAddress.Xi_lanh_day.GetAddress());
        plc.SetBit(OutputAddress.Xi_lanh_keo.GetAddress());
        System.Threading.Thread.Sleep(500);

        // Hoàn thành
        Console.WriteLine("Sequence hoàn thành!");

        // Tăng đếm sản phẩm
        int count = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
        plc.WriteInt32(DataAddress.So_luong_san_pham_OK.GetAddress(), count + 1);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi: {ex.Message}");
        plc.SetBit(OutputAddress.Coi_bao_dong.GetAddress());
    }
    finally
    {
        plc.Close();
    }
}
```

### Ví dụ 5: Sử dụng trong Windows Forms

```csharp
public partial class Form1 : Form
{
    private PLCKeyence plc;

    private void Form1_Load(object sender, EventArgs e)
    {
        // Kết nối PLC
        plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
        plc.Open();
        plc.StartCommunication();

        // Timer để cập nhật UI
        timer1.Interval = 100; // 100ms
        timer1.Start();
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        try
        {
            // Đọc và cập nhật UI
            bool motorRunning = plc.ReadBit(OutputAddress.Dong_co_chinh.GetAddress());
            lblMotorStatus.Text = motorRunning ? "ĐANG CHẠY" : "DỪNG";
            lblMotorStatus.BackColor = motorRunning ? Color.LightGreen : Color.LightGray;

            // Đọc tốc độ
            ushort speed = plc.ReadUInt16(InputAddress.Toc_do_dong_co.GetAddress());
            lblSpeed.Text = $"{speed} RPM";

            // Đọc số lượng
            int count = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
            lblProductCount.Text = count.ToString();
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"Lỗi: {ex.Message}";
        }
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        plc.SetBit(OutputAddress.Dong_co_chinh.GetAddress());
        plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());
    }

    private void btnStop_Click(object sender, EventArgs e)
    {
        plc.ResetBit(OutputAddress.Dong_co_chinh.GetAddress());
        plc.SetBit(OutputAddress.Den_bao_do.GetAddress());
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        timer1.Stop();
        plc?.Close();
    }
}
```

---

## Regenerate PLCAddresses.Generated.cs

Khi bạn thay đổi file `PLCConfig.json`, bạn cần regenerate file `PLCAddresses.Generated.cs`:

```csharp
PLCConfigManager configManager = new PLCConfigManager();

if (configManager.LoadConfig("PLCConfig.json"))
{
    if (configManager.GenerateAddressesFile("PLCAddresses.Generated.cs"))
    {
        Console.WriteLine("Đã generate file thành công!");
        Console.WriteLine("Bạn cần rebuild project để áp dụng thay đổi!");
    }
}
```

**Lưu ý:** Sau khi regenerate, bạn phải **rebuild project** để C# compiler nhận diện các constants mới.

---

## Lưu ý quan trọng

1. **Không sửa trực tiếp file PLCAddresses.Generated.cs** - File này được tạo tự động
2. **Tên biến không dấu** - Trường `Name` trong JSON phải không có dấu tiếng Việt
3. **Địa chỉ phải đúng** - Kiểm tra địa chỉ PLC trước khi thêm vào config
4. **Kiểu dữ liệu phải khớp** - DataType phải khớp với địa chỉ PLC thực tế
5. **Rebuild sau khi regenerate** - Rebuild project sau khi tạo lại file generated

---

## Các file trong hệ thống

| File | Mô tả | Có thể sửa? |
|------|-------|-------------|
| `PLCConfig.json` | File cấu hình chính | ✅ CÓ |
| `PLCConfigModels.cs` | Models cho config | ❌ KHÔNG |
| `PLCConfigManager.cs` | Quản lý load/generate config | ❌ KHÔNG |
| `PLCAddresses.Generated.cs` | Constants được generate | ❌ KHÔNG (auto-generated) |
| `PLCAddressEnums.cs` | Enum definitions | ❌ KHÔNG |
| `PLCUsageExample.cs` | Ví dụ sử dụng basic | ✅ Tham khảo |
| `PLCEnumUsageExample.cs` | Ví dụ sử dụng Enum | ✅ Tham khảo |

---

## Hỗ trợ

Nếu bạn cần thêm địa chỉ PLC mới:
1. Mở file `PLCConfig.json`
2. Thêm địa chỉ vào mục tương ứng (Input/Output/Data)
3. Chạy regenerate script
4. Rebuild project
5. Sử dụng Enum mới trong code

Chúc bạn làm việc hiệu quả! 🚀
