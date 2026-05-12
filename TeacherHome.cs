using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EvaluaTeach
{
    public class TeacherHome : Form
    {
        // ── Layout panels ──────────────────────────────────────────────
        private readonly Panel sidebar = new();
        private readonly Panel header = new();
        private readonly Panel contentPanel = new();

        // ── Sidebar buttons ────────────────────────────────────────────
        private readonly Button reportsBtn = new();
        private readonly Button profileBtn = new();
        private readonly Button logoutBtn = new();

        // ── Content sub-panels ─────────────────────────────────────────
        private readonly FlowLayoutPanel mainFlow = new();

        // ── Teacher data ───────────────────────────────────────────────
        private readonly int teacherId;
        private readonly string teacherName;
        private readonly string teacherEmail;

        private enum ActiveView { Reports, Profile }
        private ActiveView currentView = ActiveView.Reports;

        // ── Colors ─────────────────────────────────────────────────────
        private static readonly Color SidebarBg   = Color.FromArgb(24, 34, 52);
        private static readonly Color AccentGreen = Color.FromArgb(38, 166, 91);
        private static readonly Color PageBg      = Color.FromArgb(245, 247, 251);

        public TeacherHome()
        {
            teacherId   = SessionStore.UserIdNumeric ?? 0;
            teacherName = SessionStore.UserName;
            teacherEmail = SessionStore.Email;

            SuspendLayout();
            ConfigureForm();
            BuildSidebar();
            BuildHeader();
            BuildContentArea();
            Controls.Add(sidebar);
            Controls.Add(header);
            Controls.Add(contentPanel);
            ResumeLayout(false);

            Resize += (_, _) => UpdateLayout();
            UpdateLayout();
            ShowReports();
        }

        // ── Form setup ─────────────────────────────────────────────────
        private void ConfigureForm()
        {
            Text = "EvaluaTeach - Teacher Dashboard";
            MinimumSize = new Size(1100, 720);
            BackColor = PageBg;
            StartPosition = FormStartPosition.CenterScreen;
        }

        // ── Sidebar ────────────────────────────────────────────────────
        private void BuildSidebar()
        {
            sidebar.BackColor = SidebarBg;
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 240;

            // Logo label at top
            var logoLabel = new Label
            {
                Text = "EvaluaTeach",
                Font = new Font("Inter SemiBold", 14F, FontStyle.Bold),
                ForeColor = AccentGreen,
                AutoSize = false,
                Size = new Size(240, 56),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0)
            };

            var divider = new Panel
            {
                BackColor = Color.FromArgb(45, 58, 80),
                Size = new Size(200, 1),
                Location = new Point(20, 56)
            };

            ConfigureSidebarBtn(reportsBtn,  "  Reports", 80,  true);
            ConfigureSidebarBtn(profileBtn,  "  Profile", 136, false);

            // Logout at bottom
            logoutBtn.Text = "  Logout";
            logoutBtn.ForeColor = Color.FromArgb(148, 163, 184);
            logoutBtn.BackColor = Color.Transparent;
            logoutBtn.FlatStyle = FlatStyle.Flat;
            logoutBtn.FlatAppearance.BorderSize = 0;
            logoutBtn.Font = new Font("Inter", 11F);
            logoutBtn.Size = new Size(192, 44);
            logoutBtn.TextAlign = ContentAlignment.MiddleLeft;
            logoutBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            logoutBtn.Location = new Point(24, 620);
            logoutBtn.Cursor = Cursors.Hand;
            logoutBtn.Click += (_, _) =>
            {
                SessionStore.Logout();
                Program.NavigateTo(new LandingPage());
            };

            reportsBtn.Click += (_, _) => SwitchView(ActiveView.Reports);
            profileBtn.Click  += (_, _) => SwitchView(ActiveView.Profile);

            sidebar.Controls.Add(logoLabel);
            sidebar.Controls.Add(divider);
            sidebar.Controls.Add(reportsBtn);
            sidebar.Controls.Add(profileBtn);
            sidebar.Controls.Add(logoutBtn);

            sidebar.Resize += (_, _) =>
                logoutBtn.Location = new Point(24, sidebar.Height - 64);
        }

        private static void ConfigureSidebarBtn(Button btn, string text, int top, bool active)
        {
            btn.Text = text;
            btn.BackColor = active ? AccentGreen : Color.Transparent;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Inter SemiBold", 11F, FontStyle.Bold);
            btn.Size = new Size(192, 48);
            btn.Location = new Point(24, top);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Cursor = Cursors.Hand;
        }

        private void SwitchView(ActiveView view)
        {
            currentView = view;

            reportsBtn.BackColor = view == ActiveView.Reports ? AccentGreen : Color.Transparent;
            profileBtn.BackColor = view == ActiveView.Profile ? AccentGreen : Color.Transparent;

            switch (view)
            {
                case ActiveView.Reports: ShowReports(); break;
                case ActiveView.Profile: ShowProfile(); break;
            }
        }

        // ── Header ─────────────────────────────────────────────────────
        private void BuildHeader()
        {
            header.BackColor = Color.White;
            header.Height = 64;

            var titleLbl = new Label
            {
                Text = "Teacher Dashboard",
                Font = new Font("Inter SemiBold", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 18)
            };

            var nameLbl = new Label
            {
                Text = teacherName,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            header.Controls.Add(titleLbl);
            header.Controls.Add(nameLbl);

            header.Resize += (_, _) =>
                nameLbl.Location = new Point(header.Width - nameLbl.PreferredWidth - 24, 22);
        }

        // ── Content area ───────────────────────────────────────────────
        private void BuildContentArea()
        {
            contentPanel.BackColor = PageBg;
            contentPanel.AutoScroll = true;

            mainFlow.FlowDirection = FlowDirection.TopDown;
            mainFlow.WrapContents = false;
            mainFlow.AutoSize = true;
            mainFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainFlow.Padding = new Padding(24, 16, 24, 24);
            mainFlow.Dock = DockStyle.Top;

            contentPanel.Controls.Add(mainFlow);
        }

        private void UpdateLayout()
        {
            int sideW = sidebar.Width;
            header.Location = new Point(sideW, 0);
            header.Size = new Size(ClientSize.Width - sideW, 64);

            contentPanel.Location = new Point(sideW, 64);
            contentPanel.Size = new Size(ClientSize.Width - sideW, ClientSize.Height - 64);

            mainFlow.Width = contentPanel.Width - 48;
        }

        // ══════════════════════════════════════════════════════════════
        // REPORTS VIEW
        // ══════════════════════════════════════════════════════════════
        private void ShowReports()
        {
            mainFlow.Controls.Clear();

            mainFlow.Controls.Add(MakeSectionHeader("All Reports Sent by Admin"));

            List<TeacherReport> reports = new();
            try { reports = TeacherStore.GetReportsForTeacher(teacherId); }
            catch (Exception ex)
            {
                mainFlow.Controls.Add(new Label
                {
                    Text = $"Could not load reports: {ex.Message}",
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(220, 38, 38),
                    AutoSize = true,
                    Margin = new Padding(0, 8, 0, 0)
                });
                return;
            }

            if (!reports.Any())
            {
                var emptyCard = MakeCard(mainFlow.Width - 48, 140);
                emptyCard.Controls.Add(new Label
                {
                    Text = "No reports received yet.",
                    Font = new Font("Inter SemiBold", 13F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = true,
                    Location = new Point(24, 44)
                });
                emptyCard.Controls.Add(new Label
                {
                    Text = "Reports will appear here once the admin sends evaluation results to you.",
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(24, 76)
                });
                mainFlow.Controls.Add(emptyCard);
                return;
            }

            foreach (var r in reports)
                mainFlow.Controls.Add(BuildReportCard(r, mainFlow.Width - 48));
        }

        // ══════════════════════════════════════════════════════════════
        // PROFILE VIEW
        // ══════════════════════════════════════════════════════════════
        private void ShowProfile()
        {
            mainFlow.Controls.Clear();
            mainFlow.Controls.Add(MakeSectionHeader("My Profile"));

            // Load full teacher from DB for department/subjects
            Teacher? teacher = null;
            try
            {
                teacher = TeacherStore.GetAllTeachers().FirstOrDefault(t => t.TeacherID == teacherId);
            }
            catch { /* use session data only */ }

            var card = MakeCard(mainFlow.Width - 48, 260);

            var avatar = new Panel
            {
                BackColor = AccentGreen,
                Size = new Size(64, 64),
                Location = new Point(24, 24)
            };
            var initials = teacherName.Split(' ')
                .Where(s => !string.IsNullOrEmpty(s)).Take(2)
                .Select(s => s[0]).ToArray();
            avatar.Controls.Add(new Label
            {
                Text = new string(initials).ToUpper(),
                Font = new Font("Inter SemiBold", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(64, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0)
            });
            card.Controls.Add(avatar);

            int labelX = 108;
            AddProfileRow(card, "Full Name",   teacherName,                      labelX, 24);
            AddProfileRow(card, "Email",       teacherEmail,                     labelX, 66);
            AddProfileRow(card, "Department",  teacher?.Department ?? "—",        labelX, 108);
            AddProfileRow(card, "Section",     teacher?.DisplaySection ?? "—",    labelX, 150);
            AddProfileRow(card, "Course",      teacher?.DisplayCourse ?? "—",     labelX, 192);
            AddProfileRow(card, "Subjects",    teacher?.SubjectsDisplay ?? "—",   labelX, 234);

            mainFlow.Controls.Add(card);
        }

        // ── Helpers ────────────────────────────────────────────────────

        private Panel BuildReportCard(TeacherReport report, int width)
        {
            bool expanded = false;
            const int CollapsedH = 88;

            var card = MakeCard(width, CollapsedH);
            card.Cursor = Cursors.Hand;

            // Left accent bar
            var accent = new Panel
            {
                BackColor = AccentGreen,
                Size = new Size(5, card.Height),
                Location = new Point(0, 0)
            };
            card.Controls.Add(accent);

            // Form title
            var titleLbl = new Label
            {
                Text = string.IsNullOrWhiteSpace(report.FormTitle) ? $"Evaluation #{report.EvaluationID}" : report.FormTitle,
                Font = new Font("Inter SemiBold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, 12)
            };
            card.Controls.Add(titleLbl);

            // Date label
            var dateLbl = new Label
            {
                Text = report.SubmissionDate.ToString("MMM dd, yyyy  •  HH:mm"),
                Font = new Font("Inter", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(20, 36)
            };
            card.Controls.Add(dateLbl);

            // Score pill
            bool hasScore = report.AverageScore > 0;
            var scorePill = new Label
            {
                Text = hasScore ? $"★  {report.AverageScore:0.00} / 5" : "No rating",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = hasScore ? Color.FromArgb(133, 77, 14) : Color.FromArgb(100, 116, 139),
                BackColor = hasScore ? Color.FromArgb(254, 243, 199) : Color.FromArgb(241, 245, 249),
                AutoSize = true,
                Padding = new Padding(10, 4, 10, 4),
                Location = new Point(20, 58)
            };
            card.Controls.Add(scorePill);

            // Response count badge
            var countLbl = new Label
            {
                Text = $"{report.ResponseCount} response{(report.ResponseCount == 1 ? "" : "s")}",
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(38, 166, 91),
                BackColor = Color.FromArgb(220, 252, 231),
                AutoSize = true,
                Padding = new Padding(8, 4, 8, 4),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            card.Controls.Add(countLbl);

            // Chevron
            var chevron = new Label
            {
                Text = "▼",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            card.Controls.Add(chevron);

            // ── Detail panel ──────────────────────────────────────────
            var detailPanel = new Panel
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Visible = false,
                Location = new Point(0, CollapsedH),
                Width = width,
                Padding = new Padding(20, 16, 20, 16)
            };

            // Parse report lines into structured Q&A rows
            var lines = (report.ReportData ?? "")
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var detailFlow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                Location = new Point(0, 0),
                Padding = new Padding(0)
            };

            // Score summary card at top of detail
            if (hasScore)
            {
                var summaryRow = new Panel
                {
                    BackColor = Color.FromArgb(240, 253, 244),
                    Size = new Size(width - 80, 56),
                    Margin = new Padding(0, 0, 0, 12)
                };
                summaryRow.Controls.Add(new Label
                {
                    Text = $"Average Score",
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(22, 101, 52),
                    AutoSize = true,
                    Location = new Point(16, 8)
                });
                summaryRow.Controls.Add(new Label
                {
                    Text = $"{report.AverageScore:0.00} / 5.00",
                    Font = new Font("Inter SemiBold", 18F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(22, 101, 52),
                    AutoSize = true,
                    Location = new Point(16, 24)
                });
                summaryRow.Controls.Add(new Label
                {
                    Text = $"{report.ResponseCount} student response{(report.ResponseCount == 1 ? "" : "s")}",
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(160, 34)
                });
                detailFlow.Controls.Add(summaryRow);
            }

            // Parse lines: skip "-----" separators, group Q + Answer pairs
            bool inHeader = true;
            var metaLines = new List<string>();
            var qaLines   = new List<(string Question, string Answer)>();
            string? pendingQ = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("---")) { inHeader = false; continue; }
                if (inHeader)
                {
                    metaLines.Add(line);
                    continue;
                }
                // Lines like "1. Question text"
                if (Regex.IsMatch(line, @"^\d+\."))
                {
                    pendingQ = Regex.Replace(line, @"^\d+\.\s*", "").Trim();
                }
                else if (line.TrimStart().StartsWith("Answer:") && pendingQ != null)
                {
                    var ans = line.TrimStart().Substring("Answer:".Length).Trim();
                    qaLines.Add((pendingQ, string.IsNullOrWhiteSpace(ans) ? "(no answer)" : ans));
                    pendingQ = null;
                }
            }

            // Render meta info (Teacher, Department lines)
            foreach (var meta in metaLines)
            {
                var parts = meta.Split(':', 2);
                var metaRow = new Panel
                {
                    BackColor = Color.Transparent,
                    AutoSize = true,
                    Margin = new Padding(0, 0, 0, 2)
                };
                metaRow.Controls.Add(new Label
                {
                    Text = parts.Length == 2 ? parts[0].Trim() + ":" : meta,
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(0, 0)
                });
                if (parts.Length == 2)
                {
                    metaRow.Controls.Add(new Label
                    {
                        Text = parts[1].Trim(),
                        Font = new Font("Inter", 9F),
                        ForeColor = Color.FromArgb(30, 41, 59),
                        AutoSize = true,
                        Location = new Point(110, 0)
                    });
                }
                detailFlow.Controls.Add(metaRow);
            }

            if (metaLines.Any() && qaLines.Any())
            {
                detailFlow.Controls.Add(new Panel
                {
                    BackColor = Color.FromArgb(226, 232, 240),
                    Size = new Size(width - 80, 1),
                    Margin = new Padding(0, 8, 0, 8)
                });
            }

            // Render Q&A rows with alternating backgrounds
            for (int i = 0; i < qaLines.Count; i++)
            {
                var (q, a) = qaLines[i];
                bool alt = i % 2 == 1;

                var qaRow = new Panel
                {
                    BackColor = alt ? Color.FromArgb(241, 245, 249) : Color.White,
                    Size = new Size(width - 80, 44),
                    Margin = new Padding(0, 0, 0, 2)
                };

                var qNumBadge = new Label
                {
                    Text = $"Q{i + 1}",
                    Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = AccentGreen,
                    AutoSize = false,
                    Size = new Size(28, 28),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(12, 8)
                };

                var qText = new Label
                {
                    Text = q,
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = false,
                    Size = new Size((qaRow.Width - 120) / 2, 28),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Location = new Point(48, 8)
                };

                var answerLbl = new Label
                {
                    Text = a,
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    AutoSize = false,
                    Size = new Size((qaRow.Width - 120) / 2, 28),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Location = new Point(48 + (qaRow.Width - 120) / 2 + 8, 8)
                };

                qaRow.Controls.Add(qNumBadge);
                qaRow.Controls.Add(qText);
                qaRow.Controls.Add(answerLbl);
                detailFlow.Controls.Add(qaRow);
            }

            // Fallback: if no parsed Q&A, show a plain text label
            if (!qaLines.Any() && !metaLines.Any() && !string.IsNullOrWhiteSpace(report.ReportData))
            {
                detailFlow.Controls.Add(new Label
                {
                    Text = report.ReportData,
                    Font = new Font("Inter", 9F),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = true,
                    MaximumSize = new Size(width - 80, 0),
                    Margin = new Padding(0, 4, 0, 4)
                });
            }

            detailPanel.Controls.Add(detailFlow);

            // Size the detailPanel to fit its content
            detailPanel.AutoSize = false;

            card.Controls.Add(detailPanel);

            void PositionFloating()
            {
                countLbl.Location = new Point(card.Width - countLbl.PreferredWidth - 52, 12);
                chevron.Location  = new Point(card.Width - 28, 36);
                detailPanel.Width = card.Width;
            }

            card.Resize += (_, _) =>
            {
                accent.Size = new Size(5, card.Height);
                PositionFloating();
            };

            EventHandler toggle = (_, _) =>
            {
                expanded = !expanded;
                chevron.Text = expanded ? "▲" : "▼";
                if (expanded)
                {
                    detailPanel.Visible = true;
                    // Let the flow measure itself, then size panel + card around it
                    detailFlow.Width = card.Width - 40;
                    int contentH = detailFlow.GetPreferredSize(new Size(card.Width - 40, 0)).Height + 32;
                    detailPanel.Height = contentH;
                    detailPanel.Location = new Point(0, CollapsedH);
                    card.Height = CollapsedH + contentH;
                }
                else
                {
                    detailPanel.Visible = false;
                    card.Height = CollapsedH;
                }
                accent.Size = new Size(5, card.Height);
            };

            card.Click += toggle;
            foreach (Control c in card.Controls)
                if (c != detailPanel) c.Click += toggle;

            PositionFloating();
            return card;
        }

        private static Panel MakeCard(int width, int height)
        {
            return new Panel
            {
                BackColor = Color.White,
                Size = new Size(width, height),
                Margin = new Padding(0, 0, 0, 10)
            };
        }

        private Label MakeSectionHeader(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Inter SemiBold", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Margin = new Padding(0, 16, 0, 8)
            };
        }

        private static void AddProfileRow(Panel card, string label, string value, int x, int y)
        {
            card.Controls.Add(new Label
            {
                Text = label,
                Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(x, y)
            });
            card.Controls.Add(new Label
            {
                Text = value,
                Font = new Font("Inter", 10F),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(x + 110, y)
            });
        }
    }
}
