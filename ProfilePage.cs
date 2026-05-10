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
        private readonly Button buttonChangePassword = new();

        public ProfilePage()
        {
            InitializeComponent();
            ConfigureProfileUi();
        }

        // Allow other forms to populate profile fields before showing
        public void SetProfileInfo(string studentName, string studentMeta, string? email = null, string? studentId = null)
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
            // Update shared store so subscribers (Home) get the latest values
            ProfileStore.UpdateProfile(studentName, studentMeta, email, studentId);

            // Load avatar from ProfileStore if available
            if (ProfileStore.Avatar != null)
            {
                labelAvatar.Image = ProfileStore.Avatar;
                labelAvatar.Text = string.Empty;
            }
        }

        private void ConfigureProfileUi()
        {
            MinimumSize = new Size(900, 650);
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

            buttonChangePassword.FlatStyle = FlatStyle.Flat;
            buttonChangePassword.FlatAppearance.BorderSize = 0;
            buttonChangePassword.BackColor = Color.FromArgb(254, 243, 199);
            buttonChangePassword.ForeColor = Color.FromArgb(180, 83, 9);
            buttonChangePassword.Font = new Font("Inter SemiBold", 8.5F, FontStyle.Bold);
            buttonChangePassword.Text = "Change Password";
            buttonChangePassword.Click += ButtonChangePassword_Click;
            panelAccount.Controls.Add(buttonChangePassword);

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

            // Save avatar to shared store with no modifications
            ProfileStore.SetAvatar(selectedImage);

            // Use the avatar from ProfileStore for the label
            if (ProfileStore.Avatar != null)
            {
                labelAvatar.Image = ProfileStore.Avatar;
                labelAvatar.Text = string.Empty;
            }
        }

        private void ButtonChangePassword_Click(object? sender, EventArgs e)
        {
            var dialog = new Form
            {
                Text = "Change Password",
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(241, 245, 249),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Size = new Size(420, 320),
                MaximizeBox = false,
                MinimizeBox = false
            };

            int y = 24;

            var lblOld = new Label
            {
                Text = "Current Password",
                Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(32, y),
                AutoSize = true
            };
            dialog.Controls.Add(lblOld);
            y += 24;

            var txtOld = new TextBox
            {
                UseSystemPasswordChar = true,
                Font = new Font("Inter", 11F),
                Location = new Point(32, y),
                Size = new Size(340, 28),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            dialog.Controls.Add(txtOld);
            y += 48;

            var lblNew = new Label
            {
                Text = "New Password",
                Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(32, y),
                AutoSize = true
            };
            dialog.Controls.Add(lblNew);
            y += 24;

            var txtNew = new TextBox
            {
                UseSystemPasswordChar = true,
                Font = new Font("Inter", 11F),
                Location = new Point(32, y),
                Size = new Size(340, 28),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            dialog.Controls.Add(txtNew);
            y += 48;

            var lblConfirm = new Label
            {
                Text = "Confirm New Password",
                Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(32, y),
                AutoSize = true
            };
            dialog.Controls.Add(lblConfirm);
            y += 24;

            var txtConfirm = new TextBox
            {
                UseSystemPasswordChar = true,
                Font = new Font("Inter", 11F),
                Location = new Point(32, y),
                Size = new Size(340, 28),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            dialog.Controls.Add(txtConfirm);

            var btnCancel = new Button
            {
                Text = "Cancel",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(71, 85, 105),
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(100, 36),
                Location = new Point(140, 230)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (_, _) => dialog.Close();
            dialog.Controls.Add(btnCancel);

            var btnSave = new Button
            {
                Text = "Save",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(22, 163, 74),
                ForeColor = Color.White,
                Font = new Font("Inter SemiBold", 10F, FontStyle.Bold),
                Size = new Size(100, 36),
                Location = new Point(252, 230)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (_, _) =>
            {
                string oldPass = txtOld.Text;
                string newPass = txtNew.Text;
                string confirmPass = txtConfirm.Text;

                if (string.IsNullOrWhiteSpace(oldPass))
                {
                    MessageBox.Show("Please enter your current password.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(newPass))
                {
                    MessageBox.Show("Please enter a new password.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (newPass.Length < 6)
                {
                    MessageBox.Show("New password must be at least 6 characters.", "Too Short", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (newPass != confirmPass)
                {
                    MessageBox.Show("New passwords do not match.", "Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ProfileStore.ChangePassword(oldPass, newPass))
                {
                    MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dialog.Close();
                }
                else
                {
                    MessageBox.Show("Current password is incorrect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            dialog.Controls.Add(btnSave);

            dialog.ShowDialog(this);
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
            int availableWidth = ClientSize.Width - 100; // Account for margins
            
            // Responsive card sizing
            int cardWidth = Math.Max(450, Math.Min(800, availableWidth));
            
            panelHeader.Width = cardWidth;
            panelAcademic.Width = cardWidth;
            panelAccount.Width = cardWidth;

            // Panel heights
            panelHeader.Height = 172;
            panelAcademic.Height = 200;
            panelAccount.Height = 210;

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
            buttonChangePassword.Size = new Size(140, 28);
            buttonChangePassword.Location = new Point(CardPaddingSize, 128);
            labelNote.Location = new Point(CardPaddingSize, 168);
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
