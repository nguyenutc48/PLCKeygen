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
    }
}
