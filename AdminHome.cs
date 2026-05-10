using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class AdminHome : Form
    {
        private void InitializeComponent() { }

        private readonly Panel sidebar = new();
        private readonly Panel header = new();
        private readonly Panel contentPanel = new();
        private readonly FlowLayoutPanel formsListPanel = new();
        private readonly Label titleLabel = new();
        private readonly Button createFormBtn = new();
        private readonly Button logoutBtn = new();
        private readonly Button dashboardBtn = new();
        private readonly Button responsesBtn = new();
        private readonly Button teachersBtn = new();
        private readonly Label dashboardSubtitleLabel = new();
        private readonly Label responsesSubtitleLabel = new();

        private readonly Label statsLabel1 = new();
        private readonly Label statsLabel2 = new();
        private readonly Label statsLabel3 = new();
        private bool showingResponses;

        public AdminHome()
        {
            InitializeComponent();
            ConfigureAdminUi();
            LoadFormsList();
            UpdateStats();
            FormDataStore.FormsUpdated += OnDataUpdated;
            FormDataStore.ResponsesUpdated += OnDataUpdated;
            FormClosed += (_, _) =>
            {
                FormDataStore.FormsUpdated -= OnDataUpdated;
                FormDataStore.ResponsesUpdated -= OnDataUpdated;
            };
        }

        private void ConfigureAdminUi()
        {
            Text = "EvaluaTeach - Admin Dashboard";
            MinimumSize = new Size(1100, 720);
            BackColor = Color.FromArgb(245, 247, 251);
            StartPosition = FormStartPosition.CenterScreen;

            ConfigureSidebar();
            ConfigureHeader();
            ConfigureContent();
            ConfigureStatsCards();

            Controls.Add(sidebar);
            Controls.Add(header);
            Controls.Add(contentPanel);

            Resize += (_, _) => UpdateLayout();
            UpdateLayout();
        }

        private void ConfigureSidebar()
        {
            sidebar.BackColor = Color.FromArgb(24, 34, 52);
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 240;

            dashboardBtn.Text = "  Dashboard";
            dashboardBtn.BackColor = Color.FromArgb(38, 166, 91);
            dashboardBtn.ForeColor = Color.White;
            dashboardBtn.FlatStyle = FlatStyle.Flat;
            dashboardBtn.FlatAppearance.BorderSize = 0;
            dashboardBtn.Font = new Font("Inter SemiBold", 11F, FontStyle.Bold);
            dashboardBtn.Size = new Size(192, 48);
            dashboardBtn.Location = new Point(24, 24);
            dashboardBtn.TextAlign = ContentAlignment.MiddleLeft;
            dashboardBtn.Click += (_, _) => ShowDashboard();

            responsesBtn.Text = "  View Responses";
            responsesBtn.BackColor = Color.Transparent;
            responsesBtn.ForeColor = Color.FromArgb(203, 213, 225);
            responsesBtn.FlatStyle = FlatStyle.Flat;
            responsesBtn.FlatAppearance.BorderSize = 0;
            responsesBtn.Font = new Font("Inter", 11F);
            responsesBtn.Size = new Size(192, 48);
            responsesBtn.Location = new Point(24, 84);
            responsesBtn.TextAlign = ContentAlignment.MiddleLeft;
            responsesBtn.Click += (_, _) => ShowResponses();

            teachersBtn.Text = "  Manage Teachers";
            teachersBtn.BackColor = Color.Transparent;
            teachersBtn.ForeColor = Color.FromArgb(203, 213, 225);
            teachersBtn.FlatStyle = FlatStyle.Flat;
            teachersBtn.FlatAppearance.BorderSize = 0;
            teachersBtn.Font = new Font("Inter", 11F);
            teachersBtn.Size = new Size(192, 48);
            teachersBtn.Location = new Point(24, 144);
            teachersBtn.TextAlign = ContentAlignment.MiddleLeft;
            teachersBtn.Click += (_, _) => OpenTeacherManagement();

            logoutBtn.Text = "  LOGOUT";
            logoutBtn.BackColor = Color.FromArgb(220, 38, 38);
            logoutBtn.ForeColor = Color.White;
            logoutBtn.FlatStyle = FlatStyle.Flat;
            logoutBtn.FlatAppearance.BorderSize = 0;
            logoutBtn.Font = new Font("Inter Black", 12F, FontStyle.Bold);
            logoutBtn.Size = new Size(192, 56);
            logoutBtn.Location = new Point(24, 580);
            logoutBtn.TextAlign = ContentAlignment.MiddleCenter;
            logoutBtn.Click += (_, _) => Logout();

            sidebar.Controls.Add(dashboardBtn);
            sidebar.Controls.Add(responsesBtn);
            sidebar.Controls.Add(teachersBtn);
            sidebar.Controls.Add(logoutBtn);
        }

        private void ConfigureHeader()
        {
            header.BackColor = Color.White;
            header.Height = 72;
            header.Dock = DockStyle.Top;

            var logoLabel = new Label
            {
                Text = "EvaluaTeach",
                ForeColor = Color.FromArgb(38, 166, 91),
                Font = new Font("Bebas Neue", 24F),
                AutoSize = true,
                Location = new Point(16, 16)
            };

            titleLabel.Text = "Evaluation Forms Management";
            titleLabel.Font = new Font("Inter", 18F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(15, 23, 42);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(272, 22);

            createFormBtn.Text = "+ Create New Form";
            createFormBtn.BackColor = Color.FromArgb(38, 166, 91);
            createFormBtn.ForeColor = Color.White;
            createFormBtn.FlatStyle = FlatStyle.Flat;
            createFormBtn.FlatAppearance.BorderSize = 0;
            createFormBtn.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            createFormBtn.Size = new Size(160, 44);
            createFormBtn.Location = new Point(700, 14);
            createFormBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            createFormBtn.Click += (_, _) => OpenFormBuilder();
            createFormBtn.BringToFront();

            header.Controls.Add(logoLabel);
            header.Controls.Add(titleLabel);
            header.Controls.Add(createFormBtn);
            header.Width = header.Parent?.ClientSize.Width ?? 800;
        }

        private void ConfigureContent()
        {
            contentPanel.BackColor = Color.FromArgb(245, 247, 251);
            contentPanel.Location = new Point(240, 72);
            contentPanel.Size = new Size(ClientSize.Width - 240, ClientSize.Height - 72);
            contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            contentPanel.AutoScroll = true;

            dashboardSubtitleLabel.Text = "Manage your evaluation forms. Students will see active forms immediately.";
            dashboardSubtitleLabel.Font = new Font("Inter", 10F);
            dashboardSubtitleLabel.ForeColor = Color.FromArgb(100, 116, 139);
            dashboardSubtitleLabel.AutoSize = true;
            dashboardSubtitleLabel.Location = new Point(32, 20);

            responsesSubtitleLabel.Text = "View complete student responses and generate teacher-ready reports.";
            responsesSubtitleLabel.Font = new Font("Inter", 10F);
            responsesSubtitleLabel.ForeColor = Color.FromArgb(100, 116, 139);
            responsesSubtitleLabel.AutoSize = true;
            responsesSubtitleLabel.Location = new Point(32, 20);
            responsesSubtitleLabel.Visible = false;

            formsListPanel.FlowDirection = FlowDirection.TopDown;
            formsListPanel.WrapContents = false;
            formsListPanel.AutoScroll = true;
            formsListPanel.BackColor = Color.Transparent;
            formsListPanel.Location = new Point(32, 190);
            formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 206);
            formsListPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            contentPanel.Controls.Add(dashboardSubtitleLabel);
            contentPanel.Controls.Add(responsesSubtitleLabel);
            contentPanel.Controls.Add(formsListPanel);
        }

        private void ConfigureStatsCards()
        {
            int cardWidth = 200;
            int cardHeight = 118;
            int startX = 32;
            int spacing = 24;

            statsLabel1.BackColor = Color.White;
            statsLabel1.Size = new Size(cardWidth, cardHeight);
            statsLabel1.Location = new Point(startX, 52);
            statsLabel1.Padding = new Padding(16);
            statsLabel1.Font = new Font("Inter", 11F);
            statsLabel1.ForeColor = Color.FromArgb(71, 85, 105);
            statsLabel1.TextAlign = ContentAlignment.TopLeft;

            statsLabel2.BackColor = Color.White;
            statsLabel2.Size = new Size(cardWidth, cardHeight);
            statsLabel2.Location = new Point(startX + cardWidth + spacing, 52);
            statsLabel2.Padding = new Padding(16);
            statsLabel2.Font = new Font("Inter", 11F);
            statsLabel2.ForeColor = Color.FromArgb(71, 85, 105);
            statsLabel2.TextAlign = ContentAlignment.TopLeft;

            statsLabel3.BackColor = Color.White;
            statsLabel3.Size = new Size(cardWidth, cardHeight);
            statsLabel3.Location = new Point(startX + (cardWidth + spacing) * 2, 52);
            statsLabel3.Padding = new Padding(16);
            statsLabel3.Font = new Font("Inter", 11F);
            statsLabel3.ForeColor = Color.FromArgb(71, 85, 105);
            statsLabel3.TextAlign = ContentAlignment.TopLeft;

            contentPanel.Controls.Add(statsLabel1);
            contentPanel.Controls.Add(statsLabel2);
            contentPanel.Controls.Add(statsLabel3);
        }

        private void UpdateStats()
        {
            var forms = FormDataStore.GetAllForms();
            var activeCount = forms.Count(f => f.IsActive);
            var totalResponses = forms.Sum(f => FormDataStore.GetSubmissionCount(f.Id));

            statsLabel1.Text = $"Total Forms\n\n{forms.Count}";
            statsLabel2.Text = $"Active Forms\n\n{activeCount}";
            statsLabel3.Text = $"Total Responses\n\n{totalResponses}";
        }

        private void LoadFormsList()
        {
            formsListPanel.Controls.Clear();
            var forms = FormDataStore.GetAllForms().OrderByDescending(f => f.CreatedAt);

            if (!forms.Any())
            {
                var emptyLabel = new Label
                {
                    Text = "No forms yet. Click 'Create New Form' to get started.",
                    Font = new Font("Inter", 12F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(0, 20)
                };
                formsListPanel.Controls.Add(emptyLabel);
                return;
            }

            foreach (var form in forms)
            {
                var card = CreateFormCard(form);
                formsListPanel.Controls.Add(card);
            }
        }

        private Panel CreateFormCard(EvaluationForm form)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(formsListPanel.Width - 40, 140),
                Margin = new Padding(0, 0, 0, 16),
                Padding = new Padding(20)
            };

            var statusColor = form.IsActive ? Color.FromArgb(38, 166, 91) : Color.FromArgb(148, 163, 184);
            var statusText = form.IsActive ? "Active" : "Inactive";

            var title = new Label
            {
                Text = form.Title,
                Font = new Font("Inter", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var description = new Label
            {
                Text = string.IsNullOrEmpty(form.Description) ? "No description" : form.Description,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(20, 48),
                MaximumSize = new Size(card.Width - 240, 0)
            };

            var meta = new Label
            {
                Text = $"{form.Questions.Count} questions | Target: {(!string.IsNullOrWhiteSpace(form.TargetCourse) ? form.TargetCourse : "All")} | Created: {form.CreatedAt:MMM dd, yyyy}",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(20, 80)
            };

            var statusBadge = new Label
            {
                Text = statusText,
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = statusColor,
                AutoSize = true,
                Padding = new Padding(8, 4, 8, 4),
                Location = new Point(card.Width - 100, 20)
            };

            var responsesCount = FormDataStore.GetSubmissionCount(form.Id);
            var responsesLabel = new Label
            {
                Text = $"{responsesCount} responses",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(card.Width - 120, 50)
            };

            var editBtn = new Button
            {
                Text = "Edit",
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(70, 32),
                Location = new Point(card.Width - 188, 90)
            };
            editBtn.Click += (_, _) => EditForm(form);

            var toggleBtn = new Button
            {
                Text = form.IsActive ? "Deactivate" : "Activate",
                BackColor = form.IsActive ? Color.FromArgb(254, 226, 226) : Color.FromArgb(220, 252, 231),
                ForeColor = form.IsActive ? Color.FromArgb(185, 28, 28) : Color.FromArgb(22, 101, 52),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(102, 32),
                Location = new Point(card.Width - 110, 90)
            };
            toggleBtn.Click += (_, _) => ToggleFormStatus(form);

            card.Controls.Add(title);
            card.Controls.Add(description);
            card.Controls.Add(meta);
            card.Controls.Add(statusBadge);
            card.Controls.Add(responsesLabel);
            card.Controls.Add(editBtn);
            card.Controls.Add(toggleBtn);

            card.Resize += (_, _) =>
            {
                statusBadge.Location = new Point(card.Width - 100, 20);
                responsesLabel.Location = new Point(card.Width - 120, 50);
                editBtn.Location = new Point(card.Width - 188, 90);
                toggleBtn.Location = new Point(card.Width - 110, 90);
                description.MaximumSize = new Size(card.Width - 240, 0);
            };

            return card;
        }

        private void UpdateLayout()
        {
            logoutBtn.Location = new Point(24, sidebar.ClientSize.Height - logoutBtn.Height - 32);

            createFormBtn.Location = new Point(header.ClientSize.Width - createFormBtn.Width - 24, 14);

            if (!showingResponses)
            {
                formsListPanel.Location = new Point(32, 190);
                formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 206);
            }
            else
            {
                formsListPanel.Location = new Point(32, 56);
                formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 72);
            }
        }

        private void OnDataUpdated()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(OnDataUpdated));
                return;
            }

            if (showingResponses)
            {
                LoadResponsesView();
            }
            else
            {
                LoadFormsList();
            }
            UpdateStats();
        }

        private void OpenFormBuilder()
        {
            var builder = new FormBuilder();
            builder.FormSaved += () =>
            {
                LoadFormsList();
                UpdateStats();
            };
            builder.ShowDialog(this);
        }

        private void OpenTeacherManagement()
        {
            var teacherForm = new TeacherManagement();
            teacherForm.ShowDialog(this);
        }

        private void EditForm(EvaluationForm form)
        {
            var builder = new FormBuilder(form);
            builder.FormSaved += () =>
            {
                LoadFormsList();
                UpdateStats();
            };
            builder.ShowDialog(this);
        }

        private void ToggleFormStatus(EvaluationForm form)
        {
            form.IsActive = !form.IsActive;
            FormDataStore.UpdateForm(form);
        }

        private void ShowDashboard()
        {
            showingResponses = false;
            dashboardBtn.BackColor = Color.FromArgb(38, 166, 91);
            dashboardBtn.ForeColor = Color.White;
            responsesBtn.BackColor = Color.Transparent;
            responsesBtn.ForeColor = Color.FromArgb(203, 213, 225);
            titleLabel.Text = "Evaluation Forms Management";
            createFormBtn.Visible = true;
            dashboardSubtitleLabel.Visible = true;
            responsesSubtitleLabel.Visible = false;
            statsLabel1.Visible = true;
            statsLabel2.Visible = true;
            statsLabel3.Visible = true;
            formsListPanel.Location = new Point(32, 190);
            formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 206);
            LoadFormsList();
        }

        private void ShowResponses()
        {
            showingResponses = true;
            dashboardBtn.BackColor = Color.Transparent;
            dashboardBtn.ForeColor = Color.FromArgb(203, 213, 225);
            responsesBtn.BackColor = Color.FromArgb(38, 166, 91);
            responsesBtn.ForeColor = Color.White;
            titleLabel.Text = "Student Responses";
            createFormBtn.Visible = false;
            dashboardSubtitleLabel.Visible = false;
            responsesSubtitleLabel.Visible = true;
            statsLabel1.Visible = false;
            statsLabel2.Visible = false;
            statsLabel3.Visible = false;
            formsListPanel.Location = new Point(32, 56);
            formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 72);
            LoadResponsesView();
        }

        private void LoadResponsesView()
        {
            formsListPanel.Controls.Clear();
            var forms = FormDataStore.GetAllForms().OrderByDescending(f => f.CreatedAt).ToList();

            foreach (var form in forms)
            {
                var responses = FormDataStore.GetResponsesForForm(form.Id)
                    .OrderByDescending(r => r.SubmittedAt)
                    .ToList();
                if (!responses.Any()) continue;

                var section = new Panel
                {
                    BackColor = Color.White,
                    Size = new Size(formsListPanel.Width - 40, 74 + (responses.Count * 170)),
                    Margin = new Padding(0, 0, 0, 16),
                    Padding = new Padding(20),
                    AutoScroll = true
                };

                var title = new Label
                {
                    Text = $"{form.Title} ({responses.Count} responses)",
                    Font = new Font("Inter", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };

                section.Controls.Add(title);

                int y = 56;
                foreach (var response in responses)
                {
                    var responseCard = CreateResponseCard(form, response, section.Width - 40, y);
                    section.Controls.Add(responseCard);
                    y = responseCard.Bottom + 10;
                }

                section.Resize += (_, _) =>
                {
                    int top = 56;
                    foreach (Control control in section.Controls)
                    {
                        if (control is Panel card && card.Tag as string == "response-card")
                        {
                            card.Width = section.Width - 40;
                            card.Location = new Point(20, top);
                            top = card.Bottom + 10;
                        }
                    }
                };

                formsListPanel.Controls.Add(section);
            }

            if (formsListPanel.Controls.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "No responses submitted yet.",
                    Font = new Font("Inter", 12F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(0, 20)
                };
                formsListPanel.Controls.Add(emptyLabel);
            }
        }

        private Panel CreateResponseCard(EvaluationForm form, FormResponse response, int width, int top)
        {
            var card = new Panel
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Location = new Point(20, top),
                Size = new Size(width, 160),
                Padding = new Padding(12),
                Tag = "response-card"
            };

            string teacherDisplay = response.TeacherId > 0 ? response.TeacherName : (string.IsNullOrWhiteSpace(form.TargetTeacher) ? "Assigned Teacher" : form.TargetTeacher);
            var headerLabel = new Label
            {
                Text = $"{response.StudentName} ({response.StudentId}) - Evaluated: {teacherDisplay} - {response.SubmittedAt:g}",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true,
                Location = new Point(12, 10)
            };

            var answersBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Font = new Font("Inter", 9F, FontStyle.Regular),
                Location = new Point(12, 36),
                Size = new Size(card.Width - 148, 112),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ScrollBars = ScrollBars.Vertical,
                Text = BuildResponseReport(form, response)
            };

            var sendReportBtn = new Button
            {
                Text = "Send Report",
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(110, 34),
                Location = new Point(card.Width - 122, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            sendReportBtn.Click += (_, _) => SendReportToTeacher(form, response, answersBox.Text);

            card.Controls.Add(headerLabel);
            card.Controls.Add(answersBox);
            card.Controls.Add(sendReportBtn);
            return card;
        }

        private static string BuildResponseReport(EvaluationForm form, FormResponse response)
        {
            string teacherDisplay = response.TeacherId > 0 ? response.TeacherName : (string.IsNullOrWhiteSpace(form.TargetTeacher) ? "Assigned Teacher" : form.TargetTeacher);
            var lines = new List<string>
            {
                $"Teacher: {teacherDisplay}",
                $"Department: {(!string.IsNullOrWhiteSpace(form.TargetCourse) ? form.TargetCourse : "All")}",
                "------------------------------"
            };

            foreach (var question in form.Questions.OrderBy(q => q.OrderIndex))
            {
                response.Answers.TryGetValue(question.Id, out var answer);
                lines.Add($"{question.OrderIndex + 1}. {question.Text}");
                lines.Add($"   Answer: {(!string.IsNullOrWhiteSpace(answer) ? answer : "(no answer)")}");
            }

            return string.Join(Environment.NewLine, lines);
        }

        private void SendReportToTeacher(EvaluationForm form, FormResponse response, string reportBody)
        {
            string teacher = response.TeacherId > 0 ? response.TeacherName : (string.IsNullOrWhiteSpace(form.TargetTeacher) ? "Assigned Teacher" : form.TargetTeacher);
            int reportLength = reportBody.Length;
            MessageBox.Show(
                $"Report prepared for {teacher}.\n\nStudent: {response.StudentName}\nForm: {form.Title}\nReport size: {reportLength} characters\n\nIn this build, reports are generated and ready to send.",
                "Report Ready",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void Logout()
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SessionStore.Logout();
                Program.NavigateTo(new LandingPage());
            }
        }
    }
}
