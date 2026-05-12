using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class FormViewer : Form
    {
        private void InitializeComponent() { }

        private readonly EvaluationForm form;
        private readonly int teacherId;
        private readonly string teacherName;
        private readonly Dictionary<int, Control> answerControls = new();
        private readonly FlowLayoutPanel questionsPanel = new();
        private Label progressLabel = null!;
        private TextBox commentTextBox = null!;  // assigned in BuildCommentPanel before any submit
        private bool commentPendingEdit = false;

        public event Action? FormSubmitted;

        public FormViewer(EvaluationForm evaluationForm, int teacherId = 0, string teacherName = "")
        {
            form = evaluationForm;
            this.teacherId = teacherId;
            this.teacherName = teacherName;
            InitializeComponent();
            ConfigureFormViewer();
            BuildDynamicForm();
        }

        private void ConfigureFormViewer()
        {
            string title = teacherId > 0 ? $"Evaluating: {form.Title} - {teacherName}" : $"Evaluating: {form.Title}";
            Text = title;
            MinimumSize = new Size(800, 600);
            Size = new Size(900, 700);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 251);

            var header = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(32, 24, 32, 16)
            };

            var titleLabel = new Label
            {
                Text = form.Title,
                Font = new Font("Inter", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(32, 20)
            };

            var descLabel = new Label
            {
                Text = string.IsNullOrEmpty(form.Description) ? "Please answer all questions honestly." : form.Description,
                Font = new Font("Inter", 11F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(32, 52),
                MaximumSize = new Size(header.Width - 200, 0)
            };

            var closeBtn = new Button
            {
                Text = "Cancel",
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(90, 36),
                Location = new Point(header.Width - 122, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            closeBtn.Click += (_, _) =>
            {
                var result = MessageBox.Show("Are you sure you want to cancel? Your progress will be lost.",
                    "Cancel Evaluation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    Close();
            };

            header.Controls.Add(titleLabel);
            header.Controls.Add(descLabel);
            header.Controls.Add(closeBtn);

            Action updateHeaderLayout = () =>
            {
                descLabel.MaximumSize = new Size(header.Width - 200, 0);
                descLabel.Location = new Point(32, titleLabel.Bottom + 8);
                closeBtn.Location = new Point(header.Width - closeBtn.Width - 32, 24);

                // Keep the full description visible so body content starts below it.
                int requiredHeight = descLabel.Bottom + 16;
                header.Height = Math.Max(100, requiredHeight);
            };

            header.Resize += (_, _) => updateHeaderLayout();
            updateHeaderLayout();

            var scrollContainer = new Panel
            {
                Name = "scrollContainer",
                BackColor = Color.FromArgb(245, 247, 251),
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(32)
            };

            questionsPanel.FlowDirection = FlowDirection.TopDown;
            questionsPanel.WrapContents = false;
            questionsPanel.AutoSize = true;
            questionsPanel.BackColor = Color.FromArgb(245, 247, 251);
            questionsPanel.Dock = DockStyle.Top;
            questionsPanel.Padding = new Padding(0, 0, 0, 24);

            scrollContainer.Controls.Add(questionsPanel);

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 251)
            };

            contentPanel.Controls.Add(scrollContainer);
            contentPanel.Controls.Add(header);

            var footer = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Bottom,
                Height = 80,
                Padding = new Padding(32)
            };

            var submitBtn = new Button
            {
                Text = "Submit Evaluation",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 12F, FontStyle.Bold),
                Size = new Size(180, 48),
                Location = new Point(footer.Width - 212, 16),
                Anchor = AnchorStyles.Right
            };
            submitBtn.Click += SubmitForm;

            progressLabel = new Label
            {
                Text = $"0 of {form.Questions.Count} answered",
                Font = new Font("Inter", 11F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(32, 28)
            };

            footer.Controls.Add(submitBtn);
            footer.Controls.Add(progressLabel);

            Controls.Add(contentPanel);
            Controls.Add(footer);

            questionsPanel.ControlAdded += (_, _) => UpdateProgress(progressLabel);
        }

        private void BuildDynamicForm()
        {
            questionsPanel.Controls.Clear();

            if (form.Questions.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "This form has no questions.",
                    Font = new Font("Inter", 14F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Margin = new Padding(32)
                };
                questionsPanel.Controls.Add(emptyLabel);
                return;
            }

            var sortedQuestions = form.Questions.OrderBy(q => q.OrderIndex).ToList();

            foreach (var question in sortedQuestions)
            {
                var questionPanel = CreateQuestionPanel(question);
                questionsPanel.Controls.Add(questionPanel);
            }

            var commentPanel = BuildCommentPanel();
            questionsPanel.Controls.Add(commentPanel);

            questionsPanel.PerformLayout();

            // Scroll to top when form loads
            Shown += (_, _) =>
            {
                if (Controls.Find("scrollContainer", true).FirstOrDefault() is Panel scrollPanel)
                {
                    scrollPanel.AutoScrollPosition = new Point(0, 0);
                }
            };
        }

        private Panel CreateQuestionPanel(FormQuestion question)
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(800, CalculateQuestionHeight(question)),
                Margin = new Padding(0, 0, 0, 16),
                Padding = new Padding(24)
            };

            var requiredText = question.IsRequired ? " *" : "";
            var questionLabel = new Label
            {
                Text = $"{question.OrderIndex + 1}. {question.Text}{requiredText}",
                Font = new Font("Inter", 12F, FontStyle.Bold),
                ForeColor = question.IsRequired ? Color.FromArgb(15, 23, 42) : Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(24, 20),
                MaximumSize = new Size(panel.Width - 48, 0)
            };

            panel.Controls.Add(questionLabel);

            Control answerControl = CreateAnswerControl(question, panel);
            answerControls[question.Id] = answerControl;
            panel.Controls.Add(answerControl);

            panel.Resize += (_, _) =>
            {
                questionLabel.MaximumSize = new Size(panel.Width - 48, 0);
                RepositionAnswerControl(answerControl, questionLabel, panel, question);
            };

            return panel;
        }

        private int CalculateQuestionHeight(FormQuestion question)
        {
            return question.Type switch
            {
                QuestionType.Text => 180,
                QuestionType.Rating => 140,
                QuestionType.YesNo => 120,
                QuestionType.MultipleChoice => 60 + (question.Options.Count * 35),
                _ => 140
            };
        }

        private Control CreateAnswerControl(FormQuestion question, Panel parent)
        {
            Control control;

            switch (question.Type)
            {
                case QuestionType.Rating:
                    control = CreateRatingControl(question);
                    break;

                case QuestionType.Text:
                    control = CreateTextControl();
                    break;

                case QuestionType.YesNo:
                    control = CreateYesNoControl();
                    break;

                case QuestionType.MultipleChoice:
                    control = CreateMultipleChoiceControl(question);
                    break;

                default:
                    control = CreateTextControl();
                    break;
            }

            control.Location = new Point(24, 55);
            control.Width = parent.Width - 48;
            control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            return control;
        }

        private void RepositionAnswerControl(Control control, Label questionLabel, Panel parent, FormQuestion question)
        {
            int yPos = questionLabel.Bottom + 20;
            control.Location = new Point(24, yPos);
            control.Width = parent.Width - 48;

            if (question.Type == QuestionType.Text)
            {
                control.Height = parent.Height - yPos - 24;
            }
        }

        private FlowLayoutPanel CreateRatingControl(FormQuestion question)
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent,
                Height = 50
            };

            int min = question.MinRating ?? 1;
            int max = question.MaxRating ?? 5;

            var group = new GroupBox
            {
                Text = "",
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Width = (max - min + 1) * 55 + 40,
                Height = 75
            };

            var radioButtons = new List<RadioButton>();
            for (int i = min; i <= max; i++)
            {
                var rb = new RadioButton
                {
                    Text = i.ToString(),
                    Font = new Font("Inter", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    Location = new Point(20 + (i - min) * 55, 20),
                    Size = new Size(50, 30),
                    Tag = i
                };
                rb.CheckedChanged += (_, _) => UpdateProgress(progressLabel);
                group.Controls.Add(rb);
                radioButtons.Add(rb);
            }

            var lowLabel = new Label
            {
                Text = "Poor",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(20, 55)
            };

            var highLabel = new Label
            {
                Text = "Excellent",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(group.Width - 80, 55)
            };

            group.Controls.Add(lowLabel);
            group.Controls.Add(highLabel);

            panel.Controls.Add(group);
            panel.Tag = radioButtons;

            return panel;
        }

        private TextBox CreateTextControl()
        {
            var textBox = new TextBox
            {
                Multiline = true,
                Font = new Font("Inter", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252),
                ScrollBars = ScrollBars.Vertical,
                Height = 100
            };
            textBox.TextChanged += (_, _) => UpdateProgress(progressLabel);
            return textBox;
        }

        private FlowLayoutPanel CreateYesNoControl()
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var yesBtn = new RadioButton
            {
                Text = "Yes",
                Font = new Font("Inter", 12F),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Margin = new Padding(0, 0, 32, 0)
            };
            yesBtn.CheckedChanged += (_, _) => UpdateProgress(progressLabel);

            var noBtn = new RadioButton
            {
                Text = "No",
                Font = new Font("Inter", 12F),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true
            };
            noBtn.CheckedChanged += (_, _) => UpdateProgress(progressLabel);

            panel.Controls.Add(yesBtn);
            panel.Controls.Add(noBtn);
            panel.Tag = new List<RadioButton> { yesBtn, noBtn };

            return panel;
        }

        private FlowLayoutPanel CreateMultipleChoiceControl(FormQuestion question)
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var radioButtons = new List<RadioButton>();
            foreach (var option in question.Options)
            {
                var rb = new RadioButton
                {
                    Text = option,
                    Font = new Font("Inter", 11F),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    AutoSize = true,
                    Margin = new Padding(0, 4, 0, 4)
                };
                rb.CheckedChanged += (_, _) => UpdateProgress(progressLabel);
                panel.Controls.Add(rb);
                radioButtons.Add(rb);
            }

            panel.Tag = radioButtons;
            return panel;
        }

        private void UpdateProgress(Label progressLabel)
        {
            int answered = 0;
            foreach (var kvp in answerControls)
            {
                if (IsQuestionAnswered(kvp.Value))
                    answered++;
            }

            progressLabel.Text = $"{answered} of {form.Questions.Count} answered";
        }

        private Panel BuildCommentPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(800, 200),
                Margin = new Padding(0, 0, 0, 16),
                Padding = new Padding(24)
            };

            var label = new Label
            {
                Text = $"{form.Questions.Count + 1}. Additional Comments (Optional)",
                Font = new Font("Inter", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(24, 20),
                MaximumSize = new Size(panel.Width - 48, 0)
            };

            var hint = new Label
            {
                Text = "Share any additional feedback. This field is optional.",
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(24, 48)
            };

            commentTextBox = new TextBox
            {
                Multiline = true,
                Font = new Font("Inter", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252),
                ScrollBars = ScrollBars.Vertical,
                Height = 100,
                Location = new Point(24, 70),
                Width = panel.Width - 48,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            panel.Controls.Add(label);
            panel.Controls.Add(hint);
            panel.Controls.Add(commentTextBox);

            panel.Resize += (_, _) =>
            {
                label.MaximumSize = new Size(panel.Width - 48, 0);
                commentTextBox.Width = panel.Width - 48;
            };

            return panel;
        }

        private bool IsQuestionAnswered(Control control)
        {
            if (control is TextBox textBox)
                return !string.IsNullOrWhiteSpace(textBox.Text);

            if (control is FlowLayoutPanel panel && panel.Tag is List<RadioButton> radioButtons)
                return radioButtons.Any(rb => rb.Checked);

            return false;
        }

        private string GetAnswerValue(Control control)
        {
            if (control is TextBox textBox)
                return textBox.Text.Trim();

            if (control is FlowLayoutPanel panel && panel.Tag is List<RadioButton> radioButtons)
            {
                var selected = radioButtons.FirstOrDefault(rb => rb.Checked);
                return selected?.Text ?? string.Empty;
            }

            return string.Empty;
        }

        private void SubmitForm(object? sender, EventArgs e)
        {
            var missingRequired = new List<string>();
            var answers = new Dictionary<int, string>();

            foreach (var question in form.Questions)
            {
                if (answerControls.TryGetValue(question.Id, out var control))
                {
                    var value = GetAnswerValue(control);
                    answers[question.Id] = value;

                    if (question.IsRequired && string.IsNullOrWhiteSpace(value))
                    {
                        missingRequired.Add($"Question {question.OrderIndex + 1}");
                    }
                }
            }

            if (missingRequired.Any())
            {
                MessageBox.Show($"Please answer the following required questions:\n\n{string.Join("\n", missingRequired)}",
                    "Incomplete Form", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to submit this evaluation? You cannot edit it after submission.",
                "Confirm Submission", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            string commentText = commentTextBox?.Text.Trim() ?? string.Empty;
            CommentLevel detectedLevel = CommentLevel.Normal;

            if (!string.IsNullOrWhiteSpace(commentText))
            {
                detectedLevel = CommentClassifier.Classify(commentText);

                if (detectedLevel == CommentLevel.Mild && !commentPendingEdit)
                {
                    // Courtesy nudge only — comment will be posted either way
                    var editResult = MessageBox.Show(
                        "Your comment may contain slightly aggressive language.\n\n" +
                        "Please keep comments constructive and professional.\n\n" +
                        "Click YES to edit your comment, or NO to submit it as written.\n" +
                        "(Your comment will still be posted if you choose No.)",
                        "Keep Comments Constructive",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (editResult == DialogResult.Yes)
                    {
                        commentPendingEdit = true;
                        commentTextBox.Focus();
                        commentTextBox.SelectAll();
                        return;
                    }
                }
                else if (detectedLevel == CommentLevel.Moderate || detectedLevel == CommentLevel.Severe)
                {
                    if (!commentPendingEdit)
                    {
                        string levelName = detectedLevel.ToString();
                        string desc = CommentClassifier.GetLevelDescription(detectedLevel);

                        var warningResult = MessageBox.Show(
                            $"Your comment has been flagged:\n\n" +
                            $"Level: {levelName.ToUpper()}\n{desc}\n\n" +
                            "Your comment will be held for admin review before it becomes visible.\n\n" +
                            "Click YES to edit your comment, or NO to submit it as-is (pending admin approval).",
                            "Comment Flagged for Review",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (warningResult == DialogResult.Yes)
                        {
                            commentPendingEdit = true;
                            commentTextBox.Focus();
                            commentTextBox.SelectAll();
                            return;
                        }
                    }
                }
            }

            commentPendingEdit = false;

            var response = new FormResponse
            {
                FormId = form.Id,
                TeacherId = teacherId,
                TeacherName = teacherName,
                StudentId = string.IsNullOrWhiteSpace(SessionStore.UserId) ? ProfileStore.StudentId : SessionStore.UserId,
                StudentName = string.IsNullOrWhiteSpace(SessionStore.UserName) ? ProfileStore.Name : SessionStore.UserName,
                Answers = answers,
                SubmittedAt = DateTime.Now
            };

            FormDataStore.AddResponse(response);

            if (!string.IsNullOrWhiteSpace(commentText))
            {
                detectedLevel = CommentClassifier.Classify(commentText);
                // Mild and Normal are auto-approved; Moderate/Severe go to admin review
                var commentStatus = (detectedLevel == CommentLevel.Normal || detectedLevel == CommentLevel.Mild)
                    ? CommentStatus.Approved
                    : CommentStatus.Pending;

                string studentId = string.IsNullOrWhiteSpace(SessionStore.UserId) ? ProfileStore.StudentId : SessionStore.UserId;
                string studentName = string.IsNullOrWhiteSpace(SessionStore.UserName) ? ProfileStore.Name : SessionStore.UserName;
                string studentEmail = ProfileStore.Email ?? string.Empty;

                var formComment = new FormComment
                {
                    SubmissionId = response.Id,
                    StudentDbId = SessionStore.UserIdNumeric,
                    StudentId = studentId,
                    StudentName = studentName,
                    StudentEmail = studentEmail,
                    FormTitle = form.Title,
                    CommentText = commentText,
                    SystemLevel = detectedLevel,
                    Status = commentStatus,
                    SubmittedAt = DateTime.Now
                };
                FormDataStore.AddComment(formComment);
            }

            MessageBox.Show("Your evaluation has been submitted successfully!",
                "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Information);

            FormSubmitted?.Invoke();
            Close();
        }
    }
}
