using System;

namespace PLCKeygen
{
    /// <summary>
    /// Ví dụ sử dụng Enum để làm việc với PLC
    /// Enum cung cấp type-safety và IntelliSense support tốt hơn
    /// </summary>
    public class PLCEnumUsageExample
    {
        /// <summary>
        /// Ví dụ 1: Sử dụng Enum với Extension Methods
        /// </summary>
        public static void Example1_UseEnumWithExtensions()
        {
            Console.WriteLine("=== VÍ DỤ 1: SỬ DỤNG ENUM VỚI EXTENSION METHODS ===\n");

            // Kết nối PLC
            PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
            plc.Open();
            plc.StartCommunication();

            try
            {
                // ĐỌC INPUT - Sử dụng enum và extension method GetAddress()
                InputAddress sensorAddress = InputAddress.Cam_bien_va_cham;
                bool sensorValue = plc.ReadBit(sensorAddress.GetAddress());

                Console.WriteLine($"Địa chỉ: {sensorAddress.GetAddress()}");
                Console.WriteLine($"Tên: {sensorAddress.GetDisplayName()}");
                Console.WriteLine($"Giá trị: {sensorValue}");
                Console.WriteLine();

                // ĐỌC NHIỀU INPUT CÙNG LÚC
                Console.WriteLine("--- Đọc tất cả các nút điều khiển ---");
                bool startBtn = plc.ReadBit(InputAddress.Nut_start.GetAddress());
                bool stopBtn = plc.ReadBit(InputAddress.Nut_stop.GetAddress());
                bool resetBtn = plc.ReadBit(InputAddress.Nut_reset.GetAddress());

                Console.WriteLine($"Nút Start: {startBtn}");
                Console.WriteLine($"Nút Stop: {stopBtn}");
                Console.WriteLine($"Nút Reset: {resetBtn}");
                Console.WriteLine();

                // GHI OUTPUT - Điều khiển đèn báo
                Console.WriteLine("--- Điều khiển đèn báo ---");
                plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());
                plc.ResetBit(OutputAddress.Den_bao_do.GetAddress());
                plc.ResetBit(OutputAddress.Den_bao_vang.GetAddress());
                Console.WriteLine("Đã bật đèn xanh, tắt đèn đỏ và vàng");
                Console.WriteLine();

                // ĐỌC/GHI DATA
                Console.WriteLine("--- Làm việc với dữ liệu ---");
                int productOK = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
                int productNG = plc.ReadInt32(DataAddress.So_luong_san_pham_NG.GetAddress());

                Console.WriteLine($"Sản phẩm OK: {productOK}");
                Console.WriteLine($"Sản phẩm NG: {productNG}");

                // Tăng số lượng sản phẩm OK
                plc.WriteInt32(DataAddress.So_luong_san_pham_OK.GetAddress(), productOK + 1);
                Console.WriteLine("Đã tăng số lượng sản phẩm OK");
            }
            finally
            {
                plc.Close();
            }
        }

        /// <summary>
        /// Ví dụ 2: Sử dụng Enum để kiểm tra trạng thái hệ thống
        /// </summary>
        public static void Example2_CheckSystemStatus()
        {
            Console.WriteLine("\n=== VÍ DỤ 2: KIỂM TRA TRẠNG THÁI HỆ THỐNG ===\n");

            PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
            plc.Open();
            plc.StartCommunication();

            try
            {
                // Đọc chế độ vận hành
                ushort runModeValue = plc.ReadUInt16(DataAddress.Che_do_van_hanh.GetAddress());
                RunMode runMode = (RunMode)runModeValue;

                Console.WriteLine($"Chế độ vận hành: {runMode}");

                switch (runMode)
                {
                    case RunMode.Stop:
                        Console.WriteLine("Hệ thống đang dừng");
                        break;
                    case RunMode.Manual:
                        Console.WriteLine("Hệ thống ở chế độ Manual");
                        break;
                    case RunMode.Auto:
                        Console.WriteLine("Hệ thống ở chế độ Auto");
                        break;
                }

                // Đọc các trạng thái khác
                ushort systemStatus = plc.ReadUInt16(DataAddress.Trang_thai_he_thong.GetAddress());
                ushort errorCode = plc.ReadUInt16(DataAddress.Ma_loi_hien_tai.GetAddress());

                Console.WriteLine($"Trạng thái hệ thống: 0x{systemStatus:X4}");
                Console.WriteLine($"Mã lỗi: {errorCode}");

                if (errorCode != 0)
                {
                    Console.WriteLine($"CẢNH BÁO: Có lỗi xảy ra! Mã lỗi: {errorCode}");
                    // Bật còi báo động
                    plc.SetBit(OutputAddress.Coi_bao_dong.GetAddress());
                    // Bật đèn đỏ
                    plc.SetBit(OutputAddress.Den_bao_do.GetAddress());
                }
            }
            finally
            {
                plc.Close();
            }
        }

        /// <summary>
        /// Ví dụ 3: Điều khiển động cơ và van khí
        /// </summary>
        public static void Example3_ControlMotorAndValve()
        {
            Console.WriteLine("\n=== VÍ DỤ 3: ĐIỀU KHIỂN ĐỘNG CƠ VÀ VAN KHÍ ===\n");

            PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
            plc.Open();
            plc.StartCommunication();

            try
            {
                // Kiểm tra nút start có được nhấn không
                bool startPressed = plc.ReadBit(InputAddress.Nut_start.GetAddress());

                if (startPressed)
                {
                    Console.WriteLine("Nút Start đã được nhấn, bắt đầu quy trình...");

                    // Bật động cơ chính
                    plc.SetBit(OutputAddress.Dong_co_chinh.GetAddress());
                    Console.WriteLine($"Đã bật {OutputAddress.Dong_co_chinh.GetDisplayName()}");

                    // Đặt tốc độ động cơ
                    ushort targetSpeed = 1500;
                    plc.WriteUInt16(OutputAddress.Toc_do_dat_dong_co.GetAddress(), targetSpeed);
                    Console.WriteLine($"Đặt tốc độ: {targetSpeed} RPM");

                    // Mở van khí số 1
                    plc.SetBit(OutputAddress.Van_khi_1.GetAddress());
                    Console.WriteLine($"Đã mở {OutputAddress.Van_khi_1.GetDisplayName()}");

                    // Đọc tốc độ thực tế
                    System.Threading.Thread.Sleep(100); // Đợi PLC cập nhật
                    ushort actualSpeed = plc.ReadUInt16(InputAddress.Toc_do_dong_co.GetAddress());
                    Console.WriteLine($"Tốc độ thực tế: {actualSpeed} RPM");

                    // Bật đèn xanh báo đang chạy
                    plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());
                }
                else
                {
                    Console.WriteLine("Nút Start chưa được nhấn");
                }

                // Kiểm tra nút stop
                bool stopPressed = plc.ReadBit(InputAddress.Nut_stop.GetAddress());
                if (stopPressed)
                {
                    Console.WriteLine("Nút Stop đã được nhấn, dừng hệ thống...");

                    // Tắt động cơ
                    plc.ResetBit(OutputAddress.Dong_co_chinh.GetAddress());
                    // Đóng van khí
                    plc.ResetBit(OutputAddress.Van_khi_1.GetAddress());
                    // Bật đèn vàng
                    plc.SetBit(OutputAddress.Den_bao_vang.GetAddress());
                    plc.ResetBit(OutputAddress.Den_bao_xanh.GetAddress());

                    Console.WriteLine("Đã dừng hệ thống");
                }
            }
            finally
            {
                plc.Close();
            }
        }

        /// <summary>
        /// Ví dụ 4: Giám sát và ghi log dữ liệu sản xuất
        /// </summary>
        public static void Example4_MonitorProductionData()
        {
            Console.WriteLine("\n=== VÍ DỤ 4: GIÁM SÁT DỮ LIỆU SẢN XUẤT ===\n");

            PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
            plc.Open();
            plc.StartCommunication();

            try
            {
                // Đọc dữ liệu sản xuất
                int productOK = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
                int productNG = plc.ReadInt32(DataAddress.So_luong_san_pham_NG.GetAddress());
                int totalProduct = plc.ReadInt32(DataAddress.Tong_san_pham.GetAddress());
                int runTime = plc.ReadInt32(DataAddress.Tong_thoi_gian_chay.GetAddress());
                int downTime = plc.ReadInt32(DataAddress.Thoi_gian_dung_may.GetAddress());
                ushort cycleTime = plc.ReadUInt16(DataAddress.Cycle_time.GetAddress());
                ushort efficiency = plc.ReadUInt16(DataAddress.Hieu_suat.GetAddress());
                ushort oee = plc.ReadUInt16(DataAddress.OEE.GetAddress());

                // Hiển thị báo cáo
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║     BÁO CÁO SẢN XUẤT                   ║");
                Console.WriteLine("╠════════════════════════════════════════╣");
                Console.WriteLine($"║ Sản phẩm OK    : {productOK,10}         ║");
                Console.WriteLine($"║ Sản phẩm NG    : {productNG,10}         ║");
                Console.WriteLine($"║ Tổng sản phẩm  : {totalProduct,10}         ║");
                Console.WriteLine($"║ Cycle time     : {cycleTime,10} ms     ║");
                Console.WriteLine("╠════════════════════════════════════════╣");
                Console.WriteLine($"║ Thời gian chạy : {runTime,10} s      ║");
                Console.WriteLine($"║ Thời gian dừng : {downTime,10} s      ║");
                Console.WriteLine($"║ Hiệu suất      : {efficiency,10} %      ║");
                Console.WriteLine($"║ OEE            : {oee,10} %      ║");
                Console.WriteLine("╚════════════════════════════════════════╝");

                // Tính toán tỷ lệ NG
                if (totalProduct > 0)
                {
                    double ngRate = (double)productNG / totalProduct * 100;
                    Console.WriteLine($"\nTỷ lệ NG: {ngRate:F2}%");

                    if (ngRate > 5.0) // Nếu tỷ lệ NG > 5%
                    {
                        Console.WriteLine("CẢNH BÁO: Tỷ lệ NG cao!");
                        plc.SetBit(OutputAddress.Den_bao_do.GetAddress());
                        plc.SetBit(OutputAddress.Coi_bao_dong.GetAddress());
                    }
                }
            }
            finally
            {
                plc.Close();
            }
        }

        /// <summary>
        /// Ví dụ 5: Sử dụng Enum trong vòng lặp để quét tất cả sensor
        /// </summary>
        public static void Example5_ScanAllSensors()
        {
            Console.WriteLine("\n=== VÍ DỤ 5: QUÉT TẤT CẢ CẢM BIẾN ===\n");

            PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
            plc.Open();
            plc.StartCommunication();

            try
            {
                Console.WriteLine("--- Trạng thái các cảm biến đầu vào ---");

                // Danh sách các cảm biến cần quét (chỉ Bool)
                InputAddress[] sensors = new InputAddress[]
                {
                    InputAddress.Cam_bien_va_cham,
                    InputAddress.Cam_bien_quang_dau_vao,
                    InputAddress.Cam_bien_quang_dau_ra,
                    InputAddress.Cam_bien_tu,
                    InputAddress.Cam_bien_nhiet_do,
                    InputAddress.Cam_bien_ap_suat,
                    InputAddress.Home_sensor,
                    InputAddress.Limit_switch_tren,
                    InputAddress.Limit_switch_duoi
                };

                foreach (var sensor in sensors)
                {
                    bool value = plc.ReadBit(sensor.GetAddress());
                    string status = value ? "BẬT " : "TẮT";
                    Console.WriteLine($"[{status}] {sensor.GetDisplayName(),-35} ({sensor.GetAddress()})");
                }

                Console.WriteLine("\n--- Trạng thái các nút điều khiển ---");

                InputAddress[] buttons = new InputAddress[]
                {
                    InputAddress.Nut_start,
                    InputAddress.Nut_stop,
                    InputAddress.Nut_reset,
                    InputAddress.Nut_auto,
                    InputAddress.Nut_manual,
                    InputAddress.Cong_tat_bao_ve,
                    InputAddress.Cong_tat_khan_cap
                };

                foreach (var button in buttons)
                {
                    bool value = plc.ReadBit(button.GetAddress());
                    string status = value ? "NHẤN" : "-----";
                    Console.WriteLine($"[{status}] {button.GetDisplayName(),-35} ({button.GetAddress()})");
                }
            }
            finally
            {
                plc.Close();
            }
        }

        /// <summary>
        /// Ví dụ 6: Sequence điều khiển tự động
        /// </summary>
        public static void Example6_AutoSequence()
        {
            Console.WriteLine("\n=== VÍ DỤ 6: SEQUENCE ĐIỀU KHIỂN TỰ ĐỘNG ===\n");

            PLCKeyence plc = new PLCKeyence(PLCAddresses.IPAddress, PLCAddresses.Port);
            plc.Open();
            plc.StartCommunication();

            try
            {
                Console.WriteLine("Bắt đầu sequence tự động...\n");

                // Bước 1: Kiểm tra chế độ Auto
                ushort mode = plc.ReadUInt16(DataAddress.Che_do_van_hanh.GetAddress());
                if ((RunMode)mode != RunMode.Auto)
                {
                    Console.WriteLine("Hệ thống không ở chế độ Auto. Đang chuyển sang Auto...");
                    plc.WriteUInt16(DataAddress.Che_do_van_hanh.GetAddress(), (ushort)RunMode.Auto);
                }

                // Bước 2: Xi lanh hạ xuống
                Console.WriteLine("Bước 1: Xi lanh hạ xuống...");
                plc.SetBit(OutputAddress.Xi_lanh_ha.GetAddress());
                plc.ResetBit(OutputAddress.Xi_lanh_nang.GetAddress());
                System.Threading.Thread.Sleep(500);

                // Bước 3: Kẹp sản phẩm
                Console.WriteLine("Bước 2: Kẹp sản phẩm...");
                plc.SetBit(OutputAddress.Xi_lanh_kep.GetAddress());
                System.Threading.Thread.Sleep(300);

                // Bước 4: Nâng xi lanh lên
                Console.WriteLine("Bước 3: Nâng xi lanh lên...");
                plc.ResetBit(OutputAddress.Xi_lanh_ha.GetAddress());
                plc.SetBit(OutputAddress.Xi_lanh_nang.GetAddress());
                System.Threading.Thread.Sleep(500);

                // Bước 5: Đẩy ra
                Console.WriteLine("Bước 4: Đẩy sản phẩm ra...");
                plc.SetBit(OutputAddress.Xi_lanh_day.GetAddress());
                System.Threading.Thread.Sleep(500);

                // Bước 6: Thả sản phẩm
                Console.WriteLine("Bước 5: Thả sản phẩm...");
                plc.ResetBit(OutputAddress.Xi_lanh_kep.GetAddress());
                System.Threading.Thread.Sleep(200);

                // Bước 7: Kéo về
                Console.WriteLine("Bước 6: Kéo về vị trí ban đầu...");
                plc.ResetBit(OutputAddress.Xi_lanh_day.GetAddress());
                plc.SetBit(OutputAddress.Xi_lanh_keo.GetAddress());
                System.Threading.Thread.Sleep(500);

                // Bước 8: Về vị trí home
                Console.WriteLine("Bước 7: Về vị trí home...");
                plc.ResetBit(OutputAddress.Xi_lanh_nang.GetAddress());
                plc.SetBit(OutputAddress.Xi_lanh_ha.GetAddress());
                System.Threading.Thread.Sleep(500);

                // Hoàn thành
                Console.WriteLine("\nSequence hoàn thành!");

                // Tăng số lượng sản phẩm
                int productOK = plc.ReadInt32(DataAddress.So_luong_san_pham_OK.GetAddress());
                plc.WriteInt32(DataAddress.So_luong_san_pham_OK.GetAddress(), productOK + 1);

                // Bật đèn xanh
                plc.SetBit(OutputAddress.Den_bao_xanh.GetAddress());
                plc.ResetBit(OutputAddress.Den_bao_do.GetAddress());
                plc.ResetBit(OutputAddress.Den_bao_vang.GetAddress());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong sequence: {ex.Message}");
                // Bật đèn đỏ và còi báo động
                plc.SetBit(OutputAddress.Den_bao_do.GetAddress());
                plc.SetBit(OutputAddress.Coi_bao_dong.GetAddress());
            }
            finally
            {
                plc.Close();
            }
        }
    }
}
