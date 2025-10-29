using System;
using System.ComponentModel;

namespace PLCKeygen
{
    /// <summary>
    /// Enum cho các địa chỉ Input (Đầu vào)
    /// Sử dụng extension method để lấy địa chỉ PLC thực tế
    /// </summary>
    public enum InputAddress
    {
        [Description("R0")]
        [DisplayName("Cảm biến va chạm")]
        Cam_bien_va_cham,

        [Description("R1")]
        [DisplayName("Cảm biến quang đầu vào")]
        Cam_bien_quang_dau_vao,

        [Description("R2")]
        [DisplayName("Cảm biến quang đầu ra")]
        Cam_bien_quang_dau_ra,

        [Description("R3")]
        [DisplayName("Cảm biến từ")]
        Cam_bien_tu,

        [Description("R4")]
        [DisplayName("Cảm biến nhiệt độ")]
        Cam_bien_nhiet_do,

        [Description("R5")]
        [DisplayName("Cảm biến áp suất")]
        Cam_bien_ap_suat,

        [Description("R10")]
        [DisplayName("Nút Start")]
        Nut_start,

        [Description("R11")]
        [DisplayName("Nút Stop")]
        Nut_stop,

        [Description("R12")]
        [DisplayName("Nút Reset")]
        Nut_reset,

        [Description("R13")]
        [DisplayName("Nút chế độ Auto")]
        Nut_auto,

        [Description("R14")]
        [DisplayName("Nút chế độ Manual")]
        Nut_manual,

        [Description("R15")]
        [DisplayName("Công tắc bảo vệ")]
        Cong_tat_bao_ve,

        [Description("R16")]
        [DisplayName("Công tắc khẩn cấp")]
        Cong_tat_khan_cap,

        [Description("R20")]
        [DisplayName("Encoder xung A")]
        Encoder_xung_A,

        [Description("R21")]
        [DisplayName("Encoder xung B")]
        Encoder_xung_B,

        [Description("R22")]
        [DisplayName("Encoder xung Z (zero)")]
        Encoder_xung_Z,

        [Description("R23")]
        [DisplayName("Cảm biến Home position")]
        Home_sensor,

        [Description("R24")]
        [DisplayName("Limit switch trên")]
        Limit_switch_tren,

        [Description("R25")]
        [DisplayName("Limit switch dưới")]
        Limit_switch_duoi,

        [Description("DM100")]
        [DisplayName("Giá trị nhiệt độ")]
        Gia_tri_nhiet_do,

        [Description("DM101")]
        [DisplayName("Giá trị áp suất")]
        Gia_tri_ap_suat,

        [Description("DM102")]
        [DisplayName("Giá trị độ ẩm")]
        Gia_tri_do_am,

        [Description("DM103")]
        [DisplayName("Tốc độ động cơ")]
        Toc_do_dong_co,

        [Description("DM104")]
        [DisplayName("Vị trí hiện tại")]
        Vi_tri_hien_tai
    }

    /// <summary>
    /// Enum cho các địa chỉ Output (Đầu ra)
    /// </summary>
    public enum OutputAddress
    {
        [Description("R500")]
        [DisplayName("Đèn báo xanh")]
        Den_bao_xanh,

        [Description("R501")]
        [DisplayName("Đèn báo đỏ")]
        Den_bao_do,

        [Description("R502")]
        [DisplayName("Đèn báo vàng")]
        Den_bao_vang,

        [Description("R503")]
        [DisplayName("Còi báo động")]
        Coi_bao_dong,

        [Description("R510")]
        [DisplayName("Động cơ chính")]
        Dong_co_chinh,

        [Description("R511")]
        [DisplayName("Động cơ phụ")]
        Dong_co_phu,

        [Description("R512")]
        [DisplayName("Động cơ quạt")]
        Dong_co_quat,

        [Description("R513")]
        [DisplayName("Động cơ bơm")]
        Dong_co_bom,

        [Description("R520")]
        [DisplayName("Van khí số 1")]
        Van_khi_1,

        [Description("R521")]
        [DisplayName("Van khí số 2")]
        Van_khi_2,

        [Description("R522")]
        [DisplayName("Van khí số 3")]
        Van_khi_3,

        [Description("R523")]
        [DisplayName("Van điện từ")]
        Van_dien_tu,

        [Description("R524")]
        [DisplayName("Van thường đóng")]
        Van_thuong_dong,

        [Description("R525")]
        [DisplayName("Van thường mở")]
        Van_thuong_mo,

        [Description("R530")]
        [DisplayName("Xi lanh nâng")]
        Xi_lanh_nang,

        [Description("R531")]
        [DisplayName("Xi lanh hạ")]
        Xi_lanh_ha,

        [Description("R532")]
        [DisplayName("Xi lanh đẩy")]
        Xi_lanh_day,

        [Description("R533")]
        [DisplayName("Xi lanh kéo")]
        Xi_lanh_keo,

        [Description("R534")]
        [DisplayName("Xi lanh kẹp")]
        Xi_lanh_kep,

        [Description("R540")]
        [DisplayName("Máy hàn")]
        May_han,

        [Description("R541")]
        [DisplayName("Máy cắt")]
        May_cat,

        [Description("R542")]
        [DisplayName("Máy khoan")]
        May_khoan,

        [Description("R550")]
        [DisplayName("Băng tải")]
        Bang_tai,

        [Description("R560")]
        [DisplayName("Máy kiểm tra")]
        May_kiem_tra,

        [Description("R570")]
        [DisplayName("Robot pick and place")]
        Robot_pick_place,

        [Description("DM500")]
        [DisplayName("Tốc độ đặt động cơ")]
        Toc_do_dat_dong_co,

        [Description("DM501")]
        [DisplayName("Vị trí đặt")]
        Vi_tri_dat,

        [Description("DM503")]
        [DisplayName("Giá trị PWM")]
        Gia_tri_PWM,

        [Description("DM504")]
        [DisplayName("Nhiệt độ đặt")]
        Nhiet_do_dat
    }

    /// <summary>
    /// Enum cho các địa chỉ Data (Dữ liệu)
    /// </summary>
    public enum DataAddress
    {
        [Description("DM1000")]
        [DisplayName("Số lượng sản phẩm OK")]
        So_luong_san_pham_OK,

        [Description("DM1002")]
        [DisplayName("Số lượng sản phẩm NG")]
        So_luong_san_pham_NG,

        [Description("DM1004")]
        [DisplayName("Tổng sản phẩm")]
        Tong_san_pham,

        [Description("DM1006")]
        [DisplayName("Tổng thời gian chạy (giây)")]
        Tong_thoi_gian_chay,

        [Description("DM1008")]
        [DisplayName("Thời gian dừng máy (giây)")]
        Thoi_gian_dung_may,

        [Description("DM1010")]
        [DisplayName("Số lần bảo trì")]
        So_lan_bao_tri,

        [Description("DM1011")]
        [DisplayName("Số lần lỗi")]
        So_lan_loi,

        [Description("DM1012")]
        [DisplayName("Mã lỗi hiện tại")]
        Ma_loi_hien_tai,

        [Description("DM2000")]
        [DisplayName("Trạng thái hệ thống")]
        Trang_thai_he_thong,

        [Description("DM2001")]
        [DisplayName("Chế độ vận hành (0:Stop, 1:Manual, 2:Auto)")]
        Che_do_van_hanh,

        [Description("DM2002")]
        [DisplayName("Trạng thái động cơ (bitwise)")]
        Trang_thai_dong_co,

        [Description("DM2003")]
        [DisplayName("Trạng thái van khí (bitwise)")]
        Trang_thai_van_khi,

        [Description("DM2004")]
        [DisplayName("Trạng thái cảm biến (bitwise)")]
        Trang_thai_cam_bien,

        [Description("DM2010")]
        [DisplayName("Cycle time (ms)")]
        Cycle_time,

        [Description("DM2020")]
        [DisplayName("Số lượng mục tiêu")]
        Target_count,

        [Description("DM2022")]
        [DisplayName("Hiệu suất (%)")]
        Hieu_suat,

        [Description("DM2023")]
        [DisplayName("OEE - Overall Equipment Effectiveness (%)")]
        OEE,

        [Description("DM3000")]
        [DisplayName("Ngày hiện tại (YYYYMMDD)")]
        Ngay_hien_tai,

        [Description("DM3002")]
        [DisplayName("Giờ hiện tại (HHMMSS)")]
        Gio_hien_tai,

        [Description("DM9000")]
        [DisplayName("Version firmware")]
        Version_firmware,

        [Description("DM9001")]
        [DisplayName("Serial number thiết bị")]
        Serial_number
    }

    /// <summary>
    /// Enum cho chế độ vận hành
    /// </summary>
    public enum RunMode : ushort
    {
        Stop = 0,
        Manual = 1,
        Auto = 2
    }

    /// <summary>
    /// Extension methods để lấy địa chỉ PLC từ enum
    /// </summary>
    public static class PLCAddressExtensions
    {
        /// <summary>
        /// Lấy địa chỉ PLC thực tế từ enum Input
        /// </summary>
        public static string GetAddress(this InputAddress input)
        {
            var fieldInfo = input.GetType().GetField(input.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : input.ToString();
        }

        /// <summary>
        /// Lấy địa chỉ PLC thực tế từ enum Output
        /// </summary>
        public static string GetAddress(this OutputAddress output)
        {
            var fieldInfo = output.GetType().GetField(output.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : output.ToString();
        }

        /// <summary>
        /// Lấy địa chỉ PLC thực tế từ enum Data
        /// </summary>
        public static string GetAddress(this DataAddress data)
        {
            var fieldInfo = data.GetType().GetField(data.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : data.ToString();
        }

        /// <summary>
        /// Lấy tên hiển thị tiếng Việt từ enum Input
        /// </summary>
        public static string GetDisplayName(this InputAddress input)
        {
            var fieldInfo = input.GetType().GetField(input.ToString());
            var attributes = (DisplayNameAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            return attributes.Length > 0 ? attributes[0].DisplayName : input.ToString();
        }

        /// <summary>
        /// Lấy tên hiển thị tiếng Việt từ enum Output
        /// </summary>
        public static string GetDisplayName(this OutputAddress output)
        {
            var fieldInfo = output.GetType().GetField(output.ToString());
            var attributes = (DisplayNameAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            return attributes.Length > 0 ? attributes[0].DisplayName : output.ToString();
        }

        /// <summary>
        /// Lấy tên hiển thị tiếng Việt từ enum Data
        /// </summary>
        public static string GetDisplayName(this DataAddress data)
        {
            var fieldInfo = data.GetType().GetField(data.ToString());
            var attributes = (DisplayNameAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            return attributes.Length > 0 ? attributes[0].DisplayName : data.ToString();
        }
    }
}
