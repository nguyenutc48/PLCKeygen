using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PLCKeygen
{
    public partial class Form1 : Form
    {
        PLCKeygen.PLCKeyence PLCKey;
        private CameraTcpClient cameraClient12;
        private CameraTcpClient cameraClient34;
        private bool stopHandEye1 = false;
        private bool stopHandEye2 = false;

        // Motion control tracking
        private int selectedPort = 1;  // 1-4
        private string selectedAxis = "X";  // X, Y, Z, RI, RO, F
        private bool isJogMode = true;  // true=JOG, false=STEP

        // IO tab tracking
        private int selectedIOPort = 1;  // 1-4 for IO tab

        // Teaching mode password
        private const string TEACHING_PASSWORD = "1234";

        // Teaching hotkey manager
        private TeachingHotkeyManager hotkeyManager;

        // Model manager for saving/loading teaching models
        private ModelManager modelManager;

        // Data tab manager for reading/writing speed and teaching data
        private DataTabManager dataTabManager;

        // Data tab tracking
        private int selectedDataPort = 1;  // 1-4 for Data tab

        public Form1()
        {
            InitializeComponent();

            this.Text = this.Text +" - "+ Application.ProductVersion.ToString();
            this.Load += Form1_Load;
            this.FormClosing += Closing;
            this.FormClosing += CloseIOport;
            PLCKey = new PLCKeyence("192.168.0.10", 8501);

            // Subscribe to PropertyChanged event to monitor connection status
            PLCKey.PropertyChanged += PLCKey_PropertyChanged;

            PLCKey.Open();
            PLCKey.StartCommunication();

            // Setup keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            // Setup radio button event handlers for Motion tab
            rbtPort1.CheckedChanged += PortRadioButton_CheckedChanged;
            rbtPort2.CheckedChanged += PortRadioButton_CheckedChanged;
            rbtPort3.CheckedChanged += PortRadioButton_CheckedChanged;
            rbtPort4.CheckedChanged += PortRadioButton_CheckedChanged;

            // Setup radio button event handlers for IO tab
            rbtIOPort1.CheckedChanged += IOPortRadioButton_CheckedChanged;
            rbtIOPort2.CheckedChanged += IOPortRadioButton_CheckedChanged;
            rbtIOPort3.CheckedChanged += IOPortRadioButton_CheckedChanged;
            rbtIOPort4.CheckedChanged += IOPortRadioButton_CheckedChanged;

            // Setup radio button event handlers for Teaching mode
            rbtJogMode.CheckedChanged += TeachingModeRadioButton_CheckedChanged;
            rbtTeachingMode.CheckedChanged += TeachingModeRadioButton_CheckedChanged;

            rbtX.CheckedChanged += AxisRadioButton_CheckedChanged;
            rbtY.CheckedChanged += AxisRadioButton_CheckedChanged;
            rbtZ.CheckedChanged += AxisRadioButton_CheckedChanged;
            rbtRI.CheckedChanged += AxisRadioButton_CheckedChanged;
            rbtRO.CheckedChanged += AxisRadioButton_CheckedChanged;
            rbtF.CheckedChanged += AxisRadioButton_CheckedChanged;

            rbtJog.CheckedChanged += ModeRadioButton_CheckedChanged;
            rbtStep.CheckedChanged += ModeRadioButton_CheckedChanged;

            // Set default checked state
            rbtJog.Checked = true;  // JOG mode
            rbtX.Checked = true;  // X axis

            // Wire up Go button event handlers
            //btnXGo.Click += btnAxisGo_Click;
            //btnYGo.Click += btnAxisGo_Click;
            //btnZGo.Click += btnAxisGo_Click;
            //btnRIGo.Click += btnAxisGo_Click;
            //btnROGo.Click += btnAxisGo_Click;
            //btnFGo.Click += btnAxisGo_Click;

            // Wire up Org button event handlers
            btnXOrg.Click += btnAxisOrg_Click;
            btnYOrg.Click += btnAxisOrg_Click;
            btnZOrg.Click += btnAxisOrg_Click;
            btnRIOrg.Click += btnAxisOrg_Click;
            btnROOrg.Click += btnAxisOrg_Click;
            btnFOrg.Click += btnAxisOrg_Click;

            // Wire up Reset button event handlers
            btnXReset.Click += btnAxisReset_Click;
            btnYReset.Click += btnAxisReset_Click;
            btnZReset.Click += btnAxisReset_Click;
            btnRIReset.Click += btnAxisReset_Click;
            btnROReset.Click += btnAxisReset_Click;
            btnFReset.Click += btnAxisReset_Click;

            // Wire up Save button event handlers for Speed/Step
            //btnXSave.Click += btnAxisSave_Click;
            //btnYSave.Click += btnAxisSave_Click;
            //btnZSave.Click += btnAxisSave_Click;
            //btnRISave.Click += btnAxisSave_Click;
            //btnROSave.Click += btnAxisSave_Click;
            //btnFSave.Click += btnAxisSave_Click;

            // Wire up Go button event handlers for ABS movement
            //btnXGo.Click += btnAxisGo_Click;
            //btnYGo.Click += btnAxisGo_Click;
            //btnZGo.Click += btnAxisGo_Click;
            //btnRIGo.Click += btnAxisGo_Click;
            //btnROGo.Click += btnAxisGo_Click;
            //btnFGo.Click += btnAxisGo_Click;

            // Wire up IO tab output button event handlers
            btnVacLoad.Click += btnOutputToggle_Click;
            btnVacUnload.Click += btnOutputToggle_Click;
            btnCylinderSocket.Click += btnOutputToggle_Click;
            btnCylinderChart.Click += btnOutputToggle_Click;
            btnReqCamera.Click += btnOutputToggle_Click;
            btnLampStart.Click += btnOutputToggle_Click;
            btnLampStop.Click += btnOutputToggle_Click;
            btnLampRestart.Click += btnOutputToggle_Click;
            btnLampInit.Click += btnOutputToggle_Click;

            // Set default IO port
            rbtIOPort1.Checked = true;

            // Initialize Teaching groups as disabled (Jog mode is default)
            grpTeachingSocket.Enabled = false;
            grpTeachingTray.Enabled = false;

            // Initialize Teaching Hotkey Manager
            hotkeyManager = new TeachingHotkeyManager();

            // Initialize Model Manager
            modelManager = new ModelManager();

            // Initialize Data Tab Manager
            dataTabManager = new DataTabManager(PLCKey);

            // Wire up Model ComboBox event handler
            cbbModel.SelectedIndexChanged += cbbModel_SelectedIndexChanged;

            // Wire up Speed textbox events for validation and auto-save
            txtXSpeed.KeyPress += SpeedStepTextBox_KeyPress;
            txtXSpeed.KeyDown += SpeedStepTextBox_KeyDown;
            txtYSpeed.KeyPress += SpeedStepTextBox_KeyPress;
            txtYSpeed.KeyDown += SpeedStepTextBox_KeyDown;
            txtZSpeed.KeyPress += SpeedStepTextBox_KeyPress;
            txtZSpeed.KeyDown += SpeedStepTextBox_KeyDown;
            txtRISpeed.KeyPress += SpeedStepTextBox_KeyPress;
            txtRISpeed.KeyDown += SpeedStepTextBox_KeyDown;
            txtROSpeed.KeyPress += SpeedStepTextBox_KeyPress;
            txtROSpeed.KeyDown += SpeedStepTextBox_KeyDown;
            txtFSpeed.KeyPress += SpeedStepTextBox_KeyPress;
            txtFSpeed.KeyDown += SpeedStepTextBox_KeyDown;

            // Wire up Step textbox events for validation and auto-save
            txtXStep.KeyPress += SpeedStepTextBox_KeyPress;
            txtXStep.KeyDown += SpeedStepTextBox_KeyDown;
            txtYStep.KeyPress += SpeedStepTextBox_KeyPress;
            txtYStep.KeyDown += SpeedStepTextBox_KeyDown;
            txtZStep.KeyPress += SpeedStepTextBox_KeyPress;
            txtZStep.KeyDown += SpeedStepTextBox_KeyDown;
            txtRIStep.KeyPress += SpeedStepTextBox_KeyPress;
            txtRIStep.KeyDown += SpeedStepTextBox_KeyDown;
            txtROStep.KeyPress += SpeedStepTextBox_KeyPress;
            txtROStep.KeyDown += SpeedStepTextBox_KeyDown;
            txtFStep.KeyPress += SpeedStepTextBox_KeyPress;
            txtFStep.KeyDown += SpeedStepTextBox_KeyDown;

            txtTargetPos.KeyPress += SpeedStepTextBox_KeyPress;
            txtTargetPos.KeyDown += TxtTargetPos_KeyDown;

            // Wire up Teaching Point buttons - Tray Input (OK)
            btnSavePointTrayInputXYStart.Click += btnSavePoint_Click;
            btnGoPointTrayInputXYStart.Click += btnGoPoint_Click;
            btnSavePointTrayInputXEnd.Click += btnSavePoint_Click;
            btnGoPointTrayInputXEnd.Click += btnGoPoint_Click;
            btnSavePointTrayInputYEnd.Click += btnSavePoint_Click;
            btnGoPointTrayInputYEnd.Click += btnGoPoint_Click;
            btnSavePointTrayInputZ.Click += btnSavePoint_Click;
            btnGoPointTrayInputZ.Click += btnGoPoint_Click;

            // Wire up Teaching Point buttons - Tray NG1
            btnSavePointTrayNG1XYStart.Click += btnSavePoint_Click;
            btnGoPointTrayNG1XYStart.Click += btnGoPoint_Click;
            btnSavePointTrayNG1XEnd.Click += btnSavePoint_Click;
            btnGoPointTrayNG1XEnd.Click += btnGoPoint_Click;
            btnSavePointTrayNG1YEnd.Click += btnSavePoint_Click;
            btnGoPointTrayNG1YEnd.Click += btnGoPoint_Click;
            btnSavePointTrayNG1Z.Click += btnSavePoint_Click;
            btnGoPointTrayNG1Z.Click += btnGoPoint_Click;

            // Wire up Teaching Point buttons - Tray NG2
            btnSavePointTrayNG2XYStart.Click += btnSavePoint_Click;
            btnGoPointTrayNG2XYStart.Click += btnGoPoint_Click;
            btnSavePointTrayNG2XEnd.Click += btnSavePoint_Click;
            btnGoPointTrayNG2XEnd.Click += btnGoPoint_Click;
            btnSavePointTrayNG2YEnd.Click += btnSavePoint_Click;
            btnGoPointTrayNG2YEnd.Click += btnGoPoint_Click;
            btnSavePointTrayNG2Z.Click += btnSavePoint_Click;
            btnGoPointTrayNG2Z.Click += btnGoPoint_Click;

            // Wire up Teaching Point buttons - Socket
            btnSavePointSocket.Click += btnSavePoint_Click;
            btnGoPointSocket.Click += btnGoPoint_Click;
            btnSavePointSocketZLoad.Click += btnSavePoint_Click;
            btnGoPointSocketZLoad.Click += btnGoPoint_Click;
            btnSavePointSocketZUnload.Click += btnSavePoint_Click;
            btnGoPointSocketZUnload.Click += btnGoPoint_Click;
            btnSavePointSocketZReady.Click += btnSavePoint_Click;
            btnGoPointSocketZReady.Click += btnGoPoint_Click;
            btnSavePointSocketZReadyLoad.Click += btnSavePoint_Click;
            btnGoPointSocketZReadyLoad.Click += btnGoPoint_Click;
            btnSavePointSocketZReadyUnload.Click += btnSavePoint_Click;
            btnGoPointSocketZReadyUnload.Click += btnGoPoint_Click;
            btnSavePointSocketFOpened.Click += btnSavePoint_Click;
            btnGoPointSocketFOpened.Click += btnGoPoint_Click;
            btnSavePointSocketFClosed.Click += btnSavePoint_Click;
            btnGoPointSocketFClosed.Click += btnGoPoint_Click;

            // Wire up Teaching Point buttons - Camera
            btnSavePointCamera.Click += btnSavePoint_Click;
            btnGoPointCamera.Click += btnGoPoint_Click;
            btnSavePointSocketCameraZ.Click += btnSavePoint_Click;
            btnGoPointSocketCameraZ.Click += btnGoPoint_Click;

            // Load initial Speed and Step values from PLC
            LoadAxisSpeedAndStep();
        }

        private void ConnectionStatusChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TxtTargetPos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            System.Windows.Forms.TextBox txt = sender as System.Windows.Forms.TextBox;
            if (txt == null) return;

            try
            {
                // Parse the value and multiply by 100, truncate to 2 decimal places
                if (!double.TryParse(txt.Text, out double value))
                {
                    MessageBox.Show("Giá trị không hợp lệ!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Round to 2 decimal places and convert to integer (multiply by 100)
                int plcValue = (int)(Math.Round(value, 2) * 100);

                // Determine which textbox and get corresponding PLC address
                string plcAddress = GetGoABSAddress(selectedPort,selectedAxis);
                if (string.IsNullOrEmpty(plcAddress))
                {
                    MessageBox.Show("Không tìm thấy địa chỉ PLC!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Write to PLC
                PLCKey.WriteInt32(plcAddress, plcValue);

                // Update textbox with properly formatted value (2 decimals)
                txt.Text = value.ToString("F2");

                // Move focus away from textbox
                this.ActiveControl = null;

                // Show success message
                MessageBox.Show($"Đã ghi thành công: {value:F2} (PLC value: {plcValue})", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi ghi xuống PLC: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Check if Port 1 or Port 2 is in Auto Mode
            if (IsAutoModeActive(1) || IsAutoModeActive(2))
            {
                MessageBox.Show(
                    "Port 1 hoặc Port 2 đang ở chế độ Auto!\nKhông thể điều khiển Camera Cylinder khi Auto đang chạy.",
                    "Auto Mode Active",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (PLCKey.ReadBit(PLCAddresses.Output.P12_Cam_cylinder))
            {
                PLCKey.ResetBit(PLCAddresses.Output.P12_Cam_cylinder);
                button1.Text = "Sang trai";
            }
            else
            {
                PLCKey.SetBit(PLCAddresses.Output.P12_Cam_cylinder);
                button1.Text = "Sang phai";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Check if Port 3 or Port 4 is in Auto Mode
            if (IsAutoModeActive(3) || IsAutoModeActive(4))
            {
                MessageBox.Show(
                    "Port 3 hoặc Port 4 đang ở chế độ Auto!\nKhông thể điều khiển Camera Cylinder khi Auto đang chạy.",
                    "Auto Mode Active",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (PLCKey.ReadBit(PLCAddresses.Output.P34_Cam_cylinder))
            {
                PLCKey.ResetBit(PLCAddresses.Output.P34_Cam_cylinder);
                button2.Text = "Sang trai";
            }
            else
            {
                PLCKey.SetBit(PLCAddresses.Output.P34_Cam_cylinder);
                button2.Text = "Sang phai";
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtXCurMasPort1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_X_Master) / 100.0f).ToString();
            txtYCurMasPort1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_Y_Master) / 100.0f).ToString();
            txtRCurMasPort1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_R_Master) / 10.0f).ToString();
            txtXCurMasPort3.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_X_Master) / 100.0f).ToString();
            txtYCurMasPort3.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_Y_Master) / 100.0f).ToString();
            txtRCurMasPort3.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_R_Master) / 10.0f).ToString();
            txtXCurMasPort2.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Master) / 100.0f).ToString();
            txtYCurMasPort2.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Master) / 100.0f).ToString();
            txtRCurMasPort2.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_R_Master) / 10.0f).ToString();
            txtXCurMasPort4.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Master) / 100.0f).ToString();
            txtYCurMasPort4.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Master) / 100.0f).ToString();
            txtRCurMasPort4.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_R_Master) / 10.0f).ToString();

            // Update toa do hien tai cho Handeye Camera 1 (Port 2)
            txtXPosCurrent1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur) / 100.0f).ToString("F2");
            txtYPosCurrent1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur) / 100.0f).ToString("F2");
            txtRPosCurrent1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_RI_Pos_Cur) / 10.0f).ToString("F2");

            // Update toa do hien tai cho Handeye Camera 2 (Port 4)
            txtXPosCurrent2.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur) / 100.0f).ToString("F2");
            txtYPosCurrent2.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur) / 100.0f).ToString("F2");
            txtRPosCurrent2.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_RI_Pos_Cur) / 10.0f).ToString("F2");

            // Update current position displays for Motion tab
            UpdateCurrentPositionDisplays();

            // Update limit switch displays (L-, Home, L+)
            UpdateLimitSwitchDisplays();

            // Update IO tab inputs and outputs
            UpdateIOInputs();
            UpdateIOOutputs();

            // Update teaching point displays
            UpdateTeachingPointDisplays();

            // Update bypass button colors
            UpdateBypassButtonColors();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            float x = float.Parse(txtXMasPort1.Text);
            var x1 = x * 100;
            float y = float.Parse(txtYMasPort1.Text);
            var y1 = y * 100;
            float r = float.Parse(txtRMasPort1.Text);
            var r1 = r * 10;

            PLCKey.WriteInt32(PLCAddresses.Data.P1_X_Master, (int)x1);
            PLCKey.WriteInt32(PLCAddresses.Data.P1_Y_Master, (int)y1);
            PLCKey.WriteInt32(PLCAddresses.Data.P1_R_Master, (int)r1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            float x = float.Parse(txtXMasPort3.Text);
            var x1 = x * 100;
            float y = float.Parse(txtYMasPort3.Text);
            var y1 = y * 100;
            float r = float.Parse(txtRMasPort3.Text);
            var r1 = r * 10;

            PLCKey.WriteInt32(PLCAddresses.Data.P3_X_Master, (int)x1);
            PLCKey.WriteInt32(PLCAddresses.Data.P3_Y_Master, (int)y1);
            PLCKey.WriteInt32(PLCAddresses.Data.P3_R_Master, (int)r1);
        }
        private void LoadSelectedRadioPort()
        {
            switch (Properties.Settings.Default.SelectedRadio)
            {
                case "Port1":
                    rbtPort1.Checked = true;
                    break;
                case "Port2":
                    rbtPort2.Checked = true;
                    break;
                case "Port3":
                    rbtPort3.Checked = true;
                    break;
                case "Port4":
                    rbtPort4.Checked = true;
                    break;
            }
        }
        private void LoadIORadioPort()
        {
            switch (Properties.Settings.Default.SelectedRadio)
            {
                case "Port1":
                    rbtIOPort1.Checked = true;
                    break;
                case "Port2":
                    rbtIOPort2.Checked = true;
                    break;
                case "Port3":
                    rbtIOPort3.Checked = true;
                    break;
                case "Port4":
                    rbtIOPort4.Checked = true;
                    break;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSelectedRadioPort();
            LoadIORadioPort();

            // Load available models into ComboBox
            RefreshModelComboBox();

            // Hide model management buttons initially (Jog mode is default)
            btnModelAdd.Enabled = false;
            btnModelDelete.Enabled = false;
            btnModelLoad.Enabled = false;

            // Initialize bypass button colors
            UpdateBypassButtonColors();

            toolStripStatusLabel2.ToolTipText = "PLC Connected";
            toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
            txtXMasPort1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_X_Master) / 100.0f).ToString();
            txtYMasPort1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_Y_Master) / 100.0f).ToString();
            txtRMasPort1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_R_Master) / 10.0f).ToString();
            txtXMasPort3.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_X_Master) / 100.0f).ToString();
            txtYMasPort3.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_Y_Master) / 100.0f).ToString();
            txtRMasPort3.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_R_Master) / 10.0f).ToString();
            txtXMasPort2.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Master) / 100.0f).ToString();
            txtYMasPort2.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Master) / 100.0f).ToString();
            txtRMasPort2.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_R_Master) / 10.0f).ToString();
            txtXMasPort4.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Master) / 100.0f).ToString();
            txtYMasPort4.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Master) / 100.0f).ToString();
            txtRMasPort4.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_R_Master) / 10.0f).ToString();
            if (PLCKey.ReadBit(PLCAddresses.Output.P12_Cam_cylinder))
            {
                button1.Text = "Sang phai";
            }
            else
            {
                button1.Text = "Sang trai";
            }

            if (PLCKey.ReadBit(PLCAddresses.Output.P34_Cam_cylinder))
            {
                button2.Text = "Sang phai";
            }
            else
            {
                button2.Text = "Sang trai";
            }

            // Initialize Data Tab
            InitializeDataTab();
        }

        /// <summary>
        /// Handle PLC connection status changes
        /// </summary>
        private void PLCKey_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Thread-safe UI update
            if (InvokeRequired)
            {
                Invoke(new Action(() => PLCKey_PropertyChanged(sender, e)));
                return;
            }

            // Parse the property name to get connection status
            string status = e.PropertyName;

            if (status == "connected")
            {
                // PLC is connected
                toolStripStatusLabel2.Text = "PLC: Đã kết nối (192.168.0.10:8501)";
                toolStripStatusLabel2.ForeColor = Color.Green;
                toolStripProgressBar1.Visible = false;

                Console.WriteLine("[Form1] PLC connected successfully");
            }
            else if (status=="disconnected")
            {
                // PLC is disconnected
                toolStripStatusLabel2.Text = "PLC: Mất kết nối - Kiểm tra cáp mạng";
                toolStripStatusLabel2.ForeColor = Color.Red;
                toolStripProgressBar1.Visible = true;
                toolStripProgressBar1.Style = ProgressBarStyle.Marquee;

                Console.WriteLine("[Form1] PLC disconnected");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = cam12.Text.Trim();
                int port = 7890;

                if (cameraClient12 != null && cameraClient12.IsConnected)
                {
                    cameraClient12.Disconnect();
                    btnConnect12.Text = "Connect";
                    btnConnect12.BackColor = Color.LightCoral;
                    cam12.Enabled = true;
                    return;
                }

                cameraClient12 = new CameraTcpClient(ip, port);

                if (cameraClient12.Connect())
                {
                    MessageBox.Show("Ket noi camera thanh cong!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnConnect12.Text = "Connected";
                    btnConnect12.BackColor = Color.LightGreen;
                    cam12.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Ket noi camera that bai!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnConnect12.BackColor = Color.LightCoral;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = cam34.Text.Trim();
                int port = 7890;

                if (cameraClient34 != null && cameraClient34.IsConnected)
                {
                    cameraClient34.Disconnect();
                    btnConnect34.Text = "Connect";
                    btnConnect34.BackColor = Color.LightCoral;
                    cam34.Enabled = true;
                    return;
                }

                cameraClient34 = new CameraTcpClient(ip, port);



                if (cameraClient34.Connect())
                {
                    MessageBox.Show("Ket noi camera thanh cong!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnConnect34.Text = "Connected";
                    btnConnect34.BackColor = Color.LightGreen;
                    cam34.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Ket noi camera that bai!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnConnect34.BackColor = Color.LightCoral;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSetMasP2_Click(object sender, EventArgs e)
        {
            float x = float.Parse(txtXMasPort2.Text);
            var x1 = x * 100;
            float y = float.Parse(txtYMasPort2.Text);
            var y1 = y * 100;
            float r = float.Parse(txtRMasPort2.Text);
            var r1 = r * 10;

            PLCKey.WriteInt32(PLCAddresses.Data.P2_X_Master, (int)x1);
            PLCKey.WriteInt32(PLCAddresses.Data.P2_Y_Master, (int)y1);
            PLCKey.WriteInt32(PLCAddresses.Data.P2_R_Master, (int)r1);
        }

        private void btnSetMasP4_Click(object sender, EventArgs e)
        {
            float x = float.Parse(txtXMasPort4.Text);
            var x1 = x * 100;
            float y = float.Parse(txtYMasPort4.Text);
            var y1 = y * 100;
            float r = float.Parse(txtRMasPort4.Text);
            var r1 = r * 10;

            PLCKey.WriteInt32(PLCAddresses.Data.P4_X_Master, (int)x1);
            PLCKey.WriteInt32(PLCAddresses.Data.P4_Y_Master, (int)y1);
            PLCKey.WriteInt32(PLCAddresses.Data.P4_R_Master, (int)r1);
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            if (cameraClient34 == null || !cameraClient34.IsConnected)
            {
                MessageBox.Show("Chua ket noi camera! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var sgp = cameraClient34.SendCommand("GCP,1,HOME2D,0,0,0,0,0,0");
            var sgp_split = sgp.Split(',');
            if (sgp_split[0] == "GCP" && sgp_split[1] == "1")
            {
                //var a = cameraClient34.SendCommand("GCP,1,HOME2D,0,0,0,0,0,0");
                var b = sgp.Split(',');
                txtXMasPort4.Text = b[2].Trim();
                txtYMasPort4.Text = b[3].Trim();
                txtRMasPort4.Text = b[4].Trim();
                MessageBox.Show(sgp);
            }
            else
            {
                MessageBox.Show("Co loi roi");
            }

}

        private void button43_Click(object sender, EventArgs e)
        {
            if (cameraClient12 == null || !cameraClient12.IsConnected)
            {
                MessageBox.Show("Chua ket noi camera! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var sgp = cameraClient12.SendCommand("GCP,1,HOME2D,0,0,0,0,0,0");
            var sgp_split = sgp.Split(',');
            if (sgp_split[0] == "GCP" && sgp_split[1] == "1")
            {
                //var a = cameraClient12.SendCommand("GCP,1,HOME2D,0,0,0,0,0,0");
                var b = sgp.Split(',');
                txtXMasPort2.Text = b[2].Trim();
                txtYMasPort2.Text = b[3].Trim();
                txtRMasPort2.Text = b[4].Trim();
                MessageBox.Show(sgp);
            }
            else
            {
                MessageBox.Show("Co loi roi");
            }
        }

        private void button47_Click(object sender, EventArgs e)
        {
            if (cameraClient12 == null || !cameraClient12.IsConnected)
            {
                MessageBox.Show("Chua ket noi camera! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var sgp = cameraClient12.SendCommand("GCP,2,HOME2D,0,0,0,0,0,0");
            var sgp_split = sgp.Split(',');
            if (sgp_split[0] == "GCP" && sgp_split[1] == "1")
            {
                //var a = cameraClient12.SendCommand("GCP,2,HOME2D,0,0,0,0,0,0");
                var b = sgp.Split(',');
                txtXMasPort1.Text = b[2].Trim();
                txtYMasPort1.Text = b[3].Trim();
                txtRMasPort1.Text = b[4].Trim();
                MessageBox.Show(sgp);
            }
            else
            {
                MessageBox.Show("Co loi roi");
            }
        }

        private void button48_Click(object sender, EventArgs e)
        {
            if (cameraClient34 == null || !cameraClient34.IsConnected)
            {
                MessageBox.Show("Chua ket noi camera! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var sgp = cameraClient34.SendCommand("GCP,2,Home2D,0,0,0,0,0,0");
         
            var sgp_split = sgp.Split(',');
            if (sgp_split[0] == "GCP" && sgp_split[1] == "1")
            {
                //var a = cameraClient34.SendCommand("GCP,2,HOME2D,0,0,0,0,0,0");
                //var b = sgp.Split(',');
                txtXMasPort3.Text = sgp_split[2].Trim();
                txtYMasPort3.Text = sgp_split[3].Trim();
                txtRMasPort3.Text = sgp_split[4].Trim();
                MessageBox.Show(sgp);
            }
            else
            {
                MessageBox.Show("Co loi roi");
            }
        }

        private async Task RunABSPosXYRCam1Async(int x_pos,int y_pos,int r_pos)
        {
            PLCKey.WriteInt32(PLCAddresses.Data.P2_X_ABS_Setpoint, x_pos);
            await Task.Delay(500);
            PLCKey.WriteInt32(PLCAddresses.Data.P2_Y_ABS_Setpoint, y_pos);
            await Task.Delay(500);
            PLCKey.WriteInt32(PLCAddresses.Data.P2_RI_INC_Setpoint, r_pos);
            await Task.Delay(500);
            // Set bit de chay
            PLCKey.SetBit(PLCAddresses.Input.P2_XGo_ABS);
            await Task.Delay(2000);
            PLCKey.SetBit(PLCAddresses.Input.P2_YGo_ABS);
            await Task.Delay(2000);
            PLCKey.SetBit(PLCAddresses.Input.P2_RIGo_INC);
            await Task.Delay(2000);
        }

        private async Task RunABSPosXYRCam2Async(int x_pos, int y_pos, int r_pos)
        {
            PLCKey.WriteInt32(PLCAddresses.Data.P4_X_ABS_Setpoint, x_pos);
            await Task.Delay(500);
            PLCKey.WriteInt32(PLCAddresses.Data.P4_Y_ABS_Setpoint, y_pos);
            await Task.Delay(500);
            PLCKey.WriteInt32(PLCAddresses.Data.P4_RI_INC_Setpoint, r_pos);
            await Task.Delay(500);
            // Set bit de chay
            PLCKey.SetBit(PLCAddresses.Input.P4_XGo_ABS);
            await Task.Delay(2000);
            PLCKey.SetBit(PLCAddresses.Input.P4_YGo_ABS);
            await Task.Delay(2000);
            PLCKey.SetBit(PLCAddresses.Input.P4_RIGo_INC);
            await Task.Delay(2000);
        }

        enum Cmd_Type
        {
            Start,
            Send_Point,
            End
        }

        private async Task<bool> RunCmdSendCamera1(Cmd_Type type,float r = 0)
        {
            if (cbTestCam1.Checked) return true;
            if (cameraClient12 == null || !cameraClient12.IsConnected) return false;
            int x = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur);
            int y = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur);
            switch (type)
            {
                case Cmd_Type.Start:
                    string response = cameraClient12.SendCommand("HEB,1");
                    await Task.Delay(500);
                    if (!response.Contains("HEB,1")) return false;
                    break;
                case Cmd_Type.Send_Point:
                    string cmd1 = $"HE,1,1,{x / 100.0f:F2},{y / 100.0f:F2},0,{r},0,0,0";
                    response = cameraClient12.SendCommand(cmd1);
                    if (!response.Contains("HE,1")) return false;
                    break;
                case Cmd_Type.End:
                    response = cameraClient12.SendCommand("HEE,1");
                    await Task.Delay(500);
                    if (!response.Contains("HEE,1")) return false;
                    break;
                default:
                    return false;
            }
            return true;
        }

        private async Task<bool> RunCmdSendCamera2(Cmd_Type type, float r = 0)
        {
            if (cbTestCam2.Checked) return true;
            if (cameraClient34 == null || !cameraClient34.IsConnected) return false;

            int x = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur);
            int y = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur);
            switch (type)
            {
                case Cmd_Type.Start:
                    string response = cameraClient34.SendCommand("HEB,1");
                    await Task.Delay(500);
                    if (!response.Contains("HEB,1")) return false;
                    break;
                case Cmd_Type.Send_Point:
                    string cmd1 = $"HE,1,1,{x / 100.0f:F2},{y / 100.0f:F2},0,{r},0,0,0";
                    response = cameraClient34.SendCommand(cmd1);
                    if (!response.Contains("HE,1")) return false;
                    break;
                case Cmd_Type.End:
                    response = cameraClient34.SendCommand("HEE,1");
                    await Task.Delay(500);
                    if (!response.Contains("HEE,1")) return false;
                    break;
                default:
                    return false;
            }
            return true;
        }
        #region HandEye
        bool cam1HandEyeStart = false;
        bool cam2HandEyeStart = false;

        private void ClearCam1HandEyeStatus()
        {
            cam1HandEyeStart = false;
            btnCalPos1.Enabled = true;
            XY1Cam1.BackColor = Color.Transparent;
            XY2Cam1.BackColor = Color.Transparent;
            XY3Cam1.BackColor = Color.Transparent;
            XY4Cam1.BackColor = Color.Transparent;
            XY5Cam1.BackColor = Color.Transparent;
            XY6Cam1.BackColor = Color.Transparent;
            XY7Cam1.BackColor = Color.Transparent;
            XY8Cam1.BackColor = Color.Transparent;
            XY9Cam1.BackColor = Color.Transparent;
            XYR1Cam1.BackColor = Color.Transparent;
            XYR2Cam1.BackColor = Color.Transparent;
        }

        private void ClearCam2HandEyeStatus()
        {
            cam2HandEyeStart = false;
            btnCalPos2.Enabled = true;
            XY1Cam2.BackColor = Color.Transparent;
            XY2Cam2.BackColor = Color.Transparent;
            XY3Cam2.BackColor = Color.Transparent;
            XY4Cam2.BackColor = Color.Transparent;
            XY5Cam2.BackColor = Color.Transparent;
            XY6Cam2.BackColor = Color.Transparent;
            XY7Cam2.BackColor = Color.Transparent;
            XY8Cam2.BackColor = Color.Transparent;
            XY9Cam2.BackColor = Color.Transparent;
            XYR1Cam2.BackColor = Color.Transparent;
            XYR2Cam2.BackColor = Color.Transparent;
        }
        // Handeye functions for Camera 1 (Port 2)
        private async void btnCalPos1_Click(object sender, EventArgs e)
        {
            if (cam1HandEyeStart) return;
            cam1HandEyeStart=true;
            btnCalPos1.Enabled = false ;
            var _dialogResult = MessageBox.Show("Bạn có chắc chắn không", "Cảnh báo",MessageBoxButtons.YesNo);
            if (_dialogResult == DialogResult.No)
            {
                return;
            }
            if (cameraClient12 == null || !cameraClient12.IsConnected)
            {
                MessageBox.Show("Camera chưa kết nối","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
         
            try
            {
                bool cmd_result = await RunCmdSendCamera1(Cmd_Type.Start);
                if (!cmd_result) 
                {
                    MessageBox.Show("Bat dau handeye khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Lay buoc di chuyen
                int stepX = int.Parse(txtXStep1.Text)*100;
                int stepY = int.Parse(txtYStep1.Text)*100;

                int orgXPos = 0;
                int orgYPos = 0;

                // Tinh toan 9 diem 
                // Diem 1: Trung tam (X, Y)
                int x1 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur);
                int y1 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur);
                orgXPos = x1;
                orgYPos = y1;
                await RunABSPosXYRCam1Async(x1, y1,0);
                XY1Cam1.Text = $"{x1/100.0f:F2},{y1/100.0f:F2}";
                XY1Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point,0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 1 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 2: (X+stepX, Y+stepY)
                int x2 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur) - stepX;
                int y2 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur);
                await RunABSPosXYRCam1Async(x2, y2,0);
                XY2Cam1.Text = $"{x2/100.0f:F2},{y2/100.0f:F2}";
                XY2Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 2 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 3: (X+2*stepX, Y+2*stepY)
                int x3 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur);
                int y3 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur) + stepY;
                await RunABSPosXYRCam1Async(x3, y3,0);
                XY3Cam1.Text = $"{x3/100.0f:F2},{y3/100.0f:F2}";
                XY3Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 3 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 4: (X+stepX, Y)
                int x4 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur) + stepX;
                int y4 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur);
                await RunABSPosXYRCam1Async(x4, y4,0);
                XY4Cam1.Text = $"{x4/100.0f:F2},{y4/100.0f:F2}";
                XY4Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 4 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 5: (X, Y+stepY)
                int x5 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur) + stepX;
                int y5 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur);
                await RunABSPosXYRCam1Async(x5, y5,0);
                XY5Cam1.Text = $"{x5/100.0f:F2},{y5/100.0f:F2}";
                XY5Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 5 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 6: (X, Y+2*stepY)
                int x6 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur);
                int y6 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur) - stepY;
                await RunABSPosXYRCam1Async(x6, y6,0);
                XY6Cam1.Text = $"{x6/100.0f:F2},{y6/100.0f:F2}";
                XY6Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 6 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 7: (X-stepX, Y-stepY)
                int x7 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur);
                int y7 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur) - stepY;
                await RunABSPosXYRCam1Async(x7, y7,0);
                XY7Cam1.Text = $"{x7/100.0f:F2},{y7/100.0f:F2}";
                XY7Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 7 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 8: (X+stepX, Y-stepY)
                int x8 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur) - stepX;
                int y8 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur);
                await RunABSPosXYRCam1Async(x8, y8,0);
                XY8Cam1.Text = $"{x8/100.0f:F2},{y8/100.0f:F2}";
                XY8Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 9 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 9: (X+2*stepX, Y-stepY)
                int x9 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur) - stepX;
                int y9 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur);
                await RunABSPosXYRCam1Async(x9, y9,0);
                XY9Cam1.Text = $"{x9/100.0f:F2},{y9/100.0f:F2}";
                XY9Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 9 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 10: (X+2*stepX, Y-stepY)
                int x10 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur) + stepX;
                int y10 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur) + stepY;
                await RunABSPosXYRCam1Async(x10, y10,-100);
                XYR1Cam1.Text = $"{x10 / 100.0f:F2},{y10 / 100.0f:F2}";
                XYR1Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point, -10);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 10 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 100);
                    return;
                }

                // Diem 11: (X+2*stepX, Y-stepY)
                int x11 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur);
                int y11 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur);
                await RunABSPosXYRCam1Async(x10, y10, 200);
                XYR2Cam1.Text = $"{x10 / 100.0f:F2} , {y10 / 100.0f:F2}";
                XYR2Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point, 10);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 10 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, -100);
                    return;
                }

                await Task.Delay(1000);

                cmd_result = await RunCmdSendCamera1(Cmd_Type.End);
                if (!cmd_result)
                {
                    MessageBox.Show("Ket thuc handeye khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, -100);
                    return;
                }

                await RunABSPosXYRCam1Async(x11, y11, -100);

                MessageBox.Show("Da tinh toan xong 11 diem XY va 2 diem R cho Camera 1!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi khi tinh toan vi tri: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnCalPos1.Enabled = true;
        }

        // Handeye functions for Camera 34 (Port 4)
        private async void btnCalPos2_Click(object sender, EventArgs e)
        {
            if (cam2HandEyeStart) return;
            cam2HandEyeStart = true;
            btnCalPos2.Enabled = false;
            var _dialogResult = MessageBox.Show("Bạn có chắc chắn không", "Cảnh báo", MessageBoxButtons.YesNo);
            if (_dialogResult == DialogResult.No)
            {
                return;
            }
            if (cameraClient34 == null || !cameraClient34.IsConnected)
            {
                MessageBox.Show("Camera chưa kết nối", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            try
            {
                bool cmd_result = await RunCmdSendCamera2(Cmd_Type.Start);
                if (!cmd_result)
                {
                    MessageBox.Show("Bat dau handeye khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Lay buoc di chuyen
                int stepX = int.Parse(txtXStep2.Text) * 100;
                int stepY = int.Parse(txtYStep2.Text) * 100;

                int orgXPos = 0;
                int orgYPos = 0;

                // Tinh toan 9 diem 
                // Diem 1: Trung tam (X, Y)
                int x1 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur);
                int y1 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur);
                orgXPos = x1;
                orgYPos = y1;
                await RunABSPosXYRCam2Async(x1, y1, 0);
                XY1Cam2.Text = $"{x1 / 100.0f:F2},{y1 / 100.0f:F2}";
                XY1Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 1 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 2: (X+stepX, Y+stepY)
                int x2 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur) - stepX;
                int y2 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur);
                await RunABSPosXYRCam2Async(x2, y2, 0);
                XY2Cam2.Text = $"{x2 / 100.0f:F2},{y2 / 100.0f:F2}";
                XY2Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 2 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 3: (X+2*stepX, Y+2*stepY)
                int x3 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur);
                int y3 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur) + stepY;
                await RunABSPosXYRCam2Async(x3, y3, 0);
                XY3Cam2.Text = $"{x3 / 100.0f:F2},{y3 / 100.0f:F2}";
                XY3Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 3 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 4: (X+stepX, Y)
                int x4 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur) + stepX;
                int y4 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur);
                await RunABSPosXYRCam2Async(x4, y4, 0);
                XY4Cam2.Text = $"{x4 / 100.0f:F2},{y4 / 100.0f:F2}";
                XY4Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 4 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 5: (X, Y+stepY)
                int x5 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur) + stepX;
                int y5 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur);
                await RunABSPosXYRCam2Async(x5, y5, 0);
                XY5Cam2.Text = $"{x5 / 100.0f:F2},{y5 / 100.0f:F2}";
                XY5Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 5 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 6: (X, Y+2*stepY)
                int x6 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur);
                int y6 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur) - stepY;
                await RunABSPosXYRCam2Async(x6, y6, 0);
                XY6Cam2.Text = $"{x6 / 100.0f:F2},{y6 / 100.0f:F2}";
                XY6Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 6 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 7: (X-stepX, Y-stepY)
                int x7 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur);
                int y7 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur) - stepY;
                await RunABSPosXYRCam2Async(x7, y7, 0);
                XY7Cam2.Text = $"{x7 / 100.0f:F2},{y7 / 100.0f:F2}";
                XY7Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 7 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 8: (X+stepX, Y-stepY)
                int x8 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur) - stepX;
                int y8 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur);
                await RunABSPosXYRCam2Async(x8, y8, 0);
                XY8Cam2.Text = $"{x8 / 100.0f:F2},{y8 / 100.0f:F2}";
                XY8Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 9 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 9: (X+2*stepX, Y-stepY)
                int x9 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur) - stepX;
                int y9 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur);
                await RunABSPosXYRCam2Async(x9, y9, 0);
                XY9Cam2.Text = $"{x9 / 100.0f:F2},{y9 / 100.0f:F2}";
                XY9Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 9 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 0);
                    return;
                }

                // Diem 10: (X+2*stepX, Y-stepY)
                int x10 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur) + stepX;
                int y10 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur) + stepY;
                await RunABSPosXYRCam2Async(x10, y10, -100);
                XYR1Cam2.Text = $"{x10 / 100.0f:F2},{y10 / 100.0f:F2}";
                XYR1Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, -10);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 10 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, 100);
                    return;
                }

                // Diem 11: (X+2*stepX, Y-stepY)
                int x11 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur);
                int y11 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur);
                await RunABSPosXYRCam2Async(x10, y10, 200);
                XYR2Cam2.Text = $"{x10 / 100.0f:F2} , {y10 / 100.0f:F2}";
                XYR2Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, 10);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 10 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, -100);
                    return;
                }

                await Task.Delay(1000);

                cmd_result = await RunCmdSendCamera2(Cmd_Type.End);
                if (!cmd_result)
                {
                    MessageBox.Show("Ket thuc handeye khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RunABSPosXYRCam2Async(orgXPos, orgYPos, -100);
                    return;
                }

                await RunABSPosXYRCam2Async(x11, y11, -100);

                MessageBox.Show("Da tinh toan xong 11 diem XY va 2 diem R cho Camera 1!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi khi tinh toan vi tri: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnCalPos2.Enabled = true;
        }
        #endregion

        #region Motion Control - Keyboard and Radio Button Handlers

        // Keyboard shortcut handler
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Check for Ctrl+H to show hotkey help (works in any mode)
            CheckHotkeyHelpShortcut(e);
            if (e.Handled) return;

            // Check if we're in Teaching Mode and handle teaching hotkeys first
            if (rbtTeachingMode.Checked && hotkeyManager != null)
            {
                var hotkey = hotkeyManager.FindHotkey(e);
                if (hotkey != null)
                {
                    // Execute teaching hotkey action
                    ExecuteTeachingHotkey(hotkey);
                    e.Handled = true;
                    e.SuppressKeyPress = true; // Prevent beep sound
                    return;
                }
            }

            // Original Jog Mode hotkeys (only when not in Teaching Mode or no teaching hotkey matched)
            // Port selection (1-4) - Only in Jog Mode without modifiers
            if (e.KeyCode == Keys.F1 && !e.Control && !e.Alt && !e.Shift)
            {
                GetStepJogMode();
                rbtPort1.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F2 && !e.Control && !e.Alt && !e.Shift)
            {
                GetStepJogMode();
                rbtPort2.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F3 && !e.Control && !e.Alt && !e.Shift)
            {
                GetStepJogMode();
                rbtPort3.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F4 && !e.Control && !e.Alt && !e.Shift)
            {
                GetStepJogMode();
                rbtPort4.Checked = true;
                e.Handled = true;
            }
            // Axis selection (X, Y, Z, I, O, F)
            else if (e.KeyCode == Keys.X && !e.Control && !e.Alt)
            {
                rbtX.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Y && !e.Control && !e.Alt)
            {
                rbtY.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Z && !e.Control && !e.Alt)
            {
                rbtZ.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.I && !e.Control && !e.Alt)
            {
                rbtRI.Checked = true;  // RI
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.O && !e.Control && !e.Alt)
            {
                rbtRO.Checked = true;  // RO
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F && !e.Control && !e.Alt)
            {
                rbtF.Checked = true;  // F
                e.Handled = true;
            }
            // Mode selection (J=JOG, S=STEP)
            else if (e.KeyCode == Keys.Space)
            {
                if (rbtJog.Checked) rbtStep.Checked = true;
                else rbtJog.Checked = true;
                e.Handled = true;
            }
            // JOG control with Up/Down keys
            else if (e.KeyCode == Keys.Q)
            {
                string address = GetJogPlusAddress(selectedPort, selectedAxis);
                if (!string.IsNullOrEmpty(address))
                {
                    PLCKey.SetBit(address);
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.A)
            {
                string address = GetJogMinusAddress(selectedPort, selectedAxis);
                if (!string.IsNullOrEmpty(address))
                {
                    PLCKey.SetBit(address);
                }
                e.Handled = true;
            }
        }

        // Handle KeyUp to release JOG buttons
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
            {
                string address = GetJogPlusAddress(selectedPort, selectedAxis);
                if (!string.IsNullOrEmpty(address))
                {
                    PLCKey.ResetBit(address);
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.A)
            {
                string address = GetJogMinusAddress(selectedPort, selectedAxis);
                if (!string.IsNullOrEmpty(address))
                {
                    PLCKey.ResetBit(address);
                }
                e.Handled = true;
            }
        }

        // Port selection changed
        private void PortRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                rbtJogMode.Checked = true;
                if (rb == rbtPort1)      { selectedPort = 1; selectedIOPort = 1; rbtIOPort1.Checked = true; rbtIOPort1.ForeColor = Color.Red; rbtIOPort2.ForeColor = Color.Black; rbtIOPort3.ForeColor = Color.Black; rbtIOPort4.ForeColor = Color.Black; }
                else if (rb == rbtPort2) { selectedPort = 2; selectedIOPort = 2; rbtIOPort2.Checked = true; rbtIOPort2.ForeColor = Color.Red; rbtIOPort1.ForeColor = Color.Black; rbtIOPort3.ForeColor = Color.Black; rbtIOPort4.ForeColor = Color.Black;}
                else if (rb == rbtPort3) { selectedPort = 3; selectedIOPort = 3; rbtIOPort3.Checked = true; rbtIOPort3.ForeColor = Color.Red; rbtIOPort2.ForeColor = Color.Black; rbtIOPort1.ForeColor = Color.Black; rbtIOPort4.ForeColor = Color.Black;}
                else if (rb == rbtPort4) { selectedPort = 4; selectedIOPort = 4; rbtIOPort4.Checked = true; rbtIOPort4.ForeColor = Color.Red; rbtIOPort2.ForeColor = Color.Black; rbtIOPort3.ForeColor = Color.Black; rbtIOPort1.ForeColor = Color.Black; }

                // Update current position displays for new port
                UpdateCurrentPositionDisplays();

                // Update limit switch displays for new port
                UpdateLimitSwitchDisplays();

                // Load Speed and Step values for new port
                LoadAxisSpeedAndStep();

                // Load Current Jog Mode
                GetStepJogMode();

                // Immediately update IO displays for new port
                UpdateIOInputs();
                UpdateIOOutputs();

                // Update bypass button colors for new port
                UpdateBypassButtonColors();
            }
        }

        private void IOPortRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                rbtJogMode.Checked = true;
                if (rb == rbtIOPort1) { selectedIOPort = 1; selectedPort = 1;      rbtPort1.Checked = true; rbtPort1.ForeColor = Color.Red; rbtPort2.ForeColor = Color.Black; rbtPort3.ForeColor = Color.Black; rbtPort4.ForeColor = Color.Black; }
                else if (rb == rbtIOPort2) { selectedIOPort = 2; selectedPort = 2; rbtPort2.Checked = true; rbtPort2.ForeColor = Color.Red; rbtPort1.ForeColor = Color.Black; rbtPort3.ForeColor = Color.Black; rbtPort4.ForeColor = Color.Black;}
                else if (rb == rbtIOPort3) { selectedIOPort = 3; selectedPort = 3; rbtPort3.Checked = true; rbtPort3.ForeColor = Color.Red; rbtPort2.ForeColor = Color.Black; rbtPort1.ForeColor = Color.Black; rbtPort4.ForeColor = Color.Black;}
                else if (rb == rbtIOPort4) { selectedIOPort = 4; selectedPort = 4; rbtPort4.Checked = true; rbtPort4.ForeColor = Color.Red; rbtPort2.ForeColor = Color.Black; rbtPort3.ForeColor = Color.Black; rbtPort1.ForeColor = Color.Black; }

                // Update current position displays for new port
                UpdateCurrentPositionDisplays();

                // Update limit switch displays for new port
                UpdateLimitSwitchDisplays();

                // Load Speed and Step values for new port
                LoadAxisSpeedAndStep();

                // Load Current Jog Mode
                GetStepJogMode();

                // Immediately update IO displays for new port
                UpdateIOInputs();
                UpdateIOOutputs();

                // Update bypass button colors for new port
                UpdateBypassButtonColors();
            }
        }

        // Axis selection changed
        private void AxisRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                if (rb == rbtX) selectedAxis = "X";
                else if (rb == rbtY) selectedAxis = "Y";
                else if (rb == rbtZ) selectedAxis = "Z";
                else if (rb == rbtRI) selectedAxis = "RI";
                else if (rb == rbtRO) selectedAxis = "RO";
                else if (rb == rbtF) selectedAxis = "F";
            }
        }

        // Mode selection changed (JOG/STEP)
        private void ModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                if (rb == rbtJog)  // JOG
                {
                    isJogMode = true;
                    SetStepJogMode(false);  // Reset Step_Jog_Mode bit
                }
                else if (rb == rbtStep)  // STEP
                {
                    isJogMode = false;
                    SetStepJogMode(true);  // Set Step_Jog_Mode bit
                }
            }
        }

        // Set or reset the P[1-4]_Step_Jog_Mode bit based on selected port
        private void SetStepJogMode(bool isStepMode)
        {
            string address = GetStepJogModeAddress(selectedPort);
            if (!string.IsNullOrEmpty(address))
            {
                if (isStepMode)
                    PLCKey.SetBit(address);
                else
                    PLCKey.ResetBit(address);
            }
        }

        private bool GetStepJogMode()
        {
            string address = GetStepJogModeAddress(selectedPort);
            if (!string.IsNullOrEmpty(address))
            {
                if (PLCKey.ReadBit(address))
                {
                    rbtStep.Checked = true;
                    return true;
                }
                else
                {
                    rbtJog.Checked = true;
                    return false;
                }
            }
            else { return false; }
        }

        // Get Step_Jog_Mode address for the selected port
        private string GetStepJogModeAddress(int port)
        {
            switch (port)
            {
                case 1: return PLCAddresses.Input.P1_Step_Jog_Mode;
                case 2: return PLCAddresses.Input.P2_Step_Jog_Mode;
                case 3: return PLCAddresses.Input.P3_Step_Jog_Mode;
                case 4: return PLCAddresses.Input.P4_Step_Jog_Mode;
                default: return null;
            }
        }

        // Get JOG Plus address for selected port and axis
        private string GetJogPlusAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P1_XJog_Plus;
                        case "Y": return PLCAddresses.Input.P1_YJog_Plus;
                        case "Z": return PLCAddresses.Input.P1_ZJog_Plus;
                        case "RI": return PLCAddresses.Input.P1_RIJog_Plus;
                        case "RO": return PLCAddresses.Input.P1_ROJog_Plus;
                        case "F": return PLCAddresses.Input.P1_FJog_Plus;
                    }
                    break;
                case 2:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P2_XJog_Plus;
                        case "Y": return PLCAddresses.Input.P2_YJog_Plus;
                        case "Z": return PLCAddresses.Input.P2_ZJog_Plus;
                        case "RI": return PLCAddresses.Input.P2_RIJog_Plus;
                        case "RO": return PLCAddresses.Input.P2_ROJog_Plus;
                        case "F": return PLCAddresses.Input.P2_FJog_Plus;
                    }
                    break;
                case 3:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P3_XJog_Plus;
                        case "Y": return PLCAddresses.Input.P3_YJog_Plus;
                        case "Z": return PLCAddresses.Input.P3_ZJog_Plus;
                        case "RI": return PLCAddresses.Input.P3_RIJog_Plus;
                        case "RO": return PLCAddresses.Input.P3_ROJog_Plus;
                        case "F": return PLCAddresses.Input.P3_FJog_Plus;
                    }
                    break;
                case 4:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P4_XJog_Plus;
                        case "Y": return PLCAddresses.Input.P4_YJog_Plus;
                        case "Z": return PLCAddresses.Input.P4_ZJog_Plus;
                        case "RI": return PLCAddresses.Input.P4_RIJog_Plus;
                        case "RO": return PLCAddresses.Input.P4_ROJog_Plus;
                        case "F": return PLCAddresses.Input.P4_FJog_Plus;
                    }
                    break;
            }
            return null;
        }

        // Get JOG Minus address for selected port and axis
        private string GetJogMinusAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P1_XJog_Minus;
                        case "Y": return PLCAddresses.Input.P1_YJog_Minus;
                        case "Z": return PLCAddresses.Input.P1_ZJog_Minus;
                        case "RI": return PLCAddresses.Input.P1_RIJog_Minus;
                        case "RO": return PLCAddresses.Input.P1_ROJog_Minus;
                        case "F": return PLCAddresses.Input.P1_FJog_Minus;
                    }
                    break;
                case 2:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P2_XJog_Minus;
                        case "Y": return PLCAddresses.Input.P2_YJog_Minus;
                        case "Z": return PLCAddresses.Input.P2_ZJog_Minus;
                        case "RI": return PLCAddresses.Input.P2_RIJog_Minus;
                        case "RO": return PLCAddresses.Input.P2_ROJog_Minus;
                        case "F": return PLCAddresses.Input.P2_FJog_Minus;
                    }
                    break;
                case 3:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P3_XJog_Minus;
                        case "Y": return PLCAddresses.Input.P3_YJog_Minus;
                        case "Z": return PLCAddresses.Input.P3_ZJog_Minus;
                        case "RI": return PLCAddresses.Input.P3_RIJog_Minus;
                        case "RO": return PLCAddresses.Input.P3_ROJog_Minus;
                        case "F": return PLCAddresses.Input.P3_FJog_Minus;
                    }
                    break;
                case 4:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P4_XJog_Minus;
                        case "Y": return PLCAddresses.Input.P4_YJog_Minus;
                        case "Z": return PLCAddresses.Input.P4_ZJog_Minus;
                        case "RI": return PLCAddresses.Input.P4_RIJog_Minus;
                        case "RO": return PLCAddresses.Input.P4_ROJog_Minus;
                        case "F": return PLCAddresses.Input.P4_FJog_Minus;
                    }
                    break;
            }
            return null;
        }

        // Generic JOG Plus button MouseDown event handler
        private void btnJogPlus_MouseDown(object sender, MouseEventArgs e)
        {
            // Check if port is in Auto Mode
            if (IsAutoModeActive(selectedPort))
            {
                MessageBox.Show(
                    $"Port {selectedPort} đang ở chế độ Auto!\nKhông thể Jog khi Auto đang chạy.",
                    "Auto Mode Active",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string address = GetJogPlusAddress(selectedPort, selectedAxis);
            if (!string.IsNullOrEmpty(address))
            {
                PLCKey.SetBit(address);
            }
        }

        // Generic JOG Plus button MouseUp event handler
        private void btnJogPlus_MouseUp(object sender, MouseEventArgs e)
        {
            string address = GetJogPlusAddress(selectedPort, selectedAxis);
            if (!string.IsNullOrEmpty(address))
            {
                PLCKey.ResetBit(address);
            }
        }

        // Generic JOG Minus button MouseDown event handler
        private void btnJogMinus_MouseDown(object sender, MouseEventArgs e)
        {
            // Check if port is in Auto Mode
            if (IsAutoModeActive(selectedPort))
            {
                MessageBox.Show(
                    $"Port {selectedPort} đang ở chế độ Auto!\nKhông thể Jog khi Auto đang chạy.",
                    "Auto Mode Active",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string address = GetJogMinusAddress(selectedPort, selectedAxis);
            if (!string.IsNullOrEmpty(address))
            {
                PLCKey.SetBit(address);
            }
        }

        // Generic JOG Minus button MouseUp event handler
        private void btnJogMinus_MouseUp(object sender, MouseEventArgs e)
        {
            string address = GetJogMinusAddress(selectedPort, selectedAxis);
            if (!string.IsNullOrEmpty(address))
            {
                PLCKey.ResetBit(address);
            }
        }

        // Get current position address for selected port and axis
        private string GetPosCurAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Data.P1_X_Pos_Cur;
                        case "Y": return PLCAddresses.Data.P1_Y_Pos_Cur;
                        case "Z": return PLCAddresses.Data.P1_Z_Pos_Cur;
                        case "RI": return PLCAddresses.Data.P1_RI_Pos_Cur;
                        case "RO": return PLCAddresses.Data.P1_RO_Pos_Cur;
                        case "F": return PLCAddresses.Data.P1_F_Pos_Cur;
                    }
                    break;
                case 2:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Data.P2_X_Pos_Cur;
                        case "Y": return PLCAddresses.Data.P2_Y_Pos_Cur;
                        case "Z": return PLCAddresses.Data.P2_Z_Pos_Cur;
                        case "RI": return PLCAddresses.Data.P2_RI_Pos_Cur;
                        case "RO": return PLCAddresses.Data.P2_RO_Pos_Cur;
                        case "F": return PLCAddresses.Data.P2_F_Pos_Cur;
                    }
                    break;
                case 3:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Data.P3_X_Pos_Cur;
                        case "Y": return PLCAddresses.Data.P3_Y_Pos_Cur;
                        case "Z": return PLCAddresses.Data.P3_Z_Pos_Cur;
                        case "RI": return PLCAddresses.Data.P3_RI_Pos_Cur;
                        case "RO": return PLCAddresses.Data.P3_RO_Pos_Cur;
                        case "F": return PLCAddresses.Data.P3_F_Pos_Cur;
                    }
                    break;
                case 4:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Data.P4_X_Pos_Cur;
                        case "Y": return PLCAddresses.Data.P4_Y_Pos_Cur;
                        case "Z": return PLCAddresses.Data.P4_Z_Pos_Cur;
                        case "RI": return PLCAddresses.Data.P4_RI_Pos_Cur;
                        case "RO": return PLCAddresses.Data.P4_RO_Pos_Cur;
                        case "F": return PLCAddresses.Data.P4_F_Pos_Cur;
                    }
                    break;
            }
            return null;
        }

        // Get ABS setpoint address for selected port and axis
        private string GetABSSetpointAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Data.P1_X_ABS_Setpoint;
                        case "Y": return PLCAddresses.Data.P1_Y_ABS_Setpoint;
                        case "RI": return PLCAddresses.Data.P1_RI_ABS_Setpoint;
                    }
                    break;
                case 2:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Data.P2_X_ABS_Setpoint;
                        case "Y": return PLCAddresses.Data.P2_Y_ABS_Setpoint;
                        case "RI": return PLCAddresses.Data.P2_RI_ABS_Setpoint;
                    }
                    break;
                case 3:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Data.P3_X_ABS_Setpoint;
                        case "Y": return PLCAddresses.Data.P3_Y_ABS_Setpoint;
                        case "RI": return PLCAddresses.Data.P3_RI_ABS_Setpoint;
                    }
                    break;
                case 4:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Data.P4_X_ABS_Setpoint;
                        case "Y": return PLCAddresses.Data.P4_Y_ABS_Setpoint;
                        case "RI": return PLCAddresses.Data.P4_RI_ABS_Setpoint;
                    }
                    break;
            }
            return null;
        }

        // Get Home bit address
        private string GetHomeAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P1_XHome;
                        case "Y": return PLCAddresses.Input.P1_YHome;
                        case "Z": return PLCAddresses.Input.P1_ZHome;
                        case "RI": return PLCAddresses.Input.P1_RIHome;
                        case "RO": return PLCAddresses.Input.P1_ROHome;
                        case "F": return PLCAddresses.Input.P1_FHome;
                    }
                    break;
                case 2:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P2_XHome;
                        case "Y": return PLCAddresses.Input.P2_YHome;
                        case "Z": return PLCAddresses.Input.P2_ZHome;
                        case "RI": return PLCAddresses.Input.P2_RIHome;
                        case "RO": return PLCAddresses.Input.P2_ROHome;
                        case "F": return PLCAddresses.Input.P2_FHome;
                    }
                    break;
                case 3:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P3_XHome;
                        case "Y": return PLCAddresses.Input.P3_YHome;
                        case "Z": return PLCAddresses.Input.P3_ZHome;
                        case "RI": return PLCAddresses.Input.P3_RIHome;
                        case "RO": return PLCAddresses.Input.P3_ROHome;
                        case "F": return PLCAddresses.Input.P3_FHome;
                    }
                    break;
                case 4:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P4_XHome;
                        case "Y": return PLCAddresses.Input.P4_YHome;
                        case "Z": return PLCAddresses.Input.P4_ZHome;
                        case "RI": return PLCAddresses.Input.P4_RIHome;
                        case "RO": return PLCAddresses.Input.P4_ROHome;
                        case "F": return PLCAddresses.Input.P4_FHome;
                    }
                    break;
            }
            return null;
        }

        // Update current position displays for all axes of selected port
        private void UpdateCurrentPositionDisplays()
        {
            try
            {
                // X axis
                string addrX = GetPosCurAddress(selectedPort, "X");
                if (!string.IsNullOrEmpty(addrX))
                {
                    int posX = PLCKey.ReadInt32(addrX);
                    txtXCur.Text = (posX / 100.0f).ToString("F2");
                }

                // Y axis
                string addrY = GetPosCurAddress(selectedPort, "Y");
                if (!string.IsNullOrEmpty(addrY))
                {
                    int posY = PLCKey.ReadInt32(addrY);
                    txtYCur.Text = (posY / 100.0f).ToString("F2");
                }

                // Z axis
                string addrZ = GetPosCurAddress(selectedPort, "Z");
                if (!string.IsNullOrEmpty(addrZ))
                {
                    int posZ = PLCKey.ReadInt32(addrZ);
                    txtZCur.Text = (posZ / 100.0f).ToString("F2");
                }

                // RI axis
                string addrRI = GetPosCurAddress(selectedPort, "RI");
                if (!string.IsNullOrEmpty(addrRI))
                {
                    int posRI = PLCKey.ReadInt32(addrRI);
                    txtRICur.Text = (posRI / 10.0f).ToString("F2");
                }

                // RO axis
                string addrRO = GetPosCurAddress(selectedPort, "RO");
                if (!string.IsNullOrEmpty(addrRO))
                {
                    int posRO = PLCKey.ReadInt32(addrRO);
                    txtROCur.Text = (posRO / 10.0f).ToString("F2");
                }

                // F axis
                string addrF = GetPosCurAddress(selectedPort, "F");
                if (!string.IsNullOrEmpty(addrF))
                {
                    int posF = PLCKey.ReadInt32(addrF);
                    txtFCur.Text = (posF / 100.0f).ToString("F2");
                }
            }
            catch (Exception ex)
            {
                // Handle errors silently or log
            }
        }

        /// <summary>
        /// Update limit switch indicator lamps (L-, Home, L+) based on PLC status
        /// </summary>
        private void UpdateLimitSwitchDisplays()
        {
            try
            {
                // Read limit switches based on selected port
                switch (selectedPort)
                {
                    case 1:
                        UpdateLimitLamp(lampXLimitMinus, PLCAddresses.Input.P1_X_LimitMinus);
                        UpdateLimitLamp(lampXHome, PLCAddresses.Input.P1_X_Home);
                        UpdateLimitLamp(lampXLimitPlus, PLCAddresses.Input.P1_X_LimitPlus);

                        UpdateLimitLamp(lampYLimitMinus, PLCAddresses.Input.P1_Y_LimitMinus);
                        UpdateLimitLamp(lampYHome, PLCAddresses.Input.P1_Y_Home);
                        UpdateLimitLamp(lampYLimitPlus, PLCAddresses.Input.P1_Y_LimitPlus);

                        UpdateLimitLamp(lampZLimitMinus, PLCAddresses.Input.P1_Z_LimitMinus);
                        UpdateLimitLamp(lampZHome, PLCAddresses.Input.P1_Z_Home);
                        UpdateLimitLamp(lampZLimitPlus, PLCAddresses.Input.P1_Z_LimitPlus);

                        UpdateLimitLamp(lampFLimitMinus, PLCAddresses.Input.P1_F_LimitMinus);
                        UpdateLimitLamp(lampFHome, PLCAddresses.Input.P1_F_Home);
                        UpdateLimitLamp(lampFLimitPlus, PLCAddresses.Input.P1_F_LimitPlus);
                        break;

                    case 2:
                        UpdateLimitLamp(lampXLimitMinus, PLCAddresses.Input.P2_X_LimitMinus);
                        UpdateLimitLamp(lampXHome, PLCAddresses.Input.P2_X_Home);
                        UpdateLimitLamp(lampXLimitPlus, PLCAddresses.Input.P2_X_LimitPlus);

                        UpdateLimitLamp(lampYLimitMinus, PLCAddresses.Input.P2_Y_LimitMinus);
                        UpdateLimitLamp(lampYHome, PLCAddresses.Input.P2_Y_Home);
                        UpdateLimitLamp(lampYLimitPlus, PLCAddresses.Input.P2_Y_LimitPlus);

                        UpdateLimitLamp(lampZLimitMinus, PLCAddresses.Input.P2_Z_LimitMinus);
                        UpdateLimitLamp(lampZHome, PLCAddresses.Input.P2_Z_Home);
                        UpdateLimitLamp(lampZLimitPlus, PLCAddresses.Input.P2_Z_LimitPlus);

                        UpdateLimitLamp(lampFLimitMinus, PLCAddresses.Input.P2_F_LimitMinus);
                        UpdateLimitLamp(lampFHome, PLCAddresses.Input.P2_F_Home);
                        UpdateLimitLamp(lampFLimitPlus, PLCAddresses.Input.P2_F_LimitPlus);
                        break;

                    case 3:
                        UpdateLimitLamp(lampXLimitMinus, PLCAddresses.Input.P3_X_LimitMinus);
                        UpdateLimitLamp(lampXHome, PLCAddresses.Input.P3_X_Home);
                        UpdateLimitLamp(lampXLimitPlus, PLCAddresses.Input.P3_X_LimitPlus);

                        UpdateLimitLamp(lampYLimitMinus, PLCAddresses.Input.P3_Y_LimitMinus);
                        UpdateLimitLamp(lampYHome, PLCAddresses.Input.P3_Y_Home);
                        UpdateLimitLamp(lampYLimitPlus, PLCAddresses.Input.P3_Y_LimitPlus);

                        UpdateLimitLamp(lampZLimitMinus, PLCAddresses.Input.P3_Z_LimitMinus);
                        UpdateLimitLamp(lampZHome, PLCAddresses.Input.P3_Z_Home);
                        UpdateLimitLamp(lampZLimitPlus, PLCAddresses.Input.P3_Z_LimitPlus);

                        UpdateLimitLamp(lampFLimitMinus, PLCAddresses.Input.P3_F_LimitMinus);
                        UpdateLimitLamp(lampFHome, PLCAddresses.Input.P3_F_Home);
                        UpdateLimitLamp(lampFLimitPlus, PLCAddresses.Input.P3_F_LimitPlus);
                        break;

                    case 4:
                        UpdateLimitLamp(lampXLimitMinus, PLCAddresses.Input.P4_X_LimitMinus);
                        UpdateLimitLamp(lampXHome, PLCAddresses.Input.P4_X_Home);
                        UpdateLimitLamp(lampXLimitPlus, PLCAddresses.Input.P4_X_LimitPlus);

                        UpdateLimitLamp(lampYLimitMinus, PLCAddresses.Input.P4_Y_LimitMinus);
                        UpdateLimitLamp(lampYHome, PLCAddresses.Input.P4_Y_Home);
                        UpdateLimitLamp(lampYLimitPlus, PLCAddresses.Input.P4_Y_LimitPlus);

                        UpdateLimitLamp(lampZLimitMinus, PLCAddresses.Input.P4_Z_LimitMinus);
                        UpdateLimitLamp(lampZHome, PLCAddresses.Input.P4_Z_Home);
                        UpdateLimitLamp(lampZLimitPlus, PLCAddresses.Input.P4_Z_LimitPlus);

                        UpdateLimitLamp(lampFLimitMinus, PLCAddresses.Input.P4_F_LimitMinus);
                        UpdateLimitLamp(lampFHome, PLCAddresses.Input.P4_F_Home);
                        UpdateLimitLamp(lampFLimitPlus, PLCAddresses.Input.P4_F_LimitPlus);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Handle errors silently
            }
        }

        /// <summary>
        /// Update individual limit switch lamp color based on bit status
        /// Green = Active (bit = 1), Gray = Inactive (bit = 0)
        /// </summary>
        private void UpdateLimitLamp(System.Windows.Forms.Button lamp, string address)
        {
            try
            {
                var addrSplit = address.Split('.');
                bool isActive = PLCKey.ReadBitFromWord(addrSplit[0], GetBitIndexFromAddress(address));

                if (isActive)
                {
                    lamp.BackColor = Color.LimeGreen;  // Active - bright green
                    lamp.ForeColor = Color.Black;
                }
                else
                {
                    lamp.BackColor = Color.Gray;  // Inactive - gray
                    lamp.ForeColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                // On error, set to gray
                lamp.BackColor = Color.Gray;
                lamp.ForeColor = Color.White;
            }
        }

        /// <summary>
        /// Extract bit index from address like "DM388.00" -> 0, "DM388.01" -> 1
        /// </summary>
        private int GetBitIndexFromAddress(string address)
        {
            if (address.Contains("."))
            {
                string[] parts = address.Split('.');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[1], out int bitIndex))
                    {
                        return bitIndex;
                    }
                }
            }
            return 0;
        }

        // Button event handlers for Org (Home) buttons
        private void btnAxisOrg_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            if (btn == null) return;

            string axis = "";
            if (btn == btnXOrg) axis = "X";
            else if (btn == btnYOrg) axis = "Y";
            else if (btn == btnZOrg) axis = "Z";
            else if (btn == btnRIOrg) axis = "RI";
            else if (btn == btnROOrg) axis = "RO";
            else if (btn == btnFOrg) axis = "F";
            else return;

            string homeAddr = GetHomeAddress(selectedPort, axis);
            if (!string.IsNullOrEmpty(homeAddr))
            {
                PLCKey.SetBit(homeAddr);
            }
        }

        #endregion

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Trạng thái connect camera
            if (cameraClient12 != null && cameraClient12.IsConnected)
            {
                btnConnect12.Text = "Disconnect";
                btnConnect12.BackColor = Color.LightGreen;
            }
            else 
            {
                btnConnect12.Text = "Connect";
                btnConnect12.BackColor = Color.LightCoral;
            }
            if (cameraClient34 != null && cameraClient34.IsConnected)
            {
                btnConnect34.Text = "Disconnect";
                btnConnect34.BackColor = Color.LightGreen;
            }
            else 
            {
                btnConnect34.Text = "Connect";
                btnConnect34.BackColor = Color.LightCoral;
            }
            // Update trạng thái motion

        }

        #region IO Tab - Input/Output Status Update

        // IO Port radio button selection changed
       

        // Handle Teaching mode radio button changes with password protection
        private void TeachingModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                if (rb == rbtJogMode)
                {
                    // Disable teaching groups when switching to Jog mode
                    grpTeachingSocket.Enabled = false;
                    grpTeachingTray.Enabled = false;

                    // Hide model management buttons when switching to Jog mode
                    btnModelAdd.Enabled = false;
                    btnModelDelete.Enabled = false;
                    btnModelLoad.Enabled = false;

                    // Reset all save button colors when switching to Jog mode
                    ResetTeachingSaveButtonColors();
                }
                else if (rb == rbtTeachingMode)
                {
                    // Check if port is in Auto Mode
                    if (IsAutoModeActive(selectedPort))
                    {
                        MessageBox.Show(
                            $"Port {selectedPort} đang ở chế độ Auto!\nKhông thể chuyển sang Teaching Mode khi Auto đang chạy.",
                            "Auto Mode Active",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        rbtJogMode.Checked = true; // Revert to Jog mode
                        return;
                    }

                    // Show password dialog when switching to Teaching mode
                    string password = PasswordDialog.Show(
                        "Nhập mật khẩu để vào chế độ Teaching:",
                        "Teaching Mode Password",
                        this);

                    if (password == TEACHING_PASSWORD)
                    {
                        // Correct password - enable teaching groups
                        grpTeachingSocket.Enabled = true;
                        grpTeachingTray.Enabled = true;

                        // Show model management buttons when in Teaching mode
                        btnModelAdd.Enabled = true;
                        btnModelDelete.Enabled = true;
                        btnModelLoad.Enabled = true;
                    }
                    else if (password != null)  // User didn't cancel
                    {
                        // Incorrect password - show error and revert to Jog mode
                        MessageBox.Show("Mật khẩu không đúng!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        rbtJogMode.Checked = true; // Revert to Jog mode
                    }
                    else  // User cancelled
                    {
                        // Revert to Jog mode without showing error
                        rbtJogMode.Checked = true;
                    }
                }
            }
        }

        // Update input sensor status (LED buttons)
        private void UpdateIOInputs()
        {
            try
            {
                switch (selectedIOPort)
                {
                    case 1:
                        UpdateLEDStatus(ledVacLoad, PLCAddresses.Input.P1_Ss_VIn1);
                        UpdateLEDStatus(ledVacUnload, PLCAddresses.Input.P1_Ss_VOt1);
                        UpdateLEDStatus(ledSocketOpen, PLCAddresses.Input.P1_Ss_Fix1_Open);
                        UpdateLEDStatus(ledSocketClose, PLCAddresses.Input.P1_Ss_Fix1_Close);
                        UpdateLEDStatus(ledLCAStart, PLCAddresses.Input.P1_Stt_lca_Start1);
                        UpdateLEDStatus(ledLCAStop, PLCAddresses.Input.P1_Stt_lca_Stop1);
                        UpdateLEDStatus(ledDoor, PLCAddresses.Input.P1_Ss_Door1);
                        UpdateLEDStatus(ledTrayLoadJig, PLCAddresses.Input.P1_Ss_Jig_OK);
                        UpdateLEDStatus(ledTrayLoad, PLCAddresses.Input.P1_Ss_Tray_OK);
                        UpdateLEDStatus(ledTrayNG1Jig, PLCAddresses.Input.P1_Ss_Jig_NG);
                        UpdateLEDStatus(ledTrayNG1, PLCAddresses.Input.P1_Ss_Tray_NG);
                        UpdateLEDStatus(ledTrayNG2Jig, PLCAddresses.Input.P1_Ss_Jig_NG4);
                        UpdateLEDStatus(ledTrayNG2, PLCAddresses.Input.P1_Ss_Tray_NG4);
                        UpdateLEDStatus(ledEMG, PLCAddresses.Input.P1_SW_EMS1);
                        UpdateLEDStatus(ledStart, PLCAddresses.Input.P1_SW_Start1);
                        UpdateLEDStatus(ledStop, PLCAddresses.Input.P1_SW_Stop1);
                        UpdateLEDStatus(ledReset, PLCAddresses.Input.P1_SW_Reset1);
                        UpdateLEDStatus(ledInit, PLCAddresses.Input.P1_SW_Init1);
                        UpdateLEDStatus(ledLCALampGreen, PLCAddresses.Input.P1_Stt_lca_Gre1);
                        UpdateLEDStatus(ledLCALampYellow, PLCAddresses.Input.P1_Stt_lca_Yel1 );
                        UpdateLEDStatus(ledLCALampRed, PLCAddresses.Input.P1_Stt_lca_Red1 );
                        UpdateLEDStatus(ledChartIn, PLCAddresses.Input.P1_Chart_Off);
                        UpdateLEDStatus(ledChartOut, PLCAddresses.Input.P1_Chart_On);
                        UpdateLEDStatus(ledVacIn, PLCAddresses.Input.P1_Ss_AirPlus1);
                        break;

                    case 2:
                        UpdateLEDStatus(ledVacLoad, PLCAddresses.Input.P2_Ss_VIn2);
                        UpdateLEDStatus(ledVacUnload, PLCAddresses.Input.P2_Ss_VOt2);
                        UpdateLEDStatus(ledSocketOpen, PLCAddresses.Input.P2_Ss_Fix2_Open);
                        UpdateLEDStatus(ledSocketClose, PLCAddresses.Input.P2_Ss_Fix2_Close);
                        UpdateLEDStatus(ledLCAStart, PLCAddresses.Input.P2_Stt_lca_Start2_StatusTester);
                        UpdateLEDStatus(ledLCAStop, PLCAddresses.Input.P2_Stt_lca_Stop2_StatusTester);
                        UpdateLEDStatus(ledDoor, PLCAddresses.Input.P2_Ss_Door2);
                        UpdateLEDStatus(ledTrayLoadJig, PLCAddresses.Input.P2_Ss_Jig_OK);
                        UpdateLEDStatus(ledTrayLoad, PLCAddresses.Input.P2_Ss_Tray_OK);
                        UpdateLEDStatus(ledTrayNG1Jig, PLCAddresses.Input.P2_Ss_Jig_NG);
                        UpdateLEDStatus(ledTrayNG1, PLCAddresses.Input.P2_Ss_Tray_NG);
                        UpdateLEDStatus(ledTrayNG2Jig, PLCAddresses.Input.P2_Ss_Jig_NG4);
                        UpdateLEDStatus(ledTrayNG2, PLCAddresses.Input.P2_Ss_Tray_NG4);
                        UpdateLEDStatus(ledEMG, PLCAddresses.Input.P2_SW_EMS2);
                        UpdateLEDStatus(ledStart, PLCAddresses.Input.P2_SW_Start2);
                        UpdateLEDStatus(ledStop, PLCAddresses.Input.P2_SW_Stop2);
                        UpdateLEDStatus(ledReset, PLCAddresses.Input.P2_SW_Reset2);
                        UpdateLEDStatus(ledInit, PLCAddresses.Input.P2_SW_Init2);
                        UpdateLEDStatus(ledLCALampGreen, PLCAddresses.Input.P2_Stt_lca_Gre2);
                        UpdateLEDStatus(ledLCALampYellow, PLCAddresses.Input.P2_Stt_lca_Yel2);
                        UpdateLEDStatus(ledLCALampRed, PLCAddresses.Input.P2_Stt_lca_Red2);
                        UpdateLEDStatus(ledChartIn, PLCAddresses.Input.P2_Chart_Off);
                        UpdateLEDStatus(ledChartOut, PLCAddresses.Input.P2_Chart_On);
                        UpdateLEDStatus(ledVacIn, PLCAddresses.Input.P2_Ss_AirPlus2);
                        break;

                    case 3:
                        UpdateLEDStatus(ledVacLoad, PLCAddresses.Input.P3_Ss_VIn3);
                        UpdateLEDStatus(ledVacUnload, PLCAddresses.Input.P3_Ss_VOt3);
                        UpdateLEDStatus(ledSocketOpen, PLCAddresses.Input.P3_Ss_Fix3_Open);
                        UpdateLEDStatus(ledSocketClose, PLCAddresses.Input.P3_Ss_Fix3_Close);
                        UpdateLEDStatus(ledLCAStart, PLCAddresses.Input.P3_Stt_lca_Start3);
                        UpdateLEDStatus(ledLCAStop, PLCAddresses.Input.P3_Stt_lca_Stop3);
                        UpdateLEDStatus(ledDoor, PLCAddresses.Input.P3_Ss_Door3);
                        UpdateLEDStatus(ledTrayLoadJig, PLCAddresses.Input.P3_Ss_Jig_OK);
                        UpdateLEDStatus(ledTrayLoad, PLCAddresses.Input.P3_Ss_Tray_OK);
                        UpdateLEDStatus(ledTrayNG1Jig, PLCAddresses.Input.P3_Ss_Jig_NG);
                        UpdateLEDStatus(ledTrayNG1, PLCAddresses.Input.P3_Ss_Tray_NG);
                        UpdateLEDStatus(ledTrayNG2Jig, PLCAddresses.Input.P3_Ss_Jig_NG4);
                        UpdateLEDStatus(ledTrayNG2, PLCAddresses.Input.P3_Ss_Tray_NG4);
                        UpdateLEDStatus(ledEMG, PLCAddresses.Input.P3_SW_EMS3);
                        UpdateLEDStatus(ledStart, PLCAddresses.Input.P3_SW_Start3);
                        UpdateLEDStatus(ledStop, PLCAddresses.Input.P3_SW_Stop3);
                        UpdateLEDStatus(ledReset, PLCAddresses.Input.P3_SW_Reset3);
                        UpdateLEDStatus(ledInit, PLCAddresses.Input.P3_SW_Init3);
                        UpdateLEDStatus(ledLCALampGreen, PLCAddresses.Input.P3_Stt_lca_Gre3);
                        UpdateLEDStatus(ledLCALampYellow, PLCAddresses.Input.P3_Stt_lca_Yel3);
                        UpdateLEDStatus(ledLCALampRed, PLCAddresses.Input.P3_Stt_lca_Red3);
                        UpdateLEDStatus(ledChartIn, PLCAddresses.Input.P3_Chart_Off);
                        UpdateLEDStatus(ledChartOut, PLCAddresses.Input.P3_Chart_On);
                        UpdateLEDStatus(ledVacIn, PLCAddresses.Input.P3_Ss_AirPlus3);
                        break;

                    case 4:
                        UpdateLEDStatus(ledVacLoad, PLCAddresses.Input.P4_Ss_VIn4);
                        UpdateLEDStatus(ledVacUnload, PLCAddresses.Input.P4_Ss_VOt24);
                        UpdateLEDStatus(ledSocketOpen, PLCAddresses.Input.P4_Ss_Fix4_Open);
                        UpdateLEDStatus(ledSocketClose, PLCAddresses.Input.P4_Ss_Fix4_Close);
                        UpdateLEDStatus(ledLCAStart, PLCAddresses.Input.P4_Stt_lca_Start4);
                        UpdateLEDStatus(ledLCAStop, PLCAddresses.Input.P4_Stt_lca_Stop4);
                        UpdateLEDStatus(ledDoor, PLCAddresses.Input.P4_Ss_Door4);
                        UpdateLEDStatus(ledTrayLoadJig, PLCAddresses.Input.P4_Ss_Jig_OK);
                        UpdateLEDStatus(ledTrayLoad, PLCAddresses.Input.P4_Ss_Tray_OK);
                        UpdateLEDStatus(ledTrayNG1Jig, PLCAddresses.Input.P4_Ss_Jig_NG);
                        UpdateLEDStatus(ledTrayNG1, PLCAddresses.Input.P4_Ss_Tray_NG);
                        UpdateLEDStatus(ledTrayNG2Jig, PLCAddresses.Input.P4_Ss_Jig_NG4);
                        UpdateLEDStatus(ledTrayNG2, PLCAddresses.Input.P4_Ss_Tray_NG4);
                        UpdateLEDStatus(ledEMG, PLCAddresses.Input.P4_SW_EMS4);
                        UpdateLEDStatus(ledStart, PLCAddresses.Input.P4_SW_Start4);
                        UpdateLEDStatus(ledStop, PLCAddresses.Input.P4_SW_Stop4);
                        UpdateLEDStatus(ledReset, PLCAddresses.Input.P4_SW_Reset4);
                        UpdateLEDStatus(ledInit, PLCAddresses.Input.P4_SW_Init4);
                        UpdateLEDStatus(ledLCALampGreen, PLCAddresses.Input.P4_Stt_lca_Gre4);
                        UpdateLEDStatus(ledLCALampYellow, PLCAddresses.Input.P4_Stt_lca_Yel4);
                        UpdateLEDStatus(ledLCALampRed, PLCAddresses.Input.P4_Stt_lca_Red4);
                        UpdateLEDStatus(ledChartIn, PLCAddresses.Input.P4_Chart_Off);
                        UpdateLEDStatus(ledChartOut, PLCAddresses.Input.P4_Chart_On);
                        UpdateLEDStatus(ledVacIn, PLCAddresses.Input.P4_Ss_AirPlus4); 
                        break;
                }
            }
            catch (Exception ex)
            {
                // Handle errors silently to avoid disrupting UI
            }
        }

        // Update LED button color based on PLC bit status
        // Green if ON, Red if OFF
        private void UpdateLEDStatus(System.Windows.Forms.Button ledButton, string plcAddress)
        {
            if (ledButton == null || string.IsNullOrEmpty(plcAddress))
                return;

            bool bitStatus = PLCKey.ReadBit(plcAddress);
            ledButton.BackColor = bitStatus ? Color.Green : Color.Gray;
        }

        // Update output button status
        private void UpdateIOOutputs()
        {
            try
            {
                switch (selectedIOPort)
                {
                    case 1:
                        // Port 1 outputs - Tower lights and controls
                        UpdateOutputButtonStatus(btnVacLoad, PLCAddresses.Output.P1_Cylinder_Vaccum_Load);
                        UpdateOutputButtonStatus(btnVacUnload, PLCAddresses.Output.P1_Cylinder_Vaccum_Unload);
                        UpdateOutputButtonStatus(btnReqCamera, PLCAddresses.Output.P12_Cam_cylinder);
                        UpdateOutputButtonStatus(btnCylinderSocket, PLCAddresses.Output.P1_Cylinder_Fix_Socket);
                        UpdateOutputButtonStatus(btnLampInit, PLCAddresses.Output.P1_LCA_Request_Init);
                        UpdateOutputButtonStatus(btnLampStart, PLCAddresses.Output.P1_LCA_Request_Start);
                        UpdateOutputButtonStatus(btnLampStop, PLCAddresses.Output.P1_LCA_Request_Stop);
                        UpdateOutputButtonStatus(btnLampRestart, PLCAddresses.Output.P1_LCA_Request_Reset);
                        UpdateOutputButtonStatus(btnCylinderChart, PLCAddresses.Output.P1_Cylinder_Chart_Socket);
                        break;

                    case 2:
                        // Port 2 outputs
                        UpdateOutputButtonStatus(btnVacLoad, PLCAddresses.Output.P2_Cylinder_Vaccum_Load);
                        UpdateOutputButtonStatus(btnVacUnload, PLCAddresses.Output.P2_Cylinder_Vaccum_Unload);
                        UpdateOutputButtonStatus(btnReqCamera, PLCAddresses.Output.P12_Cam_cylinder);
                        UpdateOutputButtonStatus(btnCylinderSocket, PLCAddresses.Output.P2_Cylinder_Fix_Socket);
                        UpdateOutputButtonStatus(btnLampInit, PLCAddresses.Output.P2_LCA_Request_Init);
                        UpdateOutputButtonStatus(btnLampStart, PLCAddresses.Output.P2_LCA_Request_Start);
                        UpdateOutputButtonStatus(btnLampStop, PLCAddresses.Output.P2_LCA_Request_Stop);
                        UpdateOutputButtonStatus(btnLampRestart, PLCAddresses.Output.P2_LCA_Request_Reset);
                        UpdateOutputButtonStatus(btnCylinderChart, PLCAddresses.Output.P2_Cylinder_Chart_Socket);
                        break;

                    case 3:
                        // Port 3 outputs
                        UpdateOutputButtonStatus(btnVacLoad, PLCAddresses.Output.P3_Cylinder_Vaccum_Load);
                        UpdateOutputButtonStatus(btnVacUnload, PLCAddresses.Output.P3_Cylinder_Vaccum_Unload);
                        UpdateOutputButtonStatus(btnReqCamera, PLCAddresses.Output.P12_Cam_cylinder);
                        UpdateOutputButtonStatus(btnCylinderSocket, PLCAddresses.Output.P3_Cylinder_Fix_Socket);
                        UpdateOutputButtonStatus(btnLampInit, PLCAddresses.Output.P3_LCA_Request_Init);
                        UpdateOutputButtonStatus(btnLampStart, PLCAddresses.Output.P3_LCA_Request_Start);
                        UpdateOutputButtonStatus(btnLampStop, PLCAddresses.Output.P3_LCA_Request_Stop);
                        UpdateOutputButtonStatus(btnLampRestart, PLCAddresses.Output.P3_LCA_Request_Reset);
                        UpdateOutputButtonStatus(btnCylinderChart, PLCAddresses.Output.P3_Cylinder_Chart_Socket);
                        break;

                    case 4:
                        // Port 4 outputs
                        UpdateOutputButtonStatus(btnVacLoad, PLCAddresses.Output.P4_Cylinder_Vaccum_Load);
                        UpdateOutputButtonStatus(btnVacUnload, PLCAddresses.Output.P4_Cylinder_Vaccum_Unload);
                        UpdateOutputButtonStatus(btnReqCamera, PLCAddresses.Output.P12_Cam_cylinder);
                        UpdateOutputButtonStatus(btnCylinderSocket, PLCAddresses.Output.P4_Cylinder_Fix_Socket);
                        UpdateOutputButtonStatus(btnLampInit, PLCAddresses.Output.P4_LCA_Request_Init);
                        UpdateOutputButtonStatus(btnLampStart, PLCAddresses.Output.P4_LCA_Request_Start);
                        UpdateOutputButtonStatus(btnLampStop, PLCAddresses.Output.P4_LCA_Request_Stop);
                        UpdateOutputButtonStatus(btnLampRestart, PLCAddresses.Output.P4_LCA_Request_Reset);
                        UpdateOutputButtonStatus(btnCylinderChart, PLCAddresses.Output.P4_Cylinder_Chart_Socket);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Handle errors silently
            }
        }

        // Update output button color based on PLC bit status
        // Red if ON, Green if OFF
        private void UpdateOutputButtonStatus(System.Windows.Forms.Button outputButton, string plcAddress)
        {
            if (outputButton == null || string.IsNullOrEmpty(plcAddress))
                return;

            bool bitStatus = PLCKey.ReadBit(plcAddress);
            outputButton.BackColor = bitStatus ? Color.Green : Color.Gray;
        }

        // Output button click handler to toggle PLC bits
        private void btnOutputToggle_Click(object sender, EventArgs e)
        {
            // Check if port is in Auto Mode
            if (IsAutoModeActive(selectedIOPort))
            {
                MessageBox.Show(
                    $"Port {selectedIOPort} đang ở chế độ Auto!\nKhông thể điều khiển Output khi Auto đang chạy.",
                    "Auto Mode Active",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            if (btn == null) return;

            try
            {
                string plcAddress = GetOutputAddress(btn);
                if (!string.IsNullOrEmpty(plcAddress))
                {
                    // Toggle the bit
                    bool currentStatus = PLCKey.ReadBit(plcAddress);
                    if (currentStatus)
                        PLCKey.ResetBit(plcAddress);
                    else
                        PLCKey.SetBit(plcAddress);

                    // Immediately update the button color
                    UpdateOutputButtonStatus(btn, plcAddress);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling output: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Get PLC address for output button based on selected port
        private string GetOutputAddress(System.Windows.Forms.Button btn)
        {
            // Port-specific outputs
            switch (selectedIOPort)
            {
                case 1:
                    if (btn == btnVacLoad) return PLCAddresses.Output.P1_Cylinder_Vaccum_Load ;
                    if (btn == btnVacUnload) return PLCAddresses.Output.P1_Cylinder_Vaccum_Unload ;
                    if (btn == btnReqCamera) return PLCAddresses.Output.P12_Cam_cylinder ;
                    if (btn == btnCylinderSocket) return PLCAddresses.Output.P1_Cylinder_Fix_Socket ;
                    if (btn == btnLampInit) return PLCAddresses.Output.P1_LCA_Request_Init ;
                    if (btn == btnLampStart) return PLCAddresses.Output.P1_LCA_Request_Start ;
                    if (btn == btnLampStop) return PLCAddresses.Output.P1_LCA_Request_Stop ;
                    if (btn == btnLampRestart) return PLCAddresses.Output.P1_LCA_Request_Reset ;
                    if (btn == btnCylinderChart) return PLCAddresses.Output.P1_Cylinder_Chart_Socket;
                    break;

                case 2:
                    if (btn == btnVacLoad) return PLCAddresses.Output.P2_Cylinder_Vaccum_Load;
                    if (btn == btnVacUnload) return PLCAddresses.Output.P2_Cylinder_Vaccum_Unload;
                    if (btn == btnReqCamera) return PLCAddresses.Output.P12_Cam_cylinder;
                    if (btn == btnCylinderSocket) return PLCAddresses.Output.P2_Cylinder_Fix_Socket;
                    if (btn == btnLampInit) return PLCAddresses.Output.P2_LCA_Request_Init;
                    if (btn == btnLampStart) return PLCAddresses.Output.P2_LCA_Request_Start;
                    if (btn == btnLampStop) return PLCAddresses.Output.P2_LCA_Request_Stop;
                    if (btn == btnLampRestart) return PLCAddresses.Output.P2_LCA_Request_Reset;
                    if (btn == btnCylinderChart) return PLCAddresses.Output.P2_Cylinder_Chart_Socket;
                    break;

                case 3:
                    if (btn == btnVacLoad) return PLCAddresses.Output.P3_Cylinder_Vaccum_Load;
                    if (btn == btnVacUnload) return PLCAddresses.Output.P3_Cylinder_Vaccum_Unload;
                    if (btn == btnReqCamera) return PLCAddresses.Output.P12_Cam_cylinder;
                    if (btn == btnCylinderSocket) return PLCAddresses.Output.P3_Cylinder_Fix_Socket;
                    if (btn == btnLampInit) return PLCAddresses.Output.P3_LCA_Request_Init;
                    if (btn == btnLampStart) return PLCAddresses.Output.P3_LCA_Request_Start;
                    if (btn == btnLampStop) return PLCAddresses.Output.P3_LCA_Request_Stop;
                    if (btn == btnLampRestart) return PLCAddresses.Output.P3_LCA_Request_Reset;
                    if (btn == btnCylinderChart) return PLCAddresses.Output.P3_Cylinder_Chart_Socket;
                    break;

                case 4:
                    if (btn == btnVacLoad) return PLCAddresses.Output.P4_Cylinder_Vaccum_Load;
                    if (btn == btnVacUnload) return PLCAddresses.Output.P4_Cylinder_Vaccum_Unload;
                    if (btn == btnReqCamera) return PLCAddresses.Output.P12_Cam_cylinder;
                    if (btn == btnCylinderSocket) return PLCAddresses.Output.P4_Cylinder_Fix_Socket;
                    if (btn == btnLampInit) return PLCAddresses.Output.P4_LCA_Request_Init;
                    if (btn == btnLampStart) return PLCAddresses.Output.P4_LCA_Request_Start;
                    if (btn == btnLampStop) return PLCAddresses.Output.P4_LCA_Request_Stop;
                    if (btn == btnLampRestart) return PLCAddresses.Output.P4_LCA_Request_Reset;
                    if (btn == btnCylinderChart) return PLCAddresses.Output.P4_Cylinder_Chart_Socket;
                    break;
            }

            return null;
        }

        #endregion

        #region Motion Tab - Speed/Step/Reset/Go Functions

        // Load current Speed and Step values from PLC for all axes
        private void LoadAxisSpeedAndStep()
        {
            try
            {
                switch (selectedPort)
                {
                    case 1:
                        // Load Speed values (divide by 100 and format with 2 decimals)
                        txtXSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_Speed_HJog_X) / 100.0).ToString("F2");
                        txtYSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_Speed_HJog_Y) / 100.0).ToString("F2");
                        txtZSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_Speed_HJog_Z) / 100.0).ToString("F2");
                        txtRISpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_Speed_HJog_RI) / 100.0).ToString("F2");
                        txtROSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_Speed_HJog_RO) / 100.0).ToString("F2");
                        txtFSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_Speed_HJog_F) / 100.0).ToString("F2");

                        // Load Step values (divide by 100 and format with 2 decimals)
                        txtXStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_DisStep_JogPlus_X) / 100.0).ToString("F2");
                        txtYStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_DisStep_JogPlus_Y) / 100.0).ToString("F2");
                        txtZStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_DisStep_JogPlus_Z) / 100.0).ToString("F2");
                        txtRIStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_DisStep_JogPlus_RI) / 100.0).ToString("F2");
                        txtROStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_DisStep_JogPlus_RO) / 100.0).ToString("F2");
                        txtFStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_DisStep_JogPlus_F) / 100.0).ToString("F2");
                        break;

                    case 2:
                        // Load Speed values (divide by 100 and format with 2 decimals)
                        txtXSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_Speed_HJog_X) / 100.0).ToString("F2");
                        txtYSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_Speed_HJog_Y) / 100.0).ToString("F2");
                        txtZSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_Speed_HJog_Z) / 100.0).ToString("F2");
                        txtRISpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_Speed_HJog_RI) / 100.0).ToString("F2");
                        txtROSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_Speed_HJog_RO) / 100.0).ToString("F2");
                        txtFSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_Speed_HJog_F) / 100.0).ToString("F2");

                        // Load Step values (divide by 100 and format with 2 decimals)
                        txtXStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_DisStep_JogPlus_X) / 100.0).ToString("F2");
                        txtYStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_DisStep_JogPlus_Y) / 100.0).ToString("F2");
                        txtZStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_DisStep_JogPlus_Z) / 100.0).ToString("F2");
                        txtRIStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_DisStep_JogPlus_RI) / 100.0).ToString("F2");
                        txtROStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_DisStep_JogPlus_RO) / 100.0).ToString("F2");
                        txtFStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P2_DisStep_JogPlus_F) / 100.0).ToString("F2");
                        break;

                    case 3:
                        // Load Speed values (divide by 100 and format with 2 decimals)
                        txtXSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_Speed_HJog_X) / 100.0).ToString("F2");
                        txtYSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_Speed_HJog_Y) / 100.0).ToString("F2");
                        txtZSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_Speed_HJog_Z) / 100.0).ToString("F2");
                        txtRISpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_Speed_HJog_RI) / 100.0).ToString("F2");
                        txtROSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_Speed_HJog_RO) / 100.0).ToString("F2");
                        txtFSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_Speed_HJog_F) / 100.0).ToString("F2");

                        // Load Step values (divide by 100 and format with 2 decimals)
                        txtXStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_DisStep_JogPlus_X) / 100.0).ToString("F2");
                        txtYStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_DisStep_JogPlus_Y) / 100.0).ToString("F2");
                        txtZStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_DisStep_JogPlus_Z) / 100.0).ToString("F2");
                        txtRIStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_DisStep_JogPlus_RI) / 100.0).ToString("F2");
                        txtROStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_DisStep_JogPlus_RO) / 100.0).ToString("F2");
                        txtFStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P3_DisStep_JogPlus_F) / 100.0).ToString("F2");
                        break;

                    case 4:
                        // Load Speed values (divide by 100 and format with 2 decimals)
                        txtXSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_Speed_HJog_X) / 100.0).ToString("F2");
                        txtYSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_Speed_HJog_Y) / 100.0).ToString("F2");
                        txtZSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_Speed_HJog_Z) / 100.0).ToString("F2");
                        txtRISpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_Speed_HJog_RI) / 100.0).ToString("F2");
                        txtROSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_Speed_HJog_RO) / 100.0).ToString("F2");
                        txtFSpeed.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_Speed_HJog_F) / 100.0).ToString("F2");

                        // Load Step values (divide by 100 and format with 2 decimals)
                        txtXStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_DisStep_JogPlus_X) / 100.0).ToString("F2");
                        txtYStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_DisStep_JogPlus_Y) / 100.0).ToString("F2");
                        txtZStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_DisStep_JogPlus_Z) / 100.0).ToString("F2");
                        txtRIStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_DisStep_JogPlus_RI) / 100.0).ToString("F2");
                        txtROStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_DisStep_JogPlus_RO) / 100.0).ToString("F2");
                        txtFStep.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P4_DisStep_JogPlus_F) / 100.0).ToString("F2");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Handle errors silently to avoid disrupting UI
            }
        }

        // Save Speed and Step values to PLC when Save button is clicked
        //private void btnAxisSave_Click(object sender, EventArgs e)
        //{
        //    System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
        //    if (btn == null) return;

        //    try
        //    {
        //        string axis = GetAxisFromSaveButton(btn);
        //        if (string.IsNullOrEmpty(axis)) return;

        //        // Get the addresses for speed and step based on port and axis
        //        string speedAddr = GetSpeedAddress(selectedPort, axis);
        //        string stepAddr = GetStepAddress(selectedPort, axis);

        //        if (string.IsNullOrEmpty(speedAddr) || string.IsNullOrEmpty(stepAddr))
        //            return;

        //        // Get the textbox values
        //        int speedValue = 0;
        //        int stepValue = 0;

        //        switch (axis)
        //        {
        //            case "X":
        //                int.TryParse(txtXSpeed.Text, out speedValue);
        //                int.TryParse(txtXStep.Text, out stepValue);
        //                break;
        //            case "Y":
        //                int.TryParse(txtYSpeed.Text, out speedValue);
        //                int.TryParse(txtYStep.Text, out stepValue);
        //                break;
        //            case "Z":
        //                int.TryParse(txtZSpeed.Text, out speedValue);
        //                int.TryParse(txtZStep.Text, out stepValue);
        //                break;
        //            case "RI":
        //                int.TryParse(txtRISpeed.Text, out speedValue);
        //                int.TryParse(txtRIStep.Text, out stepValue);
        //                break;
        //            case "RO":
        //                int.TryParse(txtROSpeed.Text, out speedValue);
        //                int.TryParse(txtROStep.Text, out stepValue);
        //                break;
        //            case "F":
        //                int.TryParse(txtFSpeed.Text, out speedValue);
        //                int.TryParse(txtFStep.Text, out stepValue);
        //                break;
        //        }

        //        // Write to PLC
        //        PLCKey.WriteInt32(speedAddr, speedValue);
        //        PLCKey.WriteInt32(stepAddr, stepValue);

        //        MessageBox.Show($"Saved Speed={speedValue} and Step={stepValue} for {axis} axis",
        //            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error saving values: {ex.Message}",
        //            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        // Reset axis when Reset button is clicked
        private void btnAxisReset_Click(object sender, EventArgs e)
        {
            // Check if port is in Auto Mode
            if (IsAutoModeActive(selectedPort))
            {
                MessageBox.Show(
                    $"Port {selectedPort} đang ở chế độ Auto!\nKhông thể Reset khi Auto đang chạy.",
                    "Auto Mode Active",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            if (btn == null) return;

            try
            {
                string axis = GetAxisFromResetButton(btn);
                if (string.IsNullOrEmpty(axis)) return;

                // Get the reset bit address based on port and axis
                string resetAddr = GetResetAddress(selectedPort, axis);

                if (string.IsNullOrEmpty(resetAddr))
                    return;

                // Set reset bit
                PLCKey.SetBit(resetAddr);

                // Wait a short time then reset the bit
                System.Threading.Thread.Sleep(100);
                PLCKey.ResetBit(resetAddr);

                MessageBox.Show($"Reset {axis} axis for Port {selectedPort}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting axis: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Go to ABS position when Go button is clicked
        private void btnAxisGo_Click(object sender, EventArgs e)
        {
            // Check if port is in Auto Mode
            if (IsAutoModeActive(selectedPort))
            {
                MessageBox.Show(
                    $"Port {selectedPort} đang ở chế độ Auto!\nKhông thể Go khi Auto đang chạy.",
                    "Auto Mode Active",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            if (btn == null) return;

            try
            {
                // Get the target position textbox value
                double targetPos = 0;
                // Get PLC address for ABS position
                string absAddr = GetABSPositionAddress(selectedPort, selectedAxis);
                string goAddr = GetGoABSAddress(selectedPort, selectedAxis);

                if (string.IsNullOrEmpty(absAddr) || string.IsNullOrEmpty(goAddr))
                    return;

                // Convert position to PLC units (x100 for XY, x10 for rotation)
                int plcValue = 0;
                if (selectedAxis == "X" || selectedAxis == "Y" || selectedAxis == "Z")
                    plcValue = (int)(targetPos * 100);
                else
                    plcValue = (int)(targetPos * 10);

                // Write target position to PLC
                PLCKey.WriteInt32(absAddr, plcValue);

                // Trigger GO command
                PLCKey.SetBit(goAddr);
                System.Threading.Thread.Sleep(100);
                PLCKey.ResetBit(goAddr);

                //MessageBox.Show($"Moving {axis} axis to {targetPos} (Port {selectedPort})",
                //    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error moving axis: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper: Get axis name from Save button
        //private string GetAxisFromSaveButton(System.Windows.Forms.Button btn)
        //{
        //    if (btn == btnXSave) return "X";
        //    if (btn == btnYSave) return "Y";
        //    if (btn == btnZSave) return "Z";
        //    if (btn == btnRISave) return "RI";
        //    if (btn == btnROSave) return "RO";
        //    if (btn == btnFSave) return "F";
        //    return null;
        //}

        // Helper: Get axis name from Reset button
        private string GetAxisFromResetButton(System.Windows.Forms.Button btn)
        {
            if (btn == btnXReset) return "X";
            if (btn == btnYReset) return "Y";
            if (btn == btnZReset) return "Z";
            if (btn == btnRIReset) return "RI";
            if (btn == btnROReset) return "RO";
            if (btn == btnFReset) return "F";
            return null;
        }

        // Helper: Get axis name from Go button
        //private string GetAxisFromGoButton(System.Windows.Forms.Button btn)
        //{
        //    if (btn == btnXGo) return "X";
        //    if (btn == btnYGo) return "Y";
        //    if (btn == btnZGo) return "Z";
        //    if (btn == btnRIGo) return "RI";
        //    if (btn == btnROGo) return "RO";
        //    if (btn == btnFGo) return "F";
        //    return null;
        //}

        // Helper: Get Speed address based on port and axis
        private string GetSpeedAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    if (axis == "X") return PLCAddresses.Data.P1_Speed_HJog_X;
                    if (axis == "Y") return PLCAddresses.Data.P1_Speed_HJog_Y;
                    if (axis == "Z") return PLCAddresses.Data.P1_Speed_HJog_Z;
                    if (axis == "RI") return PLCAddresses.Data.P1_Speed_HJog_RI;
                    if (axis == "RO") return PLCAddresses.Data.P1_Speed_HJog_RO;
                    if (axis == "F") return PLCAddresses.Data.P1_Speed_HJog_F;
                    break;
                case 2:
                    if (axis == "X") return PLCAddresses.Data.P2_Speed_HJog_X;
                    if (axis == "Y") return PLCAddresses.Data.P2_Speed_HJog_Y;
                    if (axis == "Z") return PLCAddresses.Data.P2_Speed_HJog_Z;
                    if (axis == "RI") return PLCAddresses.Data.P2_Speed_HJog_RI;
                    if (axis == "RO") return PLCAddresses.Data.P2_Speed_HJog_RO;
                    if (axis == "F") return PLCAddresses.Data.P2_Speed_HJog_F;
                    break;
                case 3:
                    if (axis == "X") return PLCAddresses.Data.P3_Speed_HJog_X;
                    if (axis == "Y") return PLCAddresses.Data.P3_Speed_HJog_Y;
                    if (axis == "Z") return PLCAddresses.Data.P3_Speed_HJog_Z;
                    if (axis == "RI") return PLCAddresses.Data.P3_Speed_HJog_RI;
                    if (axis == "RO") return PLCAddresses.Data.P3_Speed_HJog_RO;
                    if (axis == "F") return PLCAddresses.Data.P3_Speed_HJog_F;
                    break;
                case 4:
                    if (axis == "X") return PLCAddresses.Data.P4_Speed_HJog_X;
                    if (axis == "Y") return PLCAddresses.Data.P4_Speed_HJog_Y;
                    if (axis == "Z") return PLCAddresses.Data.P4_Speed_HJog_Z;
                    if (axis == "RI") return PLCAddresses.Data.P4_Speed_HJog_RI;
                    if (axis == "RO") return PLCAddresses.Data.P4_Speed_HJog_RO;
                    if (axis == "F") return PLCAddresses.Data.P4_Speed_HJog_F;
                    break;
            }
            return null;
        }

        // Helper: Get Step address based on port and axis
        private string GetStepAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    if (axis == "X") return PLCAddresses.Data.P1_DisStep_JogPlus_X;
                    if (axis == "Y") return PLCAddresses.Data.P1_DisStep_JogPlus_Y;
                    if (axis == "Z") return PLCAddresses.Data.P1_DisStep_JogPlus_Z;
                    if (axis == "RI") return PLCAddresses.Data.P1_DisStep_JogPlus_RI;
                    if (axis == "RO") return PLCAddresses.Data.P1_DisStep_JogPlus_RO;
                    if (axis == "F") return PLCAddresses.Data.P1_DisStep_JogPlus_F;
                    break;
                case 2:
                    if (axis == "X") return PLCAddresses.Data.P2_DisStep_JogPlus_X;
                    if (axis == "Y") return PLCAddresses.Data.P2_DisStep_JogPlus_Y;
                    if (axis == "Z") return PLCAddresses.Data.P2_DisStep_JogPlus_Z;
                    if (axis == "RI") return PLCAddresses.Data.P2_DisStep_JogPlus_RI;
                    if (axis == "RO") return PLCAddresses.Data.P2_DisStep_JogPlus_RO;
                    if (axis == "F") return PLCAddresses.Data.P2_DisStep_JogPlus_F;
                    break;
                case 3:
                    if (axis == "X") return PLCAddresses.Data.P3_DisStep_JogPlus_X;
                    if (axis == "Y") return PLCAddresses.Data.P3_DisStep_JogPlus_Y;
                    if (axis == "Z") return PLCAddresses.Data.P3_DisStep_JogPlus_Z;
                    if (axis == "RI") return PLCAddresses.Data.P3_DisStep_JogPlus_RI;
                    if (axis == "RO") return PLCAddresses.Data.P3_DisStep_JogPlus_RO;
                    if (axis == "F") return PLCAddresses.Data.P3_DisStep_JogPlus_F;
                    break;
                case 4:
                    if (axis == "X") return PLCAddresses.Data.P4_DisStep_JogPlus_X;
                    if (axis == "Y") return PLCAddresses.Data.P4_DisStep_JogPlus_Y;
                    if (axis == "Z") return PLCAddresses.Data.P4_DisStep_JogPlus_Z;
                    if (axis == "RI") return PLCAddresses.Data.P4_DisStep_JogPlus_RI;
                    if (axis == "RO") return PLCAddresses.Data.P4_DisStep_JogPlus_RO;
                    if (axis == "F") return PLCAddresses.Data.P4_DisStep_JogPlus_F;
                    break;
            }
            return null;
        }

        // Helper: Get Reset bit address based on port and axis
        private string GetResetAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    if (axis == "X") return PLCAddresses.Input.P1_XReset;
                    if (axis == "Y") return PLCAddresses.Input.P1_YReset;
                    if (axis == "Z") return PLCAddresses.Input.P1_ZReset;
                    if (axis == "RI") return PLCAddresses.Input.P1_RIReset;
                    if (axis == "RO") return PLCAddresses.Input.P1_ROReset;
                    if (axis == "F") return PLCAddresses.Input.P1_FReset;
                    break;
                case 2:
                    if (axis == "X") return PLCAddresses.Input.P2_XReset;
                    if (axis == "Y") return PLCAddresses.Input.P2_YReset;
                    if (axis == "Z") return PLCAddresses.Input.P2_ZReset;
                    if (axis == "RI") return PLCAddresses.Input.P2_RIReset;
                    if (axis == "RO") return PLCAddresses.Input.P2_ROReset;
                    if (axis == "F") return PLCAddresses.Input.P2_FReset;
                    break;
                case 3:
                    if (axis == "X") return PLCAddresses.Input.P3_XReset;
                    if (axis == "Y") return PLCAddresses.Input.P3_YReset;
                    if (axis == "Z") return PLCAddresses.Input.P3_ZReset;
                    if (axis == "RI") return PLCAddresses.Input.P3_RIReset;
                    if (axis == "RO") return PLCAddresses.Input.P3_ROReset;
                    if (axis == "F") return PLCAddresses.Input.P3_FReset;
                    break;
                case 4:
                    if (axis == "X") return PLCAddresses.Input.P4_XReset;
                    if (axis == "Y") return PLCAddresses.Input.P4_YReset;
                    if (axis == "Z") return PLCAddresses.Input.P4_ZReset;
                    if (axis == "RI") return PLCAddresses.Input.P4_RIReset;
                    if (axis == "RO") return PLCAddresses.Input.P4_ROReset;
                    if (axis == "F") return PLCAddresses.Input.P4_FReset;
                    break;
            }
            return null;
        }

        // Helper: Get ABS Position address based on port and axis
        private string GetABSPositionAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    if (axis == "X") return PLCAddresses.Data.P1_Point_ABS_X;
                    if (axis == "Y") return PLCAddresses.Data.P1_Point_ABS_Y;
                    if (axis == "Z") return PLCAddresses.Data.P1_Point_ABS_Z;
                    if (axis == "F") return PLCAddresses.Data.P1_Point_ABS_F;
                    break;
                case 2:
                    if (axis == "X") return PLCAddresses.Data.P2_Point_ABS_X;
                    if (axis == "Y") return PLCAddresses.Data.P2_Point_ABS_Y;
                    if (axis == "Z") return PLCAddresses.Data.P2_Point_ABS_Z;
                    if (axis == "F") return PLCAddresses.Data.P2_Point_ABS_F;
                    break;
                case 3:
                    if (axis == "X") return PLCAddresses.Data.P3_Point_ABS_X;
                    if (axis == "Y") return PLCAddresses.Data.P3_Point_ABS_Y;
                    if (axis == "Z") return PLCAddresses.Data.P3_Point_ABS_Z;
                    if (axis == "F") return PLCAddresses.Data.P3_Point_ABS_F;
                    break;
                case 4:
                    if (axis == "X") return PLCAddresses.Data.P4_Point_ABS_X;
                    if (axis == "Y") return PLCAddresses.Data.P4_Point_ABS_Y;
                    if (axis == "Z") return PLCAddresses.Data.P4_Point_ABS_Z;
                    if (axis == "F") return PLCAddresses.Data.P4_Point_ABS_F;
                    break;
            }
            return null;
        }

        // Helper: Get Go ABS command address based on port and axis
        private string GetGoABSAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    if (axis == "X") return PLCAddresses.Input.P1_XGo_ABS;
                    if (axis == "Y") return PLCAddresses.Input.P1_YGo_ABS;
                    if (axis == "Z") return PLCAddresses.Input.P1_ZGo_ABS;
                    if (axis == "RI") return PLCAddresses.Input.P1_RIGo_ABS;
                    if (axis == "RO") return PLCAddresses.Input.P1_ROGo_ABS;
                    if (axis == "F") return PLCAddresses.Input.P1_FGo_ABS;
                    break;
                case 2:
                    if (axis == "X") return PLCAddresses.Input.P2_XGo_ABS;
                    if (axis == "Y") return PLCAddresses.Input.P2_YGo_ABS;
                    if (axis == "Z") return PLCAddresses.Input.P2_ZGo_ABS;
                    if (axis == "RI") return PLCAddresses.Input.P2_RIGo_ABS;
                    if (axis == "RO") return PLCAddresses.Input.P2_ROGo_ABS;
                    if (axis == "F") return PLCAddresses.Input.P2_FGo_ABS;
                    break;
                case 3:
                    if (axis == "X") return PLCAddresses.Input.P3_XGo_ABS;
                    if (axis == "Y") return PLCAddresses.Input.P3_YGo_ABS;
                    if (axis == "Z") return PLCAddresses.Input.P3_ZGo_ABS;
                    if (axis == "RI") return PLCAddresses.Input.P3_RIGo_ABS;
                    if (axis == "RO") return PLCAddresses.Input.P3_ROGo_ABS;
                    if (axis == "F") return PLCAddresses.Input.P3_FGo_ABS;
                    break;
                case 4:
                    if (axis == "X") return PLCAddresses.Input.P4_XGo_ABS;
                    if (axis == "Y") return PLCAddresses.Input.P4_YGo_ABS;
                    if (axis == "Z") return PLCAddresses.Input.P4_ZGo_ABS;
                    if (axis == "RI") return PLCAddresses.Input.P4_RIGo_ABS;
                    if (axis == "RO") return PLCAddresses.Input.P4_ROGo_ABS;
                    if (axis == "F") return PLCAddresses.Input.P4_FGo_ABS;
                    break;
            }
            return null;
        }

        #endregion

        #region Button Home All Reset All
        private string GetHomeAllAddress(int port)
        {
            switch (port)
            {
                case 1: return PLCAddresses.Output.P1_Home_All_Request;
                case 2: return PLCAddresses.Output.P2_Home_All_Request;
                case 3: return PLCAddresses.Output.P3_Home_All_Request;
                case 4: return PLCAddresses.Output.P4_Home_All_Request;
                default: return null;
            }
        }

        private string GetResetAllAddress(int port)
        {
            switch (port)
            {
                case 1: return PLCAddresses.Output.P1_Reset_All_Request;
                case 2: return PLCAddresses.Output.P2_Reset_All_Request;
                case 3: return PLCAddresses.Output.P3_Reset_All_Request;
                case 4: return PLCAddresses.Output.P4_Reset_All_Request;
                default: return null;
            }
        }

        private async void btnResetAll_Click(object sender, EventArgs e)
        {
            // Check if port is in Auto Mode
            if (IsAutoModeActive(selectedPort))
            {
                MessageBox.Show(
                    $"Port {selectedPort} đang ở chế độ Auto!\nKhông thể Reset All khi Auto đang chạy.",
                    "Auto Mode Active",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string addrReset = GetResetAllAddress(selectedPort);
            if (addrReset == null) return;
            PLCKey.SetBit(addrReset);
            grbJogControl.Enabled = false;
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(500);
                    if (!PLCKey.ReadBit(addrReset)) break;
                }
            });
            grbJogControl.Enabled = true;
            MessageBox.Show("Đã reset xong", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private async void btnHomeAll_Click(object sender, EventArgs e)
        {
            // Check if port is in Auto Mode
            if (IsAutoModeActive(selectedPort))
            {
                MessageBox.Show(
                    $"Port {selectedPort} đang ở chế độ Auto!\nKhông thể Home All khi Auto đang chạy.",
                    "Auto Mode Active",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var dialogResult = MessageBox.Show("Bạn có chắc chắn không","Cảnh báo",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.No) return;
            string addHome = GetHomeAllAddress(selectedPort);
            if (addHome == null) return;
            PLCKey.SetBit(addHome);
            grbJogControl.Enabled = false;
            toolStripStatusLabel2.Text = "Đang về gốc....";
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(500);
                    if (!PLCKey.ReadBit(addHome)) break;
                }
            });
            grbJogControl.Enabled = true;
            MessageBox.Show("Đã về gốc xong","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Question);
            toolStripStatusLabel2.Text = "PLC: Đã kết nối (192.168.0.10:8501)";
        }
        #endregion

        #region Teaching Mode Functions

        // Helper: Get current position address based on port and axis
        private string GetCurrentPositionAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    if (axis == "X") return PLCAddresses.Data.P1_X_Pos_Cur;
                    if (axis == "Y") return PLCAddresses.Data.P1_Y_Pos_Cur;
                    if (axis == "Z") return PLCAddresses.Data.P1_Z_Pos_Cur;
                    if (axis == "F") return PLCAddresses.Data.P1_F_Pos_Cur;
                    break;
                case 2:
                    if (axis == "X") return PLCAddresses.Data.P2_X_Pos_Cur;
                    if (axis == "Y") return PLCAddresses.Data.P2_Y_Pos_Cur;
                    if (axis == "Z") return PLCAddresses.Data.P2_Z_Pos_Cur;
                    if (axis == "F") return PLCAddresses.Data.P2_F_Pos_Cur;
                    break;
                case 3:
                    if (axis == "X") return PLCAddresses.Data.P3_X_Pos_Cur;
                    if (axis == "Y") return PLCAddresses.Data.P3_Y_Pos_Cur;
                    if (axis == "Z") return PLCAddresses.Data.P3_Z_Pos_Cur;
                    if (axis == "F") return PLCAddresses.Data.P3_F_Pos_Cur;
                    break;
                case 4:
                    if (axis == "X") return PLCAddresses.Data.P4_X_Pos_Cur;
                    if (axis == "Y") return PLCAddresses.Data.P4_Y_Pos_Cur;
                    if (axis == "Z") return PLCAddresses.Data.P4_Z_Pos_Cur;
                    if (axis == "F") return PLCAddresses.Data.P4_F_Pos_Cur;
                    break;
            }
            return null;
        }

        // Save teaching point: Read current position and save to teaching point address
        private void SaveTeachingPoint(string addressX, string addressY = null, string addressZ = null, string addressF = null, System.Windows.Forms.Button saveButton = null)
        {
            try
            {
                // Read current position for X axis
                if (!string.IsNullOrEmpty(addressX))
                {
                    string curPosAddr = GetCurrentPositionAddress(selectedPort, "X");
                    int currentPosX = PLCKey.ReadInt32(curPosAddr);
                    PLCKey.WriteInt32(addressX, currentPosX);
                }

                // Read current position for Y axis
                if (!string.IsNullOrEmpty(addressY))
                {
                    string curPosAddr = GetCurrentPositionAddress(selectedPort, "Y");
                    int currentPosY = PLCKey.ReadInt32(curPosAddr);
                    PLCKey.WriteInt32(addressY, currentPosY);
                }

                // Read current position for Z axis
                if (!string.IsNullOrEmpty(addressZ))
                {
                    string curPosAddr = GetCurrentPositionAddress(selectedPort, "Z");
                    int currentPosZ = PLCKey.ReadInt32(curPosAddr);
                    PLCKey.WriteInt32(addressZ, currentPosZ);
                }

                // Read current position for F axis
                if (!string.IsNullOrEmpty(addressF))
                {
                    string curPosAddr = GetCurrentPositionAddress(selectedPort, "F");
                    int currentPosF = PLCKey.ReadInt32(curPosAddr);
                    PLCKey.WriteInt32(addressF, currentPosF);
                }

                // Change button color to green on successful save
                if (saveButton != null)
                {
                    saveButton.BackColor = Color.LightGreen;
                }

                MessageBox.Show("Đã lưu tọa độ teaching thành công!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu tọa độ: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Go to teaching point: Read teaching point and execute ABS movement
        private async void GoToTeachingPoint(string addressX, string addressY = null, string addressZ = null, string addressF = null)
        {
            try
            {
                // Move X axis
                if (!string.IsNullOrEmpty(addressX))
                {
                    int targetPosX = PLCKey.ReadInt32(addressX);
                    string absAddr = GetABSPositionAddress(selectedPort, "X");
                    string goAddr = GetGoABSAddress(selectedPort, "X");
                    PLCKey.WriteInt32(absAddr, targetPosX);
                    PLCKey.SetBit(goAddr);

                    // Wait for movement to complete
                    await Task.Delay(100);
                    while (PLCKey.ReadBit(goAddr))
                    {
                        await Task.Delay(100);
                    }
                }

                // Move Y axis
                if (!string.IsNullOrEmpty(addressY))
                {
                    int targetPosY = PLCKey.ReadInt32(addressY);
                    string absAddr = GetABSPositionAddress(selectedPort, "Y");
                    string goAddr = GetGoABSAddress(selectedPort, "Y");
                    PLCKey.WriteInt32(absAddr, targetPosY);
                    PLCKey.SetBit(goAddr);

                    // Wait for movement to complete
                    await Task.Delay(100);
                    while (PLCKey.ReadBit(goAddr))
                    {
                        await Task.Delay(100);
                    }
                }

                // Move Z axis
                if (!string.IsNullOrEmpty(addressZ))
                {
                    int targetPosZ = PLCKey.ReadInt32(addressZ);
                    string absAddr = GetABSPositionAddress(selectedPort, "Z");
                    string goAddr = GetGoABSAddress(selectedPort, "Z");
                    PLCKey.WriteInt32(absAddr, targetPosZ);
                    PLCKey.SetBit(goAddr);

                    // Wait for movement to complete
                    await Task.Delay(100);
                    while (PLCKey.ReadBit(goAddr))
                    {
                        await Task.Delay(100);
                    }
                }

                // Move F axis
                if (!string.IsNullOrEmpty(addressF))
                {
                    int targetPosF = PLCKey.ReadInt32(addressF);
                    string absAddr = GetABSPositionAddress(selectedPort, "F");
                    string goAddr = GetGoABSAddress(selectedPort, "F");
                    PLCKey.WriteInt32(absAddr, targetPosF);
                    PLCKey.SetBit(goAddr);

                    // Wait for movement to complete
                    await Task.Delay(100);
                    while (PLCKey.ReadBit(goAddr))
                    {
                        await Task.Delay(100);
                    }
                }

                MessageBox.Show("Đã di chuyển đến vị trí teaching!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi di chuyển: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to get teaching point addresses based on port and point name
        private (string x, string y, string z, string f) GetTeachingPointAddresses(int port, string pointName)
        {
            switch (port)
            {
                case 1:
                    switch (pointName)
                    {
                        case "TrayInputXYStart":
                            return (PLCAddresses.Data.P1_TeachPoint_TrayInput_XYStart_X, PLCAddresses.Data.P1_TeachPoint_TrayInput_XYStart_Y, null, null);
                        case "TrayInputXEnd":
                            return (PLCAddresses.Data.P1_TeachPoint_TrayInput_XEnd_X, PLCAddresses.Data.P1_TeachPoint_TrayInput_XEnd_Y, null, null);
                        case "TrayInputYEnd":
                            return (PLCAddresses.Data.P1_TeachPoint_TrayInput_YEnd_X, PLCAddresses.Data.P1_TeachPoint_TrayInput_YEnd_Y, null, null);
                        case "TrayInputZ":
                            return (null, null, PLCAddresses.Data.P1_TeachPoint_TrayInput_Z, null);
                        case "TrayNG1XYStart":
                            return (PLCAddresses.Data.P1_TeachPoint_TrayNG1_XYStart_X, PLCAddresses.Data.P1_TeachPoint_TrayNG1_XYStart_Y, null, null);
                        case "TrayNG1XEnd":
                            return (PLCAddresses.Data.P1_TeachPoint_TrayNG1_XEnd_X, PLCAddresses.Data.P1_TeachPoint_TrayNG1_XEnd_Y, null, null);
                        case "TrayNG1YEnd":
                            return (PLCAddresses.Data.P1_TeachPoint_TrayNG1_YEnd_X, PLCAddresses.Data.P1_TeachPoint_TrayNG1_YEnd_Y, null, null);
                        case "TrayNG1Z":
                            return (null, null, PLCAddresses.Data.P1_TeachPoint_TrayNG1_Z, null);
                        case "TrayNG2XYStart":
                            return (PLCAddresses.Data.P1_TeachPoint_TrayNG2_XYStart_X, PLCAddresses.Data.P1_TeachPoint_TrayNG2_XYStart_Y, null, null);
                        case "TrayNG2XEnd":
                            return (PLCAddresses.Data.P1_TeachPoint_TrayNG2_XEnd_X, PLCAddresses.Data.P1_TeachPoint_TrayNG2_XEnd_Y, null, null);
                        case "TrayNG2YEnd":
                            return (PLCAddresses.Data.P1_TeachPoint_TrayNG2_YEnd_X, PLCAddresses.Data.P1_TeachPoint_TrayNG2_YEnd_Y, null, null);
                        case "TrayNG2Z":
                            return (null, null, PLCAddresses.Data.P1_TeachPoint_TrayNG2_Z, null);
                        case "Socket":
                            return (PLCAddresses.Data.P1_TeachPoint_Socket_X, PLCAddresses.Data.P1_TeachPoint_Socket_Y, null, null);
                        case "SocketZLoad":
                            return (null, null, PLCAddresses.Data.P1_TeachPoint_Socket_ZLoad, null);
                        case "SocketZUnload":
                            return (null, null, PLCAddresses.Data.P1_TeachPoint_Socket_ZUnload, null);
                        case "SocketZReady":
                            return (null, null, PLCAddresses.Data.P1_TeachPoint_Socket_ZReady, null);
                        case "SocketZReadyLoad":
                            return (null, null, PLCAddresses.Data.P1_TeachPoint_Socket_ZReadyLoad, null);
                        case "SocketZReadyUnload":
                            return (null, null, PLCAddresses.Data.P1_TeachPoint_Socket_ZReadyUnload, null);
                        case "SocketFOpened":
                            return (null, null, null, PLCAddresses.Data.P1_TeachPoint_Socket_FOpened);
                        case "SocketFClosed":
                            return (null, null, null, PLCAddresses.Data.P1_TeachPoint_Socket_FClosed);
                        case "Camera":
                            return (PLCAddresses.Data.P1_TeachPoint_Camera_X, PLCAddresses.Data.P1_TeachPoint_Camera_Y, null, null);
                        case "SocketCameraZ":
                            return (null, null, PLCAddresses.Data.P1_TeachPoint_Camera_Z, null);
                    }
                    break;
                case 2:
                    switch (pointName)
                    {
                        case "TrayInputXYStart":
                            return (PLCAddresses.Data.P2_TeachPoint_TrayInput_XYStart_X, PLCAddresses.Data.P2_TeachPoint_TrayInput_XYStart_Y, null, null);
                        case "TrayInputXEnd":
                            return (PLCAddresses.Data.P2_TeachPoint_TrayInput_XEnd_X, PLCAddresses.Data.P2_TeachPoint_TrayInput_XEnd_Y, null, null);
                        case "TrayInputYEnd":
                            return (PLCAddresses.Data.P2_TeachPoint_TrayInput_YEnd_X, PLCAddresses.Data.P2_TeachPoint_TrayInput_YEnd_Y, null, null);
                        case "TrayInputZ":
                            return (null, null, PLCAddresses.Data.P2_TeachPoint_TrayInput_Z, null);
                        case "TrayNG1XYStart":
                            return (PLCAddresses.Data.P2_TeachPoint_TrayNG1_XYStart_X, PLCAddresses.Data.P2_TeachPoint_TrayNG1_XYStart_Y, null, null);
                        case "TrayNG1XEnd":
                            return (PLCAddresses.Data.P2_TeachPoint_TrayNG1_XEnd_X, PLCAddresses.Data.P2_TeachPoint_TrayNG1_XEnd_Y, null, null);
                        case "TrayNG1YEnd":
                            return (PLCAddresses.Data.P2_TeachPoint_TrayNG1_YEnd_X, PLCAddresses.Data.P2_TeachPoint_TrayNG1_YEnd_Y, null, null);
                        case "TrayNG1Z":
                            return (null, null, PLCAddresses.Data.P2_TeachPoint_TrayNG1_Z, null);
                        case "TrayNG2XYStart":
                            return (PLCAddresses.Data.P2_TeachPoint_TrayNG2_XYStart_X, PLCAddresses.Data.P2_TeachPoint_TrayNG2_XYStart_Y, null, null);
                        case "TrayNG2XEnd":
                            return (PLCAddresses.Data.P2_TeachPoint_TrayNG2_XEnd_X, PLCAddresses.Data.P2_TeachPoint_TrayNG2_XEnd_Y, null, null);
                        case "TrayNG2YEnd":
                            return (PLCAddresses.Data.P2_TeachPoint_TrayNG2_YEnd_X, PLCAddresses.Data.P2_TeachPoint_TrayNG2_YEnd_Y, null, null);
                        case "TrayNG2Z":
                            return (null, null, PLCAddresses.Data.P2_TeachPoint_TrayNG2_Z, null);
                        case "Socket":
                            return (PLCAddresses.Data.P2_TeachPoint_Socket_X, PLCAddresses.Data.P2_TeachPoint_Socket_Y, null, null);
                        case "SocketZLoad":
                            return (null, null, PLCAddresses.Data.P2_TeachPoint_Socket_ZLoad, null);
                        case "SocketZUnload":
                            return (null, null, PLCAddresses.Data.P2_TeachPoint_Socket_ZUnload, null);
                        case "SocketZReady":
                            return (null, null, PLCAddresses.Data.P2_TeachPoint_Socket_ZReady, null);
                        case "SocketZReadyLoad":
                            return (null, null, PLCAddresses.Data.P2_TeachPoint_Socket_ZReadyLoad, null);
                        case "SocketZReadyUnload":
                            return (null, null, PLCAddresses.Data.P2_TeachPoint_Socket_ZReadyUnload, null);
                        case "SocketFOpened":
                            return (null, null, null, PLCAddresses.Data.P2_TeachPoint_Socket_FOpened);
                        case "SocketFClosed":
                            return (null, null, null, PLCAddresses.Data.P2_TeachPoint_Socket_FClosed);
                        case "Camera":
                            return (PLCAddresses.Data.P2_TeachPoint_Camera_X, PLCAddresses.Data.P2_TeachPoint_Camera_Y, null, null);
                        case "SocketCameraZ":
                            return (null, null, PLCAddresses.Data.P2_TeachPoint_Camera_Z, null);
                    }
                    break;
                case 3:
                    switch (pointName)
                    {
                        case "TrayInputXYStart":
                            return (PLCAddresses.Data.P3_TeachPoint_TrayInput_XYStart_X, PLCAddresses.Data.P3_TeachPoint_TrayInput_XYStart_Y, null, null);
                        case "TrayInputXEnd":
                            return (PLCAddresses.Data.P3_TeachPoint_TrayInput_XEnd_X, PLCAddresses.Data.P3_TeachPoint_TrayInput_XEnd_Y, null, null);
                        case "TrayInputYEnd":
                            return (PLCAddresses.Data.P3_TeachPoint_TrayInput_YEnd_X, PLCAddresses.Data.P3_TeachPoint_TrayInput_YEnd_Y, null, null);
                        case "TrayInputZ":
                            return (null, null, PLCAddresses.Data.P3_TeachPoint_TrayInput_Z, null);
                        case "TrayNG1XYStart":
                            return (PLCAddresses.Data.P3_TeachPoint_TrayNG1_XYStart_X, PLCAddresses.Data.P3_TeachPoint_TrayNG1_XYStart_Y, null, null);
                        case "TrayNG1XEnd":
                            return (PLCAddresses.Data.P3_TeachPoint_TrayNG1_XEnd_X, PLCAddresses.Data.P3_TeachPoint_TrayNG1_XEnd_Y, null, null);
                        case "TrayNG1YEnd":
                            return (PLCAddresses.Data.P3_TeachPoint_TrayNG1_YEnd_X, PLCAddresses.Data.P3_TeachPoint_TrayNG1_YEnd_Y, null, null);
                        case "TrayNG1Z":
                            return (null, null, PLCAddresses.Data.P3_TeachPoint_TrayNG1_Z, null);
                        case "TrayNG2XYStart":
                            return (PLCAddresses.Data.P3_TeachPoint_TrayNG2_XYStart_X, PLCAddresses.Data.P3_TeachPoint_TrayNG2_XYStart_Y, null, null);
                        case "TrayNG2XEnd":
                            return (PLCAddresses.Data.P3_TeachPoint_TrayNG2_XEnd_X, PLCAddresses.Data.P3_TeachPoint_TrayNG2_XEnd_Y, null, null);
                        case "TrayNG2YEnd":
                            return (PLCAddresses.Data.P3_TeachPoint_TrayNG2_YEnd_X, PLCAddresses.Data.P3_TeachPoint_TrayNG2_YEnd_Y, null, null);
                        case "TrayNG2Z":
                            return (null, null, PLCAddresses.Data.P3_TeachPoint_TrayNG2_Z, null);
                        case "Socket":
                            return (PLCAddresses.Data.P3_TeachPoint_Socket_X, PLCAddresses.Data.P3_TeachPoint_Socket_Y, null, null);
                        case "SocketZLoad":
                            return (null, null, PLCAddresses.Data.P3_TeachPoint_Socket_ZLoad, null);
                        case "SocketZUnload":
                            return (null, null, PLCAddresses.Data.P3_TeachPoint_Socket_ZUnload, null);
                        case "SocketZReady":
                            return (null, null, PLCAddresses.Data.P3_TeachPoint_Socket_ZReady, null);
                        case "SocketZReadyLoad":
                            return (null, null, PLCAddresses.Data.P3_TeachPoint_Socket_ZReadyLoad, null);
                        case "SocketZReadyUnload":
                            return (null, null, PLCAddresses.Data.P3_TeachPoint_Socket_ZReadyUnload, null);
                        case "SocketFOpened":
                            return (null, null, null, PLCAddresses.Data.P3_TeachPoint_Socket_FOpened);
                        case "SocketFClosed":
                            return (null, null, null, PLCAddresses.Data.P3_TeachPoint_Socket_FClosed);
                        case "Camera":
                            return (PLCAddresses.Data.P3_TeachPoint_Camera_X, PLCAddresses.Data.P3_TeachPoint_Camera_Y, null, null);
                        case "SocketCameraZ":
                            return (null, null, PLCAddresses.Data.P3_TeachPoint_Camera_Z, null);
                    }
                    break;
                case 4:
                    switch (pointName)
                    {
                        case "TrayInputXYStart":
                            return (PLCAddresses.Data.P4_TeachPoint_TrayInput_XYStart_X, PLCAddresses.Data.P4_TeachPoint_TrayInput_XYStart_Y, null, null);
                        case "TrayInputXEnd":
                            return (PLCAddresses.Data.P4_TeachPoint_TrayInput_XEnd_X, PLCAddresses.Data.P4_TeachPoint_TrayInput_XEnd_Y, null, null);
                        case "TrayInputYEnd":
                            return (PLCAddresses.Data.P4_TeachPoint_TrayInput_YEnd_X, PLCAddresses.Data.P4_TeachPoint_TrayInput_YEnd_Y, null, null);
                        case "TrayInputZ":
                            return (null, null, PLCAddresses.Data.P4_TeachPoint_TrayInput_Z, null);
                        case "TrayNG1XYStart":
                            return (PLCAddresses.Data.P4_TeachPoint_TrayNG1_XYStart_X, PLCAddresses.Data.P4_TeachPoint_TrayNG1_XYStart_Y, null, null);
                        case "TrayNG1XEnd":
                            return (PLCAddresses.Data.P4_TeachPoint_TrayNG1_XEnd_X, PLCAddresses.Data.P4_TeachPoint_TrayNG1_XEnd_Y, null, null);
                        case "TrayNG1YEnd":
                            return (PLCAddresses.Data.P4_TeachPoint_TrayNG1_YEnd_X, PLCAddresses.Data.P4_TeachPoint_TrayNG1_YEnd_Y, null, null);
                        case "TrayNG1Z":
                            return (null, null, PLCAddresses.Data.P4_TeachPoint_TrayNG1_Z, null);
                        case "TrayNG2XYStart":
                            return (PLCAddresses.Data.P4_TeachPoint_TrayNG2_XYStart_X, PLCAddresses.Data.P4_TeachPoint_TrayNG2_XYStart_Y, null, null);
                        case "TrayNG2XEnd":
                            return (PLCAddresses.Data.P4_TeachPoint_TrayNG2_XEnd_X, PLCAddresses.Data.P4_TeachPoint_TrayNG2_XEnd_Y, null, null);
                        case "TrayNG2YEnd":
                            return (PLCAddresses.Data.P4_TeachPoint_TrayNG2_YEnd_X, PLCAddresses.Data.P4_TeachPoint_TrayNG2_YEnd_Y, null, null);
                        case "TrayNG2Z":
                            return (null, null, PLCAddresses.Data.P4_TeachPoint_TrayNG2_Z, null);
                        case "Socket":
                            return (PLCAddresses.Data.P4_TeachPoint_Socket_X, PLCAddresses.Data.P4_TeachPoint_Socket_Y, null, null);
                        case "SocketZLoad":
                            return (null, null, PLCAddresses.Data.P4_TeachPoint_Socket_ZLoad, null);
                        case "SocketZUnload":
                            return (null, null, PLCAddresses.Data.P4_TeachPoint_Socket_ZUnload, null);
                        case "SocketZReady":
                            return (null, null, PLCAddresses.Data.P4_TeachPoint_Socket_ZReady, null);
                        case "SocketZReadyLoad":
                            return (null, null, PLCAddresses.Data.P4_TeachPoint_Socket_ZReadyLoad, null);
                        case "SocketZReadyUnload":
                            return (null, null, PLCAddresses.Data.P4_TeachPoint_Socket_ZReadyUnload, null);
                        case "SocketFOpened":
                            return (null, null, null, PLCAddresses.Data.P4_TeachPoint_Socket_FOpened);
                        case "SocketFClosed":
                            return (null, null, null, PLCAddresses.Data.P4_TeachPoint_Socket_FClosed);
                        case "Camera":
                            return (PLCAddresses.Data.P4_TeachPoint_Camera_X, PLCAddresses.Data.P4_TeachPoint_Camera_Y, null, null);
                        case "SocketCameraZ":
                            return (null, null, PLCAddresses.Data.P4_TeachPoint_Camera_Z, null);
                    }
                    break;
            }
            return (null, null, null, null);
        }

        // Generic Save button handler
        private void btnSavePoint_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            if (btn == null) return;

            // Determine which point based on button name
            string pointName = btn.Name.Replace("btnSavePoint", "");
            var addresses = GetTeachingPointAddresses(selectedPort, pointName);
            SaveTeachingPoint(addresses.x, addresses.y, addresses.z, addresses.f, btn);
        }

        // Save teaching point by name (for keyboard shortcuts)
        private void SaveTeachingPointByName(string pointName)
        {
            var addresses = GetTeachingPointAddresses(selectedPort, pointName);

            // Find the corresponding save button to highlight it
            System.Windows.Forms.Button saveButton = null;
            string buttonName = "btnSavePoint" + pointName;

            // Search for the button in teaching groups
            foreach (Control ctrl in grpTeachingTray.Controls)
            {
                if (ctrl is GroupBox grp)
                {
                    foreach (Control subCtrl in grp.Controls)
                    {
                        if (subCtrl is GroupBox subGrp)
                        {
                            foreach (Control btnCtrl in subGrp.Controls)
                            {
                                if (btnCtrl.Name == buttonName)
                                {
                                    saveButton = btnCtrl as System.Windows.Forms.Button;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // Also search in socket group
            if (saveButton == null)
            {
                foreach (Control ctrl in grpTeachingSocket.Controls)
                {
                    if (ctrl is GroupBox grp)
                    {
                        foreach (Control btnCtrl in grp.Controls)
                        {
                            if (btnCtrl.Name == buttonName)
                            {
                                saveButton = btnCtrl as System.Windows.Forms.Button;
                                break;
                            }
                        }
                    }
                }
            }

            SaveTeachingPoint(addresses.x, addresses.y, addresses.z, addresses.f, saveButton);
        }

        // Reset all teaching save button colors
        private void ResetTeachingSaveButtonColors()
        {
            // Reset Tray group buttons
            foreach (Control ctrl in grpTeachingTray.Controls)
            {
                if (ctrl is GroupBox grp)
                {
                    foreach (Control subCtrl in grp.Controls)
                    {
                        if (subCtrl is GroupBox subGrp)
                        {
                            foreach (Control btnCtrl in subGrp.Controls)
                            {
                                if (btnCtrl is System.Windows.Forms.Button btn && btn.Name.Contains("btnSavePoint"))
                                {
                                    btn.BackColor = SystemColors.Control;
                                }
                            }
                        }
                    }
                }
            }

            // Reset Socket group buttons
            foreach (Control ctrl in grpTeachingSocket.Controls)
            {
                if (ctrl is GroupBox grp)
                {
                    foreach (Control btnCtrl in grp.Controls)
                    {
                        if (btnCtrl is System.Windows.Forms.Button btn && btn.Name.Contains("btnSavePoint"))
                        {
                            btn.BackColor = SystemColors.Control;
                        }
                    }
                }
            }
        }

        // Generic Go button handler
        private void btnGoPoint_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            if (btn == null) return;

            // Determine which point based on button name
            string pointName = btn.Name.Replace("btnGoPoint", "");
            var addresses = GetTeachingPointAddresses(selectedPort, pointName);
            GoToTeachingPoint(addresses.x, addresses.y, addresses.z, addresses.f);
        }

        /// <summary>
        /// Execute teaching hotkey action (Save or Go to teaching point)
        /// </summary>
        private void ExecuteTeachingHotkey(TeachingPointHotkey hotkey)
        {
            try
            {
                // Find the button control
                System.Windows.Forms.Button targetButton = FindButtonByName(hotkey.ButtonName);

                if (targetButton != null)
                {
                    // Visual feedback - briefly highlight the button
                    FlashButton(targetButton);

                    // Determine action based on button name
                    if (hotkey.ButtonName.StartsWith("btnSavePoint"))
                    {
                        // Save teaching point
                        SaveTeachingPointByName(hotkey.PointName);

                        // Show brief notification
                        ShowHotkeyNotification($"Đã lưu: {hotkey.Description}\nPhím: {hotkey.GetHotkeyString()}");
                    }
                    else if (hotkey.ButtonName.StartsWith("btnGoPoint"))
                    {
                        // Go to teaching point
                        var addresses = GetTeachingPointAddresses(selectedPort, hotkey.PointName);
                        GoToTeachingPoint(addresses.x, addresses.y, addresses.z, addresses.f);

                        // Show brief notification
                        ShowHotkeyNotification($"Di chuyển đến: {hotkey.Description}\nPhím: {hotkey.GetHotkeyString()}");
                    }
                }
                else
                {
                    // Button not found - show error
                    MessageBox.Show($"Không tìm thấy button: {hotkey.ButtonName}\nPoint: {hotkey.PointName}",
                                  "Lỗi Hotkey",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thực hiện hotkey:\n{ex.Message}",
                              "Lỗi",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Find button control by name in teaching groups
        /// </summary>
        private System.Windows.Forms.Button FindButtonByName(string buttonName)
        {
            // Search in Tray group
            System.Windows.Forms.Button button = FindButtonInControl(grpTeachingTray, buttonName);
            if (button != null) return button;

            // Search in Socket group
            button = FindButtonInControl(grpTeachingSocket, buttonName);
            return button;
        }

        /// <summary>
        /// Recursively find button in control hierarchy
        /// </summary>
        private System.Windows.Forms.Button FindButtonInControl(Control parent, string buttonName)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is System.Windows.Forms.Button btn && ctrl.Name == buttonName)
                {
                    return btn;
                }

                // Recursively search in GroupBox or other containers
                if (ctrl.HasChildren)
                {
                    var result = FindButtonInControl(ctrl, buttonName);
                    if (result != null) return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Flash button to provide visual feedback
        /// </summary>
        private async void FlashButton(System.Windows.Forms.Button button)
        {
            if (button == null) return;

            Color originalColor = button.BackColor;

            // Flash yellow briefly
            button.BackColor = Color.Yellow;
            await Task.Delay(150);

            // For save buttons, restore to green if already saved, otherwise back to original
            if (button.Name.StartsWith("btnSavePoint"))
            {
                // Keep green for save buttons (will be set by SaveTeachingPoint)
                // The SaveTeachingPoint method already sets it to LightGreen
            }
            else
            {
                button.BackColor = originalColor;
            }
        }

        /// <summary>
        /// Show brief notification for hotkey action
        /// </summary>
        private async void ShowHotkeyNotification(string message)
        {
            // Create a temporary label to show notification
            // This is a simple implementation - can be enhanced with a custom notification form

            // For now, use status bar or title bar
            string originalTitle = this.Text;
            this.Text = $"[{message}] - {originalTitle}";

            await Task.Delay(2000); // Show for 2 seconds

            this.Text = originalTitle;
        }

        /// <summary>
        /// Show Teaching Hotkey Help Form
        /// Can be called from a button or menu item
        /// </summary>
        private void ShowTeachingHotkeyHelp()
        {
            if (hotkeyManager != null)
            {
                var helpForm = new TeachingHotkeyHelpForm(hotkeyManager);
                helpForm.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Hotkey Manager chưa được khởi tạo.",
                              "Lỗi",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handle Ctrl+H to show hotkey help (can be called from KeyDown if needed)
        /// </summary>
        private void CheckHotkeyHelpShortcut(KeyEventArgs e)
        {
            // Ctrl+H to show help
            if (e.Control && e.KeyCode == Keys.H)
            {
                ShowTeachingHotkeyHelp();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // Update teaching point textboxes with current saved values from PLC
        private void UpdateTeachingPointDisplays()
        {
            try
            {
                // Get teaching point addresses based on selected port
                var trayInputXYStart = GetTeachingPointAddresses(selectedPort, "TrayInputXYStart");
                var trayInputXEnd = GetTeachingPointAddresses(selectedPort, "TrayInputXEnd");
                var trayInputYEnd = GetTeachingPointAddresses(selectedPort, "TrayInputYEnd");
                var trayInputZ = GetTeachingPointAddresses(selectedPort, "TrayInputZ");

                var trayNG1XYStart = GetTeachingPointAddresses(selectedPort, "TrayNG1XYStart");
                var trayNG1XEnd = GetTeachingPointAddresses(selectedPort, "TrayNG1XEnd");
                var trayNG1YEnd = GetTeachingPointAddresses(selectedPort, "TrayNG1YEnd");
                var trayNG1Z = GetTeachingPointAddresses(selectedPort, "TrayNG1Z");

                var trayNG2XYStart = GetTeachingPointAddresses(selectedPort, "TrayNG2XYStart");
                var trayNG2XEnd = GetTeachingPointAddresses(selectedPort, "TrayNG2XEnd");
                var trayNG2YEnd = GetTeachingPointAddresses(selectedPort, "TrayNG2YEnd");
                var trayNG2Z = GetTeachingPointAddresses(selectedPort, "TrayNG2Z");

                var socket = GetTeachingPointAddresses(selectedPort, "Socket");
                var socketZLoad = GetTeachingPointAddresses(selectedPort, "SocketZLoad");
                var socketZUnload = GetTeachingPointAddresses(selectedPort, "SocketZUnload");
                var socketZReady = GetTeachingPointAddresses(selectedPort, "SocketZReady");
                var socketZReadyLoad = GetTeachingPointAddresses(selectedPort, "SocketZReadyLoad");
                var socketZReadyUnload = GetTeachingPointAddresses(selectedPort, "SocketZReadyUnload");
                var socketFOpened = GetTeachingPointAddresses(selectedPort, "SocketFOpened");
                var socketFClosed = GetTeachingPointAddresses(selectedPort, "SocketFClosed");

                var camera = GetTeachingPointAddresses(selectedPort, "Camera");
                var socketCameraZ = GetTeachingPointAddresses(selectedPort, "SocketCameraZ");

                // Update Tray Input textboxes
                if (!string.IsNullOrEmpty(trayInputXYStart.x))
                {
                    txtXPointTrayInputXYStart.Text = (PLCKey.ReadInt32(trayInputXYStart.x) / 100.0f).ToString("F2");
                    txtYPointTrayInputXYStart.Text = (PLCKey.ReadInt32(trayInputXYStart.y) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(trayInputXEnd.x))
                {
                    txtXPointTrayInputXEnd.Text = (PLCKey.ReadInt32(trayInputXEnd.x) / 100.0f).ToString("F2");
                    txtYPointTrayInputXEnd.Text = (PLCKey.ReadInt32(trayInputXEnd.y) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(trayInputYEnd.x))
                {
                    txtXPointTrayInputYEnd.Text = (PLCKey.ReadInt32(trayInputYEnd.x) / 100.0f).ToString("F2");
                    txtYPointTrayInputYEnd.Text = (PLCKey.ReadInt32(trayInputYEnd.y) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(trayInputZ.z))
                {
                    txtZPointTrayInputZ.Text = (PLCKey.ReadInt32(trayInputZ.z) / 100.0f).ToString("F2");
                }

                // Update Tray NG1 textboxes
                if (!string.IsNullOrEmpty(trayNG1XYStart.x))
                {
                    txtXPointTrayNG1XYStart.Text = (PLCKey.ReadInt32(trayNG1XYStart.x) / 100.0f).ToString("F2");
                    txtYPointTrayNG1XYStart.Text = (PLCKey.ReadInt32(trayNG1XYStart.y) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(trayNG1XEnd.x))
                {
                    txtXPointTrayNG1XEnd.Text = (PLCKey.ReadInt32(trayNG1XEnd.x) / 100.0f).ToString("F2");
                    txtYPointTrayNG1XEnd.Text = (PLCKey.ReadInt32(trayNG1XEnd.y) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(trayNG1YEnd.x))
                {
                    txtXPointTrayNG1YEnd.Text = (PLCKey.ReadInt32(trayNG1YEnd.x) / 100.0f).ToString("F2");
                    txtYPointTrayNG1YEnd.Text = (PLCKey.ReadInt32(trayNG1YEnd.y) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(trayNG1Z.z))
                {
                    txtZPointTrayNG1Z.Text = (PLCKey.ReadInt32(trayNG1Z.z) / 100.0f).ToString("F2");
                }

                // Update Tray NG2 textboxes
                if (!string.IsNullOrEmpty(trayNG2XYStart.x))
                {
                    txtXPointTrayNG2XYStart.Text = (PLCKey.ReadInt32(trayNG2XYStart.x) / 100.0f).ToString("F2");
                    txtYPointTrayNG2XYStart.Text = (PLCKey.ReadInt32(trayNG2XYStart.y) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(trayNG2XEnd.x))
                {
                    txtXPointTrayNG2XEnd.Text = (PLCKey.ReadInt32(trayNG2XEnd.x) / 100.0f).ToString("F2");
                    txtYPointTrayNG2XEnd.Text = (PLCKey.ReadInt32(trayNG2XEnd.y) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(trayNG2YEnd.x))
                {
                    txtXPointTrayNG2YEnd.Text = (PLCKey.ReadInt32(trayNG2YEnd.x) / 100.0f).ToString("F2");
                    txtYPointTrayNG2YEnd.Text = (PLCKey.ReadInt32(trayNG2YEnd.y) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(trayNG2Z.z))
                {
                    txtZPointTrayNG2Z.Text = (PLCKey.ReadInt32(trayNG2Z.z) / 100.0f).ToString("F2");
                }

                // Update Socket textboxes
                if (!string.IsNullOrEmpty(socket.x))
                {
                    txtXPointSocket.Text = (PLCKey.ReadInt32(socket.x) / 100.0f).ToString("F2");
                    txtYPointSocket.Text = (PLCKey.ReadInt32(socket.y) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(socketZLoad.z))
                {
                    txtZPointSocketZLoad.Text = (PLCKey.ReadInt32(socketZLoad.z) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(socketZUnload.z))
                {
                    txtZPointSocketZUnload.Text = (PLCKey.ReadInt32(socketZUnload.z) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(socketZReady.z))
                {
                    txtZPointSocketZReady.Text = (PLCKey.ReadInt32(socketZReady.z) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(socketZReadyLoad.z))
                {
                    txtZPointSocketZReadyLoad.Text = (PLCKey.ReadInt32(socketZReadyLoad.z) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(socketZReadyUnload.z))
                {
                    txtZPointSocketZReadyUnload.Text = (PLCKey.ReadInt32(socketZReadyUnload.z) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(socketFOpened.f))
                {
                    txtFPointSocketFOpened.Text = (PLCKey.ReadInt32(socketFOpened.f) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(socketFClosed.f))
                {
                    txtFPointSocketFClosed.Text = (PLCKey.ReadInt32(socketFClosed.f) / 100.0f).ToString("F2");
                }

                // Update Camera textboxes
                if (!string.IsNullOrEmpty(camera.x))
                {
                    txtXPointCamera.Text = (PLCKey.ReadInt32(camera.x) / 100.0f).ToString("F2");
                    txtYPointCamera.Text = (PLCKey.ReadInt32(camera.y) / 100.0f).ToString("F2");
                }
                if (!string.IsNullOrEmpty(socketCameraZ.z))
                {
                    txtZPointSocketCameraZ.Text = (PLCKey.ReadInt32(socketCameraZ.z) / 100.0f).ToString("F2");
                }
            }
            catch (Exception ex)
            {
                // Silently handle errors during display update
                // This prevents timer from crashing if PLC communication fails
            }
        }

        #endregion

        private void Closing(object sender, FormClosingEventArgs e)
        {
            if (rbtPort1.Checked)
            {
                Properties.Settings.Default.SelectedRadio = "Port1";
            }
            else if (rbtPort2.Checked)
            {
                Properties.Settings.Default.SelectedRadio = "Port2";
            }
            else if (rbtPort3.Checked)
            {
                Properties.Settings.Default.SelectedRadio = "Port3";
            }
            else if (rbtPort4.Checked)
            {
                Properties.Settings.Default.SelectedRadio = "Port4";
            }
            Properties.Settings.Default.Save(); 
        }
        private void CloseIOport(object sender, FormClosingEventArgs e)
        {
            if (rbtIOPort1.Checked)
            {
                Properties.Settings.Default.SelectedRadio = "Port1";
            }
            else if (rbtIOPort2.Checked)
            {
                Properties.Settings.Default.SelectedRadio = "Port2";
            }
            else if (rbtIOPort3.Checked)
            {
                Properties.Settings.Default.SelectedRadio = "Port3";
            }
            else if (rbtIOPort4.Checked)
            {
                Properties.Settings.Default.SelectedRadio = "Port4";
            }
            Properties.Settings.Default.Save();
        }

        #region Speed and Step Input Validation and Auto-Save

        // Validate numeric input with decimal point (only allow numbers and one decimal point)
        private void SpeedStepTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox txt = sender as System.Windows.Forms.TextBox;
            if (txt == null) return;

            // Allow control keys (backspace, delete, etc.)
            if (char.IsControl(e.KeyChar))
                return;

            // Allow digits
            if (char.IsDigit(e.KeyChar))
                return;

            // Allow decimal point (.) only if it doesn't already exist
            if (e.KeyChar == '.' && !txt.Text.Contains("."))
                return;

            // Block all other characters
            e.Handled = true;
        }

        // Save Speed or Step value to PLC when Enter key is pressed
        private void SpeedStepTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            System.Windows.Forms.TextBox txt = sender as System.Windows.Forms.TextBox;
            if (txt == null) return;

            try
            {
                // Parse the value and multiply by 100, truncate to 2 decimal places
                if (!double.TryParse(txt.Text, out double value))
                {
                    MessageBox.Show("Giá trị không hợp lệ!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Round to 2 decimal places and convert to integer (multiply by 100)
                int plcValue = (int)(Math.Round(value, 2) * 100);

                // Determine which textbox and get corresponding PLC address
                string plcAddress = GetSpeedStepAddress(txt);
                if (string.IsNullOrEmpty(plcAddress))
                {
                    MessageBox.Show("Không tìm thấy địa chỉ PLC!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Write to PLC
                PLCKey.WriteInt32(plcAddress, plcValue);

                // Update textbox with properly formatted value (2 decimals)
                txt.Text = value.ToString("F2");

                // Move focus away from textbox
                this.ActiveControl = null;

                // Show success message
                MessageBox.Show($"Đã ghi thành công: {value:F2} (PLC value: {plcValue})", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi ghi xuống PLC: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Get PLC address for a specific Speed or Step textbox
        private string GetSpeedStepAddress(System.Windows.Forms.TextBox txt)
        {
            // Speed textboxes
            if (txt == txtXSpeed) return GetSpeedAddress(selectedPort, "X");
            if (txt == txtYSpeed) return GetSpeedAddress(selectedPort, "Y");
            if (txt == txtZSpeed) return GetSpeedAddress(selectedPort, "Z");
            if (txt == txtRISpeed) return GetSpeedAddress(selectedPort, "RI");
            if (txt == txtROSpeed) return GetSpeedAddress(selectedPort, "RO");
            if (txt == txtFSpeed) return GetSpeedAddress(selectedPort, "F");

            // Step textboxes
            if (txt == txtXStep) return GetStepAddress(selectedPort, "X");
            if (txt == txtYStep) return GetStepAddress(selectedPort, "Y");
            if (txt == txtZStep) return GetStepAddress(selectedPort, "Z");
            if (txt == txtRIStep) return GetStepAddress(selectedPort, "RI");
            if (txt == txtROStep) return GetStepAddress(selectedPort, "RO");
            if (txt == txtFStep) return GetStepAddress(selectedPort, "F");

            return null;
        }

        #endregion

        private void btnClearCam1_Click(object sender, EventArgs e)
        {
            ClearCam1HandEyeStatus();
        }

        private void btnClearCam2_Click(object sender, EventArgs e)
        {
            ClearCam2HandEyeStatus();
        }

        #region Model Management

        /// <summary>
        /// Refresh ComboBox with available models
        /// </summary>
        private void RefreshModelComboBox()
        {
            string[] modelNames = modelManager.GetModelNames();
            cbbModel.Items.Clear();
            cbbModel.Items.AddRange(modelNames);

            if (cbbModel.Items.Count > 0)
            {
                cbbModel.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Add new model button click handler
        /// </summary>
        private void btnModelAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Prompt for model name
                string modelName = ModelNameInputDialog.Show(
                    "Nhập tên model:",
                    "Thêm Model Mới",
                    this);

                if (string.IsNullOrWhiteSpace(modelName))
                {
                    return;  // User cancelled or entered empty name
                }

                // Check if model already exists
                if (modelManager.ModelExists(modelName))
                {
                    MessageBox.Show(
                        $"Model '{modelName}' đã tồn tại!",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Read all current teaching points from PLC
                var model = new TeachingModel(modelName);

                // Read teaching points for all 4 ports
                ReadAllTeachingPointsFromPLC(model);

                // Save model
                modelManager.SaveModel(model);

                // Refresh ComboBox
                RefreshModelComboBox();

                // Select the newly added model
                cbbModel.SelectedItem = modelName;

                MessageBox.Show(
                    $"Model '{modelName}' đã được lưu thành công!",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi thêm model: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Model selection changed handler
        /// </summary>
        private void cbbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Chỉ cho phép chọn model, không tự động load
            // User phải nhấn btnModelLoad để thực sự load model vào PLC
        }

        /// <summary>
        /// Load model button click handler
        /// </summary>
        private void btnModelLoad_Click(object sender, EventArgs e)
        {
            if (cbbModel.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn model cần load!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Chỉ cho phép load model khi ở chế độ Teaching
            if (!rbtTeachingMode.Checked)
            {
                MessageBox.Show(
                    "Vui lòng chuyển sang chế độ Teaching trước khi load model!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string modelName = cbbModel.SelectedItem.ToString();
                var model = modelManager.GetModel(modelName);

                if (model == null)
                {
                    MessageBox.Show(
                        $"Không tìm thấy model '{modelName}'!",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Confirm before loading
                var result = MessageBox.Show(
                    $"Bạn có muốn load teaching points từ model '{modelName}'?\n\n" +
                    $"Thao tác này sẽ ghi đè các teaching points hiện tại trong PLC.",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Write all teaching points to PLC
                    WriteAllTeachingPointsToPLC(model);

                    MessageBox.Show(
                        $"Model '{modelName}' đã được load thành công!",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Update button colors to reflect loaded points
                    UpdateTeachingButtonColors();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi load model: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Delete model button click handler
        /// </summary>
        private void btnModelDelete_Click(object sender, EventArgs e)
        {
            if (cbbModel.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn model cần xóa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string modelName = cbbModel.SelectedItem.ToString();

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa model '{modelName}'?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    modelManager.DeleteModel(modelName);
                    RefreshModelComboBox();

                    MessageBox.Show(
                        $"Model '{modelName}' đã được xóa!",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Lỗi khi xóa model: {ex.Message}",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Read all teaching points from PLC for all 4 ports
        /// </summary>
        private void ReadAllTeachingPointsFromPLC(TeachingModel model)
        {
            // Read Port 1
            ReadPortTeachingPointsFromPLC(1, model.Port1);
            // Read Port 2
            ReadPortTeachingPointsFromPLC(2, model.Port2);
            // Read Port 3
            ReadPortTeachingPointsFromPLC(3, model.Port3);
            // Read Port 4
            ReadPortTeachingPointsFromPLC(4, model.Port4);
        }

        /// <summary>
        /// Read teaching points from PLC for a specific port
        /// </summary>
        private void ReadPortTeachingPointsFromPLC(int port, PortTeachingPoints portPoints)
        {
            // Helper function to read a single teaching point (uses existing GetTeachingPointAddresses method)
            TeachingPoint ReadPoint(string pointName)
            {
                var addresses = GetTeachingPointAddresses(port, pointName);
                return new TeachingPoint(
                    PLCKey.ReadInt32(addresses.x),  // X
                    PLCKey.ReadInt32(addresses.y),  // Y
                    PLCKey.ReadInt32(addresses.z),  // Z
                    0,  // RI (not used in current implementation)
                    0,  // RO (not used in current implementation)
                    PLCKey.ReadInt32(addresses.f)   // F
                );
            }

            // Tray Input (OK)
            portPoints.TrayInputXYStart = ReadPoint("TrayInputXYStart");
            portPoints.TrayInputXEnd = ReadPoint("TrayInputXEnd");
            portPoints.TrayInputYEnd = ReadPoint("TrayInputYEnd");
            portPoints.TrayInputZPosition = ReadPoint("TrayInputZ");

            // Tray NG1
            portPoints.TrayNG1XYStart = ReadPoint("TrayNG1XYStart");
            portPoints.TrayNG1XEnd = ReadPoint("TrayNG1XEnd");
            portPoints.TrayNG1YEnd = ReadPoint("TrayNG1YEnd");
            portPoints.TrayNG1ZPosition = ReadPoint("TrayNG1Z");

            // Tray NG2
            portPoints.TrayNG2XYStart = ReadPoint("TrayNG2XYStart");
            portPoints.TrayNG2XEnd = ReadPoint("TrayNG2XEnd");
            portPoints.TrayNG2YEnd = ReadPoint("TrayNG2YEnd");
            portPoints.TrayNG2ZPosition = ReadPoint("TrayNG2Z");

            // Socket
            portPoints.SocketXYPosition = ReadPoint("Socket");
            portPoints.SocketZLoad = ReadPoint("SocketZLoad");
            portPoints.SocketZUnload = ReadPoint("SocketZUnload");
            portPoints.SocketZReady = ReadPoint("SocketZReady");
            portPoints.SocketZReadyLoad = ReadPoint("SocketZReadyLoad");
            portPoints.SocketZReadyUnload = ReadPoint("SocketZReadyUnload");
            portPoints.SocketFOpened = ReadPoint("SocketFOpened");
            portPoints.SocketFClosed = ReadPoint("SocketFClosed");

            // Camera
            portPoints.CameraXYPosition = ReadPoint("Camera");
            portPoints.CameraZPosition = ReadPoint("SocketCameraZ");
        }

        /// <summary>
        /// Write all teaching points to PLC for all 4 ports
        /// </summary>
        private void WriteAllTeachingPointsToPLC(TeachingModel model)
        {
            // Write Port 1
            WritePortTeachingPointsToPLC(1, model.Port1);
            // Write Port 2
            WritePortTeachingPointsToPLC(2, model.Port2);
            // Write Port 3
            WritePortTeachingPointsToPLC(3, model.Port3);
            // Write Port 4
            WritePortTeachingPointsToPLC(4, model.Port4);
        }

        /// <summary>
        /// Write teaching points to PLC for a specific port
        /// </summary>
        private void WritePortTeachingPointsToPLC(int port, PortTeachingPoints portPoints)
        {
            // Helper function to write a single teaching point (uses existing GetTeachingPointAddresses method)
            void WritePoint(string pointName, TeachingPoint point)
            {
                var addresses = GetTeachingPointAddresses(port, pointName);
                PLCKey.WriteInt32(addresses.x, point.X);
                PLCKey.WriteInt32(addresses.y, point.Y);
                PLCKey.WriteInt32(addresses.z, point.Z);
                PLCKey.WriteInt32(addresses.f, point.F);
                // RI and RO are not used in current implementation
            }

            // Tray Input (OK)
            WritePoint("TrayInputXYStart", portPoints.TrayInputXYStart);
            WritePoint("TrayInputXEnd", portPoints.TrayInputXEnd);
            WritePoint("TrayInputYEnd", portPoints.TrayInputYEnd);
            WritePoint("TrayInputZ", portPoints.TrayInputZPosition);

            // Tray NG1
            WritePoint("TrayNG1XYStart", portPoints.TrayNG1XYStart);
            WritePoint("TrayNG1XEnd", portPoints.TrayNG1XEnd);
            WritePoint("TrayNG1YEnd", portPoints.TrayNG1YEnd);
            WritePoint("TrayNG1Z", portPoints.TrayNG1ZPosition);

            // Tray NG2
            WritePoint("TrayNG2XYStart", portPoints.TrayNG2XYStart);
            WritePoint("TrayNG2XEnd", portPoints.TrayNG2XEnd);
            WritePoint("TrayNG2YEnd", portPoints.TrayNG2YEnd);
            WritePoint("TrayNG2Z", portPoints.TrayNG2ZPosition);

            // Socket
            WritePoint("Socket", portPoints.SocketXYPosition);
            WritePoint("SocketZLoad", portPoints.SocketZLoad);
            WritePoint("SocketZUnload", portPoints.SocketZUnload);
            WritePoint("SocketZReady", portPoints.SocketZReady);
            WritePoint("SocketZReadyLoad", portPoints.SocketZReadyLoad);
            WritePoint("SocketZReadyUnload", portPoints.SocketZReadyUnload);
            WritePoint("SocketFOpened", portPoints.SocketFOpened);
            WritePoint("SocketFClosed", portPoints.SocketFClosed);

            // Camera
            WritePoint("Camera", portPoints.CameraXYPosition);
            WritePoint("SocketCameraZ", portPoints.CameraZPosition);
        }

        /// <summary>
        /// Update teaching button colors after loading model
        /// </summary>
        private void UpdateTeachingButtonColors()
        {
            // Set all Save buttons to green to indicate they have values
            var saveButtons = new[]
            {
                "btnSavePointTrayInputXYStart", "btnSavePointTrayInputXEnd",
                "btnSavePointTrayInputYEnd", "btnSavePointTrayInputZ",
                "btnSavePointTrayNG1XYStart", "btnSavePointTrayNG1XEnd",
                "btnSavePointTrayNG1YEnd", "btnSavePointTrayNG1Z",
                "btnSavePointTrayNG2XYStart", "btnSavePointTrayNG2XEnd",
                "btnSavePointTrayNG2YEnd", "btnSavePointTrayNG2Z",
                "btnSavePointSocket", "btnSavePointSocketZLoad",
                "btnSavePointSocketZUnload", "btnSavePointSocketZReady",
                "btnSavePointSocketZReadyLoad", "btnSavePointSocketZReadyUnload",
                "btnSavePointSocketFOpened", "btnSavePointSocketFClosed",
                "btnSavePointCamera", "btnSavePointSocketCameraZ"
            };

            foreach (var btnName in saveButtons)
            {
                var btn = FindButtonByName(btnName);
                if (btn != null)
                {
                    btn.BackColor = System.Drawing.Color.LightGreen;
                }
            }
        }

        /// <summary>
        /// Motor Disable button click handler - Toggle PLC value
        /// </summary>
        private void btnMotorDisable_Click(object sender, EventArgs e)
        {
            ToggleBypassSignal(selectedPort, "Motor");
        }

        /// <summary>
        /// Door Disable button click handler - Toggle PLC value
        /// </summary>
        private void btnDoorDisable_Click(object sender, EventArgs e)
        {
            ToggleBypassSignal(selectedPort, "Door");
        }

        /// <summary>
        /// Dry Run Mode button click handler - Toggle PLC value
        /// </summary>
        private void btnDryRunMode_Click(object sender, EventArgs e)
        {
            ToggleBypassSignal(selectedPort, "DryRun");
        }

        /// <summary>
        /// Tray Disable button click handler - Toggle PLC value
        /// </summary>
        private void btnTrayDisable_Click(object sender, EventArgs e)
        {
            ToggleBypassSignal(selectedPort, "Tray");
        }

        private void btnAutoDisable_Click(object sender, EventArgs e)
        {
            ToggleBypassSignal(selectedPort, "Auto");
        }

        private void btnChartDisable_Click(object sender, EventArgs e)
        {
            ToggleBypassSignal(selectedPort, "Chart");
        }

        /// <summary>
        /// Toggle bypass signal at PLC address
        /// </summary>
        private void ToggleBypassSignal(int port, string signalType)
        {
            var _dialogResult = MessageBox.Show("Bạn có chắc chắn không", "Cảnh báo", MessageBoxButtons.YesNo);
            if (_dialogResult == DialogResult.No)
            {
                return;
            }
            try
            {
                string address = GetBypassAddress(port, signalType);
                if (string.IsNullOrEmpty(address))
                    return;

                bool currentValue = false;
                if (address.Contains('.'))
                {
                    var addrSplit = address.Split('.');
                    currentValue = PLCKey.ReadBitFromWord(addrSplit[0], GetBitIndexFromAddress(address));
                }
                else
                {
                    // Read current value
                    currentValue = PLCKey.ReadBit(address);
                }


                // Toggle value
                if (currentValue)
                {
                    if (address.Contains('.'))
                    {
                        var addrSplit = address.Split('.');
                        currentValue = PLCKey.ResetBitInWord(addrSplit[0], GetBitIndexFromAddress(address));
                    }
                    else
                    {
                        PLCKey.ResetBit(address);
                    }
                }
                else
                {
                    if (address.Contains('.'))
                    {
                        var addrSplit = address.Split('.');
                        currentValue = PLCKey.SetBitInWord(addrSplit[0], GetBitIndexFromAddress(address));
                    }
                    else
                    {
                        PLCKey.SetBit(address);
                    }
                }   

                // Update button color immediately
                UpdateBypassButtonColor(signalType, !currentValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi toggle bypass signal: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Get PLC address for bypass signal based on port and signal type
        /// </summary>
        private string GetBypassAddress(int port, string signalType)
        {
            switch (port)
            {
                case 1:
                    switch (signalType)
                    {
                        case "Motor": return PLCAddresses.Input.P1_Motor_Disable;
                        case "Door": return PLCAddresses.Input.P1_Bypass_Door;
                        case "DryRun": return PLCAddresses.Input.P1_Dry_Run_Mode;
                        case "Tray": return PLCAddresses.Input.P1_Bypass_Sensor_Detect_Tray;
                        case "Auto": return PLCAddresses.Input.P1_Auto_Mode;
                        case "Chart": return PLCAddresses.Input.P1_Chart_Disable;
                    }
                    break;
                case 2:
                    switch (signalType)
                    {
                        case "Motor": return PLCAddresses.Input.P2_Motor_Disable;
                        case "Door": return PLCAddresses.Input.P2_Bypass_Door;
                        case "DryRun": return PLCAddresses.Input.P2_Dry_Run_Mode;
                        case "Tray": return PLCAddresses.Input.P2_Bypass_Sensor_Detect_Tray;
                        case "Auto": return PLCAddresses.Input.P2_Auto_Mode;
                        case "Chart": return PLCAddresses.Input.P2_Chart_Disable;
                    }
                    break;
                case 3:
                    switch (signalType)
                    {
                        case "Motor": return PLCAddresses.Input.P3_Motor_Disable;
                        case "Door": return PLCAddresses.Input.P3_Bypass_Door;
                        case "DryRun": return PLCAddresses.Input.P3_Dry_Run_Mode;
                        case "Tray": return PLCAddresses.Input.P3_Bypass_Sensor_Detect_Tray;
                        case "Auto": return PLCAddresses.Input.P3_Auto_Mode;
                        case "Chart": return PLCAddresses.Input.P3_Chart_Disable;
                    }
                    break;
                case 4:
                    switch (signalType)
                    {
                        case "Motor": return PLCAddresses.Input.P4_Motor_Disable;
                        case "Door": return PLCAddresses.Input.P4_Bypass_Door;
                        case "DryRun": return PLCAddresses.Input.P4_Dry_Run_Mode;
                        case "Tray": return PLCAddresses.Input.P4_Bypass_Sensor_Detect_Tray;
                        case "Auto": return PLCAddresses.Input.P4_Auto_Mode;
                        case "Chart": return PLCAddresses.Input.P4_Chart_Disable;
                    }
                    break;
            }
            return null;
        }

        /// <summary>
        /// Update button color based on signal value
        /// </summary>
        private void UpdateBypassButtonColor(string signalType, bool isActive)
        {
            System.Windows.Forms.Button btn = null;
            switch (signalType)
            {
                case "Motor": btn = btnMotorDisable; break;
                case "Door": btn = btnDoorDisable; break;
                case "DryRun": btn = btnDryRunMode; break;
                case "Tray": btn = btnTrayDisable; break;
                case "Auto": btn = btnAutoDisable; break;
                case "Chart": btn = btnChartDisable; break;
            }

            if (btn != null)
            {
                btn.BackColor = isActive ? System.Drawing.Color.LightGreen : System.Drawing.Color.LightGray;
            }
        }

        /// <summary>
        /// Check if port is in Auto Mode
        /// </summary>
        private bool IsAutoModeActive(int port)
        {
            try
            {
                string autoAddress = GetAutoModeAddress(port);
                if (string.IsNullOrEmpty(autoAddress))
                    return false;

                return PLCKey.ReadBit(autoAddress);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get Auto Mode address for specific port
        /// </summary>
        private string GetAutoModeAddress(int port)
        {
            switch (port)
            {
                case 1: return PLCAddresses.Input.P1_Auto_Mode;
                case 2: return PLCAddresses.Input.P2_Auto_Mode;
                case 3: return PLCAddresses.Input.P3_Auto_Mode;
                case 4: return PLCAddresses.Input.P4_Auto_Mode;
                default: return null;
            }
        }

        /// <summary>
        /// Update all bypass button colors from PLC (called by timer)
        /// </summary>
        private void UpdateBypassButtonColors()
        {
            try
            {
                // Motor Disable
                string motorAddr = GetBypassAddress(selectedPort, "Motor");
                if (!string.IsNullOrEmpty(motorAddr))
                {
                    bool motorValue = PLCKey.ReadBit(motorAddr);
                    UpdateBypassButtonColor("Motor", motorValue);
                }

                // Door Bypass
                string doorAddr = GetBypassAddress(selectedPort, "Door");
                if (!string.IsNullOrEmpty(doorAddr))
                {
                    bool doorValue = PLCKey.ReadBit(doorAddr);
                    UpdateBypassButtonColor("Door", doorValue);
                }

                // Dry Run Mode
                string dryRunAddr = GetBypassAddress(selectedPort, "DryRun");
                if (!string.IsNullOrEmpty(dryRunAddr))
                {
                    bool dryRunValue = PLCKey.ReadBit(dryRunAddr);
                    UpdateBypassButtonColor("DryRun", dryRunValue);
                }

                // Tray Bypass
                string trayAddr = GetBypassAddress(selectedPort, "Tray");
                if (!string.IsNullOrEmpty(trayAddr))
                {
                    bool trayValue = PLCKey.ReadBit(trayAddr);
                    UpdateBypassButtonColor("Tray", trayValue);
                }

                // Auto Bypass
                string autoAddr = GetBypassAddress(selectedPort, "Auto");
                if (!string.IsNullOrEmpty(autoAddr))
                {
                    bool autoValue = PLCKey.ReadBit(autoAddr);
                    UpdateBypassButtonColor("Auto", autoValue);
                }

                // Auto Bypass
                string chartAddr = GetBypassAddress(selectedPort, "Chart");
                if (!string.IsNullOrEmpty(chartAddr))
                {
                    var addrSplit = chartAddr.Split('.');
                    bool chartValue = PLCKey.ReadBitFromWord(addrSplit[0], GetBitIndexFromAddress(chartAddr));
                    UpdateBypassButtonColor("Chart", !chartValue);
                }
            }
            catch
            {
                // Ignore errors in timer update
            }
        }

        #endregion

        #region Data Tab Methods

        /// <summary>
        /// Initialize Data Tab - Setup radio buttons and load initial data
        /// </summary>
        private void InitializeDataTab()
        {
            // Register events for all textboxes in Data tab
            dataTabManager.RegisterTextBoxEvents(tabPage4.Controls);

            // Setup radio buttons for Data tab (need to be added in Designer first)
            // These will be added in the Designer manually
            // When added, uncomment these lines:
            // rbtDataPort1.CheckedChanged += DataPortRadioButton_CheckedChanged;
            // rbtDataPort2.CheckedChanged += DataPortRadioButton_CheckedChanged;
            // rbtDataPort3.CheckedChanged += DataPortRadioButton_CheckedChanged;
            // rbtDataPort4.CheckedChanged += DataPortRadioButton_CheckedChanged;
            // rbtDataPort1.Checked = true;  // Set default to Port 1

            // Load initial data for Port 1
            LoadDataTabValues();
        }

        /// <summary>
        /// Load all data (Speed and Teaching) for currently selected port
        /// </summary>
        private void LoadDataTabValues()
        {
            try
            {
                // Set current port in DataTabManager
                dataTabManager.SetCurrentPort(selectedDataPort);

                // Load Speed data
                dataTabManager.LoadSpeedDataToTextBoxes(tabPage4.Controls);

                // Load Teaching data
                dataTabManager.LoadTeachingDataToTextBoxes(tabPage4.Controls);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc dữ liệu từ PLC: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler when Data Port radio button is changed
        /// This will be connected when radio buttons are added in Designer
        /// </summary>
        private void DataPortRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                // Update selected port based on which radio button was clicked
                if (rb.Name.Contains("1")) selectedDataPort = 1;
                else if (rb.Name.Contains("2")) selectedDataPort = 2;
                else if (rb.Name.Contains("3")) selectedDataPort = 3;
                else if (rb.Name.Contains("4")) selectedDataPort = 4;

                // Load data for new port
                LoadDataTabValues();
            }
        }

        #endregion

    }
}
