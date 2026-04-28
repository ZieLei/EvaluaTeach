namespace EvaluaTeach
{
    partial class Signup
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
            panel1 = new Panel();
            labelSubtitle = new Label();
            labelStudentId = new Label();
            textBoxStudentId = new TextBox();
            labelFullName = new Label();
            textBoxFullName = new TextBox();
            labelEmail = new Label();
            textBoxEmail = new TextBox();
            labelProgram = new Label();
            comboBoxProgram = new ComboBox();
            labelYearLevel = new Label();
            comboBoxYearLevel = new ComboBox();
            labelSection = new Label();
            textBoxSection = new TextBox();
            labelPassword = new Label();
            textBoxPassword = new TextBox();
            labelConfirmPassword = new Label();
            textBoxConfirmPassword = new TextBox();
            buttonCreateAccount = new Button();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(labelTitle, 1, 0);
            tableLayoutPanel1.Controls.Add(panel1, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.Size = new Size(984, 661);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // labelTitle
            // 
            labelTitle.Anchor = AnchorStyles.Bottom;
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Inter", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTitle.Location = new Point(373, 46);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(237, 64);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "EvaluaTeach";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top;
            panel1.Controls.Add(buttonCreateAccount);
            panel1.Controls.Add(textBoxConfirmPassword);
            panel1.Controls.Add(labelConfirmPassword);
            panel1.Controls.Add(textBoxPassword);
            panel1.Controls.Add(labelPassword);
            panel1.Controls.Add(textBoxSection);
            panel1.Controls.Add(labelSection);
            panel1.Controls.Add(comboBoxYearLevel);
            panel1.Controls.Add(labelYearLevel);
            panel1.Controls.Add(comboBoxProgram);
            panel1.Controls.Add(labelProgram);
            panel1.Controls.Add(textBoxEmail);
            panel1.Controls.Add(labelEmail);
            panel1.Controls.Add(textBoxFullName);
            panel1.Controls.Add(labelFullName);
            panel1.Controls.Add(textBoxStudentId);
            panel1.Controls.Add(labelStudentId);
            panel1.Controls.Add(labelSubtitle);
            panel1.Location = new Point(312, 113);
            panel1.Name = "panel1";
            panel1.Size = new Size(360, 505);
            panel1.TabIndex = 1;
            // 
            // labelSubtitle
            // 
            labelSubtitle.AutoSize = true;
            labelSubtitle.Location = new Point(34, 24);
            labelSubtitle.Name = "labelSubtitle";
            labelSubtitle.Size = new Size(225, 20);
            labelSubtitle.TabIndex = 0;
            labelSubtitle.Text = "Create your student evaluation account";
            // 
            // labelStudentId
            // 
            labelStudentId.AutoSize = true;
            labelStudentId.Location = new Point(34, 60);
            labelStudentId.Name = "labelStudentId";
            labelStudentId.Size = new Size(73, 20);
            labelStudentId.TabIndex = 1;
            labelStudentId.Text = "Student ID";
            // 
            // textBoxStudentId
            // 
            textBoxStudentId.Location = new Point(34, 83);
            textBoxStudentId.Name = "textBoxStudentId";
            textBoxStudentId.Size = new Size(292, 27);
            textBoxStudentId.TabIndex = 2;
            // 
            // labelFullName
            // 
            labelFullName.AutoSize = true;
            labelFullName.Location = new Point(34, 121);
            labelFullName.Name = "labelFullName";
            labelFullName.Size = new Size(76, 20);
            labelFullName.TabIndex = 3;
            labelFullName.Text = "Full Name";
            // 
            // textBoxFullName
            // 
            textBoxFullName.Location = new Point(34, 144);
            textBoxFullName.Name = "textBoxFullName";
            textBoxFullName.Size = new Size(292, 27);
            textBoxFullName.TabIndex = 4;
            // 
            // labelEmail
            // 
            labelEmail.AutoSize = true;
            labelEmail.Location = new Point(34, 182);
            labelEmail.Name = "labelEmail";
            labelEmail.Size = new Size(46, 20);
            labelEmail.TabIndex = 5;
            labelEmail.Text = "Email";
            // 
            // textBoxEmail
            // 
            textBoxEmail.Location = new Point(34, 205);
            textBoxEmail.Name = "textBoxEmail";
            textBoxEmail.Size = new Size(292, 27);
            textBoxEmail.TabIndex = 6;
            // 
            // labelProgram
            // 
            labelProgram.AutoSize = true;
            labelProgram.Location = new Point(34, 243);
            labelProgram.Name = "labelProgram";
            labelProgram.Size = new Size(65, 20);
            labelProgram.TabIndex = 7;
            labelProgram.Text = "Program";
            // 
            // comboBoxProgram
            // 
            comboBoxProgram.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxProgram.FormattingEnabled = true;
            comboBoxProgram.Location = new Point(34, 266);
            comboBoxProgram.Name = "comboBoxProgram";
            comboBoxProgram.Size = new Size(140, 28);
            comboBoxProgram.TabIndex = 8;
            // 
            // labelYearLevel
            // 
            labelYearLevel.AutoSize = true;
            labelYearLevel.Location = new Point(186, 243);
            labelYearLevel.Name = "labelYearLevel";
            labelYearLevel.Size = new Size(76, 20);
            labelYearLevel.TabIndex = 9;
            labelYearLevel.Text = "Year Level";
            // 
            // comboBoxYearLevel
            // 
            comboBoxYearLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxYearLevel.FormattingEnabled = true;
            comboBoxYearLevel.Location = new Point(186, 266);
            comboBoxYearLevel.Name = "comboBoxYearLevel";
            comboBoxYearLevel.Size = new Size(140, 28);
            comboBoxYearLevel.TabIndex = 10;
            // 
            // labelSection
            // 
            labelSection.AutoSize = true;
            labelSection.Location = new Point(34, 305);
            labelSection.Name = "labelSection";
            labelSection.Size = new Size(54, 20);
            labelSection.TabIndex = 11;
            labelSection.Text = "Section";
            // 
            // textBoxSection
            // 
            textBoxSection.Location = new Point(34, 328);
            textBoxSection.Name = "textBoxSection";
            textBoxSection.Size = new Size(292, 27);
            textBoxSection.TabIndex = 12;
            // 
            // labelPassword
            // 
            labelPassword.AutoSize = true;
            labelPassword.Location = new Point(34, 366);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(70, 20);
            labelPassword.TabIndex = 13;
            labelPassword.Text = "Password";
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(34, 389);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(292, 27);
            textBoxPassword.TabIndex = 14;
            // 
            // labelConfirmPassword
            // 
            labelConfirmPassword.AutoSize = true;
            labelConfirmPassword.Location = new Point(34, 427);
            labelConfirmPassword.Name = "labelConfirmPassword";
            labelConfirmPassword.Size = new Size(130, 20);
            labelConfirmPassword.TabIndex = 15;
            labelConfirmPassword.Text = "Confirm Password";
            // 
            // textBoxConfirmPassword
            // 
            textBoxConfirmPassword.Location = new Point(34, 450);
            textBoxConfirmPassword.Name = "textBoxConfirmPassword";
            textBoxConfirmPassword.Size = new Size(292, 27);
            textBoxConfirmPassword.TabIndex = 16;
            // 
            // buttonCreateAccount
            // 
            buttonCreateAccount.Location = new Point(110, 496);
            buttonCreateAccount.Name = "buttonCreateAccount";
            buttonCreateAccount.Size = new Size(140, 38);
            buttonCreateAccount.TabIndex = 17;
            buttonCreateAccount.Text = "Create Account";
            buttonCreateAccount.UseVisualStyleBackColor = true;
            // 
            // Signup
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 661);
            Controls.Add(tableLayoutPanel1);
            Name = "Signup";
            Text = "Signup";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label labelTitle;
        private Panel panel1;
        private Label labelSubtitle;
        private Label labelStudentId;
        private TextBox textBoxStudentId;
        private Label labelFullName;
        private TextBox textBoxFullName;
        private Label labelEmail;
        private TextBox textBoxEmail;
        private Label labelProgram;
        private ComboBox comboBoxProgram;
        private Label labelYearLevel;
        private ComboBox comboBoxYearLevel;
        private Label labelSection;
        private TextBox textBoxSection;
        private Label labelPassword;
        private TextBox textBoxPassword;
        private Label labelConfirmPassword;
        private TextBox textBoxConfirmPassword;
        private Button buttonCreateAccount;
    }
}