namespace EvaluaTeach
{
    partial class NotificationsPage
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
            containerPanel = new Panel();
            sidebarPanel = new FlowLayoutPanel();
            buttonHome = new Button();
            buttonNotifications = new Button();
            buttonLogout = new Button();
            headerPanel = new Panel();
            labelBrand = new Label();
            labelTitle = new Label();
            labelSubtitle = new Label();
            cardPanel1 = new Panel();
            labelCard1Title = new Label();
            labelCard1Body = new Label();
            labelCard1Time = new Label();
            cardPanel2 = new Panel();
            labelCard2Title = new Label();
            labelCard2Body = new Label();
            labelCard2Time = new Label();
            cardPanel3 = new Panel();
            labelCard3Title = new Label();
            labelCard3Body = new Label();
            labelCard3Time = new Label();
            containerPanel.SuspendLayout();
            sidebarPanel.SuspendLayout();
            headerPanel.SuspendLayout();
            cardPanel1.SuspendLayout();
            cardPanel2.SuspendLayout();
            cardPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // containerPanel
            // 
            containerPanel.Controls.Add(cardPanel3);
            containerPanel.Controls.Add(cardPanel2);
            containerPanel.Controls.Add(cardPanel1);
            containerPanel.Controls.Add(labelSubtitle);
            containerPanel.Controls.Add(labelTitle);
            containerPanel.Controls.Add(headerPanel);
            containerPanel.Controls.Add(sidebarPanel);
            containerPanel.Dock = DockStyle.Fill;
            containerPanel.Location = new Point(0, 0);
            containerPanel.Name = "containerPanel";
            containerPanel.Size = new Size(1100, 720);
            containerPanel.TabIndex = 0;
            // 
            // sidebarPanel
            // 
            sidebarPanel.Controls.Add(buttonHome);
            sidebarPanel.Controls.Add(buttonNotifications);
            sidebarPanel.Controls.Add(buttonLogout);
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.FlowDirection = FlowDirection.TopDown;
            sidebarPanel.Location = new Point(0, 0);
            sidebarPanel.Name = "sidebarPanel";
            sidebarPanel.Size = new Size(84, 720);
            sidebarPanel.TabIndex = 0;
            sidebarPanel.WrapContents = false;
            // 
            // buttonHome
            // 
            buttonHome.Location = new Point(3, 3);
            buttonHome.Name = "buttonHome";
            buttonHome.Size = new Size(52, 52);
            buttonHome.TabIndex = 0;
            buttonHome.UseVisualStyleBackColor = true;
            // 
            // buttonNotifications
            // 
            buttonNotifications.Location = new Point(3, 61);
            buttonNotifications.Name = "buttonNotifications";
            buttonNotifications.Size = new Size(52, 52);
            buttonNotifications.TabIndex = 1;
            buttonNotifications.UseVisualStyleBackColor = true;
            // 
            // buttonLogout
            // 
            buttonLogout.Location = new Point(3, 119);
            buttonLogout.Name = "buttonLogout";
            buttonLogout.Size = new Size(52, 52);
            buttonLogout.TabIndex = 2;
            buttonLogout.UseVisualStyleBackColor = true;
            // 
            // headerPanel
            // 
            headerPanel.Controls.Add(labelBrand);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(84, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(1016, 84);
            headerPanel.TabIndex = 1;
            // 
            // labelBrand
            // 
            labelBrand.AutoSize = true;
            labelBrand.Location = new Point(28, 22);
            labelBrand.Name = "labelBrand";
            labelBrand.Size = new Size(104, 20);
            labelBrand.TabIndex = 0;
            labelBrand.Text = "EvaluaTeach";
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Location = new Point(118, 118);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(98, 20);
            labelTitle.TabIndex = 2;
            labelTitle.Text = "Notifications";
            // 
            // labelSubtitle
            // 
            labelSubtitle.AutoSize = true;
            labelSubtitle.Location = new Point(118, 154);
            labelSubtitle.Name = "labelSubtitle";
            labelSubtitle.Size = new Size(371, 20);
            labelSubtitle.TabIndex = 3;
            labelSubtitle.Text = "Stay updated with teacher reminders and evaluation activity.";
            // 
            // cardPanel1
            // 
            cardPanel1.Controls.Add(labelCard1Title);
            cardPanel1.Controls.Add(labelCard1Body);
            cardPanel1.Controls.Add(labelCard1Time);
            cardPanel1.Location = new Point(118, 204);
            cardPanel1.Name = "cardPanel1";
            cardPanel1.Size = new Size(900, 118);
            cardPanel1.TabIndex = 4;
            // 
            // labelCard1Title
            // 
            labelCard1Title.AutoSize = true;
            labelCard1Title.Location = new Point(24, 20);
            labelCard1Title.Name = "labelCard1Title";
            labelCard1Title.Size = new Size(215, 20);
            labelCard1Title.TabIndex = 0;
            labelCard1Title.Text = "You still have 4 pending evaluations";
            // 
            // labelCard1Body
            // 
            labelCard1Body.AutoSize = true;
            labelCard1Body.Location = new Point(24, 50);
            labelCard1Body.Name = "labelCard1Body";
            labelCard1Body.Size = new Size(496, 20);
            labelCard1Body.TabIndex = 1;
            labelCard1Body.Text = "Complete your remaining teacher evaluations before the submission period ends.";
            // 
            // labelCard1Time
            // 
            labelCard1Time.AutoSize = true;
            labelCard1Time.Location = new Point(24, 82);
            labelCard1Time.Name = "labelCard1Time";
            labelCard1Time.Size = new Size(90, 20);
            labelCard1Time.TabIndex = 2;
            labelCard1Time.Text = "5 minutes ago";
            // 
            // cardPanel2
            // 
            cardPanel2.Controls.Add(labelCard2Title);
            cardPanel2.Controls.Add(labelCard2Body);
            cardPanel2.Controls.Add(labelCard2Time);
            cardPanel2.Location = new Point(118, 340);
            cardPanel2.Name = "cardPanel2";
            cardPanel2.Size = new Size(900, 118);
            cardPanel2.TabIndex = 5;
            // 
            // labelCard2Title
            // 
            labelCard2Title.AutoSize = true;
            labelCard2Title.Location = new Point(24, 20);
            labelCard2Title.Name = "labelCard2Title";
            labelCard2Title.Size = new Size(210, 20);
            labelCard2Title.TabIndex = 0;
            labelCard2Title.Text = "New teacher schedule was posted";
            // 
            // labelCard2Body
            // 
            labelCard2Body.AutoSize = true;
            labelCard2Body.Location = new Point(24, 50);
            labelCard2Body.Name = "labelCard2Body";
            labelCard2Body.Size = new Size(510, 20);
            labelCard2Body.TabIndex = 1;
            labelCard2Body.Text = "A new evaluation schedule is now available for your current department and year level.";
            // 
            // labelCard2Time
            // 
            labelCard2Time.AutoSize = true;
            labelCard2Time.Location = new Point(24, 82);
            labelCard2Time.Name = "labelCard2Time";
            labelCard2Time.Size = new Size(74, 20);
            labelCard2Time.TabIndex = 2;
            labelCard2Time.Text = "Today, 1:30 PM";
            // 
            // cardPanel3
            // 
            cardPanel3.Controls.Add(labelCard3Title);
            cardPanel3.Controls.Add(labelCard3Body);
            cardPanel3.Controls.Add(labelCard3Time);
            cardPanel3.Location = new Point(118, 476);
            cardPanel3.Name = "cardPanel3";
            cardPanel3.Size = new Size(900, 118);
            cardPanel3.TabIndex = 6;
            // 
            // labelCard3Title
            // 
            labelCard3Title.AutoSize = true;
            labelCard3Title.Location = new Point(24, 20);
            labelCard3Title.Name = "labelCard3Title";
            labelCard3Title.Size = new Size(208, 20);
            labelCard3Title.TabIndex = 0;
            labelCard3Title.Text = "Thank you for your last submission";
            // 
            // labelCard3Body
            // 
            labelCard3Body.AutoSize = true;
            labelCard3Body.Location = new Point(24, 50);
            labelCard3Body.Name = "labelCard3Body";
            labelCard3Body.Size = new Size(465, 20);
            labelCard3Body.TabIndex = 1;
            labelCard3Body.Text = "Your evaluation for John Doe was recorded successfully and marked complete.";
            // 
            // labelCard3Time
            // 
            labelCard3Time.AutoSize = true;
            labelCard3Time.Location = new Point(24, 82);
            labelCard3Time.Name = "labelCard3Time";
            labelCard3Time.Size = new Size(89, 20);
            labelCard3Time.TabIndex = 2;
            labelCard3Time.Text = "Yesterday, 3:12 PM";
            // 
            // NotificationsPage
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 720);
            Controls.Add(containerPanel);
            Name = "NotificationsPage";
            Text = "NotificationsPage";
            containerPanel.ResumeLayout(false);
            containerPanel.PerformLayout();
            sidebarPanel.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            cardPanel1.ResumeLayout(false);
            cardPanel1.PerformLayout();
            cardPanel2.ResumeLayout(false);
            cardPanel2.PerformLayout();
            cardPanel3.ResumeLayout(false);
            cardPanel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel containerPanel;
        private FlowLayoutPanel sidebarPanel;
        private Button buttonHome;
        private Button buttonNotifications;
        private Button buttonLogout;
        private Panel headerPanel;
        private Label labelBrand;
        private Label labelTitle;
        private Label labelSubtitle;
        private Panel cardPanel1;
        private Label labelCard1Title;
        private Label labelCard1Body;
        private Label labelCard1Time;
        private Panel cardPanel2;
        private Label labelCard2Title;
        private Label labelCard2Body;
        private Label labelCard2Time;
        private Panel cardPanel3;
        private Label labelCard3Title;
        private Label labelCard3Body;
        private Label labelCard3Time;
    }
}