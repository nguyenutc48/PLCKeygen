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
            var sgp = cameraClient34.SendCommand("SGP,1,HOME2D,0,0,0");
            var sgp_split = sgp.Split(',');
            if (sgp_split[0] == "SGP" && sgp_split[1] == "1")
            {
                var a = cameraClient34.SendCommand("GCP,1,HOME2D,0,0,0,0,0,0");
                var b = a.Split(',');
                txtXMasPort4.Text = b[2].Trim();
                txtYMasPort4.Text = b[3].Trim();
                txtRMasPort4.Text = b[4].Trim();
                MessageBox.Show(a);
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
            var sgp = cameraClient12.SendCommand("SGP,1,HOME2D,0,0,0");
            var sgp_split = sgp.Split(',');
            if (sgp_split[0] == "SGP" && sgp_split[1] == "1")
            {
                var a = cameraClient12.SendCommand("GCP,1,HOME2D,0,0,0,0,0,0");
                var b = a.Split(',');
                txtXMasPort2.Text = b[2].Trim();
                txtYMasPort2.Text = b[3].Trim();
                txtRMasPort2.Text = b[4].Trim();
                MessageBox.Show(a);
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
            var sgp = cameraClient12.SendCommand("SGP,2,HOME2D,0,0,0");
            var sgp_split = sgp.Split(',');
            if (sgp_split[0] == "SGP" && sgp_split[1] == "1")
            {
                var a = cameraClient12.SendCommand("GCP,2,HOME2D,0,0,0,0,0,0");
                var b = a.Split(',');
                txtXMasPort1.Text = b[2].Trim();
                txtYMasPort1.Text = b[3].Trim();
                txtRMasPort1.Text = b[4].Trim();
                MessageBox.Show(a);
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
            var sgp = cameraClient34.SendCommand("SGP,2,HOME2D,0,0,0");
            var sgp_split = sgp.Split(',');
            if (sgp_split[0] == "SGP" && sgp_split[1] == "1")
            {
                var a = cameraClient34.SendCommand("GCP,2,HOME2D,0,0,0,0,0,0");
                var b = a.Split(',');
                txtXMasPort3.Text = b[2].Trim();
                txtYMasPort3.Text = b[3].Trim();
                txtRMasPort3.Text = b[4].Trim();
                MessageBox.Show(a);
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

        // Handeye functions for Camera 1 (Port 2)
        private void btnCalPos1_Click(object sender, EventArgs e)
        {
            try
            {
                // Doc toa do hien tai tu PLC (Port 2)
                float curX = PLCKey.ReadInt32(PLCAddresses.Data.P2_X_Pos_Cur) / 100.0f;
                float curY = PLCKey.ReadInt32(PLCAddresses.Data.P2_Y_Pos_Cur) / 100.0f;
                float curRI = PLCKey.ReadInt32(PLCAddresses.Data.P2_RI_Pos_Cur) / 10.0f;

                // Hien thi toa do hien tai
                txtXPosCurrent1.Text = curX.ToString("F2");
                txtYPosCurrent1.Text = curY.ToString("F2");
                txtRPosCurrent1.Text = curRI.ToString("F2");

                // Lay buoc di chuyen
                float stepX = float.Parse(txtXStep1.Text);
                float stepY = float.Parse(txtYStep1.Text);

                // Tinh toan 9 diem
                // Diem 1: Trung tam (X, Y)
                float x1 = curX;
                float y1 = curY;
                XY1Cam1.Text = $"{x1:F2},{y1:F2}";

                // Diem 2: (X+stepX, Y+stepY)
                float x2 = curX + stepX;
                float y2 = curY + stepY;
                XY2Cam1.Text = $"{x2:F2},{y2:F2}";

                // Diem 3: (X+2*stepX, Y+2*stepY)
                float x3 = curX + 2 * stepX;
                float y3 = curY + 2 * stepY;
                XY3Cam1.Text = $"{x3:F2},{y3:F2}";

                // Diem 4: (X+stepX, Y)
                float x4 = curX + stepX;
                float y4 = curY;
                XY4Cam1.Text = $"{x4:F2},{y4:F2}";

                // Diem 5: (X, Y+stepY)
                float x5 = curX;
                float y5 = curY + stepY;
                XY5Cam1.Text = $"{x5:F2},{y5:F2}";

                // Diem 6: (X, Y+2*stepY)
                float x6 = curX;
                float y6 = curY + 2 * stepY;
                XY6Cam1.Text = $"{x6:F2},{y6:F2}";

                // Diem 7: (X-stepX, Y-stepY)
                float x7 = curX - stepX;
                float y7 = curY - stepY;
                XY7Cam1.Text = $"{x7:F2},{y7:F2}";

                // Diem 8: (X+stepX, Y-stepY)
                float x8 = curX + stepX;
                float y8 = curY - stepY;
                XY8Cam1.Text = $"{x8:F2},{y8:F2}";

                // Diem 9: (X+2*stepX, Y-stepY)
                float x9 = curX + 2 * stepX;
                float y9 = curY - stepY;
                XY9Cam1.Text = $"{x9:F2},{y9:F2}";

                // Tinh toan 2 diem R
                float stepR = float.Parse(txtRStep1.Text);

                // XYR1: Quay -stepR do (tru 1 don vi)
                float r1 = curRI - 1;
                XYR1Cam1.Text = $"{x1:F2},{y1:F2},{r1:F2}";

                // XYR2: Quay +stepR do (cong 1 don vi)
                float r2 = curRI + 1;
                XYR2Cam1.Text = $"{x1:F2},{y1:F2},{r2:F2}";

                MessageBox.Show("Da tinh toan xong 9 diem XY va 2 diem R cho Camera 1!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi khi tinh toan vi tri: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handeye functions for Camera 2 (Port 4)
        private void btnCalPos2_Click(object sender, EventArgs e)
        {
            try
            {
                // Doc toa do hien tai tu PLC (Port 4)
                float curX = PLCKey.ReadInt32(PLCAddresses.Data.P4_X_Pos_Cur) / 100.0f;
                float curY = PLCKey.ReadInt32(PLCAddresses.Data.P4_Y_Pos_Cur) / 100.0f;
                float curRI = PLCKey.ReadInt32(PLCAddresses.Data.P4_RI_Pos_Cur) / 10.0f;

                // Hien thi toa do hien tai
                txtXPosCurrent2.Text = curX.ToString("F2");
                txtYPosCurrent2.Text = curY.ToString("F2");
                txtRPosCurrent2.Text = curRI.ToString("F2");

                // Lay buoc di chuyen
                float stepX = float.Parse(txtXStep2.Text);
                float stepY = float.Parse(txtYStep2.Text);

                // Tinh toan 9 diem
                // Diem 1: Trung tam (X, Y)
                float x1 = curX;
                float y1 = curY;
                XY1Cam2.Text = $"{x1:F2},{y1:F2}";

                // Diem 2: (X+stepX, Y+stepY)
                float x2 = curX + stepX;
                float y2 = curY + stepY;
                XY2Cam2.Text = $"{x2:F2},{y2:F2}";

                // Diem 3: (X+2*stepX, Y+2*stepY)
                float x3 = curX + 2 * stepX;
                float y3 = curY + 2 * stepY;
                XY3Cam2.Text = $"{x3:F2},{y3:F2}";

                // Diem 4: (X+stepX, Y)
                float x4 = curX + stepX;
                float y4 = curY;
                XY4Cam2.Text = $"{x4:F2},{y4:F2}";

                // Diem 5: (X, Y+stepY)
                float x5 = curX;
                float y5 = curY + stepY;
                XY5Cam2.Text = $"{x5:F2},{y5:F2}";

                // Diem 6: (X, Y+2*stepY)
                float x6 = curX;
                float y6 = curY + 2 * stepY;
                XY6Cam2.Text = $"{x6:F2},{y6:F2}";

                // Diem 7: (X-stepX, Y-stepY)
                float x7 = curX - stepX;
                float y7 = curY - stepY;
                XY7Cam2.Text = $"{x7:F2},{y7:F2}";

                // Diem 8: (X+stepX, Y-stepY)
                float x8 = curX + stepX;
                float y8 = curY - stepY;
                XY8Cam2.Text = $"{x8:F2},{y8:F2}";

                // Diem 9: (X+2*stepX, Y-stepY)
                float x9 = curX + 2 * stepX;
                float y9 = curY - stepY;
                XY9Cam2.Text = $"{x9:F2},{y9:F2}";

                // Tinh toan 2 diem R
                float stepR = float.Parse(txtRStep2.Text);

                // XYR1: Quay -stepR do (tru 1 don vi)
                float r1 = curRI - 1;
                XYR1Cam2.Text = $"{x1:F2},{y1:F2},{r1:F2}";

                // XYR2: Quay +stepR do (cong 1 don vi)
                float r2 = curRI + 1;
                XYR2Cam2.Text = $"{x1:F2},{y1:F2},{r2:F2}";

                MessageBox.Show("Da tinh toan xong 9 diem XY va 2 diem R cho Camera 2!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Start Handeye process for Camera 1 (Port 2)
        private async void btnStartHandEye1_Click(object sender, EventArgs e)
        {
            try
            {
                if (cameraClient12 == null || !cameraClient12.IsConnected)
                {
                    MessageBox.Show("Chua ket noi camera 12! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Reset flag stop
                stopHandEye1 = false;

                // Reset mau tat ca labels
                XY1Cam1.BackColor = SystemColors.Control;
                XY2Cam1.BackColor = SystemColors.Control;
                XY3Cam1.BackColor = SystemColors.Control;
                XY4Cam1.BackColor = SystemColors.Control;
                XY5Cam1.BackColor = SystemColors.Control;
                XY6Cam1.BackColor = SystemColors.Control;
                XY7Cam1.BackColor = SystemColors.Control;
                XY8Cam1.BackColor = SystemColors.Control;
                XY9Cam1.BackColor = SystemColors.Control;
                XYR1Cam1.BackColor = SystemColors.Control;
                XYR2Cam1.BackColor = SystemColors.Control;

                // Lay toa do da tinh toan tu labels
                Label[] xyLabels = { XY1Cam1, XY2Cam1, XY3Cam1, XY4Cam1, XY5Cam1, XY6Cam1, XY7Cam1, XY8Cam1, XY9Cam1 };
                float[,] points = new float[9, 2];

                // Doc toa do hien tai de gui cho camera (diem 1)
                float curX = 0, curY = 0, curRI = 0;

                // Parse toa do tu labels
                for (int i = 0; i < 9; i++)
                {
                    string[] coords = xyLabels[i].Text.Split(',');
                    if (coords.Length >= 2)
                    {
                        points[i, 0] = float.Parse(coords[0].Trim());
                        points[i, 1] = float.Parse(coords[1].Trim());
                    }
                }

                // Parse toa do R tu labels XYR
                string[] xyr1Coords = XYR1Cam1.Text.Split(',');
                string[] xyr2Coords = XYR2Cam1.Text.Split(',');
                float xyr1_x = 0, xyr1_y = 0, xyr1_r = 0;
                float xyr2_x = 0, xyr2_y = 0, xyr2_r = 0;
                float stepR = 0;

                if (xyr1Coords.Length >= 3 && xyr2Coords.Length >= 3)
                {
                    xyr1_x = float.Parse(xyr1Coords[0].Trim());
                    xyr1_y = float.Parse(xyr1Coords[1].Trim());
                    xyr1_r = float.Parse(xyr1Coords[2].Trim());

                    xyr2_x = float.Parse(xyr2Coords[0].Trim());
                    xyr2_y = float.Parse(xyr2Coords[1].Trim());
                    xyr2_r = float.Parse(xyr2Coords[2].Trim());

                    stepR = float.Parse(txtRStep1.Text);
                }

                // Doc gia tri RI hien tai de gui cho camera (R = 0)
                curRI = PLCKey.ReadInt32(PLCAddresses.Data.P2_RI_Pos_Cur) / 10.0f;

                // Bat dau qua trinh Handeye
                await Task.Run(async () =>
                {
                    try
                    {
                        // Gui lenh bat dau
                        string response = cameraClient12.SendCommand("HEB,1");
                        await Task.Delay(500);

                        // Kiem tra phan hoi HEB,1
                        if (!response.Contains("HEB,1"))
                        {
                            MessageBox.Show("Loi khi bat dau Handeye: Phan hoi khong hop le!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Chay tung diem XY
                        for (int i = 0; i < 9; i++)
                        {
                            // Kiem tra neu nhan Stop
                            if (stopHandEye1)
                            {
                                MessageBox.Show("Da dung qua trinh Handeye Camera 1!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            // Gui toa do tu label xuong PLC
                            short xPlc = (short)(points[i, 0] * 100);
                            short yPlc = (short)(points[i, 1] * 100);
                            short rPlc = (short)(curRI * 10); // Giu nguyen R

                            PLCKey.WriteInt16("DM1282", xPlc);
                            PLCKey.WriteInt16("DM1284", yPlc);
                            PLCKey.WriteInt16("DM1286", rPlc);

                            // Set bit de chay
                            PLCKey.SetBit(PLCAddresses.Input.P2_XGo_ABS);
                            await Task.Delay(100);
                            PLCKey.SetBit(PLCAddresses.Input.P2_YGo_ABS);
                            await Task.Delay(100);

                            // Cho den khi hoan thanh (can them logic check status)
                            await Task.Delay(2000);

                            // Gui lenh cho camera voi toa do tu label
                            string cmd = $"HE,1,{i + 1},{points[i, 0]:F2},{points[i, 1]:F2},0,0,0,0";
                            response = cameraClient12.SendCommand(cmd);
                            await Task.Delay(500);

                            // Kiem tra phan hoi va doi mau
                            if (!CheckCameraResponse("HE,1", response, xyLabels[i]))
                            {
                                MessageBox.Show($"Loi tai diem XY{i + 1}! Dung qua trinh Handeye.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // Delay 1s sau moi diem thanh cong
                            await Task.Delay(1000);
                        }

                        // Kiem tra neu nhan Stop
                        if (stopHandEye1)
                        {
                            MessageBox.Show("Da dung qua trinh Handeye Camera 1!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Xu ly 2 diem R
                        // Diem R1: Quay -stepR do (su dung toa do tu XYR1Cam1)
                        short r1Plc = (short)(xyr1_r * 10);

                        PLCKey.WriteInt16("DM1282", (short)(xyr1_x * 100));
                        PLCKey.WriteInt16("DM1284", (short)(xyr1_y * 100));
                        PLCKey.WriteInt16("DM1286", r1Plc);

                        PLCKey.SetBit(PLCAddresses.Input.P2_RIGo_ABS);
                        await Task.Delay(2000);

                        string cmd2 = $"HE,1,10,{xyr1_x:F2},{xyr1_y:F2},{-stepR:F2},0,0,0";
                        response = cameraClient12.SendCommand(cmd2);
                        await Task.Delay(500);

                        // Kiem tra phan hoi XYR1
                        if (!CheckCameraResponse("HE,1", response, XYR1Cam1))
                        {
                            MessageBox.Show("Loi tai diem XYR1 (R-)! Dung qua trinh Handeye.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Delay 1s sau diem thanh cong
                        await Task.Delay(1000);

                        // Kiem tra neu nhan Stop
                        if (stopHandEye1)
                        {
                            MessageBox.Show("Da dung qua trinh Handeye Camera 1!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Diem R2: Quay +stepR do (su dung toa do tu XYR2Cam1)
                        short r2Plc = (short)(xyr2_r * 10);

                        PLCKey.WriteInt16("DM1282", (short)(xyr2_x * 100));
                        PLCKey.WriteInt16("DM1284", (short)(xyr2_y * 100));
                        PLCKey.WriteInt16("DM1286", r2Plc);

                        PLCKey.SetBit(PLCAddresses.Input.P2_RIGo_ABS);
                        await Task.Delay(2000);

                        cmd2 = $"HE,1,11,{xyr2_x:F2},{xyr2_y:F2},{stepR:F2},0,0,0";
                        response = cameraClient12.SendCommand(cmd2);
                        await Task.Delay(500);

                        // Kiem tra phan hoi XYR2
                        if (!CheckCameraResponse("HE,1", response, XYR2Cam1))
                        {
                            MessageBox.Show("Loi tai diem XYR2 (R+)! Dung qua trinh Handeye.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Delay 1s sau diem thanh cong
                        await Task.Delay(1000);

                        // Ket thuc
                        response = cameraClient12.SendCommand("HEE,1");

                        // Kiem tra phan hoi HEE,1
                        if (!response.Contains("HEE,1"))
                        {
                            MessageBox.Show("Loi khi ket thuc Handeye: Phan hoi khong hop le!", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Hoan thanh qua trinh Handeye cho Camera 1!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Loi trong qua trinh Handeye: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Stop Handeye process for Camera 1
        private void btnStopHandEye1_Click(object sender, EventArgs e)
        {
            stopHandEye1 = true;
        }

        // Start Handeye process for Camera 2 (Port 4)
        private async void btnStartHandEye2_Click(object sender, EventArgs e)
        {
            try
            {
                if (cameraClient34 == null || !cameraClient34.IsConnected)
                {
                    MessageBox.Show("Chua ket noi camera 34! Vui long ket noi truoc.", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Reset flag stop
                stopHandEye2 = false;

                // Reset mau tat ca labels
                XY1Cam2.BackColor = SystemColors.Control;
                XY2Cam2.BackColor = SystemColors.Control;
                XY3Cam2.BackColor = SystemColors.Control;
                XY4Cam2.BackColor = SystemColors.Control;
                XY5Cam2.BackColor = SystemColors.Control;
                XY6Cam2.BackColor = SystemColors.Control;
                XY7Cam2.BackColor = SystemColors.Control;
                XY8Cam2.BackColor = SystemColors.Control;
                XY9Cam2.BackColor = SystemColors.Control;
                XYR1Cam2.BackColor = SystemColors.Control;
                XYR2Cam2.BackColor = SystemColors.Control;

                // Lay toa do da tinh toan tu labels
                Label[] xyLabels = { XY1Cam2, XY2Cam2, XY3Cam2, XY4Cam2, XY5Cam2, XY6Cam2, XY7Cam2, XY8Cam2, XY9Cam2 };
                float[,] points = new float[9, 2];

                // Doc toa do hien tai de gui cho camera (diem 1)
                float curX = 0, curY = 0, curRI = 0;

                // Parse toa do tu labels
                for (int i = 0; i < 9; i++)
                {
                    string[] coords = xyLabels[i].Text.Split(',');
                    if (coords.Length >= 2)
                    {
                        points[i, 0] = float.Parse(coords[0].Trim());
                        points[i, 1] = float.Parse(coords[1].Trim());
                    }
                }

                // Parse toa do R tu labels XYR
                string[] xyr1Coords = XYR1Cam2.Text.Split(',');
                string[] xyr2Coords = XYR2Cam2.Text.Split(',');
                float xyr1_x = 0, xyr1_y = 0, xyr1_r = 0;
                float xyr2_x = 0, xyr2_y = 0, xyr2_r = 0;
                float stepR = 0;

                if (xyr1Coords.Length >= 3 && xyr2Coords.Length >= 3)
                {
                    xyr1_x = float.Parse(xyr1Coords[0].Trim());
                    xyr1_y = float.Parse(xyr1Coords[1].Trim());
                    xyr1_r = float.Parse(xyr1Coords[2].Trim());

                    xyr2_x = float.Parse(xyr2Coords[0].Trim());
                    xyr2_y = float.Parse(xyr2Coords[1].Trim());
                    xyr2_r = float.Parse(xyr2Coords[2].Trim());

                    stepR = float.Parse(txtRStep2.Text);
                }

                // Doc gia tri RI hien tai de gui cho camera (R = 0)
                curRI = PLCKey.ReadInt32(PLCAddresses.Data.P4_RI_Pos_Cur) / 10.0f;

                // Bat dau qua trinh Handeye
                await Task.Run(async () =>
                {
                    try
                    {
                        // Gui lenh bat dau
                        string response = cameraClient34.SendCommand("HEB,1");
                        await Task.Delay(500);

                        // Kiem tra phan hoi HEB,1
                        if (!response.Contains("HEB,1"))
                        {
                            MessageBox.Show("Loi khi bat dau Handeye: Phan hoi khong hop le!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Chay tung diem XY
                        for (int i = 0; i < 9; i++)
                        {
                            // Kiem tra neu nhan Stop
                            if (stopHandEye2)
                            {
                                MessageBox.Show("Da dung qua trinh Handeye Camera 2!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            // Gui toa do tu label xuong PLC
                            short xPlc = (short)(points[i, 0] * 100);
                            short yPlc = (short)(points[i, 1] * 100);
                            short rPlc = (short)(curRI * 10); // Giu nguyen R

                            PLCKey.WriteInt16("DM1682", xPlc);
                            PLCKey.WriteInt16("DM1684", yPlc);
                            PLCKey.WriteInt16("DM1686", rPlc);

                            // Set bit de chay
                            PLCKey.SetBit(PLCAddresses.Input.P4_XGo_ABS);
                            await Task.Delay(100);
                            PLCKey.SetBit(PLCAddresses.Input.P4_YGo_ABS);
                            await Task.Delay(100);

                            // Cho den khi hoan thanh (can them logic check status)
                            await Task.Delay(2000);

                            // Gui lenh cho camera voi toa do tu label
                            string cmd = $"HE,1,{i + 1},{points[i, 0]:F2},{points[i, 1]:F2},0,0,0,0";
                            response = cameraClient34.SendCommand(cmd);
                            await Task.Delay(500);

                            // Kiem tra phan hoi va doi mau
                            if (!CheckCameraResponse("HE,1", response, xyLabels[i]))
                            {
                                MessageBox.Show($"Loi tai diem XY{i + 1}! Dung qua trinh Handeye.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // Delay 1s sau moi diem thanh cong
                            await Task.Delay(1000);
                        }

                        // Kiem tra neu nhan Stop
                        if (stopHandEye2)
                        {
                            MessageBox.Show("Da dung qua trinh Handeye Camera 2!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Xu ly 2 diem R
                        // Diem R1: Quay -stepR do (su dung toa do tu XYR1Cam2)
                        short r1Plc = (short)(xyr1_r * 10);

                        PLCKey.WriteInt16("DM1682", (short)(xyr1_x * 100));
                        PLCKey.WriteInt16("DM1684", (short)(xyr1_y * 100));
                        PLCKey.WriteInt16("DM1686", r1Plc);

                        PLCKey.SetBit(PLCAddresses.Input.P4_RIGo_ABS);
                        await Task.Delay(2000);

                        string cmd2 = $"HE,1,10,{xyr1_x:F2},{xyr1_y:F2},{-stepR:F2},0,0,0";
                        response = cameraClient34.SendCommand(cmd2);
                        await Task.Delay(500);

                        // Kiem tra phan hoi XYR1
                        if (!CheckCameraResponse("HE,1", response, XYR1Cam2))
                        {
                            MessageBox.Show("Loi tai diem XYR1 (R-)! Dung qua trinh Handeye.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Delay 1s sau diem thanh cong
                        await Task.Delay(1000);

                        // Kiem tra neu nhan Stop
                        if (stopHandEye2)
                        {
                            MessageBox.Show("Da dung qua trinh Handeye Camera 2!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Diem R2: Quay +stepR do (su dung toa do tu XYR2Cam2)
                        short r2Plc = (short)(xyr2_r * 10);

                        PLCKey.WriteInt16("DM1682", (short)(xyr2_x * 100));
                        PLCKey.WriteInt16("DM1684", (short)(xyr2_y * 100));
                        PLCKey.WriteInt16("DM1686", r2Plc);

                        PLCKey.SetBit(PLCAddresses.Input.P4_RIGo_ABS);
                        await Task.Delay(2000);

                        cmd2 = $"HE,1,11,{xyr2_x:F2},{xyr2_y:F2},{stepR:F2},0,0,0";
                        response = cameraClient34.SendCommand(cmd2);
                        await Task.Delay(500);

                        // Kiem tra phan hoi XYR2
                        if (!CheckCameraResponse("HE,1", response, XYR2Cam2))
                        {
                            MessageBox.Show("Loi tai diem XYR2 (R+)! Dung qua trinh Handeye.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Delay 1s sau diem thanh cong
                        await Task.Delay(1000);

                        // Ket thuc
                        response = cameraClient34.SendCommand("HEE,1");

                        // Kiem tra phan hoi HEE,1
                        if (!response.Contains("HEE,1"))
                        {
                            MessageBox.Show("Loi khi ket thuc Handeye: Phan hoi khong hop le!", "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Hoan thanh qua trinh Handeye cho Camera 2!", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Loi trong qua trinh Handeye: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi: {ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Stop Handeye process for Camera 2
        private void btnStopHandEye2_Click(object sender, EventArgs e)
        {
            stopHandEye2 = true;
        }
    }
}
