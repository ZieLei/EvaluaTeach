namespace EvaluaTeach
{
    partial class Login
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
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            button1 = new Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.12048F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39.759037F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.1204815F));
            tableLayoutPanel1.Controls.Add(textBox2, 1, 1);
            tableLayoutPanel1.Controls.Add(textBox1, 1, 2);
            tableLayoutPanel1.Controls.Add(button1, 1, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.7374F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 19.9717064F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 29.9575577F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Size = new Size(800, 450);
            tableLayoutPanel1.TabIndex = 0;
            tableLayoutPanel1.Paint += tableLayoutPanel1_Paint;
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Bottom;
            textBox2.Location = new Point(291, 138);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(215, 23);
            textBox2.TabIndex = 1;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top;
            textBox1.Location = new Point(291, 194);
            textBox1.Margin = new Padding(3, 30, 3, 3);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(215, 23);
            textBox1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top;
            button1.Location = new Point(309, 301);
            button1.Name = "button1";
            button1.Size = new Size(179, 47);
            button1.TabIndex = 3;
            button1.Text = "Login";
            button1.UseVisualStyleBackColor = true;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "Login";
            Text = "2";
            Load += Login_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TextBox textBox2;
        private TextBox textBox1;
        private Button button1;
    }
}