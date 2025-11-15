using System.Collections.Generic;

namespace PLCKeygen
{
    /// <summary>
    /// Lookup dictionary for alarm, error, and status messages
    /// Based on the provided alarm/error code table
    /// </summary>
    public static class AlarmErrorMessages
    {
        // Alarm messages (Warning level)
        public static readonly Dictionary<int, string> AlarmMessages = new Dictionary<int, string>
        {
            { 0, "Bình thường" },
            { 2, "Qúa thời gian khởi tạo X2" },
            { 3, "Dừng khẩn cấp" },
            { 4, "Lỗi Driver X2" },
            { 5, "Lỗi trục Y2" },
            { 6, "Qúa thời gian khởi tạo Y2" },
            { 7, "Lỗi Driver Y2" },
            { 8, "Lỗi trục Z2" },
            { 9, "Qúa thời gian khởi tạo Z2" },
            { 10, "Lỗi Driver Z2" },
            { 11, "Lỗi trục RI2" },
            { 12, "Lỗi Driver RI2" },
            { 13, "Lỗi trục R02" },
            { 14, "Lỗi Driver R02" },
            { 15, "Lỗi trục F2" },
            { 16, "Qúa thời gian khởi tạo F2" },
            { 17, "Lỗi Driver F2" },
            { 18, "Lỗi cảm biến khí vào" },
            { 19, "Lỗi cảm biến Vin: hút sản phẩn vào" },
            { 20, "Lỗi cảm biến Vout: hút sản phẩn ra" },
            { 21, "Lỗi cảm biến VCa: Xy lanh di chuyển camera" },
            { 22, "Lỗi cảm biến Vfix open: mở socket" },
            { 23, "Lỗi cảm biến Vfix close: đóng socket" },
            { 24, "Lỗi cảm biến cửa" },
            { 25, "Lỗi cảm biến Jig/ Tray OK" },
            { 26, "Lỗi cảm biến Jig/ Tray NG" },
            { 27, "Lỗi cảm biến Jig/ Tray NG4" },
            { 28, "Lỗi máy test LCA" },
            { 29, "Lỗi timeout quá trình test LCA" }
        };

        // Error messages (Error level)
        public static readonly Dictionary<int, string> ErrorMessages = new Dictionary<int, string>
        {
            { 0, "Bình thường" },
            { 1, "Lỗi trục X2" },
            { 2, "Qúa thời gian khởi tạo X2" },
            { 3, "Dừng khẩn cấp" },
            { 4, "Lỗi Driver X2" },
            { 5, "Lỗi trục Y2" },
            { 6, "Qúa thời gian khởi tạo Y2" },
            { 7, "Lỗi Driver Y2" },
            { 8, "Lỗi trục Z2" },
            { 9, "Qúa thời gian khởi tạo Z2" },
            { 10, "Lỗi Driver Z2" },
            { 11, "Lỗi trục RI2" },
            { 12, "Lỗi Driver RI2" },
            { 13, "Lỗi trục R02" },
            { 14, "Lỗi Driver R02" },
            { 15, "Lỗi trục F2" },
            { 16, "Qúa thời gian khởi tạo F2" },
            { 17, "Lỗi Driver F2" },
            { 18, "Lỗi cảm biến khí vào" },
            { 19, "Lỗi cảm biến Vin: hút sản phẩn vào" },
            { 20, "Lỗi cảm biến Vout: hút sản phẩn ra" },
            { 21, "Lỗi cảm biến VCa: Xy lanh di chuyển camera" },
            { 22, "Lỗi cảm biến Vfix open: mở socket" },
            { 23, "Lỗi cảm biến Vfix close: đóng socket" },
            { 30, "Lỗi cảm biến VSk: hút sản phẩn tại socket" }
        };

        // Status messages (Information level)
        public static readonly Dictionary<int, string> StatusMessages = new Dictionary<int, string>
        {
            { 0, "Yêu cầu về gốc" },
            { 1, "Đang về gốc" },
            { 2, "Lỗi, Yêu cầu xóa lỗi" },
            { 3, "Đang tạm dừng" },
            { 4, "Sẵn sàng chạy thủ công" },
            { 5, "Đang chạy thủ công" },
            { 6, "Chưa đủ điều kiện chạy tự động" },
            { 7, "Sẵn sàng chạy tự động" },
            { 8, "Đang chạy tự động" },
            { 9, "Vô hiệu hóa" }
        };

        /// <summary>
        /// Get alarm message by code
        /// </summary>
        public static string GetAlarmMessage(int code)
        {
            return AlarmMessages.TryGetValue(code, out string message) ? message : $"Cảnh báo không xác định (Code: {code})";
        }

        /// <summary>
        /// Get error message by code
        /// </summary>
        public static string GetErrorMessage(int code)
        {
            return ErrorMessages.TryGetValue(code, out string message) ? message : $"Lỗi không xác định (Code: {code})";
        }

        /// <summary>
        /// Get status message by code
        /// </summary>
        public static string GetStatusMessage(int code)
        {
            return StatusMessages.TryGetValue(code, out string message) ? message : $"Trạng thái không xác định (Code: {code})";
        }
    }
}
