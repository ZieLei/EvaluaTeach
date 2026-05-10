using System;
using System.Drawing;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public partial class AdminProfilePage : Form
    {
        private readonly Label titleLabel = new();
        private readonly Panel headerPanel = new();
        private readonly Label nameLabel = new();
        private readonly Label metaLabel = new();
        
        private readonly Panel adminPrivilegesPanel = new();
        private readonly Label privilegesTitleLabel = new();
        private readonly Label privilege1Label = new();
        private readonly Label privilege2Label = new();
        private readonly Label privilege3Label = new();
        private readonly Label privilege4Label = new();
        
        private readonly Panel accountPanel = new();
        private readonly Label accountTitleLabel = new();
        private readonly Label emailTitleLabel = new();
        private readonly Label emailValueLabel = new();
        private readonly Label statusTitleLabel = new();
        private readonly Label statusValueLabel = new();
        private readonly Button changePasswordButton = new();
        private readonly Label noteLabel = new();

        public AdminProfilePage()
        {
            SetupForm();
        }

        public void SetProfileInfo(string adminName, string adminMeta, string? email = null, string? adminId = null)
        {
            nameLabel.Text = adminName;
            metaLabel.Text = adminMeta;
            emailValueLabel.Text = email ?? "No email";
            statusValueLabel.Text = "Active";
            
            ProfileStore.UpdateProfile(adminName, adminMeta, email, adminId);
        }

        private void SetupForm()
        {
            Text = "Admin Profile";
            MinimumSize = new Size(900, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(241, 245, 249);
            AutoScroll = true;
            AutoScrollMinSize = new Size(900, 800);

            // Title
            titleLabel.Text = "Admin Profile";
            titleLabel.Font = new Font("Inter", 24F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(15, 23, 42);
            titleLabel.Location = new Point(50, 30);
            titleLabel.AutoSize = true;
            Controls.Add(titleLabel);

            // Header Panel
            headerPanel.BackColor = Color.White;
            headerPanel.Location = new Point(50, 80);
            headerPanel.Size = new Size(800, 120);
            headerPanel.Padding = new Padding(32);
            
            nameLabel.Font = new Font("Inter", 24F, FontStyle.Bold);
            nameLabel.ForeColor = Color.FromArgb(15, 23, 42);
            nameLabel.Location = new Point(32, 32);
            nameLabel.AutoSize = true;
            
            metaLabel.Font = new Font("Inter", 12F, FontStyle.Regular);
            metaLabel.ForeColor = Color.FromArgb(71, 85, 105);
            metaLabel.Location = new Point(32, 70);
            metaLabel.AutoSize = true;
            
            headerPanel.Controls.Add(nameLabel);
            headerPanel.Controls.Add(metaLabel);
            Controls.Add(headerPanel);

            // Admin Privileges Panel
            adminPrivilegesPanel.BackColor = Color.White;
            adminPrivilegesPanel.Location = new Point(50, 220);
            adminPrivilegesPanel.Size = new Size(800, 240);
            adminPrivilegesPanel.Padding = new Padding(32);
            
            privilegesTitleLabel.Text = "Admin Privileges";
            privilegesTitleLabel.Font = new Font("Inter SemiBold", 12F, FontStyle.Bold);
            privilegesTitleLabel.ForeColor = Color.FromArgb(15, 23, 42);
            privilegesTitleLabel.Location = new Point(32, 32);
            privilegesTitleLabel.AutoSize = true;
            
            privilege1Label.Text = "🔐 Manage user accounts and authentication";
            privilege1Label.Font = new Font("Inter", 10F, FontStyle.Regular);
            privilege1Label.ForeColor = Color.FromArgb(71, 85, 105);
            privilege1Label.Location = new Point(32, 67);
            privilege1Label.AutoSize = true;
            
            privilege2Label.Text = "📊 View and manage evaluation forms";
            privilege2Label.Font = new Font("Inter", 10F, FontStyle.Regular);
            privilege2Label.ForeColor = Color.FromArgb(71, 85, 105);
            privilege2Label.Location = new Point(32, 97);
            privilege2Label.AutoSize = true;
            
            privilege3Label.Text = "👥 Access teacher management system";
            privilege3Label.Font = new Font("Inter", 10F, FontStyle.Regular);
            privilege3Label.ForeColor = Color.FromArgb(71, 85, 105);
            privilege3Label.Location = new Point(32, 127);
            privilege3Label.AutoSize = true;
            
            privilege4Label.Text = "⚙️ System administration and settings";
            privilege4Label.Font = new Font("Inter", 10F, FontStyle.Regular);
            privilege4Label.ForeColor = Color.FromArgb(71, 85, 105);
            privilege4Label.Location = new Point(32, 157);
            privilege4Label.AutoSize = true;
            
            adminPrivilegesPanel.Controls.Add(privilegesTitleLabel);
            adminPrivilegesPanel.Controls.Add(privilege1Label);
            adminPrivilegesPanel.Controls.Add(privilege2Label);
            adminPrivilegesPanel.Controls.Add(privilege3Label);
            adminPrivilegesPanel.Controls.Add(privilege4Label);
            Controls.Add(adminPrivilegesPanel);

            // Account Panel
            accountPanel.BackColor = Color.White;
            accountPanel.Location = new Point(50, 480);
            accountPanel.Size = new Size(800, 200);
            accountPanel.Padding = new Padding(32);
            
            accountTitleLabel.Text = "Account Information";
            accountTitleLabel.Font = new Font("Inter", 12F, FontStyle.Bold);
            accountTitleLabel.ForeColor = Color.FromArgb(15, 23, 42);
            accountTitleLabel.Location = new Point(32, 24);
            accountTitleLabel.AutoSize = true;
            
            emailTitleLabel.Text = "Email";
            emailTitleLabel.Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold);
            emailTitleLabel.ForeColor = Color.FromArgb(100, 116, 139);
            emailTitleLabel.Location = new Point(32, 66);
            emailTitleLabel.AutoSize = true;
            
            emailValueLabel.Font = new Font("Inter", 11F, FontStyle.Bold);
            emailValueLabel.ForeColor = Color.FromArgb(30, 41, 59);
            emailValueLabel.Location = new Point(32, 92);
            emailValueLabel.AutoSize = true;
            
            statusTitleLabel.Text = "Status";
            statusTitleLabel.Font = new Font("Inter SemiBold", 9.5F, FontStyle.Bold);
            statusTitleLabel.ForeColor = Color.FromArgb(100, 116, 139);
            statusTitleLabel.Location = new Point(432, 66);
            statusTitleLabel.AutoSize = true;
            
            statusValueLabel.Font = new Font("Inter", 11F, FontStyle.Bold);
            statusValueLabel.ForeColor = Color.FromArgb(30, 41, 59);
            statusValueLabel.Location = new Point(432, 92);
            statusValueLabel.AutoSize = true;
            
            changePasswordButton.Text = "Change Password";
            changePasswordButton.FlatStyle = FlatStyle.Flat;
            changePasswordButton.FlatAppearance.BorderSize = 0;
            changePasswordButton.BackColor = Color.FromArgb(254, 243, 199);
            changePasswordButton.ForeColor = Color.FromArgb(180, 83, 9);
            changePasswordButton.Font = new Font("Inter SemiBold", 8.5F, FontStyle.Bold);
            changePasswordButton.Size = new Size(140, 28);
            changePasswordButton.Location = new Point(32, 128);
            changePasswordButton.Click += ChangePasswordButton_Click;
            
            noteLabel.Text = "Note: For security purposes, please contact your system administrator for account changes.";
            noteLabel.Font = new Font("Inter", 9F, FontStyle.Regular);
            noteLabel.ForeColor = Color.FromArgb(100, 116, 139);
            noteLabel.Location = new Point(32, 168);
            noteLabel.MaximumSize = new Size(736, 60);
            noteLabel.AutoSize = true;
            
            accountPanel.Controls.Add(accountTitleLabel);
            accountPanel.Controls.Add(emailTitleLabel);
            accountPanel.Controls.Add(emailValueLabel);
            accountPanel.Controls.Add(statusTitleLabel);
            accountPanel.Controls.Add(statusValueLabel);
            accountPanel.Controls.Add(changePasswordButton);
            accountPanel.Controls.Add(noteLabel);
            Controls.Add(accountPanel);
        }

        private void ChangePasswordButton_Click(object? sender, EventArgs e)
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
    }
}
