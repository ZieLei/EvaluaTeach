using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EvaluaTeach
{
    public partial class Login : Form
    {
        private readonly Color placeholderColor = Color.FromArgb(148, 163, 184);
        private readonly Color inputTextColor = Color.FromArgb(30, 41, 59);

        public Login()
        {
            InitializeComponent();
            ConfigureLoginUi();
        }

        private void ConfigureLoginUi()
        {
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(241, 245, 249);
            Text = "EvaluaTeach Login";

            tableLayoutPanel1.BackColor = BackColor;
            // Match the sign-up page structure so the card stays centered consistently.
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            label3.Text = "EvaluaTeach";
            label3.ForeColor = Color.FromArgb(22, 163, 74);

            panel1.BackColor = Color.White;
            panel1.Padding = new Padding(28);
            panel1.Dock = DockStyle.None;
            panel1.Anchor = AnchorStyles.None;
            panel1.Margin = new Padding(0, 12, 0, 0);
            // Slightly larger card for better readability on wide screens.
            panel1.MaximumSize = new Size(420, 340);
            panel1.MinimumSize = new Size(360, 300);

            // Global back button pinned to the top-left of the window, not inside the card.
            var backBtn = new Button
            {
                Text = "Back",
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(100, 116, 139),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter", 9F),
                Size = new Size(70, 30),
                Location = new Point(24, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            backBtn.Click += (_, _) => Program.NavigateTo(new LandingPage());
            Controls.Add(backBtn);
            backBtn.BringToFront();

            label2.Text = "User ID";
            label2.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(51, 65, 85);

            label1.Text = "Password";
            label1.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(51, 65, 85);

            StyleTextBox(textBox1, "Enter your ID");
            StyleTextBox(textBox2, "Enter your password", true);

            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = Color.FromArgb(22, 163, 74);
            button1.ForeColor = Color.White;
            button1.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            button1.Text = "Sign in";
            button1.Size = new Size(140, 38);

            tableLayoutPanel1.Refresh();
            UpdateLoginLayout();
            Resize += Login_Resize;
        }

        private void StyleTextBox(TextBox textBox, string placeholder, bool isPassword = false)
        {
            textBox.BackColor = Color.FromArgb(248, 250, 252);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Inter", 10F, FontStyle.Regular);
            textBox.Tag = placeholder;
            textBox.Text = placeholder;
            textBox.ForeColor = placeholderColor;

            textBox.Enter += (_, _) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = string.Empty;
                    textBox.ForeColor = inputTextColor;
                    if (isPassword)
                    {
                        textBox.UseSystemPasswordChar = true;
                    }
                }
            };

            textBox.Leave += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (isPassword)
                    {
                        textBox.UseSystemPasswordChar = false;
                    }

                    textBox.Text = placeholder;
                    textBox.ForeColor = placeholderColor;
                }
            };
        }

        private void UpdateLoginLayout()
        {
            // Size card; table layout handles centering like the sign-up page.
            int maxWidth = 420;
            int horizontalPadding = 160;
            int targetWidth = Math.Min(maxWidth, ClientSize.Width - horizontalPadding);
            panel1.Size = new Size(targetWidth, 300);

            label2.Location = new Point(28, 40);
            textBox1.Location = new Point(28, label2.Bottom + 10);
            textBox1.Size = new Size(panel1.Width - 56, 32);

            label1.Location = new Point(28, textBox1.Bottom + 18);
            textBox2.Location = new Point(28, label1.Bottom + 10);
            textBox2.Size = new Size(panel1.Width - 56, 32);

            button1.Location = new Point((panel1.Width - button1.Width) / 2, textBox2.Bottom + 28);
        }

        private void Login_Resize(object? sender, EventArgs e)
        {
            UpdateLoginLayout();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_3(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userId = textBox1.Text.Trim();
            string password = textBox2.Text;

            if (string.IsNullOrWhiteSpace(userId) || userId == "Enter your ID")
            {
                MessageBox.Show("Please enter your ID.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password == "Enter your password")
            {
                MessageBox.Show("Please enter your password.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Ensure default users exist
                EnsureDefaultUsers();

                // Authenticate against database - auto-detect admin or student
                var (isValid, isAdmin, numericId, name, email, course) = AuthenticateUser(userId, password);

                if (!isValid)
                {
                    MessageBox.Show("Invalid ID or password. Please check your credentials and try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                UserRole role = isAdmin ? UserRole.Admin : UserRole.Student;
                string meta = isAdmin ? "Administrator" : $"Student {course}";

                SessionStore.Login(userId, numericId, name, email, role);
                ProfileStore.UpdateProfile(name, meta, email, userId, numericId);

                // Load avatar from database for students
                if (!isAdmin && numericId.HasValue)
                {
                    ProfileStore.LoadAvatarFromDatabase(numericId.Value);
                }

                FormDataStore.SeedSampleData();

                if (isAdmin)
                {
                    Program.NavigateTo(new AdminHome());
                }
                else
                {
                    Program.NavigateTo(new Home());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private void EnsureDefaultUsers()
        {
            using var conn = Database.GetConnection();
            conn.Open();

            // Check if admin exists
            var adminCmd = new MySqlCommand("SELECT COUNT(*) FROM Admin WHERE Email = 'admin@evaluateach.edu'", conn);
            var adminCount = Convert.ToInt32(adminCmd.ExecuteScalar());

            if (adminCount == 0)
            {
                // Create default admin
                var insertAdmin = new MySqlCommand(@"
                    INSERT INTO Admin (FirstName, LastName, Email, AccessLevel, Password)
                    VALUES ('System', 'Admin', 'admin@evaluateach.edu', 'SuperAdmin', @password)", conn);
                insertAdmin.Parameters.AddWithValue("@password", HashPassword("admin123"));
                insertAdmin.ExecuteNonQuery();
            }

            // Check if student exists
            var studentCmd = new MySqlCommand("SELECT COUNT(*) FROM Student WHERE IDNumber = '2024-000001'", conn);
            var studentCount = Convert.ToInt32(studentCmd.ExecuteScalar());

            if (studentCount == 0)
            {
                // Create default student
                var insertStudent = new MySqlCommand(@"
                    INSERT INTO Student (IDNumber, FirstName, LastName, Email, Course, YearLevel, Password)
                    VALUES ('2024-000001', 'Juan', 'Dela Cruz', 'juan@student.edu', 'BSIT', 2, @password)", conn);
                insertStudent.Parameters.AddWithValue("@password", HashPassword("student123"));
                insertStudent.ExecuteNonQuery();
            }

            // Create another test student for BSCS
            var studentCmd2 = new MySqlCommand("SELECT COUNT(*) FROM Student WHERE IDNumber = '2024-000002'", conn);
            var studentCount2 = Convert.ToInt32(studentCmd2.ExecuteScalar());

            if (studentCount2 == 0)
            {
                var insertStudent2 = new MySqlCommand(@"
                    INSERT INTO Student (IDNumber, FirstName, LastName, Email, Course, YearLevel, Password)
                    VALUES ('2024-000002', 'Maria', 'Santos', 'maria@student.edu', 'BSCS', 3, @password)", conn);
                insertStudent2.Parameters.AddWithValue("@password", HashPassword("student123"));
                insertStudent2.ExecuteNonQuery();
            }
        }

        private (bool isValid, bool isAdmin, int? numericId, string name, string email, string course) AuthenticateUser(string userId, string password)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string hashedPassword = HashPassword(password);

            // First, try to find admin by AdminID or Email
            var adminCmd = new MySqlCommand(@"
                SELECT AdminID, FirstName, LastName, Email
                FROM Admin
                WHERE (AdminID = @id OR Email = @id) AND Password = @password
                LIMIT 1", conn);
            adminCmd.Parameters.AddWithValue("@id", userId);
            adminCmd.Parameters.AddWithValue("@password", hashedPassword);

            using (var adminReader = adminCmd.ExecuteReader())
            {
                if (adminReader.Read())
                {
                    int adminId = adminReader.GetInt32("AdminID");
                    string name = $"{adminReader.GetString("FirstName")} {adminReader.GetString("LastName")}";
                    string email = adminReader.GetString("Email");
                    return (true, true, adminId, name, email, "");
                }
            }

            // If not admin, try to find student by IDNumber
            var studentCmd = new MySqlCommand(@"
                SELECT StudentID, FirstName, LastName, Email, Course
                FROM Student
                WHERE IDNumber = @id AND Password = @password
                LIMIT 1", conn);
            studentCmd.Parameters.AddWithValue("@id", userId);
            studentCmd.Parameters.AddWithValue("@password", hashedPassword);

            using (var studentReader = studentCmd.ExecuteReader())
            {
                if (studentReader.Read())
                {
                    int studentId = studentReader.GetInt32("StudentID");
                    string name = $"{studentReader.GetString("FirstName")} {studentReader.GetString("LastName")}";
                    string email = studentReader.GetString("Email");
                    string course = studentReader.GetString("Course");
                    return (true, false, studentId, name, email, course);
                }
            }

            return (false, false, null, "", "", "");
        }
    }
}
