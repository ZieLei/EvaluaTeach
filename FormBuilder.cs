using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class FormBuilder : Form
    {
        private void InitializeComponent() { }

        private readonly EvaluationForm? editingForm;
        private readonly List<FormQuestion> questions = new();
        private readonly FlowLayoutPanel questionsPanel = new();
        private readonly TextBox titleInput = new();
        private readonly TextBox descriptionInput = new();
        private readonly TextBox departmentInput = new();
        private readonly ComboBox courseSelector = new();
        private readonly DateTimePicker dueDatePicker = new();
        private readonly ComboBox typeSelector = new();

        public event Action? FormSaved;

        public FormBuilder()
        {
            InitializeComponent();
            ConfigureFormBuilder();
        }

        public FormBuilder(EvaluationForm form)
        {
            editingForm = form;
            questions.AddRange(form.Questions);
            InitializeComponent();
            ConfigureFormBuilder();
            LoadExistingForm();
        }

        private void ConfigureFormBuilder()
        {
            Text = editingForm == null ? "Create New Form" : "Edit Form";
            Size = new Size(900, 700);
            MinimumSize = new Size(800, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 251);

            var header = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(24, 16, 24, 16)
            };

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
            backBtn.Click += (_, _) =>
            {
                var result = MessageBox.Show("Are you sure you want to go back? Any unsaved changes will be lost.",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    Close();
            };

            var headerLabel = new Label
            {
                Text = Text,
                Font = new Font("Inter", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(120, 18)
            };

            var saveBtn = new Button
            {
                Text = "Save Form",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                Size = new Size(140, 44),
                Location = new Point(720, 8)
            };
            saveBtn.Click += SaveForm;

            var cancelBtn = new Button
            {
                Text = "Cancel",
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(100, 44),
                Location = new Point(610, 8)
            };
            cancelBtn.Click += (_, _) =>
            {
                var result = MessageBox.Show("Are you sure you want to cancel? Any changes will be lost.",
                    "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    Close();
            };

            header.Controls.Add(backBtn);
            header.Controls.Add(headerLabel);
            header.Controls.Add(saveBtn);
            header.Controls.Add(cancelBtn);

            var scrollPanel = new Panel
            {
                Location = new Point(0, 60),
                Size = new Size(Width, Height - 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                BackColor = Color.FromArgb(245, 247, 251)
            };

            ConfigureBasicInfoSection(scrollPanel);
            ConfigureQuestionsSection(scrollPanel);

            Controls.Add(header);
            Controls.Add(scrollPanel);
        }

        private void ConfigureBasicInfoSection(Panel parent)
        {
            var section = new Panel
            {
                BackColor = Color.White,
                Location = new Point(24, 24),
                Size = new Size(parent.Width - 48, 280),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(24)
            };

            var sectionTitle = new Label
            {
                Text = "Form Details",
                Font = new Font("Inter", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            var titleLabel = new Label
            {
                Text = "Form Title *",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(24, 55)
            };

            titleInput.Location = new Point(24, 78);
            titleInput.Size = new Size(section.Width - 48, 32);
            titleInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            titleInput.Font = new Font("Inter", 11F);
            titleInput.BorderStyle = BorderStyle.FixedSingle;
            titleInput.BackColor = Color.FromArgb(248, 250, 252);

            var descLabel = new Label
            {
                Text = "Description",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(24, 120)
            };

            descriptionInput.Location = new Point(24, 143);
            descriptionInput.Size = new Size(section.Width - 48, 32);
            descriptionInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            descriptionInput.Font = new Font("Inter", 11F);
            descriptionInput.BorderStyle = BorderStyle.FixedSingle;
            descriptionInput.BackColor = Color.FromArgb(248, 250, 252);

            var courseLabel = new Label
            {
                Text = "Target Course *",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(24, 190)
            };

            courseSelector.Location = new Point(24, 213);
            courseSelector.Size = new Size(200, 32);
            courseSelector.Font = new Font("Inter", 11F);
            courseSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            courseSelector.Items.AddRange(new[] { "All", "BSIT", "BSCS", "BSCE", "BSEE", "BSME", "BSN", "BSA", "BSBA" });
            courseSelector.SelectedIndex = 0;

            var dueDateLabel = new Label
            {
                Text = "Due Date",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(250, 190)
            };

            dueDatePicker.Location = new Point(250, 213);
            dueDatePicker.Size = new Size(150, 32);
            dueDatePicker.Font = new Font("Inter", 11F);
            dueDatePicker.Format = DateTimePickerFormat.Short;
            dueDatePicker.MinDate = DateTime.Today;
            dueDatePicker.ShowCheckBox = true;
            dueDatePicker.Checked = false;

            section.Controls.Add(sectionTitle);
            section.Controls.Add(titleLabel);
            section.Controls.Add(titleInput);
            section.Controls.Add(descLabel);
            section.Controls.Add(descriptionInput);
            section.Controls.Add(courseLabel);
            section.Controls.Add(courseSelector);
            section.Controls.Add(dueDateLabel);
            section.Controls.Add(dueDatePicker);

            parent.Controls.Add(section);
        }

        private void ConfigureQuestionsSection(Panel parent)
        {
            var section = new Panel
            {
                BackColor = Color.White,
                Location = new Point(24, 328),
                Size = new Size(parent.Width - 48, parent.Height - 280),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(24)
            };

            var sectionTitle = new Label
            {
                Text = "Questions",
                Font = new Font("Inter", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            var typeLabel = new Label
            {
                Text = "Question Type:",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(24, 55)
            };

            // Move selector to avoid clipping/overlap with the label text.
            typeSelector.Location = new Point(170, 52);
            typeSelector.Size = new Size(160, 28);
            typeSelector.Font = new Font("Inter", 10F);
            typeSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            typeSelector.Items.AddRange(new[] { "Rating Scale", "Text Answer", "Yes/No", "Multiple Choice" });
            typeSelector.SelectedIndex = 0;

            var addBtn = new Button
            {
                Text = "+ Add Question",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(130, 32),
                Location = new Point(350, 50)
            };
            addBtn.Click += AddQuestion;

            questionsPanel.FlowDirection = FlowDirection.TopDown;
            questionsPanel.WrapContents = false;
            questionsPanel.AutoScroll = true;
            questionsPanel.BackColor = Color.FromArgb(248, 250, 252);
            questionsPanel.Location = new Point(24, 100);
            questionsPanel.Size = new Size(section.Width - 48, section.Height - 130);
            questionsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            questionsPanel.Padding = new Padding(16);

            section.Controls.Add(sectionTitle);
            section.Controls.Add(typeLabel);
            section.Controls.Add(typeSelector);
            section.Controls.Add(addBtn);
            section.Controls.Add(questionsPanel);

            parent.Controls.Add(section);

            RefreshQuestionsList();
        }

        private void LoadExistingForm()
        {
            if (editingForm == null) return;

            titleInput.Text = editingForm.Title;
            descriptionInput.Text = editingForm.Description;
            departmentInput.Text = editingForm.TargetDepartment;

            if (!string.IsNullOrEmpty(editingForm.TargetCourse))
            {
                int courseIndex = courseSelector.Items.IndexOf(editingForm.TargetCourse);
                if (courseIndex >= 0)
                    courseSelector.SelectedIndex = courseIndex;
            }

            if (editingForm.DueDate.HasValue)
            {
                dueDatePicker.Value = editingForm.DueDate.Value;
                dueDatePicker.Checked = true;
            }
        }

        private void AddQuestion(object? sender, EventArgs e)
        {
            QuestionType type = typeSelector.SelectedIndex switch
            {
                0 => QuestionType.Rating,
                1 => QuestionType.Text,
                2 => QuestionType.YesNo,
                3 => QuestionType.MultipleChoice,
                _ => QuestionType.Rating
            };

            var question = new FormQuestion
            {
                Type = type,
                Text = type == QuestionType.Rating ? "Rate this aspect (1-5)" :
                       type == QuestionType.Text ? "Enter your comments" :
                       type == QuestionType.YesNo ? "Yes or No question" :
                       "Select an option",
                OrderIndex = questions.Count
            };

            if (type == QuestionType.Rating)
            {
                question.MinRating = 1;
                question.MaxRating = 5;
            }

            questions.Add(question);
            RefreshQuestionsList();
        }

        private void RefreshQuestionsList()
        {
            questionsPanel.Controls.Clear();

            for (int i = 0; i < questions.Count; i++)
            {
                var questionCard = CreateQuestionCard(questions[i], i);
                questionsPanel.Controls.Add(questionCard);
            }

            if (questions.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "No questions yet. Add your first question above.",
                    Font = new Font("Inter", 11F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(16, 16)
                };
                questionsPanel.Controls.Add(emptyLabel);
            }
        }

        private Panel CreateQuestionCard(FormQuestion question, int index)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(questionsPanel.Width - 48, question.Type == QuestionType.MultipleChoice ? 200 : 120),
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(16)
            };

            var numberLabel = new Label
            {
                Text = $"{index + 1}.",
                Font = new Font("Inter", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(38, 166, 91),
                AutoSize = true,
                Location = new Point(16, 18)
            };

            var typeBadge = new Label
            {
                Text = question.Type.ToString(),
                Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                BackColor = Color.FromArgb(241, 245, 249),
                AutoSize = true,
                Padding = new Padding(6, 2, 6, 2),
                Location = new Point(50, 20)
            };

            var textInput = new TextBox
            {
                Text = question.Text,
                Font = new Font("Inter", 11F),
                Location = new Point(16, 50),
                Size = new Size(card.Width - 120, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.FixedSingle
            };
            textInput.TextChanged += (_, _) => question.Text = textInput.Text;

            var requiredCheck = new CheckBox
            {
                Text = "Required",
                Checked = question.IsRequired,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(16, 85)
            };
            requiredCheck.CheckedChanged += (_, _) => question.IsRequired = requiredCheck.Checked;

            var deleteBtn = new Button
            {
                Text = "Remove",
                BackColor = Color.FromArgb(254, 226, 226),
                ForeColor = Color.FromArgb(185, 28, 28),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(80, 28),
                Location = new Point(card.Width - 96, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            deleteBtn.Click += (_, _) =>
            {
                questions.RemoveAt(index);
                RefreshQuestionsList();
            };

            card.Controls.Add(numberLabel);
            card.Controls.Add(typeBadge);
            card.Controls.Add(textInput);
            card.Controls.Add(requiredCheck);
            card.Controls.Add(deleteBtn);

            if (question.Type == QuestionType.Rating)
            {
                var ratingLabel = new Label
                {
                    Text = "Scale: 1 to 5",
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(100, 88)
                };
                card.Controls.Add(ratingLabel);
            }
            else if (question.Type == QuestionType.MultipleChoice)
            {
                card.Height = 200;
                var optionsLabel = new Label
                {
                    Text = "Options (comma-separated):",
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(16, 115)
                };

                var optionsInput = new TextBox
                {
                    Text = string.Join(", ", question.Options),
                    Font = new Font("Inter", 10F),
                    Location = new Point(16, 135),
                    Size = new Size(card.Width - 48, 24),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };
                optionsInput.TextChanged += (_, _) =>
                {
                    question.Options = optionsInput.Text.Split(',')
                        .Select(o => o.Trim())
                        .Where(o => !string.IsNullOrEmpty(o))
                        .ToList();
                };

                card.Controls.Add(optionsLabel);
                card.Controls.Add(optionsInput);
            }

            card.Resize += (_, _) =>
            {
                textInput.Size = new Size(card.Width - 120, 28);
                deleteBtn.Location = new Point(card.Width - 96, 50);
            };

            return card;
        }

        private void SaveForm(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titleInput.Text))
            {
                MessageBox.Show("Please enter a form title.", "Required Field",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (questions.Count == 0)
            {
                MessageBox.Show("Please add at least one question.", "Required Field",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var form = editingForm ?? new EvaluationForm();
            form.Title = titleInput.Text.Trim();
            form.Description = descriptionInput.Text.Trim();
            form.TargetDepartment = departmentInput.Text.Trim();
            form.TargetCourse = courseSelector.SelectedItem?.ToString() ?? "All";
            form.DueDate = dueDatePicker.Checked ? dueDatePicker.Value : null;
            form.Questions = questions.OrderBy(q => q.OrderIndex).ToList();
            form.IsActive = true;

            if (editingForm == null)
            {
                form.CreatedBy = SessionStore.UserName;
                form.CreatedById = SessionStore.UserIdNumeric;  // Set the foreign key
                FormDataStore.AddForm(form);
            }
            else
            {
                FormDataStore.UpdateForm(form);
            }

            FormSaved?.Invoke();

            var action = editingForm == null ? "created" : "updated";
            MessageBox.Show($"Form '{form.Title}' has been {action} successfully!",
                "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Close();
        }
    }
}
