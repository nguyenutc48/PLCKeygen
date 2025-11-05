using System;
using System.Drawing;
using System.Windows.Forms;

namespace PLCKeygen
{
    /// <summary>
    /// Form hiển thị hướng dẫn phím tắt cho Teaching Mode
    /// </summary>
    public class TeachingHotkeyHelpForm : Form
    {
        private RichTextBox txtHelp;
        private Button btnClose;
        private TeachingHotkeyManager hotkeyManager;

        public TeachingHotkeyHelpForm(TeachingHotkeyManager manager)
        {
            hotkeyManager = manager;
            InitializeComponents();
            LoadHotkeyHelp();
        }

        private void InitializeComponents()
        {
            // Form settings
            this.Text = "Hướng Dẫn Phím Tắt - Teaching Mode";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // RichTextBox for help content
            txtHelp = new RichTextBox();
            txtHelp.Location = new Point(10, 10);
            txtHelp.Size = new Size(660, 500);
            txtHelp.ReadOnly = true;
            txtHelp.Font = new Font("Consolas", 10);
            txtHelp.BackColor = Color.White;
            txtHelp.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtHelp);

            // Close button
            btnClose = new Button();
            btnClose.Text = "Đóng";
            btnClose.Location = new Point(300, 520);
            btnClose.Size = new Size(80, 30);
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadHotkeyHelp()
        {
            txtHelp.Clear();

            // Title
            txtHelp.SelectionFont = new Font("Consolas", 14, FontStyle.Bold);
            txtHelp.SelectionColor = Color.DarkBlue;
            txtHelp.AppendText("HƯỚNG DẪN PHÍM TẮT - TEACHING MODE\n");
            txtHelp.AppendText("═══════════════════════════════════════════════════════════\n\n");

            // Introduction
            txtHelp.SelectionFont = new Font("Consolas", 10, FontStyle.Regular);
            txtHelp.SelectionColor = Color.Black;
            txtHelp.AppendText("Phím tắt chỉ hoạt động khi đang ở chế độ Teaching Mode.\n");
            txtHelp.AppendText("Chuyển sang Teaching Mode bằng cách chọn radio button 'Teaching Mode'.\n\n");

            // Save Points Section
            AddSectionHeader("LƯU ĐIỂM TEACHING (SAVE)");
            txtHelp.AppendText("\n");

            AddHotkeyGroup("Tray Input (OK)", new[] {
                ("Ctrl+1", "Tray Input - XY Start"),
                ("Ctrl+2", "Tray Input - X End"),
                ("Ctrl+3", "Tray Input - Y End"),
                ("Ctrl+4", "Tray Input - Z Position")
            });

            AddHotkeyGroup("Tray NG1", new[] {
                ("Ctrl+Alt+1", "Tray NG1 - XY Start"),
                ("Ctrl+Alt+2", "Tray NG1 - X End"),
                ("Ctrl+Alt+3", "Tray NG1 - Y End"),
                ("Ctrl+Alt+4", "Tray NG1 - Z Position")
            });

            AddHotkeyGroup("Tray NG2", new[] {
                ("Alt+1", "Tray NG2 - XY Start"),
                ("Alt+2", "Tray NG2 - X End"),
                ("Alt+3", "Tray NG2 - Y End"),
                ("Alt+4", "Tray NG2 - Z Position")
            });

            AddHotkeyGroup("Socket", new[] {
                ("F5", "Socket - XY Position"),
                ("F6", "Socket - Z Load"),
                ("F7", "Socket - Z Unload"),
                ("F8", "Socket - Z Ready"),
                ("F9", "Socket - F Opened"),
                ("F10", "Socket - F Closed")
            });

            AddHotkeyGroup("Camera", new[] {
                ("F11", "Camera - XY Position"),
                ("F12", "Camera - Z Position")
            });

            txtHelp.AppendText("\n");

            // Go To Points Section
            AddSectionHeader("DI CHUYỂN ĐÉN ĐIỂM TEACHING (GO)");
            txtHelp.AppendText("\n");

            AddHotkeyGroup("Tray Input (OK)", new[] {
                ("Ctrl+Shift+1", "Go to Tray Input - XY Start"),
                ("Ctrl+Shift+2", "Go to Tray Input - X End"),
                ("Ctrl+Shift+3", "Go to Tray Input - Y End"),
                ("Ctrl+Shift+4", "Go to Tray Input - Z Position")
            });

            AddHotkeyGroup("Tray NG1", new[] {
                ("Ctrl+Alt+Shift+1", "Go to Tray NG1 - XY Start"),
                ("Ctrl+Alt+Shift+2", "Go to Tray NG1 - X End"),
                ("Ctrl+Alt+Shift+3", "Go to Tray NG1 - Y End"),
                ("Ctrl+Alt+Shift+4", "Go to Tray NG1 - Z Position")
            });

            AddHotkeyGroup("Tray NG2", new[] {
                ("Alt+Shift+1", "Go to Tray NG2 - XY Start"),
                ("Alt+Shift+2", "Go to Tray NG2 - X End"),
                ("Alt+Shift+3", "Go to Tray NG2 - Y End"),
                ("Alt+Shift+4", "Go to Tray NG2 - Z Position")
            });

            AddHotkeyGroup("Socket", new[] {
                ("Shift+F5", "Go to Socket - XY"),
                ("Shift+F6", "Go to Socket - Z Load"),
                ("Shift+F7", "Go to Socket - Z Unload"),
                ("Shift+F8", "Go to Socket - Z Ready"),
                ("Shift+F9", "Go to Socket - F Opened"),
                ("Shift+F10", "Go to Socket - F Closed")
            });

            AddHotkeyGroup("Camera", new[] {
                ("Shift+F11", "Go to Camera - XY"),
                ("Shift+F12", "Go to Camera - Z")
            });

            txtHelp.AppendText("\n");

            // Notes Section
            AddSectionHeader("PHÍM TẮT JOG & ĐIỀU KHIỂN");
            txtHelp.SelectionFont = new Font("Consolas", 10, FontStyle.Regular);
            txtHelp.SelectionColor = Color.DarkBlue;
            txtHelp.AppendText("• F1-F4: Chọn Port 1-4 (cả Jog và Teaching Mode)\n");
            txtHelp.AppendText("• Q: Jog Plus (giữ để di chuyển +)\n");
            txtHelp.AppendText("• A: Jog Minus (giữ để di chuyển -)\n");
            txtHelp.AppendText("• X, Y, Z, I, O, F: Chọn trục (X, Y, Z, RI, RO, F)\n");
            txtHelp.AppendText("• Space: Chuyển đổi Jog/Step mode\n");
            txtHelp.AppendText("• Ctrl+H: Hiển thị Help này\n\n");

            AddSectionHeader("GHI CHÚ");
            txtHelp.SelectionFont = new Font("Consolas", 10, FontStyle.Regular);
            txtHelp.SelectionColor = Color.DarkRed;
            txtHelp.AppendText("• Phím tắt teaching dùng Ctrl/Alt+số để tránh xung đột\n");
            txtHelp.AppendText("• Phím tắt lưu (Save): Lưu vị trí hiện tại vào điểm teaching\n");
            txtHelp.AppendText("• Phím tắt di chuyển (Go): Di chuyển đến điểm teaching đã lưu\n");
            txtHelp.AppendText("• Khi ấn phím tắt, button tương ứng sẽ nhấp nháy màu vàng\n");
            txtHelp.AppendText("• Sau khi lưu, button Save sẽ chuyển sang màu xanh lá\n");
            txtHelp.AppendText("• Port hiện tại được chọn sẽ ảnh hưởng đến teaching point\n");
            txtHelp.AppendText("• Thoát khỏi Teaching Mode sẽ reset màu các button Save\n");

            txtHelp.ScrollToCaret();
            txtHelp.SelectionStart = 0;
        }

        private void AddSectionHeader(string header)
        {
            txtHelp.SelectionFont = new Font("Consolas", 11, FontStyle.Bold);
            txtHelp.SelectionColor = Color.DarkGreen;
            txtHelp.AppendText($"\n{header}\n");
            txtHelp.SelectionColor = Color.Gray;
            txtHelp.AppendText(new string('─', 60) + "\n");
        }

        private void AddHotkeyGroup(string groupName, (string key, string description)[] hotkeys)
        {
            txtHelp.SelectionFont = new Font("Consolas", 10, FontStyle.Bold);
            txtHelp.SelectionColor = Color.DarkBlue;
            txtHelp.AppendText($"\n  {groupName}:\n");

            txtHelp.SelectionFont = new Font("Consolas", 10, FontStyle.Regular);
            foreach (var (key, description) in hotkeys)
            {
                txtHelp.SelectionColor = Color.DarkCyan;
                txtHelp.AppendText($"    {key,-18}");
                txtHelp.SelectionColor = Color.Black;
                txtHelp.AppendText($"→  {description}\n");
            }
        }
    }
}
