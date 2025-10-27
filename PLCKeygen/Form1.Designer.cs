namespace PLCKeygen
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.X1 = new System.Windows.Forms.Label();
            this.Y1 = new System.Windows.Forms.Label();
            this.R1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.R2 = new System.Windows.Forms.Label();
            this.Y2 = new System.Windows.Forms.Label();
            this.X2 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.X1W = new System.Windows.Forms.TextBox();
            this.Y1W = new System.Windows.Forms.TextBox();
            this.R1W = new System.Windows.Forms.TextBox();
            this.R2W = new System.Windows.Forms.TextBox();
            this.Y2W = new System.Windows.Forms.TextBox();
            this.X2W = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(23, 34);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 26);
            this.button1.TabIndex = 0;
            this.button1.Text = "Xilanh cam tren";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(23, 86);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(125, 26);
            this.button2.TabIndex = 3;
            this.button2.Text = "Xilanh cam duoi";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(236, 31);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 75);
            this.button3.TabIndex = 7;
            this.button3.Text = "Save";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // X1
            // 
            this.X1.AutoSize = true;
            this.X1.Location = new System.Drawing.Point(13, 34);
            this.X1.Name = "X1";
            this.X1.Size = new System.Drawing.Size(35, 13);
            this.X1.TabIndex = 8;
            this.X1.Text = "label1";
            // 
            // Y1
            // 
            this.Y1.AutoSize = true;
            this.Y1.Location = new System.Drawing.Point(13, 60);
            this.Y1.Name = "Y1";
            this.Y1.Size = new System.Drawing.Size(35, 13);
            this.Y1.TabIndex = 9;
            this.Y1.Text = "label2";
            // 
            // R1
            // 
            this.R1.AutoSize = true;
            this.R1.Location = new System.Drawing.Point(13, 86);
            this.R1.Name = "R1";
            this.R1.Size = new System.Drawing.Size(35, 13);
            this.R1.TabIndex = 10;
            this.R1.Text = "label3";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.R1W);
            this.groupBox1.Controls.Add(this.Y1W);
            this.groupBox1.Controls.Add(this.X1W);
            this.groupBox1.Controls.Add(this.R1);
            this.groupBox1.Controls.Add(this.Y1);
            this.groupBox1.Controls.Add(this.X1);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 131);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Luu offset tang 2";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.R2W);
            this.groupBox2.Controls.Add(this.R2);
            this.groupBox2.Controls.Add(this.Y2W);
            this.groupBox2.Controls.Add(this.X2W);
            this.groupBox2.Controls.Add(this.Y2);
            this.groupBox2.Controls.Add(this.X2);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Location = new System.Drawing.Point(8, 151);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(336, 131);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Luu offset tang 1";
            // 
            // R2
            // 
            this.R2.AutoSize = true;
            this.R2.Location = new System.Drawing.Point(13, 86);
            this.R2.Name = "R2";
            this.R2.Size = new System.Drawing.Size(35, 13);
            this.R2.TabIndex = 10;
            this.R2.Text = "label3";
            // 
            // Y2
            // 
            this.Y2.AutoSize = true;
            this.Y2.Location = new System.Drawing.Point(13, 60);
            this.Y2.Name = "Y2";
            this.Y2.Size = new System.Drawing.Size(35, 13);
            this.Y2.TabIndex = 9;
            this.Y2.Text = "label2";
            // 
            // X2
            // 
            this.X2.AutoSize = true;
            this.X2.Location = new System.Drawing.Point(13, 34);
            this.X2.Name = "X2";
            this.X2.Size = new System.Drawing.Size(35, 13);
            this.X2.TabIndex = 8;
            this.X2.Text = "label1";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(236, 31);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(72, 75);
            this.button4.TabIndex = 7;
            this.button4.Text = "Save";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // X1W
            // 
            this.X1W.Location = new System.Drawing.Point(107, 31);
            this.X1W.Name = "X1W";
            this.X1W.Size = new System.Drawing.Size(90, 20);
            this.X1W.TabIndex = 11;
            // 
            // Y1W
            // 
            this.Y1W.Location = new System.Drawing.Point(107, 53);
            this.Y1W.Name = "Y1W";
            this.Y1W.Size = new System.Drawing.Size(90, 20);
            this.Y1W.TabIndex = 12;
            // 
            // R1W
            // 
            this.R1W.Location = new System.Drawing.Point(107, 79);
            this.R1W.Name = "R1W";
            this.R1W.Size = new System.Drawing.Size(90, 20);
            this.R1W.TabIndex = 13;
            // 
            // R2W
            // 
            this.R2W.Location = new System.Drawing.Point(107, 79);
            this.R2W.Name = "R2W";
            this.R2W.Size = new System.Drawing.Size(90, 20);
            this.R2W.TabIndex = 16;
            // 
            // Y2W
            // 
            this.Y2W.Location = new System.Drawing.Point(107, 53);
            this.Y2W.Name = "Y2W";
            this.Y2W.Size = new System.Drawing.Size(90, 20);
            this.Y2W.TabIndex = 15;
            // 
            // X2W
            // 
            this.X2W.Location = new System.Drawing.Point(107, 31);
            this.X2W.Name = "X2W";
            this.X2W.Size = new System.Drawing.Size(90, 20);
            this.X2W.TabIndex = 14;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Location = new System.Drawing.Point(377, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(180, 131);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Xilanh camera";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(374, 165);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(247, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Dung lenh: GCP,2,HOME2D,0,0,0,0,0,0<CR><LF>";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(374, 189);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(325, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Lay ket qua tra ve cua 3 truc, X,Y,R dien vao 3 o tren cho moi tang";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(737, 485);
            this.tabControl1.TabIndex = 19;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(729, 459);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Master";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(729, 459);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "IO";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.comboBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(729, 459);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Motion";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(138, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 485);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label X1;
        private System.Windows.Forms.Label Y1;
        private System.Windows.Forms.Label R1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label R2;
        private System.Windows.Forms.Label Y2;
        private System.Windows.Forms.Label X2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox R1W;
        private System.Windows.Forms.TextBox Y1W;
        private System.Windows.Forms.TextBox X1W;
        private System.Windows.Forms.TextBox R2W;
        private System.Windows.Forms.TextBox Y2W;
        private System.Windows.Forms.TextBox X2W;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}

