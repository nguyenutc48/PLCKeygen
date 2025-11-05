using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.VisualBasic;

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

        public Form1()
        {
            InitializeComponent();

            this.Text = this.Text +" - "+ Application.ProductVersion.ToString();
            this.Load += Form1_Load;
            this.FormClosing += Closing;
            this.FormClosing += CloseIOport;
            PLCKey = new PLCKeyence("192.168.0.10", 8501);
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

            // Load initial Speed and Step values from PLC
            LoadAxisSpeedAndStep();
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

            // Update IO tab inputs and outputs
            UpdateIOInputs();
            UpdateIOOutputs();
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
            // Port selection (1-4)
            if (e.KeyCode == Keys.F1)
            {
                GetStepJogMode();
                rbtPort1.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F2)
            {
                GetStepJogMode();
                rbtPort2.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                GetStepJogMode();
                rbtPort3.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F4)
            {
                GetStepJogMode();
                rbtPort4.Checked = true;
                e.Handled = true;
            }
            // Axis selection (X, Y, Z, I, O, F)
            else if (e.KeyCode == Keys.X)
            {
                rbtX.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Y)
            {
                rbtY.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Z)
            {
                rbtZ.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.I)
            {
                rbtRI.Checked = true;  // RI
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.O)
            {
                rbtRO.Checked = true;  // RO
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F)
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
                if (rb == rbtPort1)      { selectedPort = 1; selectedIOPort = 1; rbtIOPort1.Checked = true; }
                else if (rb == rbtPort2) { selectedPort = 2; selectedIOPort = 2; rbtIOPort2.Checked = true; }
                else if (rb == rbtPort3) { selectedPort = 3; selectedIOPort = 3; rbtIOPort3.Checked = true; }
                else if (rb == rbtPort4) { selectedPort = 4; selectedIOPort = 4; rbtIOPort4.Checked = true; }

                // Update current position displays for new port
                UpdateCurrentPositionDisplays();

                // Load Speed and Step values for new port
                LoadAxisSpeedAndStep();

                // Load Current Jog Mode
                GetStepJogMode();

                // Immediately update IO displays for new port
                UpdateIOInputs();
                UpdateIOOutputs();
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
        private void IOPortRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                if (rb == rbtIOPort1)      { selectedIOPort = 1; selectedPort = 1;  rbtPort1.Checked = true; }
                else if (rb == rbtIOPort2) { selectedIOPort = 2; selectedPort = 2;  rbtPort2.Checked = true;  }
                else if (rb == rbtIOPort3) { selectedIOPort = 3; selectedPort = 3;  rbtPort3.Checked = true;  }
                else if (rb == rbtIOPort4) { selectedIOPort = 4; selectedPort = 4;  rbtPort4.Checked = true; }

                // Update current position displays for new port
                UpdateCurrentPositionDisplays();

                // Load Speed and Step values for new port
                LoadAxisSpeedAndStep();

                // Load Current Jog Mode
                GetStepJogMode();

                // Immediately update IO displays for new port
                UpdateIOInputs();
                UpdateIOOutputs();
            }
        }

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
                }
                else if (rb == rbtTeachingMode)
                {
                    // Show password dialog when switching to Teaching mode
                    string password = Interaction.InputBox(
                        "Nhập mật khẩu để vào chế độ Teaching:",
                        "Teaching Mode Password",
                        "", -1, -1);

                    if (password == TEACHING_PASSWORD)
                    {
                        // Correct password - enable teaching groups
                        grpTeachingSocket.Enabled = true;
                        grpTeachingTray.Enabled = true;
                    }
                    else
                    {
                        // Incorrect password - show error and revert to Jog mode
                        MessageBox.Show("Mật khẩu không đúng!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        rbtJogMode.Checked = true; // Revert to Jog mode
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
            ledButton.BackColor = bitStatus ? Color.Green : Color.Red;
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
            outputButton.BackColor = bitStatus ? Color.Red : Color.Green;
        }

        // Output button click handler to toggle PLC bits
        private void btnOutputToggle_Click(object sender, EventArgs e)
        {
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
            var dialogResult = MessageBox.Show("Bạn có chắc chắn không","Cảnh báo",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.No) return;
            string addHome = GetHomeAllAddress(selectedPort);
            if (addHome == null) return;
            PLCKey.SetBit(addHome);
            grbJogControl.Enabled = false;
            tstStatus.Text = "Đang về gốc....";
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
            tstStatus.Text = "Ready!";
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
    }
}
