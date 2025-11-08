using System;
using System.Drawing;
using System.Windows.Forms;

namespace PLCKeygen
{
    /// <summary>
    /// Custom password dialog với PasswordChar để ẩn mật khẩu
    /// </summary>
    public class ModelNameInputDialog : Form
    {
        private Label lblPrompt;
        private TextBox txtPassword;
        private Button btnOK;
        private Button btnCancel;

        public string Password { get; private set; }

        public ModelNameInputDialog(string prompt = "Nhập mật khẩu:", string title = "Password")
        {
            InitializeComponents(prompt, title);
            Password = string.Empty;
        }

        private void InitializeComponents(string prompt, string title)
        {
            // Form settings
            this.Text = title;
            this.Size = new Size(400, 160);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AcceptButton = btnOK;  // Will be set after creating buttons
            this.CancelButton = btnCancel;  // Will be set after creating buttons

            // Label for prompt
            lblPrompt = new Label();
            lblPrompt.Text = prompt;
            lblPrompt.Location = new Point(20, 20);
            lblPrompt.Size = new Size(350, 20);
            lblPrompt.Font = new Font("Segoe UI", 10);
            this.Controls.Add(lblPrompt);

            // Password TextBox with PasswordChar
            txtPassword = new TextBox();
            txtPassword.Location = new Point(20, 50);
            txtPassword.Size = new Size(350, 25);
            txtPassword.Font = new Font("Segoe UI", 10);
            txtPassword.MaxLength = 20;
            this.Controls.Add(txtPassword);

            // OK Button
            btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Location = new Point(190, 85);
            btnOK.Size = new Size(80, 30);
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Click += (s, e) => { Password = txtPassword.Text; };
            this.Controls.Add(btnOK);

            // Cancel Button
            btnCancel = new Button();
            btnCancel.Text = "Hủy";
            btnCancel.Location = new Point(280, 85);
            btnCancel.Size = new Size(80, 30);
            btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            // Set Accept/Cancel buttons
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            // Focus on password textbox when shown
            this.Shown += (s, e) => { txtPassword.Focus(); };
        }

        /// <summary>
        /// Static method để dễ dàng hiển thị password dialog
        /// </summary>
        /// <param name="prompt">Prompt text</param>
        /// <param name="title">Dialog title</param>
        /// <param name="owner">Parent form</param>
        /// <returns>Password string, hoặc null nếu Cancel</returns>
        public static string Show(string prompt = "Nhập tên model:", string title = "Model", Form owner = null)
        {
            using (var dialog = new ModelNameInputDialog(prompt, title))
            {
                DialogResult result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
                return result == DialogResult.OK ? dialog.Password : null;
            }
        }
    }
}
