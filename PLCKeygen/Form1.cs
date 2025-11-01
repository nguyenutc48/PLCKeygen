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

        public Form1()
        {
            InitializeComponent();
            
            PLCKey = new PLCKeyence("192.168.0.10", 8501);
            PLCKey.Open();
            PLCKey.StartCommunication();
            
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
                if (cameraClient12 == null || !cameraClient12.IsConnected)
                {
                    MessageBox.Show("Chua ket noi camera 12! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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
                if (cameraClient34 == null || !cameraClient34.IsConnected)
                {
                    MessageBox.Show("Chua ket noi camera 12! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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
    }
}
