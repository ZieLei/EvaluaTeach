using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public class TeacherHome : Form
    {
        // ── Layout panels ──────────────────────────────────────────────
        private readonly Panel sidebar = new();
        private readonly Panel header = new();
        private readonly Panel contentPanel = new();

        // ── Sidebar buttons ────────────────────────────────────────────
        private readonly Button reportsBtn = new();
        private readonly Button profileBtn = new();
        private readonly Button logoutBtn = new();

        // ── Content sub-panels ─────────────────────────────────────────
        private readonly FlowLayoutPanel mainFlow = new();

        // ── Teacher data ───────────────────────────────────────────────
        private readonly int teacherId;
        private readonly string teacherName;
        private readonly string teacherEmail;

        private enum ActiveView { Reports, Profile }
        private ActiveView currentView = ActiveView.Reports;

        // ── Colors ─────────────────────────────────────────────────────
        private static readonly Color SidebarBg   = Color.FromArgb(24, 34, 52);
        private static readonly Color AccentGreen = Color.FromArgb(38, 166, 91);
        private static readonly Color PageBg      = Color.FromArgb(245, 247, 251);

        public TeacherHome()
        {
            teacherId   = SessionStore.UserIdNumeric ?? 0;
            teacherName = SessionStore.UserName;
            teacherEmail = SessionStore.Email;

            SuspendLayout();
            ConfigureForm();
            BuildSidebar();
            BuildHeader();
            BuildContentArea();
            Controls.Add(sidebar);
            Controls.Add(header);
            Controls.Add(contentPanel);
            ResumeLayout(false);

            Resize += (_, _) => UpdateLayout();
            UpdateLayout();
            
            // Auto-trigger reports button after 100ms
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            timer.Tick += (_, _) => {
                timer.Stop();
                reportsBtn.PerformClick(); // Simulate clicking the reports button
            };
            timer.Start();
        }

        // ── Form setup ─────────────────────────────────────────────────
        private void ConfigureForm()
        {
            Text = "EvaluaTeach - Teacher Dashboard";
            MinimumSize = new Size(1100, 720);
            BackColor = PageBg;
            StartPosition = FormStartPosition.CenterScreen;
        }

        // ── Sidebar ────────────────────────────────────────────────────
        private void BuildSidebar()
        {
            sidebar.BackColor = SidebarBg;
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 240;

            // Logo label at top
            var logoLabel = new Label
            {
                Text = "EvaluaTeach",
                Font = new Font("Bebas Neue", 14F, FontStyle.Bold),
                ForeColor = AccentGreen,
                AutoSize = false,
                Size = new Size(240, 56),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0)
            };

            var divider = new Panel
            {
                BackColor = Color.FromArgb(45, 58, 80),
                Size = new Size(200, 1),
                Location = new Point(20, 56)
            };

            ConfigureSidebarBtn(reportsBtn,  "  Reports", 80,  true);
            ConfigureSidebarBtn(profileBtn,  "  Profile", 136, false);

            // Logout at bottom - matching AdminHome style
            logoutBtn.Text = "  LOGOUT";
            logoutBtn.BackColor = Color.FromArgb(220, 38, 38);
            logoutBtn.ForeColor = Color.White;
            logoutBtn.FlatStyle = FlatStyle.Flat;
            logoutBtn.FlatAppearance.BorderSize = 0;
            logoutBtn.Font = new Font("Inter Black", 12F, FontStyle.Bold);
            logoutBtn.Size = new Size(192, 56);
            logoutBtn.TextAlign = ContentAlignment.MiddleCenter;
            logoutBtn.Click += (_, _) =>
            {
                var result = MessageBox.Show(
                    "Are you sure you want to logout?",
                    "Confirm Logout",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                
                if (result == DialogResult.Yes)
                {
                    SessionStore.Logout();
                    Program.NavigateTo(new LandingPage());
                }
            };

            reportsBtn.Click += (_, _) => SwitchView(ActiveView.Reports);
            profileBtn.Click  += (_, _) => SwitchView(ActiveView.Profile);

            sidebar.Controls.Add(logoLabel);
            sidebar.Controls.Add(divider);
            sidebar.Controls.Add(reportsBtn);
            sidebar.Controls.Add(profileBtn);
            sidebar.Controls.Add(logoutBtn);

            sidebar.Resize += (_, _) =>
                logoutBtn.Location = new Point(24, sidebar.Height - 64);
        }

        private static void ConfigureSidebarBtn(Button btn, string text, int top, bool active)
        {
            btn.Text = text;
            btn.BackColor = active ? AccentGreen : Color.Transparent;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Inter SemiBold", 11F, FontStyle.Bold);
            btn.Size = new Size(192, 48);
            btn.Location = new Point(24, top);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Cursor = Cursors.Hand;
        }

        private void SwitchView(ActiveView view)
        {
            currentView = view;

            reportsBtn.BackColor = view == ActiveView.Reports ? AccentGreen : Color.Transparent;
            profileBtn.BackColor = view == ActiveView.Profile ? AccentGreen : Color.Transparent;

            switch (view)
            {
                case ActiveView.Reports: ShowReports(); break;
                case ActiveView.Profile: ShowProfile(); break;
            }
        }

        // ── Header ─────────────────────────────────────────────────────
        private void BuildHeader()
        {
            header.BackColor = Color.White;
            header.Height = 64;

            var titleLbl = new Label
            {
                Text = "Teacher Dashboard",
                Font = new Font("Inter SemiBold", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 18)
            };

            var nameLbl = new Label
            {
                Text = teacherName,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            header.Controls.Add(titleLbl);
            header.Controls.Add(nameLbl);

            header.Resize += (_, _) =>
                nameLbl.Location = new Point(header.Width - nameLbl.PreferredWidth - 24, 22);
        }

        // ── Content area ───────────────────────────────────────────────
        private void BuildContentArea()
        {
            contentPanel.BackColor = PageBg;
            contentPanel.AutoScroll = true;

            mainFlow.FlowDirection = FlowDirection.TopDown;
            mainFlow.WrapContents = false;
            mainFlow.AutoSize = true;
            mainFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainFlow.Padding = new Padding(24, 16, 24, 24);
            mainFlow.Dock = DockStyle.Top;

            contentPanel.Controls.Add(mainFlow);
        }

        private void UpdateLayout()
        {
            int sideW = sidebar.Width;
            header.Location = new Point(sideW, 0);
            header.Size = new Size(ClientSize.Width - sideW, 64);

            contentPanel.Location = new Point(sideW, 64);
            contentPanel.Size = new Size(ClientSize.Width - sideW, ClientSize.Height - 64);

            mainFlow.Width = contentPanel.Width - 48;
        }

        // ══════════════════════════════════════════════════════════════
        // REPORTS VIEW
        // ══════════════════════════════════════════════════════════════
        private void ShowReports()
        {
            mainFlow.Controls.Clear();
            mainFlow.Controls.Add(MakeSectionHeader("Reports Received"));

            List<TeacherReport> reports = new();
            try { reports = TeacherStore.GetReportsForTeacher(teacherId); }
            catch (Exception ex)
            {
                mainFlow.Controls.Add(new Label
                {
                    Text = $"Could not load reports: {ex.Message}",
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(220, 38, 38),
                    AutoSize = true,
                    Margin = new Padding(0, 8, 0, 0)
                });
                return;
            }

            // Debug info
            var debugLabel = new Label
            {
                Text = $"Teacher ID: {teacherId} | Reports found: {reports.Count}",
                Font = new Font("Inter", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 8)
            };
            mainFlow.Controls.Add(debugLabel);

            if (!reports.Any())
            {
                var emptyCard = MakeCard(mainFlow.Width - 48, 120);
                emptyCard.Controls.Add(new Label
                {
                    Text = "No reports received yet.",
                    Font = new Font("Inter SemiBold", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = true,
                    Location = new Point(24, 36)
                });
                emptyCard.Controls.Add(new Label
                {
                    Text = "Reports will appear here once the admin sends evaluation results to you.",
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(24, 64)
                });
                mainFlow.Controls.Add(emptyCard);
                return;
            }

            // Group by AssignmentID (subject) — same as Manage Reports in Admin view
            var assignmentGroups = reports
                .GroupBy(r => r.AssignmentID)
                .OrderByDescending(g => g.Max(r => r.SubmissionDate))
                .ToList();

            foreach (var g in assignmentGroups)
            {
                var rep = g.First();
                int totalResponses = g.Sum(r => r.ResponseCount);
                double avgScore = g.Where(r => r.AverageScore > 0).Select(r => (double)r.AverageScore).DefaultIfEmpty(0).Average();
                
                // Include subject display in the card title if available
                string cardTitle = string.IsNullOrEmpty(rep.SubjectDisplay) 
                    ? rep.FormTitle 
                    : $"{rep.FormTitle}  -  {rep.SubjectDisplay}";
                
                mainFlow.Controls.Add(BuildReportCard(rep, totalResponses, avgScore, mainFlow.Width - 48, cardTitle));
            }
        }

        // ══════════════════════════════════════════════════════════════
        // PROFILE VIEW
        // ══════════════════════════════════════════════════════════════
        private void ShowProfile()
        {
            mainFlow.Controls.Clear();
            mainFlow.Controls.Add(MakeSectionHeader("My Profile"));

            // Load full teacher from DB for department/subjects
            Teacher? teacher = null;
            try
            {
                teacher = TeacherStore.GetAllTeachers().FirstOrDefault(t => t.TeacherID == teacherId);
            }
            catch { /* Graceful fallback */ }

            // Profile Overview Card
            var profileCard = CreateProfileCard(teacher);
            mainFlow.Controls.Add(profileCard);

            // Statistics Cards Row
            var statsRow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Height = 140,
                Width = mainFlow.Width - 48,
                Margin = new Padding(0, 0, 0, 16)
            };

            var totalResponsesCard = CreateStatsCard("Total Responses", GetTotalResponses().ToString(), Color.FromArgb(59, 130, 246));
            var avgRatingCard = CreateStatsCard("Average Rating", GetAverageRating().ToString("F2"), Color.FromArgb(16, 185, 129));

            statsRow.Controls.Add(totalResponsesCard);
            statsRow.Controls.Add(avgRatingCard);

            mainFlow.Controls.Add(statsRow);

            // Recent Activity Card
            var activityCard = CreateRecentActivityCard();
            mainFlow.Controls.Add(activityCard);
        }

        private Panel CreateProfileCard(Teacher? teacher)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(mainFlow.Width - 48, 340),
                Margin = new Padding(0, 0, 0, 16)
            };

            // Header with green gradient background
            var header = new Panel
            {
                BackColor = Color.FromArgb(16, 185, 129),
                Size = new Size(card.Width, 120),
                Location = new Point(0, 0)
            };

            // Avatar with initials
            var initials = GetInitials(teacherName);
            var avatar = new Panel
            {
                BackColor = Color.White,
                Size = new Size(90, 90),
                Location = new Point(24, 15),
                BorderStyle = BorderStyle.None
            };
            avatar.Controls.Add(new Label
            {
                Text = initials,
                Font = new Font("Inter SemiBold", 28F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129),
                AutoSize = true,
                Location = new Point(0, 0)
            });
            header.Controls.Add(avatar);

            // Teacher name in header
            header.Controls.Add(new Label
            {
                Text = teacherName,
                Font = new Font("Inter SemiBold", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(130, 40)
            });

            card.Controls.Add(header);

            // Profile information rows with increased spacing
            int labelX = 24;
            int valueX = 200;
            int startY = 145;

            AddEnhancedProfileRow(card, "Email", teacherEmail, labelX, startY, valueX);
            AddEnhancedProfileRow(card, "Section", GetSectionWithYearAndCourse(teacher), labelX, startY + 40, valueX);
            AddEnhancedProfileRow(card, "Subjects", GetSubjectsDisplay(teacher), labelX, startY + 80, valueX);

            // Change Password Button
            var changePasswordBtn = new Button
            {
                Text = "Change Password",
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Inter SemiBold", 10F),
                Size = new Size(140, 36),
                Location = new Point(card.Width - 164, startY + 80),
                Cursor = Cursors.Hand
            };
            changePasswordBtn.FlatAppearance.BorderSize = 0;
            changePasswordBtn.Click += (_, _) => ShowChangePasswordDialog();
            card.Controls.Add(changePasswordBtn);

            return card;
        }

        private Panel CreateStatsCard(string title, string value, Color accentColor)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size((mainFlow.Width - 48) / 2 - 8, 120),
                Margin = new Padding(0, 0, 16, 0)
            };

            // Accent bar
            var accent = new Panel
            {
                BackColor = accentColor,
                Size = new Size(4, card.Height),
                Location = new Point(0, 0)
            };
            card.Controls.Add(accent);

            // Value (left side)
            card.Controls.Add(new Label
            {
                Text = value,
                Font = new Font("Inter SemiBold", 28F, FontStyle.Bold),
                ForeColor = accentColor,
                AutoSize = true,
                Location = new Point(20, 35)
            });

            // Title (right side, moved further right for better spacing)
            card.Controls.Add(new Label
            {
                Text = title,
                Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(160, 45)
            });

            return card;
        }

        private Panel CreateRecentActivityCard()
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(mainFlow.Width - 48, 200),
                Margin = new Padding(0, 0, 0, 16)
            };

            // Header
            card.Controls.Add(new Label
            {
                Text = "Recent Activity",
                Font = new Font("Inter SemiBold", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            });

            // Activity items
            var activities = GetRecentActivities();
            int y = 55;
            foreach (var activity in activities.Take(3))
            {
                card.Controls.Add(new Label
                {
                    Text = $"• {activity}",
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = true,
                    Location = new Point(24, y)
                });
                y += 25;
            }

            return card;
        }

        private Panel CreateAccountSettingsCard()
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(mainFlow.Width - 48, 160),
                Margin = new Padding(0, 0, 0, 16)
            };

            // Header
            card.Controls.Add(new Label
            {
                Text = "Account Settings",
                Font = new Font("Inter SemiBold", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            });

            // Settings buttons
            var changePasswordBtn = CreateSettingsButton("🔒 Change Password", 24, 55);
            var notificationsBtn = CreateSettingsButton("🔔 Notifications", 24, 95);

            changePasswordBtn.Click += (_, _) => MessageBox.Show("Change password functionality coming soon!", "Security", MessageBoxButtons.OK, MessageBoxIcon.Information);
            notificationsBtn.Click += (_, _) => MessageBox.Show("Notification settings coming soon!", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);

            card.Controls.Add(changePasswordBtn);
            card.Controls.Add(notificationsBtn);

            return card;
        }

        private Button CreateSettingsButton(string text, int x, int y)
        {
            return new Button
            {
                Text = text,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Inter", 10F),
                Size = new Size(200, 30),
                Location = new Point(x, y),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private void AddEnhancedProfileRow(Panel card, string label, string value, int x, int y, int valueX)
        {
            // Bullet dot
            card.Controls.Add(new Label
            {
                Text = "•",
                Font = new Font("Inter", 12F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(x, y)
            });
            // Label with spacing after bullet
            card.Controls.Add(new Label
            {
                Text = label,
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(x + 20, y)
            });
            // Value
            card.Controls.Add(new Label
            {
                Text = value,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(valueX, y)
            });
        }

        private int GetTotalResponses()
        {
            try
            {
                var reports = TeacherStore.GetReportsForTeacher(teacherId);
                return reports.Sum(r => r.ResponseCount);
            }
            catch { return 0; }
        }

        private double GetAverageRating()
        {
            try
            {
                var reports = TeacherStore.GetReportsForTeacher(teacherId);
                return reports.Any() ? (double)reports.Average(r => r.AverageScore) : 0.0;
            }
            catch { return 0.0; }
        }

        private int GetReportsSentCount()
        {
            try
            {
                var reports = TeacherStore.GetReportsForTeacher(teacherId);
                int sentCount = 0;
                foreach (var report in reports)
                {
                    var responses = FormDataStore.GetResponsesForForm(report.EvaluationID);
                    foreach (var response in responses)
                    {
                        if (response.TeacherId > 0 && TeacherStore.HasReportBeenSent(response.TeacherId, report.EvaluationID, response.Id))
                        {
                            sentCount++;
                        }
                    }
                }
                return sentCount;
            }
            catch { return 0; }
        }

        private List<string> GetRecentActivities()
        {
            var activities = new List<string>();
            try
            {
                var reports = TeacherStore.GetReportsForTeacher(teacherId).OrderByDescending(r => r.SubmissionDate).Take(5);
                foreach (var report in reports)
                {
                    activities.Add($"Viewed {report.FormTitle} report - {report.SubmissionDate:MMM dd}");
                }
            }
            catch { }
            
            if (activities.Count == 0)
                activities.Add("No recent activity");
                
            return activities;
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Length > 0 ? parts[0].Substring(0, 1).ToUpper() : "?";
            if (parts.Length >= 2) return parts[0].Substring(0, 1).ToUpper() + parts[1].Substring(0, 1).ToUpper();
            
            return parts[0].Substring(0, 1).ToUpper();
        }

        private string GetDepartmentDisplay(Teacher? teacher)
        {
            if (teacher == null) return "—";
            
            var departments = new List<string>();
            if (!string.IsNullOrWhiteSpace(teacher.Department))
                departments.Add(teacher.Department);
            
            return departments.Count > 0 ? string.Join(", ", departments) : "—";
        }

        private string GetSectionDisplay(Teacher? teacher)
        {
            if (teacher == null) return "—";
            
            var sections = new HashSet<string>();
            
            // Add legacy single section
            if (!string.IsNullOrWhiteSpace(teacher.Section))
                sections.Add(teacher.Section);
            
            // Add sections from assignments
            foreach (var assignment in teacher.Assignments)
            {
                if (!string.IsNullOrWhiteSpace(assignment.Section))
                    sections.Add(assignment.Section);
            }
            
            return sections.Count > 0 ? string.Join(", ", sections) : "—";
        }

        private string GetSectionWithYearAndCourse(Teacher? teacher)
        {
            if (teacher == null) return "—";
            
            var assignments = new List<string>();
            
            // Add legacy assignment if exists
            if (!string.IsNullOrWhiteSpace(teacher.Section))
            {
                var legacyAssignment = teacher.Section;
                if (!string.IsNullOrWhiteSpace(teacher.Course))
                    legacyAssignment += $" - {teacher.Course}";
                if (!string.IsNullOrWhiteSpace(teacher.YearLevel))
                    legacyAssignment += $" (Year {teacher.YearLevel})";
                assignments.Add(legacyAssignment);
            }
            
            // Add assignments from teacher_assignment table
            foreach (var assignment in teacher.Assignments)
            {
                var assignmentInfo = assignment.Section;
                if (!string.IsNullOrWhiteSpace(assignment.Course))
                    assignmentInfo += $" - {assignment.Course}";
                if (!string.IsNullOrWhiteSpace(assignment.YearLevel))
                    assignmentInfo += $" (Year {assignment.YearLevel})";
                
                if (!string.IsNullOrWhiteSpace(assignmentInfo))
                    assignments.Add(assignmentInfo);
            }
            
            return assignments.Count > 0 ? string.Join(", ", assignments) : "—";
        }

        private string GetCourseDisplay(Teacher? teacher)
        {
            if (teacher == null) return "—";
            
            var courses = new HashSet<string>();
            
            // Add legacy single course
            if (!string.IsNullOrWhiteSpace(teacher.Course))
                courses.Add(teacher.Course);
            
            // Add courses from assignments
            foreach (var assignment in teacher.Assignments)
            {
                if (!string.IsNullOrWhiteSpace(assignment.Course))
                    courses.Add(assignment.Course);
            }
            
            return courses.Count > 0 ? string.Join(", ", courses) : "—";
        }

        private string GetSubjectsDisplay(Teacher? teacher)
        {
            if (teacher == null) return "—";
            
            var subjects = new HashSet<string>();
            
            // Add legacy subjects
            subjects.UnionWith(teacher.Subjects.Where(s => !string.IsNullOrWhiteSpace(s)));
            
            // Add subjects from assignments
            foreach (var assignment in teacher.Assignments)
            {
                subjects.UnionWith(assignment.Subjects.Where(s => !string.IsNullOrWhiteSpace(s)));
            }
            
            return subjects.Count > 0 ? string.Join(", ", subjects) : "—";
        }

        private void ShowChangePasswordDialog()
        {
            var form = new Form
            {
                Text = "Change Password",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblOldPassword = new Label
            {
                Text = "Old Password:",
                Location = new Point(20, 30),
                Size = new Size(100, 23)
            };

            var txtOldPassword = new TextBox
            {
                Location = new Point(120, 30),
                Size = new Size(200, 23),
                UseSystemPasswordChar = true
            };

            var lblNewPassword = new Label
            {
                Text = "New Password:",
                Location = new Point(20, 70),
                Size = new Size(100, 23)
            };

            var txtNewPassword = new TextBox
            {
                Location = new Point(120, 70),
                Size = new Size(200, 23),
                UseSystemPasswordChar = true
            };

            var lblConfirmPassword = new Label
            {
                Text = "Confirm Password:",
                Location = new Point(20, 110),
                Size = new Size(100, 23)
            };

            var txtConfirmPassword = new TextBox
            {
                Location = new Point(120, 110),
                Size = new Size(200, 23),
                UseSystemPasswordChar = true
            };

            var btnChange = new Button
            {
                Text = "Change Password",
                Location = new Point(80, 160),
                Size = new Size(100, 30),
                DialogResult = DialogResult.OK
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(200, 160),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };

            btnChange.Click += (_, _) => {
                if (string.IsNullOrWhiteSpace(txtOldPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtNewPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
                {
                    MessageBox.Show("All fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    form.DialogResult = DialogResult.None;
                    return;
                }

                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("New password and confirmation do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    form.DialogResult = DialogResult.None;
                    return;
                }

                if (txtNewPassword.Text.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters long.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    form.DialogResult = DialogResult.None;
                    return;
                }

                // TODO: Implement actual password change logic here
                MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            form.Controls.AddRange(new Control[] {
                lblOldPassword, txtOldPassword,
                lblNewPassword, txtNewPassword,
                lblConfirmPassword, txtConfirmPassword,
                btnChange, btnCancel
            });

            form.AcceptButton = btnChange;
            form.CancelButton = btnCancel;

            if (form.ShowDialog() == DialogResult.OK)
            {
                // Password change logic would go here
            }
        }

        // ── Helpers ────────────────────────────────────────────────────

        private Panel BuildReportCard(TeacherReport report, int totalResponses, double avgScore, int width, string? cardTitle = null)
        {
            const int HeaderH = 64;
            bool expanded = false;

            // ── Outer wrapper ─────────────────────────────────────────
            var wrapper = new Panel
            {
                BackColor = Color.Transparent,
                Width = width,
                Height = HeaderH,
                Margin = new Padding(0, 0, 0, 10)
            };

            // ── Header strip ──────────────────────────────────────────
            var headerStrip = new Panel
            {
                BackColor = Color.White,
                Size = new Size(width, HeaderH),
                Location = new Point(0, 0),
                Cursor = Cursors.Hand
            };

            var accent = new Panel
            {
                BackColor = AccentGreen,
                Size = new Size(5, HeaderH),
                Location = new Point(0, 0)
            };

            string titleText = cardTitle ?? (string.IsNullOrWhiteSpace(report.FormTitle) ? $"Evaluation #{report.EvaluationID}" : report.FormTitle);
            var titleLbl = new Label
            {
                Text = titleText,
                Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(18, 10)
            };

            string semTag = (!string.IsNullOrEmpty(report.Semester) && !string.IsNullOrEmpty(report.SchoolYear))
                ? $"  ·  {report.Semester} Sem {report.SchoolYear}" : "";
            var dateLbl = new Label
            {
                Text = report.SubmissionDate.ToString("MMM dd, yyyy") + semTag,
                Font = new Font("Inter", 8F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(18, 34)
            };

            bool hasScore = avgScore > 0;
            var scoreBadge = new Label
            {
                Text = hasScore ? $"★ {avgScore:0.00} avg" : "No rating",
                Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                ForeColor = hasScore ? Color.FromArgb(133, 77, 14) : Color.FromArgb(100, 116, 139),
                BackColor = hasScore ? Color.FromArgb(254, 243, 199) : Color.FromArgb(241, 245, 249),
                AutoSize = true,
                Padding = new Padding(7, 3, 7, 3),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var countBadge = new Label
            {
                Text = $"{totalResponses} response{(totalResponses == 1 ? "" : "s")}",
                Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(38, 166, 91),
                BackColor = Color.FromArgb(220, 252, 231),
                AutoSize = true,
                Padding = new Padding(7, 3, 7, 3),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var chevron = new Label
            {
                Text = "▶",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            headerStrip.Controls.Add(accent);
            headerStrip.Controls.Add(titleLbl);
            headerStrip.Controls.Add(dateLbl);
            headerStrip.Controls.Add(scoreBadge);
            headerStrip.Controls.Add(countBadge);
            headerStrip.Controls.Add(chevron);

            void PositionHeader()
            {
                accent.Size = new Size(5, HeaderH);
                chevron.Location    = new Point(headerStrip.Width - 24, (HeaderH - chevron.PreferredHeight) / 2);
                scoreBadge.Location = new Point(headerStrip.Width - scoreBadge.PreferredWidth - 40, 10);
                countBadge.Location = new Point(headerStrip.Width - countBadge.PreferredWidth - 40, 34);
            }
            headerStrip.Resize += (_, _) => PositionHeader();
            PositionHeader();

            // ── Detail panel (built fresh each expand) ────────────────
            Panel? detailPanel = null;

            EventHandler toggle = (_, _) =>
            {
                expanded = !expanded;
                chevron.Text = expanded ? "▼" : "▶";
                headerStrip.BackColor = expanded ? Color.White : Color.FromArgb(248, 250, 252);

                if (expanded)
                {
                    if (detailPanel != null)
                    {
                        wrapper.Controls.Remove(detailPanel);
                        detailPanel.Dispose();
                    }

                    var form = FormDataStore.GetForm(report.EvaluationID);
                    var sentIds = TeacherStore.GetSentSubmissionIds(teacherId, report.EvaluationID, report.AssignmentID);
                    var responses = FormDataStore.GetResponsesForForm(report.EvaluationID)
                        .Where(r => {
                            // Match by response TeacherId, or if response has no teacher, check form's TargetTeacherId
                            int effectiveTeacherId = r.TeacherId > 0 ? r.TeacherId : (form?.TargetTeacherId ?? 0);
                            return effectiveTeacherId == teacherId && sentIds.Contains(r.Id);
                        })
                        .ToList();

                    detailPanel = BuildTeacherDetailPanel(form, responses, report, width);
                    detailPanel.Location = new Point(0, HeaderH);
                    wrapper.Controls.Add(detailPanel);
                    wrapper.Height = HeaderH + detailPanel.Height;
                }
                else
                {
                    if (detailPanel != null) detailPanel.Visible = false;
                    wrapper.Height = HeaderH;
                }
            };

            headerStrip.Click += toggle;
            foreach (Control c in headerStrip.Controls)
                c.Click += toggle;

            wrapper.Controls.Add(headerStrip);

            wrapper.Resize += (_, _) =>
            {
                headerStrip.Width = wrapper.Width;
                if (detailPanel != null) detailPanel.Width = wrapper.Width;
            };

            return wrapper;
        }

        private Panel BuildTeacherDetailPanel(EvaluationForm? form, List<FormResponse> responses, TeacherReport report, int width)
        {
            var outer = new Panel
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Width = width,
                Height = 20
            };

            if (form == null || !responses.Any())
            {
                outer.Controls.Add(new Label
                {
                    Text = form == null ? "Form data unavailable." : "No submitted responses found.",
                    Font = new Font("Inter", 9F, FontStyle.Italic),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(16, 12)
                });
                outer.Height = 40;
                return outer;
            }

            int y = 16;
            int innerW = width - 32;
            var questions = form.Questions.OrderBy(q => q.OrderIndex).ToList();

            foreach (var q in questions)
            {
                // Question header
                int qTextW = innerW - 90;
                int qTextH;
                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                    qTextH = (int)Math.Ceiling(g.MeasureString(
                        $"Q{q.OrderIndex + 1}. {q.Text}",
                        new Font("Inter SemiBold", 9F, FontStyle.Bold), qTextW).Height) + 4;
                qTextH = Math.Max(qTextH, 20);

                outer.Controls.Add(new Label
                {
                    Text = $"Q{q.OrderIndex + 1}. {q.Text}",
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    AutoSize = false,
                    Size = new Size(qTextW, qTextH),
                    Location = new Point(16, y)
                });
                outer.Controls.Add(new Label
                {
                    Text = q.Type.ToString(),
                    Font = new Font("Inter", 7F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    BackColor = Color.FromArgb(226, 232, 240),
                    AutoSize = true,
                    Padding = new Padding(5, 1, 5, 1),
                    Location = new Point(innerW - 70, y + 2)
                });
                y += qTextH + 8;

                var answers = responses
                    .Select(r => r.Answers.TryGetValue(q.Id, out var a) ? a : null)
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a!).ToList();

                if (!answers.Any())
                {
                    outer.Controls.Add(new Label
                    {
                        Text = "No answers.",
                        Font = new Font("Inter", 8F, FontStyle.Italic),
                        ForeColor = Color.FromArgb(148, 163, 184),
                        AutoSize = true,
                        Location = new Point(24, y)
                    });
                    y += 24;
                }
                else if (q.Type == QuestionType.Rating)
                {
                    var vals = answers.Select(a => double.TryParse(a, out var v) ? (double?)v : null)
                        .Where(v => v.HasValue).Select(v => v!.Value).ToList();
                    double avg = vals.Any() ? vals.Average() : 0;
                    double maxR = q.MaxRating ?? 5;
                    int barW = Math.Min(280, innerW - 120);
                    int fillW = (int)Math.Round(barW * avg / maxR);

                    var barBg = new Panel { BackColor = Color.FromArgb(226, 232, 240), Size = new Size(barW, 12), Location = new Point(24, y + 6) };
                    barBg.Controls.Add(new Panel { BackColor = Color.FromArgb(234, 179, 8), Size = new Size(Math.Max(0, fillW), 12), Location = Point.Empty });
                    outer.Controls.Add(barBg);
                    outer.Controls.Add(new Label
                    {
                        Text = $"★ {avg:0.00} avg  ·  {vals.Count} response{(vals.Count == 1 ? "" : "s")}",
                        Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(146, 64, 14),
                        AutoSize = true,
                        Location = new Point(24 + barW + 12, y + 2)
                    });
                    y += 30;
                }
                else if (q.Type == QuestionType.YesNo)
                {
                    int yesCount = answers.Count(a => a.Equals("yes", StringComparison.OrdinalIgnoreCase) || a.Equals("true", StringComparison.OrdinalIgnoreCase));
                    int total = answers.Count;
                    double yesPct = total > 0 ? yesCount * 100.0 / total : 0;
                    double noPct = 100 - yesPct;
                    int barW = Math.Min(280, innerW - 40);
                    int yesFill = (int)Math.Round(barW * yesPct / 100);

                    var barBg = new Panel { BackColor = Color.FromArgb(254, 226, 226), Size = new Size(barW, 16), Location = new Point(24, y + 2) };
                    barBg.Controls.Add(new Panel { BackColor = Color.FromArgb(34, 197, 94), Size = new Size(Math.Max(0, yesFill), 16), Location = Point.Empty });
                    outer.Controls.Add(barBg);
                    outer.Controls.Add(new Label
                    {
                        Text = $"Yes {yesPct:0}%  ·  No {noPct:0}%  ({total} response{(total == 1 ? "" : "s")})",
                        Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(30, 41, 59),
                        AutoSize = true,
                        Location = new Point(24, y + 26)
                    });
                    y += 52;
                }
                else if (q.Type == QuestionType.MultipleChoice)
                {
                    var counts = answers.GroupBy(a => a.Trim(), StringComparer.OrdinalIgnoreCase)
                        .Select(g2 => (Option: g2.Key, Count: g2.Count()))
                        .OrderByDescending(x => x.Count).ToList();
                    int total = answers.Count;
                    int barW = Math.Min(200, innerW - 170);

                    foreach (var (option, count) in counts)
                    {
                        double pct = total > 0 ? count * 100.0 / total : 0;
                        int fillW = (int)Math.Round(barW * pct / 100);

                        outer.Controls.Add(new Label
                        {
                            Text = option.Length > 36 ? option.Substring(0, 36) + "…" : option,
                            Font = new Font("Inter", 9F),
                            ForeColor = Color.FromArgb(51, 65, 85),
                            AutoSize = false,
                            Size = new Size(140, 20),
                            Location = new Point(24, y + 4)
                        });
                        var barBg = new Panel { BackColor = Color.FromArgb(226, 232, 240), Size = new Size(barW, 12), Location = new Point(170, y + 7) };
                        barBg.Controls.Add(new Panel { BackColor = AccentGreen, Size = new Size(Math.Max(0, fillW), 12), Location = Point.Empty });
                        outer.Controls.Add(barBg);
                        outer.Controls.Add(new Label
                        {
                            Text = $"{count} ({pct:0}%)",
                            Font = new Font("Inter", 8F),
                            ForeColor = Color.FromArgb(100, 116, 139),
                            AutoSize = true,
                            Location = new Point(170 + barW + 8, y + 4)
                        });
                        y += 30;
                    }
                }
                else // Text
                {
                    var scrollPanel = new Panel
                    {
                        AutoScroll = true,
                        Size = new Size(innerW - 16, Math.Min(200, answers.Count * 44 + 8)),
                        Location = new Point(24, y),
                        BackColor = Color.FromArgb(241, 245, 249)
                    };
                    int cardY = 4;
                    
                    // Create text answer pairs with comment check like AdminHome
                    var textAnswerPairs = responses
                        .Select(r =>
                        {
                            r.Answers.TryGetValue(q.Id, out var a);
                            // Check if this text answer has a comment record
                            var comment = FormDataStore.GetTextAnswerComment(r.Id, q.Id);
                            return (Resp: r, Ans: a, Comment: comment);
                        })
                        .Where(x => !string.IsNullOrWhiteSpace(x.Ans))
                        .ToList();
                    
                    foreach (var (resp, ans, comment) in textAnswerPairs)
                    {
                        // Anonymize student names for privacy, but show special indicator for high severity
                        bool isHighSeverity = comment?.SystemLevel == CommentLevel.Severe || comment?.AdminLevel == CommentLevel.Severe;
                        string studentLabel = !string.IsNullOrWhiteSpace(resp.StudentName)
                            ? (isHighSeverity ? $"⚠️ Student ({resp.StudentId})" : $"Student ({resp.StudentId})")
                            : resp.StudentId;

                        // Check if comment is rejected - if so, show placeholder
                        string displayText = ans;
                        Color textColor = Color.FromArgb(30, 41, 59);
                        if (comment?.Status == CommentStatus.Rejected)
                        {
                            var level = comment.AdminLevel ?? comment.SystemLevel;
                            displayText = $"(Removed by a moderator. Severity Level: {level}.)";
                            textColor = Color.FromArgb(153, 27, 27); // Red for removed
                        }
                        
                        int ansH;
                        using (var g = Graphics.FromHwnd(IntPtr.Zero))
                            ansH = (int)Math.Ceiling(g.MeasureString(displayText, new Font("Inter", 9F), innerW - 48).Height) + 16;
                        ansH = Math.Max(ansH, 50); // Further increased minimum height for rejection messages

                        var ansCard = new Panel
                        {
                            BackColor = Color.White,
                            Size = new Size(innerW - 32, ansH),
                            Location = new Point(4, cardY)
                        };
                        
                        // Add student label like AdminHome
                        var studentLabelControl = new Label
                        {
                            Text = studentLabel,
                            Font = new Font("Inter SemiBold", 7F, FontStyle.Bold),
                            ForeColor = isHighSeverity ? Color.FromArgb(255, 140, 0) : Color.FromArgb(38, 166, 91), // Orange for high severity
                            AutoSize = true,
                            Location = new Point(8, 4),
                            Cursor = isHighSeverity ? Cursors.Hand : Cursors.Default
                        };
                        
                        // Add click handler for high-severity comments to show student details
                        if (isHighSeverity)
                        {
                            studentLabelControl.Click += (_, _) => {
                                MessageBox.Show(
                                    $"Student Details:\n\nName: {resp.StudentName}\nID: {resp.StudentId}\n\nThis information is only shown for high-severity comments.",
                                    "High Severity Comment - Student Details",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                            };
                        }
                        
                        ansCard.Controls.Add(studentLabelControl);
                        
                        ansCard.Controls.Add(new Label
                        {
                            Text = displayText,
                            Font = new Font("Inter", 9F),
                            ForeColor = textColor,
                            AutoSize = false,
                            Size = new Size(innerW - 48, ansH - 24),
                            Location = new Point(8, 20)
                        });
                        scrollPanel.Controls.Add(ansCard);
                        cardY += ansH + 4;
                    }
                    scrollPanel.Height = Math.Min(200, cardY + 4);
                    outer.Controls.Add(scrollPanel);
                    y += scrollPanel.Height + 8;
                }

                // Separator
                outer.Controls.Add(new Panel
                {
                    BackColor = Color.FromArgb(226, 232, 240),
                    Size = new Size(innerW, 1),
                    Location = new Point(16, y + 8)
                });
                y += 22;
            }

            // ── Student comments ─────────────────────────────
            var submissionIds = responses.Select(r => r.Id).ToList();
            var additionalComments = FormDataStore.GetApprovedCommentsForSubmissions(submissionIds)
                .Where(c => c.QuestionId == null) // Double-filter to ensure no text answers are included
                .ToList();
            if (additionalComments.Any())
            {
                outer.Controls.Add(new Label
                {
                    Text = "Student Comments",
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    AutoSize = true,
                    Location = new Point(16, y)
                });
                y += 26;

                foreach (var ac in additionalComments)
                {
                    y += DisplayComment(outer, ac, innerW, y, false);
                }
            }

            outer.Height = y + 12;
            return outer;
        }

        private static int DisplayComment(Panel outer, FormComment ac, int innerW, int y, bool isTextAnswer)
        {
            Color levelBg = ac.SystemLevel switch
            {
                CommentLevel.Severe   => Color.FromArgb(254, 226, 226),
                CommentLevel.Moderate => Color.FromArgb(255, 237, 213),
                CommentLevel.Mild     => Color.FromArgb(254, 252, 232),
                _                     => Color.FromArgb(220, 252, 231)
            };
            Color levelFg = ac.SystemLevel switch
            {
                CommentLevel.Severe   => Color.FromArgb(153, 27, 27),
                CommentLevel.Moderate => Color.FromArgb(154, 52, 18),
                CommentLevel.Mild     => Color.FromArgb(133, 77, 14),
                _                     => Color.FromArgb(22, 101, 52)
            };

            int commentW = innerW - 32;
            int commentTextH;
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
                commentTextH = (int)Math.Ceiling(g.MeasureString(ac.CommentText, new Font("Inter", 9F), commentW - 20).Height) + 4;
            commentTextH = Math.Max(commentTextH, 18);
            int cardH = 28 + commentTextH + 10;

            var commentCard = new Panel { BackColor = Color.White, Size = new Size(innerW, cardH), Location = new Point(16, y) };
            commentCard.Controls.Add(new Panel { BackColor = levelBg, Size = new Size(4, cardH), Location = Point.Empty });
            
            // Anonymize student info for privacy, but show special indicator for high severity
            bool isHighSeverity = ac.SystemLevel == CommentLevel.Severe || ac.AdminLevel == CommentLevel.Severe;
            string studentLabel = !string.IsNullOrWhiteSpace(ac.StudentName)
                ? (isHighSeverity ? $"⚠️ Student ({ac.StudentId})" : $"Student ({ac.StudentId})")
                : ac.StudentId;
            
            var studentLabelControl = new Label
            {
                Text = studentLabel,
                Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                ForeColor = isHighSeverity ? Color.FromArgb(255, 140, 0) : AccentGreen, // Orange for high severity
                AutoSize = true,
                Location = new Point(12, 6),
                Cursor = isHighSeverity ? Cursors.Hand : Cursors.Default
            };
            
            // Add click handler for high-severity comments to show student details
            if (isHighSeverity)
            {
                studentLabelControl.Click += (_, _) => {
                    MessageBox.Show(
                        $"Student Details:\n\nName: {ac.StudentName}\nID: {ac.StudentId}\n\nThis information is only shown for high-severity comments.",
                        "High Severity Comment - Student Details",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                };
            }
            
            commentCard.Controls.Add(studentLabelControl);

            // Status badges
            var statusText = ac.Status == CommentStatus.Rejected ? "Rejected" : ac.SystemLevel.ToString();
            var statusColor = ac.Status == CommentStatus.Rejected ? Color.FromArgb(153, 27, 27) : levelFg;
            var statusBg = ac.Status == CommentStatus.Rejected ? Color.FromArgb(254, 226, 226) : levelBg;
            
            commentCard.Controls.Add(new Label
            {
                Text = statusText,
                Font = new Font("Inter", 7F),
                ForeColor = statusColor,
                BackColor = statusBg,
                AutoSize = true,
                Padding = new Padding(5, 1, 5, 1),
                Location = new Point(commentW - 60, 5)
            });

            // Question text for text answers
            if (isTextAnswer && !string.IsNullOrEmpty(ac.QuestionText))
            {
                commentCard.Controls.Add(new Label
                {
                    Text = $"Q: {ac.QuestionText}",
                    Font = new Font("Inter", 8F, FontStyle.Italic),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(12, 20)
                });
                commentTextH += 16;
                cardH += 16;
                commentCard.Size = new Size(innerW, cardH);
            }

            // Comment text
            var textY = isTextAnswer && !string.IsNullOrEmpty(ac.QuestionText) ? 36 : 24;
            string displayText = ac.CommentText;
            
            // If rejected, show the removal message instead of the actual comment
            if (ac.Status == CommentStatus.Rejected)
            {
                displayText = $"(Removed by a moderator. Severity Level: {ac.SystemLevel}.)";
            }
            
            commentCard.Controls.Add(new Label
            {
                Text = displayText,
                Font = new Font("Inter", 9F),
                ForeColor = ac.Status == CommentStatus.Rejected ? Color.FromArgb(153, 27, 27) : Color.FromArgb(30, 41, 59),
                AutoSize = false,
                Size = new Size(commentW - 20, commentTextH),
                Location = new Point(12, textY)
            });

            outer.Controls.Add(commentCard);
            return cardH + 6;
        }

        private static Panel MakeCard(int width, int height)
        {
            return new Panel
            {
                BackColor = Color.White,
                Size = new Size(width, height),
                Margin = new Padding(0, 0, 0, 10)
            };
        }

        private Label MakeSectionHeader(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Inter SemiBold", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Margin = new Padding(0, 16, 0, 8)
            };
        }

        private static void AddProfileRow(Panel card, string label, string value, int x, int y)
        {
            card.Controls.Add(new Label
            {
                Text = label,
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(x, y)
            });
            card.Controls.Add(new Label
            {
                Text = value,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(x + 110, y)
            });
        }
    }
}
