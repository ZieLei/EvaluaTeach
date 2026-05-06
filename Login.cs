using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class Login : Form
    {
        private readonly Color placeholderColor = Color.FromArgb(148, 163, 184);
        private readonly Color inputTextColor = Color.FromArgb(30, 41, 59);
        private ComboBox roleSelector = new();

        public Login()
        {
            InitializeComponent();
            ConfigureLoginUi();
        }

        private void ConfigureLoginUi()
        {
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(241, 245, 249);
            Text = "EvaluaTeach Login";

            tableLayoutPanel1.BackColor = BackColor;
            // Match the sign-up page structure so the card stays centered consistently.
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            label3.Text = "EvaluaTeach";
            label3.ForeColor = Color.FromArgb(22, 163, 74);

            panel1.BackColor = Color.White;
            panel1.Padding = new Padding(28);
            panel1.Dock = DockStyle.None;
            panel1.Anchor = AnchorStyles.None;
            panel1.Margin = new Padding(0, 36, 0, 0);
            // Slightly larger card for better readability on wide screens.
            panel1.MaximumSize = new Size(420, 360);
            panel1.MinimumSize = new Size(360, 340);

            // Global back button pinned to the top-left of the window, not inside the card.
            var backBtn = new Button
            {
                Text = "Back",
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(100, 116, 139),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter", 9F),
                Size = new Size(70, 30),
                Location = new Point(24, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            backBtn.Click += (_, _) => Program.NavigateTo(new LandingPage());
            Controls.Add(backBtn);
            backBtn.BringToFront();

            label2.Text = "User ID";
            label2.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(51, 65, 85);

            label1.Text = "Password";
            label1.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(51, 65, 85);

            StyleTextBox(textBox1, "Enter your ID");
            StyleTextBox(textBox2, "Enter your password", true);

            var roleLabel = new Label
            {
                Text = "Login As",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(28, 180)
            };

            roleSelector = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Inter", 10F),
                BackColor = Color.FromArgb(248, 250, 252),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(28, 204),
                Size = new Size(304, 28)
            };
            roleSelector.Items.AddRange(new[] { "Student", "Admin" });
            roleSelector.SelectedIndex = 0;
            roleSelector.SelectedIndexChanged += (_, _) =>
            {
                label2.Text = roleSelector.SelectedIndex == 0 ? "Student ID" : "Admin ID";
                textBox1.Tag = roleSelector.SelectedIndex == 0 ? "Enter your student ID" : "Enter your admin ID";
                if (textBox1.Text == "Enter your ID")
                    textBox1.Text = textBox1.Tag as string ?? "";
            };

            panel1.Controls.Add(roleLabel);
            panel1.Controls.Add(roleSelector);

            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = Color.FromArgb(22, 163, 74);
            button1.ForeColor = Color.White;
            button1.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            button1.Text = "Sign in";
            button1.Size = new Size(140, 38);

            UpdateLoginLayout();
            Resize += Login_Resize;
        }

        private void StyleTextBox(TextBox textBox, string placeholder, bool isPassword = false)
        {
            textBox.BackColor = Color.FromArgb(248, 250, 252);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Inter", 10F, FontStyle.Regular);
            textBox.Tag = placeholder;
            textBox.Text = placeholder;
            textBox.ForeColor = placeholderColor;

            textBox.Enter += (_, _) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = string.Empty;
                    textBox.ForeColor = inputTextColor;
                    if (isPassword)
                    {
                        textBox.UseSystemPasswordChar = true;
                    }
                }
            };

            textBox.Leave += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (isPassword)
                    {
                        textBox.UseSystemPasswordChar = false;
                    }

                    textBox.Text = placeholder;
                    textBox.ForeColor = placeholderColor;
                }
            };
        }

        private void UpdateLoginLayout()
        {
            // Size card; table layout handles centering like the sign-up page.
            int maxWidth = 420;
            int horizontalPadding = 160;
            int targetWidth = Math.Min(maxWidth, ClientSize.Width - horizontalPadding);
            panel1.Size = new Size(targetWidth, 340);

            label2.Location = new Point(28, 60);
            textBox1.Location = new Point(28, label2.Bottom + 10);
            textBox1.Size = new Size(panel1.Width - 56, 32);

            label1.Location = new Point(28, textBox1.Bottom + 18);
            textBox2.Location = new Point(28, label1.Bottom + 10);
            textBox2.Size = new Size(panel1.Width - 56, 32);

            var roleLabel = panel1.Controls.OfType<Label>().FirstOrDefault(l => l.Text == "Login As");
            if (roleLabel != null)
            {
                roleLabel.Location = new Point(28, textBox2.Bottom + 18);
                roleSelector.Location = new Point(28, roleLabel.Bottom + 8);
                roleSelector.Size = new Size(panel1.Width - 56, 28);
                button1.Location = new Point((panel1.Width - button1.Width) / 2, roleSelector.Bottom + 20);
            }
            else
            {
                button1.Location = new Point((panel1.Width - button1.Width) / 2, textBox2.Bottom + 24);
            }
        }

        private void Login_Resize(object? sender, EventArgs e)
        {
            UpdateLoginLayout();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_3(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userId = textBox1.Text.Trim();
            string password = textBox2.Text;

            if (string.IsNullOrWhiteSpace(userId) || userId == "Enter your student ID" || userId == "Enter your admin ID")
            {
                MessageBox.Show("Please enter your ID.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password == "Enter your password")
            {
                MessageBox.Show("Please enter your password.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isAdmin = roleSelector.SelectedIndex == 1;
            UserRole role = isAdmin ? UserRole.Admin : UserRole.Student;

            string name = isAdmin ? "Administrator" : userId;
            string email = isAdmin ? "admin@evalu teach.edu" : $"{userId}@student.edu";
            string meta = isAdmin ? "Administrator" : "Student BSIT";

            SessionStore.Login(userId, name, email, role);
            ProfileStore.UpdateProfile(name, meta, email, userId);

            FormDataStore.SeedSampleData();

            if (isAdmin)
            {
                Program.NavigateTo(new AdminHome());
            }
            else
            {
                Program.NavigateTo(new Home());
            }
        }
    }
}
