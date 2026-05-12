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
        private readonly Button studentsBtn = new();
        private readonly Label dashboardSubtitleLabel = new();
        private readonly Label responsesSubtitleLabel = new();
        private readonly Label studentsSubtitleLabel = new();

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
        private readonly TextBox teacherSectionInput = new();
        private readonly TextBox teacherCourseInput = new();
        private readonly TextBox teacherYearLevelInput = new();
        private readonly Button profileBtn = new();
        private readonly Button commentsBtn = new();
        private readonly FlowLayoutPanel commentsListPanel = new();
        private readonly Label statsLabel4 = new();
        private bool showingResponses;
        private bool showingTeachers;
        private bool showingStudents;
        private bool showingComments;

        public AdminHome()
        {
            InitializeComponent();
            ConfigureAdminUi();
            LoadFormsList();
            UpdateStats();
            FormDataStore.FormsUpdated += OnDataUpdated;
            FormDataStore.ResponsesUpdated += OnDataUpdated;
            FormDataStore.CommentsUpdated += OnDataUpdated;
            FormClosed += (_, _) =>
            {
                FormDataStore.FormsUpdated -= OnDataUpdated;
                FormDataStore.ResponsesUpdated -= OnDataUpdated;
                FormDataStore.CommentsUpdated -= OnDataUpdated;
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

            studentsBtn.Text = "  Manage Students";
            studentsBtn.BackColor = Color.Transparent;
            studentsBtn.ForeColor = Color.FromArgb(203, 213, 225);
            studentsBtn.FlatStyle = FlatStyle.Flat;
            studentsBtn.FlatAppearance.BorderSize = 0;
            studentsBtn.Font = new Font("Inter", 11F);
            studentsBtn.Size = new Size(192, 48);
            studentsBtn.Location = new Point(24, 204);
            studentsBtn.TextAlign = ContentAlignment.MiddleLeft;
            studentsBtn.Click += (_, _) => ShowStudentManagement();

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

            commentsBtn.Text = "  Comments";
            commentsBtn.BackColor = Color.Transparent;
            commentsBtn.ForeColor = Color.FromArgb(203, 213, 225);
            commentsBtn.FlatStyle = FlatStyle.Flat;
            commentsBtn.FlatAppearance.BorderSize = 0;
            commentsBtn.Font = new Font("Inter", 11F);
            commentsBtn.Size = new Size(192, 48);
            commentsBtn.Location = new Point(24, 264);
            commentsBtn.TextAlign = ContentAlignment.MiddleLeft;
            commentsBtn.Click += (_, _) => ShowComments();

            sidebar.Controls.Add(dashboardBtn);
            sidebar.Controls.Add(responsesBtn);
            sidebar.Controls.Add(teachersBtn);
            sidebar.Controls.Add(studentsBtn);
            sidebar.Controls.Add(commentsBtn);
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

            profileBtn.Text = "Profile";
            profileBtn.BackColor = Color.FromArgb(241, 245, 249);
            profileBtn.ForeColor = Color.FromArgb(15, 23, 42);
            profileBtn.FlatStyle = FlatStyle.Flat;
            profileBtn.FlatAppearance.BorderSize = 0;
            profileBtn.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            profileBtn.Size = new Size(100, 44);
            profileBtn.Location = new Point(580, 14);
            profileBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            profileBtn.Click += (_, _) =>
            {
                var profilePage = new AdminProfilePage();
                profilePage.SetProfileInfo(SessionStore.UserName, "Administrator", SessionStore.Email, SessionStore.UserId);
                profilePage.ShowDialog(this);
            };

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
            header.Controls.Add(profileBtn);
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

            commentsListPanel.FlowDirection = FlowDirection.TopDown;
            commentsListPanel.WrapContents = false;
            commentsListPanel.AutoScroll = true;
            commentsListPanel.BackColor = Color.Transparent;
            commentsListPanel.Location = new Point(32, 56);
            commentsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 72);
            commentsListPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            commentsListPanel.Visible = false;

            contentPanel.Controls.Add(dashboardSubtitleLabel);
            contentPanel.Controls.Add(responsesSubtitleLabel);
            contentPanel.Controls.Add(formsListPanel);
            contentPanel.Controls.Add(teachersListPanel);
            contentPanel.Controls.Add(commentsListPanel);
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

            statsLabel4.BackColor = Color.FromArgb(255, 247, 237);
            statsLabel4.Size = new Size(cardWidth, cardHeight);
            statsLabel4.Location = new Point(startX + (cardWidth + spacing) * 3, 52);
            statsLabel4.Padding = new Padding(16);
            statsLabel4.Font = new Font("Inter", 11F);
            statsLabel4.ForeColor = Color.FromArgb(154, 52, 18);
            statsLabel4.TextAlign = ContentAlignment.TopLeft;

            contentPanel.Controls.Add(statsLabel1);
            contentPanel.Controls.Add(statsLabel2);
            contentPanel.Controls.Add(statsLabel3);
            contentPanel.Controls.Add(statsLabel4);
        }

        private void UpdateStats()
        {
            var forms = FormDataStore.GetAllForms();
            var activeCount = forms.Count(f => f.IsActive);
            var totalResponses = forms.Sum(f => FormDataStore.GetSubmissionCount(f.Id));

            int pendingComments = 0;
            try { pendingComments = FormDataStore.GetPendingCommentCount(); } catch { }
            statsLabel1.Text = $"Total Forms\n\n{forms.Count}";
            statsLabel2.Text = $"Active Forms\n\n{activeCount}";
            statsLabel3.Text = $"Total Responses\n\n{totalResponses}";
            statsLabel4.Text = $"Pending Comments\n\n{pendingComments}";
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

            profileBtn.Location = new Point(header.ClientSize.Width - createFormBtn.Width - profileBtn.Width - 36, 14);
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
                LoadResponsesView();
            else if (showingComments)
                LoadCommentsView(false);
            else
                LoadFormsList();

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
            showingStudents = false;
            showingComments = false;
            UpdateNavButtons();
            titleLabel.Text = "Evaluation Forms Management";
            createFormBtn.Visible = true;
            dashboardSubtitleLabel.Visible = true;
            responsesSubtitleLabel.Visible = false;
            statsLabel1.Visible = true;
            statsLabel2.Visible = true;
            statsLabel3.Visible = true;
            statsLabel4.Visible = true;
            formsListPanel.Visible = true;
            formsListPanel.Location = new Point(32, 190);
            formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 206);
            teachersListPanel.Visible = false;
            commentsListPanel.Visible = false;
            LoadFormsList();
        }

        private void ShowResponses()
        {
            showingResponses = true;
            showingTeachers = false;
            showingStudents = false;
            showingComments = false;
            UpdateNavButtons();
            titleLabel.Text = "Student Responses";
            createFormBtn.Visible = false;
            dashboardSubtitleLabel.Visible = false;
            responsesSubtitleLabel.Visible = true;
            statsLabel1.Visible = false;
            statsLabel2.Visible = false;
            statsLabel3.Visible = false;
            statsLabel4.Visible = false;
            formsListPanel.Visible = true;
            formsListPanel.Location = new Point(32, 56);
            formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 72);
            teachersListPanel.Visible = false;
            commentsListPanel.Visible = false;
            LoadResponsesView();
        }

        private void ShowTeachers()
        {
            showingResponses = false;
            showingTeachers = true;
            showingStudents = false;
            showingComments = false;
            UpdateNavButtons();
            titleLabel.Text = "Teacher Management";
            createFormBtn.Visible = false;
            dashboardSubtitleLabel.Visible = false;
            responsesSubtitleLabel.Visible = false;
            statsLabel1.Visible = false;
            statsLabel2.Visible = false;
            statsLabel3.Visible = false;
            statsLabel4.Visible = false;
            formsListPanel.Visible = false;
            teachersListPanel.Visible = true;
            commentsListPanel.Visible = false;
            LoadTeachersView();
        }

        private void ShowComments()
        {
            showingResponses = false;
            showingTeachers = false;
            showingStudents = false;
            showingComments = true;
            UpdateNavButtons();
            titleLabel.Text = "Comment Moderation";
            createFormBtn.Visible = false;
            dashboardSubtitleLabel.Visible = false;
            responsesSubtitleLabel.Visible = false;
            statsLabel1.Visible = false;
            statsLabel2.Visible = false;
            statsLabel3.Visible = false;
            statsLabel4.Visible = false;
            formsListPanel.Visible = false;
            teachersListPanel.Visible = false;
            commentsListPanel.Visible = true;
            commentsListPanel.Location = new Point(32, 56);
            commentsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 72);
            LoadCommentsView(false);
        }

        private void UpdateNavButtons()
        {
            bool onDash = !showingResponses && !showingTeachers && !showingStudents && !showingComments;
            dashboardBtn.BackColor = onDash ? Color.FromArgb(38, 166, 91) : Color.Transparent;
            dashboardBtn.ForeColor = onDash ? Color.White : Color.FromArgb(203, 213, 225);
            responsesBtn.BackColor = showingResponses ? Color.FromArgb(38, 166, 91) : Color.Transparent;
            responsesBtn.ForeColor = showingResponses ? Color.White : Color.FromArgb(203, 213, 225);
            teachersBtn.BackColor = showingTeachers ? Color.FromArgb(38, 166, 91) : Color.Transparent;
            teachersBtn.ForeColor = showingTeachers ? Color.White : Color.FromArgb(203, 213, 225);
            studentsBtn.BackColor = showingStudents ? Color.FromArgb(38, 166, 91) : Color.Transparent;
            studentsBtn.ForeColor = showingStudents ? Color.White : Color.FromArgb(203, 213, 225);
            commentsBtn.BackColor = showingComments ? Color.FromArgb(38, 166, 91) : Color.Transparent;
            commentsBtn.ForeColor = showingComments ? Color.White : Color.FromArgb(203, 213, 225);
        }

        private void LoadResponsesView()
        {
            formsListPanel.Controls.Clear();
            var forms = FormDataStore.GetAllForms().OrderByDescending(f => f.CreatedAt).ToList();

            var totalResponses = forms.Sum(f => FormDataStore.GetSubmissionCount(f.Id));
            var formsWithResponses = forms.Count(f => FormDataStore.GetSubmissionCount(f.Id) > 0);

            // Header stats bar
            var statsPanel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(formsListPanel.Width - 40, 64),
                Margin = new Padding(0, 0, 0, 12)
            };
            statsPanel.Controls.Add(new Label
            {
                Text = "Response Overview",
                Font = new Font("Inter", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, 10)
            });
            statsPanel.Controls.Add(new Label
            {
                Text = $"{formsWithResponses} forms with responses  •  {totalResponses} total submissions",
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(20, 38)
            });
            formsListPanel.Controls.Add(statsPanel);

            bool hasAny = false;
            foreach (var form in forms)
            {
                var responses = FormDataStore.GetResponsesForForm(form.Id)
                    .OrderByDescending(r => r.SubmittedAt)
                    .ToList();
                if (!responses.Any()) continue;
                hasAny = true;

                formsListPanel.Controls.Add(CreateAccordionSection(form, responses));
            }

            if (!hasAny)
            {
                var emptyPanel = new Panel
                {
                    BackColor = Color.White,
                    Size = new Size(formsListPanel.Width - 40, 200),
                    Margin = new Padding(0, 16, 0, 0)
                };
                emptyPanel.Controls.Add(new Label
                {
                    Text = "No responses submitted yet",
                    Font = new Font("Inter", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = true,
                    Location = new Point(32, 80)
                });
                emptyPanel.Controls.Add(new Label
                {
                    Text = "Student evaluations will appear here once submitted",
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(32, 112)
                });
                formsListPanel.Controls.Add(emptyPanel);
            }
        }

        private Panel CreateAccordionSection(EvaluationForm form, List<FormResponse> responses)
        {
            bool expanded = true;

            // Rows panel — FlowLayout so height auto-grows
            var rowsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(12, 8, 12, 8),
                Visible = true
            };

            // Accordion header
            var header = new Panel
            {
                BackColor = Color.White,
                Height = 52,
                Cursor = Cursors.Hand
            };

            var indicator = new Panel
            {
                BackColor = Color.FromArgb(38, 166, 91),
                Size = new Size(4, 52),
                Location = new Point(0, 0)
            };

            var formTitleLabel = new Label
            {
                Text = form.Title,
                Font = new Font("Inter SemiBold", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(18, 16)
            };

            var countBadge = new Label
            {
                Text = $"{responses.Count} response{(responses.Count == 1 ? "" : "s")}",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(38, 166, 91),
                BackColor = Color.FromArgb(220, 252, 231),
                AutoSize = true,
                Padding = new Padding(8, 3, 8, 3),
                Location = new Point(18 + formTitleLabel.PreferredWidth + 12, 16)
            };

            var chevron = new Label
            {
                Text = "▲",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var sendAllBtn = new Button
            {
                Text = "Send All",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(80, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };

            header.Controls.Add(indicator);
            header.Controls.Add(formTitleLabel);
            header.Controls.Add(countBadge);
            header.Controls.Add(sendAllBtn);
            header.Controls.Add(chevron);

            header.Resize += (_, _) =>
            {
                chevron.Location = new Point(header.Width - 28, 18);
                sendAllBtn.Location = new Point(header.Width - 120, 11);
            };

            // Build row controls before wiring toggle so they exist
            foreach (var response in responses)
                rowsPanel.Controls.Add(CreateCompactResponseRow(form, response));

            sendAllBtn.Click += (_, _) =>
            {
                int sent = 0;
                int skipped = 0;
                int noTeacher = 0;
                string lastTeacherName = "";
                foreach (var response in responses)
                {
                    if (response.TeacherId <= 0) { noTeacher++; continue; }
                    if (TeacherStore.HasReportBeenSent(response.TeacherId, form.Id, response.Id)) { skipped++; continue; }
                    decimal avgScore = 0;
                    var ratingQs = form.Questions.Where(q => q.Type == QuestionType.Rating).ToList();
                    if (ratingQs.Any())
                    {
                        var vals = ratingQs
                            .Select(q => response.Answers.TryGetValue(q.Id, out var a) && decimal.TryParse(a, out var v) ? (decimal?)v : null)
                            .Where(v => v.HasValue).Select(v => v!.Value).ToList();
                        if (vals.Any()) avgScore = vals.Average();
                    }
                    try
                    {
                        TeacherStore.SaveReport(response.TeacherId, form.Id, avgScore, 1, BuildResponseReport(form, response));
                        sent++;
                        lastTeacherName = response.TeacherName;
                    }
                    catch { /* individual failures are silent in batch */ }
                }

                var msg = sent > 0
                    ? $"Sent {sent} report{(sent == 1 ? "" : "s")}."
                    : "No new reports to send.";
                if (skipped > 0) msg += $"\n{skipped} already sent (skipped).";
                if (noTeacher > 0) msg += $"\n{noTeacher} response{(noTeacher == 1 ? "" : "s")} skipped (no linked teacher).";

                MessageBox.Show(msg, sent > 0 ? "Send All Complete" : "Nothing to Send",
                    MessageBoxButtons.OK, sent > 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                if (sent > 0)
                    LoadResponsesView();
            };

            // Toggle expand/collapse
            EventHandler toggleHandler = (_, _) =>
            {
                expanded = !expanded;
                rowsPanel.Visible = expanded;
                chevron.Text = expanded ? "▲" : "▼";
                header.BackColor = expanded ? Color.White : Color.FromArgb(248, 250, 252);
            };
            header.Click += toggleHandler;
            // Don't attach toggle to sendAllBtn — it handles its own click
            foreach (Control c in header.Controls)
                if (c != sendAllBtn) c.Click += toggleHandler;

            // Outer wrapper stacks header + rows
            var wrapper = new Panel
            {
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 12),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            header.Dock = DockStyle.Top;
            rowsPanel.Dock = DockStyle.Top;

            wrapper.Controls.Add(rowsPanel);
            wrapper.Controls.Add(header);   // added second so it docks on top

            wrapper.Width = formsListPanel.Width - 40;
            wrapper.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            return wrapper;
        }

        private Panel CreateCompactResponseRow(EvaluationForm form, FormResponse response)
        {
            string teacherDisplay = response.TeacherId > 0
                ? response.TeacherName
                : (string.IsNullOrWhiteSpace(form.TargetTeacher) ? "—" : form.TargetTeacher);

            string displayName = string.IsNullOrWhiteSpace(response.StudentName)
                ? response.StudentId
                : response.StudentName;

            // Quick average rating
            double? avgRating = null;
            var ratingQs = form.Questions.Where(q => q.Type == QuestionType.Rating).ToList();
            if (ratingQs.Any())
            {
                var vals = ratingQs
                    .Select(q => response.Answers.TryGetValue(q.Id, out var a) && int.TryParse(a, out var v) ? (int?)v : null)
                    .Where(v => v.HasValue).Select(v => v!.Value).ToList();
                if (vals.Any()) avgRating = vals.Average();
            }

            var row = new Panel
            {
                BackColor = Color.White,
                Size = new Size(formsListPanel.Width - 64, 72),
                Margin = new Padding(0, 0, 0, 4),
                Cursor = Cursors.Default,
                Tag = "response-row"
            };

            var accentBar = new Panel
            {
                BackColor = Color.FromArgb(38, 166, 91),
                Size = new Size(3, 72),
                Location = new Point(0, 0)
            };

            // Avatar — photo if available, otherwise initials circle
            Control avatar;
            if (response.Avatar != null && response.Avatar.Length > 0)
            {
                using var ms = new System.IO.MemoryStream(response.Avatar);
                avatar = new PictureBox
                {
                    Size = new Size(38, 38),
                    Location = new Point(12, 17),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = Image.FromStream(ms),
                    BackColor = Color.White
                };
            }
            else
            {
                var initPanel = new Panel
                {
                    BackColor = Color.FromArgb(38, 166, 91),
                    Size = new Size(38, 38),
                    Location = new Point(12, 17)
                };
                var initChars = displayName.Split(' ')
                    .Where(s => !string.IsNullOrEmpty(s)).Take(2)
                    .Select(s => s[0]).ToArray();
                initPanel.Controls.Add(new Label
                {
                    Text = new string(initChars).ToUpper(),
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = false,
                    Size = new Size(38, 38),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(0, 0)
                });
                avatar = initPanel;
            }

            var nameLabel = new Label
            {
                Text = displayName,
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(58, 10)
            };

            var idLabel = new Label
            {
                Text = response.StudentId,
                Font = new Font("Inter", 8F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(58, 30)
            };

            var teacherLabel = new Label
            {
                Text = $"→ {teacherDisplay}",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(58, 46)
            };

            var dateLabel = new Label
            {
                Text = response.SubmittedAt.ToString("MMM dd, yyyy  HH:mm"),
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var ratingLabel = new Label
            {
                Text = avgRating.HasValue ? $"★ {avgRating.Value:0.0}" : "",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(234, 179, 8),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var detailsBtn = new Button
            {
                Text = "Details",
                BackColor = Color.FromArgb(240, 253, 244),
                ForeColor = Color.FromArgb(22, 101, 52),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(70, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            detailsBtn.Click += (_, _) => ShowResponseDetails(form, response);

            bool alreadySent = response.TeacherId > 0 &&
                TeacherStore.HasReportBeenSent(response.TeacherId, form.Id, response.Id);

            var sendBtn = new Button
            {
                Text = alreadySent ? "Sent" : "Send",
                BackColor = alreadySent ? Color.FromArgb(148, 163, 184) : Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(60, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = alreadySent ? Cursors.Default : Cursors.Hand,
                Enabled = !alreadySent
            };
            sendBtn.Click += (_, _) => SendReportToTeacher(form, response, BuildResponseReport(form, response));

            row.Controls.Add(accentBar);
            row.Controls.Add(avatar);
            row.Controls.Add(nameLabel);
            row.Controls.Add(idLabel);
            row.Controls.Add(teacherLabel);
            row.Controls.Add(dateLabel);
            row.Controls.Add(ratingLabel);
            row.Controls.Add(detailsBtn);
            row.Controls.Add(sendBtn);

            row.Resize += (_, _) =>
            {
                accentBar.Size       = new Size(3, row.Height);
                dateLabel.Location   = new Point(row.Width - 420, 27);
                ratingLabel.Location = new Point(row.Width - 260, 27);
                detailsBtn.Location  = new Point(row.Width - 160, 22);
                sendBtn.Location     = new Point(row.Width - 80, 22);
            };

            // trigger initial layout
            int rowW = formsListPanel.Width - 64;
            dateLabel.Location   = new Point(rowW - 420, 27);
            ratingLabel.Location = new Point(rowW - 260, 27);
            detailsBtn.Location  = new Point(rowW - 160, 22);
            sendBtn.Location     = new Point(rowW - 80, 22);

            return row;
        }

        private Panel CreateEnhancedResponseCard(EvaluationForm form, FormResponse response, int width, int top)
        {
            return CreateCompactResponseRow(form, response);
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

            // Additional comment section
            var comment = FormDataStore.GetCommentForSubmission(response.Id);
            if (comment != null)
            {
                var commentSectionPanel = new Panel
                {
                    Size = new Size(600, 110),
                    Location = new Point(32, y),
                    BackColor = Color.FromArgb(248, 250, 252),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Color levelBg = comment.SystemLevel switch
                {
                    CommentLevel.Severe   => Color.FromArgb(254, 226, 226),
                    CommentLevel.Moderate => Color.FromArgb(255, 237, 213),
                    CommentLevel.Mild     => Color.FromArgb(254, 252, 232),
                    _                     => Color.FromArgb(220, 252, 231)
                };
                Color levelFg = comment.SystemLevel switch
                {
                    CommentLevel.Severe   => Color.FromArgb(153, 27, 27),
                    CommentLevel.Moderate => Color.FromArgb(154, 52, 18),
                    CommentLevel.Mild     => Color.FromArgb(133, 77, 14),
                    _                     => Color.FromArgb(22, 101, 52)
                };
                Color statusFg = comment.Status switch
                {
                    CommentStatus.Approved => Color.FromArgb(22, 101, 52),
                    CommentStatus.Rejected => Color.FromArgb(153, 27, 27),
                    _                      => Color.FromArgb(154, 52, 18)
                };
                Color statusBg = comment.Status switch
                {
                    CommentStatus.Approved => Color.FromArgb(220, 252, 231),
                    CommentStatus.Rejected => Color.FromArgb(254, 226, 226),
                    _                      => Color.FromArgb(255, 247, 237)
                };

                var commentHeaderLabel = new Label
                {
                    Text = "Additional Comment",
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    AutoSize = true,
                    Location = new Point(16, 12)
                };

                var levelBadge = new Label
                {
                    Text = comment.SystemLevel.ToString(),
                    Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                    ForeColor = levelFg,
                    BackColor = levelBg,
                    AutoSize = true,
                    Padding = new Padding(6, 2, 6, 2),
                    Location = new Point(152, 10)
                };

                var statusBadge = new Label
                {
                    Text = comment.Status.ToString(),
                    Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                    ForeColor = statusFg,
                    BackColor = statusBg,
                    AutoSize = true,
                    Padding = new Padding(6, 2, 6, 2),
                    Location = new Point(220, 10)
                };

                var commentTextBox = new TextBox
                {
                    Text = comment.CommentText,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Inter", 10F),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    Location = new Point(16, 40),
                    Size = new Size(568, 60)
                };

                commentSectionPanel.Controls.Add(commentHeaderLabel);
                commentSectionPanel.Controls.Add(levelBadge);
                commentSectionPanel.Controls.Add(statusBadge);
                commentSectionPanel.Controls.Add(commentTextBox);
                scrollPanel.Controls.Add(commentSectionPanel);
                y += 125;
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
                $"SubmissionID:{response.Id}",
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
            if (response.TeacherId <= 0)
            {
                MessageBox.Show("This response has no linked teacher. Assign a teacher to the evaluation form first.", "Cannot Send", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (TeacherStore.HasReportBeenSent(response.TeacherId, form.Id, response.Id))
            {
                MessageBox.Show("This report has already been sent to the teacher.", "Already Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Compute average rating score
            decimal avgScore = 0;
            var ratingQs = form.Questions.Where(q => q.Type == QuestionType.Rating).ToList();
            if (ratingQs.Any())
            {
                var vals = ratingQs
                    .Select(q => response.Answers.TryGetValue(q.Id, out var a) && decimal.TryParse(a, out var v) ? (decimal?)v : null)
                    .Where(v => v.HasValue).Select(v => v!.Value).ToList();
                if (vals.Any()) avgScore = vals.Average();
            }

            try
            {
                TeacherStore.SaveReport(response.TeacherId, form.Id, avgScore, 1, reportBody);
                MessageBox.Show(
                    $"Report sent to {response.TeacherName}.\n\nStudent: {response.StudentName}\nForm: {form.Title}",
                    "Report Sent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCommentsView(bool showAll)
        {
            commentsListPanel.Controls.Clear();

            var comments = showAll ? FormDataStore.GetAllComments() : FormDataStore.GetPendingComments();

            // Filter tab strip
            var tabStrip = new Panel
            {
                BackColor = Color.White,
                Size = new Size(commentsListPanel.Width - 40, 52),
                Margin = new Padding(0, 0, 0, 12)
            };

            var pendingTab = new Button
            {
                Text = $"Pending ({FormDataStore.GetPendingCommentCount()})",
                BackColor = !showAll ? Color.FromArgb(38, 166, 91) : Color.FromArgb(248, 250, 252),
                ForeColor = !showAll ? Color.White : Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(140, 36),
                Location = new Point(16, 8)
            };
            pendingTab.Click += (_, _) => LoadCommentsView(false);

            var allTab = new Button
            {
                Text = "All Comments",
                BackColor = showAll ? Color.FromArgb(38, 166, 91) : Color.FromArgb(248, 250, 252),
                ForeColor = showAll ? Color.White : Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(140, 36),
                Location = new Point(168, 8)
            };
            allTab.Click += (_, _) => LoadCommentsView(true);

            tabStrip.Controls.Add(pendingTab);
            tabStrip.Controls.Add(allTab);
            commentsListPanel.Controls.Add(tabStrip);

            if (!comments.Any())
            {
                var emptyPanel = new Panel
                {
                    BackColor = Color.White,
                    Size = new Size(commentsListPanel.Width - 40, 160),
                    Margin = new Padding(0, 0, 0, 0)
                };
                var emptyLabel = new Label
                {
                    Text = showAll ? "No comments found." : "No pending comments.",
                    Font = new Font("Inter", 13F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(32, 60)
                };
                emptyPanel.Controls.Add(emptyLabel);
                commentsListPanel.Controls.Add(emptyPanel);
                return;
            }

            foreach (var comment in comments)
            {
                var card = CreateCommentCard(comment, showAll);
                commentsListPanel.Controls.Add(card);
            }
        }

        private Panel CreateCommentCard(FormComment comment, bool showAll)
        {
            var effectiveLevel = comment.AdminLevel ?? comment.SystemLevel;

            Color levelBg = effectiveLevel switch
            {
                CommentLevel.Severe   => Color.FromArgb(254, 226, 226),
                CommentLevel.Moderate => Color.FromArgb(255, 237, 213),
                CommentLevel.Mild     => Color.FromArgb(254, 252, 232),
                _                     => Color.FromArgb(220, 252, 231)
            };
            Color levelFg = effectiveLevel switch
            {
                CommentLevel.Severe   => Color.FromArgb(153, 27, 27),
                CommentLevel.Moderate => Color.FromArgb(154, 52, 18),
                CommentLevel.Mild     => Color.FromArgb(133, 77, 14),
                _                     => Color.FromArgb(22, 101, 52)
            };
            Color accentColor = effectiveLevel switch
            {
                CommentLevel.Severe   => Color.FromArgb(220, 38, 38),
                CommentLevel.Moderate => Color.FromArgb(249, 115, 22),
                CommentLevel.Mild     => Color.FromArgb(234, 179, 8),
                _                     => Color.FromArgb(38, 166, 91)
            };

            bool isSevere = effectiveLevel == CommentLevel.Severe;
            int cardHeight = isSevere ? 320 : 270;

            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(commentsListPanel.Width - 40, cardHeight),
                Margin = new Padding(0, 0, 0, 14),
                Padding = new Padding(0)
            };

            var accentBar = new Panel
            {
                BackColor = accentColor,
                Size = new Size(5, card.Height),
                Location = new Point(0, 0)
            };
            card.Controls.Add(accentBar);

            int x = 20;
            int y = 16;

            // Student name
            var nameLabel = new Label
            {
                Text = string.IsNullOrEmpty(comment.StudentName) ? comment.StudentId : comment.StudentName,
                Font = new Font("Inter SemiBold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(x, y)
            };
            card.Controls.Add(nameLabel);

            // Form title badge
            var formBadge = new Label
            {
                Text = comment.FormTitle,
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                BackColor = Color.FromArgb(241, 245, 249),
                AutoSize = true,
                Padding = new Padding(6, 2, 6, 2),
                Location = new Point(x, y + 28)
            };
            card.Controls.Add(formBadge);

            // Date submitted
            var dateLabel = new Label
            {
                Text = comment.SubmittedAt.ToString("MMM dd, yyyy HH:mm"),
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(x, y + 52)
            };
            card.Controls.Add(dateLabel);

            // System level badge
            var sysLevelBadge = new Label
            {
                Text = $"System: {comment.SystemLevel}",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = levelFg,
                BackColor = levelBg,
                AutoSize = true,
                Padding = new Padding(8, 3, 8, 3),
                Location = new Point(card.Width - 280, y)
            };
            sysLevelBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            card.Controls.Add(sysLevelBadge);

            // Status badge
            Color statusBg = comment.Status switch
            {
                CommentStatus.Approved => Color.FromArgb(220, 252, 231),
                CommentStatus.Rejected => Color.FromArgb(254, 226, 226),
                _                      => Color.FromArgb(255, 247, 237)
            };
            Color statusFg = comment.Status switch
            {
                CommentStatus.Approved => Color.FromArgb(22, 101, 52),
                CommentStatus.Rejected => Color.FromArgb(153, 27, 27),
                _                      => Color.FromArgb(154, 52, 18)
            };
            var statusBadge = new Label
            {
                Text = comment.Status.ToString(),
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = statusFg,
                BackColor = statusBg,
                AutoSize = true,
                Padding = new Padding(8, 3, 8, 3),
                Location = new Point(card.Width - 170, y)
            };
            statusBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            card.Controls.Add(statusBadge);

            // Comment text box
            var commentBox = new TextBox
            {
                Text = comment.CommentText,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Inter", 11F),
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(x, y + 82),
                Size = new Size(card.Width - x * 2 - 10, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            card.Controls.Add(commentBox);

            // Classifier description
            var classifierDesc = new Label
            {
                Text = CommentClassifier.GetLevelDescription(comment.SystemLevel),
                Font = new Font("Inter", 9F, FontStyle.Italic),
                ForeColor = levelFg,
                AutoSize = false,
                Size = new Size(card.Width - x * 2 - 10, 20),
                Location = new Point(x, y + 158),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            card.Controls.Add(classifierDesc);

            // Severe: student contact info banner
            if (isSevere)
            {
                var contactBanner = new Panel
                {
                    BackColor = Color.FromArgb(254, 226, 226),
                    Size = new Size(card.Width - x * 2 - 10, 44),
                    Location = new Point(x, y + 184),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };
                var contactLabel = new Label
                {
                    Text = $"Contact Student  |  ID: {comment.StudentId}" +
                           (string.IsNullOrEmpty(comment.StudentEmail) ? "" : $"  |  Email: {comment.StudentEmail}"),
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(153, 27, 27),
                    AutoSize = false,
                    Size = new Size(contactBanner.Width - 16, 44),
                    Location = new Point(8, 0),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                contactBanner.Controls.Add(contactLabel);
                card.Controls.Add(contactBanner);
            }

            int btnY = isSevere ? y + 240 : y + 190;

            // Admin level override
            var levelLabel = new Label
            {
                Text = "Set Level:",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(x, btnY + 4)
            };
            card.Controls.Add(levelLabel);

            var levelDropdown = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Inter", 10F),
                Location = new Point(x + 72, btnY),
                Size = new Size(110, 26)
            };
            levelDropdown.Items.AddRange(new[] { "Normal", "Mild", "Moderate", "Severe" });
            levelDropdown.SelectedItem = (comment.AdminLevel ?? comment.SystemLevel).ToString();
            card.Controls.Add(levelDropdown);

            // Approve button
            var approveBtn = new Button
            {
                Text = "Approve",
                BackColor = Color.FromArgb(220, 252, 231),
                ForeColor = Color.FromArgb(22, 101, 52),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(90, 32),
                Location = new Point(card.Width - 204, btnY),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            approveBtn.Click += (_, _) =>
            {
                var selectedLevel = levelDropdown.SelectedItem?.ToString() ?? comment.SystemLevel.ToString();
                var adminLevel = Enum.TryParse<CommentLevel>(selectedLevel, out var lv) ? lv : comment.SystemLevel;
                FormDataStore.UpdateCommentStatus(comment.Id, CommentStatus.Approved, adminLevel, SessionStore.UserName);
                LoadCommentsView(showAll);
                UpdateStats();
            };
            card.Controls.Add(approveBtn);

            // Reject button
            var rejectBtn = new Button
            {
                Text = "Reject",
                BackColor = Color.FromArgb(254, 226, 226),
                ForeColor = Color.FromArgb(153, 27, 27),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(90, 32),
                Location = new Point(card.Width - 104, btnY),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            rejectBtn.Click += (_, _) =>
            {
                var selectedLevel = levelDropdown.SelectedItem?.ToString() ?? comment.SystemLevel.ToString();
                var adminLevel = Enum.TryParse<CommentLevel>(selectedLevel, out var lv) ? lv : comment.SystemLevel;
                FormDataStore.UpdateCommentStatus(comment.Id, CommentStatus.Rejected, adminLevel, SessionStore.UserName);
                LoadCommentsView(showAll);
                UpdateStats();
            };
            card.Controls.Add(rejectBtn);

            // Disable approve/reject if already reviewed
            if (comment.Status != CommentStatus.Pending)
            {
                approveBtn.Enabled = false;
                rejectBtn.Enabled = false;
                levelDropdown.Enabled = false;
                var reviewedLabel = new Label
                {
                    Text = $"Reviewed by {(string.IsNullOrEmpty(comment.ReviewedBy) ? "admin" : comment.ReviewedBy)} on {comment.ReviewedAt:MMM dd, yyyy}",
                    Font = new Font("Inter", 8F, FontStyle.Italic),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(x, btnY + 36)
                };
                card.Controls.Add(reviewedLabel);
            }

            card.Resize += (_, _) =>
            {
                accentBar.Size = new Size(5, card.Height);
                commentBox.Size = new Size(card.Width - x * 2 - 10, 70);
                classifierDesc.Size = new Size(card.Width - x * 2 - 10, 20);
                if (isSevere)
                {
                    var banner = card.Controls.OfType<Panel>().FirstOrDefault(p => p.BackColor == Color.FromArgb(254, 226, 226) && p.Location.Y == y + 184);
                    if (banner != null) banner.Size = new Size(card.Width - x * 2 - 10, 44);
                }
                sysLevelBadge.Location = new Point(card.Width - 280, y);
                statusBadge.Location = new Point(card.Width - 170, y);
                approveBtn.Location = new Point(card.Width - 204, btnY);
                rejectBtn.Location = new Point(card.Width - 104, btnY);
            };

            return card;
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
                Size = new Size(teachersListPanel.Width - 40, 130),
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

            // Edit button
            var editBtn = new Button
            {
                Text = "✏️ Edit",
                BackColor = Color.FromArgb(224, 242, 254),
                ForeColor = Color.FromArgb(3, 105, 161),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(75, 32),
                Location = new Point(card.Width - 200, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            editBtn.Click += (_, _) => EditTeacher(teacher);
            editBtn.MouseEnter += (_, _) => editBtn.BackColor = Color.FromArgb(186, 230, 253);
            editBtn.MouseLeave += (_, _) => editBtn.BackColor = Color.FromArgb(224, 242, 254);

            // Delete button
            var deleteBtn = new Button
            {
                Text = "🗑 Delete",
                BackColor = Color.FromArgb(254, 226, 226),
                ForeColor = Color.FromArgb(185, 28, 28),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(100, 32),
                Location = new Point(card.Width - 115, 70),
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
            card.Controls.Add(editBtn);
            card.Controls.Add(deleteBtn);

            card.Resize += (_, _) =>
            {
                accentBar.Size = new Size(4, card.Height);
                editBtn.Location = new Point(card.Width - 200, 70);
                deleteBtn.Location = new Point(card.Width - 115, 70);
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

        private void EditTeacher(Teacher teacher)
        {
            var dialog = new Form
            {
                Text = "Edit Teacher",
                Size = new Size(500, 480),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            int y = 16;

            var title = new Label
            {
                Text = "Edit Teacher",
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

            var txtFirstName = new TextBox
            {
                Text = teacher.FirstName,
                Location = new Point(20, y),
                Size = new Size(200, 26),
                Font = new Font("Inter", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            dialog.Controls.Add(txtFirstName);

            var txtLastName = new TextBox
            {
                Text = teacher.LastName,
                Location = new Point(240, y),
                Size = new Size(220, 26),
                Font = new Font("Inter", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            dialog.Controls.Add(txtLastName);
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

            var txtEmail = new TextBox
            {
                Text = teacher.Email,
                Location = new Point(20, y),
                Size = new Size(440, 26),
                Font = new Font("Inter", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            dialog.Controls.Add(txtEmail);
            y += 44;

            // Row 3: Department
            var deptLabel = new Label
            {
                Text = "Department *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            dialog.Controls.Add(deptLabel);
            y += 20;

            var cmbDepartment = new ComboBox
            {
                Location = new Point(20, y),
                Size = new Size(200, 26),
                Font = new Font("Inter", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDepartment.Items.AddRange(new[] { "BSIT", "BSCS", "BSCE", "BSEE", "BSME", "BSN", "BSA", "BSBA", "Education", "Science", "Engineering", "Other" });
            cmbDepartment.SelectedItem = teacher.Department ?? "BSIT";
            dialog.Controls.Add(cmbDepartment);
            y += 44;

            // Row 4: Section | Course | Year Level
            var sectionLabel = new Label
            {
                Text = "Section",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            dialog.Controls.Add(sectionLabel);

            var courseLabel2 = new Label
            {
                Text = "Course *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(160, y)
            };
            dialog.Controls.Add(courseLabel2);

            var yearLevelLabel = new Label
            {
                Text = "Year Level *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(320, y)
            };
            dialog.Controls.Add(yearLevelLabel);
            y += 20;

            var txtSection = new TextBox
            {
                Text = teacher.Section,
                Location = new Point(20, y),
                Size = new Size(120, 26),
                Font = new Font("Inter", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            dialog.Controls.Add(txtSection);

            var txtCourse = new TextBox
            {
                Text = teacher.Course,
                Location = new Point(160, y),
                Size = new Size(140, 26),
                Font = new Font("Inter", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            dialog.Controls.Add(txtCourse);

            var txtYearLevel = new TextBox
            {
                Text = teacher.YearLevel,
                Location = new Point(320, y),
                Size = new Size(140, 26),
                Font = new Font("Inter", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            dialog.Controls.Add(txtYearLevel);
            y += 44;

            // Row 5: Subjects
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

            var txtSubjects = new TextBox
            {
                Text = string.Join(", ", teacher.Subjects),
                Location = new Point(20, y),
                Size = new Size(440, 26),
                Font = new Font("Inter", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            dialog.Controls.Add(txtSubjects);
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

            var saveBtn = new Button
            {
                Text = "Save Changes",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(110, 36),
                Location = new Point(370, y)
            };
            saveBtn.Click += (_, _) =>
            {
                if (ValidateTeacherEditInput(txtFirstName.Text, txtLastName.Text, txtEmail.Text, txtCourse.Text, txtYearLevel.Text, txtSubjects.Text))
                {
                    teacher.FirstName = txtFirstName.Text.Trim();
                    teacher.LastName = txtLastName.Text.Trim();
                    teacher.Email = txtEmail.Text.Trim();
                    teacher.Department = cmbDepartment.SelectedItem?.ToString() ?? "";
                    teacher.Section = txtSection.Text.Trim();
                    teacher.Course = txtCourse.Text.Trim();
                    teacher.YearLevel = txtYearLevel.Text.Trim();
                    teacher.Subjects = txtSubjects.Text.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();

                    try
                    {
                        TeacherStore.UpdateTeacher(teacher);
                        MessageBox.Show("Teacher updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dialog.Close();
                        LoadTeachersView();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating teacher: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            dialog.Controls.Add(cancelBtn);
            dialog.Controls.Add(saveBtn);

            dialog.ShowDialog(this);
        }

        private bool ValidateTeacherEditInput(string firstName, string lastName, string email, string course, string yearLevel, string subjects)
        {
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

            if (string.IsNullOrWhiteSpace(course))
            {
                MessageBox.Show("Please enter the course.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(yearLevel))
            {
                MessageBox.Show("Please enter the year level.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(subjects))
            {
                MessageBox.Show("Please enter at least one subject.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ShowAddTeacherForm()
        {
            // Show add teacher form panel inline
            var dialog = new Form
            {
                Text = "Add New Teacher",
                Size = new Size(500, 480),
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

            // Row 5: Section | Course | Year Level (for student matching)
            var sectionLabel = new Label
            {
                Text = "Section (for student matching)",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            dialog.Controls.Add(sectionLabel);

            var courseLabel2 = new Label
            {
                Text = "Course *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(160, y)
            };
            dialog.Controls.Add(courseLabel2);

            var yearLevelLabel = new Label
            {
                Text = "Year Level *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(320, y)
            };
            dialog.Controls.Add(yearLevelLabel);
            y += 20;

            teacherSectionInput.PlaceholderText = "e.g. A";
            teacherSectionInput.Location = new Point(20, y);
            teacherSectionInput.Size = new Size(120, 26);
            teacherSectionInput.Font = new Font("Inter", 10F);
            teacherSectionInput.BorderStyle = BorderStyle.FixedSingle;
            teacherSectionInput.BackColor = Color.FromArgb(248, 250, 252);
            dialog.Controls.Add(teacherSectionInput);

            teacherCourseInput.PlaceholderText = "e.g. BSIT";
            teacherCourseInput.Location = new Point(160, y);
            teacherCourseInput.Size = new Size(140, 26);
            teacherCourseInput.Font = new Font("Inter", 10F);
            teacherCourseInput.BorderStyle = BorderStyle.FixedSingle;
            teacherCourseInput.BackColor = Color.FromArgb(248, 250, 252);
            dialog.Controls.Add(teacherCourseInput);

            teacherYearLevelInput.PlaceholderText = "e.g. 2";
            teacherYearLevelInput.Location = new Point(320, y);
            teacherYearLevelInput.Size = new Size(140, 26);
            teacherYearLevelInput.Font = new Font("Inter", 10F);
            teacherYearLevelInput.BorderStyle = BorderStyle.FixedSingle;
            teacherYearLevelInput.BackColor = Color.FromArgb(248, 250, 252);
            dialog.Controls.Add(teacherYearLevelInput);
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
            string section = teacherSectionInput.Text.Trim();
            string course = teacherCourseInput.Text.Trim();
            string yearLevel = teacherYearLevelInput.Text.Trim();

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

            if (string.IsNullOrWhiteSpace(course))
            {
                MessageBox.Show("Please enter the course this teacher is assigned to.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(yearLevel))
            {
                MessageBox.Show("Please enter the year level this teacher is assigned to.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                Section = section,
                Course = course,
                YearLevel = yearLevel,
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
                teacherSectionInput.Clear();
                teacherCourseInput.Clear();
                teacherYearLevelInput.Clear();
                departmentSelector.SelectedIndex = 0;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding teacher: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ShowStudentManagement()
        {
            showingResponses = false;
            showingTeachers = false;
            showingStudents = true;
            showingComments = false;
            UpdateNavButtons();
            titleLabel.Text = "Student Management";
            createFormBtn.Visible = false;
            dashboardSubtitleLabel.Visible = false;
            responsesSubtitleLabel.Visible = false;
            statsLabel1.Visible = false;
            statsLabel2.Visible = false;
            statsLabel3.Visible = false;
            statsLabel4.Visible = false;
            formsListPanel.Visible = false;
            teachersListPanel.Visible = true;
            commentsListPanel.Visible = false;
            LoadStudentsView();
        }

        private void LoadStudentsView()
        {
            teachersListPanel.Controls.Clear();
            var students = StudentStore.GetAllStudents();

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
                Text = "All Students",
                Font = new Font("Inter", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            var statsText = $"{students.Count} Total Students";
            if (students.Count > 0)
            {
                var courses = students.Select(s => s.Course).Distinct().Count();
                var yearLevels = students.Select(s => s.YearLevel).Distinct().Count();
                statsText += $"  ·  {courses} Courses  ·  {yearLevels} Year Levels";
            }

            var statsLabel = new Label
            {
                Text = statsText,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(24, 50)
            };

            // Add Student button in header
            var addBtn = new Button
            {
                Text = "+ Add New Student",
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
            addBtn.Click += (_, _) => AddStudent();

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(statsLabel);
            headerPanel.Controls.Add(addBtn);
            headerPanel.Resize += (_, _) => addBtn.Location = new Point(headerPanel.Width - 184, 50);

            teachersListPanel.Controls.Add(headerPanel);

            if (students.Count == 0)
            {
                var emptyPanel = new Panel
                {
                    BackColor = Color.White,
                    Size = new Size(teachersListPanel.Width - 40, 200),
                    Margin = new Padding(0, 16, 0, 0)
                };

                var emptyIcon = new Label
                {
                    Text = "🎓",
                    Font = new Font("Segoe UI", 48F),
                    AutoSize = true,
                    Location = new Point((emptyPanel.Width - 60) / 2, 40)
                };

                var emptyLabel = new Label
                {
                    Text = "No students added yet",
                    Font = new Font("Inter", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = true,
                    Location = new Point((emptyPanel.Width - 200) / 2, 110)
                };

                var emptySubLabel = new Label
                {
                    Text = "Click '+ Add New Student' to add your first student",
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

            foreach (var student in students)
            {
                var card = CreateStudentCard(student);
                teachersListPanel.Controls.Add(card);
            }
        }

        private Panel CreateStudentCard(Student student)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(teachersListPanel.Width - 40, 130),
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

            // Avatar - show profile picture if available, otherwise initials
            Control avatar;
            if (student.Avatar != null && student.Avatar.Length > 0)
            {
                using var ms = new System.IO.MemoryStream(student.Avatar);
                var img = Image.FromStream(ms);
                avatar = new PictureBox
                {
                    Image = new Bitmap(img, new Size(50, 50)),
                    Size = new Size(50, 50),
                    Location = new Point(20, 20),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent
                };
                img.Dispose();
            }
            else
            {
                var avatarPanel = new Panel
                {
                    BackColor = Color.FromArgb(38, 166, 91),
                    Size = new Size(50, 50),
                    Location = new Point(20, 20)
                };
                var initials = GetInitials($"{student.FirstName} {student.LastName}");
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
                avatarPanel.Controls.Add(initialsLabel);
                avatar = avatarPanel;
            }

            // Name
            var nameLabel = new Label
            {
                Text = $"{student.FirstName} {student.LastName}",
                Font = new Font("Inter SemiBold", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                MaximumSize = new Size(card.Width - 290, 0),
                Location = new Point(85, 20)
            };

            // ID
            var idLabel = new Label
            {
                Text = $"ID: {student.IDNumber}",
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(85, 48)
            };

            // Email
            var emailLabel = new Label
            {
                Text = student.Email,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                MaximumSize = new Size(card.Width - 290, 0),
                Location = new Point(85, 66)
            };

            // Course, Year, and Section
            var sectionText = string.IsNullOrEmpty(student.Section) ? "N/A" : student.Section;
            var courseLabel = new Label
            {
                Text = $"{student.Course} - Year {student.YearLevel} - Section {sectionText}",
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                MaximumSize = new Size(card.Width - 290, 0),
                Location = new Point(85, 84)
            };

            // Action buttons - positioned below text info
            var editButton = new Button
            {
                Text = "Edit",
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(60, 32),
                Location = new Point(card.Width - 245, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            editButton.Click += (_, _) => EditStudent(student);

            var viewTeachersButton = new Button
            {
                Text = "Teachers",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(80, 32),
                Location = new Point(card.Width - 170, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            viewTeachersButton.Click += (_, _) => ViewStudentTeachers(student);

            var deleteButton = new Button
            {
                Text = "Delete",
                BackColor = Color.FromArgb(220, 38, 38),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(70, 32),
                Location = new Point(card.Width - 85, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            deleteButton.Click += (_, _) => DeleteStudent(student);

            card.Resize += (_, _) =>
            {
                editButton.Location = new Point(card.Width - 245, 70);
                viewTeachersButton.Location = new Point(card.Width - 170, 70);
                deleteButton.Location = new Point(card.Width - 85, 70);
                nameLabel.MaximumSize = new Size(card.Width - 290, 0);
                emailLabel.MaximumSize = new Size(card.Width - 290, 0);
                courseLabel.MaximumSize = new Size(card.Width - 290, 0);
            };

            card.Controls.AddRange(new Control[] { accentBar, avatar, nameLabel, idLabel, emailLabel, courseLabel, editButton, viewTeachersButton, deleteButton });

            return card;
        }

        private void AddStudent()
        {
            var dialog = new Form
            {
                Text = "Add New Student",
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(241, 245, 249),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Size = new Size(500, 500),
                MaximizeBox = false,
                MinimizeBox = false
            };

            int y = 24;

            // ID Number
            var lblId = new Label { Text = "ID Number:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblId);
            y += 24;

            var txtId = new TextBox { Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(400, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtId);
            y += 48;

            // First Name
            var lblFirstName = new Label { Text = "First Name:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblFirstName);
            y += 24;

            var txtFirstName = new TextBox { Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(400, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtFirstName);
            y += 48;

            // Last Name
            var lblLastName = new Label { Text = "Last Name:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblLastName);
            y += 24;

            var txtLastName = new TextBox { Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(400, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtLastName);
            y += 48;

            // Email
            var lblEmail = new Label { Text = "Email:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblEmail);
            y += 24;

            var txtEmail = new TextBox { Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(400, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtEmail);
            y += 48;

            // Course
            var lblCourse = new Label { Text = "Course:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblCourse);
            y += 24;

            var txtCourse = new TextBox { Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(400, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtCourse);
            y += 48;

            // Year Level
            var lblYear = new Label { Text = "Year Level:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblYear);
            y += 24;

            var txtYear = new TextBox { Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(100, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtYear);

            var lblSection = new Label { Text = "Section:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(150, y - 24), AutoSize = true };
            dialog.Controls.Add(lblSection);

            var txtSection = new TextBox { Font = new Font("Inter", 11F), Location = new Point(150, y), Size = new Size(100, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtSection);
            y += 48;

            // Password
            var lblPassword = new Label { Text = "Password:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblPassword);
            y += 24;

            var txtPassword = new TextBox { Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(400, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, UseSystemPasswordChar = true };
            dialog.Controls.Add(txtPassword);

            // Buttons
            var btnCancel = new Button
            {
                Text = "Cancel",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(71, 85, 105),
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(100, 36),
                Location = new Point(280, 420)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (_, _) => dialog.Close();
            dialog.Controls.Add(btnCancel);

            var btnSave = new Button
            {
                Text = "Save",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(100, 36),
                Location = new Point(390, 420)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (_, _) =>
            {
                if (ValidateStudentInput(txtId.Text, txtFirstName.Text, txtLastName.Text, txtEmail.Text, txtCourse.Text, txtYear.Text, txtSection.Text, txtPassword.Text))
                {
                    var student = new Student
                    {
                        IDNumber = txtId.Text,
                        FirstName = txtFirstName.Text,
                        LastName = txtLastName.Text,
                        Email = txtEmail.Text,
                        Course = txtCourse.Text,
                        YearLevel = txtYear.Text,
                        Section = txtSection.Text
                    };

                    try
                    {
                        StudentStore.AddStudent(student, txtPassword.Text);
                        MessageBox.Show("Student added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dialog.Close();
                        LoadStudentsView(); // Refresh the list
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            dialog.Controls.Add(btnSave);

            dialog.ShowDialog(this);
        }

        private void EditStudent(Student student)
        {
            var dialog = new Form
            {
                Text = "Edit Student",
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(241, 245, 249),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Size = new Size(500, 480),
                MaximizeBox = false,
                MinimizeBox = false
            };

            int y = 24;

            // First Name
            var lblFirstName = new Label { Text = "First Name:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblFirstName);
            y += 24;

            var txtFirstName = new TextBox { Text = student.FirstName, Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(400, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtFirstName);
            y += 48;

            // Last Name
            var lblLastName = new Label { Text = "Last Name:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblLastName);
            y += 24;

            var txtLastName = new TextBox { Text = student.LastName, Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(400, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtLastName);
            y += 48;

            // Email
            var lblEmail = new Label { Text = "Email:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblEmail);
            y += 24;

            var txtEmail = new TextBox { Text = student.Email, Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(400, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtEmail);
            y += 48;

            // Course
            var lblCourse = new Label { Text = "Course:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblCourse);
            y += 24;

            var txtCourse = new TextBox { Text = student.Course, Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(400, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtCourse);
            y += 48;

            // Year Level
            var lblYear = new Label { Text = "Year Level:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, y), AutoSize = true };
            dialog.Controls.Add(lblYear);
            y += 24;

            var txtYear = new TextBox { Text = student.YearLevel, Font = new Font("Inter", 11F), Location = new Point(32, y), Size = new Size(100, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtYear);

            var lblSection = new Label { Text = "Section:", Font = new Font("Inter SemiBold", 10F), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(150, y - 24), AutoSize = true };
            dialog.Controls.Add(lblSection);

            var txtSection = new TextBox { Text = student.Section, Font = new Font("Inter", 11F), Location = new Point(150, y), Size = new Size(100, 28), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            dialog.Controls.Add(txtSection);

            // Buttons
            var btnCancel = new Button
            {
                Text = "Cancel",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(71, 85, 105),
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(100, 36),
                Location = new Point(280, 370)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (_, _) => dialog.Close();
            dialog.Controls.Add(btnCancel);

            var btnSave = new Button
            {
                Text = "Save",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(100, 36),
                Location = new Point(390, 370)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (_, _) =>
            {
                if (ValidateStudentEditInput(txtFirstName.Text, txtLastName.Text, txtEmail.Text, txtCourse.Text, txtYear.Text, txtSection.Text))
                {
                    student.FirstName = txtFirstName.Text;
                    student.LastName = txtLastName.Text;
                    student.Email = txtEmail.Text;
                    student.Course = txtCourse.Text;
                    student.YearLevel = txtYear.Text;
                    student.Section = txtSection.Text;

                    try
                    {
                        StudentStore.UpdateStudent(student);
                        MessageBox.Show("Student updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dialog.Close();
                        LoadStudentsView(); // Refresh the list
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            dialog.Controls.Add(btnSave);

            dialog.ShowDialog(this);
        }

        private void DeleteStudent(Student student)
        {
            var result = MessageBox.Show($"Are you sure you want to delete student '{student.FirstName} {student.LastName}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            
            if (result == DialogResult.Yes)
            {
                try
                {
                    StudentStore.DeleteStudent(student.StudentID);
                    MessageBox.Show("Student deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStudentsView(); // Refresh the list
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ViewStudentTeachers(Student student)
        {
            var dialog = new Form
            {
                Text = $"Teachers for {student.FirstName} {student.LastName}",
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(241, 245, 249),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Size = new Size(500, 400),
                MaximizeBox = false,
                MinimizeBox = false
            };

            // Header
            var headerPanel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(464, 80),
                Location = new Point(16, 16)
            };

            var studentInfoLabel = new Label
            {
                Text = $"{student.FirstName} {student.LastName} - {student.Course} Year {student.YearLevel}",
                Font = new Font("Inter SemiBold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(16, 12)
            };

            var sectionLabel = new Label
            {
                Text = $"Section: {student.DisplaySection} | ID: {student.IDNumber}",
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(16, 38)
            };

            headerPanel.Controls.Add(studentInfoLabel);
            headerPanel.Controls.Add(sectionLabel);
            dialog.Controls.Add(headerPanel);

            // Teachers list
            var listPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Location = new Point(16, 104),
                Size = new Size(464, 250)
            };

            var teachers = TeacherStore.GetTeachersForStudent(student.Section, student.Course, student.YearLevel);

            if (teachers.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "No teachers assigned for this student's section/course/year.",
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(0, 20)
                };
                listPanel.Controls.Add(emptyLabel);
            }
            else
            {
                foreach (var teacher in teachers)
                {
                    var teacherCard = CreateTeacherCard(teacher, listPanel.Width - 20);
                    listPanel.Controls.Add(teacherCard);
                }
            }

            dialog.Controls.Add(listPanel);

            // Close button
            var closeBtn = new Button
            {
                Text = "Close",
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(100, 36),
                Location = new Point(380, 360)
            };
            closeBtn.Click += (_, _) => dialog.Close();
            dialog.Controls.Add(closeBtn);

            dialog.ShowDialog(this);
        }

        private Panel CreateTeacherCard(Teacher teacher, int width)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(width, 70),
                Margin = new Padding(0, 0, 0, 8)
            };

            // Accent bar
            var accentBar = new Panel
            {
                BackColor = Color.FromArgb(59, 130, 246),
                Size = new Size(4, card.Height),
                Location = new Point(0, 0)
            };

            // Avatar with initials
            var avatar = new Panel
            {
                BackColor = Color.FromArgb(59, 130, 246),
                Size = new Size(40, 40),
                Location = new Point(16, 15)
            };

            var initials = GetInitials(teacher.FullName);
            var initialsLabel = new Label
            {
                Text = initials,
                Font = new Font("Inter SemiBold", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(40, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0)
            };
            avatar.Controls.Add(initialsLabel);

            // Name
            var nameLabel = new Label
            {
                Text = teacher.FullName,
                Font = new Font("Inter SemiBold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(68, 12)
            };

            // Department/Subjects
            var deptLabel = new Label
            {
                Text = $"{teacher.Department} | {teacher.SubjectsDisplay}",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                MaximumSize = new Size(card.Width - 100, 0),
                Location = new Point(68, 34)
            };

            card.Controls.Add(accentBar);
            card.Controls.Add(avatar);
            card.Controls.Add(nameLabel);
            card.Controls.Add(deptLabel);

            return card;
        }

        private bool ValidateStudentInput(string id, string firstName, string lastName, string email, string course, string year, string section, string password)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || 
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(year) || 
                string.IsNullOrWhiteSpace(section) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("All fields are required.", "Required Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!email.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Weak Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (StudentStore.EmailExists(email))
            {
                MessageBox.Show("A student with this email already exists.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (StudentStore.IdExists(id))
            {
                MessageBox.Show("A student with this ID already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool ValidateStudentEditInput(string firstName, string lastName, string email, string course, string year, string section)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || 
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(year) || 
                string.IsNullOrWhiteSpace(section))
            {
                MessageBox.Show("All fields are required.", "Required Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!email.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}
