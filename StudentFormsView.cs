using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class StudentFormsView : Form
    {
        private void InitializeComponent() { }

        private readonly FlowLayoutPanel formsListPanel = new();
        private readonly Label statusLabel = new();
        private readonly Panel contentPanel = new();
        private readonly ComboBox subjectDropdown = new();
        private readonly int? filterTeacherId;
        private readonly string? filterTeacherName;
        private readonly int? filterAssignmentId;
        private List<TeacherAssignment> teacherAssignments = new();

        public StudentFormsView()
        {
            InitializeComponent();
            ConfigureStudentFormsView();
            LoadAvailableForms();
            FormDataStore.FormsUpdated += OnFormsUpdated;
            FormDataStore.ResponsesUpdated += OnFormsUpdated;
            FormClosed += (_, _) =>
            {
                FormDataStore.FormsUpdated -= OnFormsUpdated;
                FormDataStore.ResponsesUpdated -= OnFormsUpdated;
            };
        }

        public StudentFormsView(int teacherId, string teacherName, int? assignmentId = null)
        {
            filterTeacherId = teacherId;
            filterTeacherName = teacherName;
            filterAssignmentId = assignmentId;
            InitializeComponent();
            ConfigureStudentFormsView();
            LoadAvailableForms();
            FormDataStore.FormsUpdated += OnFormsUpdated;
            FormDataStore.ResponsesUpdated += OnFormsUpdated;
            FormClosed += (_, _) =>
            {
                FormDataStore.FormsUpdated -= OnFormsUpdated;
                FormDataStore.ResponsesUpdated -= OnFormsUpdated;
            };
        }

        private void ConfigureStudentFormsView()
        {
            string title = filterTeacherId.HasValue ? $"Forms for {filterTeacherName}" : "Available Evaluation Forms";
            string subtitle = filterTeacherId.HasValue ? $"Complete evaluations for {filterTeacherName}" : "Complete the following evaluations for your teachers";

            Text = title;
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 251);
            Size = new Size(900, 700);

            var header = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(32, 24, 32, 16)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Inter", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(32, 20)
            };

            var subtitleLabel = new Label
            {
                Text = subtitle,
                Font = new Font("Inter", 11F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(32, 50),
                MaximumSize = new Size(header.Width - 200, 0)
            };

            var backBtn = new Button
            {
                Text = "Back to Home",
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(120, 36),
                Location = new Point(Width - 152, 22),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            backBtn.Click += (_, _) => Close();

            // Subject dropdown for per-subject evaluation (shown when viewing specific teacher)
            if (filterTeacherId.HasValue)
            {
                LoadTeacherAssignments();
                
                subjectDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
                subjectDropdown.Font = new Font("Inter", 10F);
                subjectDropdown.Size = new Size(400, 28);
                subjectDropdown.Location = new Point(32, 24);
                subjectDropdown.FlatStyle = FlatStyle.Flat;
                subjectDropdown.BackColor = Color.FromArgb(248, 250, 252);
                subjectDropdown.SelectedIndexChanged += (_, _) => 
                {
                    if (subjectDropdown.SelectedItem is TeacherAssignment assignment)
                    {
                        // Update the filter assignment ID and reload forms
                        var field = typeof(StudentFormsView).GetField("filterAssignmentId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        field?.SetValue(this, assignment.AssignmentID);
                        LoadAvailableForms();
                    }
                };
                
                // Add to content panel instead of header
                contentPanel.Controls.Add(subjectDropdown);
                
                // Move status label down to accommodate dropdown
                statusLabel.Location = new Point(32, 64);
                
                // Move forms list down to accommodate both dropdown and status
                formsListPanel.Location = new Point(32, 100);
                formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 120);
            }

            header.Controls.Add(titleLabel);
            header.Controls.Add(subtitleLabel);
            header.Controls.Add(backBtn);

            Action updateHeaderLayout = () =>
            {
                subtitleLabel.MaximumSize = new Size(header.Width - 200, 0);
                subtitleLabel.Location = new Point(32, titleLabel.Bottom + 8);
                backBtn.Location = new Point(header.Width - backBtn.Width - 32, 22);

                int requiredHeight = subtitleLabel.Bottom + 24;
                header.Height = Math.Max(80, requiredHeight);
            };

            header.Resize += (_, _) => updateHeaderLayout();
            updateHeaderLayout();

            contentPanel.BackColor = Color.FromArgb(245, 247, 251);
            contentPanel.Location = new Point(0, header.Height);
            contentPanel.Size = new Size(Width, Height - header.Height);
            contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            contentPanel.AutoScroll = true;

            header.SizeChanged += (_, _) =>
            {
                contentPanel.Location = new Point(0, header.Height);
                contentPanel.Size = new Size(Width, Height - header.Height);
            };

            statusLabel.Font = new Font("Inter", 12F);
            statusLabel.ForeColor = Color.FromArgb(148, 163, 184);
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(32, 32);
            statusLabel.Text = "Loading forms...";

            formsListPanel.FlowDirection = FlowDirection.TopDown;
            formsListPanel.WrapContents = false;
            formsListPanel.AutoScroll = true;
            formsListPanel.BackColor = Color.Transparent;
            formsListPanel.Location = new Point(32, 80);
            formsListPanel.Size = new Size(contentPanel.Width - 64, contentPanel.Height - 100);
            formsListPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            contentPanel.Controls.Add(statusLabel);
            contentPanel.Controls.Add(formsListPanel);

            Controls.Add(header);
            Controls.Add(contentPanel);
        }

        private void LoadAvailableForms()
        {
            formsListPanel.Controls.Clear();

            var department = ProfileStore.Meta.Replace("Student ", "").Trim();
            var forms = FormDataStore.GetFormsForStudent(department);
            
            // If viewing a specific teacher, also include forms targeted at that teacher
            if (filterTeacherId.HasValue)
            {
                var teacherForms = FormDataStore.GetFormsForTeacher(filterTeacherId.Value);
                forms = forms.Union(teacherForms).ToList();
            }
            
            var studentId = SessionStore.UserId;

            if (string.IsNullOrEmpty(studentId))
            {
                studentId = ProfileStore.StudentId;
            }

            int tid = filterTeacherId ?? 0;
            int? aid = filterAssignmentId;
            var availableForms = forms.Where(f => !FormDataStore.HasStudentSubmitted(f.Id, studentId, tid, aid)).ToList();
            var completedForms = forms.Where(f => FormDataStore.HasStudentSubmitted(f.Id, studentId, tid, aid)).ToList();
            var expiredCount = availableForms.Count(f => f.DueDate.HasValue && f.DueDate.Value < DateTime.Now);

            if (!availableForms.Any() && !completedForms.Any())
            {
                statusLabel.Text = "No evaluation forms available at this time.";
                statusLabel.ForeColor = Color.FromArgb(148, 163, 184);
                return;
            }

            if (expiredCount > 0)
                statusLabel.Text = $"{availableForms.Count - expiredCount} pending, {expiredCount} expired, {completedForms.Count} completed";
            else
                statusLabel.Text = $"{availableForms.Count} pending, {completedForms.Count} completed";
            statusLabel.ForeColor = Color.FromArgb(100, 116, 139);

            if (availableForms.Any())
            {
                var pendingHeader = new Label
                {
                    Text = "Pending Evaluations",
                    Font = new Font("Inter", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    AutoSize = true,
                    Location = new Point(0, 0),
                    Margin = new Padding(0, 0, 0, 16)
                };
                formsListPanel.Controls.Add(pendingHeader);

                foreach (var form in availableForms)
                {
                    var card = CreateFormCard(form, false);
                    formsListPanel.Controls.Add(card);
                }
            }

            if (completedForms.Any())
            {
                int yOffset = availableForms.Any() ? 32 : 0;
                var completedHeader = new Label
                {
                    Text = "Completed Evaluations",
                    Font = new Font("Inter", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(0, 0),
                    Margin = new Padding(0, yOffset, 0, 16)
                };
                formsListPanel.Controls.Add(completedHeader);

                foreach (var form in completedForms)
                {
                    var card = CreateFormCard(form, true);
                    formsListPanel.Controls.Add(card);
                }
            }
        }

        private Panel CreateFormCard(EvaluationForm form, bool isCompleted)
        {
            bool isExpired = form.DueDate.HasValue && form.DueDate.Value < DateTime.Now;
            
            var card = new Panel
            {
                BackColor = isCompleted ? Color.FromArgb(248, 250, 252) : (isExpired ? Color.FromArgb(254, 242, 242) : Color.White),
                Size = new Size(formsListPanel.Width - 40, 160),
                Margin = new Padding(0, 0, 0, 16),
                Padding = new Padding(24)
            };

            var statusColor = isCompleted ? Color.FromArgb(148, 163, 184) : (isExpired ? Color.FromArgb(239, 68, 68) : Color.FromArgb(38, 166, 91));
            var statusText = isCompleted ? "Completed" : (isExpired ? "Expired" : "Pending");

            var title = new Label
            {
                Text = form.Title,
                Font = new Font("Inter", 15F, FontStyle.Bold),
                ForeColor = isCompleted ? Color.FromArgb(100, 116, 139) : Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            var description = new Label
            {
                Text = string.IsNullOrEmpty(form.Description) ? "No description provided" : form.Description,
                Font = new Font("Inter", 10F),
                ForeColor = isCompleted ? Color.FromArgb(148, 163, 184) : Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(24, 50),
                MaximumSize = new Size(card.Width - 200, 0)
            };

            var meta = new Label
            {
                Text = $"{form.Questions.Count} questions",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(24, 85)
            };

            if (form.DueDate.HasValue)
            {
                var dueLabel = new Label
                {
                    Text = isExpired ? $"Expired: {form.DueDate.Value:MMM dd, yyyy}" : $"Due: {form.DueDate.Value:MMM dd, yyyy}",
                    Font = new Font("Inter", 9F),
                    ForeColor = isExpired
                        ? Color.FromArgb(239, 68, 68)
                        : (form.DueDate.Value < DateTime.Now.AddDays(3)
                            ? Color.FromArgb(239, 68, 68)
                            : Color.FromArgb(148, 163, 184)),
                    AutoSize = true,
                    Location = new Point(120, 85)
                };
                card.Controls.Add(dueLabel);
            }

            var statusBadge = new Label
            {
                Text = statusText,
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = statusColor,
                AutoSize = true,
                Padding = new Padding(8, 4, 8, 4),
                Location = new Point(card.Width - 100, 24)
            };

            card.Controls.Add(title);
            card.Controls.Add(description);
            card.Controls.Add(meta);
            card.Controls.Add(statusBadge);

            if (!isCompleted && !isExpired)
            {
                var startBtn = new Button
                {
                    Text = "Start Evaluation",
                    BackColor = Color.FromArgb(38, 166, 91),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    Size = new Size(140, 40),
                    Location = new Point(card.Width - 164, 100)
                };
                startBtn.Click += (_, _) => OpenFormViewer(form);

                card.Controls.Add(startBtn);
            }
            else if (isExpired)
            {
                var expiredLabel = new Label
                {
                    Text = "Past Due",
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(239, 68, 68),
                    AutoSize = true,
                    Location = new Point(card.Width - 80, 108)
                };
                card.Controls.Add(expiredLabel);
            }
            else
            {
                var submittedLabel = new Label
                {
                    Text = "Submitted",
                    Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(card.Width - 90, 108)
                };
                card.Controls.Add(submittedLabel);
            }

            card.Resize += (_, _) =>
            {
                statusBadge.Location = new Point(card.Width - 100, 24);
                description.MaximumSize = new Size(card.Width - 200, 0);
                if (!isCompleted && !isExpired)
                {
                    var btn = card.Controls.OfType<Button>().FirstOrDefault();
                    if (btn != null) btn.Location = new Point(card.Width - 164, 100);
                }
                else if (isExpired)
                {
                    var lbl = card.Controls.OfType<Label>().FirstOrDefault(l => l.Text == "Past Due");
                    if (lbl != null) lbl.Location = new Point(card.Width - 80, 108);
                }
                else
                {
                    var lbl = card.Controls.OfType<Label>().FirstOrDefault(l => l.Text == "Submitted");
                    if (lbl != null) lbl.Location = new Point(card.Width - 90, 108);
                }
            };

            return card;
        }

        private void OpenFormViewer(EvaluationForm form)
        {
            // Use the teacher the student selected, or the form's target teacher, or 0
            int tid = filterTeacherId ?? form.TargetTeacherId ?? 0;
            string tname = filterTeacherName ?? form.TargetTeacher ?? "";
            // Use selected assignment from dropdown if available
            int? selectedAssignmentId = filterAssignmentId;
            if (subjectDropdown.SelectedItem is TeacherAssignment assignment)
            {
                selectedAssignmentId = assignment.AssignmentID;
            }
            var viewer = new FormViewer(form, tid, tname, selectedAssignmentId);
            viewer.FormSubmitted += () =>
            {
                LoadAvailableForms();
            };
            viewer.ShowDialog(this);
        }

        private void LoadTeacherAssignments()
        {
            if (!filterTeacherId.HasValue) return;
            
            teacherAssignments = TeacherStore.GetTeacherAssignments(filterTeacherId.Value);
            
            subjectDropdown.Items.Clear();
            foreach (var assignment in teacherAssignments)
            {
                subjectDropdown.Items.Add(assignment);
            }
            
            // ToString() is overridden in TeacherAssignment to show: "Subjects (Course · Year · Section)"
            
            // Pre-select the assignment that matches filterAssignmentId, or first one
            if (filterAssignmentId.HasValue)
            {
                var matching = teacherAssignments.FirstOrDefault(a => a.AssignmentID == filterAssignmentId.Value);
                if (matching != null)
                {
                    subjectDropdown.SelectedItem = matching;
                }
                else if (subjectDropdown.Items.Count > 0)
                {
                    subjectDropdown.SelectedIndex = 0;
                }
            }
            else if (subjectDropdown.Items.Count > 0)
            {
                subjectDropdown.SelectedIndex = 0;
            }
        }

        private void OnFormsUpdated()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(OnFormsUpdated));
                return;
            }
            LoadAvailableForms();
        }
    }
}
