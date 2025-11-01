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

        public Form1()
        {
            InitializeComponent();

            PLCKey = new PLCKeyence("192.168.0.10", 8501);
            PLCKey.Open();
            PLCKey.StartCommunication();

            // Setup keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            // Setup radio button event handlers
            rbtPort1.CheckedChanged += PortRadioButton_CheckedChanged;
            rbtPort2.CheckedChanged += PortRadioButton_CheckedChanged;
            rbtPort3.CheckedChanged += PortRadioButton_CheckedChanged;
            rbtPort4.CheckedChanged += PortRadioButton_CheckedChanged;

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
            btnXGo.Click += btnAxisGo_Click;
            btnYGo.Click += btnAxisGo_Click;
            btnZGo.Click += btnAxisGo_Click;
            btnRIGo.Click += btnAxisGo_Click;
            btnROGo.Click += btnAxisGo_Click;
            btnFGo.Click += btnAxisGo_Click;

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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (PLCKey.ReadBit("MR5002"))
            {
                PLCKey.ResetBit("MR5002");
                button1.Text = "Sang trai";
            } 
            else
            {
                PLCKey.SetBit("MR5002");
                button1.Text = "Sang phai";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            X1.Text =  (PLCKey.ReadInt32("DM2082")/100.0f).ToString();
            Y1.Text =  (PLCKey.ReadInt32("DM2084")/100.0f).ToString();
            R1.Text =  (PLCKey.ReadInt32("DM2086")/10.0f).ToString();
            X2.Text = (PLCKey.ReadInt32("DM2482") / 100.0f).ToString();
            Y2.Text = (PLCKey.ReadInt32("DM2484") / 100.0f).ToString();
            R2.Text = (PLCKey.ReadInt32("DM2486") / 10.0f).ToString();
            txtXCurMasPort2.Text = (PLCKey.ReadInt32("DM1282") / 100.0f).ToString();
            txtYCurMasPort2.Text = (PLCKey.ReadInt32("DM1284") / 100.0f).ToString();
            txtRCurMasPort2.Text = (PLCKey.ReadInt32("DM1286") / 10.0f).ToString();
            txtXCurMasPort4.Text = (PLCKey.ReadInt32("DM1682") / 100.0f).ToString();
            txtYCurMasPort4.Text = (PLCKey.ReadInt32("DM1684") / 100.0f).ToString();
            txtRCurMasPort4.Text = (PLCKey.ReadInt32("DM1686") / 10.0f).ToString();

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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (PLCKey.ReadBit("MR6002"))
            {
                PLCKey.ResetBit("MR6002");
                button2.Text = "Sang trai";
            }
            else
            {
                PLCKey.SetBit("MR6002");
                button2.Text = "Sang phai";
            }
                
        }

        private void button3_Click(object sender, EventArgs e)
        {
            float x = float.Parse(txtXMasPort1.Text);
            var x1 = x * 100;
            float y = float.Parse(txtYMasPort1.Text);
            var y1 = y * 100;
            float r = float.Parse(txtRMasPort1.Text);
            var r1 = r * 10;

            PLCKey.WriteInt16("DM2082", (short)x1);
            PLCKey.WriteInt16("DM2084", (short)y1);
            PLCKey.WriteInt16("DM2086", (short)r1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            float x = float.Parse(txtXMasPort3.Text);
            var x1 = x * 100;
            float y = float.Parse(txtYMasPort3.Text);
            var y1 = y * 100;
            float r = float.Parse(txtRMasPort3.Text);
            var r1 = r * 10;

            PLCKey.WriteInt16("DM2482", (short)x1);
            PLCKey.WriteInt16("DM2484", (short)y1);
            PLCKey.WriteInt16("DM2486", (short)r1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel2.ToolTipText = "PLC Connected";
            toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
            txtXMasPort1.Text = (PLCKey.ReadInt32("DM2082") / 100.0f).ToString();
            txtYMasPort1.Text = (PLCKey.ReadInt32("DM2084") / 100.0f).ToString();
            txtRMasPort1.Text = (PLCKey.ReadInt32("DM2086") / 10.0f).ToString();
            txtXMasPort3.Text = (PLCKey.ReadInt32("DM2482") / 100.0f).ToString();
            txtYMasPort3.Text = (PLCKey.ReadInt32("DM2484") / 100.0f).ToString();
            txtRMasPort3.Text = (PLCKey.ReadInt32("DM2486") / 10.0f).ToString();
            txtXMasPort2.Text = (PLCKey.ReadInt32("DM1282") / 100.0f).ToString();
            txtYMasPort2.Text = (PLCKey.ReadInt32("DM1284") / 100.0f).ToString();
            txtRMasPort2.Text = (PLCKey.ReadInt32("DM1286") / 10.0f).ToString();
            txtXMasPort4.Text = (PLCKey.ReadInt32("DM1682") / 100.0f).ToString();
            txtYMasPort4.Text = (PLCKey.ReadInt32("DM1684") / 100.0f).ToString();
            txtRMasPort4.Text = (PLCKey.ReadInt32("DM1686") / 10.0f).ToString();
            if (PLCKey.ReadBit("MR5002"))
            {
                button1.Text = "Sang phai";
            }
            else
            {
                button1.Text = "Sang trai";
            }

            if (PLCKey.ReadBit("MR6002"))
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
                    btnConnect12.Text = "Ket noi";
                    btnConnect12.BackColor = Color.LightCoral;
                    cam12.Enabled = true;
                    return;
                }

                cameraClient12 = new CameraTcpClient(ip, port);

                if (cameraClient12.Connect())
                {
                    MessageBox.Show("Ket noi camera thanh cong!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnConnect12.Text = "Da ket noi";
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
                    btnConnect34.Text = "Ket noi";
                    btnConnect34.BackColor = Color.LightCoral;
                    cam34.Enabled = true;
                    return;
                }

                cameraClient34 = new CameraTcpClient(ip, port);



                if (cameraClient34.Connect())
                {
                    MessageBox.Show("Ket noi camera thanh cong!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnConnect34.Text = "Da ket noi";
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

        private async void button7_Click(object sender, EventArgs e)
        {
            if (cameraClient12 == null || !cameraClient12.IsConnected)
            {
                MessageBox.Show("Chua ket noi camera! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await Task.Factory.StartNew(() =>
            {
                PLCKey.SetBit("MR5002");
                Task.Delay(3000);
                try
                {
                    string response = cameraClient12.SendCommand("GCP,1,HOME2D,0,0,0,0,0,0");
                    MessageBox.Show($"Da gui lenh HOME2D!\nPhan hoi: {response}", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Loi khi gui lenh: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            try
            {
                string response = cameraClient12.SendCommand("GCP,1,HOME2D,0,0,0,0,0,0");
                MessageBox.Show($"Da gui lenh HOME2D!\nPhan hoi: {response}", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi khi gui lenh: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (cameraClient34 == null || !cameraClient34.IsConnected)
            {
                MessageBox.Show("Chua ket noi camera! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string response = cameraClient34.SendCommand("GCP,1,HOME2D,0,0,0,0,0,0");
                MessageBox.Show($"Da gui lenh HOME2D!\nPhan hoi: {response}", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi khi gui lenh: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void groupBox20_Enter(object sender, EventArgs e)
        {

        }

        private void btnSetMasP2_Click(object sender, EventArgs e)
        {
            float x = float.Parse(txtXMasPort2.Text);
            var x1 = x * 100;
            float y = float.Parse(txtYMasPort2.Text);
            var y1 = y * 100;
            float r = float.Parse(txtRMasPort2.Text);
            var r1 = r * 10;

            PLCKey.WriteInt16("DM1282", (short)x1);
            PLCKey.WriteInt16("DM1284", (short)y1);
            PLCKey.WriteInt16("DM1286", (short)r1);
        }

        private void btnSetMasP4_Click(object sender, EventArgs e)
        {
            float x = float.Parse(txtXMasPort4.Text);
            var x1 = x * 100;
            float y = float.Parse(txtYMasPort4.Text);
            var y1 = y * 100;
            float r = float.Parse(txtRMasPort4.Text);
            var r1 = r * 10;

            PLCKey.WriteInt16("DM1682", (short)x1);
            PLCKey.WriteInt16("DM1684", (short)y1);
            PLCKey.WriteInt16("DM1686", (short)r1);
        }

        private async Task btnGetMasCamP2_Click(object sender, EventArgs e)
        {
            if (cameraClient12 == null || !cameraClient12.IsConnected)
            {
                MessageBox.Show("Chua ket noi camera! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await Task.Factory.StartNew(() => 
            {
                PLCKey.ResetBit("MR5002");
                Task.Delay(3000);
                try
                {
                    string response = cameraClient12.SendCommand("GCP,1,HOME2D,0,0,0,0,0,0");
                    MessageBox.Show($"Da gui lenh HOME2D!\nPhan hoi: {response}", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Loi khi gui lenh: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } );

            try
            {
                string response = cameraClient12.SendCommand("GCP,1,HOME2D,0,0,0,0,0,0");
                MessageBox.Show($"Da gui lenh HOME2D!\nPhan hoi: {response}", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi khi gui lenh: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnGetMasCamP3_Click(object sender, EventArgs e)
        {
            if (cameraClient34 == null || !cameraClient34.IsConnected)
            {
                MessageBox.Show("Chua ket noi camera! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string response = cameraClient34.SendCommand("GCP,2,HOME2D,0,0,0,0,0,0");
                MessageBox.Show($"Da gui lenh HOME2D!\nPhan hoi: {response}", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi khi gui lenh: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTriggerCamP4_Click(object sender, EventArgs e)
        {
            PLCKey.SetBit("MR10410"); // cho sua lai bien
            while (PLCKey.ReadBit("MR10410")) { Task.Delay(100); };
            MessageBox.Show("nhan Save de luu ket qua");
            var x = PLCKey.ReadInt32("DM800")/10/100.0f;//100
            txtXMasPort4.Text = x.ToString();
            var y = PLCKey.ReadInt32("DM802")/10/100.0f;//100
            txtYMasPort4.Text = y.ToString();
            var r = PLCKey.ReadInt32("DM804")/100/100.0f;//10
            txtRMasPort4.Text = r.ToString();
            //800-804
        }

        private void btnLoadCamP4_Click(object sender, EventArgs e)
        {
            PLCKey.SetBit("MR26003"); // cho sua lai bien
            MessageBox.Show("Da load xong");
        }

        private void btnTriggerCamP3_Click(object sender, EventArgs e)
        {
            PLCKey.SetBit("MR10410"); // cho sua lai bien
            while (PLCKey.ReadBit("MR10410")) { Task.Delay(100); };
            MessageBox.Show("nhan Save de luu ket qua");
            // cho sua lai bien
            var x = PLCKey.ReadInt32("DM810")/10/100.0f;
            txtXMasPort3.Text = x.ToString();
            var y = PLCKey.ReadInt32("DM812")/10/100.0f;
            txtYMasPort3.Text = y.ToString();
            var r = PLCKey.ReadInt32("DM814")/100/100.0f;
            txtRMasPort3.Text = r.ToString();
            //810-814
        }

        private void btnLoadCamP3_Click(object sender, EventArgs e)
        {
            PLCKey.SetBit("MR26003"); // cho sua lai bien
            MessageBox.Show("Da load xong");
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            //port 1
            PLCKey.SetBit("MR10110");
            while (PLCKey.ReadBit("MR10110")) { Task.Delay(100); }
            ;
            MessageBox.Show("nhan Save de luu ket qua");
            var x = PLCKey.ReadInt32("DM780") / 10/100.0f;
            txtXMasPort1.Text = x.ToString();
            var y = PLCKey.ReadInt32("DM782") / 10/100.0f;
            txtYMasPort1.Text= y.ToString();
            var r = PLCKey.ReadInt32("DM784") / 100/100.0f;
            txtRMasPort1.Text = r.ToString();
        }

        private void button44_Click(object sender, EventArgs e)
        {
            PLCKey.SetBit("MR10110"); //dm720/10 dm724/10 dm724/100
            while (PLCKey.ReadBit("MR10110")) { Task.Delay(100); }
            ;
            MessageBox.Show("nhan Save de luu ket qua");
            var x = PLCKey.ReadInt32("DM720") / 10/100.0f;
            txtXMasPort2.Text = x.ToString();
            var y = PLCKey.ReadInt32("DM722") / 10/100.0f;
            txtYMasPort2.Text= y.ToString();
            var r = PLCKey.ReadInt32("DM724") / 100 /100.0f;
            txtRMasPort2.Text= r.ToString();
            //dm1282,1284,1286
        }

        private void groupBox42_Enter(object sender, EventArgs e)
        {
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

        private void button51_Click(object sender, EventArgs e)
        {
            PLCUsageExample.Example2_LoadConfigAndCreatePLC();
            

        }

        private void Job_Plus_Press_Click(object sender, KeyPressEventArgs e)
        {
            MessageBox.Show("hehehe");
        }

        private void Jog_Minus_Press_Click(object sender, KeyPressEventArgs e)
        {
            MessageBox.Show("hihi");
        }
        int a = 0;
        private void Jog_Minus_Down(object sender, KeyEventArgs e)
        {
            a = 1;
        }

        private void Jog_Minus_Up(object sender, KeyEventArgs e)
        {
            a = 2;
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

        // Handeye functions for Camera 1 (Port 2)
        private async void btnCalPos1_Click(object sender, EventArgs e)
        {
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

                // Tinh toan 9 diem 
                // Diem 1: Trung tam (X, Y)
                int x1 = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur);
                int y1 = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur);
                await RunABSPosXYRCam1Async(x1, y1,0);
                XY1Cam1.Text = $"{x1/100.0f:F2},{y1/100.0f:F2}";
                XY1Cam1.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera1(Cmd_Type.Send_Point,0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 1 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                }

                await Task.Delay(1000);

                cmd_result = await RunCmdSendCamera1(Cmd_Type.End);
                if (!cmd_result)
                {
                    MessageBox.Show("Ket thuc handeye khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                MessageBox.Show("Da tinh toan xong 11 diem XY va 2 diem R cho Camera 1!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi khi tinh toan vi tri: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handeye functions for Camera 34 (Port 4)
        private async void btnCalPos2_Click(object sender, EventArgs e)
        {
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

                // Tinh toan 9 diem 
                // Diem 1: Trung tam (X, Y)
                int x1 = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur);
                int y1 = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur);
                await RunABSPosXYRCam2Async(x1, y1, 0);
                XY1Cam2.Text = $"{x1 / 100.0f:F2},{y1 / 100.0f:F2}";
                XY1Cam2.BackColor = Color.Green;

                cmd_result = await RunCmdSendCamera2(Cmd_Type.Send_Point, 0);
                if (!cmd_result)
                {
                    MessageBox.Show("Diem 1 khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                }

                await Task.Delay(1000);

                cmd_result = await RunCmdSendCamera2(Cmd_Type.End);
                if (!cmd_result)
                {
                    MessageBox.Show("Ket thuc handeye khong thanh cong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                MessageBox.Show("Da tinh toan xong 11 diem XY va 2 diem R cho Camera 1!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi khi tinh toan vi tri: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper function to check camera response
        private bool CheckCameraResponse(string expectedCommand, string response, Label labelToUpdate)
        {
            // Phan tich phan hoi tu camera
            // Format: HEB,1 hoac HE,1 hoac HEE,1
            if (string.IsNullOrEmpty(response))
            {
                this.Invoke((MethodInvoker)delegate {
                    labelToUpdate.BackColor = Color.Yellow;
                });
                return false;
            }

            string[] parts = response.Split(',');
            if (parts.Length < 2)
            {
                this.Invoke((MethodInvoker)delegate {
                    labelToUpdate.BackColor = Color.Yellow;
                });
                return false;
            }

            // Kiem tra phan hoi co khop voi lenh gui khong
            string[] expectedParts = expectedCommand.Split(',');
            if (parts[0] != expectedParts[0] || parts[1] != "1")
            {
                this.Invoke((MethodInvoker)delegate {
                    labelToUpdate.BackColor = Color.Yellow;
                });
                return false;
            }

            // Thanh cong - doi mau xanh
            this.Invoke((MethodInvoker)delegate {
                labelToUpdate.BackColor = Color.LightGreen;
            });
            return true;
        }

        private void XY5Cam1_Click(object sender, EventArgs e)
        {

        }

        #region Motion Control - Keyboard and Radio Button Handlers

        // Keyboard shortcut handler
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Port selection (1-4)
            if (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1)
            {
                rbtPort1.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2)
            {
                rbtPort2.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3)
            {
                rbtPort3.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.D4 || e.KeyCode == Keys.NumPad4)
            {
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
            else if (e.KeyCode == Keys.J)
            {
                rbtJog.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.S)
            {
                rbtStep.Checked = true;
                e.Handled = true;
            }
            // JOG control with Up/Down keys
            else if (e.KeyCode == Keys.Up)
            {
                string address = GetJogPlusAddress(selectedPort, selectedAxis);
                if (!string.IsNullOrEmpty(address))
                {
                    PLCKey.SetBit(address);
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
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
            if (e.KeyCode == Keys.Up)
            {
                string address = GetJogPlusAddress(selectedPort, selectedAxis);
                if (!string.IsNullOrEmpty(address))
                {
                    PLCKey.ResetBit(address);
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
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
                if (rb == rbtPort1) selectedPort = 1;
                else if (rb == rbtPort2) selectedPort = 2;
                else if (rb == rbtPort3) selectedPort = 3;
                else if (rb == rbtPort4) selectedPort = 4;

                // Update current position displays for new port
                UpdateCurrentPositionDisplays();
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

        // Get Go (ABS) bit address
        private string GetGoABSAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P1_XGo_ABS;
                        case "Y": return PLCAddresses.Input.P1_YGo_ABS;
                        case "Z": return PLCAddresses.Input.P1_ZGo_ABS;
                        case "RI": return PLCAddresses.Input.P1_RIGo_ABS;
                        case "RO": return PLCAddresses.Input.P1_ROGo_ABS;
                        case "F": return PLCAddresses.Input.P1_FGo_ABS;
                    }
                    break;
                case 2:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P2_XGo_ABS;
                        case "Y": return PLCAddresses.Input.P2_YGo_ABS;
                        case "Z": return PLCAddresses.Input.P2_ZGo_ABS;
                        case "RI": return PLCAddresses.Input.P2_RIGo_ABS;
                        case "RO": return PLCAddresses.Input.P2_ROGo_ABS;
                        case "F": return PLCAddresses.Input.P2_FGo_ABS;
                    }
                    break;
                case 3:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P3_XGo_ABS;
                        case "Y": return PLCAddresses.Input.P3_YGo_ABS;
                        case "Z": return PLCAddresses.Input.P3_ZGo_ABS;
                        case "RI": return PLCAddresses.Input.P3_RIGo_ABS;
                        case "RO": return PLCAddresses.Input.P3_ROGo_ABS;
                        case "F": return PLCAddresses.Input.P3_FGo_ABS;
                    }
                    break;
                case 4:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P4_XGo_ABS;
                        case "Y": return PLCAddresses.Input.P4_YGo_ABS;
                        case "Z": return PLCAddresses.Input.P4_ZGo_ABS;
                        case "RI": return PLCAddresses.Input.P4_RIGo_ABS;
                        case "RO": return PLCAddresses.Input.P4_ROGo_ABS;
                        case "F": return PLCAddresses.Input.P4_FGo_ABS;
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

        // Get Reset bit address
        private string GetResetAddress(int port, string axis)
        {
            switch (port)
            {
                case 1:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P1_XReset;
                        case "Y": return PLCAddresses.Input.P1_YReset;
                        case "Z": return PLCAddresses.Input.P1_ZReset;
                        case "RI": return PLCAddresses.Input.P1_RIReset;
                        case "RO": return PLCAddresses.Input.P1_ROReset;
                        case "F": return PLCAddresses.Input.P1_FReset;
                    }
                    break;
                case 2:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P2_XReset;
                        case "Y": return PLCAddresses.Input.P2_YReset;
                        case "Z": return PLCAddresses.Input.P2_ZReset;
                        case "RI": return PLCAddresses.Input.P2_RIReset;
                        case "RO": return PLCAddresses.Input.P2_ROReset;
                        case "F": return PLCAddresses.Input.P2_FReset;
                    }
                    break;
                case 3:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P3_XReset;
                        case "Y": return PLCAddresses.Input.P3_YReset;
                        case "Z": return PLCAddresses.Input.P3_ZReset;
                        case "RI": return PLCAddresses.Input.P3_RIReset;
                        case "RO": return PLCAddresses.Input.P3_ROReset;
                        case "F": return PLCAddresses.Input.P3_FReset;
                    }
                    break;
                case 4:
                    switch (axis)
                    {
                        case "X": return PLCAddresses.Input.P4_XReset;
                        case "Y": return PLCAddresses.Input.P4_YReset;
                        case "Z": return PLCAddresses.Input.P4_ZReset;
                        case "RI": return PLCAddresses.Input.P4_RIReset;
                        case "RO": return PLCAddresses.Input.P4_ROReset;
                        case "F": return PLCAddresses.Input.P4_FReset;
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

        // Button event handlers for Go buttons
        private void btnAxisGo_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            if (btn == null) return;

            // Determine which axis based on button
            string axis = "";
            System.Windows.Forms.TextBox txtGoPos = null;

            if (btn == btnXGo) { axis = "X"; txtGoPos = txtXGoPos; }
            else if (btn == btnYGo) { axis = "Y"; txtGoPos = txtYGoPos; }
            else if (btn == btnZGo) { axis = "Z"; txtGoPos = txtZGoPos; }
            else if (btn == btnRIGo) { axis = "RI"; txtGoPos = txtRIGoPos; }
            else if (btn == btnROGo) { axis = "RO"; txtGoPos = txtROGoPos; }
            else if (btn == btnFGo) { axis = "F"; txtGoPos = txtFGoPos; }
            else return;

            try
            {
                // Get setpoint address (only for X, Y, RI)
                string setpointAddr = GetABSSetpointAddress(selectedPort, axis);
                if (string.IsNullOrEmpty(setpointAddr))
                {
                    MessageBox.Show($"Axis {axis} does not support ABS movement", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Parse target position
                float targetPos = float.Parse(txtGoPos.Text);
                int targetPosInt;

                // Convert based on axis type
                if (axis == "RI" || axis == "RO")
                    targetPosInt = (int)(targetPos * 10);  // Rotation axes: x10
                else
                    targetPosInt = (int)(targetPos * 100);  // Linear axes: x100

                // Write setpoint
                PLCKey.WriteInt32(setpointAddr, targetPosInt);

                // Delay to ensure write completes
                System.Threading.Thread.Sleep(100);

                // Set Go bit
                string goAddr = GetGoABSAddress(selectedPort, axis);
                if (!string.IsNullOrEmpty(goAddr))
                {
                    PLCKey.SetBit(goAddr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // Button event handlers for Reset buttons
        private void btnAxisReset_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            if (btn == null) return;

            string axis = "";
            if (btn == btnXReset) axis = "X";
            else if (btn == btnYReset) axis = "Y";
            else if (btn == btnZReset) axis = "Z";
            else if (btn == btnRIReset) axis = "RI";
            else if (btn == btnROReset) axis = "RO";
            else if (btn == btnFReset) axis = "F";
            else return;

            string resetAddr = GetResetAddress(selectedPort, axis);
            if (!string.IsNullOrEmpty(resetAddr))
            {
                PLCKey.SetBit(resetAddr);
            }
        }

        #endregion
    }
}
