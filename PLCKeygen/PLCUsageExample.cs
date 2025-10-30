using System;
using System.Windows.Forms;

namespace PLCKeygen
{
    /// <summary>
    /// Ví dụ cách sử dụng hệ thống cấu hình PLC
    /// </summary>
    public class PLCUsageExample
    {
        /// <summary>
        /// Ví dụ 1: Sử dụng địa chỉ PLC đã được generate
        /// </summary>
        public static void Example1_UseGeneratedAddresses()
        {
            // Tạo kết nối PLC
            PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);

            try
            {
                // Kết nối
                plc.Open();
                plc.StartCommunication();


                // Ghi Output
                plc.SetBit(PLCAddresses.Output.Den_bao);  // Bật đèn báo
                plc.SetBit(PLCAddresses.Output.Dong_co_chinh);  // Bật động cơ
                plc.WriteInt16(PLCAddresses.Output.Gia_tri_dat, 1500);

       
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

        /// <summary>
        /// Ví dụ 2: Load config và tạo PLC instance từ config
        /// </summary>
        public static void Example2_LoadConfigAndCreatePLC()
        {
            // Tạo config manager
            PLCConfigManager configManager = new PLCConfigManager();

            // Load config từ file JSON
            if (configManager.LoadConfig("PLCConfig.json"))
            {
                Console.WriteLine("Config đã được load thành công!");
                Console.WriteLine($"PLC Name: {configManager.Config.PLCName}");
                Console.WriteLine($"IP: {configManager.Config.IPAddress}:{configManager.Config.Port}");

                // Tạo PLC instance từ config
                PLCKeyence plc = configManager.CreatePLCInstance();

                // Sử dụng như bình thường
                plc.Open();
                plc.StartCommunication();

                // ... làm việc với PLC ...

                plc.Close();
            }
            else
            {
                Console.WriteLine("Không thể load config!");
            }
        }

        /// <summary>
        /// Ví dụ 3: Regenerate file PLCAddresses.Generated.cs từ JSON
        /// Chạy method này khi bạn thay đổi PLCConfig.json
        /// </summary>
        public static void Example3_RegenerateAddressesFile()
        {
            PLCConfigManager configManager = new PLCConfigManager();

            // Load config
            if (configManager.LoadConfig("PLCConfig.json"))
            {
                // Generate lại file PLCAddresses.Generated.cs
                string outputPath = "PLCAddresses.Generated.cs";

                if (configManager.GenerateAddressesFile(outputPath))
                {
                    Console.WriteLine($"Đã generate file: {outputPath}");
                    Console.WriteLine("Bạn cần rebuild project để áp dụng thay đổi!");
                }
                else
                {
                    Console.WriteLine("Không thể generate file!");
                }
            }
        }

        /// <summary>
        /// Ví dụ 4: Lấy thông tin chi tiết của địa chỉ
        /// </summary>
        public static void Example4_GetAddressInfo()
        {
            PLCConfigManager configManager = new PLCConfigManager();

            if (configManager.LoadConfig("PLCConfig.json"))
            {
                // Lấy thông tin của một địa chỉ
                PLCAddressInfo addrInfo = configManager.GetAddressInfo("Input", "Cam_bien_va_cham");

                if (addrInfo != null)
                {
                    Console.WriteLine($"Tên: {addrInfo.Name}");
                    Console.WriteLine($"Tên hiển thị: {addrInfo.DisplayName}");
                    Console.WriteLine($"Địa chỉ: {addrInfo.Address}");
                    Console.WriteLine($"Kiểu dữ liệu: {addrInfo.DataType}");
                }
            }
        }

        /// <summary>
        /// Ví dụ 5: Sử dụng trong Form với event handler
        /// </summary>
        public static void Example5_UseInForm()
        {
            // Trong Form1.cs, bạn có thể khai báo:
            // private PLCKeyence plc;

            // Trong Form1_Load:
            // plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
            // plc.Open();
            // plc.StartCommunication();

            // Trong button click event:
            // private void btnStart_Click(object sender, EventArgs e)
            // {
            //     // Đọc trạng thái nút start từ PLC
            //     bool startPressed = plc.ReadBit(PLCAddresses.Input.Nut_start);
            //
            //     if (startPressed)
            //     {
            //         // Bật động cơ
            //         plc.SetBit(PLCAddresses.Output.Dong_co_chinh);
            //         MessageBox.Show("Động cơ đã bật!");
            //     }
            // }

            // Trong Form1_FormClosing:
            // plc?.Close();
        }
    }
}
