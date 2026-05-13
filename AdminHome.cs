using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly Button reportsBtn = new();
        private readonly FlowLayoutPanel reportsListPanel = new();
        private readonly ComboBox reportsFilter = new();
        private bool showingResponses;
        private bool showingTeachers;
        private bool showingStudents;
        private bool showingComments;
        private bool showingReports;
        private readonly ComboBox semesterFilter = new();
        private bool populatingSemesterFilter;

        // Student management pagination fields
        private List<Student>? _allStudents;
        private List<Student>? _filteredStudents;
        private readonly TextBox _studentSearchBox = new();
        private readonly Label _studentCountLabel = new();
        private readonly Panel _studentListContainer = new();
        private readonly Label _pageInfoLabel = new();
        private readonly Button _prevPageBtn = new();
        private readonly Button _nextPageBtn = new();
        private const int _studentsPerPage = 50;
        private int _currentPage = 1;
        private System.Windows.Forms.Timer? _searchDebounceTimer;

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

            // Delayed refresh after 200ms to ensure proper layout
            var refreshTimer = new System.Windows.Forms.Timer { Interval = 200 };
            refreshTimer.Tick += (_, _) =>
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
                ShowDashboard();
            };
            refreshTimer.Start();
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

            reportsBtn.Text = "  Manage Reports";
            reportsBtn.BackColor = Color.Transparent;
            reportsBtn.ForeColor = Color.FromArgb(203, 213, 225);
            reportsBtn.FlatStyle = FlatStyle.Flat;
            reportsBtn.FlatAppearance.BorderSize = 0;
            reportsBtn.Font = new Font("Inter", 11F);
            reportsBtn.Size = new Size(192, 48);
            reportsBtn.Location = new Point(24, 324);
            reportsBtn.TextAlign = ContentAlignment.MiddleLeft;
            reportsBtn.Click += (_, _) => ShowReports();

            sidebar.Controls.Add(dashboardBtn);
            sidebar.Controls.Add(responsesBtn);
            sidebar.Controls.Add(teachersBtn);
            sidebar.Controls.Add(studentsBtn);
            sidebar.Controls.Add(commentsBtn);
            sidebar.Controls.Add(reportsBtn);
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

            reportsListPanel.FlowDirection = FlowDirection.TopDown;
            reportsListPanel.WrapContents = false;
            reportsListPanel.AutoScroll = true;
            reportsListPanel.BackColor = Color.Transparent;
            reportsListPanel.Location = new Point(32, 56);
            reportsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 72);
            reportsListPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            reportsListPanel.Visible = false;

            reportsFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            reportsFilter.Font = new Font("Inter", 10F);
            reportsFilter.Location = new Point(32, 18);
            reportsFilter.Size = new Size(240, 32);
            reportsFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            reportsFilter.Visible = false;
            reportsFilter.SelectedIndexChanged += (_, _) => { if (showingReports) LoadReportsView(); };
            contentPanel.Controls.Add(reportsFilter);

            semesterFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            semesterFilter.Font = new Font("Inter", 10F);
            semesterFilter.Location = new Point(32, 140);
            semesterFilter.Size = new Size(220, 32);
            semesterFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            semesterFilter.Visible = false;
            semesterFilter.SelectedIndexChanged += (_, _) => { if (!populatingSemesterFilter && !showingResponses && !showingTeachers && !showingStudents && !showingComments) LoadFormsList(); };

            contentPanel.Controls.Add(dashboardSubtitleLabel);
            contentPanel.Controls.Add(responsesSubtitleLabel);
            contentPanel.Controls.Add(semesterFilter);
            contentPanel.Controls.Add(formsListPanel);
            contentPanel.Controls.Add(teachersListPanel);
            contentPanel.Controls.Add(commentsListPanel);
            contentPanel.Controls.Add(reportsListPanel);
        }

        private void ConfigureStatsCards()
        {
            // Compact layout
            int cardWidth = 155;
            int cardHeight = 75;
            int startX = 24;
            int spacing = 10;

            statsLabel1.BackColor = Color.White;
            statsLabel1.Size = new Size(cardWidth, cardHeight);
            statsLabel1.Location = new Point(startX, 52);
            statsLabel1.Padding = new Padding(12, 10, 12, 10);
            statsLabel1.Font = new Font("Inter", 10F);
            statsLabel1.ForeColor = Color.FromArgb(71, 85, 105);
            statsLabel1.TextAlign = ContentAlignment.TopLeft;

            statsLabel2.BackColor = Color.White;
            statsLabel2.Size = new Size(cardWidth, cardHeight);
            statsLabel2.Location = new Point(startX + cardWidth + spacing, 52);
            statsLabel2.Padding = new Padding(12, 10, 12, 10);
            statsLabel2.Font = new Font("Inter", 10F);
            statsLabel2.ForeColor = Color.FromArgb(71, 85, 105);
            statsLabel2.TextAlign = ContentAlignment.TopLeft;

            statsLabel3.BackColor = Color.White;
            statsLabel3.Size = new Size(cardWidth, cardHeight);
            statsLabel3.Location = new Point(startX + (cardWidth + spacing) * 2, 52);
            statsLabel3.Padding = new Padding(12, 10, 12, 10);
            statsLabel3.Font = new Font("Inter", 10F);
            statsLabel3.ForeColor = Color.FromArgb(71, 85, 105);
            statsLabel3.TextAlign = ContentAlignment.TopLeft;

            statsLabel4.BackColor = Color.FromArgb(255, 247, 237);
            statsLabel4.Size = new Size(cardWidth, cardHeight);
            statsLabel4.Location = new Point(startX + (cardWidth + spacing) * 3, 52);
            statsLabel4.Padding = new Padding(12, 10, 12, 10);
            statsLabel4.Font = new Font("Inter", 10F);
            statsLabel4.ForeColor = Color.FromArgb(154, 52, 18);
            statsLabel4.TextAlign = ContentAlignment.TopLeft;

            contentPanel.Controls.Add(statsLabel1);
            contentPanel.Controls.Add(statsLabel2);
            contentPanel.Controls.Add(statsLabel3);
            contentPanel.Controls.Add(statsLabel4);

            PopulateSemesterFilter();
        }

        private void PopulateSemesterFilter()
        {
            populatingSemesterFilter = true;
            try
            {
                var forms = FormDataStore.GetAllForms();
                var semesters = forms
                    .Where(f => !string.IsNullOrEmpty(f.Semester) && !string.IsNullOrEmpty(f.SchoolYear))
                    .Select(f => $"{f.Semester} Sem {f.SchoolYear}")
                    .Distinct()
                    .OrderByDescending(s => s)
                    .ToList();

                string? prev = semesterFilter.SelectedItem?.ToString();
                semesterFilter.Items.Clear();
                semesterFilter.Items.Add("All Semesters");
                foreach (var s in semesters)
                    semesterFilter.Items.Add(s);

                int idx = prev != null ? semesterFilter.Items.IndexOf(prev) : 0;
                semesterFilter.SelectedIndex = idx >= 0 ? idx : 0;
            }
            finally
            {
                populatingSemesterFilter = false;
            }
        }

        private void UpdateStats()
        {
            var forms = FormDataStore.GetAllForms();
            var activeCount = forms.Count(f => f.IsActive);
            var totalResponses = forms.Sum(f => FormDataStore.GetSubmissionCount(f.Id));

            int pendingComments = 0;
            try { pendingComments = FormDataStore.GetPendingCommentCount(); } catch { }
            statsLabel1.Text = $"Total Forms\n{forms.Count}";
            statsLabel2.Text = $"Active Forms\n{activeCount}";
            statsLabel3.Text = $"Responses\n{totalResponses}";
            statsLabel4.Text = $"Pending\n{pendingComments}";
        }

        private void LoadFormsList()
        {
            PopulateSemesterFilter();
            formsListPanel.Controls.Clear();
            var allForms = FormDataStore.GetAllForms().OrderByDescending(f => f.CreatedAt).ToList();

            string? filterValue = semesterFilter.SelectedItem?.ToString();
            IEnumerable<EvaluationForm> forms = allForms;
            if (!string.IsNullOrEmpty(filterValue) && filterValue != "All Semesters")
            {
                forms = allForms.Where(f =>
                    !string.IsNullOrEmpty(f.Semester) && !string.IsNullOrEmpty(f.SchoolYear) &&
                    $"{f.Semester} Sem {f.SchoolYear}" == filterValue);
            }

            if (!forms.Any())
            {
                var emptyLabel = new Label
                {
                    Text = allForms.Any() ? "No forms match the selected semester filter." : "No forms yet. Click 'Create New Form' to get started.",
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

            string semesterTag = (!string.IsNullOrEmpty(form.Semester) && !string.IsNullOrEmpty(form.SchoolYear))
                ? $" | {form.Semester} Sem {form.SchoolYear}" : "";
            var meta = new Label
            {
                Text = $"{form.Questions.Count} questions | Target: {(!string.IsNullOrWhiteSpace(form.TargetCourse) ? form.TargetCourse : "All")}{semesterTag} | Created: {form.CreatedAt:MMM dd, yyyy}",
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

            Label? semBadge = null;
            if (!string.IsNullOrEmpty(form.Semester) && !string.IsNullOrEmpty(form.SchoolYear))
            {
                semBadge = new Label
                {
                    Text = $"{form.Semester} Sem {form.SchoolYear}",
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(59, 130, 246),
                    AutoSize = true,
                    Padding = new Padding(8, 4, 8, 4),
                    Location = new Point(card.Width - 260, 20)
                };
            }

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
                Location = new Point(card.Width - 298, 90)
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
                Location = new Point(card.Width - 220, 90)
            };
            toggleBtn.Click += (_, _) => ToggleFormStatus(form);

            var deleteBtn = new Button
            {
                Text = "Delete",
                BackColor = Color.FromArgb(254, 242, 242),
                ForeColor = Color.FromArgb(185, 28, 28),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(70, 32),
                Location = new Point(card.Width - 110, 90)
            };
            deleteBtn.Click += (_, _) => DeleteForm(form);

            card.Controls.Add(title);
            card.Controls.Add(description);
            card.Controls.Add(meta);
            card.Controls.Add(statusBadge);
            if (semBadge != null) card.Controls.Add(semBadge);
            card.Controls.Add(responsesLabel);
            card.Controls.Add(editBtn);
            card.Controls.Add(toggleBtn);
            card.Controls.Add(deleteBtn);

            void RepositionBadges()
            {
                statusBadge.Location = new Point(card.Width - 100, 20);
                if (semBadge != null)
                    semBadge.Location = new Point(card.Width - semBadge.Width - 16, statusBadge.Bottom + 8);
            }
            card.Layout += (_, _) => RepositionBadges();
            card.Resize += (_, _) =>
            {
                RepositionBadges();
                responsesLabel.Location = new Point(card.Width - 120, 50);
                editBtn.Location = new Point(card.Width - 298, 90);
                toggleBtn.Location = new Point(card.Width - 220, 90);
                deleteBtn.Location = new Point(card.Width - 110, 90);
                description.MaximumSize = new Size(card.Width - 320, 0);
            };

            return card;
        }

        private void UpdateLayout()
        {
            logoutBtn.Location = new Point(24, sidebar.ClientSize.Height - logoutBtn.Height - 32);

            profileBtn.Location = new Point(header.ClientSize.Width - createFormBtn.Width - profileBtn.Width - 36, 14);
            createFormBtn.Location = new Point(header.ClientSize.Width - createFormBtn.Width - 24, 14);

            bool onDashboard = !showingResponses && !showingTeachers && !showingStudents && !showingComments && !showingReports;
            if (onDashboard)
            {
                formsListPanel.Location = new Point(32, 220);
                formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 236);
            }
            else if (showingResponses)
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

        private void DeleteForm(EvaluationForm form)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete the form '{form.Title}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    FormDataStore.DeleteForm(form.Id);
                    LoadFormsList();
                    UpdateStats();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowDashboard()
        {
            showingResponses = false;
            showingTeachers = false;
            showingStudents = false;
            showingComments = false;
            showingReports = false;
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
            formsListPanel.Location = new Point(32, 220);
            formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 236);
            semesterFilter.Visible = true;
            teachersListPanel.Visible = false;
            commentsListPanel.Visible = false;
            reportsListPanel.Visible = false;
            reportsFilter.Visible = false;
            LoadFormsList();
        }

        private void ShowResponses()
        {
            showingResponses = true;
            showingTeachers = false;
            showingStudents = false;
            showingComments = false;
            showingReports = false;
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
            semesterFilter.Visible = false;
            teachersListPanel.Visible = false;
            commentsListPanel.Visible = false;
            reportsListPanel.Visible = false;
            reportsFilter.Visible = false;
            LoadResponsesView();
        }

        private void ShowTeachers()
        {
            showingResponses = false;
            showingTeachers = true;
            showingStudents = false;
            showingComments = false;
            showingReports = false;
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
            semesterFilter.Visible = false;
            teachersListPanel.Visible = true;
            commentsListPanel.Visible = false;
            reportsListPanel.Visible = false;
            reportsFilter.Visible = false;
            LoadTeachersView();
        }

        private void ShowComments()
        {
            showingResponses = false;
            showingTeachers = false;
            showingStudents = false;
            showingComments = true;
            showingReports = false;
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
            semesterFilter.Visible = false;
            teachersListPanel.Visible = false;
            commentsListPanel.Visible = true;
            commentsListPanel.Location = new Point(32, 56);
            commentsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 72);
            reportsListPanel.Visible = false;
            reportsFilter.Visible = false;
            LoadCommentsView(false);
        }

        private void ShowReports()
        {
            showingResponses = false;
            showingTeachers = false;
            showingStudents = false;
            showingComments = false;
            showingReports = true;
            UpdateNavButtons();
            titleLabel.Text = "Sent Reports";
            createFormBtn.Visible = false;
            dashboardSubtitleLabel.Visible = false;
            responsesSubtitleLabel.Visible = false;
            statsLabel1.Visible = false;
            statsLabel2.Visible = false;
            statsLabel3.Visible = false;
            statsLabel4.Visible = false;
            formsListPanel.Visible = false;
            semesterFilter.Visible = false;
            teachersListPanel.Visible = false;
            commentsListPanel.Visible = false;
            reportsListPanel.Visible = true;
            reportsListPanel.Location = new Point(32, 64);
            reportsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 80);

            // Populate filter from all report semesters
            var allForFilter = TeacherStore.GetAllReports();
            var semOptions = allForFilter
                .Where(r => !string.IsNullOrEmpty(r.Semester) && !string.IsNullOrEmpty(r.SchoolYear))
                .Select(r => $"{r.Semester} Sem {r.SchoolYear}")
                .Distinct()
                .OrderByDescending(s => s)
                .ToList();
            reportsFilter.Items.Clear();
            reportsFilter.Items.Add("All Semesters");
            foreach (var opt in semOptions) reportsFilter.Items.Add(opt);
            reportsFilter.SelectedIndex = 0;
            reportsFilter.Visible = true;

            LoadReportsView();
        }

        private void LoadReportsView()
        {
            reportsListPanel.Controls.Clear();
            var allReports = TeacherStore.GetAllReports();

            // Apply semester filter
            string selectedFilter = reportsFilter.SelectedItem as string ?? "All Semesters";
            if (selectedFilter != "All Semesters")
            {
                allReports = allReports
                    .Where(r => $"{r.Semester} Sem {r.SchoolYear}" == selectedFilter)
                    .ToList();
            }

            // Group by teacher-subject combination (AssignmentID)
            var teacherSubjectGroups = allReports
                .GroupBy(r => new { r.TeacherID, r.AssignmentID })
                .OrderBy(g => g.First().TeacherName)
                .ThenBy(g => g.First().SubjectDisplay)
                .ToList();

            int cardCount = teacherSubjectGroups.Count;
            int formCount = allReports.Select(r => new { r.TeacherID, r.EvaluationID }).Distinct().Count();

            // Header stats bar
            var statsPanel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(reportsListPanel.Width - 40, 64),
                Margin = new Padding(0, 0, 0, 12)
            };
            statsPanel.Controls.Add(new Label
            {
                Text = "Sent Reports",
                Font = new Font("Inter", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, 10)
            });
            statsPanel.Controls.Add(new Label
            {
                Text = cardCount == 0
                    ? "No reports have been sent yet"
                    : $"{cardCount} report card{(cardCount == 1 ? "" : "s")}  ·  {formCount} form evaluation{(formCount == 1 ? "" : "s")}",
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(20, 38)
            });
            reportsListPanel.Controls.Add(statsPanel);

            if (!teacherSubjectGroups.Any())
            {
                var emptyPanel = new Panel
                {
                    BackColor = Color.White,
                    Size = new Size(reportsListPanel.Width - 40, 160),
                    Margin = new Padding(0, 0, 0, 0)
                };
                emptyPanel.Controls.Add(new Label
                {
                    Text = "No reports sent yet.",
                    Font = new Font("Inter", 13F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(32, 60)
                });
                reportsListPanel.Controls.Add(emptyPanel);
                return;
            }

            foreach (var group in teacherSubjectGroups)
            {
                // Per teacher-subject: group by form, average the scores
                var formRows = group
                    .GroupBy(r => r.EvaluationID)
                    .Select(fg => (
                        EvaluationID: fg.Key,
                        FormTitle: fg.First().FormTitle,
                        AvgScore: fg.Where(r => r.AverageScore > 0).Any()
                            ? fg.Where(r => r.AverageScore > 0).Average(r => (double)r.AverageScore)
                            : 0.0,
                        TotalResponses: fg.Sum(r => r.ResponseCount),
                        LatestDate: fg.Max(r => r.SubmissionDate),
                        ReportIDs: fg.Select(r => r.ReportID).ToList()
                    ))
                    .OrderByDescending(f => f.LatestDate)
                    .ToList();

                int teacherID = group.Key.TeacherID;
                string teacherName = group.First().TeacherName;
                string subjectDisplay = group.First().SubjectDisplay;
                string cardTitle = string.IsNullOrEmpty(subjectDisplay)
                    ? teacherName
                    : $"{teacherName}  -  {subjectDisplay}";
                
                var accordion = CreateTeacherReportAccordion(teacherID, cardTitle, formRows);
                reportsListPanel.Controls.Add(accordion);
            }
        }

        private Panel CreateTeacherReportAccordion(
            int teacherID,
            string teacherName,
            List<(int EvaluationID, string FormTitle, double AvgScore, int TotalResponses, DateTime LatestDate, List<int> ReportIDs)> formRows)
        {
            const int headerH = 60;
            const int rowHeaderH = 56;
            int panelW = reportsListPanel.Width - 40;

            // Tracks current height of each row (collapsed = rowHeaderH, expanded = rowHeaderH + detail)
            var rowHeights = new int[formRows.Count];
            for (int k = 0; k < rowHeights.Length; k++) rowHeights[k] = rowHeaderH;

            // Outer wrapper — starts collapsed (teacher header only)
            var wrapper = new Panel
            {
                BackColor = Color.White,
                Size = new Size(panelW, headerH),
                Margin = new Padding(0, 0, 0, 10)
            };

            bool teacherExpanded = false;

            // ── Teacher Header ───────────────────────────────────────
            var headerPanel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(panelW, headerH),
                Location = new Point(0, 0),
                Cursor = Cursors.Hand
            };

            var accentBar = new Panel
            {
                BackColor = Color.FromArgb(38, 166, 91),
                Size = new Size(4, headerH),
                Location = new Point(0, 0)
            };
            headerPanel.Controls.Add(accentBar);

            headerPanel.Controls.Add(new Label
            {
                Text = string.IsNullOrEmpty(teacherName) ? "Unknown Teacher" : teacherName,
                Font = new Font("Inter SemiBold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, 12)
            });

            int totalResponses = formRows.Sum(f => f.TotalResponses);
            headerPanel.Controls.Add(new Label
            {
                Text = $"{formRows.Count} form{(formRows.Count == 1 ? "" : "s")}  ·  {totalResponses} response{(totalResponses == 1 ? "" : "s")}",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(20, 38)
            });

            var chevron = new Label
            {
                Text = "▶",
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            headerPanel.Controls.Add(chevron);
            headerPanel.Layout += (_, _) =>
            {
                chevron.Location = new Point(headerPanel.Width - chevron.Width - 16, (headerH - chevron.Height) / 2);
                accentBar.Size = new Size(4, headerH);
            };
            wrapper.Controls.Add(headerPanel);

            // Helper: recalculate wrapper height and reposition all row panels
            void RecalcLayout(List<Panel> rps)
            {
                if (!teacherExpanded) { wrapper.Height = headerH; return; }
                int y = headerH;
                for (int k = 0; k < rps.Count; k++)
                {
                    rps[k].Location = new Point(0, y);
                    rps[k].Height = rowHeights[k];
                    y += rowHeights[k];
                }
                wrapper.Height = y;
            }

            // ── Form rows ────────────────────────────────────────────
            var rowPanels = new List<Panel>();
            for (int i = 0; i < formRows.Count; i++)
            {
                var fr = formRows[i];
                int capturedIdx = i;
                var rowPanel = new Panel
                {
                    BackColor = i % 2 == 0 ? Color.FromArgb(250, 251, 252) : Color.White,
                    Size = new Size(panelW, rowHeaderH),
                    Location = new Point(0, headerH + i * rowHeaderH),
                    Visible = false
                };

                // Left accent
                var rowAccent = new Panel
                {
                    BackColor = Color.FromArgb(187, 247, 208),
                    Size = new Size(3, rowHeaderH),
                    Location = new Point(0, 0)
                };
                rowPanel.Controls.Add(rowAccent);

                // Form title
                var titleLbl = new Label
                {
                    Text = string.IsNullOrEmpty(fr.FormTitle) ? "(Untitled form)" : fr.FormTitle,
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    AutoSize = true,
                    Location = new Point(20, 10)
                };
                rowPanel.Controls.Add(titleLbl);

                // Date + response count
                rowPanel.Controls.Add(new Label
                {
                    Text = $"{fr.LatestDate:MMM dd, yyyy}  ·  {fr.TotalResponses} response{(fr.TotalResponses == 1 ? "" : "s")}",
                    Font = new Font("Inter", 8F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(20, 34)
                });

                // Avg score badge
                var avgBadge = new Label
                {
                    Text = fr.AvgScore > 0 ? $"★ {fr.AvgScore:0.00} / 5" : "No rating",
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    ForeColor = fr.AvgScore > 0 ? Color.FromArgb(146, 64, 14) : Color.FromArgb(100, 116, 139),
                    BackColor = fr.AvgScore > 0 ? Color.FromArgb(254, 243, 199) : Color.FromArgb(241, 245, 249),
                    AutoSize = true,
                    Padding = new Padding(8, 3, 8, 3),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                rowPanel.Controls.Add(avgBadge);

                // Details toggle button
                var detailsBtn = new Button
                {
                    Text = "▶ Details",
                    BackColor = Color.FromArgb(241, 245, 249),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                    Size = new Size(80, 24),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Cursor = Cursors.Hand
                };
                rowPanel.Controls.Add(detailsBtn);

                // Delete button
                var deleteBtn = new Button
                {
                    Text = "🗑️",
                    BackColor = Color.FromArgb(254, 226, 226),
                    ForeColor = Color.FromArgb(185, 28, 28),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Inter", 9F),
                    Size = new Size(30, 30),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Cursor = Cursors.Hand
                };
                var capturedIDs = fr.ReportIDs;
                var capturedTeacher = teacherName;
                var capturedForm = fr.FormTitle;
                deleteBtn.Click += (_, _) =>
                {
                    var result = MessageBox.Show(
                        $"Delete all report entries for \"{capturedForm}\" sent to {capturedTeacher}?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        foreach (var id in capturedIDs)
                            TeacherStore.DeleteReport(id);
                        LoadReportsView();
                    }
                };
                deleteBtn.MouseEnter += (_, _) => deleteBtn.BackColor = Color.FromArgb(252, 210, 210);
                deleteBtn.MouseLeave += (_, _) => deleteBtn.BackColor = Color.FromArgb(254, 226, 226);
                rowPanel.Controls.Add(deleteBtn);

                rowPanel.Layout += (_, _) =>
                {
                    int rw = rowPanel.Width;
                    avgBadge.Location = new Point(rw - avgBadge.Width - 130, (rowHeaderH - avgBadge.Height) / 2);
                    detailsBtn.Location = new Point(rw - 126, (rowHeaderH - 24) / 2);
                    deleteBtn.Location = new Point(rw - 40, (rowHeaderH - 30) / 2);
                    rowAccent.Height = rowPanel.Height;
                };

                // ── Detail panel (rebuilt fresh on every expand) ──────
                Panel? detailPanel = null;
                bool detailExpanded = false;

                detailsBtn.Click += (_, _) =>
                {
                    detailExpanded = !detailExpanded;
                    detailsBtn.Text = detailExpanded ? "▼ Details" : "▶ Details";
                    detailsBtn.BackColor = detailExpanded ? Color.FromArgb(220, 252, 231) : Color.FromArgb(241, 245, 249);
                    detailsBtn.ForeColor = detailExpanded ? Color.FromArgb(22, 101, 52) : Color.FromArgb(71, 85, 105);

                    if (detailExpanded)
                    {
                        // Remove old panel so we always get fresh data
                        if (detailPanel != null)
                        {
                            rowPanel.Controls.Remove(detailPanel);
                            detailPanel.Dispose();
                        }
                        var form = FormDataStore.GetForm(fr.EvaluationID);
                        var sentIds = TeacherStore.GetSentSubmissionIds(teacherID, fr.EvaluationID);
                        var responses = FormDataStore.GetResponsesForForm(fr.EvaluationID)
                            .Where(r => r.TeacherId == teacherID && sentIds.Contains(r.Id))
                            .ToList();
                        detailPanel = BuildFormDetailPanel(form, responses, rowPanel.Width - 4);
                        detailPanel.Location = new Point(4, rowHeaderH);
                        rowPanel.Controls.Add(detailPanel);
                        rowHeights[capturedIdx] = rowHeaderH + detailPanel.Height;
                    }
                    else
                    {
                        if (detailPanel != null) detailPanel.Visible = false;
                        rowHeights[capturedIdx] = rowHeaderH;
                    }
                    RecalcLayout(rowPanels);
                };

                wrapper.Controls.Add(rowPanel);
                rowPanels.Add(rowPanel);
            }

            // ── Teacher header toggle ─────────────────────────────────
            void ToggleTeacher()
            {
                teacherExpanded = !teacherExpanded;
                chevron.Text = teacherExpanded ? "▼" : "▶";
                foreach (var rp in rowPanels)
                    rp.Visible = teacherExpanded;
                RecalcLayout(rowPanels);
            }

            headerPanel.Click += (_, _) => ToggleTeacher();
            foreach (Control c in headerPanel.Controls)
                c.Click += (_, _) => ToggleTeacher();

            wrapper.Resize += (_, _) =>
            {
                headerPanel.Width = wrapper.Width;
                foreach (var rp in rowPanels)
                    rp.Width = wrapper.Width;
            };

            return wrapper;
        }

        private static Panel BuildFormDetailPanel(EvaluationForm? form, List<FormResponse> responses, int width)
        {
            var outer = new Panel
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Width = width,
                Height = 20  // grown dynamically
            };

            if (form == null || !responses.Any())
            {
                outer.Controls.Add(new Label
                {
                    Text = form == null ? "Form data unavailable." : "No responses found for this teacher.",
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
                // ── Question header ─────────────────────────────────
                // Measure how tall the question text will be so it never clips
                int qTextW = innerW - 90;
                int qTextH;
                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                    qTextH = (int)Math.Ceiling(g.MeasureString(
                        $"Q{q.OrderIndex + 1}. {q.Text}",
                        new Font("Inter SemiBold", 9F, FontStyle.Bold), qTextW).Height) + 4;
                qTextH = Math.Max(qTextH, 20);

                var qHeaderLbl = new Label
                {
                    Text = $"Q{q.OrderIndex + 1}. {q.Text}",
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    AutoSize = false,
                    Size = new Size(qTextW, qTextH),
                    Location = new Point(16, y)
                };
                outer.Controls.Add(qHeaderLbl);

                var typeBadge = new Label
                {
                    Text = q.Type.ToString(),
                    Font = new Font("Inter", 7F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    BackColor = Color.FromArgb(226, 232, 240),
                    AutoSize = true,
                    Padding = new Padding(5, 1, 5, 1),
                    Location = new Point(innerW - 70, y + 2)
                };
                outer.Controls.Add(typeBadge);
                y += qTextH + 8;

                // Collect all answers for this question
                var answers = responses
                    .Select(r => r.Answers.TryGetValue(q.Id, out var a) ? a : null)
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a!)
                    .ToList();

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
                    // ── Rating: avg + filled bar ─────────────────────
                    var vals = answers
                        .Select(a => double.TryParse(a, out var v) ? (double?)v : null)
                        .Where(v => v.HasValue).Select(v => v!.Value).ToList();
                    double avg = vals.Any() ? vals.Average() : 0;
                    double maxR = q.MaxRating ?? 5;

                    int barW = Math.Min(280, innerW - 120);
                    int fillW = (int)Math.Round(barW * avg / maxR);

                    var barBg = new Panel
                    {
                        BackColor = Color.FromArgb(226, 232, 240),
                        Size = new Size(barW, 12),
                        Location = new Point(24, y + 6)
                    };
                    barBg.Controls.Add(new Panel
                    {
                        BackColor = Color.FromArgb(234, 179, 8),
                        Size = new Size(Math.Max(0, fillW), 12),
                        Location = new Point(0, 0)
                    });
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
                    // ── YesNo: percentage bar ────────────────────────
                    int yesCount = answers.Count(a => a.Equals("yes", StringComparison.OrdinalIgnoreCase) || a.Equals("true", StringComparison.OrdinalIgnoreCase));
                    int noCount = answers.Count - yesCount;
                    int total = answers.Count;
                    double yesPct = total > 0 ? yesCount * 100.0 / total : 0;
                    double noPct = 100 - yesPct;

                    int barW = Math.Min(280, innerW - 40);
                    int yesFill = (int)Math.Round(barW * yesPct / 100);

                    var barBg = new Panel
                    {
                        BackColor = Color.FromArgb(254, 226, 226),
                        Size = new Size(barW, 16),
                        Location = new Point(24, y + 2)
                    };
                    barBg.Controls.Add(new Panel
                    {
                        BackColor = Color.FromArgb(34, 197, 94),
                        Size = new Size(Math.Max(0, yesFill), 16),
                        Location = new Point(0, 0)
                    });
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
                    // ── Multiple choice: ranked options ──────────────
                    var counts = answers
                        .GroupBy(a => a.Trim(), StringComparer.OrdinalIgnoreCase)
                        .Select(g => (Option: g.Key, Count: g.Count()))
                        .OrderByDescending(x => x.Count)
                        .ToList();
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

                        var barBg = new Panel
                        {
                            BackColor = Color.FromArgb(226, 232, 240),
                            Size = new Size(barW, 12),
                            Location = new Point(170, y + 7)
                        };
                        barBg.Controls.Add(new Panel
                        {
                            BackColor = Color.FromArgb(99, 102, 241),
                            Size = new Size(Math.Max(0, fillW), 12),
                            Location = new Point(0, 0)
                        });
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
                else
                {
                    // ── Text answers: scrollable list ────────────────
                    int maxScrollH = 240;
                    int cardW = innerW - 16;

                    var textAnswerPairs = responses
                        .Select(r =>
                        {
                            r.Answers.TryGetValue(q.Id, out var a);
                            return (Resp: r, Ans: a);
                        })
                        .Where(x => !string.IsNullOrWhiteSpace(x.Ans))
                        .ToList();

                    // Pre-measure each answer height so cards don't clip
                    var cardHeights = textAnswerPairs.Select(p =>
                    {
                        int ansH;
                        using (var g = Graphics.FromHwnd(IntPtr.Zero))
                            ansH = (int)Math.Ceiling(g.MeasureString(
                                p.Ans ?? "", new Font("Inter", 9F), cardW - 20).Height) + 4;
                        return Math.Max(ansH, 16) + 26; // 26 = student label row + padding
                    }).ToList();

                    int totalContentH = cardHeights.Sum() + textAnswerPairs.Count * 6 + 8;
                    var scrollPanel = new Panel
                    {
                        BackColor = Color.FromArgb(241, 245, 249),
                        Size = new Size(innerW, Math.Min(maxScrollH, totalContentH)),
                        Location = new Point(16, y),
                        AutoScroll = true
                    };

                    int ay = 6;
                    for (int ti = 0; ti < textAnswerPairs.Count; ti++)
                    {
                        var (resp, ans) = textAnswerPairs[ti];
                        int cardH = cardHeights[ti];
                        string studentLabel = !string.IsNullOrWhiteSpace(resp.StudentName)
                            ? $"{resp.StudentName} ({resp.StudentId})"
                            : resp.StudentId;

                        var ansCard = new Panel
                        {
                            BackColor = Color.White,
                            Size = new Size(cardW, cardH),
                            Location = new Point(6, ay)
                        };

                        ansCard.Controls.Add(new Label
                        {
                            Text = studentLabel,
                            Font = new Font("Inter SemiBold", 7F, FontStyle.Bold),
                            ForeColor = Color.FromArgb(38, 166, 91),
                            AutoSize = true,
                            Location = new Point(8, 4)
                        });
                        ansCard.Controls.Add(new Label
                        {
                            Text = ans,
                            Font = new Font("Inter", 9F),
                            ForeColor = Color.FromArgb(30, 41, 59),
                            AutoSize = false,
                            Size = new Size(cardW - 20, cardH - 24),
                            Location = new Point(8, 20)
                        });

                        scrollPanel.Controls.Add(ansCard);
                        ay += cardH + 6;
                    }
                    outer.Controls.Add(scrollPanel);
                    y += scrollPanel.Height + 6;
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

            // ── Approved student comments ────────────────────────────
            var submissionIds = responses.Select(r => r.Id).ToList();
            var approvedComments = FormDataStore.GetApprovedCommentsForSubmissions(submissionIds);

            if (approvedComments.Any())
            {
                // Section header
                outer.Controls.Add(new Label
                {
                    Text = "Student Comments",
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    AutoSize = true,
                    Location = new Point(16, y)
                });
                y += 26;

                foreach (var ac in approvedComments)
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

                    // Measure comment text height
                    int commentW = innerW - 32;
                    int commentTextH;
                    using (var g = Graphics.FromHwnd(IntPtr.Zero))
                        commentTextH = (int)Math.Ceiling(g.MeasureString(
                            ac.CommentText, new Font("Inter", 9F), commentW - 20).Height) + 4;
                    commentTextH = Math.Max(commentTextH, 18);

                    int cardH = 28 + commentTextH + 10; // student label row + text + padding

                    var commentCard = new Panel
                    {
                        BackColor = Color.White,
                        Size = new Size(innerW, cardH),
                        Location = new Point(16, y)
                    };

                    // Left accent coloured by level
                    commentCard.Controls.Add(new Panel
                    {
                        BackColor = levelBg,
                        Size = new Size(4, cardH),
                        Location = new Point(0, 0)
                    });

                    string studentLabel = !string.IsNullOrWhiteSpace(ac.StudentName)
                        ? $"{ac.StudentName} ({ac.StudentId})"
                        : ac.StudentId;

                    commentCard.Controls.Add(new Label
                    {
                        Text = studentLabel,
                        Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(38, 166, 91),
                        AutoSize = true,
                        Location = new Point(12, 6)
                    });

                    var levelBadge = new Label
                    {
                        Text = ac.SystemLevel.ToString(),
                        Font = new Font("Inter", 7F),
                        ForeColor = levelFg,
                        BackColor = levelBg,
                        AutoSize = true,
                        Padding = new Padding(5, 1, 5, 1),
                        Location = new Point(commentW - 60, 5)
                    };
                    commentCard.Controls.Add(levelBadge);

                    commentCard.Controls.Add(new Label
                    {
                        Text = ac.CommentText,
                        Font = new Font("Inter", 9F),
                        ForeColor = Color.FromArgb(30, 41, 59),
                        AutoSize = false,
                        Size = new Size(commentW - 20, commentTextH),
                        Location = new Point(12, 24)
                    });

                    outer.Controls.Add(commentCard);
                    y += cardH + 6;
                }
            }

            outer.Height = y + 8;
            return outer;
        }

        private void UpdateNavButtons()
        {
            bool onDash = !showingResponses && !showingTeachers && !showingStudents && !showingComments && !showingReports;
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
            reportsBtn.BackColor = showingReports ? Color.FromArgb(38, 166, 91) : Color.Transparent;
            reportsBtn.ForeColor = showingReports ? Color.White : Color.FromArgb(203, 213, 225);
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

            // Group forms by semester/school year
            // Key: display label, ordered newest first (school year desc, then 2nd > 1st > Summer)
            var formsWithResponsesList = forms
                .Where(f => FormDataStore.GetSubmissionCount(f.Id) > 0)
                .ToList();

            if (!formsWithResponsesList.Any())
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
                return;
            }

            // Build ordered groups: newest school year first, within year: 2nd > 1st > Summer, then no-semester last
            static int SemOrder(string s) => s switch { "2nd" => 0, "1st" => 1, "Summer" => 2, _ => 99 };

            var groups = formsWithResponsesList
                .GroupBy(f =>
                    (!string.IsNullOrEmpty(f.Semester) && !string.IsNullOrEmpty(f.SchoolYear))
                        ? $"{f.Semester} Sem {f.SchoolYear}"
                        : "(No Semester Set)")
                .OrderByDescending(g => g.Key == "(No Semester Set)" ? "" : g.First().SchoolYear)
                .ThenBy(g => g.Key == "(No Semester Set)" ? 99 : SemOrder(g.First().Semester))
                .ToList();

            foreach (var group in groups)
            {
                // Section header
                var sectionHeader = new Panel
                {
                    BackColor = Color.FromArgb(241, 245, 249),
                    Size = new Size(formsListPanel.Width - 40, 40),
                    Margin = new Padding(0, 8, 0, 4)
                };
                sectionHeader.Controls.Add(new Label
                {
                    Text = group.Key,
                    Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    AutoSize = true,
                    Location = new Point(16, 10)
                });
                int groupCount = group.Sum(f => FormDataStore.GetSubmissionCount(f.Id));
                sectionHeader.Controls.Add(new Label
                {
                    Text = $"{groupCount} submission{(groupCount == 1 ? "" : "s")}",
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(sectionHeader.Width - 130, 12),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                });
                formsListPanel.Controls.Add(sectionHeader);

                foreach (var form in group.OrderByDescending(f => f.CreatedAt))
                {
                    var responses = FormDataStore.GetResponsesForForm(form.Id)
                        .OrderByDescending(r => r.SubmittedAt)
                        .ToList();
                    if (!responses.Any()) continue;
                    formsListPanel.Controls.Add(CreateAccordionSection(form, responses));
                }
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
                        TeacherStore.SaveReport(response.TeacherId, form.Id, response.AssignmentId, avgScore, 1, BuildResponseReport(form, response));
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
                Size = new Size(formsListPanel.Width - 64, 88),
                Margin = new Padding(0, 0, 0, 4),
                Cursor = Cursors.Default,
                Tag = "response-row"
            };

            var accentBar = new Panel
            {
                BackColor = Color.FromArgb(38, 166, 91),
                Size = new Size(3, 88),
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

            // Subject badge (shows subject context for per-subject evaluations)
            var subjectBadge = new Label
            {
                Text = response.SubjectDisplay,
                Font = new Font("Inter", 8F),
                ForeColor = !string.IsNullOrEmpty(response.SubjectName) ? Color.FromArgb(3, 105, 161) : Color.FromArgb(148, 163, 184),
                BackColor = !string.IsNullOrEmpty(response.SubjectName) ? Color.FromArgb(224, 242, 254) : Color.FromArgb(248, 250, 252),
                AutoSize = true,
                Padding = new Padding(6, 3, 6, 3),
                Location = new Point(58, 64)
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

            var pendingComment = FormDataStore.GetCommentForSubmission(response.Id);
            bool hasPendingComment = pendingComment != null && pendingComment.Status == CommentStatus.Pending;

            string sendBtnText;
            Color sendBtnColor;
            bool sendBtnEnabled;
            if (alreadySent)
            {
                sendBtnText = "Sent"; sendBtnColor = Color.FromArgb(148, 163, 184); sendBtnEnabled = false;
            }
            else if (hasPendingComment)
            {
                sendBtnText = "Pending"; sendBtnColor = Color.FromArgb(234, 179, 8); sendBtnEnabled = false;
            }
            else
            {
                sendBtnText = "Send"; sendBtnColor = Color.FromArgb(38, 166, 91); sendBtnEnabled = true;
            }

            var sendBtn = new Button
            {
                Text = sendBtnText,
                BackColor = sendBtnColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(68, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = sendBtnEnabled ? Cursors.Hand : Cursors.Default,
                Enabled = sendBtnEnabled
            };
            if (hasPendingComment)
                sendBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(234, 179, 8);

            // Revoke button — only visible when already sent
            var revokeBtn = new Button
            {
                Text = "Revoke",
                BackColor = Color.FromArgb(254, 226, 226),
                ForeColor = Color.FromArgb(185, 28, 28),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(62, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand,
                Visible = alreadySent
            };
            revokeBtn.MouseEnter += (_, _) => revokeBtn.BackColor = Color.FromArgb(252, 202, 202);
            revokeBtn.MouseLeave += (_, _) => revokeBtn.BackColor = Color.FromArgb(254, 226, 226);

            sendBtn.Click += (_, _) =>
            {
                SendReportToTeacher(form, response, BuildResponseReport(form, response));
                sendBtn.Text = "Sent";
                sendBtn.BackColor = Color.FromArgb(148, 163, 184);
                sendBtn.Enabled = false;
                sendBtn.Cursor = Cursors.Default;
                revokeBtn.Visible = true;
            };

            revokeBtn.Click += (_, _) =>
            {
                var confirm = MessageBox.Show(
                    $"Revoke the report sent to {response.TeacherName} for this submission?\nThe report will be removed and can be re-sent.",
                    "Revoke Report",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes) return;

                int reportId = TeacherStore.GetReportIdForSubmission(response.TeacherId, form.Id, response.Id);
                if (reportId > 0)
                    TeacherStore.DeleteReport(reportId);

                // Reset send button to sendable state
                sendBtn.Text = "Send";
                sendBtn.BackColor = Color.FromArgb(38, 166, 91);
                sendBtn.Enabled = true;
                sendBtn.Cursor = Cursors.Hand;
                revokeBtn.Visible = false;
            };

            row.Controls.Add(accentBar);
            row.Controls.Add(avatar);
            row.Controls.Add(nameLabel);
            row.Controls.Add(idLabel);
            row.Controls.Add(teacherLabel);
            row.Controls.Add(subjectBadge);
            row.Controls.Add(dateLabel);
            row.Controls.Add(ratingLabel);
            row.Controls.Add(detailsBtn);
            row.Controls.Add(sendBtn);
            row.Controls.Add(revokeBtn);

            row.Resize += (_, _) =>
            {
                accentBar.Size       = new Size(3, row.Height);
                dateLabel.Location   = new Point(row.Width - 490, 27);
                ratingLabel.Location = new Point(row.Width - 320, 27);
                detailsBtn.Location  = new Point(row.Width - 230, 22);
                sendBtn.Location     = new Point(row.Width - 148, 22);
                revokeBtn.Location   = new Point(row.Width - 74, 22);
            };

            // trigger initial layout
            int rowW = formsListPanel.Width - 64;
            dateLabel.Location   = new Point(rowW - 490, 27);
            ratingLabel.Location = new Point(rowW - 320, 27);
            detailsBtn.Location  = new Point(rowW - 230, 22);
            sendBtn.Location     = new Point(rowW - 148, 22);
            revokeBtn.Location   = new Point(rowW - 74, 22);

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
                Size = new Size(720, 650),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(248, 250, 252),
                FormBorderStyle = FormBorderStyle.Sizable,
                MaximizeBox = true,
                MinimizeBox = true
            };

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(28, 24, 28, 24),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            int panelW = 636;
            int y = 0;

            // ── Header ──────────────────────────────────────────────
            var headerPanel = new Panel
            {
                Size = new Size(panelW, 90),
                Location = new Point(28, y),
                BackColor = Color.FromArgb(240, 253, 244),
                Padding = new Padding(20)
            };
            headerPanel.Controls.Add(new Label
            {
                Text = form.Title,
                Font = new Font("Inter", 15F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, 14)
            });
            string semTag = (!string.IsNullOrEmpty(form.Semester) && !string.IsNullOrEmpty(form.SchoolYear))
                ? $"  ·  {form.Semester} Sem {form.SchoolYear}" : "";
            headerPanel.Controls.Add(new Label
            {
                Text = $"Submitted by {response.StudentName} ({response.StudentId})  ·  {response.SubmittedAt:MMM dd, yyyy  h:mm tt}{semTag}",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(20, 46)
            });
            headerPanel.Controls.Add(new Label
            {
                Text = $"Teacher: {(response.TeacherId > 0 ? response.TeacherName : (!string.IsNullOrWhiteSpace(form.TargetTeacher) ? form.TargetTeacher : "—"))}",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(20, 66)
            });
            scrollPanel.Controls.Add(headerPanel);
            y += 102;

            // ── Group questions by category ──────────────────────────
            var orderedQuestions = form.Questions.OrderBy(q => q.OrderIndex).ToList();
            var categoryGroups = orderedQuestions
                .GroupBy(q => string.IsNullOrWhiteSpace(q.Category) ? "General" : q.Category)
                .Select(g => (
                    Category: g.Key,
                    Questions: g.OrderBy(q => q.OrderIndex).ToList(),
                    FirstOrder: g.Min(q => q.OrderIndex)
                ))
                .OrderBy(g => g.FirstOrder)
                .ToList();

            foreach (var group in categoryGroups)
            {
                // Compute per-category avg rating (only answered rating questions)
                var ratingQs = group.Questions.Where(q => q.Type == QuestionType.Rating).ToList();
                var ratingVals = ratingQs
                    .Select(q => response.Answers.TryGetValue(q.Id, out var a) && double.TryParse(a, out var v) ? (double?)v : null)
                    .Where(v => v.HasValue).Select(v => v!.Value).ToList();
                double? catAvg = ratingVals.Any() ? ratingVals.Average() : null;

                // Category header
                var catHeader = new Panel
                {
                    Size = new Size(panelW, 38),
                    Location = new Point(28, y),
                    BackColor = Color.FromArgb(226, 232, 240)
                };
                catHeader.Controls.Add(new Label
                {
                    Text = group.Category,
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    AutoSize = true,
                    Location = new Point(14, 10)
                });
                if (catAvg.HasValue)
                {
                    var avgBadge = new Label
                    {
                        Text = $"★ {catAvg.Value:0.0} avg",
                        Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                        ForeColor = Color.White,
                        BackColor = Color.FromArgb(38, 166, 91),
                        AutoSize = true,
                        Padding = new Padding(8, 3, 8, 3),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };
                    catHeader.Controls.Add(avgBadge);
                    catHeader.Layout += (_, _) =>
                        avgBadge.Location = new Point(catHeader.Width - avgBadge.Width - 12, 7);
                }
                scrollPanel.Controls.Add(catHeader);
                y += 42;

                // Questions in this category
                foreach (var question in group.Questions)
                {
                    response.Answers.TryGetValue(question.Id, out var answer);
                    string formattedAnswer = FormatFullAnswer(question, answer);

                    // Measure answer height (wrap at ~520px, ~18px per line)
                    int answerLines = Math.Max(1, (int)Math.Ceiling(
                        System.Drawing.Graphics.FromHwnd(IntPtr.Zero)
                            .MeasureString(formattedAnswer, new Font("Inter", 10F), 520).Height / 18.0));
                    int answerH = Math.Max(32, answerLines * 20 + 8);
                    int qPanelH = 20 + 20 + 26 + answerH + 16; // text + type badge + answer + padding

                    var qPanel = new Panel
                    {
                        Size = new Size(panelW, qPanelH),
                        Location = new Point(28, y),
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    var qNumLabel = new Label
                    {
                        Text = $"Q{question.OrderIndex + 1}",
                        Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(38, 166, 91),
                        AutoSize = true,
                        Location = new Point(14, 14)
                    };

                    var qTextLabel = new Label
                    {
                        Text = question.Text,
                        Font = new Font("Inter", 10F),
                        ForeColor = Color.FromArgb(30, 41, 59),
                        AutoSize = false,
                        Size = new Size(panelW - 60, 20),
                        Location = new Point(46, 14)
                    };

                    var typeBadge = new Label
                    {
                        Text = question.Type.ToString(),
                        Font = new Font("Inter", 8F),
                        ForeColor = Color.FromArgb(100, 116, 139),
                        BackColor = Color.FromArgb(241, 245, 249),
                        AutoSize = true,
                        Padding = new Padding(6, 2, 6, 2),
                        Location = new Point(46, 38)
                    };

                    var answerBox = new Panel
                    {
                        Size = new Size(panelW - 56, answerH),
                        Location = new Point(46, 66),
                        BackColor = Color.FromArgb(250, 251, 252)
                    };
                    answerBox.Controls.Add(new Label
                    {
                        Text = formattedAnswer,
                        Font = new Font("Inter", 10F),
                        ForeColor = Color.FromArgb(15, 23, 42),
                        AutoSize = false,
                        Size = new Size(panelW - 72, answerH - 8),
                        Location = new Point(10, 4)
                    });

                    qPanel.Controls.Add(qNumLabel);
                    qPanel.Controls.Add(qTextLabel);
                    qPanel.Controls.Add(typeBadge);
                    qPanel.Controls.Add(answerBox);
                    scrollPanel.Controls.Add(qPanel);
                    y += qPanelH + 4;
                }

                y += 8; // gap after category
            }

            // ── Additional comment ───────────────────────────────────
            var comment = FormDataStore.GetCommentForSubmission(response.Id);
            if (comment != null)
            {
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

                var commentSectionPanel = new Panel
                {
                    Size = new Size(panelW, 116),
                    Location = new Point(28, y),
                    BackColor = Color.FromArgb(248, 250, 252),
                    BorderStyle = BorderStyle.FixedSingle
                };
                commentSectionPanel.Controls.Add(new Label
                {
                    Text = "Additional Comment",
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    AutoSize = true,
                    Location = new Point(16, 12)
                });
                commentSectionPanel.Controls.Add(new Label
                {
                    Text = comment.SystemLevel.ToString(),
                    Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                    ForeColor = levelFg,
                    BackColor = levelBg,
                    AutoSize = true,
                    Padding = new Padding(6, 2, 6, 2),
                    Location = new Point(400, 10)
                });
                commentSectionPanel.Controls.Add(new Label
                {
                    Text = comment.Status.ToString(),
                    Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                    ForeColor = statusFg,
                    BackColor = statusBg,
                    AutoSize = true,
                    Padding = new Padding(6, 2, 6, 2),
                    Location = new Point(480, 10)
                });
                commentSectionPanel.Controls.Add(new TextBox
                {
                    Text = comment.CommentText,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Inter", 10F),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    Location = new Point(16, 40),
                    Size = new Size(panelW - 32, 66)
                });
                scrollPanel.Controls.Add(commentSectionPanel);
                y += 128;
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

            var pendingCheck = FormDataStore.GetCommentForSubmission(response.Id);
            if (pendingCheck != null && pendingCheck.Status == CommentStatus.Pending)
            {
                MessageBox.Show(
                    "This submission has a comment that is still under review.\nResolve it in Comment Moderation before sending the report.",
                    "Pending Comment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
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
                TeacherStore.SaveReport(response.TeacherId, form.Id, response.AssignmentId, avgScore, 1, reportBody);
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

            // Header section - no Dock in FlowLayoutPanel
            var headerPanel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(teachersListPanel.Width - 40, 180),
                Margin = new Padding(0, 0, 0, 16),
                Padding = new Padding(24),
                Location = new Point(0, 0)
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
            var perfData = TeacherStore.GetTeacherPerformanceBySemester(teacher.TeacherID);

            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(teachersListPanel.Width - 40, 160),
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
                Location = new Point(86, 16)
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

            // Subjects/Assignments preview
            string assignmentsText = teacher.Assignments.Count > 0
                ? $"📚 {teacher.Assignments.Count} subject group(s)"
                : "⚠️ No subjects - students won't see this teacher";
            var assignmentsLabel = new Label
            {
                Text = assignmentsText,
                Font = new Font("Inter", 9F),
                ForeColor = teacher.Assignments.Count > 0 ? Color.FromArgb(71, 85, 105) : Color.FromArgb(239, 68, 68),
                AutoSize = true,
                Location = new Point(200, 74)
            };

            // ── Ratings strip (last 3 semesters) ─────────────────────
            var ratingsStrip = new Panel
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Size = new Size(card.Width - 20, 32),
                Location = new Point(16, 118),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            if (perfData.Any())
            {
                int chipX = 8;
                foreach (var entry in perfData.Take(3))
                {
                    // Short label: "2nd 25-26"
                    string syShort = entry.SchoolYear.Length >= 7
                        ? entry.SchoolYear.Substring(2, 2) + "-" + entry.SchoolYear.Substring(7, 2)
                        : entry.SchoolYear;
                    string chipText = $"{entry.Semester} {syShort}  ★ {entry.AvgScore:0.0}";

                    var chip = new Label
                    {
                        Text = chipText,
                        Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(22, 101, 52),
                        BackColor = Color.FromArgb(220, 252, 231),
                        AutoSize = true,
                        Padding = new Padding(7, 4, 7, 4),
                        Location = new Point(chipX, 4)
                    };
                    ratingsStrip.Controls.Add(chip);
                    chipX += chip.PreferredWidth + 22 + 8; // approx width + gap
                }
            }
            else
            {
                ratingsStrip.Controls.Add(new Label
                {
                    Text = "No evaluation data yet",
                    Font = new Font("Inter", 8F, FontStyle.Italic),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(8, 8)
                });
            }

            // ── Buttons ───────────────────────────────────────────────
            var perfBtn = new Button
            {
                Text = "📊 Performance",
                BackColor = Color.FromArgb(243, 244, 246),
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(118, 32),
                Location = new Point(card.Width - 340, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            perfBtn.Click += (_, _) => ShowTeacherPerformanceDialog(teacher, perfData);
            perfBtn.MouseEnter += (_, _) => perfBtn.BackColor = Color.FromArgb(229, 231, 235);
            perfBtn.MouseLeave += (_, _) => perfBtn.BackColor = Color.FromArgb(243, 244, 246);

            var manageBtn = new Button
            {
                Text = "📋 Subjects",
                BackColor = Color.FromArgb(224, 242, 254),
                ForeColor = Color.FromArgb(3, 105, 161),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(100, 32),
                Location = new Point(card.Width - 215, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            manageBtn.Click += (_, _) => ShowTeacherSubjectsDialog(teacher);
            manageBtn.MouseEnter += (_, _) => manageBtn.BackColor = Color.FromArgb(186, 230, 253);
            manageBtn.MouseLeave += (_, _) => manageBtn.BackColor = Color.FromArgb(224, 242, 254);

            var editBtn = new Button
            {
                Text = "✏️ Edit",
                BackColor = Color.FromArgb(243, 244, 246),
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(72, 32),
                Location = new Point(card.Width - 108, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            editBtn.Click += (_, _) => EditTeacher(teacher);
            editBtn.MouseEnter += (_, _) => editBtn.BackColor = Color.FromArgb(229, 231, 235);
            editBtn.MouseLeave += (_, _) => editBtn.BackColor = Color.FromArgb(243, 244, 246);

            var deleteBtn = new Button
            {
                Text = "🗑️",
                BackColor = Color.FromArgb(254, 226, 226),
                ForeColor = Color.FromArgb(185, 28, 28),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(32, 32),
                Location = new Point(card.Width - 48, 70),
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
            card.Controls.Add(assignmentsLabel);
            card.Controls.Add(ratingsStrip);
            card.Controls.Add(perfBtn);
            card.Controls.Add(manageBtn);
            card.Controls.Add(editBtn);
            card.Controls.Add(deleteBtn);

            card.Resize += (_, _) =>
            {
                accentBar.Size = new Size(4, card.Height);
                ratingsStrip.Size = new Size(card.Width - 20, 32);
                perfBtn.Location = new Point(card.Width - 340, 70);
                manageBtn.Location = new Point(card.Width - 215, 70);
                editBtn.Location = new Point(card.Width - 108, 70);
                deleteBtn.Location = new Point(card.Width - 48, 70);
            };

            return card;
        }

        private void ShowTeacherPerformanceDialog(Teacher teacher,
            List<(string Label, string Semester, string SchoolYear, decimal AvgScore, int ResponseCount)> perfData)
        {
            var dlg = new Form
            {
                Text = $"Performance History — {teacher.FullName}",
                Size = new Size(640, 520),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(248, 250, 252),
                FormBorderStyle = FormBorderStyle.Sizable,
                MaximizeBox = true,
                MinimizeBox = false
            };

            var scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(28, 20, 28, 20)
            };
            dlg.Controls.Add(scroll);

            int panelW = 560;
            int y = 0;

            // ── Header ──────────────────────────────────────────────
            var hdr = new Panel
            {
                Size = new Size(panelW, 76),
                Location = new Point(28, y),
                BackColor = Color.FromArgb(240, 253, 244)
            };
            hdr.Controls.Add(new Label
            {
                Text = teacher.FullName,
                Font = new Font("Inter", 15F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(16, 12)
            });
            hdr.Controls.Add(new Label
            {
                Text = $"{teacher.Department}  ·  {teacher.Email}",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(16, 44)
            });
            scroll.Controls.Add(hdr);
            y += 88;

            if (!perfData.Any())
            {
                scroll.Controls.Add(new Label
                {
                    Text = "No rating data available for this teacher yet.",
                    Font = new Font("Inter", 11F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(28, y + 32)
                });
                dlg.ShowDialog(this);
                return;
            }

            // ── Subject Performance Section ──────────────────────────
            var subjectData = TeacherStore.GetTeacherPerformanceBySubject(teacher.TeacherID);
            if (subjectData.Any())
            {
                scroll.Controls.Add(new Label
                {
                    Text = "Performance by Subject",
                    Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    AutoSize = true,
                    Location = new Point(28, y)
                });
                y += 28;

                const decimal maxRatingSubject = 5m;
                for (int i = 0; i < subjectData.Count && i < 5; i++) // Show top 5 subjects
                {
                    var entry = subjectData[i];
                    var row = new Panel
                    {
                        Size = new Size(panelW, 64),
                        Location = new Point(28, y),
                        BackColor = Color.White
                    };

                    row.Controls.Add(new Panel
                    {
                        BackColor = Color.FromArgb(59, 130, 246),
                        Size = new Size(4, 64),
                        Location = new Point(0, 0)
                    });

                    row.Controls.Add(new Label
                    {
                        Text = entry.SubjectLabel,
                        Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(15, 23, 42),
                        AutoSize = true,
                        Location = new Point(14, 10)
                    });

                    row.Controls.Add(new Label
                    {
                        Text = $"{entry.ResponseCount} response{(entry.ResponseCount == 1 ? "" : "s")}",
                        Font = new Font("Inter", 8F),
                        ForeColor = Color.FromArgb(100, 116, 139),
                        AutoSize = true,
                        Location = new Point(14, 34)
                    });

                    int barW = 180, barH = 10;
                    var barBg = new Panel
                    {
                        BackColor = Color.FromArgb(226, 232, 240),
                        Size = new Size(barW, barH),
                        Location = new Point(14, 50)
                    };
                    int fillW = (int)Math.Round(barW * (double)entry.AvgScore / (double)maxRatingSubject);
                    barBg.Controls.Add(new Panel
                    {
                        BackColor = Color.FromArgb(59, 130, 246),
                        Size = new Size(Math.Max(0, fillW), barH),
                        Location = new Point(0, 0)
                    });
                    row.Controls.Add(barBg);

                    var avgLabel = new Label
                    {
                        Text = $"★ {entry.AvgScore:0.00} / {maxRatingSubject:0}",
                        Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(234, 179, 8),
                        AutoSize = true,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };
                    row.Controls.Add(avgLabel);
                    row.Layout += (_, _) => avgLabel.Location = new Point(row.Width - avgLabel.Width - 80, 18);

                    scroll.Controls.Add(row);
                    y += 68;
                }
                y += 20; // Spacing before semester section
            }

            // ── Semester Performance Section ─────────────────────────
            scroll.Controls.Add(new Label
            {
                Text = "Semester Performance",
                Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(28, y)
            });
            y += 28;

            const decimal maxRating = 5m;

            for (int i = 0; i < perfData.Count; i++)
            {
                var entry = perfData[i];

                // Trend vs previous semester (next in list = older)
                string trendText = "";
                Color trendFg = Color.FromArgb(100, 116, 139);
                Color trendBg = Color.FromArgb(241, 245, 249);
                if (i + 1 < perfData.Count)
                {
                    decimal delta = entry.AvgScore - perfData[i + 1].AvgScore;
                    if (delta > 0)
                    {
                        trendText = $"▲ +{delta:0.0}";
                        trendFg = Color.FromArgb(22, 101, 52);
                        trendBg = Color.FromArgb(220, 252, 231);
                    }
                    else if (delta < 0)
                    {
                        trendText = $"▼ {delta:0.0}";
                        trendFg = Color.FromArgb(153, 27, 27);
                        trendBg = Color.FromArgb(254, 226, 226);
                    }
                    else
                    {
                        trendText = "= 0.0";
                    }
                }

                var row = new Panel
                {
                    Size = new Size(panelW, 64),
                    Location = new Point(28, y),
                    BackColor = Color.White
                };

                // Left accent
                row.Controls.Add(new Panel
                {
                    BackColor = Color.FromArgb(38, 166, 91),
                    Size = new Size(4, 64),
                    Location = new Point(0, 0)
                });

                // Semester label
                row.Controls.Add(new Label
                {
                    Text = entry.Label,
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    AutoSize = true,
                    Location = new Point(14, 10)
                });

                // Response count
                row.Controls.Add(new Label
                {
                    Text = $"{entry.ResponseCount} response{(entry.ResponseCount == 1 ? "" : "s")}",
                    Font = new Font("Inter", 8F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(14, 34)
                });

                // Rating bar background
                int barW = 180;
                int barH = 10;
                int barX = 14;
                int barY = 50;
                var barBg = new Panel
                {
                    BackColor = Color.FromArgb(226, 232, 240),
                    Size = new Size(barW, barH),
                    Location = new Point(barX, barY)
                };
                int fillW = (int)Math.Round(barW * (double)entry.AvgScore / (double)maxRating);
                barBg.Controls.Add(new Panel
                {
                    BackColor = Color.FromArgb(38, 166, 91),
                    Size = new Size(Math.Max(0, fillW), barH),
                    Location = new Point(0, 0)
                });
                row.Controls.Add(barBg);

                // Numeric avg — anchored right
                var avgLabel = new Label
                {
                    Text = $"★ {entry.AvgScore:0.00} / {maxRating:0}",
                    Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(234, 179, 8),
                    AutoSize = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                row.Controls.Add(avgLabel);
                row.Layout += (_, _) => avgLabel.Location = new Point(row.Width - avgLabel.Width - 80, 18);

                // Trend badge — anchored far right
                if (!string.IsNullOrEmpty(trendText))
                {
                    var trendBadge = new Label
                    {
                        Text = trendText,
                        Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                        ForeColor = trendFg,
                        BackColor = trendBg,
                        AutoSize = true,
                        Padding = new Padding(6, 3, 6, 3),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };
                    row.Controls.Add(trendBadge);
                    row.Layout += (_, _) => trendBadge.Location = new Point(row.Width - trendBadge.Width - 12, 20);
                }

                scroll.Controls.Add(row);
                y += 68;
            }

            dlg.ShowDialog(this);
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
                    MessageBox.Show($"Error deleting teacher: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EditTeacher(Teacher teacher)
        {
            var dialog = new Form
            {
                Text = "Edit Teacher",
                Size = new Size(520, 360),
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
            y += 56;

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
                if (ValidateTeacherEditInput(txtFirstName.Text, txtLastName.Text, txtEmail.Text))
                {
                    teacher.FirstName = txtFirstName.Text.Trim();
                    teacher.LastName = txtLastName.Text.Trim();
                    teacher.Email = txtEmail.Text.Trim();
                    teacher.Department = cmbDepartment.SelectedItem?.ToString() ?? "";

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

        private void ShowTeacherSubjectsDialog(Teacher teacher)
        {
            var dialog = new Form
            {
                Text = $"Manage Subjects - {teacher.FullName}",
                Size = new Size(550, 600),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            int y = 16;

            // Header
            var titleLabel = new Label
            {
                Text = $"Subjects for {teacher.FullName}",
                Font = new Font("Inter", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, y)
            };
            dialog.Controls.Add(titleLabel);
            y += 32;

            var subtitleLabel = new Label
            {
                Text = "Add sections where this teacher teaches. Students will only see teachers matching their section/course/year.",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                MaximumSize = new Size(500, 0),
                Location = new Point(20, y)
            };
            dialog.Controls.Add(subtitleLabel);
            y += 50;

            // Subjects container (scrollable)
            var subjectsPanel = new Panel
            {
                Location = new Point(20, y),
                Size = new Size(500, 380),
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            // Load existing assignments
            var assignments = TeacherStore.GetTeacherAssignments(teacher.TeacherID);
            int subjectY = 10;

            void RefreshSubjectsList()
            {
                // Clear existing controls
                for (int i = subjectsPanel.Controls.Count - 1; i >= 0; i--)
                {
                    subjectsPanel.Controls.RemoveAt(i);
                }
                subjectY = 10;

                // Refresh from database
                assignments = TeacherStore.GetTeacherAssignments(teacher.TeacherID);

                if (assignments.Count == 0)
                {
                    var emptyLabel = new Label
                    {
                        Text = "No subjects yet. Click 'Add Subject Group' below.",
                        Font = new Font("Inter", 10F, FontStyle.Italic),
                        ForeColor = Color.FromArgb(148, 163, 184),
                        AutoSize = true,
                        Location = new Point(20, subjectY)
                    };
                    subjectsPanel.Controls.Add(emptyLabel);
                    subjectY += 40;
                }
                else
                {
                    foreach (var assignment in assignments)
                    {
                        CreateSubjectCard(subjectsPanel, assignment, ref subjectY, RefreshSubjectsList);
                    }
                }

                subjectY += 10;
            }

            void CreateSubjectCard(Panel parent, TeacherAssignment assignment, ref int cardY, Action onDelete)
            {
                var card = new Panel
                {
                    Location = new Point(10, cardY),
                    Size = new Size(460, 110),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Section info
                var sectionInfo = new Label
                {
                    Text = $"📍 Section: {assignment.Section ?? "All"} | Course: {assignment.Course ?? "All"} | Year: {assignment.YearLevel ?? "All"}",
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    AutoSize = true,
                    Location = new Point(12, 10)
                };
                card.Controls.Add(sectionInfo);

                // Subjects
                var subjectsLabel = new Label
                {
                    Text = $"📚 Subjects: {assignment.SubjectsDisplay}",
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = true,
                    MaximumSize = new Size(360, 0),
                    Location = new Point(12, 36)
                };
                card.Controls.Add(subjectsLabel);

                // Edit button
                var editBtn = new Button
                {
                    Text = "✏️",
                    BackColor = Color.FromArgb(224, 242, 254),
                    ForeColor = Color.FromArgb(3, 105, 161),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    Size = new Size(32, 28),
                    Location = new Point(380, 10),
                    Cursor = Cursors.Hand
                };
                editBtn.Click += (_, _) => ShowEditSubjectDialog(teacher, assignment, onDelete);
                card.Controls.Add(editBtn);

                // Delete button
                var deleteBtn = new Button
                {
                    Text = "🗑️",
                    BackColor = Color.FromArgb(254, 226, 226),
                    ForeColor = Color.FromArgb(185, 28, 28),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    Size = new Size(32, 28),
                    Location = new Point(420, 10),
                    Cursor = Cursors.Hand
                };
                deleteBtn.Click += (_, _) =>
                {
                    var result = MessageBox.Show("Delete this subject group?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        TeacherStore.DeleteTeacherAssignment(assignment.AssignmentID);
                        onDelete?.Invoke();
                    }
                };
                card.Controls.Add(deleteBtn);

                parent.Controls.Add(card);
                cardY += 120;
            }

            RefreshSubjectsList();
            dialog.Controls.Add(subjectsPanel);

            // Add New Subject Group button
            var addSubjectBtn = new Button
            {
                Text = "+ Add Subject Group",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(160, 36),
                Location = new Point(20, y + 390)
            };
            addSubjectBtn.Click += (_, _) => ShowAddSubjectDialog(teacher, RefreshSubjectsList);
            dialog.Controls.Add(addSubjectBtn);

            // Close button
            var closeBtn = new Button
            {
                Text = "Close",
                BackColor = Color.FromArgb(243, 244, 246),
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(90, 36),
                Location = new Point(430, y + 390)
            };
            closeBtn.Click += (_, _) => dialog.Close();
            dialog.Controls.Add(closeBtn);

            dialog.ShowDialog(this);
            LoadTeachersView(); // Refresh to show updated count
        }

        private void ShowAddSubjectDialog(Teacher teacher, Action onSave)
        {
            var dialog = new Form
            {
                Text = "Add New Subject Group",
                Size = new Size(480, 400),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                AutoScroll = true
            };

            var scrollPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(464, 360),
                BackColor = Color.White,
                AutoScroll = true
            };
            dialog.Controls.Add(scrollPanel);

            int y = 20;

            // Section
            var sectionLabel = new Label
            {
                Text = "Section (e.g., A, B, C or leave empty for all)",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            scrollPanel.Controls.Add(sectionLabel);
            y += 24;

            var sectionInput = new TextBox
            {
                Location = new Point(20, y),
                Size = new Size(200, 26),
                Font = new Font("Inter", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            scrollPanel.Controls.Add(sectionInput);
            y += 44;

            // Course (using Department as default)
            var courseLabel = new Label
            {
                Text = "Course *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            scrollPanel.Controls.Add(courseLabel);
            y += 24;

            var courseCombo = new ComboBox
            {
                Location = new Point(20, y),
                Size = new Size(200, 26),
                Font = new Font("Inter", 10F),
                DropDownStyle = ComboBoxStyle.DropDown,
                Text = teacher.Department ?? "BSIT"
            };
            courseCombo.Items.AddRange(new[] { "BSIT", "BSCS", "BSCE", "BSEE", "BSME", "BSN", "BSA", "BSBA", "Education", "Science", "Engineering", "Other" });
            scrollPanel.Controls.Add(courseCombo);
            y += 44;

            // Year Level - Limited to 1-4
            var yearLabel = new Label
            {
                Text = "Year Level (1-4 or leave empty for all)",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            scrollPanel.Controls.Add(yearLabel);
            y += 24;

            var yearCombo = new ComboBox
            {
                Location = new Point(20, y),
                Size = new Size(200, 26),
                Font = new Font("Inter", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            yearCombo.Items.Add(""); // Empty for "all"
            yearCombo.Items.Add("1");
            yearCombo.Items.Add("2");
            yearCombo.Items.Add("3");
            yearCombo.Items.Add("4");
            yearCombo.SelectedIndex = 0;
            scrollPanel.Controls.Add(yearCombo);
            y += 44;

            // Subjects
            var subjectsLabel = new Label
            {
                Text = "Subjects (comma-separated) *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            scrollPanel.Controls.Add(subjectsLabel);
            y += 24;

            var subjectsInput = new TextBox
            {
                Location = new Point(20, y),
                Size = new Size(420, 26),
                Font = new Font("Inter", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252),
                PlaceholderText = "e.g. Math, Physics, Programming"
            };
            scrollPanel.Controls.Add(subjectsInput);
            y += 56;

            // Set scroll panel height to fit content
            scrollPanel.Height = Math.Min(360, y + 80);

            // Buttons - positioned at bottom of dialog
            var cancelBtn = new Button
            {
                Text = "Cancel",
                BackColor = Color.FromArgb(243, 244, 246),
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(90, 36),
                Location = new Point(260, scrollPanel.Bottom + 10)
            };
            cancelBtn.Click += (_, _) => dialog.Close();
            dialog.Controls.Add(cancelBtn);

            var saveBtn = new Button
            {
                Text = "Add",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(90, 36),
                Location = new Point(360, scrollPanel.Bottom + 10)
            };
            saveBtn.Click += (_, _) =>
            {
                string course = courseCombo.Text.Trim();
                string subjectsText = subjectsInput.Text.Trim();

                if (string.IsNullOrWhiteSpace(course))
                {
                    MessageBox.Show("Course is required.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(subjectsText))
                {
                    MessageBox.Show("Please enter at least one subject.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var assignment = new TeacherAssignment
                {
                    Section = sectionInput.Text.Trim(),
                    Course = course,
                    YearLevel = yearCombo.SelectedItem?.ToString() ?? "",
                    Subjects = subjectsText.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList()
                };

                TeacherStore.AddTeacherAssignment(teacher.TeacherID, assignment);
                onSave?.Invoke();
                dialog.Close();
            };
            dialog.Controls.Add(saveBtn);

            dialog.ShowDialog(this);
        }

        private void ShowEditSubjectDialog(Teacher teacher, TeacherAssignment assignment, Action onSave)
        {
            var dialog = new Form
            {
                Text = "Edit Subject Group",
                Size = new Size(480, 400),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                AutoScroll = true
            };

            var scrollPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(464, 360),
                BackColor = Color.White,
                AutoScroll = true
            };
            dialog.Controls.Add(scrollPanel);

            int y = 20;

            // Section
            var sectionLabel = new Label
            {
                Text = "Section (e.g., A, B, C or leave empty for all)",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            scrollPanel.Controls.Add(sectionLabel);
            y += 24;

            var sectionInput = new TextBox
            {
                Text = assignment.Section,
                Location = new Point(20, y),
                Size = new Size(200, 26),
                Font = new Font("Inter", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            scrollPanel.Controls.Add(sectionInput);
            y += 44;

            // Course
            var courseLabel = new Label
            {
                Text = "Course *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            scrollPanel.Controls.Add(courseLabel);
            y += 24;

            var courseCombo = new ComboBox
            {
                Location = new Point(20, y),
                Size = new Size(200, 26),
                Font = new Font("Inter", 10F),
                DropDownStyle = ComboBoxStyle.DropDown,
                Text = assignment.Course ?? teacher.Department ?? "BSIT"
            };
            courseCombo.Items.AddRange(new[] { "BSIT", "BSCS", "BSCE", "BSEE", "BSME", "BSN", "BSA", "BSBA", "Education", "Science", "Engineering", "Other" });
            scrollPanel.Controls.Add(courseCombo);
            y += 44;

            // Year Level - Limited to 1-4
            var yearLabel = new Label
            {
                Text = "Year Level (1-4 or leave empty for all)",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            scrollPanel.Controls.Add(yearLabel);
            y += 24;

            var yearCombo = new ComboBox
            {
                Location = new Point(20, y),
                Size = new Size(200, 26),
                Font = new Font("Inter", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            yearCombo.Items.Add(""); // Empty for "all"
            yearCombo.Items.Add("1");
            yearCombo.Items.Add("2");
            yearCombo.Items.Add("3");
            yearCombo.Items.Add("4");
            // Select current value
            int selectedIndex = 0;
            for (int i = 0; i < yearCombo.Items.Count; i++)
            {
                if (yearCombo.Items[i]?.ToString() == assignment.YearLevel)
                {
                    selectedIndex = i;
                    break;
                }
            }
            yearCombo.SelectedIndex = selectedIndex;
            scrollPanel.Controls.Add(yearCombo);
            y += 44;

            // Subjects
            var subjectsLabel = new Label
            {
                Text = "Subjects (comma-separated) *",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(20, y)
            };
            scrollPanel.Controls.Add(subjectsLabel);
            y += 24;

            var subjectsInput = new TextBox
            {
                Text = string.Join(", ", assignment.Subjects),
                Location = new Point(20, y),
                Size = new Size(420, 26),
                Font = new Font("Inter", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252),
                PlaceholderText = "e.g. Math, Physics, Programming"
            };
            scrollPanel.Controls.Add(subjectsInput);
            y += 56;

            // Set scroll panel height
            scrollPanel.Height = Math.Min(360, y + 80);

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
                Location = new Point(260, scrollPanel.Bottom + 10)
            };
            cancelBtn.Click += (_, _) => dialog.Close();
            dialog.Controls.Add(cancelBtn);

            var saveBtn = new Button
            {
                Text = "Save",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(90, 36),
                Location = new Point(360, scrollPanel.Bottom + 10)
            };
            saveBtn.Click += (_, _) =>
            {
                string course = courseCombo.Text.Trim();
                string subjectsText = subjectsInput.Text.Trim();

                if (string.IsNullOrWhiteSpace(course))
                {
                    MessageBox.Show("Course is required.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(subjectsText))
                {
                    MessageBox.Show("Please enter at least one subject.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Delete old and create new (simple approach)
                TeacherStore.DeleteTeacherAssignment(assignment.AssignmentID);
                
                var updatedAssignment = new TeacherAssignment
                {
                    Section = sectionInput.Text.Trim(),
                    Course = course,
                    YearLevel = yearCombo.SelectedItem?.ToString() ?? "",
                    Subjects = subjectsText.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList()
                };

                TeacherStore.AddTeacherAssignment(teacher.TeacherID, updatedAssignment);
                onSave?.Invoke();
                dialog.Close();
            };
            dialog.Controls.Add(saveBtn);

            dialog.ShowDialog(this);
        }

        private bool ValidateTeacherEditInput(string firstName, string lastName, string email)
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

            return true;
        }

        private void ShowAddTeacherForm()
        {
            // Show add teacher form panel inline
            var dialog = new Form
            {
                Text = "Add New Teacher",
                Size = new Size(520, 380),
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

            // Note: Assignments (Section/Year/Subjects) are added after teacher creation
            var noteLabel = new Label
            {
                Text = "💡 You can add sections and subjects after creating the teacher",
                Font = new Font("Inter", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(20, y)
            };
            dialog.Controls.Add(noteLabel);
            y += 56;

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

            if (TeacherStore.EmailExists(email))
            {
                MessageBox.Show("A teacher with this email already exists.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var teacher = new Teacher
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Department = department
            };

            try
            {
                TeacherStore.AddTeacher(teacher, password);

                MessageBox.Show($"Teacher '{teacher.FullName}' added successfully!\n\nYou can now add sections and subjects by clicking 'Manage Assignments' on the teacher card.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear inputs
                firstNameInput.Clear();
                lastNameInput.Clear();
                emailInput.Clear();
                passwordInput.Clear();
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
            semesterFilter.Visible = false;
            teachersListPanel.Visible = true;
            commentsListPanel.Visible = false;
            LoadStudentsView();
        }

        private void LoadStudentsView()
        {
            teachersListPanel.Controls.Clear();
            teachersListPanel.SuspendLayout();

            try
            {
                // Load students from cache
                _allStudents = StudentStore.GetAllStudents(useCache: true);
                _filteredStudents = _allStudents;
                _currentPage = 1;

                // Header section - FlowLayoutPanel doesn't support Dock properly
                var headerPanel = new Panel
                {
                    BackColor = Color.White,
                    Size = new Size(teachersListPanel.Width - 40, 180),
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

                _studentCountLabel.Text = $"{_allStudents.Count} Total Students";
                _studentCountLabel.Font = new Font("Inter", 10F);
                _studentCountLabel.ForeColor = Color.FromArgb(100, 116, 139);
                _studentCountLabel.AutoSize = true;
                _studentCountLabel.Location = new Point(24, 50);

                // Search box
                _studentSearchBox.Text = "Search by name, ID, course, or section...";
                _studentSearchBox.Font = new Font("Inter", 10F);
                _studentSearchBox.Size = new Size(320, 28);
                _studentSearchBox.Location = new Point(24, 80);
                _studentSearchBox.BorderStyle = BorderStyle.FixedSingle;
                _studentSearchBox.BackColor = Color.FromArgb(248, 250, 252);
                _studentSearchBox.ForeColor = Color.FromArgb(148, 163, 184);
                _studentSearchBox.GotFocus += (_, _) =>
                {
                    if (_studentSearchBox.Text == "Search by name, ID, course, or section...")
                    {
                        _studentSearchBox.Text = "";
                        _studentSearchBox.ForeColor = Color.FromArgb(15, 23, 42);
                    }
                };
                _studentSearchBox.LostFocus += (_, _) =>
                {
                    if (string.IsNullOrWhiteSpace(_studentSearchBox.Text))
                    {
                        _studentSearchBox.Text = "Search by name, ID, course, or section...";
                        _studentSearchBox.ForeColor = Color.FromArgb(148, 163, 184);
                    }
                };
                _studentSearchBox.TextChanged += (_, _) => OnStudentSearchTextChanged();

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
                    Location = new Point(headerPanel.Width - 184, 20),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Cursor = Cursors.Hand
                };
                addBtn.Click += (_, _) => AddStudent();

                // Sync Student Registry button
                var syncBtn = new Button
                {
                    Text = "⟳ Sync Registry",
                    BackColor = Color.FromArgb(59, 130, 246),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    Size = new Size(148, 40),
                    Location = new Point(headerPanel.Width - 348, 20),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Cursor = Cursors.Hand
                };
                syncBtn.Click += async (_, _) =>
                {
                    syncBtn.Enabled = false;
                    syncBtn.Text = "Syncing…";
                    try
                    {
                        var (ins, upd) = await StudentStore.SyncFromRegistry();
                        MessageBox.Show(
                            $"Sync complete.\n\n  {ins} student{(ins == 1 ? "" : "s")} inserted\n  {upd} student{(upd == 1 ? "" : "s")} updated",
                            "Registry Sync",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        LoadStudentsView();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Sync failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        syncBtn.Enabled = true;
                        syncBtn.Text = "⟳ Sync Registry";
                    }
                };

                headerPanel.Controls.Add(titleLabel);
                headerPanel.Controls.Add(_studentCountLabel);
                headerPanel.Controls.Add(_studentSearchBox);
                headerPanel.Controls.Add(syncBtn);
                headerPanel.Controls.Add(addBtn);
                headerPanel.Resize += (_, _) =>
                {
                    addBtn.Location = new Point(headerPanel.Width - 184, 20);
                    syncBtn.Location = new Point(headerPanel.Width - 348, 20);
                };

                teachersListPanel.Controls.Add(headerPanel);

                // Handle parent panel resize
                teachersListPanel.Resize += (_, _) =>
                {
                    if (_studentListContainer != null)
                    {
                        var newHeight = Math.Max(300, teachersListPanel.Height - 196 - 60 - 40);
                        _studentListContainer.Size = new Size(teachersListPanel.Width - 40, newHeight);
                    }
                };

                if (_allStudents.Count == 0)
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

                var headerHeight = 180 + 16; // header panel height + margin
                var footerHeight = 60; // pagination controls height
                var availableHeight = Math.Max(300, teachersListPanel.Height - headerHeight - footerHeight - 40);
                _studentListContainer.BackColor = Color.White;
                _studentListContainer.Size = new Size(teachersListPanel.Width - 40, availableHeight);
                _studentListContainer.AutoScroll = false; // No scroll, use pagination
                _studentListContainer.Padding = new Padding(0, 12, 0, 12);
                _studentListContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                teachersListPanel.Controls.Add(_studentListContainer);

                // Pagination controls panel
                var paginationPanel = new Panel
                {
                    BackColor = Color.White,
                    Size = new Size(teachersListPanel.Width - 40, 50),
                    Margin = new Padding(0, 8, 0, 0)
                };

                _prevPageBtn.Text = "← Previous";
                _prevPageBtn.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
                _prevPageBtn.Size = new Size(110, 36);
                _prevPageBtn.Location = new Point(24, 8);
                _prevPageBtn.BackColor = Color.FromArgb(38, 166, 91); // Green color
                _prevPageBtn.ForeColor = Color.White;
                _prevPageBtn.FlatStyle = FlatStyle.Flat;
                _prevPageBtn.FlatAppearance.BorderSize = 0;
                _prevPageBtn.Click += (_, _) => ChangePage(-1);

                _pageInfoLabel.Text = "Page 1 of 1";
                _pageInfoLabel.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
                _pageInfoLabel.ForeColor = Color.FromArgb(71, 85, 105);
                _pageInfoLabel.AutoSize = true;
                _pageInfoLabel.Location = new Point(160, 16);

                _nextPageBtn.Text = "Next →";
                _nextPageBtn.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
                _nextPageBtn.Size = new Size(110, 36);
                _nextPageBtn.Location = new Point(380, 8);
                _nextPageBtn.BackColor = Color.FromArgb(38, 166, 91); // Green color
                _nextPageBtn.ForeColor = Color.White;
                _nextPageBtn.FlatStyle = FlatStyle.Flat;
                _nextPageBtn.FlatAppearance.BorderSize = 0;
                _nextPageBtn.Click += (_, _) => ChangePage(1);

                paginationPanel.Controls.Add(_prevPageBtn);
                paginationPanel.Controls.Add(_pageInfoLabel);
                paginationPanel.Controls.Add(_nextPageBtn);
                paginationPanel.Resize += (_, _) =>
                {
                    _nextPageBtn.Location = new Point(paginationPanel.Width - 124, 8);
                    _pageInfoLabel.Location = new Point((paginationPanel.Width - _pageInfoLabel.Width) / 2, 16);
                };

                teachersListPanel.Controls.Add(paginationPanel);

                // Handle parent panel resize
                teachersListPanel.Resize += (_, _) =>
                {
                    if (_studentListContainer != null)
                    {
                        var newHeight = Math.Max(300, teachersListPanel.Height - 196 - 60 - 40);
                        _studentListContainer.Size = new Size(teachersListPanel.Width - 40, newHeight);
                        paginationPanel.Width = teachersListPanel.Width - 40;
                        RenderCurrentPage();
                    }
                };

                // Initial render
                _currentPage = 1;
                RenderCurrentPage();
            }
            finally
            {
                teachersListPanel.ResumeLayout(true);
            }
        }

        private void OnStudentSearchTextChanged()
        {
            // Cancel existing timer
            _searchDebounceTimer?.Stop();
            _searchDebounceTimer?.Dispose();

            // Create new debounce timer (300ms delay)
            _searchDebounceTimer = new System.Windows.Forms.Timer { Interval = 300 };
            _searchDebounceTimer.Tick += (_, _) =>
            {
                _searchDebounceTimer.Stop();
                _searchDebounceTimer.Dispose();
                _searchDebounceTimer = null;

                // Perform search on UI thread
                if (InvokeRequired)
                {
                    Invoke(new Action(PerformStudentSearch));
                }
                else
                {
                    PerformStudentSearch();
                }
            };
            _searchDebounceTimer.Start();
        }

        private void PerformStudentSearch()
        {
            var searchText = _studentSearchBox.Text;
            if (searchText == "Search by name, ID, course, or section...")
                searchText = "";

            _filteredStudents = string.IsNullOrWhiteSpace(searchText)
                ? _allStudents
                : StudentStore.SearchStudents(searchText);

            _currentPage = 1; // Reset to first page on search
            _studentCountLabel.Text = $"{_filteredStudents?.Count ?? 0} Students Found";

            // Clear and re-render
            RenderCurrentPage();
        }

        private void RenderCurrentPage()
        {
            if (_filteredStudents == null || _filteredStudents.Count == 0)
            {
                _pageInfoLabel.Text = "No students";
                _prevPageBtn.Enabled = false;
                _nextPageBtn.Enabled = false;
                return;
            }

            // Calculate page info
            var totalPages = (int)Math.Ceiling((double)_filteredStudents.Count / _studentsPerPage);
            if (_currentPage < 1) _currentPage = 1;
            if (_currentPage > totalPages) _currentPage = totalPages;

            var startIndex = (_currentPage - 1) * _studentsPerPage;
            var endIndex = Math.Min(startIndex + _studentsPerPage, _filteredStudents.Count);

            // Update pagination controls
            _pageInfoLabel.Text = $"Page {_currentPage} of {totalPages} ({_filteredStudents.Count} total)";
            _prevPageBtn.Enabled = _currentPage > 1;
            _nextPageBtn.Enabled = _currentPage < totalPages;

            // Suspend layout during update
            _studentListContainer.SuspendLayout();

            try
            {
                // Clear existing cards
                _studentListContainer.Controls.Clear();

                // Add cards for current page
                for (int i = startIndex; i < endIndex; i++)
                {
                    var student = _filteredStudents[i];
                    var card = CreateStudentCard(student, _studentListContainer.Width - 24);
                    card.Location = new Point(0, (i - startIndex) * 142); // 130 + 12 margin
                    card.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    _studentListContainer.Controls.Add(card);
                }
            }
            finally
            {
                _studentListContainer.ResumeLayout(true);
            }
        }

        private void ChangePage(int direction)
        {
            _currentPage += direction;
            RenderCurrentPage();
        }

        private Panel CreateStudentCard(Student student, int cardWidth = 0)
        {
            var width = cardWidth > 0 ? cardWidth : teachersListPanel.Width - 40;
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(width, 130),
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
