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
            mainFlow.Controls.Add(MakeSectionHeader("Reports Received"));

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
                var emptyCard = MakeCard(mainFlow.Width - 48, 120);
                emptyCard.Controls.Add(new Label
                {
                    Text = "No reports received yet.",
                    Font = new Font("Inter SemiBold", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    AutoSize = true,
                    Location = new Point(24, 36)
                });
                emptyCard.Controls.Add(new Label
                {
                    Text = "Reports will appear here once the admin sends evaluation results to you.",
                    Font = new Font("Inter", 10F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(24, 64)
                });
                mainFlow.Controls.Add(emptyCard);
                return;
            }

            // Group by EvaluationID — one accordion card per form
            var groups = reports
                .GroupBy(r => r.EvaluationID)
                .OrderByDescending(g => g.Max(r => r.SubmissionDate))
                .ToList();

            foreach (var g in groups)
            {
                var rep = g.First();
                int totalResponses = g.Sum(r => r.ResponseCount);
                double avgScore = g.Where(r => r.AverageScore > 0).Select(r => (double)r.AverageScore).DefaultIfEmpty(0).Average();
                mainFlow.Controls.Add(BuildReportCard(rep, totalResponses, avgScore, mainFlow.Width - 48));
            }
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

        private Panel BuildReportCard(TeacherReport report, int totalResponses, double avgScore, int width)
        {
            const int HeaderH = 64;
            bool expanded = false;

            // ── Outer wrapper ─────────────────────────────────────────
            var wrapper = new Panel
            {
                BackColor = Color.Transparent,
                Width = width,
                Height = HeaderH,
                Margin = new Padding(0, 0, 0, 10)
            };

            // ── Header strip ──────────────────────────────────────────
            var headerStrip = new Panel
            {
                BackColor = Color.White,
                Size = new Size(width, HeaderH),
                Location = new Point(0, 0),
                Cursor = Cursors.Hand
            };

            var accent = new Panel
            {
                BackColor = AccentGreen,
                Size = new Size(5, HeaderH),
                Location = new Point(0, 0)
            };

            var titleLbl = new Label
            {
                Text = string.IsNullOrWhiteSpace(report.FormTitle) ? $"Evaluation #{report.EvaluationID}" : report.FormTitle,
                Font = new Font("Inter SemiBold", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(18, 10)
            };

            string semTag = (!string.IsNullOrEmpty(report.Semester) && !string.IsNullOrEmpty(report.SchoolYear))
                ? $"  ·  {report.Semester} Sem {report.SchoolYear}" : "";
            var dateLbl = new Label
            {
                Text = report.SubmissionDate.ToString("MMM dd, yyyy") + semTag,
                Font = new Font("Inter", 8F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(18, 34)
            };

            bool hasScore = avgScore > 0;
            var scoreBadge = new Label
            {
                Text = hasScore ? $"★ {avgScore:0.00} avg" : "No rating",
                Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                ForeColor = hasScore ? Color.FromArgb(133, 77, 14) : Color.FromArgb(100, 116, 139),
                BackColor = hasScore ? Color.FromArgb(254, 243, 199) : Color.FromArgb(241, 245, 249),
                AutoSize = true,
                Padding = new Padding(7, 3, 7, 3),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var countBadge = new Label
            {
                Text = $"{totalResponses} response{(totalResponses == 1 ? "" : "s")}",
                Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(38, 166, 91),
                BackColor = Color.FromArgb(220, 252, 231),
                AutoSize = true,
                Padding = new Padding(7, 3, 7, 3),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var chevron = new Label
            {
                Text = "▶",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            headerStrip.Controls.Add(accent);
            headerStrip.Controls.Add(titleLbl);
            headerStrip.Controls.Add(dateLbl);
            headerStrip.Controls.Add(scoreBadge);
            headerStrip.Controls.Add(countBadge);
            headerStrip.Controls.Add(chevron);

            void PositionHeader()
            {
                accent.Size = new Size(5, HeaderH);
                chevron.Location    = new Point(headerStrip.Width - 24, (HeaderH - chevron.PreferredHeight) / 2);
                scoreBadge.Location = new Point(headerStrip.Width - scoreBadge.PreferredWidth - 40, 10);
                countBadge.Location = new Point(headerStrip.Width - countBadge.PreferredWidth - 40, 34);
            }
            headerStrip.Resize += (_, _) => PositionHeader();
            PositionHeader();

            // ── Detail panel (built fresh each expand) ────────────────
            Panel? detailPanel = null;

            EventHandler toggle = (_, _) =>
            {
                expanded = !expanded;
                chevron.Text = expanded ? "▼" : "▶";
                headerStrip.BackColor = expanded ? Color.White : Color.FromArgb(248, 250, 252);

                if (expanded)
                {
                    if (detailPanel != null)
                    {
                        wrapper.Controls.Remove(detailPanel);
                        detailPanel.Dispose();
                    }

                    var form = FormDataStore.GetForm(report.EvaluationID);
                    var sentIds = TeacherStore.GetSentSubmissionIds(teacherId, report.EvaluationID);
                    var responses = FormDataStore.GetResponsesForForm(report.EvaluationID)
                        .Where(r => r.TeacherId == teacherId && sentIds.Contains(r.Id))
                        .ToList();

                    detailPanel = BuildTeacherDetailPanel(form, responses, report, width);
                    detailPanel.Location = new Point(0, HeaderH);
                    wrapper.Controls.Add(detailPanel);
                    wrapper.Height = HeaderH + detailPanel.Height;
                }
                else
                {
                    if (detailPanel != null) detailPanel.Visible = false;
                    wrapper.Height = HeaderH;
                }
            };

            headerStrip.Click += toggle;
            foreach (Control c in headerStrip.Controls)
                c.Click += toggle;

            wrapper.Controls.Add(headerStrip);

            wrapper.Resize += (_, _) =>
            {
                headerStrip.Width = wrapper.Width;
                if (detailPanel != null) detailPanel.Width = wrapper.Width;
            };

            return wrapper;
        }

        private Panel BuildTeacherDetailPanel(EvaluationForm? form, List<FormResponse> responses, TeacherReport report, int width)
        {
            var outer = new Panel
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Width = width,
                Height = 20
            };

            if (form == null || !responses.Any())
            {
                outer.Controls.Add(new Label
                {
                    Text = form == null ? "Form data unavailable." : "No submitted responses found.",
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
                // Question header
                int qTextW = innerW - 90;
                int qTextH;
                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                    qTextH = (int)Math.Ceiling(g.MeasureString(
                        $"Q{q.OrderIndex + 1}. {q.Text}",
                        new Font("Inter SemiBold", 9F, FontStyle.Bold), qTextW).Height) + 4;
                qTextH = Math.Max(qTextH, 20);

                outer.Controls.Add(new Label
                {
                    Text = $"Q{q.OrderIndex + 1}. {q.Text}",
                    Font = new Font("Inter SemiBold", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    AutoSize = false,
                    Size = new Size(qTextW, qTextH),
                    Location = new Point(16, y)
                });
                outer.Controls.Add(new Label
                {
                    Text = q.Type.ToString(),
                    Font = new Font("Inter", 7F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    BackColor = Color.FromArgb(226, 232, 240),
                    AutoSize = true,
                    Padding = new Padding(5, 1, 5, 1),
                    Location = new Point(innerW - 70, y + 2)
                });
                y += qTextH + 8;

                var answers = responses
                    .Select(r => r.Answers.TryGetValue(q.Id, out var a) ? a : null)
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a!).ToList();

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
                    var vals = answers.Select(a => double.TryParse(a, out var v) ? (double?)v : null)
                        .Where(v => v.HasValue).Select(v => v!.Value).ToList();
                    double avg = vals.Any() ? vals.Average() : 0;
                    double maxR = q.MaxRating ?? 5;
                    int barW = Math.Min(280, innerW - 120);
                    int fillW = (int)Math.Round(barW * avg / maxR);

                    var barBg = new Panel { BackColor = Color.FromArgb(226, 232, 240), Size = new Size(barW, 12), Location = new Point(24, y + 6) };
                    barBg.Controls.Add(new Panel { BackColor = Color.FromArgb(234, 179, 8), Size = new Size(Math.Max(0, fillW), 12), Location = Point.Empty });
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
                    int yesCount = answers.Count(a => a.Equals("yes", StringComparison.OrdinalIgnoreCase) || a.Equals("true", StringComparison.OrdinalIgnoreCase));
                    int total = answers.Count;
                    double yesPct = total > 0 ? yesCount * 100.0 / total : 0;
                    double noPct = 100 - yesPct;
                    int barW = Math.Min(280, innerW - 40);
                    int yesFill = (int)Math.Round(barW * yesPct / 100);

                    var barBg = new Panel { BackColor = Color.FromArgb(254, 226, 226), Size = new Size(barW, 16), Location = new Point(24, y + 2) };
                    barBg.Controls.Add(new Panel { BackColor = Color.FromArgb(34, 197, 94), Size = new Size(Math.Max(0, yesFill), 16), Location = Point.Empty });
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
                    var counts = answers.GroupBy(a => a.Trim(), StringComparer.OrdinalIgnoreCase)
                        .Select(g2 => (Option: g2.Key, Count: g2.Count()))
                        .OrderByDescending(x => x.Count).ToList();
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
                        var barBg = new Panel { BackColor = Color.FromArgb(226, 232, 240), Size = new Size(barW, 12), Location = new Point(170, y + 7) };
                        barBg.Controls.Add(new Panel { BackColor = AccentGreen, Size = new Size(Math.Max(0, fillW), 12), Location = Point.Empty });
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
                else // Text
                {
                    var scrollPanel = new Panel
                    {
                        AutoScroll = true,
                        Size = new Size(innerW - 16, Math.Min(200, answers.Count * 44 + 8)),
                        Location = new Point(24, y),
                        BackColor = Color.FromArgb(241, 245, 249)
                    };
                    int cardY = 4;
                    foreach (var ans in answers)
                    {
                        int ansH;
                        using (var g = Graphics.FromHwnd(IntPtr.Zero))
                            ansH = (int)Math.Ceiling(g.MeasureString(ans, new Font("Inter", 9F), innerW - 60).Height) + 8;
                        ansH = Math.Max(ansH, 28);

                        var ansCard = new Panel
                        {
                            BackColor = Color.White,
                            Size = new Size(innerW - 32, ansH),
                            Location = new Point(4, cardY)
                        };
                        ansCard.Controls.Add(new Label
                        {
                            Text = ans,
                            Font = new Font("Inter", 9F),
                            ForeColor = Color.FromArgb(30, 41, 59),
                            AutoSize = false,
                            Size = new Size(innerW - 48, ansH - 8),
                            Location = new Point(8, 4)
                        });
                        scrollPanel.Controls.Add(ansCard);
                        cardY += ansH + 4;
                    }
                    scrollPanel.Height = Math.Min(200, cardY + 4);
                    outer.Controls.Add(scrollPanel);
                    y += scrollPanel.Height + 8;
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

            // ── Approved student comments ─────────────────────────────
            var submissionIds = responses.Select(r => r.Id).ToList();
            var approvedComments = FormDataStore.GetApprovedCommentsForSubmissions(submissionIds);
            if (approvedComments.Any())
            {
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

                    int commentW = innerW - 32;
                    int commentTextH;
                    using (var g = Graphics.FromHwnd(IntPtr.Zero))
                        commentTextH = (int)Math.Ceiling(g.MeasureString(ac.CommentText, new Font("Inter", 9F), commentW - 20).Height) + 4;
                    commentTextH = Math.Max(commentTextH, 18);
                    int cardH = 28 + commentTextH + 10;

                    var commentCard = new Panel { BackColor = Color.White, Size = new Size(innerW, cardH), Location = new Point(16, y) };
                    commentCard.Controls.Add(new Panel { BackColor = levelBg, Size = new Size(4, cardH), Location = Point.Empty });
                    commentCard.Controls.Add(new Label
                    {
                        Text = !string.IsNullOrWhiteSpace(ac.StudentName) ? $"{ac.StudentName} ({ac.StudentId})" : ac.StudentId,
                        Font = new Font("Inter SemiBold", 8F, FontStyle.Bold),
                        ForeColor = AccentGreen,
                        AutoSize = true,
                        Location = new Point(12, 6)
                    });
                    commentCard.Controls.Add(new Label
                    {
                        Text = ac.SystemLevel.ToString(),
                        Font = new Font("Inter", 7F),
                        ForeColor = levelFg,
                        BackColor = levelBg,
                        AutoSize = true,
                        Padding = new Padding(5, 1, 5, 1),
                        Location = new Point(commentW - 60, 5)
                    });
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

            outer.Height = y + 12;
            return outer;
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
