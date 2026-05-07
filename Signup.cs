using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class Signup : Form
    {
        private readonly Color placeholderColor = Color.FromArgb(148, 163, 184);
        private readonly Color inputTextColor = Color.FromArgb(30, 41, 59);

        public Signup()
        {
            InitializeComponent();
            ConfigureSignupUi();
        }

        private void ConfigureSignupUi()
        {
            MinimumSize = new Size(980, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(241, 245, 249);
            Text = "EvaluaTeach Sign Up";

            tableLayoutPanel1.BackColor = BackColor;

            labelTitle.ForeColor = Color.FromArgb(22, 163, 74);

            var topBackBtn = new Button
            {
                Text = "Back",
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(100, 116, 139),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter", 10F),
                Size = new Size(80, 32),
                Location = new Point(24, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            topBackBtn.Click += (_, _) => Program.NavigateTo(new LandingPage());
            Controls.Add(topBackBtn);
            topBackBtn.BringToFront();

            panel1.BackColor = Color.White;
            panel1.Padding = new Padding(24);
            // Keep card centered by the table layout and push it a bit lower.
            panel1.Anchor = AnchorStyles.None;
            panel1.Margin = new Padding(0, 36, 0, 0);

            labelSubtitle.ForeColor = Color.FromArgb(100, 116, 139);
            labelSubtitle.Font = new Font("Inter", 9.5F, FontStyle.Regular);

            StyleLabel(labelStudentId);
            StyleLabel(labelFullName);
            StyleLabel(labelEmail);
            StyleLabel(labelProgram);
            StyleLabel(labelYearLevel);
            StyleLabel(labelSection);
            StyleLabel(labelPassword);
            StyleLabel(labelConfirmPassword);

            StyleTextBox(textBoxStudentId, "e.g. 2024-000123");
            StyleTextBox(textBoxFullName, "Enter your full name");
            StyleTextBox(textBoxEmail, "Enter your school email");
            StyleTextBox(textBoxSection, "e.g. BSIT 2A");
            StyleTextBox(textBoxPassword, "Create a password", true);
            StyleTextBox(textBoxConfirmPassword, "Confirm your password", true);

            StyleComboBox(comboBoxProgram, new[]
            {
                "BSIT",
                "BSCS",
                "BSEd",
                "BEEd",
                "BSBA",
                "BSA",
                "Other"
            });

            StyleComboBox(comboBoxYearLevel, new[]
            {
                "1st Year",
                "2nd Year",
                "3rd Year",
                "4th Year",
                "5th Year"
            });

            buttonCreateAccount.FlatStyle = FlatStyle.Flat;
            buttonCreateAccount.FlatAppearance.BorderSize = 0;
            buttonCreateAccount.BackColor = Color.FromArgb(22, 163, 74);
            buttonCreateAccount.ForeColor = Color.White;
            buttonCreateAccount.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);

            UpdateSignupLayout();
            Resize += Signup_Resize;
        }

        private void StyleLabel(Label label)
        {
            label.Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(51, 65, 85);
        }

        private void StyleTextBox(TextBox textBox, string placeholder, bool isPassword = false)
        {
            textBox.BackColor = Color.FromArgb(248, 250, 252);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Inter", 10F, FontStyle.Regular);
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
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    return;
                }

                if (isPassword)
                {
                    textBox.UseSystemPasswordChar = false;
                }

                textBox.Text = placeholder;
                textBox.ForeColor = placeholderColor;
            };
        }

        private void StyleComboBox(ComboBox comboBox, string[] items)
        {
            comboBox.BackColor = Color.FromArgb(248, 250, 252);
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = new Font("Inter", 10F, FontStyle.Regular);
            comboBox.ForeColor = inputTextColor;
            comboBox.Items.AddRange(items);
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        private void UpdateSignupLayout()
        {
            // Slightly larger and centered sign-up card for better visibility.
            int maxWidth = 520;
            int minWidth = 400;
            int horizontalPadding = 260;
            int targetWidth = Math.Min(maxWidth, Math.Max(minWidth, ClientSize.Width - horizontalPadding));
            panel1.Width = targetWidth;
            panel1.Height = 580;

            int left = 34;
            int fullWidth = panel1.Width - 68;
            int halfWidth = (fullWidth - 12) / 2;

            labelSubtitle.Location = new Point(left, 24);

            labelStudentId.Location = new Point(left, 60);
            textBoxStudentId.Location = new Point(left, 84);
            textBoxStudentId.Size = new Size(fullWidth, 32);

            labelFullName.Location = new Point(left, 122);
            textBoxFullName.Location = new Point(left, 146);
            textBoxFullName.Size = new Size(fullWidth, 32);

            labelEmail.Location = new Point(left, 184);
            textBoxEmail.Location = new Point(left, 208);
            textBoxEmail.Size = new Size(fullWidth, 32);

            labelProgram.Location = new Point(left, 246);
            comboBoxProgram.Location = new Point(left, 270);
            comboBoxProgram.Size = new Size(halfWidth, 33);

            labelYearLevel.Location = new Point(left + halfWidth + 12, 246);
            comboBoxYearLevel.Location = new Point(left + halfWidth + 12, 270);
            comboBoxYearLevel.Size = new Size(halfWidth, 33);

            labelSection.Location = new Point(left, 309);
            textBoxSection.Location = new Point(left, 333);
            textBoxSection.Size = new Size(fullWidth, 32);

            labelPassword.Location = new Point(left, 371);
            textBoxPassword.Location = new Point(left, 395);
            textBoxPassword.Size = new Size(fullWidth, 32);

            labelConfirmPassword.Location = new Point(left, 433);
            textBoxConfirmPassword.Location = new Point(left, 457);
            textBoxConfirmPassword.Size = new Size(fullWidth, 32);

            buttonCreateAccount.Size = new Size(160, 40);
            buttonCreateAccount.Location = new Point((panel1.Width - buttonCreateAccount.Width) / 2, 507);
        }

        private void Signup_Resize(object? sender, EventArgs e)
        {
            UpdateSignupLayout();
        }

        private void buttonCreateAccount_Click(object sender, EventArgs e)
        {
            string studentId = textBoxStudentId.Text.Trim();
            string fullName = textBoxFullName.Text.Trim();
            string email = textBoxEmail.Text.Trim();
            string program = comboBoxProgram.SelectedItem?.ToString() ?? "BSIT";
            string password = textBoxPassword.Text;
            string confirmPassword = textBoxConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(studentId) || studentId == "e.g. 2024-000123")
            {
                MessageBox.Show("Please enter your Student ID.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(fullName) || fullName == "Enter your full name")
            {
                MessageBox.Show("Please enter your full name.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || email == "Enter your school email")
            {
                MessageBox.Show("Please enter your email.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password == "Create a password")
            {
                MessageBox.Show("Please create a password.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string meta = $"Student {program}";
            SessionStore.Login(studentId, null, fullName, email, UserRole.Student);
            ProfileStore.UpdateProfile(fullName, meta, email, studentId);

            FormDataStore.SeedSampleData();

            MessageBox.Show("Account created successfully! Welcome to EvaluaTeach.",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Program.NavigateTo(new Home());
        }
    }
}
