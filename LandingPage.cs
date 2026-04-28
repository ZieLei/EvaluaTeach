using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class LandingPage : Form
    {
        public LandingPage()
        {
            InitializeComponent();
            ConfigureLandingUi();
        }

        private void ConfigureLandingUi()
        {
            MinimumSize = new Size(1180, 720);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EvaluaTeach";
            BackColor = Color.FromArgb(239, 246, 255);

            mainLayout.BackColor = BackColor;
            topBarPanel.BackColor = Color.Transparent;
            heroPanel.BackColor = Color.FromArgb(15, 23, 42);

            labelBrand.Font = new Font("Bebas Neue", 22F, FontStyle.Regular);
            labelBrand.ForeColor = Color.FromArgb(22, 163, 74);

            buttonLogin.FlatStyle = FlatStyle.Flat;
            buttonLogin.FlatAppearance.BorderSize = 0;
            buttonLogin.BackColor = Color.White;
            buttonLogin.ForeColor = Color.FromArgb(15, 23, 42);
            buttonLogin.Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold);
            buttonLogin.Click += (_, _) => Program.NavigateTo(new Login());

            buttonSignup.FlatStyle = FlatStyle.Flat;
            buttonSignup.FlatAppearance.BorderSize = 0;
            buttonSignup.BackColor = Color.FromArgb(22, 163, 74);
            buttonSignup.ForeColor = Color.White;
            buttonSignup.Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold);
            buttonSignup.Click += (_, _) => Program.NavigateTo(new Signup());

            labelBadge.BackColor = Color.FromArgb(30, 41, 59);
            labelBadge.ForeColor = Color.FromArgb(134, 239, 172);
            labelBadge.Font = new Font("Inter SemiBold", 9F, FontStyle.Bold);
            labelBadge.Padding = new Padding(12, 6, 12, 6);

            labelHeadline.Font = new Font("Inter", 26F, FontStyle.Bold);
            labelHeadline.ForeColor = Color.White;
            labelHeadline.MaximumSize = new Size(620, 0);

            labelSubheadline.Font = new Font("Inter", 11F, FontStyle.Regular);
            labelSubheadline.ForeColor = Color.FromArgb(203, 213, 225);
            labelSubheadline.MaximumSize = new Size(620, 0);

            StylePrimaryButton(buttonGetStarted, Color.FromArgb(22, 163, 74), Color.White);
            buttonGetStarted.Click += (_, _) => Program.NavigateTo(new Signup());

            StyleStatsPanel();
            StyleFeatureCard(cardPanel1, labelCard1Title, labelCard1Body);
            StyleFeatureCard(cardPanel2, labelCard2Title, labelCard2Body);
            StyleFeatureCard(cardPanel3, labelCard3Title, labelCard3Body);

            UpdateLandingLayout();
            Resize += LandingPage_Resize;
        }

        private void StylePrimaryButton(Button button, Color backColor, Color foreColor)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
        }

        private void StyleStatsPanel()
        {
            panelStats.BackColor = Color.FromArgb(30, 41, 59);

            StyleStatLabel(labelStats1);
            StyleStatLabel(labelStats2);
            StyleStatLabel(labelStats3);
        }

        private void StyleStatLabel(Label label)
        {
            label.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(226, 232, 240);
        }

        private void StyleFeatureCard(Panel panel, Label title, Label body)
        {
            panel.BackColor = Color.White;

            title.Font = new Font("Inter", 11F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(15, 23, 42);

            body.Font = new Font("Inter", 9.5F, FontStyle.Regular);
            body.ForeColor = Color.FromArgb(71, 85, 105);
            body.MaximumSize = new Size(220, 0);
        }

        private void UpdateLandingLayout()
        {
            topBarPanel.Padding = new Padding(0, 6, 0, 6);
            heroPanel.Padding = new Padding(36);

            buttonLogin.Location = new Point(topBarPanel.Width - buttonLogin.Width, 6);
            buttonSignup.Location = new Point(buttonLogin.Left - buttonSignup.Width - 12, 6);

            int contentLeft = 24;
            int contentTop = 24;
            int contentWidth = heroPanel.ClientSize.Width - (contentLeft * 2);
            int cardGap = 24;
            int cardWidth = (contentWidth - (cardGap * 2)) / 3;

            labelBadge.Location = new Point(contentLeft, contentTop);
            labelHeadline.Location = new Point(contentLeft, labelBadge.Bottom + 20);
            labelSubheadline.Location = new Point(contentLeft, labelHeadline.Bottom + 20);

            buttonGetStarted.Location = new Point(contentLeft, labelSubheadline.Bottom + 28);

            panelStats.Location = new Point(contentLeft, buttonGetStarted.Bottom + 30);
            panelStats.Width = Math.Min(620, contentWidth);
            panelStats.Height = 92;

            cardPanel1.Width = cardWidth;
            cardPanel2.Width = cardWidth;
            cardPanel3.Width = cardWidth;
            cardPanel1.Height = 138;
            cardPanel2.Height = 138;
            cardPanel3.Height = 138;

            int cardsTop = panelStats.Bottom + 28;
            cardPanel1.Location = new Point(contentLeft, cardsTop);
            cardPanel2.Location = new Point(cardPanel1.Right + cardGap, cardsTop);
            cardPanel3.Location = new Point(cardPanel2.Right + cardGap, cardsTop);

            labelCard1Body.MaximumSize = new Size(cardPanel1.Width - 40, 0);
            labelCard2Body.MaximumSize = new Size(cardPanel2.Width - 40, 0);
            labelCard3Body.MaximumSize = new Size(cardPanel3.Width - 40, 0);
        }

        private void LandingPage_Resize(object? sender, EventArgs e)
        {
            UpdateLandingLayout();
        }
    }
}
