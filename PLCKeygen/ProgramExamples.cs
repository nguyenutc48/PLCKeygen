using System;
using System.Windows.Forms;

namespace PLCKeygen
{
    /// <summary>
    /// Các ví dụ Program.cs cho Console và Windows Forms
    /// </summary>
    public class ProgramExamples
    {
        // ========================================================================
        // CONSOLE APPLICATION - Program.cs
        // ========================================================================

        /// <summary>
        /// Console Application - Program.cs
        /// Copy code này vào file Program.cs của Console Application
        /// </summary>
        public static void ConsoleApplicationMain()
        {
            /*
            using System;
            using PLCKeygen;

            namespace YourConsoleApp
            {
                class Program
                {
                    static void Main(string[] args)
                    {
                        Console.WriteLine("===========================================");
                        Console.WriteLine("    PLC MONITORING SYSTEM - CONSOLE APP");
                        Console.WriteLine("===========================================\n");

                        // BƯỚC 1: Khởi tạo PLCManager và load config
                        Console.WriteLine("Đang khởi tạo hệ thống...");
                        if (!PLCManager.Instance.Initialize("PLCConfig.json"))
                        {
                            Console.WriteLine("\n✗ KHÔNG THỂ LOAD CONFIG FILE!");
                            Console.WriteLine("Nhấn phím bất kỳ để thoát...");
                            Console.ReadKey();
                            return;
                        }

                        // BƯỚC 2: Kết nối đến PLC
                        Console.WriteLine("\nĐang kết nối đến PLC...");
                        if (!PLCManager.Instance.Connect())
                        {
                            Console.WriteLine("\n✗ KHÔNG THỂ KẾT NỐI ĐẾN PLC!");
                            Console.Write("Bạn có muốn tiếp tục không? (Y/N): ");
                            string answer = Console.ReadLine();
                            if (answer?.ToUpper() != "Y")
                            {
                                return;
                            }
                        }

                        Console.WriteLine("\n===========================================");
                        Console.WriteLine("    HỆ THỐNG ĐÃ SẴN SÀNG");
                        Console.WriteLine("===========================================\n");

                        // BƯỚC 3: Menu chính
                        bool running = true;
                        while (running)
                        {
                            ShowMenu();
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
                                    ViewProductionData();
                                    break;
                                case "4":
                                    ViewSystemStatus();
                                    break;
                                case "5":
                                    AutoSequence();
                                    break;
                                case "6":
                                    ViewConfigInfo();
                                    break;
                                case "7":
                                    TestConnection();
                                    break;
                                case "0":
                                    running = false;
                                    break;
                                default:
                                    Console.WriteLine("Lựa chọn không hợp lệ!");
                                    break;
                            }

                            if (running)
                            {
                                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                                Console.ReadKey();
                            }
                        }

                        // BƯỚC 4: Dọn dẹp và thoát
                        Console.WriteLine("\nĐang ngắt kết nối...");
                        PLCManager.Instance.Disconnect();
                        Console.WriteLine("Tạm biệt!");
                    }

                    static void ShowMenu()
                    {
                        Console.Clear();
                        Console.WriteLine("===========================================");
                        Console.WriteLine("             MENU CHÍNH");
                        Console.WriteLine("===========================================");
                        Console.WriteLine("1. Đọc trạng thái Input");
                        Console.WriteLine("2. Điều khiển Output");
                        Console.WriteLine("3. Xem dữ liệu sản xuất");
                        Console.WriteLine("4. Xem trạng thái hệ thống");
                        Console.WriteLine("5. Chạy sequence tự động");
                        Console.WriteLine("6. Xem thông tin config");
                        Console.WriteLine("7. Kiểm tra kết nối");
                        Console.WriteLine("0. Thoát");
                        Console.WriteLine("===========================================");
                        Console.Write("Nhập lựa chọn của bạn: ");
                    }

                    static void ReadInputs()
                    {
                        if (!EnsureConnected()) return;

                        Console.WriteLine("\n=== TRẠNG THÁI INPUT ===\n");
                        PLCKeyence plc = PLCManager.Instance.PLC;

                        try
                        {
                            bool startBtn = plc.ReadBit(InputAddress.Nut_start.GetAddress());
                            bool stopBtn = plc.ReadBit(InputAddress.Nut_stop.GetAddress());
                            bool resetBtn = plc.ReadBit(InputAddress.Nut_reset.GetAddress());
                            ushort speed = plc.ReadUInt16(InputAddress.Toc_do_dong_co.GetAddress());

                            Console.WriteLine($"Nút Start:  {(startBtn ? "[NHẤN]" : "[----]")}");
                            Console.WriteLine($"Nút Stop:   {(stopBtn ? "[NHẤN]" : "[----]")}");
                            Console.WriteLine($"Nút Reset:  {(resetBtn ? "[NHẤN]" : "[----]")}");
                            Console.WriteLine($"Tốc độ:     {speed} RPM");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi: {ex.Message}");
                        }
                    }

                    static void ControlOutputs()
                    {
                        if (!EnsureConnected()) return;

                        Console.WriteLine("\n=== ĐIỀU KHIỂN OUTPUT ===\n");
                        Console.WriteLine("1. Bật động cơ");
                        Console.WriteLine("2. Tắt động cơ");
                        Console.WriteLine("3. Bật đèn xanh");
                        Console.WriteLine("4. Bật đèn đỏ");
                        Console.WriteLine("5. Tắt tất cả đèn");
                        Console.Write("\nChọn: ");

                        string choice = Console.ReadLine();
                        PLCKeyence plc = PLCManager.Instance.PLC;

                        try
                        {
                            switch (choice)
                            {
                                case "1":
                                    plc.SetBit(OutputAddress.Dong_co_chinh.GetAddress());
                                    Console.WriteLine("✓ Đã bật động cơ");
                                    break;
                                case "2":
                                    plc.ResetBit(OutputAddress.Dong_co_chinh.GetAddress());
                                    Console.WriteLine("✓ Đã tắt động cơ");
                                    break;
                                case "3":
                                    plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());
                                    Console.WriteLine("✓ Đã bật đèn xanh");
                                    break;
                                case "4":
                                    plc.SetBit(OutputAddress.Den_bao_do.GetAddress());
                                    Console.WriteLine("✓ Đã bật đèn đỏ");
                                    break;
                                case "5":
                                    plc.ResetBit(OutputAddress.Den_bao_xanh.GetAddress());
                                    plc.ResetBit(OutputAddress.Den_bao_do.GetAddress());
                                    plc.ResetBit(OutputAddress.Den_bao_vang.GetAddress());
                                    Console.WriteLine("✓ Đã tắt tất cả đèn");
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi: {ex.Message}");
                        }
                    }

                    static void ViewProductionData()
                    {
                        if (!EnsureConnected()) return;

                        Console.WriteLine("\n=== DỮ LIỆU SẢN XUẤT ===\n");
                        PLCKeyence plc = PLCManager.Instance.PLC;

                        try
                        {
                            int productOK = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
                            int productNG = plc.ReadInt32(DataAddress.So_luong_san_pham_NG.GetAddress());
                            int totalProduct = plc.ReadInt32(DataAddress.Tong_san_pham.GetAddress());
                            ushort efficiency = plc.ReadUInt16(DataAddress.Hieu_suat.GetAddress());

                            Console.WriteLine($"Sản phẩm OK:  {productOK}");
                            Console.WriteLine($"Sản phẩm NG:  {productNG}");
                            Console.WriteLine($"Tổng:         {totalProduct}");
                            Console.WriteLine($"Hiệu suất:    {efficiency}%");

                            if (totalProduct > 0)
                            {
                                double ngRate = (double)productNG / totalProduct * 100;
                                Console.WriteLine($"Tỷ lệ NG:     {ngRate:F2}%");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi: {ex.Message}");
                        }
                    }

                    static void ViewSystemStatus()
                    {
                        if (!EnsureConnected()) return;

                        Console.WriteLine("\n=== TRẠNG THÁI HỆ THỐNG ===\n");
                        PLCKeyence plc = PLCManager.Instance.PLC;

                        try
                        {
                            ushort runModeValue = plc.ReadUInt16(DataAddress.Che_do_van_hanh.GetAddress());
                            RunMode runMode = (RunMode)runModeValue;
                            ushort errorCode = plc.ReadUInt16(DataAddress.Ma_loi_hien_tai.GetAddress());

                            Console.WriteLine($"Chế độ:       {runMode}");
                            Console.WriteLine($"Mã lỗi:       {errorCode}");

                            if (errorCode != 0)
                            {
                                Console.WriteLine("\n⚠ CÓ LỖI XẢY RA!");
                            }
                            else
                            {
                                Console.WriteLine("\n✓ Hệ thống hoạt động bình thường");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi: {ex.Message}");
                        }
                    }

                    static void AutoSequence()
                    {
                        if (!EnsureConnected()) return;

                        Console.WriteLine("\n=== SEQUENCE TỰ ĐỘNG ===\n");
                        Console.WriteLine("Đang chạy sequence...");

                        PLCEnumUsageExample.Example6_AutoSequence();
                    }

                    static void ViewConfigInfo()
                    {
                        Console.WriteLine("\n=== THÔNG TIN CONFIG ===\n");
                        PLCManagerUsageExample.Example4_ViewConfigInfo();
                    }

                    static void TestConnection()
                    {
                        Console.WriteLine("\n=== KIỂM TRA KẾT NỐI ===\n");

                        if (PLCManager.Instance.TestConnection())
                        {
                            Console.WriteLine("✓ Kết nối PLC OK");
                        }
                        else
                        {
                            Console.WriteLine("✗ Mất kết nối PLC");
                            Console.WriteLine("Đang thử kết nối lại...");

                            if (PLCManager.Instance.AutoReconnect())
                            {
                                Console.WriteLine("✓ Đã kết nối lại thành công");
                            }
                            else
                            {
                                Console.WriteLine("✗ Không thể kết nối lại");
                            }
                        }
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
            }
            */
        }

        // ========================================================================
        // WINDOWS FORMS APPLICATION - Program.cs
        // ========================================================================

        /// <summary>
        /// Windows Forms Application - Program.cs
        /// Copy code này vào file Program.cs của Windows Forms Application
        /// </summary>
        public static void WindowsFormsApplicationMain()
        {
            /*
            using System;
            using System.Windows.Forms;
            using PLCKeygen;

            namespace YourWinFormsApp
            {
                static class Program
                {
                    [STAThread]
                    static void Main()
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        // HIỂN THỊ SPLASH SCREEN (Optional)
                        using (SplashScreen splash = new SplashScreen())
                        {
                            splash.Show();
                            splash.UpdateStatus("Đang khởi tạo hệ thống...");
                            Application.DoEvents();

                            // BƯỚC 1: Khởi tạo PLCManager và load config
                            splash.UpdateStatus("Đang load config file...");
                            Application.DoEvents();

                            if (!PLCManager.Instance.Initialize("PLCConfig.json"))
                            {
                                splash.Close();
                                MessageBox.Show(
                                    "Không thể load file PLCConfig.json!\n\n" +
                                    "Vui lòng kiểm tra:\n" +
                                    "- File PLCConfig.json có tồn tại không\n" +
                                    "- Cú pháp JSON có đúng không\n" +
                                    "- Đường dẫn file có chính xác không",
                                    "Lỗi khởi tạo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return;
                            }

                            // BƯỚC 2: Kết nối đến PLC
                            splash.UpdateStatus("Đang kết nối đến PLC...");
                            Application.DoEvents();

                            if (!PLCManager.Instance.Connect())
                            {
                                splash.Close();
                                DialogResult result = MessageBox.Show(
                                    $"Không thể kết nối đến PLC!\n\n" +
                                    $"PLC: {PLCManager.Instance.CurrentConfig.PLCName}\n" +
                                    $"IP: {PLCManager.Instance.CurrentConfig.IPAddress}\n" +
                                    $"Port: {PLCManager.Instance.CurrentConfig.Port}\n\n" +
                                    $"Bạn có muốn tiếp tục ở chế độ Offline không?",
                                    "Cảnh báo kết nối",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning);

                                if (result == DialogResult.No)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                splash.UpdateStatus("Kết nối PLC thành công!");
                                Application.DoEvents();
                                System.Threading.Thread.Sleep(500); // Hiển thị message 500ms
                            }

                            splash.UpdateStatus("Đang khởi động giao diện...");
                            Application.DoEvents();
                        }

                        // BƯỚC 3: Chạy Form chính
                        try
                        {
                            Application.Run(new Form1());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                $"Lỗi nghiêm trọng:\n{ex.Message}\n\n{ex.StackTrace}",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        finally
                        {
                            // BƯỚC 4: Ngắt kết nối PLC khi thoát
                            PLCManager.Instance.Disconnect();
                        }
                    }
                }

                // SPLASH SCREEN FORM (Optional)
                public class SplashScreen : Form
                {
                    private Label lblStatus;
                    private ProgressBar progressBar;

                    public SplashScreen()
                    {
                        this.Size = new System.Drawing.Size(400, 200);
                        this.FormBorderStyle = FormBorderStyle.None;
                        this.StartPosition = FormStartPosition.CenterScreen;
                        this.BackColor = System.Drawing.Color.White;

                        Label lblTitle = new Label();
                        lblTitle.Text = "PLC MONITORING SYSTEM";
                        lblTitle.Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold);
                        lblTitle.AutoSize = true;
                        lblTitle.Location = new System.Drawing.Point(50, 50);
                        this.Controls.Add(lblTitle);

                        lblStatus = new Label();
                        lblStatus.Text = "Đang khởi tạo...";
                        lblStatus.AutoSize = true;
                        lblStatus.Location = new System.Drawing.Point(50, 100);
                        this.Controls.Add(lblStatus);

                        progressBar = new ProgressBar();
                        progressBar.Style = ProgressBarStyle.Marquee;
                        progressBar.Location = new System.Drawing.Point(50, 130);
                        progressBar.Size = new System.Drawing.Size(300, 20);
                        this.Controls.Add(progressBar);
                    }

                    public void UpdateStatus(string status)
                    {
                        lblStatus.Text = status;
                    }
                }
            }
            */
        }
    }
}
