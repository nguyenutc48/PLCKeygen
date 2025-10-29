# HƯỚNG DẪN SỬ DỤNG DYNAMIC ADDRESS (ĐỊA CHỈ ĐỘNG)

## 🎯 Mục đích

**THAY ĐỔI ĐỊA CHỈ PLC KHÔNG CẦN REBUILD CODE!**

Khi bạn thay đổi địa chỉ trong file `PLCConfig.json`, chỉ cần **RESTART ỨNG DỤNG** là địa chỉ mới được áp dụng ngay. **KHÔNG CẦN REBUILD** hay **RELEASE** lại chương trình!

---

## ❌ Vấn đề với cách cũ

### Cách 1: Hard-code địa chỉ

```csharp
bool sensor = plc.ReadBit("R0");
```

**Nhược điểm:**
- Thay đổi địa chỉ `R0` -> `R10` → Phải sửa code → Rebuild → Release
- Dễ gõ sai địa chỉ
- Không có IntelliSense

### Cách 2: Dùng const (Generated)

```csharp
// File PLCAddresses.Generated.cs
public const string Cam_bien_va_cham = "R0";  // HARD-CODED!

// Sử dụng
bool sensor = plc.ReadBit(PLCAddresses.Input.Cam_bien_va_cham);
```

**Nhược điểm:**
- Địa chỉ được HARD-CODE trong file .cs
- Thay đổi JSON → Phải GENERATE lại → REBUILD → RELEASE
- Không linh hoạt

---

## ✅ Giải pháp: Dynamic Address

### Cách mới (KHUYẾN NGHỊ)

```csharp
PLCAddressProvider addr = PLCManager.Instance.Addresses;

// Địa chỉ được LOAD ĐỘNG từ PLCConfig.json
string address = addr.GetInputAddress("Cam_bien_va_cham");
bool sensor = plc.ReadBit(address);
```

**Ưu điểm:**
- ✅ Thay đổi JSON → Restart app → ÁP DỤNG NGAY!
- ✅ **KHÔNG CẦN REBUILD**
- ✅ **KHÔNG CẦN RELEASE LẠI**
- ✅ Có IntelliSense (với constant names)
- ✅ Lấy được DisplayName tiếng Việt
- ✅ Tránh gõ sai tên

---

## 🚀 Cách sử dụng

### Bước 1: Khởi tạo (1 lần trong Program.cs)

```csharp
using PLCKeygen;

class Program
{
    static void Main()
    {
        // Khởi tạo - Load config & addresses từ JSON
        PLCManager.Instance.Initialize("PLCConfig.json");
        PLCManager.Instance.Connect();

        // Sử dụng trong code
        UseWithDynamicAddress();

        PLCManager.Instance.Disconnect();
    }
}
```

### Bước 2: Sử dụng trong code

```csharp
void UseWithDynamicAddress()
{
    PLCKeyence plc = PLCManager.Instance.PLC;
    PLCAddressProvider addr = PLCManager.Instance.Addresses;

    // ĐỌC INPUT - Địa chỉ load từ JSON
    string sensorAddr = addr.GetInputAddress("Cam_bien_va_cham");
    bool sensor = plc.ReadBit(sensorAddr);

    // GHI OUTPUT - Địa chỉ load từ JSON
    string lightAddr = addr.GetOutputAddress("Den_bao_xanh");
    plc.SetBit(lightAddr);

    // ĐỌC DATA - Địa chỉ load từ JSON
    string countAddr = addr.GetDataAddress("So_luong_san_pham_OK");
    int count = plc.ReadInt32(countAddr);
}
```

---

## 📝 Các phương thức của PLCAddressProvider

### 1. GetInputAddress - Lấy địa chỉ Input

```csharp
PLCAddressProvider addr = PLCManager.Instance.Addresses;

string address = addr.GetInputAddress("Cam_bien_va_cham");
// Trả về: "R0" (hoặc giá trị nào trong JSON)
```

### 2. GetOutputAddress - Lấy địa chỉ Output

```csharp
string address = addr.GetOutputAddress("Den_bao_xanh");
// Trả về: "R500" (hoặc giá trị nào trong JSON)
```

### 3. GetDataAddress - Lấy địa chỉ Data

```csharp
string address = addr.GetDataAddress("So_luong_san_pham_OK");
// Trả về: "DM1000" (hoặc giá trị nào trong JSON)
```

### 4. GetInputInfo - Lấy thông tin đầy đủ

```csharp
PLCAddressInfo info = addr.GetInputInfo("Cam_bien_va_cham");

Console.WriteLine($"Tên: {info.Name}");               // "Cam_bien_va_cham"
Console.WriteLine($"Hiển thị: {info.DisplayName}");   // "Cảm biến va chạm"
Console.WriteLine($"Địa chỉ: {info.Address}");        // "R0"
Console.WriteLine($"Kiểu: {info.DataType}");          // "Bool"
```

### 5. HasInput/HasOutput/HasData - Kiểm tra tồn tại

```csharp
if (addr.HasInput("Cam_bien_va_cham"))
{
    string address = addr.GetInputAddress("Cam_bien_va_cham");
    // Sử dụng address
}
```

### 6. GetAllInputs/GetAllOutputs/GetAllData - Lấy tất cả

```csharp
var allInputs = addr.GetAllInputs();

foreach (var input in allInputs)
{
    Console.WriteLine($"{input.Name}: {input.Address}");
}
```

### 7. SearchByDisplayName - Tìm kiếm

```csharp
var results = addr.SearchByDisplayName("động cơ");

foreach (var result in results)
{
    Console.WriteLine($"{result.DisplayName}: {result.Address}");
}
```

---

## 🎨 Sử dụng với Constant Names

Để tránh gõ sai tên (magic string), dùng constant names:

```csharp
PLCAddressProvider addr = PLCManager.Instance.Addresses;

// ✅ Dùng constant names - Có IntelliSense, tránh gõ sai
string address = addr.GetInputAddress(PLCAddressProvider.InputNames.Cam_bien_va_cham);
bool sensor = plc.ReadBit(address);

// ❌ Dùng magic string - Dễ gõ sai
string address = addr.GetInputAddress("Cam_bien_va_cham");
```

### Danh sách Constant Names

**InputNames:**
```csharp
PLCAddressProvider.InputNames.Cam_bien_va_cham
PLCAddressProvider.InputNames.Nut_start
PLCAddressProvider.InputNames.Toc_do_dong_co
// ... và 22 tên khác
```

**OutputNames:**
```csharp
PLCAddressProvider.OutputNames.Den_bao_xanh
PLCAddressProvider.OutputNames.Dong_co_chinh
PLCAddressProvider.OutputNames.Van_khi_1
// ... và 26 tên khác
```

**DataNames:**
```csharp
PLCAddressProvider.DataNames.So_luong_san_pham_OK
PLCAddressProvider.DataNames.Trang_thai_he_thong
PLCAddressProvider.DataNames.Che_do_van_hanh
// ... và 20 tên khác
```

---

## 📖 Ví dụ hoàn chỉnh

### Ví dụ 1: Đọc Input và điều khiển Output

```csharp
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

        // Đọc nút Start (địa chỉ từ JSON)
        string startBtnAddr = addr.GetInputAddress(
            PLCAddressProvider.InputNames.Nut_start);
        bool startPressed = plc.ReadBit(startBtnAddr);

        if (startPressed)
        {
            // Bật động cơ (địa chỉ từ JSON)
            string motorAddr = addr.GetOutputAddress(
                PLCAddressProvider.OutputNames.Dong_co_chinh);
            plc.SetBit(motorAddr);

            // Bật đèn xanh
            string greenLightAddr = addr.GetOutputAddress(
                PLCAddressProvider.OutputNames.Den_bao_xanh);
            plc.SetBit(greenLightAddr);

            Console.WriteLine("Hệ thống đã khởi động!");
        }

        PLCManager.Instance.Disconnect();
    }
}
```

### Ví dụ 2: Đọc dữ liệu sản xuất

```csharp
void MonitorProduction()
{
    PLCKeyence plc = PLCManager.Instance.PLC;
    PLCAddressProvider addr = PLCManager.Instance.Addresses;

    // Tất cả địa chỉ đều load từ JSON
    int productOK = plc.ReadInt32(
        addr.GetDataAddress(PLCAddressProvider.DataNames.So_luong_san_pham_OK));

    int productNG = plc.ReadInt32(
        addr.GetDataAddress(PLCAddressProvider.DataNames.So_luong_san_pham_NG));

    ushort efficiency = plc.ReadUInt16(
        addr.GetDataAddress(PLCAddressProvider.DataNames.Hieu_suat));

    Console.WriteLine($"Sản phẩm OK: {productOK}");
    Console.WriteLine($"Sản phẩm NG: {productNG}");
    Console.WriteLine($"Hiệu suất: {efficiency}%");
}
```

### Ví dụ 3: Sử dụng trong Windows Forms

```csharp
public partial class Form1 : Form
{
    private Timer monitorTimer;

    private void Form1_Load(object sender, EventArgs e)
    {
        // Timer để cập nhật UI
        monitorTimer = new Timer();
        monitorTimer.Interval = 100;
        monitorTimer.Tick += MonitorTimer_Tick;
        monitorTimer.Start();
    }

    private void MonitorTimer_Tick(object sender, EventArgs e)
    {
        if (!PLCManager.Instance.IsConnected)
            return;

        PLCKeyence plc = PLCManager.Instance.PLC;
        PLCAddressProvider addr = PLCManager.Instance.Addresses;

        // Đọc địa chỉ động từ JSON
        bool motorRunning = plc.ReadBit(
            addr.GetOutputAddress(PLCAddressProvider.OutputNames.Dong_co_chinh));

        ushort speed = plc.ReadUInt16(
            addr.GetInputAddress(PLCAddressProvider.InputNames.Toc_do_dong_co));

        int productCount = plc.ReadInt32(
            addr.GetDataAddress(PLCAddressProvider.DataNames.So_luong_san_pham_OK));

        // Cập nhật UI
        lblMotorStatus.Text = motorRunning ? "ĐANG CHẠY" : "DỪNG";
        lblSpeed.Text = $"{speed} RPM";
        lblProductCount.Text = productCount.ToString();
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        PLCKeyence plc = PLCManager.Instance.PLC;
        PLCAddressProvider addr = PLCManager.Instance.Addresses;

        // Bật động cơ - Địa chỉ từ JSON
        plc.SetBit(addr.GetOutputAddress(PLCAddressProvider.OutputNames.Dong_co_chinh));
        plc.SetBit(addr.GetOutputAddress(PLCAddressProvider.OutputNames.Den_bao_xanh));
    }
}
```

---

## 🔄 Thay đổi địa chỉ trong JSON

### Quy trình thay đổi địa chỉ

**Bước 1:** Mở file `PLCConfig.json`

```json
{
  "Addresses": {
    "Input": [
      {
        "Name": "Cam_bien_va_cham",
        "DisplayName": "Cảm biến va chạm",
        "DataType": "Bool",
        "Address": "R0"    ← Thay đổi từ "R0" thành "R10"
      }
    ]
  }
}
```

**Bước 2:** Thay đổi địa chỉ

```json
{
  "Addresses": {
    "Input": [
      {
        "Name": "Cam_bien_va_cham",
        "DisplayName": "Cảm biến va chạm",
        "DataType": "Bool",
        "Address": "R10"   ← Đã thay đổi!
      }
    ]
  }
}
```

**Bước 3:** Lưu file

**Bước 4:** RESTART ứng dụng

**Bước 5:** Địa chỉ mới được áp dụng ngay! ✅

**KHÔNG CẦN:**
- ❌ Rebuild code
- ❌ Generate lại file
- ❌ Release lại chương trình
- ❌ Deploy lại

---

## 📊 So sánh 3 cách

| Tiêu chí | Hard-code | Const (Generated) | **Dynamic** ⭐ |
|----------|-----------|-------------------|----------------|
| IntelliSense | ❌ Không | ✅ Có | ✅ Có |
| Tránh gõ sai | ❌ Không | ✅ Có | ✅ Có |
| Thay đổi địa chỉ | Sửa code → Rebuild | Generate → Rebuild | **Sửa JSON → Restart** |
| Rebuild khi đổi | ✅ Cần | ✅ Cần | ❌ **KHÔNG CẦN** |
| Release lại | ✅ Cần | ✅ Cần | ❌ **KHÔNG CẦN** |
| Linh hoạt | ❌ Thấp | ⚠️ Trung bình | ✅ **Cao** |
| DisplayName | ❌ Không | ❌ Không | ✅ **Có** |

---

## 🎯 Khi nào nên dùng Dynamic Address?

### ✅ NÊN dùng khi:

1. **Dự án production** - Không muốn rebuild khi thay đổi địa chỉ
2. **Deploy lên máy khách** - Khách hàng có thể tự config địa chỉ
3. **Nhiều PLC khác nhau** - Mỗi PLC có địa chỉ khác nhau
4. **Thay đổi thường xuyên** - Địa chỉ PLC hay thay đổi
5. **Cần flexibility** - Muốn config linh hoạt

### ⚠️ CÓ THỂ không cần khi:

1. **Dự án nhỏ** - Chỉ vài địa chỉ, ít thay đổi
2. **Địa chỉ cố định** - Địa chỉ không bao giờ thay đổi
3. **Performance critical** - Cần tốc độ tối đa (nhưng cache trong AddressProvider rất nhanh)

---

## 🔧 Advanced Usage

### 1. Reload config trong runtime

```csharp
// Reload config mà không cần restart app
PLCManager.Instance.ReloadConfig("PLCConfig.json");

// Địa chỉ mới sẽ được load ngay lập tức
PLCAddressProvider addr = PLCManager.Instance.Addresses;
string newAddress = addr.GetInputAddress("Cam_bien_va_cham");
```

### 2. Kiểm tra địa chỉ trước khi dùng

```csharp
PLCAddressProvider addr = PLCManager.Instance.Addresses;

if (addr.HasInput("Cam_bien_va_cham"))
{
    string address = addr.GetInputAddress("Cam_bien_va_cham");
    bool value = plc.ReadBit(address);
}
else
{
    Console.WriteLine("Địa chỉ không tồn tại trong config!");
}
```

### 3. Tìm địa chỉ không biết loại (Input/Output/Data)

```csharp
PLCAddressProvider addr = PLCManager.Instance.Addresses;

PLCAddressInfo info = addr.FindAddress("Cam_bien_va_cham");
Console.WriteLine($"Tìm thấy: {info.Address} - {info.DisplayName}");
```

### 4. Tìm kiếm theo DisplayName

```csharp
var results = addr.SearchByDisplayName("động cơ");

foreach (var result in results)
{
    Console.WriteLine($"{result.DisplayName}: {result.Address}");
}
// Output:
// Động cơ chính: R510
// Động cơ phụ: R511
// Động cơ quạt: R512
// ...
```

---

## 📚 Tài liệu tham khảo

- **DynamicAddressExample.cs** - 9 ví dụ chi tiết
- **PLCAddressProvider.cs** - Source code với đầy đủ XML comments
- **HUONG_DAN_PLC_MANAGER.md** - Hướng dẫn PLCManager

---

## ✅ Checklist sử dụng

- [ ] Khởi tạo `PLCManager.Instance.Initialize()` trong Program.cs
- [ ] Lấy `PLCAddressProvider` từ `PLCManager.Instance.Addresses`
- [ ] Sử dụng `GetInputAddress()`, `GetOutputAddress()`, `GetDataAddress()`
- [ ] Dùng constant names (`PLCAddressProvider.InputNames.xxx`) thay vì magic string
- [ ] Thay đổi địa chỉ trong `PLCConfig.json`
- [ ] Restart app để áp dụng thay đổi

---

## 🎉 Tổng kết

**Dynamic Address System** giúp bạn:

✅ **KHÔNG CẦN REBUILD** khi thay đổi địa chỉ
✅ **Linh hoạt** - Thay đổi JSON và restart là đủ
✅ **Type-safe** - Có IntelliSense với constant names
✅ **Dễ bảo trì** - Tất cả config tập trung trong JSON
✅ **Production-ready** - Phù hợp deploy lên máy khách

**Thay đổi địa chỉ PLC chưa bao giờ dễ dàng đến thế!** 🚀
