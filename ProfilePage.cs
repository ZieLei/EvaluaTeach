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
        public ProfilePage()
        {
            InitializeComponent();
            ConfigureProfileUi();
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

            UpdateProfileLayout();
            Resize += ProfilePage_Resize;
        }

        private void StyleCard(Panel panel)
        {
            panel.BackColor = Color.White;
            panel.Padding = new Padding(24);
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

            panelHeader.Height = 160;
            panelAcademic.Height = 190;
            panelAccount.Height = 160;

            labelAvatar.Size = new Size(84, 84);
            labelAvatar.Location = new Point(28, 28);

            labelStudentName.Location = new Point(132, 36);
            labelStudentMeta.Location = new Point(132, 80);

            int rightColumnLeft = cardWidth / 2 + 10;

            labelAcademicTitle.Location = new Point(28, 20);
            labelStudentIdTitle.Location = new Point(28, 60);
            labelStudentIdValue.Location = new Point(28, 84);
            labelProgramTitle.Location = new Point(rightColumnLeft, 60);
            labelProgramValue.Location = new Point(rightColumnLeft, 84);
            labelYearLevelTitle.Location = new Point(28, 126);
            labelYearLevelValue.Location = new Point(28, 150);
            labelSectionTitle.Location = new Point(rightColumnLeft, 126);
            labelSectionValue.Location = new Point(rightColumnLeft, 150);

            labelAccountTitle.Location = new Point(28, 20);
            labelEmailTitle.Location = new Point(28, 58);
            labelEmailValue.Location = new Point(28, 82);
            labelStatusTitle.Location = new Point(rightColumnLeft, 58);
            labelStatusValue.Location = new Point(rightColumnLeft, 82);
            labelNote.Location = new Point(28, 116);
            labelNote.MaximumSize = new Size(cardWidth - 56, 0);
        }

        private void ProfilePage_Resize(object? sender, EventArgs e)
        {
            UpdateProfileLayout();
        }
    }
}
