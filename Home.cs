using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
            ConfigureHomeUi();
            Resize += Home_Resize;
            Shown += Home_Shown;
        }

        private void ConfigureHomeUi()
        {
            MinimumSize = new Size(900, 560);
            BackColor = Color.FromArgb(245, 247, 251);

            panel1.BackColor = BackColor;

            flowLayoutPanel1.BackColor = Color.White;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.Padding = new Padding(20, 12, 20, 12);
            flowLayoutPanel1.Height = 72;

            flowLayoutPanel2.BackColor = Color.FromArgb(24, 34, 52);
            flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel2.Padding = new Padding(0, 16, 0, 16);
            flowLayoutPanel2.WrapContents = false;
            flowLayoutPanel2.Width = 72;

            flowLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel3.BackColor = Color.Transparent;
            flowLayoutPanel3.Padding = new Padding(0, 4, 0, 12);
            flowLayoutPanel3.WrapContents = false;
            flowLayoutPanel3.AutoScroll = true;

            textBox1.BackColor = Color.FromArgb(244, 247, 250);
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.ForeColor = Color.FromArgb(90, 99, 112);

            label1.ForeColor = Color.FromArgb(38, 166, 91);
            label2.Font = new Font("Inter SemiBold", 14F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(31, 41, 55);

            label4.ForeColor = Color.FromArgb(107, 114, 128);
            label5.ForeColor = Color.FromArgb(107, 114, 128);
            label6.ForeColor = Color.FromArgb(107, 114, 128);

            panel2.BackColor = Color.White;
            panel2.Padding = new Padding(8);
            panel3.AutoSize = false;

            button4.FlatStyle = FlatStyle.Flat;
            button4.FlatAppearance.BorderSize = 0;
            button4.BackColor = Color.FromArgb(38, 166, 91);

            StyleNavButton(button1, true);
            StyleNavButton(button2, false);
            StyleNavButton(button3, false);
            StyleIconButton(button6);
            StyleIconButton(button7);
            StyleIconButton(button8);

            UpdateResponsiveLayout();
        }

        private void StyleNavButton(Button button, bool isActive)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = isActive ? Color.FromArgb(38, 166, 91) : Color.FromArgb(44, 58, 80);
            button.Margin = new Padding(10, 6, 10, 6);
            button.Padding = new Padding(10);
        }

        private void StyleIconButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.White;
        }

        private void Home_Shown(object? sender, EventArgs e)
        {
            UpdateResponsiveLayout();
        }

        private void Home_Resize(object? sender, EventArgs e)
        {
            UpdateResponsiveLayout();
        }

        private void UpdateResponsiveLayout()
        {
            int contentLeft = flowLayoutPanel2.Right + 24;
            int contentWidth = Math.Max(420, ClientSize.Width - contentLeft - 24);

            label2.Location = new Point(contentLeft, flowLayoutPanel1.Bottom + 20);
            label4.Location = new Point(contentLeft + 20, label2.Bottom + 24);
            label5.Location = new Point(contentLeft + 180, label4.Top);
            label6.Location = new Point(contentLeft + 360, label4.Top);

            flowLayoutPanel3.Location = new Point(contentLeft, label4.Bottom + 12);
            flowLayoutPanel3.Size = new Size(contentWidth, Math.Max(220, ClientSize.Height - flowLayoutPanel3.Top - 24));

            panel2.Width = Math.Max(420, flowLayoutPanel3.ClientSize.Width - 8);
            panel2.Height = 78;

            label3.Location = new Point(24, 28);
            label7.Location = new Point(Math.Max(150, panel2.Width / 4), 28);
            label8.Location = new Point(Math.Max(290, panel2.Width / 2 - 40), 28);

            button4.Size = new Size(108, 40);
            button4.Location = new Point(panel2.Width - button4.Width - 20, 19);

            panel3.Width = 200;
            panel3.Height = 50;

            int headerReservedWidth =
                flowLayoutPanel1.Padding.Left +
                flowLayoutPanel1.Padding.Right +
                label1.Width +
                button8.Width +
                panel3.Width +
                button6.Width +
                72;

            textBox1.Width = Math.Max(180, flowLayoutPanel1.ClientSize.Width - headerReservedWidth);
            panel3.Margin = new Padding(16, 0, 6, 0);
            button6.Margin = new Padding(0, 10, 0, 0);
            flowLayoutPanel2.Height = Math.Max(0, ClientSize.Height - flowLayoutPanel1.Height);

            int sidebarAvailableHeight = flowLayoutPanel2.Height - flowLayoutPanel2.Padding.Top - flowLayoutPanel2.Padding.Bottom;
            int logoutTopMargin = Math.Max(24, sidebarAvailableHeight - button1.Height - button2.Height - button3.Height - 40);
            button3.Margin = new Padding(10, logoutTopMargin, 10, 0);
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
