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
        private readonly FlowLayoutPanel teachersListPanel = new();
        private readonly TextBox firstNameInput = new();
        private readonly TextBox lastNameInput = new();
        private readonly TextBox emailInput = new();
        private readonly TextBox passwordInput = new();
        private readonly TextBox subjectsInput = new();
        private readonly ComboBox departmentSelector = new();
        private bool showingResponses;
        private bool showingTeachers;

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

            // Teachers panel (hidden by default)
            teachersListPanel.FlowDirection = FlowDirection.TopDown;
            teachersListPanel.WrapContents = false;
            teachersListPanel.AutoScroll = true;
            teachersListPanel.BackColor = Color.Transparent;
            teachersListPanel.Location = new Point(32, 90);
            teachersListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 110);
            teachersListPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            teachersListPanel.Visible = false;

            contentPanel.Controls.Add(dashboardSubtitleLabel);
            contentPanel.Controls.Add(responsesSubtitleLabel);
            contentPanel.Controls.Add(formsListPanel);
            contentPanel.Controls.Add(teachersListPanel);
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
            ShowTeachers();
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
            showingTeachers = false;
            UpdateNavButtons();
            titleLabel.Text = "Evaluation Forms Management";
            createFormBtn.Visible = true;
            dashboardSubtitleLabel.Visible = true;
            responsesSubtitleLabel.Visible = false;
            statsLabel1.Visible = true;
            statsLabel2.Visible = true;
            statsLabel3.Visible = true;
            formsListPanel.Visible = true;
            formsListPanel.Location = new Point(32, 190);
            formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 206);
            teachersListPanel.Visible = false;
            LoadFormsList();
        }

        private void ShowResponses()
        {
            showingResponses = true;
            showingTeachers = false;
            UpdateNavButtons();
            titleLabel.Text = "Student Responses";
            createFormBtn.Visible = false;
            dashboardSubtitleLabel.Visible = false;
            responsesSubtitleLabel.Visible = true;
            statsLabel1.Visible = false;
            statsLabel2.Visible = false;
            statsLabel3.Visible = false;
            formsListPanel.Visible = true;
            formsListPanel.Location = new Point(32, 56);
            formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 72);
            teachersListPanel.Visible = false;
            LoadResponsesView();
        }

        private void ShowTeachers()
        {
            showingResponses = false;
            showingTeachers = true;
            UpdateNavButtons();
            titleLabel.Text = "Teacher Management";
            createFormBtn.Visible = false;
            dashboardSubtitleLabel.Visible = false;
            responsesSubtitleLabel.Visible = false;
            statsLabel1.Visible = false;
            statsLabel2.Visible = false;
            statsLabel3.Visible = false;
            formsListPanel.Visible = false;
            teachersListPanel.Visible = true;
            LoadTeachersView();
        }

        private void UpdateNavButtons()
        {
            dashboardBtn.BackColor = showingResponses || showingTeachers ? Color.Transparent : Color.FromArgb(38, 166, 91);
            dashboardBtn.ForeColor = showingResponses || showingTeachers ? Color.FromArgb(203, 213, 225) : Color.White;
            responsesBtn.BackColor = showingResponses ? Color.FromArgb(38, 166, 91) : Color.Transparent;
            responsesBtn.ForeColor = showingResponses ? Color.White : Color.FromArgb(203, 213, 225);
            teachersBtn.BackColor = showingTeachers ? Color.FromArgb(38, 166, 91) : Color.Transparent;
            teachersBtn.ForeColor = showingTeachers ? Color.White : Color.FromArgb(203, 213, 225);
        }

        private void LoadResponsesView()
        {
            formsListPanel.Controls.Clear();
            var forms = FormDataStore.GetAllForms().OrderByDescending(f => f.CreatedAt).ToList();

            // Header stats panel
            var totalResponses = forms.Sum(f => FormDataStore.GetSubmissionCount(f.Id));
            var formsWithResponses = forms.Count(f => FormDataStore.GetSubmissionCount(f.Id) > 0);

            var statsPanel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(formsListPanel.Width - 40, 80),
                Margin = new Padding(0, 0, 0, 16),
                Padding = new Padding(20)
            };

            var statsTitle = new Label
            {
                Text = "Response Overview",
                Font = new Font("Inter", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, 12)
            };

            var statsSubtitle = new Label
            {
                Text = $"{formsWithResponses} forms with responses  •  {totalResponses} total submissions",
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(20, 46)
            };

            statsPanel.Controls.Add(statsTitle);
            statsPanel.Controls.Add(statsSubtitle);
            formsListPanel.Controls.Add(statsPanel);

            foreach (var form in forms)
            {
                var responses = FormDataStore.GetResponsesForForm(form.Id)
                    .OrderByDescending(r => r.SubmittedAt)
                    .ToList();
                if (!responses.Any()) continue;

                var section = new Panel
                {
                    BackColor = Color.White,
                    Size = new Size(formsListPanel.Width - 40, 90 + (responses.Count * 200)),
                    Margin = new Padding(0, 0, 0, 20),
                    Padding = new Padding(24, 20, 24, 20)
                };

                // Form title with icon indicator
                var titleContainer = new Panel
                {
                    BackColor = Color.Transparent,
                    Size = new Size(section.Width - 48, 36),
                    Location = new Point(24, 20)
                };

                var titleIndicator = new Panel
                {
                    BackColor = Color.FromArgb(38, 166, 91),
                    Size = new Size(4, 24),
                    Location = new Point(0, 6),
                    BorderStyle = BorderStyle.None
                };

                var title = new Label
                {
                    Text = form.Title,
                    Font = new Font("Inter", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    AutoSize = true,
                    Location = new Point(16, 8)
                };

                var responseCountBadge = new Panel
                {
                    BackColor = Color.FromArgb(220, 252, 231),
                    Size = new Size(100, 28),
                    Location = new Point(titleContainer.Width - 100, 4)
                };

                var responseCountLabel = new Label
                {
                    Text = $"{responses.Count} responses",
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(38, 166, 91),
                    AutoSize = false,
                    Size = new Size(100, 28),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(0, 0)
                };

                responseCountBadge.Controls.Add(responseCountLabel);
                titleContainer.Controls.Add(titleIndicator);
                titleContainer.Controls.Add(title);
                titleContainer.Controls.Add(responseCountBadge);
                section.Controls.Add(titleContainer);

                // Course info
                var courseLabel = new Label
                {
                    Text = $"Target: {form.TargetCourse ?? "All Courses"}",
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(40, 56)
                };
                section.Controls.Add(courseLabel);

                int y = 84;
                foreach (var response in responses)
                {
                    var responseCard = CreateEnhancedResponseCard(form, response, section.Width - 48, y);
                    section.Controls.Add(responseCard);
                    y = responseCard.Bottom + 16;
                }

                section.Resize += (_, _) =>
                {
                    titleContainer.Width = section.Width - 48;
                    responseCountBadge.Location = new Point(titleContainer.Width - 100, 4);

                    int top = 84;
                    foreach (Control control in section.Controls)
                    {
                        if (control is Panel card && card.Tag as string == "response-card")
                        {
                            card.Width = section.Width - 48;
                            card.Location = new Point(24, top);
                            top = card.Bottom + 16;
                        }
                    }
                };

                formsListPanel.Controls.Add(section);
            }

            if (formsListPanel.Controls.Count <= 1)
            {
                var emptyPanel = new Panel
                {
                    BackColor = Color.White,
                    Size = new Size(formsListPanel.Width - 40, 200),
                    Margin = new Padding(0, 20, 0, 0)
                };

                var emptyIcon = new Label
                {
                    Text = "📋",
                    Font = new Font("Segoe UI", 48F),
                    AutoSize = true,
                    Location = new Point((emptyPanel.Width - 60) / 2, 30)
                };

                var emptyLabel = new Label
                {
                    Text = "No responses submitted yet",
                    Font = new Font("Inter", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = true,
                    Location = new Point((emptyPanel.Width - 220) / 2, 100)
                };

                var emptySubLabel = new Label
                {
                    Text = "Student evaluations will appear here once submitted",
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point((emptyPanel.Width - 280) / 2, 130)
                };

                emptyPanel.Controls.Add(emptyIcon);
                emptyPanel.Controls.Add(emptyLabel);
                emptyPanel.Controls.Add(emptySubLabel);
                formsListPanel.Controls.Add(emptyPanel);
            }
        }

        private Panel CreateEnhancedResponseCard(EvaluationForm form, FormResponse response, int width, int top)
        {
            string teacherDisplay = response.TeacherId > 0 ? response.TeacherName : (string.IsNullOrWhiteSpace(form.TargetTeacher) ? "Assigned Teacher" : form.TargetTeacher);

            var card = new Panel
            {
                BackColor = Color.FromArgb(250, 251, 252),
                Location = new Point(24, top),
                Size = new Size(width, 180),
                Padding = new Padding(0),
                Tag = "response-card",
                BorderStyle = BorderStyle.None
            };

            // Top bar with accent color
            var accentBar = new Panel
            {
                BackColor = Color.FromArgb(38, 166, 91),
                Size = new Size(4, card.Height - 2),
                Location = new Point(0, 1)
            };

            // Header section with student info
            var headerPanel = new Panel
            {
                BackColor = Color.Transparent,
                Size = new Size(card.Width - 24, 44),
                Location = new Point(16, 12)
            };

            var studentAvatar = new Panel
            {
                BackColor = Color.FromArgb(38, 166, 91),
                Size = new Size(32, 32),
                Location = new Point(0, 6)
            };

            // Student initials
            var initials = response.StudentName?.Split(' ')
                .Where(s => !string.IsNullOrEmpty(s))
                .Take(2)
                .Select(s => s[0])
                .ToArray() ?? new[] { 'S' };
            var initialsLabel = new Label
            {
                Text = new string(initials).ToUpper(),
                Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(32, 32),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0)
            };
            studentAvatar.Controls.Add(initialsLabel);

            var studentNameLabel = new Label
            {
                Text = response.StudentName,
                Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(44, 4)
            };

            var studentIdLabel = new Label
            {
                Text = response.StudentId,
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(44, 24)
            };

            headerPanel.Controls.Add(studentAvatar);
            headerPanel.Controls.Add(studentNameLabel);
            headerPanel.Controls.Add(studentIdLabel);

            // Submitted time badge
            var timeBadge = new Panel
            {
                BackColor = Color.FromArgb(241, 245, 249),
                Size = new Size(155, 28),
                Location = new Point(headerPanel.Width - 155, 8),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var timeIcon = new Label
            {
                Text = "🕐",
                Font = new Font("Segoe UI", 9F),
                AutoSize = true,
                Location = new Point(8, 4)
            };

            var timeLabel = new Label
            {
                Text = response.SubmittedAt.ToString("MMM dd, HH:mm"),
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(36, 6)
            };

            timeBadge.Controls.Add(timeIcon);
            timeBadge.Controls.Add(timeLabel);
            headerPanel.Controls.Add(timeBadge);

            // Teacher being evaluated
            var teacherPanel = new Panel
            {
                BackColor = Color.FromArgb(240, 253, 244),
                Size = new Size(card.Width - 180, 32),
                Location = new Point(16, 60)
            };

            var teacherIcon = new Label
            {
                Text = "👨‍🏫",
                Font = new Font("Segoe UI", 12F),
                AutoSize = true,
                Location = new Point(10, 4)
            };

            var teacherLabel = new Label
            {
                Text = $"Evaluating: {teacherDisplay}",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(22, 101, 52),
                AutoSize = true,
                Location = new Point(36, 8)
            };

            teacherPanel.Controls.Add(teacherIcon);
            teacherPanel.Controls.Add(teacherLabel);

            // Answers summary - scrollable panel instead of textbox
            var answersPanel = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(card.Width - 160, 72),
                Location = new Point(16, 100),
                AutoScroll = true,
                Padding = new Padding(12)
            };

            BuildAnswersSummary(form, response, answersPanel);

            // Action buttons
            var sendReportBtn = new Button
            {
                Text = "📤 Send",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(90, 32),
                Location = new Point(card.Width - 106, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            sendReportBtn.Click += (_, _) => SendReportToTeacher(form, response, BuildResponseReport(form, response));
            sendReportBtn.MouseEnter += (_, _) => { sendReportBtn.BackColor = Color.FromArgb(22, 163, 74); };
            sendReportBtn.MouseLeave += (_, _) => { sendReportBtn.BackColor = Color.FromArgb(38, 166, 91); };

            var viewDetailsBtn = new Button
            {
                Text = "� Details",
                BackColor = Color.FromArgb(240, 253, 244),
                ForeColor = Color.FromArgb(22, 101, 52),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(90, 32),
                Location = new Point(card.Width - 106, 138),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            viewDetailsBtn.Click += (_, _) => ShowResponseDetails(form, response);
            viewDetailsBtn.MouseEnter += (_, _) => { viewDetailsBtn.BackColor = Color.FromArgb(220, 252, 231); };
            viewDetailsBtn.MouseLeave += (_, _) => { viewDetailsBtn.BackColor = Color.FromArgb(240, 253, 244); };

            card.Controls.Add(accentBar);
            card.Controls.Add(headerPanel);
            card.Controls.Add(teacherPanel);
            card.Controls.Add(answersPanel);
            card.Controls.Add(sendReportBtn);
            card.Controls.Add(viewDetailsBtn);

            return card;
        }

        private void BuildAnswersSummary(EvaluationForm form, FormResponse response, Panel container)
        {
            int y = 8;
            int questionNum = 1;

            foreach (var question in form.Questions.OrderBy(q => q.OrderIndex).Take(4))
            {
                response.Answers.TryGetValue(question.Id, out var answer);
                var displayAnswer = FormatAnswerDisplay(question, answer);

                var qLabel = new Label
                {
                    Text = $"Q{questionNum}: {displayAnswer}",
                    Font = new Font("Inter", 8F),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = false,
                    Size = new Size(container.Width - 24, 18),
                    Location = new Point(8, y),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                container.Controls.Add(qLabel);
                y += 20;
                questionNum++;
            }

            if (form.Questions.Count > 4)
            {
                var moreLabel = new Label
                {
                    Text = $"+{form.Questions.Count - 4} more questions...",
                    Font = new Font("Inter", 8F, FontStyle.Italic),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(8, y)
                };
                container.Controls.Add(moreLabel);
            }
        }

        private string FormatAnswerDisplay(FormQuestion question, string? answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return "—";

            if (question.Type == QuestionType.Rating && int.TryParse(answer, out var rating))
            {
                var stars = new string('★', rating) + new string('☆', (question.MaxRating ?? 5) - rating);
                return $"{stars} ({rating}/{question.MaxRating ?? 5})";
            }

            if (question.Type == QuestionType.YesNo)
            {
                var color = answer.ToLower() == "yes" ? "✓" : "✗";
                return $"{color} {answer}";
            }

            if (answer.Length > 30)
                return answer.Substring(0, 30) + "...";

            return answer;
        }

        private void ShowResponseDetails(EvaluationForm form, FormResponse response)
        {
            var detailsForm = new Form
            {
                Text = $"Response Details - {response.StudentName}",
                Size = new Size(700, 600),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.Sizable,
                MaximizeBox = true,
                MinimizeBox = true
            };

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(32)
            };

            int y = 0;

            // Header
            var headerPanel = new Panel
            {
                Size = new Size(600, 80),
                Location = new Point(32, y),
                BackColor = Color.FromArgb(240, 253, 244),
                Padding = new Padding(20)
            };

            var headerTitle = new Label
            {
                Text = form.Title,
                Font = new Font("Inter", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, 16)
            };

            var headerSubtitle = new Label
            {
                Text = $"Submitted by {response.StudentName} ({response.StudentId}) on {response.SubmittedAt:MMMM dd, yyyy at h:mm tt}",
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(20, 48)
            };

            headerPanel.Controls.Add(headerTitle);
            headerPanel.Controls.Add(headerSubtitle);
            scrollPanel.Controls.Add(headerPanel);
            y += 100;

            // Questions and answers
            foreach (var question in form.Questions.OrderBy(q => q.OrderIndex))
            {
                response.Answers.TryGetValue(question.Id, out var answer);

                var qPanel = new Panel
                {
                    Size = new Size(600, 100),
                    Location = new Point(32, y),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var qNumLabel = new Label
                {
                    Text = $"Q{question.OrderIndex + 1}",
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(38, 166, 91),
                    AutoSize = true,
                    Location = new Point(16, 16)
                };

                var qTextLabel = new Label
                {
                    Text = question.Text,
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    AutoSize = false,
                    Size = new Size(560, 20),
                    Location = new Point(48, 16)
                };

                var typeBadge = new Label
                {
                    Text = question.Type.ToString(),
                    Font = new Font("Inter", 8F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    BackColor = Color.FromArgb(241, 245, 249),
                    AutoSize = true,
                    Padding = new Padding(6, 2, 6, 2),
                    Location = new Point(48, 42)
                };

                var answerPanel = new Panel
                {
                    Size = new Size(560, 40),
                    Location = new Point(48, 50),
                    BackColor = Color.FromArgb(250, 251, 252)
                };

                var answerLabel = new Label
                {
                    Text = FormatFullAnswer(question, answer),
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    AutoSize = false,
                    Size = new Size(540, 36),
                    Location = new Point(10, 8)
                };

                answerPanel.Controls.Add(answerLabel);

                qPanel.Controls.Add(qNumLabel);
                qPanel.Controls.Add(qTextLabel);
                qPanel.Controls.Add(typeBadge);
                qPanel.Controls.Add(answerPanel);

                scrollPanel.Controls.Add(qPanel);
                y += 115;
            }

            detailsForm.Controls.Add(scrollPanel);
            detailsForm.ShowDialog(this);
        }

        private string FormatFullAnswer(FormQuestion question, string? answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return "No answer provided";

            if (question.Type == QuestionType.Rating && int.TryParse(answer, out var rating))
            {
                var stars = new string('★', rating) + new string('☆', (question.MaxRating ?? 5) - rating);
                return $"{stars} ({rating} out of {question.MaxRating ?? 5})";
            }

            if (question.Type == QuestionType.YesNo)
            {
                return answer.ToLower() == "yes" ? "✓ Yes" : "✗ No";
            }

            return answer;
        }

        private Panel CreateResponseCard(EvaluationForm form, FormResponse response, int width, int top)
        {
            return CreateEnhancedResponseCard(form, response, width, top);
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

        private void LoadTeachersView()
        {
            teachersListPanel.Controls.Clear();
            var teachers = TeacherStore.GetAllTeachers();

            // Header section
            var headerPanel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(teachersListPanel.Width - 40, 140),
                Margin = new Padding(0, 0, 0, 16),
                Padding = new Padding(24)
            };

            var titleLabel = new Label
            {
                Text = "All Teachers",
                Font = new Font("Inter", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            var statsText = $"{teachers.Count} Total Teachers";
            if (teachers.Count > 0)
            {
                var departments = teachers.Select(t => t.Department).Distinct().Count();
                var subjects = teachers.SelectMany(t => t.Subjects).Distinct().Count();
                statsText += $"  ·  {departments} Departments  ·  {subjects} Subjects";
            }

            var statsLabel = new Label
            {
                Text = statsText,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(24, 50)
            };

            // Add Teacher button in header
            var addBtn = new Button
            {
                Text = "+ Add New Teacher",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(160, 40),
                Location = new Point(headerPanel.Width - 184, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            addBtn.Click += (_, _) => ShowAddTeacherForm();

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(statsLabel);
            headerPanel.Controls.Add(addBtn);
            headerPanel.Resize += (_, _) => addBtn.Location = new Point(headerPanel.Width - 184, 50);

            teachersListPanel.Controls.Add(headerPanel);

            if (teachers.Count == 0)
            {
                var emptyPanel = new Panel
                {
                    BackColor = Color.White,
                    Size = new Size(teachersListPanel.Width - 40, 200),
                    Margin = new Padding(0, 16, 0, 0)
                };

                var emptyIcon = new Label
                {
                    Text = "👨‍🏫",
                    Font = new Font("Segoe UI", 48F),
                    AutoSize = true,
                    Location = new Point((emptyPanel.Width - 60) / 2, 40)
                };

                var emptyLabel = new Label
                {
                    Text = "No teachers added yet",
                    Font = new Font("Inter", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = true,
                    Location = new Point((emptyPanel.Width - 200) / 2, 110)
                };

                var emptySubLabel = new Label
                {
                    Text = "Click '+ Add New Teacher' to add your first faculty member",
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point((emptyPanel.Width - 350) / 2, 140)
                };

                emptyPanel.Controls.Add(emptyIcon);
                emptyPanel.Controls.Add(emptyLabel);
                emptyPanel.Controls.Add(emptySubLabel);
                teachersListPanel.Controls.Add(emptyPanel);
                return;
            }

            foreach (var teacher in teachers)
            {
                var card = CreateTeacherCard(teacher);
                teachersListPanel.Controls.Add(card);
            }
        }

        private Panel CreateTeacherCard(Teacher teacher)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(teachersListPanel.Width - 40, 110),
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(20)
            };

            // Green accent bar
            var accentBar = new Panel
            {
                BackColor = Color.FromArgb(38, 166, 91),
                Size = new Size(4, card.Height),
                Location = new Point(0, 0)
            };

            // Avatar with initials
            var avatar = new Panel
            {
                BackColor = Color.FromArgb(38, 166, 91),
                Size = new Size(50, 50),
                Location = new Point(20, 20)
            };

            var initials = GetInitials(teacher.FullName);
            var initialsLabel = new Label
            {
                Text = initials,
                Font = new Font("Inter SemiBold", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(50, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0)
            };
            avatar.Controls.Add(initialsLabel);

            // Name
            var nameLabel = new Label
            {
                Text = teacher.FullName,
                Font = new Font("Inter SemiBold", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(86, 20)
            };

            // Email
            var emailLabel = new Label
            {
                Text = teacher.Email,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(86, 44)
            };

            // Department badge
            var deptBadge = new Label
            {
                Text = teacher.Department,
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 101, 52),
                BackColor = Color.FromArgb(220, 252, 231),
                AutoSize = true,
                Padding = new Padding(8, 4, 8, 4),
                Location = new Point(86, 70)
            };

            // Subjects
            var subjectsLabel = new Label
            {
                Text = $"📚 {teacher.SubjectsDisplay}",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(200, 74)
            };

            // Delete button
            var deleteBtn = new Button
            {
                Text = "🗑 Delete",
                BackColor = Color.FromArgb(254, 226, 226),
                ForeColor = Color.FromArgb(185, 28, 28),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(110, 32),
                Location = new Point(card.Width - 130, 39),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            deleteBtn.Click += (_, _) => DeleteTeacher(teacher.TeacherID, teacher.FullName);
            deleteBtn.MouseEnter += (_, _) => deleteBtn.BackColor = Color.FromArgb(252, 210, 210);
            deleteBtn.MouseLeave += (_, _) => deleteBtn.BackColor = Color.FromArgb(254, 226, 226);

            card.Controls.Add(accentBar);
            card.Controls.Add(avatar);
            card.Controls.Add(nameLabel);
            card.Controls.Add(emailLabel);
            card.Controls.Add(deptBadge);
            card.Controls.Add(subjectsLabel);
            card.Controls.Add(deleteBtn);

            card.Resize += (_, _) =>
            {
                accentBar.Size = new Size(4, card.Height);
                deleteBtn.Location = new Point(card.Width - 130, 39);
            };

            return card;
        }

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            if (parts.Length == 1) return parts[0][0].ToString().ToUpper();
            return (parts[0][0].ToString() + parts[^1][0].ToString()).ToUpper();
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
                    LoadTeachersView();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("foreign key") || ex.Message.Contains("CONSTRAINT"))
                    {
                        MessageBox.Show(
                            $"Cannot delete '{fullName}' because they have existing evaluation submissions in the system.\n\n" +
                            "You must delete or reassign those submissions before removing this teacher.",
                            "Cannot Delete Teacher",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Error deleting teacher: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ShowAddTeacherForm()
        {
            // Show add teacher form panel inline
            var dialog = new Form
            {
                Text = "Add New Teacher",
                Size = new Size(500, 380),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            int y = 16;

            var title = new Label
            {
                Text = "Add New Teacher",
                Font = new Font("Inter", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, y)
            };
            dialog.Controls.Add(title);
            y += 32;

            // Row 1: First Name | Last Name
            var firstNameLabel = new Label
            {
                Text = "First Name *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            dialog.Controls.Add(firstNameLabel);

            var lastNameLabel = new Label
            {
                Text = "Last Name *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(240, y)
            };
            dialog.Controls.Add(lastNameLabel);
            y += 20;

            firstNameInput.Location = new Point(20, y);
            firstNameInput.Size = new Size(200, 26);
            firstNameInput.Font = new Font("Inter", 10F);
            firstNameInput.BorderStyle = BorderStyle.FixedSingle;
            firstNameInput.BackColor = Color.FromArgb(248, 250, 252);
            dialog.Controls.Add(firstNameInput);

            lastNameInput.Location = new Point(240, y);
            lastNameInput.Size = new Size(220, 26);
            lastNameInput.Font = new Font("Inter", 10F);
            lastNameInput.BorderStyle = BorderStyle.FixedSingle;
            lastNameInput.BackColor = Color.FromArgb(248, 250, 252);
            dialog.Controls.Add(lastNameInput);
            y += 44;

            // Row 2: Email (full width)
            var emailLabel = new Label
            {
                Text = "Email *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            dialog.Controls.Add(emailLabel);
            y += 20;

            emailInput.Location = new Point(20, y);
            emailInput.Size = new Size(440, 26);
            emailInput.Font = new Font("Inter", 10F);
            emailInput.BorderStyle = BorderStyle.FixedSingle;
            emailInput.BackColor = Color.FromArgb(248, 250, 252);
            dialog.Controls.Add(emailInput);
            y += 44;

            // Row 3: Department | Password
            var deptLabel = new Label
            {
                Text = "Department *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            dialog.Controls.Add(deptLabel);

            var passwordLabel = new Label
            {
                Text = "Password *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(240, y)
            };
            dialog.Controls.Add(passwordLabel);
            y += 20;

            departmentSelector.Items.Clear();
            departmentSelector.Items.AddRange(new[] { "BSIT", "BSCS", "BSCE", "BSEE", "BSME", "BSN", "BSA", "BSBA", "Education", "Science", "Engineering", "Other" });
            departmentSelector.SelectedIndex = 0;
            departmentSelector.Location = new Point(20, y);
            departmentSelector.Size = new Size(200, 26);
            departmentSelector.Font = new Font("Inter", 10F);
            departmentSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            dialog.Controls.Add(departmentSelector);

            passwordInput.UseSystemPasswordChar = true;
            passwordInput.Location = new Point(240, y);
            passwordInput.Size = new Size(220, 26);
            passwordInput.Font = new Font("Inter", 10F);
            passwordInput.BorderStyle = BorderStyle.FixedSingle;
            passwordInput.BackColor = Color.FromArgb(248, 250, 252);
            dialog.Controls.Add(passwordInput);
            y += 44;

            // Row 4: Subjects (full width)
            var subjectsLabel = new Label
            {
                Text = "Subjects (comma-separated) *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            dialog.Controls.Add(subjectsLabel);
            y += 20;

            subjectsInput.PlaceholderText = "e.g. Math, Physics, Programming";
            subjectsInput.Location = new Point(20, y);
            subjectsInput.Size = new Size(440, 26);
            subjectsInput.Font = new Font("Inter", 10F);
            subjectsInput.BorderStyle = BorderStyle.FixedSingle;
            subjectsInput.BackColor = Color.FromArgb(248, 250, 252);
            dialog.Controls.Add(subjectsInput);
            y += 48;

            // Buttons
            var cancelBtn = new Button
            {
                Text = "Cancel",
                BackColor = Color.FromArgb(243, 244, 246),
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(90, 36),
                Location = new Point(268, y)
            };
            cancelBtn.Click += (_, _) => dialog.Close();

            var addBtn = new Button
            {
                Text = "Add Teacher",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(110, 36),
                Location = new Point(370, y)
            };
            addBtn.Click += (_, _) =>
            {
                if (ValidateAndAddTeacher())
                {
                    dialog.Close();
                    LoadTeachersView();
                }
            };

            dialog.Controls.Add(cancelBtn);
            dialog.Controls.Add(addBtn);

            dialog.ShowDialog(this);
        }

        private static void AddInputField(Form parent, string labelText, ref int y, TextBox input)
        {
            var label = new Label
            {
                Text = labelText,
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(24, y)
            };
            parent.Controls.Add(label);
            y += 24;

            input.Location = new Point(24, y);
            input.Size = new Size(440, 28);
            input.Font = new Font("Inter", 10F);
            input.BorderStyle = BorderStyle.FixedSingle;
            input.BackColor = Color.FromArgb(248, 250, 252);
            parent.Controls.Add(input);
        }

        private bool ValidateAndAddTeacher()
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
                return false;
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Weak Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(subjectsText))
            {
                MessageBox.Show("Please enter at least one subject.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (TeacherStore.EmailExists(email))
            {
                MessageBox.Show("A teacher with this email already exists.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
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

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding teacher: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
