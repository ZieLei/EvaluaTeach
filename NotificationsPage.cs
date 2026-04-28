using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class NotificationsPage : Form
    {
        public NotificationsPage()
        {
            InitializeComponent();
            ConfigureNotificationsUi();
        }

        private void ConfigureNotificationsUi()
        {
            MinimumSize = new Size(1100, 720);
            BackColor = Color.FromArgb(245, 247, 251);
            Text = "EvaluaTeach Notifications";

            containerPanel.BackColor = BackColor;
            containerPanel.Padding = new Padding(0, 0, 24, 24);

            sidebarPanel.BackColor = Color.FromArgb(24, 34, 52);
            sidebarPanel.Padding = new Padding(0, 22, 0, 22);

            headerPanel.BackColor = Color.White;

            labelBrand.Font = new Font("Bebas Neue", 22F, FontStyle.Regular);
            labelBrand.ForeColor = Color.FromArgb(38, 166, 91);

            labelTitle.Font = new Font("Inter", 22F, FontStyle.Bold);
            labelTitle.ForeColor = Color.FromArgb(31, 41, 55);

            labelSubtitle.Font = new Font("Inter", 10F, FontStyle.Regular);
            labelSubtitle.ForeColor = Color.FromArgb(100, 116, 139);

            StyleSidebarButton(buttonHome, false);
            StyleSidebarButton(buttonNotifications, true);
            StyleSidebarButton(buttonLogout, false);

            StyleNotificationCard(cardPanel1, labelCard1Title, labelCard1Body, labelCard1Time);
            StyleNotificationCard(cardPanel2, labelCard2Title, labelCard2Body, labelCard2Time);
            StyleNotificationCard(cardPanel3, labelCard3Title, labelCard3Body, labelCard3Time);

            buttonHome.Click += (_, _) => Program.NavigateTo(new Home());
            buttonNotifications.Click += (_, _) => { };
            buttonLogout.Click += (_, _) => ConfirmLogout();
        }

        private void StyleSidebarButton(Button button, bool isActive)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button.BackColor = Color.Transparent;
            button.Margin = new Padding(16, 8, 16, 8);
            button.Padding = new Padding(6);
            button.Size = new Size(52, 52);
            button.UseVisualStyleBackColor = false;
            button.ForeColor = isActive ? Color.FromArgb(38, 166, 91) : Color.White;

            if (button == buttonHome)
            {
                button.Text = "H";
            }
            else if (button == buttonNotifications)
            {
                button.Text = "N";
            }
            else
            {
                button.Text = "L";
            }

            button.Font = new Font("Inter", 12F, FontStyle.Bold);
        }

        private void StyleNotificationCard(Panel panel, Label title, Label body, Label time)
        {
            panel.BackColor = Color.White;
            panel.Padding = new Padding(24);

            title.Font = new Font("Inter", 12F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(15, 23, 42);

            body.Font = new Font("Inter", 10F, FontStyle.Regular);
            body.ForeColor = Color.FromArgb(71, 85, 105);
            body.MaximumSize = new Size(760, 0);

            time.Font = new Font("Inter SemiBold", 9F, FontStyle.Bold);
            time.ForeColor = Color.FromArgb(100, 116, 139);
        }

        private void ConfirmLogout()
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Program.NavigateTo(new LandingPage());
            }
        }
    }
}
