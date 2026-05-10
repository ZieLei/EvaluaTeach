using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class TeacherManagement : Form
    {
        private readonly Panel header = new();
        private readonly FlowLayoutPanel teachersList = new();
        private readonly TextBox firstNameInput = new();
        private readonly TextBox lastNameInput = new();
        private readonly TextBox emailInput = new();
        private readonly TextBox passwordInput = new();
        private readonly TextBox subjectsInput = new();
        private readonly ComboBox departmentSelector = new();

        public TeacherManagement()
        {
            ConfigureUi();
            LoadTeachers();
            TeacherStore.TeachersUpdated += OnTeachersUpdated;
            FormClosed += (_, _) => TeacherStore.TeachersUpdated -= OnTeachersUpdated;
        }

        private void ConfigureUi()
        {
            Text = "EvaluaTeach - Teacher Management";
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 251);

            ConfigureHeader();
            ConfigureAddSection();
            ConfigureTeachersList();
        }

        private void ConfigureHeader()
        {
            header.BackColor = Color.White;
            header.Dock = DockStyle.Top;
            header.Height = 60;
            header.Padding = new Padding(24, 16, 24, 16);

            var backBtn = new Button
            {
                Text = "Back",
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(80, 36),
                Location = new Point(24, 12)
            };
            backBtn.Click += (_, _) => Close();

            var title = new Label
            {
                Text = "Manage Teachers",
                Font = new Font("Inter", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(120, 18)
            };

            header.Controls.Add(backBtn);
            header.Controls.Add(title);
            Controls.Add(header);
        }

        private void ConfigureAddSection()
        {
            var addPanel = new Panel
            {
                BackColor = Color.White,
                Location = new Point(24, 84),
                Size = new Size(852, 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var sectionTitle = new Label
            {
                Text = "Add New Teacher",
                Font = new Font("Inter", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            // First Name
            var firstNameLabel = new Label
            {
                Text = "First Name *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(24, 55)
            };

            firstNameInput.Location = new Point(24, 75);
            firstNameInput.Size = new Size(180, 28);
            firstNameInput.Font = new Font("Inter", 10F);
            firstNameInput.BorderStyle = BorderStyle.FixedSingle;
            firstNameInput.BackColor = Color.FromArgb(248, 250, 252);

            // Last Name
            var lastNameLabel = new Label
            {
                Text = "Last Name *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(220, 55)
            };

            lastNameInput.Location = new Point(220, 75);
            lastNameInput.Size = new Size(180, 28);
            lastNameInput.Font = new Font("Inter", 10F);
            lastNameInput.BorderStyle = BorderStyle.FixedSingle;
            lastNameInput.BackColor = Color.FromArgb(248, 250, 252);

            // Email
            var emailLabel = new Label
            {
                Text = "Email *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(420, 55)
            };

            emailInput.Location = new Point(420, 75);
            emailInput.Size = new Size(200, 28);
            emailInput.Font = new Font("Inter", 10F);
            emailInput.BorderStyle = BorderStyle.FixedSingle;
            emailInput.BackColor = Color.FromArgb(248, 250, 252);

            // Department
            var deptLabel = new Label
            {
                Text = "Department *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(24, 115)
            };

            departmentSelector.Location = new Point(24, 135);
            departmentSelector.Size = new Size(180, 28);
            departmentSelector.Font = new Font("Inter", 10F);
            departmentSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            departmentSelector.Items.AddRange(new[] { "BSIT", "BSCS", "BSCE", "BSEE", "BSME", "BSN", "BSA", "BSBA", "Education", "Science", "Engineering", "Other" });
            departmentSelector.SelectedIndex = 0;

            // Password
            var passwordLabel = new Label
            {
                Text = "Password *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(220, 115)
            };

            passwordInput.Location = new Point(220, 135);
            passwordInput.Size = new Size(180, 28);
            passwordInput.Font = new Font("Inter", 10F);
            passwordInput.BorderStyle = BorderStyle.FixedSingle;
            passwordInput.BackColor = Color.FromArgb(248, 250, 252);
            passwordInput.UseSystemPasswordChar = true;

            // Subjects
            var subjectsLabel = new Label
            {
                Text = "Subjects (comma-separated) *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(420, 115)
            };

            subjectsInput.Location = new Point(420, 135);
            subjectsInput.Size = new Size(200, 28);
            subjectsInput.Font = new Font("Inter", 10F);
            subjectsInput.BorderStyle = BorderStyle.FixedSingle;
            subjectsInput.BackColor = Color.FromArgb(248, 250, 252);
            subjectsInput.PlaceholderText = "e.g. Math, Physics";

            // Add Button
            var addBtn = new Button
            {
                Text = "+ Add Teacher",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(120, 36),
                Location = new Point(640, 130)
            };
            addBtn.Click += AddTeacher;

            addPanel.Controls.AddRange(new Control[]
            {
                sectionTitle, firstNameLabel, firstNameInput,
                lastNameLabel, lastNameInput, emailLabel, emailInput,
                deptLabel, departmentSelector, passwordLabel, passwordInput,
                subjectsLabel, subjectsInput, addBtn
            });

            Controls.Add(addPanel);
        }

        private void ConfigureTeachersList()
        {
            var listPanel = new Panel
            {
                BackColor = Color.White,
                Location = new Point(24, 300),
                Size = new Size(852, ClientSize.Height - 324),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true
            };

            var listTitle = new Label
            {
                Text = "All Teachers",
                Font = new Font("Inter", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            teachersList.FlowDirection = FlowDirection.TopDown;
            teachersList.WrapContents = false;
            teachersList.AutoScroll = true;
            teachersList.BackColor = Color.White;
            teachersList.Location = new Point(24, 55);
            teachersList.Size = new Size(804, listPanel.Height - 80);
            teachersList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            listPanel.Controls.Add(listTitle);
            listPanel.Controls.Add(teachersList);
            Controls.Add(listPanel);
        }

        private void LoadTeachers()
        {
            teachersList.Controls.Clear();
            var teachers = TeacherStore.GetAllTeachers();

            if (teachers.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "No teachers added yet. Add your first teacher above.",
                    Font = new Font("Inter", 11F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(16, 16)
                };
                teachersList.Controls.Add(emptyLabel);
                return;
            }

            foreach (var teacher in teachers)
            {
                var card = CreateTeacherCard(teacher);
                teachersList.Controls.Add(card);
            }
        }

        private Panel CreateTeacherCard(Teacher teacher)
        {
            var card = new Panel
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Size = new Size(780, 100),
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(16)
            };

            var nameLabel = new Label
            {
                Text = teacher.FullName,
                Font = new Font("Inter SemiBold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(16, 12)
            };

            var subjectsLabel = new Label
            {
                Text = $"Subjects: {teacher.SubjectsDisplay}",
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(16, 36)
            };

            var emailLabel = new Label
            {
                Text = teacher.Email,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(16, 56)
            };

            var deptBadge = new Label
            {
                Text = teacher.Department,
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(38, 166, 91),
                BackColor = Color.FromArgb(220, 252, 231),
                AutoSize = true,
                Padding = new Padding(8, 4, 8, 4),
                Location = new Point(16, 76)
            };

            var deleteBtn = new Button
            {
                Text = "Delete",
                BackColor = Color.FromArgb(254, 226, 226),
                ForeColor = Color.FromArgb(185, 28, 28),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(70, 28),
                Location = new Point(690, 36)
            };
            deleteBtn.Click += (_, _) => DeleteTeacher(teacher.TeacherID, teacher.FullName);

            card.Controls.Add(nameLabel);
            card.Controls.Add(subjectsLabel);
            card.Controls.Add(emailLabel);
            card.Controls.Add(deptBadge);
            card.Controls.Add(deleteBtn);

            return card;
        }

        private void AddTeacher(object? sender, EventArgs e)
        {
            string firstName = firstNameInput.Text.Trim();
            string lastName = lastNameInput.Text.Trim();
            string email = emailInput.Text.Trim();
            string password = passwordInput.Text;
            string department = departmentSelector.SelectedItem?.ToString() ?? "";
            string subjectsText = subjectsInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Please enter both first and last name.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Weak Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(subjectsText))
            {
                MessageBox.Show("Please enter at least one subject.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (TeacherStore.EmailExists(email))
            {
                MessageBox.Show("A teacher with this email already exists.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var subjects = subjectsText.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();

            var teacher = new Teacher
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Department = department,
                Subjects = subjects
            };

            try
            {
                TeacherStore.AddTeacher(teacher, password);
                MessageBox.Show($"Teacher '{teacher.FullName}' added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear inputs
                firstNameInput.Clear();
                lastNameInput.Clear();
                emailInput.Clear();
                passwordInput.Clear();
                subjectsInput.Clear();
                departmentSelector.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding teacher: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteTeacher(int teacherId, string fullName)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete '{fullName}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    TeacherStore.DeleteTeacher(teacherId);
                    MessageBox.Show("Teacher deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting teacher: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnTeachersUpdated()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(LoadTeachers));
                return;
            }
            LoadTeachers();
        }
    }
}
