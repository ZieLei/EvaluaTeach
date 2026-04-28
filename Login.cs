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

        public Login()
        {
            InitializeComponent();
            ConfigureLoginUi();
        }

        private void ConfigureLoginUi()
        {
            MinimumSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(241, 245, 249);
            Text = "EvaluaTeach Login";

            tableLayoutPanel1.BackColor = BackColor;

            label3.Text = "EvaluaTeach";
            label3.ForeColor = Color.FromArgb(22, 163, 74);

            panel1.BackColor = Color.White;
            panel1.Padding = new Padding(28);
            panel1.MaximumSize = new Size(360, 220);
            panel1.MinimumSize = new Size(320, 220);

            label2.Text = "Student ID";
            label2.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(51, 65, 85);

            label1.Text = "Password";
            label1.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(51, 65, 85);

            StyleTextBox(textBox1, "Enter your student ID");
            StyleTextBox(textBox2, "Enter your password", true);

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
            panel1.Size = new Size(Math.Min(360, ClientSize.Width - 120), 220);

            label2.Location = new Point(28, 24);
            textBox1.Location = new Point(28, label2.Bottom + 10);
            textBox1.Size = new Size(panel1.Width - 56, 32);

            label1.Location = new Point(28, textBox1.Bottom + 18);
            textBox2.Location = new Point(28, label1.Bottom + 10);
            textBox2.Size = new Size(panel1.Width - 56, 32);

            button1.Location = new Point((panel1.Width - button1.Width) / 2, textBox2.Bottom + 24);
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
            Program.NavigateTo(new Home());
        }
    }
}
