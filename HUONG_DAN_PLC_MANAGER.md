# HƯỚNG DẪN SỬ DỤNG PLC MANAGER

## Giới thiệu

**PLCManager** là một Singleton class giúp quản lý config và kết nối PLC tập trung. Thay vì phải load config và tạo PLC instance ở nhiều nơi, bạn chỉ cần khởi tạo **một lần** khi chương trình bắt đầu, sau đó sử dụng ở **mọi nơi** trong ứng dụng.

### Ưu điểm:

✅ **Singleton Pattern** - Đảm bảo chỉ có 1 instance duy nhất
✅ **Auto Load Config** - Tự động load file PLCConfig.json khi khởi động
✅ **Centralized Management** - Quản lý tập trung config và connection
✅ **Auto Reconnect** - Tự động kết nối lại khi mất kết nối
✅ **Thread-Safe** - An toàn khi dùng trong môi trường multi-thread
✅ **Easy Access** - Truy cập dễ dàng từ bất kỳ đâu: `PLCManager.Instance`

---

## Cấu trúc PLCManager

```csharp
public sealed class PLCManager
{
    // Singleton instance
    public static PLCManager Instance { get; }

    // Properties
    public PLCConfigManager ConfigManager { get; }    // Config manager
    public PLCKeyence PLC { get; }                    // PLC instance
    public bool IsConfigLoaded { get; }               // Config đã load?
    public bool IsConnected { get; }                  // PLC đã kết nối?
    public PLCConfig CurrentConfig { get; }           // Config hiện tại

    // Methods
    public bool Initialize(string configFilePath)    // Khởi tạo & load config
    public bool Connect()                             // Kết nối PLC
    public void Disconnect()                          // Ngắt kết nối
    public bool ReloadConfig(string configFilePath)   // Reload config
    public bool TestConnection()                      // Test kết nối
    public bool AutoReconnect()                       // Tự động reconnect
    public PLCAddressInfo GetAddressInfo(...)         // Lấy thông tin địa chỉ
}
```

---

## Cách sử dụng

### 1. Console Application

#### Bước 1: Thêm vào Program.cs

```csharp
using System;
using PLCKeygen;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== PLC MONITORING SYSTEM ===\n");

        // KHỞI TẠO PLC MANAGER
        if (!PLCManager.Instance.Initialize("PLCConfig.json"))
        {
            Console.WriteLine("Không thể load config!");
            return;
        }

        // KẾT NỐI PLC
        if (!PLCManager.Instance.Connect())
        {
            Console.WriteLine("Không thể kết nối PLC!");
            return;
        }

        // SỬ DỤNG PLC
        MainLoop();

        // NGẮT KẾT NỐI KHI THOÁT
        PLCManager.Instance.Disconnect();
    }

    static void MainLoop()
    {
        // Sử dụng PLC ở đây
        PLCKeyence plc = PLCManager.Instance.PLC;

        bool sensor = plc.ReadBit(InputAddress.Cam_bien_va_cham.GetAddress());
        Console.WriteLine($"Sensor: {sensor}");

        plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());
        Console.WriteLine("Đã bật đèn xanh");
    }
}
```

### 2. Windows Forms Application

#### Bước 1: Sửa file Program.cs

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

        // KHỞI TẠO PLC MANAGER
        if (!PLCManager.Instance.Initialize("PLCConfig.json"))
        {
            MessageBox.Show("Không thể load config file!", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // KẾT NỐI PLC
        if (!PLCManager.Instance.Connect())
        {
            DialogResult result = MessageBox.Show(
                "Không thể kết nối đến PLC. Tiếp tục?",
                "Cảnh báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.No)
                return;
        }

        // CHẠY FORM CHÍNH
        Application.Run(new Form1());

        // NGẮT KẾT NỐI KHI THOÁT
        PLCManager.Instance.Disconnect();
    }
}
```

#### Bước 2: Sử dụng trong Form1.cs

```csharp
using System;
using System.Windows.Forms;
using PLCKeygen;

public partial class Form1 : Form
{
    private Timer monitorTimer;

    private void Form1_Load(object sender, EventArgs e)
    {
        // Kiểm tra đã kết nối chưa
        if (!PLCManager.Instance.IsConnected)
        {
            lblStatus.Text = "PLC chưa kết nối";
            lblStatus.BackColor = Color.Red;
            return;
        }

        // Hiển thị thông tin PLC
        lblPLCName.Text = PLCManager.Instance.CurrentConfig.PLCName;
        lblIPAddress.Text = $"{PLCManager.Instance.CurrentConfig.IPAddress}:" +
                           $"{PLCManager.Instance.CurrentConfig.Port}";

        // Khởi tạo timer để cập nhật dữ liệu
        monitorTimer = new Timer();
        monitorTimer.Interval = 100; // 100ms
        monitorTimer.Tick += MonitorTimer_Tick;
        monitorTimer.Start();
    }

    private void MonitorTimer_Tick(object sender, EventArgs e)
    {
        if (!PLCManager.Instance.IsConnected)
            return;

        try
        {
            PLCKeyence plc = PLCManager.Instance.PLC;

            // Đọc dữ liệu
            bool motorRunning = plc.ReadBit(
                OutputAddress.Dong_co_chinh.GetAddress());
            ushort speed = plc.ReadUInt16(
                InputAddress.Toc_do_dong_co.GetAddress());
            int count = plc.ReadInt32(
                DataAddress.So_luong_san_pham_OK.GetAddress());

            // Cập nhật UI
            lblMotorStatus.Text = motorRunning ? "ĐANG CHẠY" : "DỪNG";
            lblMotorStatus.BackColor = motorRunning ? Color.Green : Color.Gray;
            lblSpeed.Text = $"{speed} RPM";
            lblCount.Text = count.ToString();
        }
        catch (Exception ex)
        {
            // Tự động reconnect
            if (!PLCManager.Instance.AutoReconnect())
            {
                monitorTimer.Stop();
                MessageBox.Show($"Mất kết nối: {ex.Message}");
            }
        }
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        if (!PLCManager.Instance.IsConnected)
        {
            MessageBox.Show("PLC chưa kết nối!");
            return;
        }

        PLCKeyence plc = PLCManager.Instance.PLC;

        // Bật động cơ
        plc.SetBit(OutputAddress.Dong_co_chinh.GetAddress());
        plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());

        MessageBox.Show("Đã khởi động!");
    }

    private void btnStop_Click(object sender, EventArgs e)
    {
        if (!PLCManager.Instance.IsConnected)
        {
            MessageBox.Show("PLC chưa kết nối!");
            return;
        }

        PLCKeyence plc = PLCManager.Instance.PLC;

        // Tắt động cơ
        plc.ResetBit(OutputAddress.Dong_co_chinh.GetAddress());
        plc.SetBit(OutputAddress.Den_bao_do.GetAddress());

        MessageBox.Show("Đã dừng!");
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        monitorTimer?.Stop();
        // Disconnect sẽ được gọi trong Program.cs
    }
}
```

---

## Các tính năng chính

### 1. Initialize - Khởi tạo và Load Config

```csharp
// Load config từ file
bool success = PLCManager.Instance.Initialize("PLCConfig.json");

// Hoặc dùng đường dẫn tuyệt đối
bool success = PLCManager.Instance.Initialize(@"C:\MyApp\PLCConfig.json");

// Kiểm tra kết quả
if (success)
{
    Console.WriteLine("Config loaded!");
}
```

**Lưu ý:** Method này sẽ tự động:
- Tìm file config
- Load và parse JSON
- Tạo PLC instance
- Cập nhật `IsConfigLoaded = true`

### 2. Connect - Kết nối PLC

```csharp
// Kết nối PLC
if (PLCManager.Instance.Connect())
{
    Console.WriteLine("Đã kết nối!");
}
else
{
    Console.WriteLine("Kết nối thất bại!");
}

// Kiểm tra trạng thái
if (PLCManager.Instance.IsConnected)
{
    // PLC đã kết nối
}
```

### 3. Disconnect - Ngắt kết nối

```csharp
// Ngắt kết nối PLC
PLCManager.Instance.Disconnect();

// Kiểm tra
Console.WriteLine($"Connected: {PLCManager.Instance.IsConnected}"); // False
```

### 4. AutoReconnect - Tự động kết nối lại

```csharp
// Trong vòng lặp hoặc timer
while (true)
{
    // Tự động reconnect nếu mất kết nối
    if (!PLCManager.Instance.AutoReconnect())
    {
        Console.WriteLine("Không thể kết nối!");
        break;
    }

    // Đọc dữ liệu
    PLCKeyence plc = PLCManager.Instance.PLC;
    bool sensor = plc.ReadBit(InputAddress.Cam_bien_va_cham.GetAddress());

    Thread.Sleep(1000);
}
```

### 5. TestConnection - Kiểm tra kết nối

```csharp
// Kiểm tra kết nối có OK không
if (PLCManager.Instance.TestConnection())
{
    Console.WriteLine("Kết nối OK");
}
else
{
    Console.WriteLine("Mất kết nối");
}
```

### 6. ReloadConfig - Reload config trong runtime

```csharp
// Reload config (sẽ tự động disconnect trước)
if (PLCManager.Instance.ReloadConfig("PLCConfig.json"))
{
    Console.WriteLine("Config đã được reload!");

    // Kết nối lại
    PLCManager.Instance.Connect();
}
```

### 7. Truy cập PLC Instance

```csharp
// Lấy PLC instance
PLCKeyence plc = PLCManager.Instance.PLC;

// Sử dụng như bình thường
bool sensor = plc.ReadBit(InputAddress.Cam_bien_va_cham.GetAddress());
plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());
int count = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
```

### 8. Truy cập Config

```csharp
// Lấy config hiện tại
PLCConfig config = PLCManager.Instance.CurrentConfig;

Console.WriteLine($"PLC Name: {config.PLCName}");
Console.WriteLine($"IP: {config.IPAddress}");
Console.WriteLine($"Port: {config.Port}");
Console.WriteLine($"Số Input: {config.Addresses.Input.Count}");
```

### 9. Lấy thông tin địa chỉ

```csharp
// Lấy thông tin một địa chỉ
PLCAddressInfo info = PLCManager.Instance.GetAddressInfo("Input", "Cam_bien_va_cham");

Console.WriteLine($"Tên: {info.Name}");
Console.WriteLine($"Tên hiển thị: {info.DisplayName}");
Console.WriteLine($"Địa chỉ: {info.Address}");
Console.WriteLine($"Kiểu: {info.DataType}");
```

---

## Ví dụ đầy đủ

### Console Application hoàn chỉnh

```csharp
using System;
using System.Threading;
using PLCKeygen;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== PLC SYSTEM ===\n");

        // 1. Khởi tạo
        if (!PLCManager.Instance.Initialize("PLCConfig.json"))
        {
            Console.WriteLine("Load config thất bại!");
            return;
        }

        // 2. Kết nối
        if (!PLCManager.Instance.Connect())
        {
            Console.WriteLine("Kết nối thất bại!");
            return;
        }

        // 3. Menu
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("1. Đọc Input");
            Console.WriteLine("2. Điều khiển Output");
            Console.WriteLine("3. Xem Data");
            Console.WriteLine("0. Thoát");
            Console.Write("Chọn: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ReadInputs();
                    break;
                case "2":
                    ControlOutputs();
                    break;
                case "3":
                    ViewData();
                    break;
                case "0":
                    running = false;
                    break;
            }

            if (running)
            {
                Console.WriteLine("\nNhấn phím...");
                Console.ReadKey();
            }
        }

        // 4. Ngắt kết nối
        PLCManager.Instance.Disconnect();
    }

    static void ReadInputs()
    {
        if (!EnsureConnected()) return;

        PLCKeyence plc = PLCManager.Instance.PLC;

        Console.WriteLine("\n=== INPUT ===");
        bool start = plc.ReadBit(InputAddress.Nut_start.GetAddress());
        bool stop = plc.ReadBit(InputAddress.Nut_stop.GetAddress());
        ushort speed = plc.ReadUInt16(InputAddress.Toc_do_dong_co.GetAddress());

        Console.WriteLine($"Start: {start}");
        Console.WriteLine($"Stop: {stop}");
        Console.WriteLine($"Speed: {speed}");
    }

    static void ControlOutputs()
    {
        if (!EnsureConnected()) return;

        PLCKeyence plc = PLCManager.Instance.PLC;

        Console.WriteLine("\n=== OUTPUT ===");
        Console.WriteLine("1. Bật động cơ");
        Console.WriteLine("2. Tắt động cơ");
        Console.Write("Chọn: ");

        string choice = Console.ReadLine();

        if (choice == "1")
        {
            plc.SetBit(OutputAddress.Dong_co_chinh.GetAddress());
            Console.WriteLine("Đã bật động cơ");
        }
        else if (choice == "2")
        {
            plc.ResetBit(OutputAddress.Dong_co_chinh.GetAddress());
            Console.WriteLine("Đã tắt động cơ");
        }
    }

    static void ViewData()
    {
        if (!EnsureConnected()) return;

        PLCKeyence plc = PLCManager.Instance.PLC;

        Console.WriteLine("\n=== DATA ===");
        int productOK = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
        int productNG = plc.ReadInt32(DataAddress.So_luong_san_pham_NG.GetAddress());

        Console.WriteLine($"Sản phẩm OK: {productOK}");
        Console.WriteLine($"Sản phẩm NG: {productNG}");
    }

    static bool EnsureConnected()
    {
        if (!PLCManager.Instance.IsConnected)
        {
            Console.WriteLine("PLC chưa kết nối!");
            return false;
        }
        return true;
    }
}
```

---

## Best Practices

### ✅ NÊN:

1. **Khởi tạo trong Program.cs hoặc Application Startup**
   ```csharp
   // Trong Main()
   PLCManager.Instance.Initialize("PLCConfig.json");
   PLCManager.Instance.Connect();
   ```

2. **Sử dụng AutoReconnect trong vòng lặp**
   ```csharp
   if (!PLCManager.Instance.AutoReconnect())
   {
       // Xử lý lỗi
   }
   ```

3. **Kiểm tra IsConnected trước khi đọc/ghi**
   ```csharp
   if (PLCManager.Instance.IsConnected)
   {
       PLCKeyence plc = PLCManager.Instance.PLC;
       // ... sử dụng plc
   }
   ```

4. **Disconnect khi thoát ứng dụng**
   ```csharp
   // Cuối Main() hoặc trong FormClosing
   PLCManager.Instance.Disconnect();
   ```

### ❌ KHÔNG NÊN:

1. **Tạo nhiều instance PLCKeyence**
   ```csharp
   // KHÔNG làm thế này!
   PLCKeyence plc1 = new PLCKeyence(...);
   PLCKeyence plc2 = new PLCKeyence(...);
   ```

2. **Load config nhiều lần không cần thiết**
   ```csharp
   // KHÔNG làm thế này!
   PLCManager.Instance.Initialize(...);
   PLCManager.Instance.Initialize(...); // Trùng lặp!
   ```

3. **Quên Disconnect**
   ```csharp
   // KHÔNG quên disconnect khi thoát!
   PLCManager.Instance.Disconnect(); // Phải có!
   ```

---

## Troubleshooting

### Lỗi: "Config chưa được load!"

**Nguyên nhân:** Chưa gọi `Initialize()`

**Giải pháp:**
```csharp
PLCManager.Instance.Initialize("PLCConfig.json");
```

### Lỗi: "PLC chưa kết nối!"

**Nguyên nhân:** Chưa gọi `Connect()` hoặc kết nối thất bại

**Giải pháp:**
```csharp
if (!PLCManager.Instance.Connect())
{
    // Kiểm tra IP, Port trong PLCConfig.json
    // Kiểm tra PLC có bật không
    // Kiểm tra network
}
```

### Lỗi: Mất kết nối giữa chừng

**Giải pháp:** Sử dụng `AutoReconnect()`
```csharp
try
{
    PLCKeyence plc = PLCManager.Instance.PLC;
    bool data = plc.ReadBit("R0");
}
catch
{
    PLCManager.Instance.AutoReconnect();
}
```

---

## Tóm tắt

| Method | Mô tả | Khi nào dùng |
|--------|-------|--------------|
| `Initialize()` | Load config | Lúc khởi động app |
| `Connect()` | Kết nối PLC | Sau khi Initialize |
| `Disconnect()` | Ngắt kết nối | Khi thoát app |
| `AutoReconnect()` | Tự động reconnect | Trong loop/timer |
| `TestConnection()` | Kiểm tra kết nối | Khi cần check |
| `ReloadConfig()` | Reload config | Khi config thay đổi |

**Workflow chuẩn:**

```
1. Initialize()  -> Load config
2. Connect()     -> Kết nối PLC
3. Use PLC       -> Đọc/ghi dữ liệu
4. Disconnect()  -> Ngắt kết nối
```

Chúc bạn sử dụng thành công! 🚀
