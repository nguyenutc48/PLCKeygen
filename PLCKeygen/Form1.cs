using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            R1.Text =  (PLCKey.ReadInt32("DM2086")/100.0f).ToString();
            X2.Text = (PLCKey.ReadInt32("DM2482") / 100.0f).ToString();
            Y2.Text = (PLCKey.ReadInt32("DM2484") / 100.0f).ToString();
            R2.Text = (PLCKey.ReadInt32("DM2486") / 100.0f).ToString();
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
            float x = float.Parse(X1W.Text);
            var x1 = x * 100;
            float y = float.Parse(Y1W.Text);
            var y1 = y * 100;
            float r = float.Parse(R1W.Text);
            var r1 = r * 100;

            PLCKey.WriteInt16("DM2082", (short)x1);
            PLCKey.WriteInt16("DM2084", (short)y1);
            PLCKey.WriteInt16("DM2086", (short)r1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            float x = float.Parse(X2W.Text);
            var x1 = x * 100;
            float y = float.Parse(Y2W.Text);
            var y1 = y * 100;
            float r = float.Parse(R2W.Text);
            var r1 = r * 100;

            PLCKey.WriteInt16("DM2482", (short)x1);
            PLCKey.WriteInt16("DM2484", (short)y1);
            PLCKey.WriteInt16("DM2486", (short)r1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            X1W.Text = (PLCKey.ReadInt32("DM2082") / 100.0f).ToString();
            Y1W.Text = (PLCKey.ReadInt32("DM2084") / 100.0f).ToString();
            R1W.Text = (PLCKey.ReadInt32("DM2086") / 100.0f).ToString();
            X2W.Text = (PLCKey.ReadInt32("DM2482") / 100.0f).ToString();
            Y2W.Text = (PLCKey.ReadInt32("DM2484") / 100.0f).ToString();
            R2W.Text = (PLCKey.ReadInt32("DM2486") / 100.0f).ToString();
            if (PLCKey.ReadBit("MR5002"))
            {
                button1.Text = "Sang trai";
            }
            else
            {
                button1.Text = "Sang phai";
            }
            if (PLCKey.ReadBit("MR6002"))
            {
                button2.Text = "Sang trai";
            }
            else
            {
                button2.Text = "Sang phai";
            }
        }
    }
}
