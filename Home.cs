using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class Home : Form
    {
        private readonly Panel summaryPanel = new();
        private readonly Label summaryTitle = new();
        private readonly Label summaryBody = new();
        private readonly Label metricLabel1 = new();
        private readonly Label metricLabel2 = new();
        private readonly Label metricLabel3 = new();
        private readonly Label sectionSubtitle = new();
        private readonly Panel listContainer = new();
        private readonly Panel notificationsContainer = new();
        private readonly Label notificationsSubtitle = new();
        private readonly Panel notificationCard1 = new();
        private readonly Panel notificationCard2 = new();
        private readonly Panel notificationCard3 = new();
        private readonly Label notificationTitle1 = new();
        private readonly Label notificationTitle2 = new();
        private readonly Label notificationTitle3 = new();
        private readonly Label notificationBody1 = new();
        private readonly Label notificationBody2 = new();
        private readonly Label notificationBody3 = new();
        private readonly Label notificationTime1 = new();
        private readonly Label notificationTime2 = new();
        private readonly Label notificationTime3 = new();

        public Home()
        {
            InitializeComponent();
            ConfigureHomeUi();
            Resize += Home_Resize;
            Shown += Home_Shown;
            // subscribe to profile updates to keep header and avatar in sync
            ProfileStore.ProfileUpdated += OnProfileUpdated;
            // initialize header from store
            OnProfileUpdated();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Open the profile page and transfer saved profile info if available
            ProfilePage profile = new();

            // If the Home view has displayed user details in the header labels, pass them to the profile
            var name = label9?.Text ?? string.Empty;
            var meta = label10?.Text ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(meta))
            {
                profile.SetProfileInfo(name, meta);
            }
            profile.Show();
        }

        private void OnProfileUpdated()
        {
            // Update header labels and avatar from the shared ProfileStore
            if (InvokeRequired)
            {
                Invoke(new Action(OnProfileUpdated));
                return;
            }

            label9.Text = ProfileStore.Name;
            label10.Text = ProfileStore.Meta;

            if (ProfileStore.Avatar != null)
            {
                // show a scaled copy in the header avatar button
                try
                {
                    var avatarCopy = new Bitmap(ProfileStore.Avatar, button7.Size);
                    button7.BackgroundImage = avatarCopy;
                    button7.BackgroundImageLayout = ImageLayout.Zoom;
                    button7.Text = string.Empty;
                }
                catch
                {
                    // ignore image errors
                }

                return;
            }

            button7.BackgroundImage = null;
            button7.Text = GetInitials(ProfileStore.Name);
        }

        private void ConfigureHomeUi()
        {
            MinimumSize = new Size(1100, 720);
            BackColor = Color.FromArgb(245, 247, 251);
            Text = "EvaluaTeach Home";

            panel1.BackColor = BackColor;
            panel1.Padding = new Padding(0, 0, 24, 24);

            flowLayoutPanel1.BackColor = Color.White;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.Padding = new Padding(24, 16, 24, 16);
            flowLayoutPanel1.Height = 84;

            flowLayoutPanel2.BackColor = Color.FromArgb(24, 34, 52);
            flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel2.Padding = new Padding(0, 22, 0, 22);
            flowLayoutPanel2.WrapContents = false;
            flowLayoutPanel2.Width = 84;

            flowLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel3.BackColor = Color.Transparent;
            flowLayoutPanel3.Padding = new Padding(0, 8, 0, 12);
            flowLayoutPanel3.WrapContents = false;
            flowLayoutPanel3.AutoScroll = true;

            textBox1.BackColor = Color.FromArgb(248, 250, 252);
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.ForeColor = Color.FromArgb(90, 99, 112);
            textBox1.Font = new Font("Inter", 10F, FontStyle.Regular);
            textBox1.Text = "Search teachers or subjects";

            label1.ForeColor = Color.FromArgb(38, 166, 91);
            label2.Font = new Font("Inter", 22F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(31, 41, 55);
            label2.Text = "Teacher Evaluation Dashboard";

            label4.ForeColor = Color.FromArgb(100, 116, 139);
            label5.ForeColor = Color.FromArgb(100, 116, 139);
            label6.ForeColor = Color.FromArgb(100, 116, 139);
            label4.Font = new Font("Inter SemiBold", 9F, FontStyle.Bold);
            label5.Font = new Font("Inter SemiBold", 9F, FontStyle.Bold);
            label6.Font = new Font("Inter SemiBold", 9F, FontStyle.Bold);

            panel2.BackColor = Color.White;
            panel2.Padding = new Padding(16);
            panel3.AutoSize = false;
            panel3.BackColor = Color.FromArgb(248, 250, 252);

            button4.FlatStyle = FlatStyle.Flat;
            button4.FlatAppearance.BorderSize = 0;
            button4.BackColor = Color.FromArgb(38, 166, 91);
            button4.Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold);

            StyleNavButton(button1, true);
            StyleNavButton(button2, false);
            StyleNavButton(button3, false);
            StyleIconButton(button6);
            StyleIconButton(button7);
            StyleIconButton(button8);
            StyleProfileAvatarButton();
            ConfigureDashboardPanels();
            StyleTeacherCard();
            ConfigureNotificationsPanel();

            button1.Click += (_, _) => ShowDashboardView();
            button2.Click += (_, _) => ShowNotificationsView();

            ShowDashboardView();
            UpdateResponsiveLayout();
        }

        private void ConfigureDashboardPanels()
        {
            summaryPanel.BackColor = Color.FromArgb(15, 23, 42);
            summaryPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            summaryPanel.Padding = new Padding(24);

            summaryTitle.AutoSize = true;
            summaryTitle.Font = new Font("Inter", 18F, FontStyle.Bold);
            summaryTitle.ForeColor = Color.White;
            summaryTitle.Text = "Welcome back, Mang Juan";

            summaryBody.AutoSize = true;
            summaryBody.Font = new Font("Inter", 10F, FontStyle.Regular);
            summaryBody.ForeColor = Color.FromArgb(203, 213, 225);
            summaryBody.Text = "Track pending evaluations, explore teacher profiles, and submit thoughtful feedback in one clean space.";
            summaryBody.MaximumSize = new Size(620, 0);

            sectionSubtitle.AutoSize = true;
            sectionSubtitle.Font = new Font("Inter", 10F, FontStyle.Regular);
            sectionSubtitle.ForeColor = Color.FromArgb(100, 116, 139);
            sectionSubtitle.Text = "Review the available teachers and start an evaluation whenever you are ready.";

            listContainer.BackColor = Color.White;
            listContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listContainer.Padding = new Padding(20, 18, 20, 20);

            StyleMetric(metricLabel1, "12 Teachers");
            StyleMetric(metricLabel2, "4 Pending");
            StyleMetric(metricLabel3, "1 Completed");

            if (!summaryPanel.Controls.Contains(summaryTitle))
            {
                summaryPanel.Controls.Add(summaryTitle);
                summaryPanel.Controls.Add(summaryBody);
                summaryPanel.Controls.Add(metricLabel1);
                summaryPanel.Controls.Add(metricLabel2);
                summaryPanel.Controls.Add(metricLabel3);
            }

            if (!panel1.Controls.Contains(summaryPanel))
            {
                panel1.Controls.Add(summaryPanel);
                panel1.Controls.Add(sectionSubtitle);
                panel1.Controls.Add(listContainer);
                listContainer.Controls.Add(label4);
                listContainer.Controls.Add(label5);
                listContainer.Controls.Add(label6);
                listContainer.Controls.Add(flowLayoutPanel3);
            }
        }

        private void StyleMetric(Label label, string text)
        {
            label.AutoSize = false;
            label.BackColor = Color.FromArgb(30, 41, 59);
            label.ForeColor = Color.FromArgb(226, 232, 240);
            label.Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Text = text;
        }

        private void StyleTeacherCard()
        {
            label3.Font = new Font("Inter", 12F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(15, 23, 42);

            label7.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            label7.ForeColor = Color.FromArgb(51, 65, 85);

            label8.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            label8.ForeColor = Color.FromArgb(51, 65, 85);

            panel2.BorderStyle = BorderStyle.FixedSingle;
        }

        private void ConfigureNotificationsPanel()
        {
            notificationsSubtitle.AutoSize = true;
            notificationsSubtitle.Font = new Font("Inter", 10F, FontStyle.Regular);
            notificationsSubtitle.ForeColor = Color.FromArgb(100, 116, 139);
            notificationsSubtitle.Text = "Stay updated with teacher reminders, submission activity, and schedule changes.";

            notificationsContainer.BackColor = Color.Transparent;
            notificationsContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            notificationsContainer.AutoScroll = true;

            ConfigureNotificationCard(
                notificationCard1,
                notificationTitle1,
                notificationBody1,
                notificationTime1,
                "You still have 4 pending evaluations",
                "Complete your remaining teacher evaluations before the submission period ends.",
                "5 minutes ago");

            ConfigureNotificationCard(
                notificationCard2,
                notificationTitle2,
                notificationBody2,
                notificationTime2,
                "New teacher schedule was posted",
                "A new evaluation schedule is now available for your current department and year level.",
                "Today, 1:30 PM");

            ConfigureNotificationCard(
                notificationCard3,
                notificationTitle3,
                notificationBody3,
                notificationTime3,
                "Thank you for your last submission",
                "Your evaluation for John Doe was recorded successfully and marked complete.",
                "Yesterday, 3:12 PM");

            if (!panel1.Controls.Contains(notificationsSubtitle))
            {
                panel1.Controls.Add(notificationsSubtitle);
                panel1.Controls.Add(notificationsContainer);
                notificationsContainer.Controls.Add(notificationCard1);
                notificationsContainer.Controls.Add(notificationCard2);
                notificationsContainer.Controls.Add(notificationCard3);
            }
        }

        private void ConfigureNotificationCard(Panel panel, Label title, Label body, Label time, string titleText, string bodyText, string timeText)
        {
            panel.BackColor = Color.White;
            panel.Padding = new Padding(24);
            panel.BorderStyle = BorderStyle.FixedSingle;

            title.AutoSize = true;
            title.Font = new Font("Inter", 12F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(15, 23, 42);
            title.Text = titleText;

            body.AutoSize = true;
            body.Font = new Font("Inter", 10F, FontStyle.Regular);
            body.ForeColor = Color.FromArgb(71, 85, 105);
            body.Text = bodyText;

            time.AutoSize = true;
            time.Font = new Font("Inter SemiBold", 9F, FontStyle.Bold);
            time.ForeColor = Color.FromArgb(100, 116, 139);
            time.Text = timeText;

            if (!panel.Controls.Contains(title))
            {
                panel.Controls.Add(title);
                panel.Controls.Add(body);
                panel.Controls.Add(time);
            }
        }

        private void StyleNavButton(Button button, bool isActive)
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
        }

        private void StyleIconButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button.BackColor = Color.Transparent;
            button.UseVisualStyleBackColor = false;
        }

        private void StyleProfileAvatarButton()
        {
            button7.BackColor = Color.FromArgb(22, 163, 74);
            button7.ForeColor = Color.White;
            button7.Font = new Font("Inter", 12F, FontStyle.Bold);
            button7.TextAlign = ContentAlignment.MiddleCenter;
            button7.BackgroundImage = null;
            button7.Text = GetInitials(label9.Text);
        }

        private static string GetInitials(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "MJ";
            }

            string[] parts = fullName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length == 0)
            {
                return "MJ";
            }

            if (parts.Length == 1)
            {
                return parts[0].Length >= 2
                    ? parts[0][..2].ToUpperInvariant()
                    : parts[0].ToUpperInvariant();
            }

            return string.Concat(
                char.ToUpperInvariant(parts[0][0]),
                char.ToUpperInvariant(parts[^1][0]));
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
            int contentLeft = flowLayoutPanel2.Right + 32;
            int contentWidth = Math.Max(620, ClientSize.Width - contentLeft - 32);
            int contentTop = flowLayoutPanel1.Bottom + 28;
            bool notificationsVisible = notificationsContainer.Visible;

            summaryPanel.Location = new Point(contentLeft, contentTop);
            summaryPanel.Size = new Size(contentWidth, 170);

            summaryTitle.Location = new Point(24, 22);
            summaryBody.Location = new Point(24, summaryTitle.Bottom + 12);

            int metricsTop = summaryBody.Bottom + 18;
            metricLabel1.Location = new Point(24, metricsTop);
            metricLabel2.Location = new Point(160, metricsTop);
            metricLabel3.Location = new Point(296, metricsTop);
            metricLabel1.Size = new Size(120, 34);
            metricLabel2.Size = new Size(120, 34);
            metricLabel3.Size = new Size(120, 34);

            int viewHeaderTop = notificationsVisible
                ? flowLayoutPanel1.Bottom - 4
                : summaryPanel.Bottom + 28;

            label2.Location = new Point(contentLeft, viewHeaderTop);
            sectionSubtitle.Location = new Point(contentLeft, label2.Bottom + 8);
            notificationsSubtitle.Location = new Point(contentLeft, label2.Bottom + 2);

            listContainer.Location = new Point(contentLeft, sectionSubtitle.Bottom + 16);
            listContainer.Size = new Size(contentWidth, Math.Max(260, ClientSize.Height - listContainer.Top - 28));
            int notificationsTop = notificationsSubtitle.Visible
                ? notificationsSubtitle.Bottom + 8
                : label2.Bottom + 10;

            notificationsContainer.Location = new Point(contentLeft, notificationsTop);
            notificationsContainer.Size = new Size(contentWidth, Math.Max(260, ClientSize.Height - notificationsContainer.Top - 28));

            label4.Location = new Point(20, 20);
            label5.Location = new Point(Math.Max(180, listContainer.Width / 3), 20);
            label6.Location = new Point(Math.Max(360, listContainer.Width / 2 + 40), 20);

            flowLayoutPanel3.Location = new Point(20, label4.Bottom + 14);
            flowLayoutPanel3.Size = new Size(listContainer.Width - 40, Math.Max(220, listContainer.Height - flowLayoutPanel3.Top - 20));

            LayoutNotificationCard(notificationCard1, notificationTitle1, notificationBody1, notificationTime1, 0, notificationsContainer.Width);
            LayoutNotificationCard(notificationCard2, notificationTitle2, notificationBody2, notificationTime2, notificationCard1.Bottom + 16, notificationsContainer.Width);
            LayoutNotificationCard(notificationCard3, notificationTitle3, notificationBody3, notificationTime3, notificationCard2.Bottom + 16, notificationsContainer.Width);

            panel2.Width = Math.Max(420, flowLayoutPanel3.ClientSize.Width - 8);
            panel2.Height = 94;

            label3.Location = new Point(22, 34);
            label7.Location = new Point(Math.Max(180, panel2.Width / 3), 34);
            label8.Location = new Point(Math.Max(360, panel2.Width / 2 + 20), 34);

            button4.Size = new Size(118, 42);
            button4.Location = new Point(panel2.Width - button4.Width - 22, 26);

            panel3.Width = 200;
            panel3.Height = 52;
            button7.Size = new Size(46, 46);
            button7.Region = new Region(new Rectangle(0, 0, button7.Width, button7.Height));
            using (GraphicsPath path = new())
            {
                path.AddEllipse(0, 0, button7.Width - 1, button7.Height - 1);
                button7.Region = new Region(path);
            }

            int headerReservedWidth =
                flowLayoutPanel1.Padding.Left +
                flowLayoutPanel1.Padding.Right +
                label1.Width +
                button8.Width +
                panel3.Width +
                button6.Width +
                104;

            textBox1.Width = Math.Max(230, flowLayoutPanel1.ClientSize.Width - headerReservedWidth);
            panel3.Margin = new Padding(16, 0, 6, 0);
            button6.Margin = new Padding(0, 10, 0, 0);
            flowLayoutPanel2.Height = Math.Max(0, ClientSize.Height - flowLayoutPanel1.Height);

            int sidebarAvailableHeight = flowLayoutPanel2.Height - flowLayoutPanel2.Padding.Top - flowLayoutPanel2.Padding.Bottom;
            int logoutTopMargin = Math.Max(40, sidebarAvailableHeight - button1.Height - button2.Height - button3.Height - 56);
            button3.Margin = new Padding(16, logoutTopMargin, 16, 0);
        }

        private void LayoutNotificationCard(Panel panel, Label title, Label body, Label time, int top, int containerWidth)
        {
            panel.Location = new Point(0, top);
            panel.Size = new Size(containerWidth, 118);

            title.Location = new Point(24, 20);
            body.Location = new Point(24, 50);
            body.MaximumSize = new Size(panel.Width - 48, 0);
            time.Location = new Point(24, 82);
        }

        private void ShowDashboardView()
        {
            label2.Text = "Teacher Evaluation Dashboard";
            sectionSubtitle.Visible = true;
            notificationsSubtitle.Visible = false;
            summaryPanel.Visible = true;
            listContainer.Visible = true;
            notificationsContainer.Visible = false;
            UpdateResponsiveLayout();
        }

        private void ShowNotificationsView()
        {
            label2.Text = "Notifications";
            sectionSubtitle.Visible = false;
            notificationsSubtitle.Visible = false;
            summaryPanel.Visible = false;
            listContainer.Visible = false;
            notificationsContainer.Visible = true;
            UpdateResponsiveLayout();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            Program.NavigateTo(new LandingPage());
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
