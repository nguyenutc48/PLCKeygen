using System;

namespace PLCKeygen
{
    /// <summary>
    /// VÍ DỤ SỬ DỤNG ĐỊA CHỈ ĐỘNG TỪ JSON
    /// Thay đổi PLCConfig.json và RESTART APP là đủ, KHÔNG CẦN REBUILD!
    /// </summary>
    public class DynamicAddressExample
    {
        /// <summary>
        /// Ví dụ 1: Sử dụng địa chỉ động cơ bản
        /// THAY ĐỔI JSON -> RESTART -> ÁP DỤNG NGAY!
        /// </summary>
        public static void Example1_BasicDynamicAddress()
        {
            Console.WriteLine("=== VÍ DỤ 1: SỬ DỤNG ĐỊA CHỈ ĐỘNG ===\n");

            // Khởi tạo (chỉ cần 1 lần)
            if (!PLCManager.Instance.Initialize("PLCConfig.json"))
            {
                Console.WriteLine("Không thể load config!");
                return;
            }

            if (!PLCManager.Instance.Connect())
            {
                Console.WriteLine("Không thể kết nối PLC!");
                return;
            }

            PLCKeyence plc = PLCManager.Instance.PLC;
            PLCAddressProvider addr = PLCManager.Instance.Addresses;

            // ✅ CÁCH DÙNG ĐỘNG - Địa chỉ load từ JSON
            // Thay đổi JSON: "Cam_bien_va_cham": "R0" -> "R10"
            // Restart app -> Địa chỉ tự động thay đổi!

            string sensorAddr = addr.GetInputAddress("Cam_bien_va_cham");
            bool sensorValue = plc.ReadBit(sensorAddr);

            Console.WriteLine($"Địa chỉ: {sensorAddr}");  // Load từ JSON
            Console.WriteLine($"Giá trị: {sensorValue}");

            PLCManager.Instance.Disconnect();
        }

        /// <summary>
        /// Ví dụ 2: Sử dụng với constant names để tránh magic string
        /// </summary>
        public static void Example2_UseConstantNames()
        {
            Console.WriteLine("\n=== VÍ DỤ 2: DÙNG CONSTANT NAMES ===\n");

            if (!EnsureConnected()) return;

            PLCKeyence plc = PLCManager.Instance.PLC;
            PLCAddressProvider addr = PLCManager.Instance.Addresses;

            // ✅ Sử dụng constant names (tránh gõ sai tên)
            // Địa chỉ vẫn load động từ JSON!

            // Đọc Input
            bool sensor = plc.ReadBit(
                addr.GetInputAddress(PLCAddressProvider.InputNames.Cam_bien_va_cham));

            bool startBtn = plc.ReadBit(
                addr.GetInputAddress(PLCAddressProvider.InputNames.Nut_start));

            ushort speed = plc.ReadUInt16(
                addr.GetInputAddress(PLCAddressProvider.InputNames.Toc_do_dong_co));

            Console.WriteLine($"Cảm biến: {sensor}");
            Console.WriteLine($"Nút start: {startBtn}");
            Console.WriteLine($"Tốc độ: {speed} RPM");

            // Ghi Output
            plc.SetBit(
                addr.GetOutputAddress(PLCAddressProvider.OutputNames.Den_bao_xanh));

            plc.ResetBit(
                addr.GetOutputAddress(PLCAddressProvider.OutputNames.Den_bao_do));

            Console.WriteLine("Đã điều khiển output");

            // Đọc/Ghi Data
            int productOK = plc.ReadInt32(
                addr.GetDataAddress(PLCAddressProvider.DataNames.So_luong_san_pham_OK));

            plc.WriteInt32(
                addr.GetDataAddress(PLCAddressProvider.DataNames.So_luong_san_pham_OK),
                productOK + 1);

            Console.WriteLine($"Số sản phẩm: {productOK + 1}");
        }

        /// <summary>
        /// Ví dụ 3: Lấy thông tin đầy đủ của địa chỉ
        /// </summary>
        public static void Example3_GetFullAddressInfo()
        {
            Console.WriteLine("\n=== VÍ DỤ 3: LẤY THÔNG TIN ĐẦY ĐỦ ===\n");

            if (!PLCManager.Instance.IsConfigLoaded)
            {
                PLCManager.Instance.Initialize("PLCConfig.json");
            }

            PLCAddressProvider addr = PLCManager.Instance.Addresses;

            // Lấy thông tin đầy đủ (bao gồm DisplayName tiếng Việt)
            PLCAddressInfo sensorInfo = addr.GetInputInfo("Cam_bien_va_cham");

            Console.WriteLine($"Tên: {sensorInfo.Name}");
            Console.WriteLine($"Tên hiển thị: {sensorInfo.DisplayName}");
            Console.WriteLine($"Địa chỉ PLC: {sensorInfo.Address}");
            Console.WriteLine($"Kiểu dữ liệu: {sensorInfo.DataType}");

            // Sử dụng thông tin này
            if (PLCManager.Instance.IsConnected)
            {
                PLCKeyence plc = PLCManager.Instance.PLC;
                bool value = plc.ReadBit(sensorInfo.Address);
                Console.WriteLine($"\n{sensorInfo.DisplayName}: {value}");
            }
        }

        /// <summary>
        /// Ví dụ 4: Tìm địa chỉ theo tên hiển thị (Search)
        /// </summary>
        public static void Example4_SearchByDisplayName()
        {
            Console.WriteLine("\n=== VÍ DỤ 4: TÌM KIẾM ĐỊA CHỈ ===\n");

            if (!PLCManager.Instance.IsConfigLoaded)
            {
                PLCManager.Instance.Initialize("PLCConfig.json");
            }

            PLCAddressProvider addr = PLCManager.Instance.Addresses;

            // Tìm tất cả địa chỉ có chứa "động cơ"
            var results = addr.SearchByDisplayName("động cơ");

            Console.WriteLine($"Tìm thấy {results.Count} kết quả cho 'động cơ':\n");

            foreach (var info in results)
            {
                Console.WriteLine($"- {info.DisplayName}");
                Console.WriteLine($"  Tên: {info.Name}");
                Console.WriteLine($"  Địa chỉ: {info.Address}");
                Console.WriteLine($"  Kiểu: {info.DataType}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Ví dụ 5: Liệt kê tất cả địa chỉ Input/Output/Data
        /// </summary>
        public static void Example5_ListAllAddresses()
        {
            Console.WriteLine("\n=== VÍ DỤ 5: LIỆT KÊ TẤT CẢ ĐỊA CHỈ ===\n");

            if (!PLCManager.Instance.IsConfigLoaded)
            {
                PLCManager.Instance.Initialize("PLCConfig.json");
            }

            PLCAddressProvider addr = PLCManager.Instance.Addresses;

            // Liệt kê Input
            Console.WriteLine("=== INPUT ADDRESSES ===");
            var inputs = addr.GetAllInputs();
            foreach (var input in inputs)
            {
                Console.WriteLine($"{input.Name,-30} {input.Address,-10} {input.DisplayName}");
            }

            // Liệt kê Output
            Console.WriteLine("\n=== OUTPUT ADDRESSES ===");
            var outputs = addr.GetAllOutputs();
            foreach (var output in outputs)
            {
                Console.WriteLine($"{output.Name,-30} {output.Address,-10} {output.DisplayName}");
            }

            // Liệt kê Data
            Console.WriteLine("\n=== DATA ADDRESSES ===");
            var data = addr.GetAllData();
            foreach (var d in data)
            {
                Console.WriteLine($"{d.Name,-30} {d.Address,-10} {d.DisplayName}");
            }

            Console.WriteLine($"\nTổng: {inputs.Count} Input, {outputs.Count} Output, {data.Count} Data");
        }

        /// <summary>
        /// Ví dụ 6: Sử dụng trong vòng lặp - Đọc nhiều sensor
        /// </summary>
        public static void Example6_LoopReadSensors()
        {
            Console.WriteLine("\n=== VÍ DỤ 6: ĐỌC NHIỀU SENSOR ===\n");

            if (!EnsureConnected()) return;

            PLCKeyence plc = PLCManager.Instance.PLC;
            PLCAddressProvider addr = PLCManager.Instance.Addresses;

            // Danh sách các sensor cần đọc
            string[] sensorNames = new string[]
            {
                "Cam_bien_va_cham",
                "Cam_bien_quang_dau_vao",
                "Cam_bien_quang_dau_ra",
                "Cam_bien_tu",
                "Home_sensor"
            };

            Console.WriteLine("Trạng thái các cảm biến:\n");

            foreach (string name in sensorNames)
            {
                // Kiểm tra sensor có tồn tại không
                if (addr.HasInput(name))
                {
                    // Lấy thông tin
                    PLCAddressInfo info = addr.GetInputInfo(name);

                    // Đọc giá trị
                    bool value = plc.ReadBit(info.Address);

                    // Hiển thị
                    string status = value ? "[BẬT ]" : "[TẮT]";
                    Console.WriteLine($"{status} {info.DisplayName,-35} ({info.Address})");
                }
                else
                {
                    Console.WriteLine($"[???] {name} - Không tìm thấy trong config");
                }
            }
        }

        /// <summary>
        /// Ví dụ 7: Thay đổi JSON trong runtime và reload
        /// </summary>
        public static void Example7_RuntimeReload()
        {
            Console.WriteLine("\n=== VÍ DỤ 7: RELOAD CONFIG RUNTIME ===\n");

            // Load config lần đầu
            if (!PLCManager.Instance.Initialize("PLCConfig.json"))
            {
                Console.WriteLine("Không thể load config!");
                return;
            }

            PLCAddressProvider addr = PLCManager.Instance.Addresses;

            // Đọc địa chỉ hiện tại
            string oldAddress = addr.GetInputAddress("Cam_bien_va_cham");
            Console.WriteLine($"Địa chỉ cũ: {oldAddress}");

            Console.WriteLine("\n--- Bây giờ bạn có thể: ---");
            Console.WriteLine("1. Mở file PLCConfig.json");
            Console.WriteLine("2. Thay đổi địa chỉ 'Cam_bien_va_cham' từ 'R0' thành 'R10'");
            Console.WriteLine("3. Lưu file");
            Console.WriteLine("\nNhấn Enter để reload config...");
            Console.ReadLine();

            // Reload config
            if (PLCManager.Instance.ReloadConfig("PLCConfig.json"))
            {
                Console.WriteLine("\n✓ Config đã được reload!");

                // Đọc địa chỉ mới
                string newAddress = addr.GetInputAddress("Cam_bien_va_cham");
                Console.WriteLine($"Địa chỉ mới: {newAddress}");

                if (oldAddress != newAddress)
                {
                    Console.WriteLine("\n✓ Địa chỉ đã thay đổi thành công!");
                }
                else
                {
                    Console.WriteLine("\nĐịa chỉ không thay đổi (có thể bạn chưa sửa JSON)");
                }
            }
            else
            {
                Console.WriteLine("✗ Reload thất bại!");
            }
        }

        /// <summary>
        /// Ví dụ 8: So sánh HARD-CODE vs DYNAMIC
        /// </summary>
        public static void Example8_CompareHardCodeVsDynamic()
        {
            Console.WriteLine("\n=== VÍ DỤ 8: SO SÁNH HARD-CODE VS DYNAMIC ===\n");

            if (!EnsureConnected()) return;

            PLCKeyence plc = PLCManager.Instance.PLC;
            PLCAddressProvider addr = PLCManager.Instance.Addresses;

            Console.WriteLine("--- CÁCH 1: HARD-CODE (KHÔNG KHUYẾN NGHỊ) ---");
            Console.WriteLine("bool sensor = plc.ReadBit(\"R0\");");
            Console.WriteLine("❌ Thay đổi địa chỉ -> Phải REBUILD code");
            Console.WriteLine("❌ Dễ gõ sai địa chỉ");
            Console.WriteLine("❌ Không có IntelliSense\n");

            Console.WriteLine("--- CÁCH 2: DÙNG CONST (CŨ) ---");
            Console.WriteLine("bool sensor = plc.ReadBit(PLCAddresses.Input.Cam_bien_va_cham);");
            Console.WriteLine("❌ Thay đổi địa chỉ -> Phải GENERATE lại -> REBUILD");
            Console.WriteLine("✅ Có IntelliSense");
            Console.WriteLine("✅ Tránh gõ sai\n");

            Console.WriteLine("--- CÁCH 3: DÙNG DYNAMIC ADDRESS (KHUYẾN NGHỊ) ---");
            Console.WriteLine("string addr = addr.GetInputAddress(PLCAddressProvider.InputNames.Cam_bien_va_cham);");
            Console.WriteLine("bool sensor = plc.ReadBit(addr);");
            Console.WriteLine("✅ Thay đổi JSON -> RESTART app -> Địa chỉ tự động cập nhật!");
            Console.WriteLine("✅ KHÔNG CẦN REBUILD!");
            Console.WriteLine("✅ Có IntelliSense (constant names)");
            Console.WriteLine("✅ Tránh gõ sai");
            Console.WriteLine("✅ Lấy được DisplayName tiếng Việt\n");

            // Demo thực tế
            Console.WriteLine("--- DEMO THỰC TẾ ---");

            string dynamicAddr = addr.GetInputAddress(PLCAddressProvider.InputNames.Cam_bien_va_cham);
            PLCAddressInfo info = addr.GetInputInfo(PLCAddressProvider.InputNames.Cam_bien_va_cham);
            bool value = plc.ReadBit(dynamicAddr);

            Console.WriteLine($"Địa chỉ: {dynamicAddr} (load từ JSON)");
            Console.WriteLine($"Tên hiển thị: {info.DisplayName}");
            Console.WriteLine($"Giá trị: {value}");
            Console.WriteLine("\n🎉 Bây giờ thay đổi JSON và restart app để test!");
        }

        /// <summary>
        /// Ví dụ 9: Sử dụng trong Windows Forms với Timer
        /// </summary>
        public static void Example9_WindowsFormsPattern()
        {
            Console.WriteLine("\n=== VÍ DỤ 9: WINDOWS FORMS PATTERN ===\n");

            Console.WriteLine(@"
// Trong Form1_Load:
private void Form1_Load(object sender, EventArgs e)
{
    // Timer để cập nhật UI
    timer1.Interval = 100;
    timer1.Tick += Timer1_Tick;
    timer1.Start();
}

// Trong Timer_Tick:
private void Timer1_Tick(object sender, EventArgs e)
{
    if (!PLCManager.Instance.IsConnected) return;

    PLCKeyence plc = PLCManager.Instance.PLC;
    PLCAddressProvider addr = PLCManager.Instance.Addresses;

    // Đọc địa chỉ động từ JSON
    bool motor = plc.ReadBit(
        addr.GetOutputAddress(PLCAddressProvider.OutputNames.Dong_co_chinh));

    ushort speed = plc.ReadUInt16(
        addr.GetInputAddress(PLCAddressProvider.InputNames.Toc_do_dong_co));

    // Cập nhật UI
    lblMotor.Text = motor ? ""CHẠY"" : ""DỪNG"";
    lblSpeed.Text = $""{speed} RPM"";
}

// Trong Button Click:
private void btnStart_Click(object sender, EventArgs e)
{
    PLCKeyence plc = PLCManager.Instance.PLC;
    PLCAddressProvider addr = PLCManager.Instance.Addresses;

    // Bật động cơ - Địa chỉ load từ JSON
    plc.SetBit(addr.GetOutputAddress(PLCAddressProvider.OutputNames.Dong_co_chinh));

    // Bật đèn xanh
    plc.SetBit(addr.GetOutputAddress(PLCAddressProvider.OutputNames.Den_bao_xanh));
}
");
        }

        /// <summary>
        /// Helper: Đảm bảo PLC đã kết nối
        /// </summary>
        private static bool EnsureConnected()
        {
            if (!PLCManager.Instance.IsConfigLoaded)
            {
                if (!PLCManager.Instance.Initialize("PLCConfig.json"))
                {
                    Console.WriteLine("Không thể load config!");
                    return false;
                }
            }

            if (!PLCManager.Instance.IsConnected)
            {
                if (!PLCManager.Instance.Connect())
                {
                    Console.WriteLine("PLC chưa kết nối!");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Main demo - Chạy tất cả ví dụ
        /// </summary>
        public static void RunAllExamples()
        {
            Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
            Console.WriteLine("║     DYNAMIC ADDRESS SYSTEM - KHÔNG CẦN REBUILD!       ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

            try
            {
                Example1_BasicDynamicAddress();
                Console.WriteLine("\nNhấn phím để tiếp tục...");
                Console.ReadKey();

                Example2_UseConstantNames();
                Console.WriteLine("\nNhấn phím để tiếp tục...");
                Console.ReadKey();

                Example3_GetFullAddressInfo();
                Console.WriteLine("\nNhấn phím để tiếp tục...");
                Console.ReadKey();

                Example4_SearchByDisplayName();
                Console.WriteLine("\nNhấn phím để tiếp tục...");
                Console.ReadKey();

                Example5_ListAllAddresses();
                Console.WriteLine("\nNhấn phím để tiếp tục...");
                Console.ReadKey();

                Example6_LoopReadSensors();
                Console.WriteLine("\nNhấn phím để tiếp tục...");
                Console.ReadKey();

                Example8_CompareHardCodeVsDynamic();

                Console.WriteLine("\n\n✓ Hoàn thành tất cả ví dụ!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nLỗi: {ex.Message}");
            }
            finally
            {
                PLCManager.Instance.Disconnect();
            }
        }
    }
}
