using System;
using System.Threading;
using System.Windows.Forms;

namespace PLCKeygen
{
    /// <summary>
    /// Ví dụ sử dụng PLCManager trong các trường hợp khác nhau
    /// </summary>
    public class PLCManagerUsageExample
    {
        /// <summary>
        /// Ví dụ 1: Khởi tạo trong Console Application
        /// Gọi trong Program.cs Main()
        /// </summary>
        public static void Example1_ConsoleApplication()
        {
            Console.WriteLine("=== CONSOLE APPLICATION - KHỞI ĐỘNG ===\n");

            // Bước 1: Khởi tạo và load config
            if (PLCManager.Instance.Initialize("PLCConfig.json"))
            {
                // Bước 2: Kết nối PLC
                if (PLCManager.Instance.Connect())
                {
                    // Bước 3: Sử dụng PLC
                    UseWithPLCManager();

                    // Bước 4: Ngắt kết nối khi thoát
                    PLCManager.Instance.Disconnect();
                }
                else
                {
                    Console.WriteLine("Không thể kết nối đến PLC!");
                }
            }
            else
            {
                Console.WriteLine("Không thể load config file!");
            }
        }

        /// <summary>
        /// Ví dụ 2: Khởi tạo trong Windows Forms Application
        /// Gọi trong Program.cs hoặc Form_Load
        /// </summary>
        public static void Example2_WindowsFormsStartup()
        {
            /*
             * Trong Program.cs:
             */

            // [STAThread]
            // static void Main()
            // {
            //     Application.EnableVisualStyles();
            //     Application.SetCompatibleTextRenderingDefault(false);
            //
            //     // KHỞI TẠO PLC MANAGER
            //     if (!PLCManager.Instance.Initialize("PLCConfig.json"))
            //     {
            //         MessageBox.Show("Không thể load config file!", "Lỗi",
            //                         MessageBoxButtons.OK, MessageBoxIcon.Error);
            //         return;
            //     }
            //
            //     // KẾT NỐI PLC
            //     if (!PLCManager.Instance.Connect())
            //     {
            //         DialogResult result = MessageBox.Show(
            //             "Không thể kết nối đến PLC. Bạn có muốn tiếp tục không?",
            //             "Cảnh báo",
            //             MessageBoxButtons.YesNo,
            //             MessageBoxIcon.Warning);
            //
            //         if (result == DialogResult.No)
            //             return;
            //     }
            //
            //     // Chạy form chính
            //     Application.Run(new Form1());
            //
            //     // NGẮT KẾT NỐI KHI THOÁT
            //     PLCManager.Instance.Disconnect();
            // }

            Console.WriteLine("Xem comment trong source code để biết cách implement");
        }

        /// <summary>
        /// Ví dụ 3: Sử dụng PLCManager trong code
        /// </summary>
        public static void UseWithPLCManager()
        {
            Console.WriteLine("\n=== SỬ DỤNG PLC MANAGER ===\n");

            // Kiểm tra đã kết nối chưa
            if (!PLCManager.Instance.IsConnected)
            {
                Console.WriteLine("PLC chưa kết nối!");
                return;
            }

            PLCKeyence plc = PLCManager.Instance.PLC;

            try
            {
                // Đọc Input
                Console.WriteLine("--- Đọc Input ---");
                bool sensor = plc.ReadBit(InputAddress.Cam_bien_va_cham.GetAddress());
                bool startBtn = plc.ReadBit(InputAddress.Nut_start.GetAddress());
                ushort speed = plc.ReadUInt16(InputAddress.Toc_do_dong_co.GetAddress());

                Console.WriteLine($"Cảm biến va chạm: {sensor}");
                Console.WriteLine($"Nút Start: {startBtn}");
                Console.WriteLine($"Tốc độ: {speed} RPM");

                // Ghi Output
                Console.WriteLine("\n--- Điều khiển Output ---");
                plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());
                plc.ResetBit(OutputAddress.Den_bao_do.GetAddress());
                Console.WriteLine("Đã bật đèn xanh, tắt đèn đỏ");

                // Đọc Data
                Console.WriteLine("\n--- Đọc Data ---");
                int productOK = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
                int productNG = plc.ReadInt32(DataAddress.So_luong_san_pham_NG.GetAddress());
                ushort runMode = plc.ReadUInt16(DataAddress.Che_do_van_hanh.GetAddress());

                Console.WriteLine($"Sản phẩm OK: {productOK}");
                Console.WriteLine($"Sản phẩm NG: {productNG}");
                Console.WriteLine($"Chế độ: {(RunMode)runMode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Ví dụ 4: Xem thông tin config đã load
        /// </summary>
        public static void Example4_ViewConfigInfo()
        {
            Console.WriteLine("\n=== THÔNG TIN CONFIG ===\n");

            if (!PLCManager.Instance.IsConfigLoaded)
            {
                Console.WriteLine("Config chưa được load!");
                return;
            }

            PLCConfig config = PLCManager.Instance.CurrentConfig;

            Console.WriteLine($"PLC Name: {config.PLCName}");
            Console.WriteLine($"IP Address: {config.IPAddress}");
            Console.WriteLine($"Port: {config.Port}");
            Console.WriteLine($"\nSố lượng địa chỉ:");
            Console.WriteLine($"  Input: {config.Addresses.Input.Count}");
            Console.WriteLine($"  Output: {config.Addresses.Output.Count}");
            Console.WriteLine($"  Data: {config.Addresses.Data.Count}");

            // Hiển thị một số địa chỉ Input
            Console.WriteLine("\nMột số địa chỉ Input:");
            for (int i = 0; i < Math.Min(5, config.Addresses.Input.Count); i++)
            {
                var addr = config.Addresses.Input[i];
                Console.WriteLine($"  - {addr.DisplayName} ({addr.Name}): {addr.Address} [{addr.DataType}]");
            }

            // Hiển thị một số địa chỉ Output
            Console.WriteLine("\nMột số địa chỉ Output:");
            for (int i = 0; i < Math.Min(5, config.Addresses.Output.Count); i++)
            {
                var addr = config.Addresses.Output[i];
                Console.WriteLine($"  - {addr.DisplayName} ({addr.Name}): {addr.Address} [{addr.DataType}]");
            }
        }

        /// <summary>
        /// Ví dụ 5: Tự động reconnect khi mất kết nối
        /// </summary>
        public static void Example5_AutoReconnect()
        {
            Console.WriteLine("\n=== TẰT ĐỘNG RECONNECT ===\n");

            // Giả lập vòng lặp đọc dữ liệu
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"\nLần đọc thứ {i + 1}:");

                // Kiểm tra và tự động reconnect
                if (!PLCManager.Instance.AutoReconnect())
                {
                    Console.WriteLine("Không thể kết nối đến PLC!");
                    break;
                }

                try
                {
                    // Đọc dữ liệu
                    PLCKeyence plc = PLCManager.Instance.PLC;
                    bool sensor = plc.ReadBit(InputAddress.Cam_bien_va_cham.GetAddress());
                    Console.WriteLine($"Sensor: {sensor}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi: {ex.Message}");
                }

                Thread.Sleep(1000); // Đợi 1 giây
            }
        }

        /// <summary>
        /// Ví dụ 6: Lấy thông tin địa chỉ từ config
        /// </summary>
        public static void Example6_GetAddressInfo()
        {
            Console.WriteLine("\n=== LẤY THÔNG TIN ĐỊA CHỈ ===\n");

            if (!PLCManager.Instance.IsConfigLoaded)
            {
                Console.WriteLine("Config chưa được load!");
                return;
            }

            // Lấy thông tin một địa chỉ Input
            PLCAddressInfo inputInfo = PLCManager.Instance.GetAddressInfo("Input", "Cam_bien_va_cham");
            if (inputInfo != null)
            {
                Console.WriteLine($"Tên: {inputInfo.Name}");
                Console.WriteLine($"Tên hiển thị: {inputInfo.DisplayName}");
                Console.WriteLine($"Địa chỉ: {inputInfo.Address}");
                Console.WriteLine($"Kiểu: {inputInfo.DataType}");
            }

            Console.WriteLine();

            // Lấy thông tin một địa chỉ Output
            PLCAddressInfo outputInfo = PLCManager.Instance.GetAddressInfo("Output", "Dong_co_chinh");
            if (outputInfo != null)
            {
                Console.WriteLine($"Tên: {outputInfo.Name}");
                Console.WriteLine($"Tên hiển thị: {outputInfo.DisplayName}");
                Console.WriteLine($"Địa chỉ: {outputInfo.Address}");
                Console.WriteLine($"Kiểu: {outputInfo.DataType}");
            }
        }

        /// <summary>
        /// Ví dụ 7: Reload config trong runtime
        /// </summary>
        public static void Example7_ReloadConfig()
        {
            Console.WriteLine("\n=== RELOAD CONFIG ===\n");

            Console.WriteLine("Config hiện tại:");
            Console.WriteLine($"  PLC Name: {PLCManager.Instance.CurrentConfig?.PLCName}");
            Console.WriteLine($"  IP: {PLCManager.Instance.CurrentConfig?.IPAddress}");

            Console.WriteLine("\nĐang reload config...");

            if (PLCManager.Instance.ReloadConfig("PLCConfig.json"))
            {
                Console.WriteLine("✓ Reload thành công!");
                Console.WriteLine($"  PLC Name: {PLCManager.Instance.CurrentConfig.PLCName}");
                Console.WriteLine($"  IP: {PLCManager.Instance.CurrentConfig.IPAddress}");
            }
            else
            {
                Console.WriteLine("✗ Reload thất bại!");
            }
        }
    }

    // ============================================================================
    // WINDOWS FORMS EXAMPLE - ĐẦY ĐỦ
    // ============================================================================

    /// <summary>
    /// Ví dụ Form sử dụng PLCManager
    /// Copy code này vào Form1.cs của bạn
    /// </summary>
    public partial class FormPLCExample : Form
    {
        private System.Windows.Forms.Timer monitorTimer;

        public FormPLCExample()
        {
            InitializeComponent();
        }

        private void FormPLCExample_Load(object sender, EventArgs e)
        {
            // Kiểm tra PLCManager đã được khởi tạo trong Program.cs chưa
            if (!PLCManager.Instance.IsConfigLoaded)
            {
                MessageBox.Show("Config chưa được load! Kiểm tra Program.cs", "Lỗi");
                this.Close();
                return;
            }

            // Hiển thị thông tin PLC
            UpdatePLCInfo();

            // Khởi tạo timer để cập nhật dữ liệu
            monitorTimer = new System.Windows.Forms.Timer();
            monitorTimer.Interval = 100; // 100ms
            monitorTimer.Tick += MonitorTimer_Tick;
            monitorTimer.Start();
        }

        private void UpdatePLCInfo()
        {
            /*
            lblPLCName.Text = PLCManager.Instance.CurrentConfig.PLCName;
            lblIPAddress.Text = $"{PLCManager.Instance.CurrentConfig.IPAddress}:{PLCManager.Instance.CurrentConfig.Port}";
            lblConnectionStatus.Text = PLCManager.Instance.IsConnected ? "Đã kết nối" : "Chưa kết nối";
            lblConnectionStatus.BackColor = PLCManager.Instance.IsConnected ? Color.LightGreen : Color.LightCoral;
            */
        }

        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            if (!PLCManager.Instance.IsConnected)
            {
                return;
            }

            try
            {
                PLCKeyence plc = PLCManager.Instance.PLC;

                // Đọc và cập nhật UI
                /*
                bool motorRunning = plc.ReadBit(OutputAddress.Dong_co_chinh.GetAddress());
                lblMotorStatus.Text = motorRunning ? "ĐANG CHẠY" : "DỪNG";
                lblMotorStatus.BackColor = motorRunning ? Color.LightGreen : Color.LightGray;

                ushort speed = plc.ReadUInt16(InputAddress.Toc_do_dong_co.GetAddress());
                lblSpeed.Text = $"{speed} RPM";

                int productCount = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
                lblProductCount.Text = productCount.ToString();
                */
            }
            catch (Exception ex)
            {
                // Thử reconnect
                if (!PLCManager.Instance.AutoReconnect())
                {
                    monitorTimer.Stop();
                    MessageBox.Show($"Mất kết nối PLC: {ex.Message}", "Lỗi");
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!PLCManager.Instance.IsConnected)
            {
                MessageBox.Show("PLC chưa kết nối!", "Cảnh báo");
                return;
            }

            try
            {
                PLCKeyence plc = PLCManager.Instance.PLC;

                // Bật động cơ
                plc.SetBit(OutputAddress.Dong_co_chinh.GetAddress());
                // Đặt tốc độ
                plc.WriteUInt16(OutputAddress.Toc_do_dat_dong_co.GetAddress(), 1500);
                // Bật đèn xanh
                plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());
                plc.ResetBit(OutputAddress.Den_bao_do.GetAddress());

                MessageBox.Show("Đã khởi động động cơ!", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (!PLCManager.Instance.IsConnected)
            {
                MessageBox.Show("PLC chưa kết nối!", "Cảnh báo");
                return;
            }

            try
            {
                PLCKeyence plc = PLCManager.Instance.PLC;

                // Tắt động cơ
                plc.ResetBit(OutputAddress.Dong_co_chinh.GetAddress());
                // Bật đèn đỏ
                plc.SetBit(OutputAddress.Den_bao_do.GetAddress());
                plc.ResetBit(OutputAddress.Den_bao_xanh.GetAddress());

                MessageBox.Show("Đã dừng động cơ!", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
            }
        }

        private void btnReconnect_Click(object sender, EventArgs e)
        {
            if (PLCManager.Instance.AutoReconnect())
            {
                UpdatePLCInfo();
                MessageBox.Show("Đã kết nối lại thành công!", "Thông báo");

                if (!monitorTimer.Enabled)
                {
                    monitorTimer.Start();
                }
            }
            else
            {
                MessageBox.Show("Không thể kết nối đến PLC!", "Lỗi");
            }
        }

        private void FormPLCExample_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dừng timer
            monitorTimer?.Stop();

            // Không cần disconnect ở đây vì sẽ disconnect trong Program.cs
            // Nhưng nếu bạn muốn disconnect ngay khi đóng form:
            // PLCManager.Instance.Disconnect();
        }

        // Dummy InitializeComponent để code compile được
        private void InitializeComponent()
        {
            // Bạn cần tạo UI trong Designer
        }
    }
}
