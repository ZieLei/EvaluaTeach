namespace EvaluaTeach
{
    partial class LandingPage
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
            mainLayout = new TableLayoutPanel();
            topBarPanel = new Panel();
            labelBrand = new Label();
            buttonSignup = new Button();
            buttonLogin = new Button();
            heroPanel = new Panel();
            labelBadge = new Label();
            labelHeadline = new Label();
            labelSubheadline = new Label();
            buttonGetStarted = new Button();
            panelStats = new Panel();
            labelStats1 = new Label();
            labelStats2 = new Label();
            labelStats3 = new Label();
            cardPanel1 = new Panel();
            labelCard1Title = new Label();
            labelCard1Body = new Label();
            cardPanel2 = new Panel();
            labelCard2Title = new Label();
            labelCard2Body = new Label();
            cardPanel3 = new Panel();
            labelCard3Title = new Label();
            labelCard3Body = new Label();
            mainLayout.SuspendLayout();
            topBarPanel.SuspendLayout();
            heroPanel.SuspendLayout();
            panelStats.SuspendLayout();
            cardPanel1.SuspendLayout();
            cardPanel2.SuspendLayout();
            cardPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 3;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8F));
            mainLayout.Controls.Add(topBarPanel, 1, 0);
            mainLayout.Controls.Add(heroPanel, 1, 1);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 0);
            mainLayout.Name = "mainLayout";
            mainLayout.RowCount = 3;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            mainLayout.Size = new Size(1180, 720);
            mainLayout.TabIndex = 0;
            // 
            // topBarPanel
            // 
            topBarPanel.Controls.Add(labelBrand);
            topBarPanel.Controls.Add(buttonSignup);
            topBarPanel.Controls.Add(buttonLogin);
            topBarPanel.Dock = DockStyle.Fill;
            topBarPanel.Location = new Point(97, 12);
            topBarPanel.Margin = new Padding(3, 12, 3, 12);
            topBarPanel.Name = "topBarPanel";
            topBarPanel.Size = new Size(985, 64);
            topBarPanel.TabIndex = 0;
            // 
            // labelBrand
            // 
            labelBrand.AutoSize = true;
            labelBrand.Location = new Point(0, 12);
            labelBrand.Name = "labelBrand";
            labelBrand.Size = new Size(104, 20);
            labelBrand.TabIndex = 0;
            labelBrand.Text = "EvaluaTeach";
            // 
            // buttonLogin
            // 
            buttonLogin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonLogin.Location = new Point(856, 8);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(129, 40);
            buttonLogin.TabIndex = 1;
            buttonLogin.Text = "Student Login";
            buttonLogin.UseVisualStyleBackColor = true;
            // 
            // buttonSignup
            // 
            buttonSignup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSignup.Location = new Point(713, 8);
            buttonSignup.Name = "buttonSignup";
            buttonSignup.Size = new Size(129, 40);
            buttonSignup.TabIndex = 2;
            buttonSignup.Text = "Sign Up";
            buttonSignup.UseVisualStyleBackColor = true;
            // 
            // heroPanel
            // 
            heroPanel.Controls.Add(labelBadge);
            heroPanel.Controls.Add(labelHeadline);
            heroPanel.Controls.Add(labelSubheadline);
            heroPanel.Controls.Add(buttonGetStarted);
            heroPanel.Controls.Add(panelStats);
            heroPanel.Controls.Add(cardPanel1);
            heroPanel.Controls.Add(cardPanel2);
            heroPanel.Controls.Add(cardPanel3);
            heroPanel.Dock = DockStyle.Fill;
            heroPanel.Location = new Point(97, 100);
            heroPanel.Margin = new Padding(3, 12, 3, 12);
            heroPanel.Name = "heroPanel";
            heroPanel.Size = new Size(985, 576);
            heroPanel.TabIndex = 1;
            // 
            // labelBadge
            // 
            labelBadge.AutoSize = true;
            labelBadge.Location = new Point(24, 24);
            labelBadge.Name = "labelBadge";
            labelBadge.Size = new Size(221, 20);
            labelBadge.TabIndex = 0;
            labelBadge.Text = "A smarter way to evaluate teaching";
            // 
            // labelHeadline
            // 
            labelHeadline.AutoSize = true;
            labelHeadline.Location = new Point(24, 64);
            labelHeadline.Name = "labelHeadline";
            labelHeadline.Size = new Size(413, 40);
            labelHeadline.TabIndex = 1;
            labelHeadline.Text = "Make every teacher evaluation feel\r\nclear, fair, and worth answering.";
            // 
            // labelSubheadline
            // 
            labelSubheadline.AutoSize = true;
            labelSubheadline.Location = new Point(24, 128);
            labelSubheadline.Name = "labelSubheadline";
            labelSubheadline.Size = new Size(569, 60);
            labelSubheadline.TabIndex = 2;
            labelSubheadline.Text = "EvaluaTeach helps students submit thoughtful feedback through a clean,\r\norganized experience, so schools can collect better insights and teachers\r\nreceive more meaningful evaluation results.";
            // 
            // buttonGetStarted
            // 
            buttonGetStarted.Location = new Point(24, 214);
            buttonGetStarted.Name = "buttonGetStarted";
            buttonGetStarted.Size = new Size(156, 46);
            buttonGetStarted.TabIndex = 3;
            buttonGetStarted.Text = "Get Started";
            buttonGetStarted.UseVisualStyleBackColor = true;
            // 
            // panelStats
            // 
            panelStats.Controls.Add(labelStats1);
            panelStats.Controls.Add(labelStats2);
            panelStats.Controls.Add(labelStats3);
            panelStats.Location = new Point(24, 290);
            panelStats.Name = "panelStats";
            panelStats.Size = new Size(580, 92);
            panelStats.TabIndex = 5;
            // 
            // labelStats1
            // 
            labelStats1.AutoSize = true;
            labelStats1.Location = new Point(18, 18);
            labelStats1.Name = "labelStats1";
            labelStats1.Size = new Size(115, 40);
            labelStats1.TabIndex = 0;
            labelStats1.Text = "Guided forms\r\nfor students";
            // 
            // labelStats2
            // 
            labelStats2.AutoSize = true;
            labelStats2.Location = new Point(216, 18);
            labelStats2.Name = "labelStats2";
            labelStats2.Size = new Size(136, 40);
            labelStats2.TabIndex = 1;
            labelStats2.Text = "Cleaner feedback\r\ncollection";
            // 
            // labelStats3
            // 
            labelStats3.AutoSize = true;
            labelStats3.Location = new Point(415, 18);
            labelStats3.Name = "labelStats3";
            labelStats3.Size = new Size(125, 40);
            labelStats3.TabIndex = 2;
            labelStats3.Text = "Better evaluation\r\nexperience";
            // 
            // cardPanel1
            // 
            cardPanel1.Controls.Add(labelCard1Title);
            cardPanel1.Controls.Add(labelCard1Body);
            cardPanel1.Location = new Point(24, 410);
            cardPanel1.Name = "cardPanel1";
            cardPanel1.Size = new Size(290, 130);
            cardPanel1.TabIndex = 6;
            // 
            // labelCard1Title
            // 
            labelCard1Title.AutoSize = true;
            labelCard1Title.Location = new Point(20, 18);
            labelCard1Title.Name = "labelCard1Title";
            labelCard1Title.Size = new Size(111, 20);
            labelCard1Title.TabIndex = 0;
            labelCard1Title.Text = "Focused Feedback";
            // 
            // labelCard1Body
            // 
            labelCard1Body.AutoSize = true;
            labelCard1Body.Location = new Point(20, 48);
            labelCard1Body.Name = "labelCard1Body";
            labelCard1Body.Size = new Size(244, 60);
            labelCard1Body.TabIndex = 1;
            labelCard1Body.Text = "Help students respond with clarity using a structured and less overwhelming evaluation flow.";
            // 
            // cardPanel2
            // 
            cardPanel2.Controls.Add(labelCard2Title);
            cardPanel2.Controls.Add(labelCard2Body);
            cardPanel2.Location = new Point(347, 410);
            cardPanel2.Name = "cardPanel2";
            cardPanel2.Size = new Size(290, 130);
            cardPanel2.TabIndex = 7;
            // 
            // labelCard2Title
            // 
            labelCard2Title.AutoSize = true;
            labelCard2Title.Location = new Point(20, 18);
            labelCard2Title.Name = "labelCard2Title";
            labelCard2Title.Size = new Size(108, 20);
            labelCard2Title.TabIndex = 0;
            labelCard2Title.Text = "Student Friendly";
            // 
            // labelCard2Body
            // 
            labelCard2Body.AutoSize = true;
            labelCard2Body.Location = new Point(20, 48);
            labelCard2Body.Name = "labelCard2Body";
            labelCard2Body.Size = new Size(232, 60);
            labelCard2Body.TabIndex = 1;
            labelCard2Body.Text = "Designed for students first, with a simpler interface that feels modern and easy to trust.";
            // 
            // cardPanel3
            // 
            cardPanel3.Controls.Add(labelCard3Title);
            cardPanel3.Controls.Add(labelCard3Body);
            cardPanel3.Location = new Point(670, 410);
            cardPanel3.Name = "cardPanel3";
            cardPanel3.Size = new Size(290, 130);
            cardPanel3.TabIndex = 8;
            // 
            // labelCard3Title
            // 
            labelCard3Title.AutoSize = true;
            labelCard3Title.Location = new Point(20, 18);
            labelCard3Title.Name = "labelCard3Title";
            labelCard3Title.Size = new Size(117, 20);
            labelCard3Title.TabIndex = 0;
            labelCard3Title.Text = "School Ready";
            // 
            // labelCard3Body
            // 
            labelCard3Body.AutoSize = true;
            labelCard3Body.Location = new Point(20, 48);
            labelCard3Body.Name = "labelCard3Body";
            labelCard3Body.Size = new Size(228, 60);
            labelCard3Body.TabIndex = 1;
            labelCard3Body.Text = "A stronger landing experience gives the platform a more official and credible first impression.";
            // 
            // LandingPage
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1180, 720);
            Controls.Add(mainLayout);
            Name = "LandingPage";
            Text = "LandingPage";
            mainLayout.ResumeLayout(false);
            topBarPanel.ResumeLayout(false);
            topBarPanel.PerformLayout();
            heroPanel.ResumeLayout(false);
            heroPanel.PerformLayout();
            panelStats.ResumeLayout(false);
            panelStats.PerformLayout();
            cardPanel1.ResumeLayout(false);
            cardPanel1.PerformLayout();
            cardPanel2.ResumeLayout(false);
            cardPanel2.PerformLayout();
            cardPanel3.ResumeLayout(false);
            cardPanel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayout;
        private Panel topBarPanel;
        private Label labelBrand;
        private Button buttonSignup;
        private Button buttonLogin;
        private Panel heroPanel;
        private Label labelBadge;
        private Label labelHeadline;
        private Label labelSubheadline;
        private Button buttonGetStarted;
        private Panel panelStats;
        private Label labelStats1;
        private Label labelStats2;
        private Label labelStats3;
        private Panel cardPanel1;
        private Label labelCard1Title;
        private Label labelCard1Body;
        private Panel cardPanel2;
        private Label labelCard2Title;
        private Label labelCard2Body;
        private Panel cardPanel3;
        private Label labelCard3Title;
        private Label labelCard3Body;
    }
}