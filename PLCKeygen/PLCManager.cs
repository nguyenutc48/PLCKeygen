using System;
using System.IO;

namespace PLCKeygen
{
    /// <summary>
    /// Singleton Manager để quản lý config và kết nối PLC
    /// Tự động load config khi khởi tạo lần đầu
    /// </summary>
    public sealed class PLCManager
    {
        private static readonly object _lock = new object();
        private static PLCManager _instance;
        private PLCConfigManager _configManager;
        private PLCKeyence _plc;
        private PLCAddressProvider _addressProvider;
        private bool _isConfigLoaded;
        private bool _isConnected;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PLCManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new PLCManager();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Config Manager
        /// </summary>
        public PLCConfigManager ConfigManager => _configManager;

        /// <summary>
        /// PLC Instance
        /// </summary>
        public PLCKeyence PLC => _plc;

        /// <summary>
        /// Config đã được load chưa
        /// </summary>
        public bool IsConfigLoaded => _isConfigLoaded;

        /// <summary>
        /// PLC đã kết nối chưa
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Thông tin PLC hiện tại
        /// </summary>
        public PLCConfig CurrentConfig => _configManager?.Config;

        /// <summary>
        /// Address Provider - Lấy địa chỉ PLC động từ config (KHÔNG HARD-CODE)
        /// Thay đổi PLCConfig.json và restart app, KHÔNG CẦN REBUILD
        /// </summary>
        public PLCAddressProvider Addresses => _addressProvider;

        /// <summary>
        /// Private constructor
        /// </summary>
        private PLCManager()
        {
            _configManager = new PLCConfigManager();
            _isConfigLoaded = false;
            _isConnected = false;
        }

        /// <summary>
        /// Khởi tạo và load config từ file
        /// Gọi method này trong Program.cs hoặc Form_Load
        /// </summary>
        /// <param name="configFilePath">Đường dẫn đến file PLCConfig.json</param>
        /// <returns>True nếu load thành công</returns>
        public bool Initialize(string configFilePath = "PLCConfig.json")
        {
            try
            {
                // Kiểm tra file tồn tại
                if (!File.Exists(configFilePath))
                {
                    Console.WriteLine($"CẢNH BÁO: File config không tồn tại: {configFilePath}");

                    // Tìm file trong thư mục PLCKeygen
                    string altPath = Path.Combine("PLCKeygen", configFilePath);
                    if (File.Exists(altPath))
                    {
                        configFilePath = altPath;
                        Console.WriteLine($"Tìm thấy file tại: {altPath}");
                    }
                    else
                    {
                        Console.WriteLine("Không tìm thấy file config!");
                        return false;
                    }
                }

                // Load config
                if (_configManager.LoadConfig(configFilePath))
                {
                    _isConfigLoaded = true;
                    Console.WriteLine("✓ Config đã được load thành công!");
                    Console.WriteLine($"  PLC Name: {CurrentConfig.PLCName}");
                    Console.WriteLine($"  IP Address: {CurrentConfig.IPAddress}");
                    Console.WriteLine($"  Port: {CurrentConfig.Port}");
                    Console.WriteLine($"  Input addresses: {CurrentConfig.Addresses.Input.Count}");
                    Console.WriteLine($"  Output addresses: {CurrentConfig.Addresses.Output.Count}");
                    Console.WriteLine($"  Data addresses: {CurrentConfig.Addresses.Data.Count}");

                    // Tạo PLC instance
                    _plc = _configManager.CreatePLCInstance();

                    // Tạo Address Provider - Load địa chỉ động từ config
                    _addressProvider = new PLCAddressProvider(CurrentConfig);
                    Console.WriteLine("✓ Address Provider đã được khởi tạo (Dynamic addresses from JSON)");

                    return true;
                }
                else
                {
                    Console.WriteLine("✗ Không thể load config!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Lỗi khi khởi tạo PLCManager: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Kết nối đến PLC
        /// </summary>
        /// <returns>True nếu kết nối thành công</returns>
        public bool Connect()
        {
            if (!_isConfigLoaded)
            {
                Console.WriteLine("✗ Config chưa được load! Gọi Initialize() trước.");
                return false;
            }

            if (_isConnected)
            {
                Console.WriteLine("⚠ PLC đã kết nối rồi!");
                return true;
            }

            try
            {
                _plc.Open();
                _plc.StartCommunication();
                _isConnected = true;
                Console.WriteLine($"✓ Đã kết nối đến PLC: {CurrentConfig.PLCName} ({CurrentConfig.IPAddress}:{CurrentConfig.Port})");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Không thể kết nối PLC: {ex.Message}");
                _isConnected = false;
                return false;
            }
        }

        /// <summary>
        /// Ngắt kết nối PLC
        /// </summary>
        public void Disconnect()
        {
            if (_plc != null && _isConnected)
            {
                try
                {
                    _plc.Close();
                    _isConnected = false;
                    Console.WriteLine("✓ Đã ngắt kết nối PLC");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠ Lỗi khi ngắt kết nối: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Reload config từ file
        /// </summary>
        /// <param name="configFilePath">Đường dẫn file config</param>
        /// <returns>True nếu reload thành công</returns>
        public bool ReloadConfig(string configFilePath = "PLCConfig.json")
        {
            // Ngắt kết nối trước khi reload
            if (_isConnected)
            {
                Disconnect();
            }

            // Load lại config
            return Initialize(configFilePath);
        }

        /// <summary>
        /// Lấy thông tin địa chỉ từ config
        /// </summary>
        public PLCAddressInfo GetAddressInfo(string category, string name)
        {
            if (!_isConfigLoaded)
            {
                throw new InvalidOperationException("Config chưa được load!");
            }

            return _configManager.GetAddressInfo(category, name);
        }

        /// <summary>
        /// Kiểm tra kết nối PLC
        /// </summary>
        public bool TestConnection()
        {
            if (!_isConnected)
            {
                return false;
            }

            try
            {
                // Thử đọc một địa chỉ để test
                // Giả sử R0 luôn tồn tại
                _plc.ReadBit("R0");
                return true;
            }
            catch
            {
                _isConnected = false;
                return false;
            }
        }

        /// <summary>
        /// Tự động kết nối lại nếu mất kết nối
        /// </summary>
        public bool AutoReconnect()
        {
            if (_isConnected && TestConnection())
            {
                return true; // Đã kết nối tốt
            }

            Console.WriteLine("⚠ Phát hiện mất kết nối, đang thử kết nối lại...");

            if (_isConnected)
            {
                Disconnect();
            }

            return Connect();
        }
    }
}
