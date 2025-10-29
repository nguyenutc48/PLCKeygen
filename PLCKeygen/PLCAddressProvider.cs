using System;
using System.Collections.Generic;
using System.Linq;

namespace PLCKeygen
{
    /// <summary>
    /// Provider để lấy địa chỉ PLC từ config đã load (KHÔNG HARD-CODE)
    /// Thay đổi PLCConfig.json và restart app là đủ, KHÔNG CẦN REBUILD
    /// </summary>
    public class PLCAddressProvider
    {
        private readonly PLCConfig _config;
        private readonly Dictionary<string, PLCAddressInfo> _inputCache;
        private readonly Dictionary<string, PLCAddressInfo> _outputCache;
        private readonly Dictionary<string, PLCAddressInfo> _dataCache;

        public PLCAddressProvider(PLCConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            // Tạo cache để tăng tốc độ truy vấn
            _inputCache = _config.Addresses.Input.ToDictionary(x => x.Name, x => x);
            _outputCache = _config.Addresses.Output.ToDictionary(x => x.Name, x => x);
            _dataCache = _config.Addresses.Data.ToDictionary(x => x.Name, x => x);
        }

        // ============================================================
        // INPUT ADDRESSES - Lấy địa chỉ Input
        // ============================================================

        /// <summary>
        /// Lấy địa chỉ Input theo tên
        /// </summary>
        public string GetInputAddress(string name)
        {
            if (_inputCache.TryGetValue(name, out PLCAddressInfo info))
            {
                return info.Address;
            }
            throw new KeyNotFoundException($"Không tìm thấy Input address: {name}");
        }

        /// <summary>
        /// Lấy thông tin đầy đủ của Input
        /// </summary>
        public PLCAddressInfo GetInputInfo(string name)
        {
            if (_inputCache.TryGetValue(name, out PLCAddressInfo info))
            {
                return info;
            }
            throw new KeyNotFoundException($"Không tìm thấy Input address: {name}");
        }

        /// <summary>
        /// Kiểm tra Input có tồn tại không
        /// </summary>
        public bool HasInput(string name)
        {
            return _inputCache.ContainsKey(name);
        }

        /// <summary>
        /// Lấy tất cả Input addresses
        /// </summary>
        public IReadOnlyList<PLCAddressInfo> GetAllInputs()
        {
            return _config.Addresses.Input.AsReadOnly();
        }

        // ============================================================
        // OUTPUT ADDRESSES - Lấy địa chỉ Output
        // ============================================================

        /// <summary>
        /// Lấy địa chỉ Output theo tên
        /// </summary>
        public string GetOutputAddress(string name)
        {
            if (_outputCache.TryGetValue(name, out PLCAddressInfo info))
            {
                return info.Address;
            }
            throw new KeyNotFoundException($"Không tìm thấy Output address: {name}");
        }

        /// <summary>
        /// Lấy thông tin đầy đủ của Output
        /// </summary>
        public PLCAddressInfo GetOutputInfo(string name)
        {
            if (_outputCache.TryGetValue(name, out PLCAddressInfo info))
            {
                return info;
            }
            throw new KeyNotFoundException($"Không tìm thấy Output address: {name}");
        }

        /// <summary>
        /// Kiểm tra Output có tồn tại không
        /// </summary>
        public bool HasOutput(string name)
        {
            return _outputCache.ContainsKey(name);
        }

        /// <summary>
        /// Lấy tất cả Output addresses
        /// </summary>
        public IReadOnlyList<PLCAddressInfo> GetAllOutputs()
        {
            return _config.Addresses.Output.AsReadOnly();
        }

        // ============================================================
        // DATA ADDRESSES - Lấy địa chỉ Data
        // ============================================================

        /// <summary>
        /// Lấy địa chỉ Data theo tên
        /// </summary>
        public string GetDataAddress(string name)
        {
            if (_dataCache.TryGetValue(name, out PLCAddressInfo info))
            {
                return info.Address;
            }
            throw new KeyNotFoundException($"Không tìm thấy Data address: {name}");
        }

        /// <summary>
        /// Lấy thông tin đầy đủ của Data
        /// </summary>
        public PLCAddressInfo GetDataInfo(string name)
        {
            if (_dataCache.TryGetValue(name, out PLCAddressInfo info))
            {
                return info;
            }
            throw new KeyNotFoundException($"Không tìm thấy Data address: {name}");
        }

        /// <summary>
        /// Kiểm tra Data có tồn tại không
        /// </summary>
        public bool HasData(string name)
        {
            return _dataCache.ContainsKey(name);
        }

        /// <summary>
        /// Lấy tất cả Data addresses
        /// </summary>
        public IReadOnlyList<PLCAddressInfo> GetAllData()
        {
            return _config.Addresses.Data.AsReadOnly();
        }

        // ============================================================
        // GENERIC METHODS - Tìm kiếm tổng quát
        // ============================================================

        /// <summary>
        /// Tìm địa chỉ trong tất cả categories (Input, Output, Data)
        /// </summary>
        public PLCAddressInfo FindAddress(string name)
        {
            if (_inputCache.TryGetValue(name, out PLCAddressInfo info))
                return info;

            if (_outputCache.TryGetValue(name, out info))
                return info;

            if (_dataCache.TryGetValue(name, out info))
                return info;

            throw new KeyNotFoundException($"Không tìm thấy address: {name}");
        }

        /// <summary>
        /// Thử tìm địa chỉ (không throw exception)
        /// </summary>
        public bool TryFindAddress(string name, out PLCAddressInfo info)
        {
            if (_inputCache.TryGetValue(name, out info))
                return true;

            if (_outputCache.TryGetValue(name, out info))
                return true;

            if (_dataCache.TryGetValue(name, out info))
                return true;

            info = null;
            return false;
        }

        /// <summary>
        /// Tìm tất cả địa chỉ theo DisplayName
        /// </summary>
        public List<PLCAddressInfo> SearchByDisplayName(string searchText)
        {
            var results = new List<PLCAddressInfo>();

            foreach (var addr in _inputCache.Values)
            {
                if (addr.DisplayName.Contains(searchText))
                    results.Add(addr);
            }

            foreach (var addr in _outputCache.Values)
            {
                if (addr.DisplayName.Contains(searchText))
                    results.Add(addr);
            }

            foreach (var addr in _dataCache.Values)
            {
                if (addr.DisplayName.Contains(searchText))
                    results.Add(addr);
            }

            return results;
        }

        // ============================================================
        // CONSTANTS NAMES - Tên địa chỉ (để dùng thay vì magic string)
        // ============================================================

        /// <summary>
        /// Class chứa tên các địa chỉ Input (constant strings)
        /// Chỉ chứa TÊN, không chứa địa chỉ PLC thực tế
        /// </summary>
        public static class InputNames
        {
            public const string Cam_bien_va_cham = "Cam_bien_va_cham";
            public const string Cam_bien_quang_dau_vao = "Cam_bien_quang_dau_vao";
            public const string Cam_bien_quang_dau_ra = "Cam_bien_quang_dau_ra";
            public const string Cam_bien_tu = "Cam_bien_tu";
            public const string Cam_bien_nhiet_do = "Cam_bien_nhiet_do";
            public const string Cam_bien_ap_suat = "Cam_bien_ap_suat";
            public const string Nut_start = "Nut_start";
            public const string Nut_stop = "Nut_stop";
            public const string Nut_reset = "Nut_reset";
            public const string Nut_auto = "Nut_auto";
            public const string Nut_manual = "Nut_manual";
            public const string Cong_tat_bao_ve = "Cong_tat_bao_ve";
            public const string Cong_tat_khan_cap = "Cong_tat_khan_cap";
            public const string Encoder_xung_A = "Encoder_xung_A";
            public const string Encoder_xung_B = "Encoder_xung_B";
            public const string Encoder_xung_Z = "Encoder_xung_Z";
            public const string Home_sensor = "Home_sensor";
            public const string Limit_switch_tren = "Limit_switch_tren";
            public const string Limit_switch_duoi = "Limit_switch_duoi";
            public const string Gia_tri_nhiet_do = "Gia_tri_nhiet_do";
            public const string Gia_tri_ap_suat = "Gia_tri_ap_suat";
            public const string Gia_tri_do_am = "Gia_tri_do_am";
            public const string Toc_do_dong_co = "Toc_do_dong_co";
            public const string Vi_tri_hien_tai = "Vi_tri_hien_tai";
        }

        /// <summary>
        /// Class chứa tên các địa chỉ Output (constant strings)
        /// </summary>
        public static class OutputNames
        {
            public const string Den_bao_xanh = "Den_bao_xanh";
            public const string Den_bao_do = "Den_bao_do";
            public const string Den_bao_vang = "Den_bao_vang";
            public const string Coi_bao_dong = "Coi_bao_dong";
            public const string Dong_co_chinh = "Dong_co_chinh";
            public const string Dong_co_phu = "Dong_co_phu";
            public const string Dong_co_quat = "Dong_co_quat";
            public const string Dong_co_bom = "Dong_co_bom";
            public const string Van_khi_1 = "Van_khi_1";
            public const string Van_khi_2 = "Van_khi_2";
            public const string Van_khi_3 = "Van_khi_3";
            public const string Van_dien_tu = "Van_dien_tu";
            public const string Van_thuong_dong = "Van_thuong_dong";
            public const string Van_thuong_mo = "Van_thuong_mo";
            public const string Xi_lanh_nang = "Xi_lanh_nang";
            public const string Xi_lanh_ha = "Xi_lanh_ha";
            public const string Xi_lanh_day = "Xi_lanh_day";
            public const string Xi_lanh_keo = "Xi_lanh_keo";
            public const string Xi_lanh_kep = "Xi_lanh_kep";
            public const string May_han = "May_han";
            public const string May_cat = "May_cat";
            public const string May_khoan = "May_khoan";
            public const string Bang_tai = "Bang_tai";
            public const string May_kiem_tra = "May_kiem_tra";
            public const string Robot_pick_place = "Robot_pick_place";
            public const string Toc_do_dat_dong_co = "Toc_do_dat_dong_co";
            public const string Vi_tri_dat = "Vi_tri_dat";
            public const string Gia_tri_PWM = "Gia_tri_PWM";
            public const string Nhiet_do_dat = "Nhiet_do_dat";
        }

        /// <summary>
        /// Class chứa tên các địa chỉ Data (constant strings)
        /// </summary>
        public static class DataNames
        {
            public const string So_luong_san_pham_OK = "So_luong_san_pham_OK";
            public const string So_luong_san_pham_NG = "So_luong_san_pham_NG";
            public const string Tong_san_pham = "Tong_san_pham";
            public const string Tong_thoi_gian_chay = "Tong_thoi_gian_chay";
            public const string Thoi_gian_dung_may = "Thoi_gian_dung_may";
            public const string So_lan_bao_tri = "So_lan_bao_tri";
            public const string So_lan_loi = "So_lan_loi";
            public const string Ma_loi_hien_tai = "Ma_loi_hien_tai";
            public const string Trang_thai_he_thong = "Trang_thai_he_thong";
            public const string Che_do_van_hanh = "Che_do_van_hanh";
            public const string Trang_thai_dong_co = "Trang_thai_dong_co";
            public const string Trang_thai_van_khi = "Trang_thai_van_khi";
            public const string Trang_thai_cam_bien = "Trang_thai_cam_bien";
            public const string Cycle_time = "Cycle_time";
            public const string Target_count = "Target_count";
            public const string Hieu_suat = "Hieu_suat";
            public const string OEE = "OEE";
            public const string Ngay_hien_tai = "Ngay_hien_tai";
            public const string Gio_hien_tai = "Gio_hien_tai";
            public const string Version_firmware = "Version_firmware";
            public const string Serial_number = "Serial_number";
        }
    }
}
