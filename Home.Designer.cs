namespace EvaluaTeach
{
    partial class Home
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            panel1 = new Panel();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            flowLayoutPanel3 = new FlowLayoutPanel();
            panel2 = new Panel();
            button4 = new Button();
            label8 = new Label();
            label7 = new Label();
            label3 = new Label();
            label2 = new Label();
            flowLayoutPanel2 = new FlowLayoutPanel();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            flowLayoutPanel1 = new FlowLayoutPanel();
            label1 = new Label();
            textBox1 = new TextBox();
            button8 = new Button();
            panel3 = new Panel();
            label10 = new Label();
            label9 = new Label();
            button7 = new Button();
            button6 = new Button();
            panel1.SuspendLayout();
            flowLayoutPanel3.SuspendLayout();
            panel2.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(label6);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(flowLayoutPanel3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(flowLayoutPanel2);
            panel1.Controls.Add(flowLayoutPanel1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(919, 450);
            panel1.TabIndex = 0;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(397, 113);
            label6.Name = "label6";
            label6.Size = new Size(70, 15);
            label6.TabIndex = 7;
            label6.Text = "Department";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(272, 113);
            label5.Name = "label5";
            label5.Size = new Size(56, 15);
            label5.TabIndex = 6;
            label5.Text = "Subject/s";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(160, 113);
            label4.Name = "label4";
            label4.Size = new Size(39, 15);
            label4.TabIndex = 1;
            label4.Text = "Name";
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.Controls.Add(panel2);
            flowLayoutPanel3.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel3.Location = new Point(124, 131);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(694, 319);
            flowLayoutPanel3.TabIndex = 5;
            // 
            // panel2
            // 
            panel2.Controls.Add(button4);
            panel2.Controls.Add(label8);
            panel2.Controls.Add(label7);
            panel2.Controls.Add(label3);
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(691, 66);
            panel2.TabIndex = 0;
            // 
            // button4
            // 
            button4.BackColor = Color.Lime;
            button4.Font = new Font("Inter", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button4.ForeColor = Color.White;
            button4.Location = new Point(580, 13);
            button4.Name = "button4";
            button4.Size = new Size(96, 38);
            button4.TabIndex = 3;
            button4.Text = "Evaluate";
            button4.UseVisualStyleBackColor = false;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Inter Medium", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(285, 22);
            label8.Name = "label8";
            label8.Size = new Size(35, 17);
            label8.TabIndex = 2;
            label8.Text = "BSIT";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Inter Medium", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(145, 22);
            label7.Name = "label7";
            label7.Size = new Size(65, 17);
            label7.TabIndex = 1;
            label7.Text = "MATH 101";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Inter Medium", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(28, 22);
            label3.Name = "label3";
            label3.Size = new Size(62, 17);
            label3.TabIndex = 0;
            label3.Text = "John Doe";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Inter SemiBold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(87, 68);
            label2.Name = "label2";
            label2.Size = new Size(67, 18);
            label2.TabIndex = 4;
            label2.Text = "Teachers";
            label2.Click += label2_Click;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(button1);
            flowLayoutPanel2.Controls.Add(button2);
            flowLayoutPanel2.Controls.Add(button3);
            flowLayoutPanel2.Dock = DockStyle.Left;
            flowLayoutPanel2.Location = new Point(0, 53);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(66, 397);
            flowLayoutPanel2.TabIndex = 3;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(0, 192, 0);
            button1.BackgroundImage = (Image)resources.GetObject("button1.BackgroundImage");
            button1.BackgroundImageLayout = ImageLayout.Zoom;
            button1.Location = new Point(10, 3);
            button1.Margin = new Padding(10, 3, 3, 3);
            button1.Name = "button1";
            button1.Size = new Size(52, 51);
            button1.TabIndex = 0;
            button1.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            button2.BackgroundImage = (Image)resources.GetObject("button2.BackgroundImage");
            button2.BackgroundImageLayout = ImageLayout.Zoom;
            button2.Location = new Point(10, 60);
            button2.Margin = new Padding(10, 3, 3, 3);
            button2.Name = "button2";
            button2.Size = new Size(52, 47);
            button2.TabIndex = 1;
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.BackgroundImage = (Image)resources.GetObject("button3.BackgroundImage");
            button3.BackgroundImageLayout = ImageLayout.Zoom;
            button3.ForeColor = Color.Red;
            button3.Location = new Point(10, 340);
            button3.Margin = new Padding(10, 230, 3, 3);
            button3.Name = "button3";
            button3.Size = new Size(52, 45);
            button3.TabIndex = 2;
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(label1);
            flowLayoutPanel1.Controls.Add(textBox1);
            flowLayoutPanel1.Controls.Add(button8);
            flowLayoutPanel1.Controls.Add(panel3);
            flowLayoutPanel1.Controls.Add(button6);
            flowLayoutPanel1.Dock = DockStyle.Top;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(919, 53);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Bebas Neue", 23.9999962F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Green;
            label1.Location = new Point(3, 5);
            label1.Margin = new Padding(3, 5, 3, 0);
            label1.Name = "label1";
            label1.Size = new Size(151, 51);
            label1.TabIndex = 0;
            label1.Text = "EvaluaTeach";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(160, 10);
            textBox1.Margin = new Padding(3, 10, 3, 3);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(287, 23);
            textBox1.TabIndex = 1;
            textBox1.Text = "Search";
            // 
            // button8
            // 
            button8.BackgroundImage = (Image)resources.GetObject("button8.BackgroundImage");
            button8.BackgroundImageLayout = ImageLayout.Zoom;
            button8.Location = new Point(453, 10);
            button8.Margin = new Padding(3, 10, 3, 3);
            button8.Name = "button8";
            button8.Size = new Size(28, 28);
            button8.TabIndex = 4;
            button8.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            panel3.Controls.Add(label10);
            panel3.Controls.Add(label9);
            panel3.Controls.Add(button7);
            panel3.Location = new Point(634, 3);
            panel3.Margin = new Padding(150, 3, 3, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(200, 50);
            panel3.TabIndex = 3;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = Color.FromArgb(64, 64, 64);
            label10.Location = new Point(62, 25);
            label10.Name = "label10";
            label10.Size = new Size(74, 15);
            label10.TabIndex = 2;
            label10.Text = "Student BSIT";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Inter", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.Location = new Point(62, 8);
            label9.Name = "label9";
            label9.Size = new Size(72, 17);
            label9.TabIndex = 1;
            label9.Text = "Mang Juan";
            // 
            // button7
            // 
            button7.BackgroundImage = (Image)resources.GetObject("button7.BackgroundImage");
            button7.BackgroundImageLayout = ImageLayout.Zoom;
            button7.Location = new Point(3, 3);
            button7.Name = "button7";
            button7.Size = new Size(46, 44);
            button7.TabIndex = 0;
            button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.BackgroundImage = (Image)resources.GetObject("button6.BackgroundImage");
            button6.BackgroundImageLayout = ImageLayout.Zoom;
            button6.Location = new Point(840, 10);
            button6.Margin = new Padding(3, 10, 3, 3);
            button6.Name = "button6";
            button6.Size = new Size(22, 23);
            button6.TabIndex = 2;
            button6.UseVisualStyleBackColor = true;
            // 
            // Home
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(919, 450);
            Controls.Add(panel1);
            Name = "Home";
            Text = "Home";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            flowLayoutPanel3.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private Label label1;
        private Button button1;
        private Button button2;
        private Button button3;
        private TextBox textBox1;
        private Label label2;
        private FlowLayoutPanel flowLayoutPanel3;
        private Panel panel2;
        private Label label3;
        private Label label6;
        private Label label5;
        private Label label4;
        private Button button4;
        private Label label8;
        private Label label7;
        private Button button6;
        private Button button8;
        private Panel panel3;
        private Label label10;
        private Label label9;
        private Button button7;
    }
}