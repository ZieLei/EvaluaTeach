using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

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
        private readonly ComboBox targetModeSelector = new();
        private readonly ComboBox courseSelector = new();
        private readonly ComboBox teacherSelector = new();
        private Label courseLabel = new();
        private Label teacherLabel = new();
        private readonly DateTimePicker dueDatePicker = new();
        private readonly ComboBox typeSelector = new();
        private readonly ComboBox semesterSelector = new();
        private readonly ComboBox schoolYearInput = new();
        private readonly List<string> categories = new();
        private readonly FlowLayoutPanel categoriesPanel = new();
        private readonly TextBox newCategoryInput = new();
        private readonly Button addCategoryBtn = new();
        private readonly HashSet<string> collapsedCategories = new();

        public event Action? FormSaved;

        public FormBuilder()
        {
            InitializeComponent();
            ConfigureFormBuilder();
            
            // Add 200ms timer to refresh the form after opening
            var refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 200;
            refreshTimer.Tick += (_, _) =>
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
                
                // Refresh the UI components
                RefreshCategoriesList();
                RefreshQuestionsList();
                Invalidate();
                Update();
            };
            refreshTimer.Start();
        }

        public FormBuilder(EvaluationForm form)
        {
            editingForm = form;
            questions.AddRange(form.Questions);
            InitializeComponent();
            ConfigureFormBuilder();
            LoadExistingForm();
            
            // Add 200ms timer to refresh the form after opening
            var refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 200;
            refreshTimer.Tick += (_, _) =>
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
                
                // Refresh the UI components
                RefreshCategoriesList();
                RefreshQuestionsList();
                Invalidate();
                Update();
            };
            refreshTimer.Start();
        }

        private void ConfigureFormBuilder()
        {
            Text = editingForm == null ? "Create New Form" : "Edit Form";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(900, 700);
            StartPosition = FormStartPosition.CenterScreen;
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

            // Target Mode Selection
            var targetModeLabel = new Label
            {
                Text = "Target *",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(24, 190)
            };

            targetModeSelector.Location = new Point(24, 213);
            targetModeSelector.Size = new Size(160, 32);
            targetModeSelector.Font = new Font("Inter", 11F);
            targetModeSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            targetModeSelector.Items.AddRange(new[] { "All Students", "By Course", "By Teacher" });
            targetModeSelector.SelectedIndex = 0;
            targetModeSelector.SelectedIndexChanged += (_, _) => UpdateTargetModeUI();

            // Course selection (shown when By Course selected)
            courseLabel = new Label
            {
                Text = "Course",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(200, 190),
                Visible = false
            };

            courseSelector.Location = new Point(200, 213);
            courseSelector.Size = new Size(140, 32);
            courseSelector.Font = new Font("Inter", 11F);
            courseSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            courseSelector.Items.AddRange(new[] { "All", "BSIT", "BSCS", "BSCE", "BSEE", "BSME", "BSN", "BSA", "BSBA" });
            courseSelector.SelectedIndex = 0;
            courseSelector.Visible = false;

            // Teacher selection (shown when By Teacher selected)
            teacherLabel = new Label
            {
                Text = "Teacher",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(200, 190),
                Visible = false
            };

            teacherSelector.Location = new Point(200, 213);
            teacherSelector.Size = new Size(200, 32);
            teacherSelector.Font = new Font("Inter", 11F);
            teacherSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            teacherSelector.Visible = false;
            LoadTeacherOptions();

            var dueDateLabel = new Label
            {
                Text = "Due Date",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(420, 190)
            };

            dueDatePicker.Location = new Point(420, 213);
            dueDatePicker.Size = new Size(150, 32);
            dueDatePicker.Font = new Font("Inter", 11F);
            dueDatePicker.Format = DateTimePickerFormat.Short;
            dueDatePicker.MinDate = DateTime.Today;
            dueDatePicker.ShowCheckBox = true;
            dueDatePicker.Checked = false;

            var semesterLabel = new Label
            {
                Text = "Semester",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(24, 265)
            };

            semesterSelector.Location = new Point(24, 288);
            semesterSelector.Size = new Size(140, 32);
            semesterSelector.Font = new Font("Inter", 11F);
            semesterSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            semesterSelector.Items.AddRange(new[] { "(none)", "1st", "2nd", "Summer" });
            semesterSelector.SelectedItem = FormDataStore.GetCurrentSemester();
            if (semesterSelector.SelectedIndex < 0) semesterSelector.SelectedIndex = 0;

            var schoolYearLabel = new Label
            {
                Text = "School Year",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(180, 265)
            };

            schoolYearInput.Location = new Point(180, 288);
            schoolYearInput.Size = new Size(140, 32);
            schoolYearInput.Font = new Font("Inter", 11F);
            schoolYearInput.DropDownStyle = ComboBoxStyle.DropDownList;
            string currentSy = FormDataStore.GetCurrentSchoolYear();
            int baseYear = int.Parse(currentSy.Split('-')[0]);
            for (int y = baseYear - 3; y <= baseYear + 3; y++)
                schoolYearInput.Items.Add($"{y}-{y + 1}");
            schoolYearInput.SelectedItem = currentSy;
            if (schoolYearInput.SelectedIndex < 0) schoolYearInput.SelectedIndex = 3;

            section.Size = new Size(section.Width, targetModeSelector.SelectedIndex == 2 ? 380 : 340);

            section.Controls.Add(sectionTitle);
            section.Controls.Add(titleLabel);
            section.Controls.Add(titleInput);
            section.Controls.Add(descLabel);
            section.Controls.Add(descriptionInput);
            section.Controls.Add(targetModeLabel);
            section.Controls.Add(targetModeSelector);
            section.Controls.Add(courseLabel);
            section.Controls.Add(courseSelector);
            section.Controls.Add(teacherLabel);
            section.Controls.Add(teacherSelector);
            section.Controls.Add(dueDateLabel);
            section.Controls.Add(dueDatePicker);
            section.Controls.Add(semesterLabel);
            section.Controls.Add(semesterSelector);
            section.Controls.Add(schoolYearLabel);
            section.Controls.Add(schoolYearInput);

            parent.Controls.Add(section);
        }

        private void ConfigureQuestionsSection(Panel parent)
        {
            var section = new Panel
            {
                BackColor = Color.White,
                Location = new Point(24, 350),
                Size = new Size(parent.Width - 48, parent.Height - 310),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(24)
            };

            var sectionTitle = new Label
            {
                Text = "Categories & Questions",
                Font = new Font("Inter", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            // Categories Management Section
            var categoriesLabel = new Label
            {
                Text = "Categories:",
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(24, 60)
            };

            newCategoryInput.Location = new Point(130, 57);
            newCategoryInput.Size = new Size(200, 28);
            newCategoryInput.Font = new Font("Inter", 10F);
            newCategoryInput.BorderStyle = BorderStyle.FixedSingle;
            newCategoryInput.BackColor = Color.FromArgb(248, 250, 252);
            newCategoryInput.PlaceholderText = "Enter category name";

            addCategoryBtn.Text = "+ Add Category";
            addCategoryBtn.BackColor = Color.FromArgb(59, 130, 246);
            addCategoryBtn.ForeColor = Color.White;
            addCategoryBtn.FlatStyle = FlatStyle.Flat;
            addCategoryBtn.FlatAppearance.BorderSize = 0;
            addCategoryBtn.Font = new Font("Inter SemiBold", 10F, FontStyle.Bold);
            addCategoryBtn.Size = new Size(120, 32);
            addCategoryBtn.Location = new Point(340, 55);
            addCategoryBtn.Click += AddCategory;

            // Categories Display Panel
            categoriesPanel.FlowDirection = FlowDirection.LeftToRight;
            categoriesPanel.WrapContents = true;
            categoriesPanel.AutoScroll = false;
            categoriesPanel.AutoSize = true;
            categoriesPanel.BackColor = Color.FromArgb(248, 250, 252);
            categoriesPanel.Location = new Point(24, 95);
            categoriesPanel.Size = new Size(section.Width - 48, 60);
            categoriesPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            categoriesPanel.Padding = new Padding(8);
            categoriesPanel.Margin = new Padding(0, 0, 0, 16);

            
            // Create a scrollable container for questions - adjusted position for new UI
            var scrollContainer = new Panel
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Location = new Point(24, 200),
                Size = new Size(section.Width - 48, section.Height - 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                Padding = new Padding(8)
            };

            questionsPanel.FlowDirection = FlowDirection.TopDown;
            questionsPanel.WrapContents = false;
            questionsPanel.AutoScroll = false;
            questionsPanel.AutoSize = true;
            questionsPanel.BackColor = Color.FromArgb(248, 250, 252);
            questionsPanel.Dock = DockStyle.Top;
            questionsPanel.Padding = new Padding(8);
            questionsPanel.Margin = new Padding(0);

            scrollContainer.Controls.Add(questionsPanel);

            section.Controls.Add(sectionTitle);
            section.Controls.Add(categoriesLabel);
            section.Controls.Add(newCategoryInput);
            section.Controls.Add(addCategoryBtn);
            section.Controls.Add(categoriesPanel);
            section.Controls.Add(scrollContainer);

            parent.Controls.Add(section);

            RefreshCategoriesList();
            RefreshQuestionsList();
        }

        private void LoadExistingForm()
        {
            if (editingForm == null) return;

            titleInput.Text = editingForm.Title;
            descriptionInput.Text = editingForm.Description;
            departmentInput.Text = editingForm.TargetDepartment;

            // Extract categories from existing questions
            var existingCategories = editingForm.Questions
                .Where(q => !string.IsNullOrEmpty(q.Category))
                .Select(q => q.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            
            categories.Clear();
            categories.AddRange(existingCategories);

            // Load target mode and settings
            if (editingForm.TargetTeacherId.HasValue)
            {
                targetModeSelector.SelectedIndex = 2; // By Teacher
                // Select the teacher in dropdown
                for (int i = 0; i < teacherSelector.Items.Count; i++)
                {
                    if (teacherSelector.Items[i] is TeacherListItem item && item.Id == editingForm.TargetTeacherId.Value)
                    {
                        teacherSelector.SelectedIndex = i;
                        break;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(editingForm.TargetCourse) && editingForm.TargetCourse != "All")
            {
                targetModeSelector.SelectedIndex = 1; // By Course
                int courseIndex = courseSelector.Items.IndexOf(editingForm.TargetCourse);
                if (courseIndex >= 0)
                    courseSelector.SelectedIndex = courseIndex;
            }
            else
            {
                targetModeSelector.SelectedIndex = 0; // All Students
            }
            
            UpdateTargetModeUI();

            if (editingForm.DueDate.HasValue)
            {
                dueDatePicker.MinDate = editingForm.DueDate.Value < DateTime.Today
                    ? editingForm.DueDate.Value
                    : DateTime.Today;
                dueDatePicker.Value = editingForm.DueDate.Value;
                dueDatePicker.Checked = true;
            }

            if (!string.IsNullOrEmpty(editingForm.Semester))
            {
                int semIdx = semesterSelector.Items.IndexOf(editingForm.Semester);
                semesterSelector.SelectedIndex = semIdx >= 0 ? semIdx : 0;
            }

            if (!string.IsNullOrEmpty(editingForm.SchoolYear))
            {
                int syIdx = schoolYearInput.Items.IndexOf(editingForm.SchoolYear);
                if (syIdx >= 0)
                    schoolYearInput.SelectedIndex = syIdx;
                else
                {
                    schoolYearInput.Items.Add(editingForm.SchoolYear);
                    schoolYearInput.SelectedItem = editingForm.SchoolYear;
                }
            }
        }

        private void AddCategory(object? sender, EventArgs e)
        {
            string categoryName = newCategoryInput.Text.Trim();
            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Please enter a category name.", "Required Field",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (categories.Contains(categoryName, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show("This category already exists.", "Duplicate Category",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            categories.Add(categoryName);
            newCategoryInput.Clear();
            RefreshCategoriesList();
            RefreshQuestionsList();
        }

        private void RefreshCategoriesList()
        {
            categoriesPanel.Controls.Clear();

            foreach (var category in categories.OrderBy(c => c))
            {
                var categoryLabel = new Label
                {
                    Text = category,
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true,
                    MaximumSize = new Size(300, 0) // Max width of 300px, then wrap
                };

                // Calculate required size based on text
                var textSize = TextRenderer.MeasureText(category, categoryLabel.Font, new Size(300, 0), TextFormatFlags.WordBreak);
                var chipWidth = Math.Max(textSize.Width + 56, 120); // Min width 120px, extra room for delete btn
                var chipHeight = Math.Max(textSize.Height + 12, 32); // Min height 32px

                var categoryChip = new Panel
                {
                    BackColor = Color.FromArgb(38, 166, 91),
                    Size = new Size(chipWidth, chipHeight),
                    Margin = new Padding(0, 0, 8, 4),
                    Padding = new Padding(12, 6, 12, 6),
                    Cursor = Cursors.Hand,
                    Tag = category, // Store category name for drag operations
                    AllowDrop = true
                };

                categoryLabel.Location = new Point(12, (chipHeight - textSize.Height) / 2);

                // Add drag-and-drop functionality
                categoryChip.MouseDown += (_, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        categoryChip.DoDragDrop(category, DragDropEffects.Move);
                    }
                };

                categoryChip.DragEnter += (_, e) =>
                {
                    if (e.Data.GetDataPresent(DataFormats.Text) && e.AllowedEffect == DragDropEffects.Move)
                    {
                        categoryChip.BackColor = Color.FromArgb(34, 150, 81); // Darker green for hover
                        e.Effect = DragDropEffects.Move;
                    }
                };

                categoryChip.DragLeave += (_, _) =>
                {
                    categoryChip.BackColor = Color.FromArgb(38, 166, 91); // Restore original color
                };

                categoryChip.DragDrop += (_, e) =>
                {
                    if (e.Data.GetDataPresent(DataFormats.Text))
                    {
                        var draggedCategory = e.Data.GetData(DataFormats.Text).ToString();
                        var targetCategory = category;
                        
                        if (draggedCategory != targetCategory)
                        {
                            ReorderCategoriesByDragDrop(draggedCategory, targetCategory);
                        }
                        
                        categoryChip.BackColor = Color.FromArgb(38, 166, 91); // Restore original color
                    }
                };

                var deleteBtn = new Button
                {
                    Text = "×",
                    BackColor = Color.Transparent,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Inter", 9F, FontStyle.Bold),
                    Size = new Size(22, 22),
                    Location = new Point(categoryChip.Width - 26, (chipHeight - 22) / 2),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Cursor = Cursors.Hand,
                    Margin = new Padding(8, 0, 0, 0)
                };
                deleteBtn.Click += (_, _) =>
                {
                    var result = MessageBox.Show($"Delete category '{category}'?\n\nQuestions in this category will be removed.",
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // Remove questions in this category
                        questions.RemoveAll(q => q.Category == category);
                        categories.Remove(category);
                        RefreshCategoriesList();
                        RefreshQuestionsList();
                    }
                };

                categoryChip.Controls.Add(categoryLabel);
                categoryChip.Controls.Add(deleteBtn);
                categoryChip.Resize += (_, _) => deleteBtn.Location = new Point(categoryChip.Width - 32, 6);

                categoriesPanel.Controls.Add(categoryChip);
            }
        }

        private void AddQuestion(string category, QuestionType questionType)
        {
            var question = new FormQuestion
            {
                Type = questionType,
                Text = questionType == QuestionType.Rating ? "Rate this aspect (1-5)" :
                       questionType == QuestionType.Text ? "Enter your comments" :
                       questionType == QuestionType.YesNo ? "Yes or No question" :
                       "Select an option",
                Category = category,
                OrderIndex = questions.Count
            };

            if (questionType == QuestionType.Rating)
            {
                question.MinRating = 1;
                question.MaxRating = 5;
            }

            questions.Add(question);
            RefreshQuestionsList();
        }

        private void ShowQuestionTypeDialog(string category)
        {
            var dialog = new Form
            {
                Text = $"Add Question to {category}",
                Size = new Size(400, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var titleLabel = new Label
            {
                Text = "Select Question Type:",
                Font = new Font("Inter", 12F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var typeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Inter", 11F),
                Location = new Point(20, 60),
                Size = new Size(200, 28),
                Items = { "Rating Scale", "Text Answer", "Yes/No", "Multiple Choice" },
                SelectedIndex = 0
            };

            var addButton = new Button
            {
                Text = "Add Question",
                BackColor = Color.FromArgb(38, 166, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(120, 36),
                Location = new Point(20, 110),
                DialogResult = DialogResult.OK
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(100, 36),
                Location = new Point(150, 110),
                DialogResult = DialogResult.Cancel
            };

            dialog.Controls.AddRange(new Control[] { titleLabel, typeComboBox, addButton, cancelButton });

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                QuestionType selectedType = typeComboBox.SelectedIndex switch
                {
                    0 => QuestionType.Rating,
                    1 => QuestionType.Text,
                    2 => QuestionType.YesNo,
                    3 => QuestionType.MultipleChoice,
                    _ => QuestionType.Rating
                };

                AddQuestion(category, selectedType);
            }
        }

        private void RefreshQuestionsList()
        {
            questionsPanel.Controls.Clear();

            // Group questions by category (only questions with actual categories)
            var groupedQuestions = questions
                .Where(q => !string.IsNullOrEmpty(q.Category))
                .Select((q, index) => new { Question = q, OriginalIndex = index })
                .GroupBy(x => x.Question.Category!)
                .OrderBy(g => g.Key)
                .ToList();

            // Show all categories, even those without questions
            foreach (var category in categories.OrderBy(c => c))
            {
                var group = groupedQuestions.FirstOrDefault(g => g.Key == category);
                var questionsInCategory = group?.Select(x => (dynamic)x).ToList() ?? new List<dynamic>();
                var isCollapsed = collapsedCategories.Contains(category);

                // Add category section (card=150 + margin=12 per question, plus 40 extra whitespace at bottom)
                var sectionHeight = isCollapsed ? 40 : 40 + (questionsInCategory.Count * 162) + 40;
                var categorySection = new Panel
                {
                    BackColor = Color.FromArgb(248, 250, 252),
                    Size = new Size(questionsPanel.Width - 32, sectionHeight),
                    Margin = new Padding(0, 0, 0, 16),
                    Padding = new Padding(0)
                };

                // Category header
                var categoryHeader = new Panel
                {
                    BackColor = Color.FromArgb(38, 166, 91),
                    Size = new Size(categorySection.Width, 40),
                    Dock = DockStyle.Top,
                    Padding = new Padding(16, 0, 16, 0),
                    Cursor = Cursors.Hand
                };
                var collapseIndicator = new Label
                {
                    Text = isCollapsed ? "▶" : "▼",
                    Font = new Font("Inter", 12F, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(16, 10)
                };

                var categoryLabel = new Label
                {
                    Text = category,
                    Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(48, 10)
                };

                var questionCountLabel = new Label
                {
                    Text = $"({questionsInCategory.Count} question{(questionsInCategory.Count > 1 ? "s" : "")})",
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(200, 230, 210),
                    AutoSize = true,
                    Location = new Point(categoryLabel.Right + 8, 12)
                };

                var addQuestionBtn = new Button
                {
                    Text = "+ Add Question",
                    BackColor = Color.FromArgb(255, 255, 255),
                    ForeColor = Color.FromArgb(38, 166, 91),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    Size = new Size(110, 28),
                    Location = new Point(categoryHeader.Width - 130, 6),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Cursor = Cursors.Hand
                };
                addQuestionBtn.Click += (_, _) => ShowQuestionTypeDialog(category);

                // Collapse/expand functionality
                categoryHeader.Click += (_, _) =>
                {
                    if (collapsedCategories.Contains(category))
                        collapsedCategories.Remove(category);
                    else
                        collapsedCategories.Add(category);
                    RefreshQuestionsList();
                };

                categoryHeader.Controls.Add(collapseIndicator);
                categoryHeader.Controls.Add(categoryLabel);
                categoryHeader.Controls.Add(questionCountLabel);
                categoryHeader.Controls.Add(addQuestionBtn);

                // Questions container for this category
                var questionsContainer = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    AutoScroll = false,
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(8, 40, 8, 8),
                    Visible = !isCollapsed
                };

                // Add questions in this category
                for (int i = 0; i < questionsInCategory.Count; i++)
                {
                    var item = questionsInCategory[i];
                    var questionCard = CreateQuestionCard(item.Question, i + 1); // Use category-specific numbering
                    questionsContainer.Controls.Add(questionCard);
                }

                // If no questions in this category, add a hint
                if (questionsInCategory.Count == 0 && !isCollapsed)
                {
                    var hintLabel = new Label
                    {
                        Text = "No questions in this category. Click '+ Add Question' above to add one.",
                        Font = new Font("Inter", 10F),
                        ForeColor = Color.FromArgb(148, 163, 184),
                        AutoSize = true,
                        Margin = new Padding(8, 16, 8, 8)
                    };
                    questionsContainer.Controls.Add(hintLabel);
                }

                categorySection.Controls.Add(categoryHeader);
                categorySection.Controls.Add(questionsContainer);
                questionsPanel.Controls.Add(categorySection);
            }

            // If no categories at all, show initial message
            if (categories.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "No categories yet. Add categories above to get started.",
                    Font = new Font("Inter", 11F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(16, 16)
                };
                questionsPanel.Controls.Add(emptyLabel);
            }
        }

        private void ReorderCategoriesByDragDrop(string draggedCategory, string targetCategory)
        {
            var draggedIndex = categories.IndexOf(draggedCategory);
            var targetIndex = categories.IndexOf(targetCategory);
            
            if (draggedIndex == -1 || targetIndex == -1 || draggedIndex == targetIndex)
                return;

            // Remove from old position
            categories.RemoveAt(draggedIndex);
            
            // Insert at new position
            var adjustedTargetIndex = draggedIndex < targetIndex ? targetIndex - 1 : targetIndex;
            categories.Insert(adjustedTargetIndex, draggedCategory);

            // Refresh the UI
            RefreshCategoriesList();
            RefreshQuestionsList();
        }

        
        private Panel CreateQuestionCard(FormQuestion question, int questionNumber)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(questionsPanel.Width - 48, 150),
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(16)
            };

            // Row 1: Number + Type badge + Category dropdown + Remove button
            var numberLabel = new Label
            {
                Text = $"{questionNumber}.",
                Font = new Font("Inter", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(38, 166, 91),
                AutoSize = true,
                Location = new Point(12, 12)
            };

            var typeBadge = new Label
            {
                Text = question.Type.ToString(),
                Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                BackColor = Color.FromArgb(241, 245, 249),
                AutoSize = true,
                Padding = new Padding(6, 2, 6, 2),
                Location = new Point(40, 12)
            };

            var categoryDropdown = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Inter", 9F),
                Location = new Point(200, 10),
                Size = new Size(180, 24),
                BackColor = Color.FromArgb(248, 250, 252),
                FlatStyle = FlatStyle.Flat
            };
            
            categoryDropdown.Items.AddRange(categories.OrderBy(c => c).ToArray());
            var currentCategoryIndex = categoryDropdown.Items.IndexOf(question.Category);
            if (currentCategoryIndex >= 0)
                categoryDropdown.SelectedIndex = currentCategoryIndex;
            else if (categoryDropdown.Items.Count > 0)
                categoryDropdown.SelectedIndex = 0;
                
            categoryDropdown.SelectedIndexChanged += (_, _) => 
            {
                question.Category = categoryDropdown.SelectedItem?.ToString() ?? string.Empty;
                RefreshQuestionsList();
            };

            var deleteBtn = new Button
            {
                Text = "Remove",
                BackColor = Color.FromArgb(254, 226, 226),
                ForeColor = Color.FromArgb(185, 28, 28),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                Size = new Size(85, 26),
                Location = new Point(card.Width - 101, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            deleteBtn.Click += (_, _) =>
            {
                questions.Remove(question);
                RefreshQuestionsList();
            };

            // Row 2: Question text input (full width)
            var textInput = new TextBox
            {
                Text = question.Text,
                Font = new Font("Inter", 10F),
                Location = new Point(12, 42),
                Size = new Size(card.Width - 40, 26),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.FixedSingle
            };
            textInput.TextChanged += (_, _) => question.Text = textInput.Text;

            // Row 3: Required checkbox + type-specific info
            var requiredCheck = new CheckBox
            {
                Text = "Required",
                Checked = question.IsRequired,
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(12, 76)
            };
            requiredCheck.CheckedChanged += (_, _) => question.IsRequired = requiredCheck.Checked;

            card.Controls.Add(numberLabel);
            card.Controls.Add(typeBadge);
            card.Controls.Add(categoryDropdown);
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
                    Location = new Point(100, 78)
                };
                card.Controls.Add(ratingLabel);
            }
            else if (question.Type == QuestionType.MultipleChoice)
            {
                var optionsLabel = new Label
                {
                    Text = "Options (comma-separated):",
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(12, 104)
                };

                var optionsInput = new TextBox
                {
                    Text = string.Join(", ", question.Options),
                    Font = new Font("Inter", 10F),
                    Location = new Point(12, 122),
                    Size = new Size(card.Width - 40, 24),
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
                textInput.Size = new Size(card.Width - 40, 26);
                deleteBtn.Location = new Point(card.Width - 101, 10);
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
            
            // Set target based on mode
            var mode = targetModeSelector.SelectedIndex;
            if (mode == 0) // All Students
            {
                form.TargetCourse = "All";
                form.TargetTeacherId = null;
            }
            else if (mode == 1) // By Course
            {
                form.TargetCourse = courseSelector.SelectedItem?.ToString() ?? "All";
                form.TargetTeacherId = null;
            }
            else // By Teacher
            {
                form.TargetCourse = "All";
                if (teacherSelector.SelectedItem is TeacherListItem item && item.Id > 0)
                {
                    form.TargetTeacherId = item.Id;
                    form.TargetTeacher = item.Name;
                }
                else
                {
                    form.TargetTeacherId = null;
                }
            }
            form.DueDate = dueDatePicker.Checked ? dueDatePicker.Value : null;
            form.Semester = semesterSelector.SelectedItem?.ToString() == "(none)" ? "" : (semesterSelector.SelectedItem?.ToString() ?? "");
            form.SchoolYear = schoolYearInput.SelectedItem?.ToString() ?? "";
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
                // Check if form has existing responses
                int submissionCount = FormDataStore.GetSubmissionCount(editingForm.Id);
                System.Diagnostics.Debug.WriteLine($"DEBUG: Form {editingForm.Id} has {submissionCount} submissions");
                if (submissionCount > 0)
                {
                    var result = MessageBox.Show(
                        $"This form has {submissionCount} existing submission(s).\n\n" +
                        "Editing this form will delete all existing responses.\n" +
                        "Do you want to continue?",
                        "Warning: Existing Responses",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return;
                }

                FormDataStore.UpdateForm(form);
            }

            FormSaved?.Invoke();

            var action = editingForm == null ? "created" : "updated";
            MessageBox.Show($"Form '{form.Title}' has been {action} successfully!",
                "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Close();
        }

        private void LoadTeacherOptions()
        {
            teacherSelector.Items.Clear();
            teacherSelector.Items.Add(new TeacherListItem(0, "-- Select Teacher --"));
            
            var teachers = TeacherStore.GetAllTeachers()
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToList();
            
            foreach (var teacher in teachers)
            {
                string displayName = $"{teacher.LastName}, {teacher.FirstName}";
                teacherSelector.Items.Add(new TeacherListItem(teacher.TeacherID, displayName));
            }
            
            teacherSelector.SelectedIndex = 0;
        }

        private void UpdateTargetModeUI()
        {
            var mode = targetModeSelector.SelectedIndex;
            courseLabel.Visible = mode == 1;
            courseSelector.Visible = mode == 1;
            teacherLabel.Visible = mode == 2;
            teacherSelector.Visible = mode == 2;
        }

        private class TeacherListItem
        {
            public int Id { get; }
            public string Name { get; }
            
            public TeacherListItem(int id, string name)
            {
                Id = id;
                Name = name;
            }
            
            public override string ToString() => Name;
        }
    }
}
