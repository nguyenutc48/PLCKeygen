using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PLCKeygen
{
    /// <summary>
    /// Quản lý việc đọc/ghi dữ liệu Speed và Teaching cho tab Data
    /// </summary>
    public class DataTabManager
    {
        private PLCKeyence plcKeyence;
        private int currentPort = 1; // Port hiện tại đang được chọn (1-4)

        // Dictionary để map textbox name với PLC address theo port
        private Dictionary<int, Dictionary<string, string>> speedAddressMap;
        private Dictionary<int, Dictionary<string, string>> teachingAddressMap;

        // HashSet để tracking các TextBox đã được đăng ký event
        private HashSet<TextBox> registeredTextBoxes = new HashSet<TextBox>();

        public DataTabManager(PLCKeyence plc)
        {
            this.plcKeyence = plc;
            InitializeAddressMaps();
        }

        /// <summary>
        /// Khởi tạo mapping giữa tên textbox và địa chỉ PLC
        /// </summary>
        private void InitializeAddressMaps()
        {
            // Initialize Speed Address Map
            speedAddressMap = new Dictionary<int, Dictionary<string, string>>
            {
                {
                    1, new Dictionary<string, string>
                    {
                        // Unload Speed
                        {"txtUnload_Speed_ZReady", PLCAddresses.Data.P1_Unload_Speed_ZReady},
                        {"txtUnload_Speed_XY", PLCAddresses.Data.P1_Unload_Speed_XY},
                        {"txtUnload_Speed_Z", PLCAddresses.Data.P1_Unload_Speed_Z},
                        {"txtUnload_Speed_XReady", PLCAddresses.Data.P1_Unload_Speed_XReady},
                        {"txtUnload_Speed_YReady", PLCAddresses.Data.P1_Unload_Speed_YReady},
                        {"txtUnload_Speed_XYReady", PLCAddresses.Data.P1_Unload_Speed_XYReady},
                        {"txtUnload_Speed_RO", PLCAddresses.Data.P1_Unload_Speed_RO},
                        // Load Speed
                        {"txtLoad_Speed_XY", PLCAddresses.Data.P1_Load_Speed_XY},
                        {"txtLoad_Speed_Z", PLCAddresses.Data.P1_Load_Speed_Z},
                        {"txtLoad_Speed_XReady", PLCAddresses.Data.P1_Load_Speed_XReady},
                        {"txtLoad_Speed_YReady", PLCAddresses.Data.P1_Load_Speed_YReady},
                        {"txtLoad_Speed_XYReady", PLCAddresses.Data.P1_Load_Speed_XYReady},
                        // Camera Speed
                        {"txtCamera_Speed_XY", PLCAddresses.Data.P1_Camera_Speed_XY},
                        {"txtCamera_Speed_RI", PLCAddresses.Data.P1_Camera_Speed_RI},
                        {"txtCamera_Speed_XReady", PLCAddresses.Data.P1_Camera_Speed_XReady},
                        {"txtCamera_Speed_YReady", PLCAddresses.Data.P1_Camera_Speed_YReady},
                        {"txtCamera_Speed_XYReady", PLCAddresses.Data.P1_Camera_Speed_XYReady},
                        {"txtCamera_Speed_Z", PLCAddresses.Data.P1_Camera_Speed_Z},
                        // Unload Socket Speed
                        {"txtUnload_Socket_Speed_XY", PLCAddresses.Data.P1_Unload_Socket_Speed_XY},
                        {"txtUnload_Socket_Speed_Z", PLCAddresses.Data.P1_Unload_Socket_Speed_Z},
                        {"txtUnload_Socket_Speed_XReady", PLCAddresses.Data.P1_Unload_Socket_Speed_XReady},
                        {"txtUnload_Socket_Speed_YReady", PLCAddresses.Data.P1_Unload_Socket_Speed_YReady},
                        {"txtUnload_Socket_Speed_XYReady", PLCAddresses.Data.P1_Unload_Socket_Speed_XYReady},
                        // Load Socket Speed
                        {"txtLoad_Socket_Speed_XY", PLCAddresses.Data.P1_Load_Socket_Speed_XY},
                        {"txtLoad_Socket_Speed_Z", PLCAddresses.Data.P1_Load_Socket_Speed_Z},
                        {"txtLoad_Socket_Speed_XReady", PLCAddresses.Data.P1_Load_Socket_Speed_XReady},
                        {"txtLoad_Socket_Speed_YReady", PLCAddresses.Data.P1_Load_Socket_Speed_YReady},
                        {"txtLoad_Socket_Speed_XYReady", PLCAddresses.Data.P1_Load_Socket_Speed_XYReady},
                        // Socket Speed Close/Open
                        {"txtSocket_Speed_Close", PLCAddresses.Data.P1_Socket_Speed_Close},
                        {"txtSocket_Speed_Open", PLCAddresses.Data.P1_Socket_Speed_Open},
                        {"txtPosYReady", PLCAddresses.Data.P1_PosYReady}
                    }
                },
                {
                    2, new Dictionary<string, string>
                    {
                        // Unload Speed
                        {"txtUnload_Speed_ZReady", PLCAddresses.Data.P2_Unload_Speed_ZReady},
                        {"txtUnload_Speed_XY", PLCAddresses.Data.P2_Unload_Speed_XY},
                        {"txtUnload_Speed_Z", PLCAddresses.Data.P2_Unload_Speed_Z},
                        {"txtUnload_Speed_XReady", PLCAddresses.Data.P2_Unload_Speed_XReady},
                        {"txtUnload_Speed_YReady", PLCAddresses.Data.P2_Unload_Speed_YReady},
                        {"txtUnload_Speed_XYReady", PLCAddresses.Data.P2_Unload_Speed_XYReady},
                        {"txtUnload_Speed_RO", PLCAddresses.Data.P2_Unload_Speed_RO},
                        // Load Speed
                        {"txtLoad_Speed_XY", PLCAddresses.Data.P2_Load_Speed_XY},
                        {"txtLoad_Speed_Z", PLCAddresses.Data.P2_Load_Speed_Z},
                        {"txtLoad_Speed_XReady", PLCAddresses.Data.P2_Load_Speed_XReady},
                        {"txtLoad_Speed_YReady", PLCAddresses.Data.P2_Load_Speed_YReady},
                        {"txtLoad_Speed_XYReady", PLCAddresses.Data.P2_Load_Speed_XYReady},
                        // Camera Speed
                        {"txtCamera_Speed_XY", PLCAddresses.Data.P2_Camera_Speed_XY},
                        {"txtCamera_Speed_RI", PLCAddresses.Data.P2_Camera_Speed_RI},
                        {"txtCamera_Speed_XReady", PLCAddresses.Data.P2_Camera_Speed_XReady},
                        {"txtCamera_Speed_YReady", PLCAddresses.Data.P2_Camera_Speed_YReady},
                        {"txtCamera_Speed_XYReady", PLCAddresses.Data.P2_Camera_Speed_XYReady},
                        {"txtCamera_Speed_Z", PLCAddresses.Data.P2_Camera_Speed_Z},
                        // Unload Socket Speed
                        {"txtUnload_Socket_Speed_XY", PLCAddresses.Data.P2_Unload_Socket_Speed_XY},
                        {"txtUnload_Socket_Speed_Z", PLCAddresses.Data.P2_Unload_Socket_Speed_Z},
                        {"txtUnload_Socket_Speed_XReady", PLCAddresses.Data.P2_Unload_Socket_Speed_XReady},
                        {"txtUnload_Socket_Speed_YReady", PLCAddresses.Data.P2_Unload_Socket_Speed_YReady},
                        {"txtUnload_Socket_Speed_XYReady", PLCAddresses.Data.P2_Unload_Socket_Speed_XYReady},
                        // Load Socket Speed
                        {"txtLoad_Socket_Speed_XY", PLCAddresses.Data.P2_Load_Socket_Speed_XY},
                        {"txtLoad_Socket_Speed_Z", PLCAddresses.Data.P2_Load_Socket_Speed_Z},
                        {"txtLoad_Socket_Speed_XReady", PLCAddresses.Data.P2_Load_Socket_Speed_XReady},
                        {"txtLoad_Socket_Speed_YReady", PLCAddresses.Data.P2_Load_Socket_Speed_YReady},
                        {"txtLoad_Socket_Speed_XYReady", PLCAddresses.Data.P2_Load_Socket_Speed_XYReady},
                        // Socket Speed Close/Open
                        {"txtSocket_Speed_Close", PLCAddresses.Data.P2_Socket_Speed_Close},
                        {"txtSocket_Speed_Open", PLCAddresses.Data.P2_Socket_Speed_Open},
                        {"txtPosYReady", PLCAddresses.Data.P2_PosYReady}
                    }
                },
                {
                    3, new Dictionary<string, string>
                    {
                        // Unload Speed
                        {"txtUnload_Speed_ZReady", PLCAddresses.Data.P3_Unload_Speed_ZReady},
                        {"txtUnload_Speed_XY", PLCAddresses.Data.P3_Unload_Speed_XY},
                        {"txtUnload_Speed_Z", PLCAddresses.Data.P3_Unload_Speed_Z},
                        {"txtUnload_Speed_XReady", PLCAddresses.Data.P3_Unload_Speed_XReady},
                        {"txtUnload_Speed_YReady", PLCAddresses.Data.P3_Unload_Speed_YReady},
                        {"txtUnload_Speed_XYReady", PLCAddresses.Data.P3_Unload_Speed_XYReady},
                        {"txtUnload_Speed_RO", PLCAddresses.Data.P3_Unload_Speed_RO},
                        // Load Speed
                        {"txtLoad_Speed_XY", PLCAddresses.Data.P3_Load_Speed_XY},
                        {"txtLoad_Speed_Z", PLCAddresses.Data.P3_Load_Speed_Z},
                        {"txtLoad_Speed_XReady", PLCAddresses.Data.P3_Load_Speed_XReady},
                        {"txtLoad_Speed_YReady", PLCAddresses.Data.P3_Load_Speed_YReady},
                        {"txtLoad_Speed_XYReady", PLCAddresses.Data.P3_Load_Speed_XYReady},
                        // Camera Speed
                        {"txtCamera_Speed_XY", PLCAddresses.Data.P3_Camera_Speed_XY},
                        {"txtCamera_Speed_RI", PLCAddresses.Data.P3_Camera_Speed_RI},
                        {"txtCamera_Speed_XReady", PLCAddresses.Data.P3_Camera_Speed_XReady},
                        {"txtCamera_Speed_YReady", PLCAddresses.Data.P3_Camera_Speed_YReady},
                        {"txtCamera_Speed_XYReady", PLCAddresses.Data.P3_Camera_Speed_XYReady},
                        {"txtCamera_Speed_Z", PLCAddresses.Data.P3_Camera_Speed_Z},
                        // Unload Socket Speed
                        {"txtUnload_Socket_Speed_XY", PLCAddresses.Data.P3_Unload_Socket_Speed_XY},
                        {"txtUnload_Socket_Speed_Z", PLCAddresses.Data.P3_Unload_Socket_Speed_Z},
                        {"txtUnload_Socket_Speed_XReady", PLCAddresses.Data.P3_Unload_Socket_Speed_XReady},
                        {"txtUnload_Socket_Speed_YReady", PLCAddresses.Data.P3_Unload_Socket_Speed_YReady},
                        {"txtUnload_Socket_Speed_XYReady", PLCAddresses.Data.P3_Unload_Socket_Speed_XYReady},
                        // Load Socket Speed
                        {"txtLoad_Socket_Speed_XY", PLCAddresses.Data.P3_Load_Socket_Speed_XY},
                        {"txtLoad_Socket_Speed_Z", PLCAddresses.Data.P3_Load_Socket_Speed_Z},
                        {"txtLoad_Socket_Speed_XReady", PLCAddresses.Data.P3_Load_Socket_Speed_XReady},
                        {"txtLoad_Socket_Speed_YReady", PLCAddresses.Data.P3_Load_Socket_Speed_YReady},
                        {"txtLoad_Socket_Speed_XYReady", PLCAddresses.Data.P3_Load_Socket_Speed_XYReady},
                        // Socket Speed Close/Open
                        {"txtSocket_Speed_Close", PLCAddresses.Data.P3_Socket_Speed_Close},
                        {"txtSocket_Speed_Open", PLCAddresses.Data.P3_Socket_Speed_Open},
                        {"txtPosYReady", PLCAddresses.Data.P3_PosYReady}
                    }
                },
                {
                    4, new Dictionary<string, string>
                    {
                        // Unload Speed
                        {"txtUnload_Speed_ZReady", PLCAddresses.Data.P4_Unload_Speed_ZReady},
                        {"txtUnload_Speed_XY", PLCAddresses.Data.P4_Unload_Speed_XY},
                        {"txtUnload_Speed_Z", PLCAddresses.Data.P4_Unload_Speed_Z},
                        {"txtUnload_Speed_XReady", PLCAddresses.Data.P4_Unload_Speed_XReady},
                        {"txtUnload_Speed_YReady", PLCAddresses.Data.P4_Unload_Speed_YReady},
                        {"txtUnload_Speed_XYReady", PLCAddresses.Data.P4_Unload_Speed_XYReady},
                        {"txtUnload_Speed_RO", PLCAddresses.Data.P4_Unload_Speed_RO},
                        // Load Speed
                        {"txtLoad_Speed_XY", PLCAddresses.Data.P4_Load_Speed_XY},
                        {"txtLoad_Speed_Z", PLCAddresses.Data.P4_Load_Speed_Z},
                        {"txtLoad_Speed_XReady", PLCAddresses.Data.P4_Load_Speed_XReady},
                        {"txtLoad_Speed_YReady", PLCAddresses.Data.P4_Load_Speed_YReady},
                        {"txtLoad_Speed_XYReady", PLCAddresses.Data.P4_Load_Speed_XYReady},
                        // Camera Speed
                        {"txtCamera_Speed_XY", PLCAddresses.Data.P4_Camera_Speed_XY},
                        {"txtCamera_Speed_RI", PLCAddresses.Data.P4_Camera_Speed_RI},
                        {"txtCamera_Speed_XReady", PLCAddresses.Data.P4_Camera_Speed_XReady},
                        {"txtCamera_Speed_YReady", PLCAddresses.Data.P4_Camera_Speed_YReady},
                        {"txtCamera_Speed_XYReady", PLCAddresses.Data.P4_Camera_Speed_XYReady},
                        {"txtCamera_Speed_Z", PLCAddresses.Data.P4_Camera_Speed_Z},
                        // Unload Socket Speed
                        {"txtUnload_Socket_Speed_XY", PLCAddresses.Data.P4_Unload_Socket_Speed_XY},
                        {"txtUnload_Socket_Speed_Z", PLCAddresses.Data.P4_Unload_Socket_Speed_Z},
                        {"txtUnload_Socket_Speed_XReady", PLCAddresses.Data.P4_Unload_Socket_Speed_XReady},
                        {"txtUnload_Socket_Speed_YReady", PLCAddresses.Data.P4_Unload_Socket_Speed_YReady},
                        {"txtUnload_Socket_Speed_XYReady", PLCAddresses.Data.P4_Unload_Socket_Speed_XYReady},
                        // Load Socket Speed
                        {"txtLoad_Socket_Speed_XY", PLCAddresses.Data.P4_Load_Socket_Speed_XY},
                        {"txtLoad_Socket_Speed_Z", PLCAddresses.Data.P4_Load_Socket_Speed_Z},
                        {"txtLoad_Socket_Speed_XReady", PLCAddresses.Data.P4_Load_Socket_Speed_XReady},
                        {"txtLoad_Socket_Speed_YReady", PLCAddresses.Data.P4_Load_Socket_Speed_YReady},
                        {"txtLoad_Socket_Speed_XYReady", PLCAddresses.Data.P4_Load_Socket_Speed_XYReady},
                        // Socket Speed Close/Open
                        {"txtSocket_Speed_Close", PLCAddresses.Data.P4_Socket_Speed_Close},
                        {"txtSocket_Speed_Open", PLCAddresses.Data.P4_Socket_Speed_Open},
                        {"txtPosYReady", PLCAddresses.Data.P4_PosYReady}
                    }
                }
            };

            // Initialize Teaching Address Map (for positions)
            teachingAddressMap = new Dictionary<int, Dictionary<string, string>>
            {
                {
                    1, new Dictionary<string, string>
                    {
                        {"txtTray_Col_Number", PLCAddresses.Data.P1_Tray_Col_Number},
                        {"txtTray_Row_Number", PLCAddresses.Data.P1_Tray_Row_Number},
                        {"txtTray_Row_NG1", PLCAddresses.Data.P1_Tray_Row_NG1},
                        {"txtTray_Row_NG2", PLCAddresses.Data.P1_Tray_Row_NG2},
                        {"txtTray_Row_NG3", PLCAddresses.Data.P1_Tray_Row_NG3},
                        {"txtTray_Row_NG4", PLCAddresses.Data.P1_Tray_Row_NG4},
                        {"txtRORI_Distance_X", PLCAddresses.Data.P1_RORI_Distance_X},
                        {"txtSocket_Angle", PLCAddresses.Data.P1_Socket_Angle}
                    }
                },
                {
                    2, new Dictionary<string, string>
                    {
                        {"txtTray_Col_Number", PLCAddresses.Data.P2_Tray_Col_Number},
                        {"txtTray_Row_Number", PLCAddresses.Data.P2_Tray_Row_Number},
                        {"txtTray_Row_NG1", PLCAddresses.Data.P2_Tray_Row_NG1},
                        {"txtTray_Row_NG2", PLCAddresses.Data.P2_Tray_Row_NG2},
                        {"txtTray_Row_NG3", PLCAddresses.Data.P2_Tray_Row_NG3},
                        {"txtTray_Row_NG4", PLCAddresses.Data.P2_Tray_Row_NG4},
                        {"txtRORI_Distance_X", PLCAddresses.Data.P2_RORI_Distance_X},
                        {"txtSocket_Angle", PLCAddresses.Data.P2_Socket_Angle}
                    }
                },
                {
                    3, new Dictionary<string, string>
                    {
                        {"txtTray_Col_Number", PLCAddresses.Data.P3_Tray_Col_Number},
                        {"txtTray_Row_Number", PLCAddresses.Data.P3_Tray_Row_Number},
                        {"txtTray_Row_NG1", PLCAddresses.Data.P3_Tray_Row_NG1},
                        {"txtTray_Row_NG2", PLCAddresses.Data.P3_Tray_Row_NG2},
                        {"txtTray_Row_NG3", PLCAddresses.Data.P3_Tray_Row_NG3},
                        {"txtTray_Row_NG4", PLCAddresses.Data.P3_Tray_Row_NG4},
                        {"txtRORI_Distance_X", PLCAddresses.Data.P3_RORI_Distance_X},
                        {"txtSocket_Angle", PLCAddresses.Data.P3_Socket_Angle}
                    }
                },
                {
                    4, new Dictionary<string, string>
                    {
                        {"txtTray_Col_Number", PLCAddresses.Data.P4_Tray_Col_Number},
                        {"txtTray_Row_Number", PLCAddresses.Data.P4_Tray_Row_Number},
                        {"txtTray_Row_NG1", PLCAddresses.Data.P4_Tray_Row_NG1},
                        {"txtTray_Row_NG2", PLCAddresses.Data.P4_Tray_Row_NG2},
                        {"txtTray_Row_NG3", PLCAddresses.Data.P4_Tray_Row_NG3},
                        {"txtTray_Row_NG4", PLCAddresses.Data.P4_Tray_Row_NG4},
                        {"txtRORI_Distance_X", PLCAddresses.Data.P4_RORI_Distance_X},
                        {"txtSocket_Angle", PLCAddresses.Data.P4_Socket_Angle}
                    }
                }
            };
        }

        /// <summary>
        /// Thiết lập port hiện tại
        /// </summary>
        public void SetCurrentPort(int port)
        {
            if (port < 1 || port > 4)
                throw new ArgumentException("Port must be between 1 and 4");

            currentPort = port;
        }

        /// <summary>
        /// Đọc tất cả giá trị Speed từ PLC và cập nhật lên các TextBox
        /// </summary>
        public void LoadSpeedDataToTextBoxes(Control.ControlCollection controls)
        {
            if (!speedAddressMap.ContainsKey(currentPort))
                return;

            var addressMap = speedAddressMap[currentPort];

            foreach (Control control in controls)
            {
                if (control is TextBox textBox && addressMap.ContainsKey(textBox.Name))
                {
                    string plcAddress = addressMap[textBox.Name];
                    try
                    {
                        int value = plcKeyence.ReadInt32(plcAddress);

                        // Các tốc độ góc (độ/giây) và trục Z (độ/giây): hệ số x10
                        if (textBox.Name == "txtUnload_Speed_RO" ||
                            textBox.Name == "txtCamera_Speed_RI" ||
                            textBox.Name == "txtSocket_Speed_Close" ||
                            textBox.Name == "txtSocket_Speed_Open" ||
                            textBox.Name == "txtUnload_Speed_Z" ||
                            textBox.Name == "txtUnload_Speed_ZReady" ||
                            textBox.Name == "txtLoad_Speed_Z" ||
                            textBox.Name == "txtCamera_Speed_Z" ||
                            textBox.Name == "txtUnload_Socket_Speed_Z" ||
                            textBox.Name == "txtLoad_Socket_Speed_Z")
                        {
                            double displayValue = value / 10.0;
                            textBox.Text = displayValue.ToString("F1"); // 1 chữ số thập phân
                        }
                        // Các tốc độ trục khác (mm/s): hệ số x100
                        else
                        {
                            double displayValue = value / 100.0;
                            textBox.Text = displayValue.ToString("F2"); // 2 chữ số thập phân
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error or show message
                        Console.WriteLine($"Error reading {plcAddress}: {ex.Message}");
                    }
                }

                // Đệ quy để tìm controls trong GroupBox
                if (control.HasChildren)
                {
                    LoadSpeedDataToTextBoxes(control.Controls);
                }
            }
        }

        /// <summary>
        /// Đọc tất cả giá trị Teaching từ PLC và cập nhật lên các TextBox
        /// </summary>
        public void LoadTeachingDataToTextBoxes(Control.ControlCollection controls)
        {
            if (!teachingAddressMap.ContainsKey(currentPort))
                return;

            var addressMap = teachingAddressMap[currentPort];

            foreach (Control control in controls)
            {
                if (control is TextBox textBox && addressMap.ContainsKey(textBox.Name))
                {
                    string plcAddress = addressMap[textBox.Name];
                    try
                    {
                        int value = plcKeyence.ReadInt32(plcAddress);

                        // Xử lý đặc biệt cho txtRORI_Distance_X: chia cho 100 để hiển thị mm
                        if (textBox.Name == "txtRORI_Distance_X")
                        {
                            double displayValue = value / 100.0;
                            textBox.Text = displayValue.ToString("F2"); // Format 2 chữ số thập phân
                        }
                        // Xử lý đặc biệt cho txtSocket_Angle: chia cho 10 để hiển thị độ
                        else if (textBox.Name == "txtSocket_Angle")
                        {
                            double displayValue = value / 10.0;
                            textBox.Text = displayValue.ToString("F1"); // Format 1 chữ số thập phân
                        }
                        else
                        {
                            textBox.Text = value.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error or show message
                        Console.WriteLine($"Error reading {plcAddress}: {ex.Message}");
                    }
                }

                // Đệ quy để tìm controls trong GroupBox
                if (control.HasChildren)
                {
                    LoadTeachingDataToTextBoxes(control.Controls);
                }
            }
        }

        /// <summary>
        /// Ghi giá trị từ TextBox xuống PLC
        /// </summary>
        public bool SaveTextBoxValueToPLC(TextBox textBox)
        {
            string plcAddress = GetPLCAddressForTextBox(textBox.Name);

            if (string.IsNullOrEmpty(plcAddress))
                return false;

            try
            {
                int valueToWrite;
                string displayValue = textBox.Text;

                // Xử lý các giá trị có hệ số chuyển đổi
                if (textBox.Name == "txtRORI_Distance_X")
                {
                    // Hệ số x100: mm
                    if (!double.TryParse(textBox.Text, out double doubleValue))
                    {
                        MessageBox.Show("Giá trị không hợp lệ. Vui lòng nhập số (ví dụ: 67.05).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    doubleValue = Math.Round(doubleValue, 2);
                    valueToWrite = (int)(doubleValue * 100);
                    displayValue = doubleValue.ToString("F2");
                }
                else if (textBox.Name == "txtSocket_Angle")
                {
                    // Hệ số x10: góc (độ)
                    if (!double.TryParse(textBox.Text, out double doubleValue))
                    {
                        MessageBox.Show("Giá trị không hợp lệ. Vui lòng nhập số (ví dụ: 90.0).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    doubleValue = Math.Round(doubleValue, 1);
                    valueToWrite = (int)(doubleValue * 10);
                    displayValue = doubleValue.ToString("F1");
                }
                else if (textBox.Name == "txtUnload_Speed_RO" ||
                         textBox.Name == "txtCamera_Speed_RI" ||
                         textBox.Name == "txtSocket_Speed_Close" ||
                         textBox.Name == "txtSocket_Speed_Open" ||
                         textBox.Name == "txtUnload_Speed_Z" ||
                         textBox.Name == "txtUnload_Speed_ZReady" ||
                         textBox.Name == "txtLoad_Speed_Z" ||
                         textBox.Name == "txtCamera_Speed_Z" ||
                         textBox.Name == "txtUnload_Socket_Speed_Z" ||
                         textBox.Name == "txtLoad_Socket_Speed_Z")
                {
                    // Hệ số x10: tốc độ góc và trục Z (độ/giây)
                    if (!double.TryParse(textBox.Text, out double doubleValue))
                    {
                        MessageBox.Show("Giá trị không hợp lệ. Vui lòng nhập số (ví dụ: 10.0).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    doubleValue = Math.Round(doubleValue, 1);
                    valueToWrite = (int)(doubleValue * 10);
                    displayValue = doubleValue.ToString("F1");
                }
                else if (speedAddressMap.ContainsKey(currentPort) &&
                         speedAddressMap[currentPort].ContainsKey(textBox.Name))
                {
                    // Hệ số x100: các tốc độ trục khác (mm/s)
                    if (!double.TryParse(textBox.Text, out double doubleValue))
                    {
                        MessageBox.Show("Giá trị không hợp lệ. Vui lòng nhập số (ví dụ: 100.00).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    doubleValue = Math.Round(doubleValue, 2);
                    valueToWrite = (int)(doubleValue * 100);
                    displayValue = doubleValue.ToString("F2");
                }
                else
                {
                    // Các giá trị nguyên khác
                    if (!int.TryParse(textBox.Text, out valueToWrite))
                    {
                        MessageBox.Show("Giá trị không hợp lệ. Vui lòng nhập số nguyên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                // Hiển thị hộp thoại xác nhận duy nhất
                string confirmMessage = $"Bạn có chắc chắn muốn thay đổi giá trị thành '{displayValue}'?";

                DialogResult result = MessageBox.Show(
                    confirmMessage,
                    "Xác nhận thay đổi",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                );

                // Nếu người dùng chọn Cancel, không lưu
                if (result != DialogResult.OK)
                {
                    return false;
                }

                // Write to PLC
                plcKeyence.WriteInt32(plcAddress, valueToWrite);

                // Read back to confirm
                int readValue = plcKeyence.ReadInt32(plcAddress);

                // Display read value with proper formatting
                if (textBox.Name == "txtRORI_Distance_X")
                {
                    double readDisplayValue = readValue / 100.0;
                    textBox.Text = readDisplayValue.ToString("F2");
                }
                else if (textBox.Name == "txtSocket_Angle")
                {
                    double readDisplayValue = readValue / 10.0;
                    textBox.Text = readDisplayValue.ToString("F1");
                }
                else if (textBox.Name == "txtUnload_Speed_RO" ||
                         textBox.Name == "txtCamera_Speed_RI" ||
                         textBox.Name == "txtSocket_Speed_Close" ||
                         textBox.Name == "txtSocket_Speed_Open" ||
                         textBox.Name == "txtUnload_Speed_Z" ||
                         textBox.Name == "txtUnload_Speed_ZReady" ||
                         textBox.Name == "txtLoad_Speed_Z" ||
                         textBox.Name == "txtCamera_Speed_Z" ||
                         textBox.Name == "txtUnload_Socket_Speed_Z" ||
                         textBox.Name == "txtLoad_Socket_Speed_Z")
                {
                    double readDisplayValue = readValue / 10.0;
                    textBox.Text = readDisplayValue.ToString("F1");
                }
                else if (speedAddressMap.ContainsKey(currentPort) &&
                         speedAddressMap[currentPort].ContainsKey(textBox.Name))
                {
                    double readDisplayValue = readValue / 100.0;
                    textBox.Text = readDisplayValue.ToString("F2");
                }
                else
                {
                    textBox.Text = readValue.ToString();
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi ghi giá trị xuống PLC: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Lấy địa chỉ PLC tương ứng với tên TextBox
        /// </summary>
        private string GetPLCAddressForTextBox(string textBoxName)
        {
            // Check in speed map
            if (speedAddressMap.ContainsKey(currentPort) &&
                speedAddressMap[currentPort].ContainsKey(textBoxName))
            {
                return speedAddressMap[currentPort][textBoxName];
            }

            // Check in teaching map
            if (teachingAddressMap.ContainsKey(currentPort) &&
                teachingAddressMap[currentPort].ContainsKey(textBoxName))
            {
                return teachingAddressMap[currentPort][textBoxName];
            }

            return null;
        }

        /// <summary>
        /// Đăng ký event handler cho tất cả TextBox trong tab Data
        /// </summary>
        public void RegisterTextBoxEvents(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is TextBox textBox)
                {
                    // Kiểm tra nếu textbox có mapping với PLC address và chưa được đăng ký
                    if (!string.IsNullOrEmpty(GetPLCAddressForTextBox(textBox.Name))
                        && !registeredTextBoxes.Contains(textBox))
                    {
                        textBox.KeyDown += DataTextBox_KeyDown;
                        registeredTextBoxes.Add(textBox);
                    }
                }

                // Đệ quy để tìm controls trong GroupBox
                if (control.HasChildren)
                {
                    RegisterTextBoxEvents(control.Controls);
                }
            }
        }

        /// <summary>
        /// Event handler khi nhấn phím trong TextBox
        /// </summary>
        private void DataTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent beep sound
                e.Handled = true; // Mark event as handled to prevent propagation

                if (sender is TextBox textBox)
                {
                    SaveTextBoxValueToPLC(textBox);
                }
            }
        }
    }
}
