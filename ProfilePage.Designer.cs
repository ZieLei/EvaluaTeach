namespace EvaluaTeach
{
    partial class ProfilePage
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
            tableLayoutPanel1 = new TableLayoutPanel();
            labelTitle = new Label();
            panelHeader = new Panel();
            labelAvatar = new Label();
            labelStudentName = new Label();
            labelStudentMeta = new Label();
            panelAcademic = new Panel();
            labelAcademicTitle = new Label();
            labelStudentIdTitle = new Label();
            labelStudentIdValue = new Label();
            labelProgramTitle = new Label();
            labelProgramValue = new Label();
            labelYearLevelTitle = new Label();
            labelYearLevelValue = new Label();
            labelSectionTitle = new Label();
            labelSectionValue = new Label();
            panelAccount = new Panel();
            labelAccountTitle = new Label();
            labelEmailTitle = new Label();
            labelEmailValue = new Label();
            labelStatusTitle = new Label();
            labelStatusValue = new Label();
            labelNote = new Label();
            tableLayoutPanel1.SuspendLayout();
            panelHeader.SuspendLayout();
            panelAcademic.SuspendLayout();
            panelAccount.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.Controls.Add(labelTitle, 1, 0);
            tableLayoutPanel1.Controls.Add(panelHeader, 1, 1);
            tableLayoutPanel1.Controls.Add(panelAcademic, 1, 2);
            tableLayoutPanel1.Controls.Add(panelAccount, 1, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 170F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 190F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 170F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1000, 700);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // labelTitle
            // 
            labelTitle.Anchor = AnchorStyles.Bottom;
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Inter", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTitle.Location = new Point(405, 51);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(190, 49);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "My Profile";
            // 
            // panelHeader
            // 
            panelHeader.Anchor = AnchorStyles.Top;
            panelHeader.Controls.Add(labelAvatar);
            panelHeader.Controls.Add(labelStudentName);
            panelHeader.Controls.Add(labelStudentMeta);
            panelHeader.Location = new Point(250, 112);
            panelHeader.Margin = new Padding(50, 12, 50, 12);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(500, 146);
            panelHeader.TabIndex = 1;
            // 
            // labelAvatar
            // 
            labelAvatar.Location = new Point(28, 28);
            labelAvatar.Name = "labelAvatar";
            labelAvatar.Size = new Size(84, 84);
            labelAvatar.TabIndex = 0;
            labelAvatar.Text = "MJ";
            labelAvatar.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelStudentName
            // 
            labelStudentName.AutoSize = true;
            labelStudentName.Location = new Point(138, 40);
            labelStudentName.Name = "labelStudentName";
            labelStudentName.Size = new Size(91, 20);
            labelStudentName.TabIndex = 1;
            labelStudentName.Text = "Mang Juan";
            // 
            // labelStudentMeta
            // 
            labelStudentMeta.AutoSize = true;
            labelStudentMeta.Location = new Point(138, 72);
            labelStudentMeta.Name = "labelStudentMeta";
            labelStudentMeta.Size = new Size(209, 20);
            labelStudentMeta.TabIndex = 2;
            labelStudentMeta.Text = "BSIT • 3rd Year • Active Student";
            // 
            // panelAcademic
            // 
            panelAcademic.Anchor = AnchorStyles.Top;
            panelAcademic.Controls.Add(labelAcademicTitle);
            panelAcademic.Controls.Add(labelStudentIdTitle);
            panelAcademic.Controls.Add(labelStudentIdValue);
            panelAcademic.Controls.Add(labelProgramTitle);
            panelAcademic.Controls.Add(labelProgramValue);
            panelAcademic.Controls.Add(labelYearLevelTitle);
            panelAcademic.Controls.Add(labelYearLevelValue);
            panelAcademic.Controls.Add(labelSectionTitle);
            panelAcademic.Controls.Add(labelSectionValue);
            panelAcademic.Location = new Point(250, 282);
            panelAcademic.Margin = new Padding(50, 12, 50, 12);
            panelAcademic.Name = "panelAcademic";
            panelAcademic.Size = new Size(500, 166);
            panelAcademic.TabIndex = 2;
            // 
            // labelAcademicTitle
            // 
            labelAcademicTitle.AutoSize = true;
            labelAcademicTitle.Location = new Point(28, 22);
            labelAcademicTitle.Name = "labelAcademicTitle";
            labelAcademicTitle.Size = new Size(120, 20);
            labelAcademicTitle.TabIndex = 0;
            labelAcademicTitle.Text = "Academic Details";
            // 
            // labelStudentIdTitle
            // 
            labelStudentIdTitle.AutoSize = true;
            labelStudentIdTitle.Location = new Point(28, 62);
            labelStudentIdTitle.Name = "labelStudentIdTitle";
            labelStudentIdTitle.Size = new Size(73, 20);
            labelStudentIdTitle.TabIndex = 1;
            labelStudentIdTitle.Text = "Student ID";
            // 
            // labelStudentIdValue
            // 
            labelStudentIdValue.AutoSize = true;
            labelStudentIdValue.Location = new Point(28, 88);
            labelStudentIdValue.Name = "labelStudentIdValue";
            labelStudentIdValue.Size = new Size(95, 20);
            labelStudentIdValue.TabIndex = 2;
            labelStudentIdValue.Text = "2024-000123";
            // 
            // labelProgramTitle
            // 
            labelProgramTitle.AutoSize = true;
            labelProgramTitle.Location = new Point(270, 62);
            labelProgramTitle.Name = "labelProgramTitle";
            labelProgramTitle.Size = new Size(65, 20);
            labelProgramTitle.TabIndex = 3;
            labelProgramTitle.Text = "Program";
            // 
            // labelProgramValue
            // 
            labelProgramValue.AutoSize = true;
            labelProgramValue.Location = new Point(270, 88);
            labelProgramValue.Name = "labelProgramValue";
            labelProgramValue.Size = new Size(37, 20);
            labelProgramValue.TabIndex = 4;
            labelProgramValue.Text = "BSIT";
            // 
            // labelYearLevelTitle
            // 
            labelYearLevelTitle.AutoSize = true;
            labelYearLevelTitle.Location = new Point(28, 118);
            labelYearLevelTitle.Name = "labelYearLevelTitle";
            labelYearLevelTitle.Size = new Size(76, 20);
            labelYearLevelTitle.TabIndex = 5;
            labelYearLevelTitle.Text = "Year Level";
            // 
            // labelYearLevelValue
            // 
            labelYearLevelValue.AutoSize = true;
            labelYearLevelValue.Location = new Point(28, 142);
            labelYearLevelValue.Name = "labelYearLevelValue";
            labelYearLevelValue.Size = new Size(73, 20);
            labelYearLevelValue.TabIndex = 6;
            labelYearLevelValue.Text = "3rd Year";
            // 
            // labelSectionTitle
            // 
            labelSectionTitle.AutoSize = true;
            labelSectionTitle.Location = new Point(270, 118);
            labelSectionTitle.Name = "labelSectionTitle";
            labelSectionTitle.Size = new Size(54, 20);
            labelSectionTitle.TabIndex = 7;
            labelSectionTitle.Text = "Section";
            // 
            // labelSectionValue
            // 
            labelSectionValue.AutoSize = true;
            labelSectionValue.Location = new Point(270, 142);
            labelSectionValue.Name = "labelSectionValue";
            labelSectionValue.Size = new Size(51, 20);
            labelSectionValue.TabIndex = 8;
            labelSectionValue.Text = "BSIT 3A";
            // 
            // panelAccount
            // 
            panelAccount.Anchor = AnchorStyles.Top;
            panelAccount.Controls.Add(labelAccountTitle);
            panelAccount.Controls.Add(labelEmailTitle);
            panelAccount.Controls.Add(labelEmailValue);
            panelAccount.Controls.Add(labelStatusTitle);
            panelAccount.Controls.Add(labelStatusValue);
            panelAccount.Controls.Add(labelNote);
            panelAccount.Location = new Point(250, 472);
            panelAccount.Margin = new Padding(50, 12, 50, 12);
            panelAccount.Name = "panelAccount";
            panelAccount.Size = new Size(500, 146);
            panelAccount.TabIndex = 3;
            // 
            // labelAccountTitle
            // 
            labelAccountTitle.AutoSize = true;
            labelAccountTitle.Location = new Point(28, 22);
            labelAccountTitle.Name = "labelAccountTitle";
            labelAccountTitle.Size = new Size(106, 20);
            labelAccountTitle.TabIndex = 0;
            labelAccountTitle.Text = "Account Details";
            // 
            // labelEmailTitle
            // 
            labelEmailTitle.AutoSize = true;
            labelEmailTitle.Location = new Point(28, 58);
            labelEmailTitle.Name = "labelEmailTitle";
            labelEmailTitle.Size = new Size(46, 20);
            labelEmailTitle.TabIndex = 1;
            labelEmailTitle.Text = "Email";
            // 
            // labelEmailValue
            // 
            labelEmailValue.AutoSize = true;
            labelEmailValue.Location = new Point(28, 82);
            labelEmailValue.Name = "labelEmailValue";
            labelEmailValue.Size = new Size(165, 20);
            labelEmailValue.TabIndex = 2;
            labelEmailValue.Text = "mangjuan@student.edu";
            // 
            // labelStatusTitle
            // 
            labelStatusTitle.AutoSize = true;
            labelStatusTitle.Location = new Point(270, 58);
            labelStatusTitle.Name = "labelStatusTitle";
            labelStatusTitle.Size = new Size(49, 20);
            labelStatusTitle.TabIndex = 3;
            labelStatusTitle.Text = "Status";
            // 
            // labelStatusValue
            // 
            labelStatusValue.AutoSize = true;
            labelStatusValue.Location = new Point(270, 82);
            labelStatusValue.Name = "labelStatusValue";
            labelStatusValue.Size = new Size(48, 20);
            labelStatusValue.TabIndex = 4;
            labelStatusValue.Text = "Active";
            // 
            // labelNote
            // 
            labelNote.AutoSize = true;
            labelNote.Location = new Point(28, 112);
            labelNote.Name = "labelNote";
            labelNote.Size = new Size(262, 20);
            labelNote.TabIndex = 5;
            labelNote.Text = "Keep your student details updated before evaluating.";
            // 
            // ProfilePage
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 700);
            Controls.Add(tableLayoutPanel1);
            Name = "ProfilePage";
            Text = "ProfilePage";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelAcademic.ResumeLayout(false);
            panelAcademic.PerformLayout();
            panelAccount.ResumeLayout(false);
            panelAccount.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label labelTitle;
        private Panel panelHeader;
        private Label labelAvatar;
        private Label labelStudentName;
        private Label labelStudentMeta;
        private Panel panelAcademic;
        private Label labelAcademicTitle;
        private Label labelStudentIdTitle;
        private Label labelStudentIdValue;
        private Label labelProgramTitle;
        private Label labelProgramValue;
        private Label labelYearLevelTitle;
        private Label labelYearLevelValue;
        private Label labelSectionTitle;
        private Label labelSectionValue;
        private Panel panelAccount;
        private Label labelAccountTitle;
        private Label labelEmailTitle;
        private Label labelEmailValue;
        private Label labelStatusTitle;
        private Label labelStatusValue;
        private Label labelNote;
    }
}