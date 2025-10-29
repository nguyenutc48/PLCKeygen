using System;
using System.Collections.Generic;

namespace PLCKeygen
{
    /// <summary>
    /// Thông tin cấu hình PLC từ file JSON
    /// </summary>
    public class PLCConfig
    {
        public string PLCName { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public PLCAddressesConfig Addresses { get; set; }
    }

    /// <summary>
    /// Container cho các loại địa chỉ PLC
    /// </summary>
    public class PLCAddressesConfig
    {
        public List<PLCAddressInfo> Input { get; set; }
        public List<PLCAddressInfo> Output { get; set; }
        public List<PLCAddressInfo> Data { get; set; }

        public PLCAddressesConfig()
        {
            Input = new List<PLCAddressInfo>();
            Output = new List<PLCAddressInfo>();
            Data = new List<PLCAddressInfo>();
        }
    }

    /// <summary>
    /// Thông tin chi tiết của một địa chỉ PLC
    /// </summary>
    public class PLCAddressInfo
    {
        /// <summary>
        /// Tên biến (không dấu cách) - dùng để generate enum/constant
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tên hiển thị có dấu tiếng Việt
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Kiểu dữ liệu: Bool, UInt16, Int16, Int32, UInt32
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Địa chỉ trong PLC (ví dụ: R0, DM100, ...)
        /// </summary>
        public string Address { get; set; }

        public override string ToString()
        {
            return $"{Name} ({DisplayName}) - {Address} [{DataType}]";
        }
    }

    /// <summary>
    /// Enum cho các kiểu dữ liệu hỗ trợ
    /// </summary>
    public enum PLCDataType
    {
        Bool,
        UInt16,
        Int16,
        Int32,
        UInt32
    }
}
