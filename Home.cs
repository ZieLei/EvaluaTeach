using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class Home : Form
    {
        private readonly Panel summaryPanel = new();
        private readonly Label summaryTitle = new();
        private readonly Label summaryBody = new();
        private readonly Label metricLabel1 = new();
        private readonly Label metricLabel2 = new();
        private readonly Label metricLabel3 = new();
        private readonly Label sectionSubtitle = new();
        private readonly Panel listContainer = new();
        private readonly Panel notificationsContainer = new();
        private readonly Label notificationsSubtitle = new();
        private readonly FlowLayoutPanel teachersPanel = new();
        private readonly ComboBox courseFilter = new();
        private readonly ComboBox yearFilter = new();
        private readonly ComboBox sectionFilter = new();
        private readonly Button clearFiltersBtn = new();
        private readonly Panel filterPanel = new();
        private bool dashboardLayoutInitialized;
        private bool showingNotifications;
        private bool applyingViewState;

        public Home()
        {
            InitializeComponent();
            ConfigureHomeUi();
            Resize += Home_Resize;
            Shown += Home_Shown;
            // subscribe to profile updates to keep header and avatar in sync
            ProfileStore.ProfileUpdated += OnProfileUpdated;
            // initialize header from store
            OnProfileUpdated();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Open the profile page and transfer saved profile info if available
            ProfilePage profile = new();

            // If the Home view has displayed user details in the header labels, pass them to the profile
            var name = label9?.Text ?? string.Empty;
            var meta = label10?.Text ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(meta))
            {
                profile.SetProfileInfo(name, meta, ProfileStore.Email, ProfileStore.StudentId, ProfileStore.DatabaseStudentId);
            }
            profile.Show();
        }

        private void OnProfileUpdated()
        {
            // Update header labels and avatar from the shared ProfileStore
            if (InvokeRequired)
            {
                Invoke(new Action(OnProfileUpdated));
                return;
            }

            label9.Text = ProfileStore.Name;
            label10.Text = ProfileStore.Meta;

            if (ProfileStore.Avatar != null)
            {
                // show avatar directly without any resizing
                try
                {
                    button7.BackgroundImage = ProfileStore.Avatar;
                    button7.BackgroundImageLayout = ImageLayout.Zoom;
                    button7.Text = string.Empty;
                }
                catch
                {
                    // ignore image errors
                }

                return;
            }

            button7.BackgroundImage = null;
            button7.Text = GetInitials(ProfileStore.Name);
        }

        private void ConfigureHomeUi()
        {
            MinimumSize = new Size(1100, 720);
            BackColor = Color.FromArgb(245, 247, 251);
            Text = "EvaluaTeach Home";

            panel1.BackColor = BackColor;
            panel1.Padding = new Padding(0, 0, 24, 24);

            flowLayoutPanel1.BackColor = Color.White;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.Padding = new Padding(24, 16, 24, 16);
            flowLayoutPanel1.Height = 84;

            flowLayoutPanel2.BackColor = Color.FromArgb(24, 34, 52);
            flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel2.Padding = new Padding(0, 22, 0, 22);
            flowLayoutPanel2.WrapContents = false;
            flowLayoutPanel2.Width = 84;

            flowLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel3.BackColor = Color.Transparent;
            flowLayoutPanel3.Padding = new Padding(0, 8, 0, 12);
            flowLayoutPanel3.WrapContents = false;
            flowLayoutPanel3.AutoScroll = true;

            textBox1.BackColor = Color.FromArgb(248, 250, 252);
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.ForeColor = Color.FromArgb(90, 99, 112);
            textBox1.Font = new Font("Inter", 10F, FontStyle.Regular);
            textBox1.Text = "Search teachers or subjects";

            label1.ForeColor = Color.FromArgb(38, 166, 91);
            label2.Font = new Font("Inter", 22F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(31, 41, 55);
            label2.Text = "Teacher Evaluation Dashboard";

            label4.ForeColor = Color.FromArgb(100, 116, 139);
            label5.ForeColor = Color.FromArgb(100, 116, 139);
            label6.ForeColor = Color.FromArgb(100, 116, 139);
            label4.Font = new Font("Inter SemiBold", 9F, FontStyle.Bold);
            label5.Font = new Font("Inter SemiBold", 9F, FontStyle.Bold);
            label6.Font = new Font("Inter SemiBold", 9F, FontStyle.Bold);

            panel2.BackColor = Color.White;
            panel2.Padding = new Padding(16);
            panel3.AutoSize = false;
            panel3.BackColor = Color.FromArgb(248, 250, 252);

            button4.FlatStyle = FlatStyle.Flat;
            button4.FlatAppearance.BorderSize = 0;
            button4.BackColor = Color.FromArgb(38, 166, 91);
            button4.Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold);

            StyleNavButton(button1, true);
            StyleNavButton(button2, false);
            StyleNavButton(button3, false);
            StyleIconButton(button6);
            StyleIconButton(button7);
            StyleIconButton(button8);
            StyleProfileAvatarButton();
            ConfigureDashboardPanels();
            ConfigureFilterPanel();
            ConfigureTeachersPanel();
            StyleTeacherCard();
            ConfigureNotificationsPanel();

            button1.Click += (_, _) => ShowDashboardView();
            button2.Click += (_, _) => ShowNotificationsView();
            button4.Click += (_, _) => OpenStudentForms();
            panel1.Layout += (_, _) => ApplyCurrentViewState(false);

            FormDataStore.FormsUpdated += OnFormsUpdated;
            FormDataStore.ResponsesUpdated += OnFormsUpdated;
            TeacherStore.TeachersUpdated += OnTeachersUpdated;
            FormClosed += (_, _) =>
            {
                FormDataStore.FormsUpdated -= OnFormsUpdated;
                FormDataStore.ResponsesUpdated -= OnFormsUpdated;
                TeacherStore.TeachersUpdated -= OnTeachersUpdated;
                ProfileStore.ProfileUpdated -= OnProfileUpdated;
            };

            ShowDashboardView();
            UpdateResponsiveLayout();
            UpdateMetrics();
            LoadTeachers();
            PopulateFilterDropdowns();
        }

        private void OpenStudentForms()
        {
            var formsView = new StudentFormsView();
            formsView.ShowDialog(this);
        }

        private void OnFormsUpdated()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(OnFormsUpdated));
                return;
            }
            UpdateMetrics();
            if (showingNotifications)
            {
                LoadNotifications();
            }
        }

        private void UpdateMetrics()
        {
            var department = ProfileStore.Meta.Replace("Student ", "").Trim();
            var forms = FormDataStore.GetFormsForStudent(department);
            var studentId = string.IsNullOrWhiteSpace(SessionStore.UserId)
                ? ProfileStore.StudentId
                : SessionStore.UserId;

            int total = forms.Count;
            int pending = forms.Count(f => !FormDataStore.HasStudentSubmitted(f.Id, studentId));
            int completed = total - pending;

            metricLabel1.Text = $"{total} Forms";
            metricLabel2.Text = $"{pending} Pending";
            metricLabel3.Text = $"{completed} Completed";

            // Metrics text changes can affect parent measurements in this manual layout.
            summaryPanel.PerformLayout();
            listContainer.PerformLayout();
            summaryPanel.Refresh();
            listContainer.Refresh();
        }

        private void ConfigureDashboardPanels()
        {
            summaryPanel.BackColor = Color.FromArgb(15, 23, 42);
            summaryPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            summaryPanel.Padding = new Padding(24);

            summaryTitle.AutoSize = true;
            summaryTitle.Font = new Font("Inter", 18F, FontStyle.Bold);
            summaryTitle.ForeColor = Color.White;
            summaryTitle.Text = $"Welcome back, {SessionStore.UserName ?? ProfileStore.Name}";
            SessionStore.SessionUpdated += () =>
            {
                summaryTitle.Text = $"Welcome back, {SessionStore.UserName}";
            };

            summaryBody.AutoSize = true;
            summaryBody.Font = new Font("Inter", 10F, FontStyle.Regular);
            summaryBody.ForeColor = Color.FromArgb(203, 213, 225);
            summaryBody.Text = "Track pending evaluations, explore teacher profiles, and submit thoughtful feedback in one clean space.";
            summaryBody.MaximumSize = new Size(620, 0);

            sectionSubtitle.AutoSize = true;
            sectionSubtitle.Font = new Font("Inter", 10F, FontStyle.Regular);
            sectionSubtitle.ForeColor = Color.FromArgb(100, 116, 139);
            sectionSubtitle.Text = "Review the available teachers and start an evaluation whenever you are ready.";

            listContainer.BackColor = Color.White;
            listContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listContainer.Padding = new Padding(20, 18, 20, 20);

            StyleMetric(metricLabel1, "12 Teachers");
            StyleMetric(metricLabel2, "4 Pending");
            StyleMetric(metricLabel3, "1 Completed");

            if (!summaryPanel.Controls.Contains(summaryTitle))
            {
                summaryPanel.Controls.Add(summaryTitle);
                summaryPanel.Controls.Add(summaryBody);
                summaryPanel.Controls.Add(metricLabel1);
                summaryPanel.Controls.Add(metricLabel2);
                summaryPanel.Controls.Add(metricLabel3);
            }

            if (!panel1.Controls.Contains(summaryPanel))
            {
                panel1.Controls.Add(summaryPanel);
                panel1.Controls.Add(sectionSubtitle);
                panel1.Controls.Add(listContainer);
                listContainer.Controls.Add(filterPanel);
                listContainer.Controls.Add(label4);
                listContainer.Controls.Add(label5);
                listContainer.Controls.Add(label6);
                listContainer.Controls.Add(flowLayoutPanel3);
                listContainer.Controls.Add(teachersPanel);
            }
        }

        private void ConfigureTeachersPanel()
        {
            teachersPanel.FlowDirection = FlowDirection.TopDown;
            teachersPanel.WrapContents = false;
            teachersPanel.AutoScroll = true;
            teachersPanel.BackColor = Color.White;
            teachersPanel.Padding = new Padding(0, 12, 0, 12);
            teachersPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            // Position below filter panel (filter panel is 50px tall)
            teachersPanel.Location = new Point(20, 70);
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

        private void LoadTeachers()
        {
            teachersPanel.Controls.Clear();
            
            // Get filter values from dropdowns
            var courseFilterValue = courseFilter.SelectedItem?.ToString();
            var yearFilterValue = yearFilter.SelectedItem?.ToString();
            var sectionFilterValue = sectionFilter.SelectedItem?.ToString();
            
            // Get filtered teachers based on selected criteria
            var teachers = TeacherStore.GetFilteredTeachersForStudent(courseFilterValue, yearFilterValue, sectionFilterValue);

            // Update metric
            metricLabel1.Text = $"{teachers.Count} Teachers";

            // Card width fills the teachers panel with padding
            int cardWidth = teachersPanel.Width - 24;

            if (teachers.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "No teachers available for the selected filters. Try adjusting your filters or clear them to see all teachers.",
                    Font = new Font("Inter", 11F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    MaximumSize = new Size(cardWidth - 40, 0),
                    Margin = new Padding(8)
                };
                teachersPanel.Controls.Add(emptyLabel);
                return;
            }

            foreach (var teacher in teachers)
            {
                var card = CreateTeacherCard(teacher, cardWidth);
                teachersPanel.Controls.Add(card);
            }
        }

        private Panel CreateTeacherCard(Teacher teacher, int cardWidth)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(cardWidth, 140),
                Margin = new Padding(0, 0, 0, 12),
                BorderStyle = BorderStyle.None
            };

            // Left accent bar with department color
            var accentBar = new Panel
            {
                BackColor = GetDepartmentColor(teacher.Department),
                Size = new Size(4, card.Height),
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            // Teacher initials avatar (larger like admin)
            var initialsPanel = new Panel
            {
                BackColor = GetDepartmentColor(teacher.Department),
                Size = new Size(50, 50),
                Location = new Point(16, 20),
                Region = new Region(CreateRoundedRectangle(0, 0, 50, 50, 25))
            };

            var initialsLabel = new Label
            {
                Text = GetInitials(teacher.FullName),
                Font = new Font("Inter SemiBold", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(50, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0)
            };
            initialsPanel.Controls.Add(initialsLabel);

            // Name label
            var nameLabel = new Label
            {
                Text = teacher.FullName,
                Font = new Font("Inter SemiBold", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(78, 16)
            };

            // Email label
            var emailLabel = new Label
            {
                Text = teacher.Email,
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(78, 40)
            };

            // Department badge (green like admin)
            var deptBadge = new Label
            {
                Text = teacher.Department,
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 101, 52),
                BackColor = Color.FromArgb(220, 252, 231),
                AutoSize = true,
                Padding = new Padding(8, 4, 8, 4),
                Location = new Point(78, 62)
            };

            // Build subjects text from all assignments
            var allSubjects = teacher.Assignments.SelectMany(a => a.Subjects).Distinct().ToList();
            if (allSubjects.Count == 0 && teacher.Subjects.Count > 0)
                allSubjects = teacher.Subjects;

            string subjectsText = allSubjects.Count > 0
                ? $"📚 {string.Join(", ", allSubjects.Take(3))}{(allSubjects.Count > 3 ? $" +{allSubjects.Count - 3} more" : "")}"
                : "No subjects assigned";

            // Subjects label
            var subjectsLabel = new Label
            {
                Text = subjectsText,
                Font = new Font("Inter", 9F),
                ForeColor = allSubjects.Count > 0 ? Color.FromArgb(71, 85, 105) : Color.FromArgb(239, 68, 68),
                AutoSize = true,
                Location = new Point(78, 90),
                MaximumSize = new Size(cardWidth - 240, 0)
            };

            // Assignment info (Course/Year/Section)
            var assignmentInfo = teacher.Assignments.FirstOrDefault();
            string classText = assignmentInfo != null
                ? $"{assignmentInfo.Course} · Year {assignmentInfo.YearLevel} · {assignmentInfo.Section}"
                : "No class assignments";

            var classLabel = new Label
            {
                Text = classText,
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(78, 110)
            };

            // View Forms button
            var viewFormsBtn = new Button
            {
                Text = "View Forms",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold),
                Size = new Size(118, 40),
                Location = new Point(card.Width - 140, 50)
            };
            viewFormsBtn.Click += (_, _) => OpenTeacherForms(teacher);

            // Card resize handler
            card.Resize += (_, _) =>
            {
                accentBar.Height = card.Height;
                viewFormsBtn.Location = new Point(card.Width - 140, 50);
                subjectsLabel.MaximumSize = new Size(card.Width - 240, 0);
            };

            card.Controls.Add(accentBar);
            card.Controls.Add(initialsPanel);
            card.Controls.Add(nameLabel);
            card.Controls.Add(emailLabel);
            card.Controls.Add(deptBadge);
            card.Controls.Add(subjectsLabel);
            card.Controls.Add(classLabel);
            card.Controls.Add(viewFormsBtn);

            return card;
        }

        private static GraphicsPath CreateRoundedRectangle(int x, int y, int width, int height, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(x, y, radius * 2, radius * 2, 180, 90);
            path.AddArc(x + width - radius * 2, y, radius * 2, radius * 2, 270, 90);
            path.AddArc(x + width - radius * 2, y + height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(x, y + height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static Color GetDepartmentColor(string department)
        {
            return department?.ToLower() switch
            {
                "education" => Color.FromArgb(38, 166, 91),
                "science" => Color.FromArgb(59, 130, 246),
                "engineering" => Color.FromArgb(249, 115, 22),
                "nursing" => Color.FromArgb(236, 72, 153),
                "business" => Color.FromArgb(139, 92, 246),
                _ => Color.FromArgb(100, 116, 139)
            };
        }

        private void OpenTeacherForms(Teacher teacher, int? assignmentId = null)
        {
            // Open the student forms view filtered for this teacher
            // If no assignment specified, use first available assignment for per-subject tracking
            if (!assignmentId.HasValue && teacher.Assignments.Count > 0)
            {
                assignmentId = teacher.Assignments.First().AssignmentID;
            }
            var formsView = new StudentFormsView(teacher.TeacherID, teacher.FullName, assignmentId);
            formsView.ShowDialog(this);
        }

        private void StyleMetric(Label label, string text)
        {
            label.AutoSize = false;
            label.BackColor = Color.FromArgb(30, 41, 59);
            label.ForeColor = Color.FromArgb(226, 232, 240);
            label.Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Text = text;
        }

        private void StyleTeacherCard()
        {
            label3.Font = new Font("Inter", 12F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(15, 23, 42);

            label7.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            label7.ForeColor = Color.FromArgb(51, 65, 85);

            label8.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            label8.ForeColor = Color.FromArgb(51, 65, 85);

            panel2.BorderStyle = BorderStyle.FixedSingle;
        }

        private void ConfigureNotificationsPanel()
        {
            notificationsSubtitle.AutoSize = true;
            notificationsSubtitle.Font = new Font("Inter", 10F, FontStyle.Regular);
            notificationsSubtitle.ForeColor = Color.FromArgb(100, 116, 139);
            notificationsSubtitle.Text = "Stay updated with pending evaluations and new forms from your department.";

            notificationsContainer.BackColor = Color.Transparent;
            notificationsContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            notificationsContainer.AutoScroll = true;

            if (!panel1.Controls.Contains(notificationsSubtitle))
            {
                panel1.Controls.Add(notificationsSubtitle);
                panel1.Controls.Add(notificationsContainer);
            }
        }

        private void LoadNotifications()
        {
            notificationsContainer.Controls.Clear();

            var department = ProfileStore.Meta.Replace("Student ", "").Trim();
            var studentId = string.IsNullOrWhiteSpace(SessionStore.UserId)
                ? ProfileStore.StudentId
                : SessionStore.UserId;

            var forms = FormDataStore.GetFormsForStudent(department);
            var pending = forms.Where(f => !FormDataStore.HasStudentSubmitted(f.Id, studentId)).ToList();
            var completed = forms.Where(f => FormDataStore.HasStudentSubmitted(f.Id, studentId)).ToList();

            int yOffset = 0;

            if (pending.Count > 0)
            {
                var pendingCard = BuildPendingCard(pending, notificationsContainer.Width);
                pendingCard.Location = new Point(0, yOffset);
                notificationsContainer.Controls.Add(pendingCard);
                yOffset += pendingCard.Height + 16;
            }

            var otherNotifications = new List<(string Title, string Body, string Time, Color Accent)>();

            foreach (var form in forms.OrderByDescending(f => f.CreatedAt).Take(5))
            {
                bool isNew = (DateTime.Now - form.CreatedAt).TotalDays <= 3;
                string timeStr = FormatRelativeTime(form.CreatedAt);
                string dueStr = form.DueDate.HasValue
                    ? $" Due: {form.DueDate.Value:MMM dd, yyyy}."
                    : string.Empty;
                string courseStr = form.TargetCourse == "All" ? "all departments" : form.TargetCourse;
                otherNotifications.Add((
                    (isNew ? "New form: " : "Form: ") + form.Title,
                    $"An evaluation form targeting {courseStr} is now available.{dueStr}",
                    timeStr,
                    isNew ? Color.FromArgb(38, 166, 91) : Color.FromArgb(100, 116, 139)));
            }

            foreach (var sub in completed.OrderByDescending(f => f.CreatedAt).Take(3))
            {
                var responses = FormDataStore.GetResponsesByStudent(studentId);
                var match = responses.FirstOrDefault(r => r.FormId == sub.Id);
                string timeStr = match != null ? FormatRelativeTime(match.SubmittedAt) : "Recently";
                otherNotifications.Add((
                    $"Submitted: {sub.Title}",
                    "Your evaluation was recorded successfully.",
                    timeStr,
                    Color.FromArgb(22, 163, 74)));
            }

            if (pending.Count == 0 && otherNotifications.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "No notifications yet. Check back after new forms are published.",
                    Font = new Font("Inter", 11F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(0, 16)
                };
                notificationsContainer.Controls.Add(emptyLabel);
                return;
            }

            foreach (var (title, body, time, accent) in otherNotifications)
            {
                var card = BuildNotificationCard(title, body, time, accent, notificationsContainer.Width);
                card.Location = new Point(0, yOffset);
                notificationsContainer.Controls.Add(card);
                yOffset += card.Height + 16;
            }
        }

        private Panel BuildPendingCard(List<EvaluationForm> pendingForms, int containerWidth)
        {
            bool expandable = pendingForms.Count > 1;
            string formWord = pendingForms.Count == 1 ? "evaluation" : "evaluations";
            string dueInfo = pendingForms
                .Where(f => f.DueDate.HasValue)
                .OrderBy(f => f.DueDate)
                .Select(f => $" Nearest due: {f.DueDate!.Value:MMM dd, yyyy}.")
                .FirstOrDefault() ?? string.Empty;

            int collapsedHeight = 118;
            var accentColor = Color.FromArgb(234, 88, 12);

            var card = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(Math.Max(300, containerWidth), collapsedHeight),
                Padding = new Padding(24)
            };

            var accent = new Panel
            {
                BackColor = accentColor,
                Size = new Size(4, card.Height),
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            var title = new Label
            {
                Text = $"You have {pendingForms.Count} pending {formWord}",
                Font = new Font("Inter", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            var body = new Label
            {
                Text = $"Complete your teacher evaluations before the deadline.{dueInfo}",
                Font = new Font("Inter", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                MaximumSize = new Size(Math.Max(200, card.Width - 72), 0),
                Location = new Point(24, 50)
            };

            var time = new Label
            {
                Text = "Just now",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(24, 82)
            };

            card.Controls.Add(accent);
            card.Controls.Add(title);
            card.Controls.Add(body);
            card.Controls.Add(time);

            if (!expandable)
            {
                card.Resize += (_, _) =>
                {
                    accent.Height = card.Height;
                    body.MaximumSize = new Size(Math.Max(200, card.Width - 72), 0);
                    time.Location = new Point(24, body.Bottom + 8);
                    card.Height = Math.Max(collapsedHeight, time.Bottom + 20);
                };
                return card;
            }

            bool expanded = false;

            var toggleBtn = new Button
            {
                Text = "\u25B6  Show details",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = accentColor,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent, MouseDownBackColor = Color.Transparent },
                AutoSize = true,
                Location = new Point(22, 94)
            };
            card.Controls.Add(toggleBtn);
            card.Height = collapsedHeight + toggleBtn.Height + 4;

            var detailsPanel = new Panel
            {
                BackColor = Color.FromArgb(255, 247, 237),
                Visible = false,
                Width = card.Width - 8,
                Location = new Point(4, card.Height)
            };
            card.Controls.Add(detailsPanel);

            int rowH = 0;
            foreach (var f in pendingForms)
            {
                var row = new Panel
                {
                    BackColor = Color.Transparent,
                    Width = detailsPanel.Width,
                    Height = 56,
                    Location = new Point(0, rowH)
                };

                var dot = new Panel
                {
                    BackColor = accentColor,
                    Size = new Size(8, 8),
                    Location = new Point(16, 24)
                };

                var rowTitle = new Label
                {
                    Text = f.Title,
                    Font = new Font("Inter", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    AutoSize = true,
                    Location = new Point(34, 10)
                };

                string descText = string.Empty;
                if (!string.IsNullOrWhiteSpace(f.Description))
                    descText += f.Description;
                if (f.DueDate.HasValue)
                    descText += (descText.Length > 0 ? "  \u2022  " : "") + $"Due: {f.DueDate.Value:MMM dd, yyyy}";
                if (string.IsNullOrEmpty(descText))
                    descText = "No due date set";

                var rowDesc = new Label
                {
                    Text = descText,
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    MaximumSize = new Size(detailsPanel.Width - 50, 0),
                    Location = new Point(34, rowTitle.Bottom + 2)
                };

                row.Controls.Add(dot);
                row.Controls.Add(rowTitle);
                row.Controls.Add(rowDesc);

                row.Height = Math.Max(56, rowDesc.Bottom + 12);
                dot.Location = new Point(16, row.Height / 2 - 4);

                detailsPanel.Controls.Add(row);
                rowH += row.Height;
            }
            detailsPanel.Height = rowH + 8;

            void RefreshLayout()
            {
                accent.Height = card.Height;
                body.MaximumSize = new Size(Math.Max(200, card.Width - 72), 0);
                time.Location = new Point(24, body.Bottom + 8);
                int baseHeight = Math.Max(collapsedHeight, time.Bottom + 20);
                toggleBtn.Location = new Point(22, baseHeight - 24);
                detailsPanel.Width = card.Width - 8;
                detailsPanel.Location = new Point(4, baseHeight + 4);
                foreach (Panel row in detailsPanel.Controls.OfType<Panel>())
                    foreach (Label lbl in row.Controls.OfType<Label>().Where(l => l.AutoSize && l.MaximumSize.Width > 0))
                        lbl.MaximumSize = new Size(detailsPanel.Width - 50, 0);
                card.Height = expanded
                    ? baseHeight + 4 + detailsPanel.Height + 8
                    : baseHeight + toggleBtn.Height + 4;
            }

            toggleBtn.Click += (_, _) =>
            {
                expanded = !expanded;
                detailsPanel.Visible = expanded;
                toggleBtn.Text = expanded ? "\u25BC  Hide details" : "\u25B6  Show details";
                RefreshLayout();
                LayoutNotificationCards(notificationsContainer.Width);
            };

            card.Resize += (_, _) => RefreshLayout();

            RefreshLayout();
            return card;
        }

        private Panel BuildNotificationCard(string titleText, string bodyText, string timeText, Color accentColor, int containerWidth)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(Math.Max(300, containerWidth), 118),
                Padding = new Padding(24)
            };

            var accent = new Panel
            {
                BackColor = accentColor,
                Size = new Size(4, card.Height),
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            var title = new Label
            {
                Text = titleText,
                Font = new Font("Inter", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            var body = new Label
            {
                Text = bodyText,
                Font = new Font("Inter", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                MaximumSize = new Size(Math.Max(200, card.Width - 72), 0),
                Location = new Point(24, 50)
            };

            var time = new Label
            {
                Text = timeText,
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(24, 82)
            };

            card.Controls.Add(accent);
            card.Controls.Add(title);
            card.Controls.Add(body);
            card.Controls.Add(time);

            card.Resize += (_, _) =>
            {
                accent.Height = card.Height;
                body.MaximumSize = new Size(Math.Max(200, card.Width - 72), 0);
                time.Location = new Point(24, body.Bottom + 8);
                card.Height = Math.Max(118, time.Bottom + 20);
            };

            return card;
        }

        private static string FormatRelativeTime(DateTime dt)
        {
            var diff = DateTime.Now - dt;
            if (diff.TotalMinutes < 1) return "Just now";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} min ago";
            if (diff.TotalHours < 24) return $"Today, {dt:h:mm tt}";
            if (diff.TotalDays < 2) return $"Yesterday, {dt:h:mm tt}";
            return dt.ToString("MMM dd, yyyy");
        }

        private void StyleNavButton(Button button, bool isActive)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button.BackColor = Color.Transparent;
            button.Margin = new Padding(16, 8, 16, 8);
            button.Padding = new Padding(6);
            button.Size = new Size(52, 52);
            button.UseVisualStyleBackColor = false;
        }

        private void StyleIconButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button.BackColor = Color.Transparent;
            button.UseVisualStyleBackColor = false;
        }

        private void StyleProfileAvatarButton()
        {
            button7.BackColor = Color.FromArgb(22, 163, 74);
            button7.ForeColor = Color.White;
            button7.Font = new Font("Inter", 12F, FontStyle.Bold);
            button7.TextAlign = ContentAlignment.MiddleCenter;
            button7.BackgroundImage = null;
            button7.Text = GetInitials(label9.Text);
        }

        private static string GetInitials(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "?";
            }

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return "?";
            }

            if (parts.Length == 1)
            {
                return char.ToUpperInvariant(parts[0][0]).ToString();
            }

            return string.Concat(
                char.ToUpperInvariant(parts[0][0]),
                char.ToUpperInvariant(parts[^1][0]));
        }

        private void Home_Shown(object? sender, EventArgs e)
        {
            BeginInvoke(new Action(EnsureDashboardInitialLayout));
            
            // Reload avatar from database to ensure we have the latest
            if (ProfileStore.DatabaseStudentId.HasValue)
            {
                ProfileStore.LoadAvatarFromDatabase(ProfileStore.DatabaseStudentId.Value);
            }
        }

        private void Home_Resize(object? sender, EventArgs e)
        {
            UpdateResponsiveLayout();
        }

        private void EnsureDashboardInitialLayout()
        {
            if (dashboardLayoutInitialized)
            {
                return;
            }

            dashboardLayoutInitialized = true;

            SuspendLayout();
            panel1.SuspendLayout();
            listContainer.SuspendLayout();
            flowLayoutPanel3.SuspendLayout();

            ShowDashboardView();
            UpdateMetrics();
            UpdateResponsiveLayout();

            flowLayoutPanel3.ResumeLayout(true);
            listContainer.ResumeLayout(true);
            panel1.ResumeLayout(true);
            ResumeLayout(true);

            PerformLayout();
            panel1.PerformLayout();
            listContainer.PerformLayout();
            flowLayoutPanel3.PerformLayout();
            Refresh();
        }

        private void UpdateResponsiveLayout()
        {
            // Ensure docked controls have computed bounds before reading dimensions.
            panel1.PerformLayout();

            int headerHeight = Math.Max(flowLayoutPanel1.Height, 84);
            int sidebarWidth = Math.Max(flowLayoutPanel2.Width, 84);
            int contentLeft = sidebarWidth + 32;
            int contentTop = headerHeight + 28;
            int contentWidth = Math.Max(620, panel1.ClientSize.Width - contentLeft - 32);
            bool notificationsVisible = notificationsContainer.Visible;

            summaryPanel.Location = new Point(contentLeft, contentTop);
            summaryPanel.Size = new Size(contentWidth, 170);

            summaryTitle.Location = new Point(24, 22);
            summaryBody.Location = new Point(24, summaryTitle.Bottom + 12);

            int metricsTop = summaryBody.Bottom + 18;
            metricLabel1.Location = new Point(24, metricsTop);
            metricLabel2.Location = new Point(160, metricsTop);
            metricLabel3.Location = new Point(296, metricsTop);
            metricLabel1.Size = new Size(120, 34);
            metricLabel2.Size = new Size(120, 34);
            metricLabel3.Size = new Size(120, 34);

            // Keep bottom padding consistent even when summary text wraps.
            int summaryBottomPadding = 24;
            int summaryHeight = metricLabel1.Bottom + summaryBottomPadding;
            summaryPanel.Height = Math.Max(170, summaryHeight);

            int viewHeaderTop = notificationsVisible
                ? flowLayoutPanel1.Bottom - 4
                : summaryPanel.Bottom + 28;

            label2.Location = new Point(contentLeft, viewHeaderTop);
            sectionSubtitle.Location = new Point(contentLeft, label2.Bottom + 8);
            notificationsSubtitle.Location = new Point(contentLeft, label2.Bottom + 2);

            listContainer.Location = new Point(contentLeft, sectionSubtitle.Bottom + 16);
            listContainer.Size = new Size(contentWidth, Math.Max(260, panel1.ClientSize.Height - listContainer.Top - 28));
            int notificationsTop = notificationsSubtitle.Visible
                ? notificationsSubtitle.Bottom + 8
                : label2.Bottom + 10;

            notificationsContainer.Location = new Point(contentLeft, notificationsTop);
            notificationsContainer.Size = new Size(contentWidth, Math.Max(260, panel1.ClientSize.Height - notificationsContainer.Top - 28));

            label4.Location = new Point(20, filterPanel.Height + 20);
            label5.Location = new Point(Math.Max(180, listContainer.Width / 3), filterPanel.Height + 20);
            label6.Location = new Point(Math.Max(360, listContainer.Width / 2 + 40), filterPanel.Height + 20);

            // Teachers panel fills the list container below the filter panel (filterPanel is 50px tall)
            teachersPanel.Location = new Point(20, filterPanel.Height + 10);
            teachersPanel.Size = new Size(listContainer.Width - 40, Math.Max(300, listContainer.Height - teachersPanel.Top - 20));

            LayoutNotificationCards(notificationsContainer.Width);

            panel3.Width = 200;
            panel3.Height = 52;
            button7.Size = new Size(46, 46);
            button7.Region = new Region(new Rectangle(0, 0, button7.Width, button7.Height));
            using (GraphicsPath path = new())
            {
                path.AddEllipse(0, 0, button7.Width - 1, button7.Height - 1);
                button7.Region = new Region(path);
            }

            int headerReservedWidth =
                flowLayoutPanel1.Padding.Left +
                flowLayoutPanel1.Padding.Right +
                label1.Width +
                button8.Width +
                panel3.Width +
                button6.Width +
                104;

            textBox1.Width = Math.Max(230, flowLayoutPanel1.ClientSize.Width - headerReservedWidth);
            panel3.Margin = new Padding(16, 0, 6, 0);
            button6.Margin = new Padding(0, 10, 0, 0);
            flowLayoutPanel2.Height = Math.Max(0, panel1.ClientSize.Height - headerHeight);

            int sidebarAvailableHeight = flowLayoutPanel2.Height - flowLayoutPanel2.Padding.Top - flowLayoutPanel2.Padding.Bottom;
            int logoutTopMargin = Math.Max(40, sidebarAvailableHeight - button1.Height - button2.Height - button3.Height - 56);
            button3.Margin = new Padding(16, logoutTopMargin, 16, 0);
        }

        private void LayoutNotificationCards(int containerWidth)
        {
            int yOffset = 0;
            foreach (Panel card in notificationsContainer.Controls.OfType<Panel>())
            {
                card.Location = new Point(0, yOffset);
                card.Width = Math.Max(300, containerWidth);
                yOffset += card.Height + 16;
            }
        }

        private void ShowDashboardView()
        {
            showingNotifications = false;
            ApplyCurrentViewState();
        }

        private void ShowNotificationsView()
        {
            showingNotifications = true;
            LoadNotifications();
            ApplyCurrentViewState();
        }

        private void ApplyCurrentViewState(bool updateLayout = true)
        {
            if (applyingViewState)
            {
                return;
            }

            applyingViewState = true;

            if (flowLayoutPanel3.Parent != listContainer)
            {
                listContainer.Controls.Add(flowLayoutPanel3);
            }
            if (filterPanel.Parent != listContainer)
            {
                listContainer.Controls.Add(filterPanel);
            }
            if (teachersPanel.Parent != listContainer)
            {
                listContainer.Controls.Add(teachersPanel);
            }

            bool showDashboard = !showingNotifications;

            label2.Text = showDashboard ? "Teacher Evaluation Dashboard" : "Notifications";
            sectionSubtitle.Visible = showDashboard;
            notificationsSubtitle.Visible = false;
            summaryPanel.Visible = showDashboard;
            listContainer.Visible = showDashboard;
            flowLayoutPanel3.Visible = showDashboard;
            teachersPanel.Visible = showDashboard;
            filterPanel.Visible = showDashboard;
            notificationsContainer.Visible = !showDashboard;

            if (showDashboard)
            {
                summaryPanel.BringToFront();
                sectionSubtitle.BringToFront();
                listContainer.BringToFront();
                filterPanel.BringToFront();
                teachersPanel.BringToFront();
            }
            else
            {
                notificationsContainer.BringToFront();
            }

            if (updateLayout)
            {
                UpdateResponsiveLayout();
                panel1.PerformLayout();
                listContainer.PerformLayout();
            }

            applyingViewState = false;
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            Program.NavigateTo(new LandingPage());
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void ConfigureFilterPanel()
        {
            // Filter panel setup
            filterPanel.BackColor = Color.White;
            filterPanel.Height = 50;
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Padding = new Padding(20, 8, 20, 8);

            // Course filter label and dropdown
            var courseLabel = new Label
            {
                Text = "Course:",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(20, 15)
            };

            courseFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            courseFilter.Font = new Font("Inter", 10F);
            courseFilter.Size = new Size(140, 28);
            courseFilter.Location = new Point(80, 12);
            courseFilter.FlatStyle = FlatStyle.Flat;
            courseFilter.BackColor = Color.FromArgb(248, 250, 252);
            courseFilter.SelectedIndexChanged += (_, _) => LoadTeachers();

            // Year filter label and dropdown
            var yearLabel = new Label
            {
                Text = "Year:",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(240, 15)
            };

            yearFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            yearFilter.Font = new Font("Inter", 10F);
            yearFilter.Size = new Size(100, 28);
            yearFilter.Location = new Point(290, 12);
            yearFilter.FlatStyle = FlatStyle.Flat;
            yearFilter.BackColor = Color.FromArgb(248, 250, 252);
            yearFilter.SelectedIndexChanged += (_, _) => LoadTeachers();

            // Section filter label and dropdown
            var sectionLabel = new Label
            {
                Text = "Section:",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(410, 15)
            };

            sectionFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            sectionFilter.Font = new Font("Inter", 10F);
            sectionFilter.Size = new Size(100, 28);
            sectionFilter.Location = new Point(470, 12);
            sectionFilter.FlatStyle = FlatStyle.Flat;
            sectionFilter.BackColor = Color.FromArgb(248, 250, 252);
            sectionFilter.SelectedIndexChanged += (_, _) => LoadTeachers();

            // Clear filters button
            clearFiltersBtn.Text = "Clear Filters";
            clearFiltersBtn.Font = new Font("Inter SemiBold", 9F, FontStyle.Bold);
            clearFiltersBtn.Size = new Size(110, 28);
            clearFiltersBtn.Location = new Point(600, 12);
            clearFiltersBtn.BackColor = Color.FromArgb(241, 245, 249);
            clearFiltersBtn.ForeColor = Color.FromArgb(71, 85, 105);
            clearFiltersBtn.FlatStyle = FlatStyle.Flat;
            clearFiltersBtn.FlatAppearance.BorderSize = 0;
            clearFiltersBtn.Click += (_, _) => ClearFilters();

            filterPanel.Controls.Add(courseLabel);
            filterPanel.Controls.Add(courseFilter);
            filterPanel.Controls.Add(yearLabel);
            filterPanel.Controls.Add(yearFilter);
            filterPanel.Controls.Add(sectionLabel);
            filterPanel.Controls.Add(sectionFilter);
            filterPanel.Controls.Add(clearFiltersBtn);

            // Add filter panel to list container at the top
            listContainer.Controls.Add(filterPanel);
        }

        private void PopulateFilterDropdowns()
        {
            try
            {
                // Get unique values from teacher assignments
                var courses = TeacherStore.GetUniqueCourses();
                var years = TeacherStore.GetUniqueYearLevels();
                var sections = TeacherStore.GetUniqueSections();

                // Populate course filter
                courseFilter.Items.Clear();
                courseFilter.Items.Add("All");
                foreach (var course in courses)
                    courseFilter.Items.Add(course);
                courseFilter.SelectedIndex = 0;

                // Populate year filter
                yearFilter.Items.Clear();
                yearFilter.Items.Add("All");
                foreach (var year in years)
                    yearFilter.Items.Add(year);
                yearFilter.SelectedIndex = 0;

                // Populate section filter
                sectionFilter.Items.Clear();
                sectionFilter.Items.Add("All");
                foreach (var section in sections)
                    sectionFilter.Items.Add(section);
                sectionFilter.SelectedIndex = 0;
            }
            catch
            {
                // Silently handle errors - dropdowns will remain empty
            }
        }

        private void ClearFilters()
        {
            courseFilter.SelectedIndex = 0;
            yearFilter.SelectedIndex = 0;
            sectionFilter.SelectedIndex = 0;
            LoadTeachers();
        }
    }
}
