using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class ProfilePage : Form
    {
        private const int CardPaddingSize = 32;
        private const int CardMarginSize = 12;

        public ProfilePage()
        {
            InitializeComponent();
            ConfigureProfileUi();
        }

        // Allow other forms to populate profile fields before showing
        public void SetProfileInfo(string studentName, string studentMeta, string email = null, string studentId = null)
        {
            if (!string.IsNullOrWhiteSpace(studentName))
            {
                labelStudentName.Text = studentName;
            }

            if (!string.IsNullOrWhiteSpace(studentMeta))
            {
                labelStudentMeta.Text = studentMeta;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                labelEmailValue.Text = email;
            }

            if (!string.IsNullOrWhiteSpace(studentId))
            {
                labelStudentIdValue.Text = studentId;
            }
        }

        private void ConfigureProfileUi()
        {
            MinimumSize = new Size(980, 720);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(241, 245, 249);
            Text = "Student Profile";

            tableLayoutPanel1.BackColor = BackColor;

            labelTitle.ForeColor = Color.FromArgb(15, 23, 42);

            StyleCard(panelHeader);
            StyleCard(panelAcademic);
            StyleCard(panelAccount);

            labelAvatar.BackColor = Color.FromArgb(22, 163, 74);
            labelAvatar.ForeColor = Color.White;
            labelAvatar.Font = new Font("Inter", 20F, FontStyle.Bold);

            buttonEditPhoto.FlatStyle = FlatStyle.Flat;
            buttonEditPhoto.FlatAppearance.BorderSize = 0;
            buttonEditPhoto.BackColor = Color.FromArgb(220, 252, 231);
            buttonEditPhoto.ForeColor = Color.FromArgb(21, 128, 61);
            buttonEditPhoto.Font = new Font("Inter SemiBold", 8.5F, FontStyle.Bold);
            buttonEditPhoto.Text = "Edit photo";
            buttonEditPhoto.Click += ButtonEditPhoto_Click;

            labelStudentName.Font = new Font("Inter", 18F, FontStyle.Bold);
            labelStudentName.ForeColor = Color.FromArgb(15, 23, 42);

            labelStudentMeta.Font = new Font("Inter", 10F, FontStyle.Regular);
            labelStudentMeta.ForeColor = Color.FromArgb(100, 116, 139);

            StyleSectionTitle(labelAcademicTitle);
            StyleSectionTitle(labelAccountTitle);

            StyleFieldTitle(labelStudentIdTitle);
            StyleFieldTitle(labelProgramTitle);
            StyleFieldTitle(labelYearLevelTitle);
            StyleFieldTitle(labelSectionTitle);
            StyleFieldTitle(labelEmailTitle);
            StyleFieldTitle(labelStatusTitle);

            StyleFieldValue(labelStudentIdValue);
            StyleFieldValue(labelProgramValue);
            StyleFieldValue(labelYearLevelValue);
            StyleFieldValue(labelSectionValue);
            StyleFieldValue(labelEmailValue);
            StyleFieldValue(labelStatusValue);

            labelNote.ForeColor = Color.FromArgb(100, 116, 139);
            labelNote.Font = new Font("Inter", 9F, FontStyle.Regular);

            tableLayoutPanel1.RowStyles[1].Height = 196F;
            tableLayoutPanel1.RowStyles[2].Height = 224F;
            tableLayoutPanel1.RowStyles[3].Height = 192F;

            UpdateProfileLayout();
            Resize += ProfilePage_Resize;
        }

        private void ButtonEditPhoto_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialog = new()
            {
                Title = "Choose Profile Photo",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                CheckFileExists = true
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using Image selectedImage = Image.FromFile(dialog.FileName);
            labelAvatar.Image = new Bitmap(selectedImage, new Size(84, 84));
            labelAvatar.Text = string.Empty;
        }

        private void StyleCard(Panel panel)
        {
            panel.BackColor = Color.White;
            panel.Padding = new Padding(CardPaddingSize);
        }

        private void StyleSectionTitle(Label label)
        {
            label.Font = new Font("Inter", 12F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(15, 23, 42);
        }

        private void StyleFieldTitle(Label label)
        {
            label.Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(100, 116, 139);
        }

        private void StyleFieldValue(Label label)
        {
            label.Font = new Font("Inter", 11F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(30, 41, 59);
        }

        private void UpdateProfileLayout()
        {
            int cardWidth = Math.Min(560, Math.Max(460, ClientSize.Width - 280));

            panelHeader.Width = cardWidth;
            panelAcademic.Width = cardWidth;
            panelAccount.Width = cardWidth;

            panelHeader.Height = 172;
            panelAcademic.Height = 200;
            panelAccount.Height = 168;

            labelAvatar.Size = new Size(84, 84);
            labelAvatar.Location = new Point(CardPaddingSize, CardPaddingSize);
            labelAvatar.ImageAlign = ContentAlignment.MiddleCenter;

            buttonEditPhoto.Size = new Size(96, 28);
            buttonEditPhoto.Location = new Point(CardPaddingSize - 6, labelAvatar.Bottom + 12);

            int headerTextLeft = labelAvatar.Right + 24;
            labelStudentName.Location = new Point(headerTextLeft, CardPaddingSize + 6);
            labelStudentMeta.Location = new Point(headerTextLeft, labelStudentName.Bottom + 12);

            int rightColumnLeft = cardWidth / 2 + 16;

            labelAcademicTitle.Location = new Point(CardPaddingSize, CardPaddingSize - 8);
            labelStudentIdTitle.Location = new Point(CardPaddingSize, 68);
            labelStudentIdValue.Location = new Point(CardPaddingSize, 94);
            labelProgramTitle.Location = new Point(rightColumnLeft, 68);
            labelProgramValue.Location = new Point(rightColumnLeft, 94);
            labelYearLevelTitle.Location = new Point(CardPaddingSize, 134);
            labelYearLevelValue.Location = new Point(CardPaddingSize, 160);
            labelSectionTitle.Location = new Point(rightColumnLeft, 134);
            labelSectionValue.Location = new Point(rightColumnLeft, 160);

            labelAccountTitle.Location = new Point(CardPaddingSize, CardPaddingSize - 8);
            labelEmailTitle.Location = new Point(CardPaddingSize, 66);
            labelEmailValue.Location = new Point(CardPaddingSize, 92);
            labelStatusTitle.Location = new Point(rightColumnLeft, 66);
            labelStatusValue.Location = new Point(rightColumnLeft, 92);
            labelNote.Location = new Point(CardPaddingSize, 126);
            labelNote.MaximumSize = new Size(cardWidth - (CardPaddingSize * 2), 0);

            panelHeader.Margin = new Padding(50, CardMarginSize, 50, CardMarginSize);
            panelAcademic.Margin = new Padding(50, CardMarginSize, 50, CardMarginSize);
            panelAccount.Margin = new Padding(50, CardMarginSize, 50, CardMarginSize);
        }

        private void ProfilePage_Resize(object? sender, EventArgs e)
        {
            UpdateProfileLayout();
        }
    }
}
