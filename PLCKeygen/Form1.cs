using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Task.Factory.StartNew(() => {
                while (true) {
                    
                }
            });

        
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool STATUS = PLCKey.ReadBit("MR112");

            textBox1.AppendText("BIT : " + $"{STATUS}\r\n"); 
            Task.Delay(100);
        }
    }
}
