using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace EvaluaTeach
{
    public static class TeacherStore
    {
        public static event Action? TeachersUpdated;

        public static List<Teacher> GetAllTeachers()
        {
            var teachers = new List<Teacher>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT TeacherID, FirstName, LastName, Email, Department, Section, Course, YearLevel, Subjects, CreatedAt
                FROM Teacher
                ORDER BY CreatedAt DESC", conn);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var subjectsStr = reader.IsDBNull(reader.GetOrdinal("Subjects")) ? "" : reader.GetString("Subjects");
                    teachers.Add(new Teacher
                    {
                        TeacherID = reader.GetInt32("TeacherID"),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email"),
                        Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? "" : reader.GetString("Department"),
                        Section = reader.IsDBNull(reader.GetOrdinal("Section")) ? "" : reader.GetString("Section"),
                        Course = reader.IsDBNull(reader.GetOrdinal("Course")) ? "" : reader.GetString("Course"),
                        YearLevel = reader.IsDBNull(reader.GetOrdinal("YearLevel")) ? "" : reader.GetInt32("YearLevel").ToString(),
                        Subjects = string.IsNullOrEmpty(subjectsStr) ? new List<string>() : subjectsStr.Split(',').Select(s => s.Trim()).ToList(),
                        CreatedAt = reader.GetDateTime("CreatedAt")
                    });
                }
            }

            // Load assignments for each teacher
            foreach (var teacher in teachers)
            {
                teacher.Assignments = GetTeacherAssignments(teacher.TeacherID);
            }

            return teachers;
        }

        public static List<TeacherAssignment> GetTeacherAssignments(int teacherId)
        {
            var assignments = new List<TeacherAssignment>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT AssignmentID, TeacherID, Section, Course, YearLevel, Subjects
                FROM teacher_assignment
                WHERE TeacherID = @teacherId
                ORDER BY AssignmentID", conn);
            cmd.Parameters.AddWithValue("@teacherId", teacherId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var subjectsStr = reader.IsDBNull(reader.GetOrdinal("Subjects")) ? "" : reader.GetString("Subjects");
                assignments.Add(new TeacherAssignment
                {
                    AssignmentID = reader.GetInt32("AssignmentID"),
                    TeacherID = reader.GetInt32("TeacherID"),
                    Section = reader.IsDBNull(reader.GetOrdinal("Section")) ? "" : reader.GetString("Section"),
                    Course = reader.IsDBNull(reader.GetOrdinal("Course")) ? "" : reader.GetString("Course"),
                    YearLevel = reader.IsDBNull(reader.GetOrdinal("YearLevel")) ? "" : reader.GetString("YearLevel"),
                    Subjects = string.IsNullOrEmpty(subjectsStr) ? new List<string>() : subjectsStr.Split(',').Select(s => s.Trim()).ToList()
                });
            }

            return assignments;
        }

        public static void SaveTeacherAssignments(int teacherId, List<TeacherAssignment> assignments)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            // Delete existing assignments
            var deleteCmd = new MySqlCommand("DELETE FROM teacher_assignment WHERE TeacherID = @teacherId", conn);
            deleteCmd.Parameters.AddWithValue("@teacherId", teacherId);
            deleteCmd.ExecuteNonQuery();

            // Insert new assignments
            foreach (var assignment in assignments)
            {
                var insertCmd = new MySqlCommand(@"
                    INSERT INTO teacher_assignment (TeacherID, Section, Course, YearLevel, Subjects)
                    VALUES (@teacherId, @section, @course, @yearLevel, @subjects)", conn);
                insertCmd.Parameters.AddWithValue("@teacherId", teacherId);
                insertCmd.Parameters.AddWithValue("@section", string.IsNullOrEmpty(assignment.Section) ? (object)DBNull.Value : assignment.Section);
                insertCmd.Parameters.AddWithValue("@course", string.IsNullOrEmpty(assignment.Course) ? (object)DBNull.Value : assignment.Course);
                insertCmd.Parameters.AddWithValue("@yearLevel", string.IsNullOrEmpty(assignment.YearLevel) ? (object)DBNull.Value : assignment.YearLevel);
                insertCmd.Parameters.AddWithValue("@subjects", string.Join(", ", assignment.Subjects));
                insertCmd.ExecuteNonQuery();
            }
        }

        public static void AddTeacherAssignment(int teacherId, TeacherAssignment assignment)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                INSERT INTO teacher_assignment (TeacherID, Section, Course, YearLevel, Subjects)
                VALUES (@teacherId, @section, @course, @yearLevel, @subjects)", conn);
            cmd.Parameters.AddWithValue("@teacherId", teacherId);
            cmd.Parameters.AddWithValue("@section", string.IsNullOrEmpty(assignment.Section) ? (object)DBNull.Value : assignment.Section);
            cmd.Parameters.AddWithValue("@course", string.IsNullOrEmpty(assignment.Course) ? (object)DBNull.Value : assignment.Course);
            cmd.Parameters.AddWithValue("@yearLevel", string.IsNullOrEmpty(assignment.YearLevel) ? (object)DBNull.Value : assignment.YearLevel);
            cmd.Parameters.AddWithValue("@subjects", string.Join(", ", assignment.Subjects));
            cmd.ExecuteNonQuery();

            TeachersUpdated?.Invoke();
        }

        public static void DeleteTeacherAssignment(int assignmentId)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM teacher_assignment WHERE AssignmentID = @id", conn);
            cmd.Parameters.AddWithValue("@id", assignmentId);
            cmd.ExecuteNonQuery();

            TeachersUpdated?.Invoke();
        }

        public static void AddTeacher(Teacher teacher, string password)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                INSERT INTO Teacher (FirstName, LastName, Email, Department, Section, Course, YearLevel, Subjects, Password, CreatedAt)
                VALUES (@firstName, @lastName, @email, @department, @section, @course, @yearLevel, @subjects, @password, NOW())", conn);

            cmd.Parameters.AddWithValue("@firstName", teacher.FirstName);
            cmd.Parameters.AddWithValue("@lastName", teacher.LastName);
            cmd.Parameters.AddWithValue("@email", teacher.Email);
            cmd.Parameters.AddWithValue("@department", teacher.Department);
            cmd.Parameters.AddWithValue("@section", string.IsNullOrEmpty(teacher.Section) ? (object)DBNull.Value : teacher.Section);
            cmd.Parameters.AddWithValue("@course", string.IsNullOrEmpty(teacher.Course) ? (object)DBNull.Value : teacher.Course);
            cmd.Parameters.AddWithValue("@yearLevel", string.IsNullOrEmpty(teacher.YearLevel) ? (object)DBNull.Value : teacher.YearLevel);
            cmd.Parameters.AddWithValue("@subjects", string.Join(", ", teacher.Subjects));
            cmd.Parameters.AddWithValue("@password", HashPassword(password));

            cmd.ExecuteNonQuery();
            teacher.TeacherID = (int)cmd.LastInsertedId;
            TeachersUpdated?.Invoke();
        }

        public static void UpdateTeacher(Teacher teacher)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                UPDATE Teacher
                SET FirstName = @firstName, LastName = @lastName,
                    Email = @email, Department = @department, Section = @section, Course = @course, YearLevel = @yearLevel, Subjects = @subjects
                WHERE TeacherID = @id", conn);

            cmd.Parameters.AddWithValue("@id", teacher.TeacherID);
            cmd.Parameters.AddWithValue("@firstName", teacher.FirstName);
            cmd.Parameters.AddWithValue("@lastName", teacher.LastName);
            cmd.Parameters.AddWithValue("@email", teacher.Email);
            cmd.Parameters.AddWithValue("@department", teacher.Department);
            cmd.Parameters.AddWithValue("@section", string.IsNullOrEmpty(teacher.Section) ? (object)DBNull.Value : teacher.Section);
            cmd.Parameters.AddWithValue("@course", string.IsNullOrEmpty(teacher.Course) ? (object)DBNull.Value : teacher.Course);
            cmd.Parameters.AddWithValue("@yearLevel", string.IsNullOrEmpty(teacher.YearLevel) ? (object)DBNull.Value : teacher.YearLevel);
            cmd.Parameters.AddWithValue("@subjects", string.Join(", ", teacher.Subjects));

            cmd.ExecuteNonQuery();
            TeachersUpdated?.Invoke();
        }

        public static void DeleteTeacher(int teacherId)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            // Delete comments tied to submissions for this teacher
            new MySqlCommand(@"
                DELETE c FROM comment c
                INNER JOIN FormSubmission fs ON fs.SubmissionID = c.SubmissionID
                WHERE fs.TeacherID = @id", conn)
                { Parameters = { new MySqlParameter("@id", teacherId) } }.ExecuteNonQuery();

            // Delete reports
            new MySqlCommand("DELETE FROM report WHERE TeacherID = @id", conn)
                { Parameters = { new MySqlParameter("@id", teacherId) } }.ExecuteNonQuery();

            // Delete survey responses (answers) for this teacher's submissions
            new MySqlCommand(@"
                DELETE sr FROM SurveyResponse sr
                INNER JOIN FormSubmission fs ON fs.SubmissionID = sr.SubmissionID
                WHERE fs.TeacherID = @id", conn)
                { Parameters = { new MySqlParameter("@id", teacherId) } }.ExecuteNonQuery();

            // Delete submissions
            new MySqlCommand("DELETE FROM FormSubmission WHERE TeacherID = @id", conn)
                { Parameters = { new MySqlParameter("@id", teacherId) } }.ExecuteNonQuery();

            // Delete teacher assignments
            new MySqlCommand("DELETE FROM teacher_assignment WHERE TeacherID = @id", conn)
                { Parameters = { new MySqlParameter("@id", teacherId) } }.ExecuteNonQuery();

            // Finally delete the teacher
            new MySqlCommand("DELETE FROM Teacher WHERE TeacherID = @id", conn)
                { Parameters = { new MySqlParameter("@id", teacherId) } }.ExecuteNonQuery();

            TeachersUpdated?.Invoke();
        }

        public static bool EmailExists(string email, int? excludeId = null)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string sql = "SELECT COUNT(*) FROM Teacher WHERE Email = @email";
            if (excludeId.HasValue)
                sql += " AND TeacherID != @excludeId";

            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", email);
            if (excludeId.HasValue)
                cmd.Parameters.AddWithValue("@excludeId", excludeId.Value);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static List<Teacher> GetTeachersForStudent(string section, string course, string yearLevel)
        {
            var teachers = new List<Teacher>();
            using var conn = Database.GetConnection();
            conn.Open();

            // Use teacher_assignment table for matching - teacher must have an assignment
            // that matches ALL of: section, course, yearLevel
            var cmd = new MySqlCommand(@"
                SELECT DISTINCT t.TeacherID, t.FirstName, t.LastName, t.Email, t.Department, t.Section, t.Course, t.YearLevel, t.Subjects, t.CreatedAt
                FROM Teacher t
                INNER JOIN teacher_assignment ta ON ta.TeacherID = t.TeacherID
                WHERE (ta.Section = @section OR ta.Section IS NULL OR ta.Section = '')
                  AND (ta.Course = @course OR ta.Course IS NULL OR ta.Course = '')
                  AND (ta.YearLevel = @yearLevel OR ta.YearLevel IS NULL OR ta.YearLevel = '')
                ORDER BY t.LastName, t.FirstName", conn);

            cmd.Parameters.AddWithValue("@section", section);
            cmd.Parameters.AddWithValue("@course", course);
            cmd.Parameters.AddWithValue("@yearLevel", yearLevel);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var subjectsStr = reader.IsDBNull(reader.GetOrdinal("Subjects")) ? "" : reader.GetString("Subjects");
                    var teacherId = reader.GetInt32("TeacherID");
                    teachers.Add(new Teacher
                    {
                        TeacherID = teacherId,
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email"),
                        Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? "" : reader.GetString("Department"),
                        Section = reader.IsDBNull(reader.GetOrdinal("Section")) ? "" : reader.GetString("Section"),
                        Course = reader.IsDBNull(reader.GetOrdinal("Course")) ? "" : reader.GetString("Course"),
                        YearLevel = reader.IsDBNull(reader.GetOrdinal("YearLevel")) ? "" : reader.GetInt32("YearLevel").ToString(),
                        Subjects = string.IsNullOrEmpty(subjectsStr) ? new List<string>() : subjectsStr.Split(',').Select(s => s.Trim()).ToList(),
                        CreatedAt = reader.GetDateTime("CreatedAt"),
                        // Load only matching assignments for this student
                        Assignments = new List<TeacherAssignment>() // Will be populated separately if needed
                    });
                }
            }

            // Load matching assignments for each teacher
            foreach (var teacher in teachers)
            {
                teacher.Assignments = GetMatchingAssignmentsForStudent(teacher.TeacherID, section, course, yearLevel);
            }

            return teachers;
        }

        private static List<TeacherAssignment> GetMatchingAssignmentsForStudent(int teacherId, string section, string course, string yearLevel)
        {
            var assignments = new List<TeacherAssignment>();
            using var conn = Database.GetConnection();
            conn.Open();

            // Build dynamic query - if filter is empty/"All", match all values
            var sql = @"
                SELECT AssignmentID, TeacherID, Section, Course, YearLevel, Subjects
                FROM teacher_assignment
                WHERE TeacherID = @teacherId";

            // Only filter on section if a specific value is provided
            if (!string.IsNullOrEmpty(section) && section != "All")
            {
                sql += " AND (Section = @section OR Section IS NULL OR Section = '')";
            }

            // Only filter on course if a specific value is provided
            if (!string.IsNullOrEmpty(course) && course != "All")
            {
                sql += " AND (Course = @course OR Course IS NULL OR Course = '')";
            }

            // Only filter on year level if a specific value is provided
            if (!string.IsNullOrEmpty(yearLevel) && yearLevel != "All")
            {
                sql += " AND (YearLevel = @yearLevel OR YearLevel IS NULL OR YearLevel = '')";
            }

            sql += " ORDER BY AssignmentID";

            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@teacherId", teacherId);

            if (!string.IsNullOrEmpty(section) && section != "All")
                cmd.Parameters.AddWithValue("@section", section);
            if (!string.IsNullOrEmpty(course) && course != "All")
                cmd.Parameters.AddWithValue("@course", course);
            if (!string.IsNullOrEmpty(yearLevel) && yearLevel != "All")
                cmd.Parameters.AddWithValue("@yearLevel", yearLevel);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var subjectsStr = reader.IsDBNull(reader.GetOrdinal("Subjects")) ? "" : reader.GetString("Subjects");
                assignments.Add(new TeacherAssignment
                {
                    AssignmentID = reader.GetInt32("AssignmentID"),
                    TeacherID = reader.GetInt32("TeacherID"),
                    Section = reader.IsDBNull(reader.GetOrdinal("Section")) ? "" : reader.GetString("Section"),
                    Course = reader.IsDBNull(reader.GetOrdinal("Course")) ? "" : reader.GetString("Course"),
                    YearLevel = reader.IsDBNull(reader.GetOrdinal("YearLevel")) ? "" : reader.GetString("YearLevel"),
                    Subjects = string.IsNullOrEmpty(subjectsStr) ? new List<string>() : subjectsStr.Split(',').Select(s => s.Trim()).ToList()
                });
            }

            return assignments;
        }

        public static bool HasReportBeenSent(int teacherId, int evaluationId, int submissionId)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT COUNT(*) FROM report
                WHERE TeacherID = @teacherId
                  AND EvaluationID = @evaluationId
                  AND ReportData LIKE @submissionPattern", conn);

            cmd.Parameters.AddWithValue("@teacherId", teacherId);
            cmd.Parameters.AddWithValue("@evaluationId", evaluationId);
            cmd.Parameters.AddWithValue("@submissionPattern", $"%SubmissionID:{submissionId}%");

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static HashSet<int> GetSentSubmissionIds(int teacherId, int evaluationId)
        {
            var ids = new HashSet<int>();
            using var conn = Database.GetConnection();
            conn.Open();
            var cmd = new MySqlCommand(@"
                SELECT ReportData FROM report
                WHERE TeacherID = @teacherId AND EvaluationID = @evaluationId", conn);
            cmd.Parameters.AddWithValue("@teacherId", teacherId);
            cmd.Parameters.AddWithValue("@evaluationId", evaluationId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var data = reader.IsDBNull(0) ? "" : reader.GetString(0);
                var match = System.Text.RegularExpressions.Regex.Match(data, @"SubmissionID:(\d+)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out var sid))
                    ids.Add(sid);
            }
            return ids;
        }

        public static int GetReportIdForSubmission(int teacherId, int evaluationId, int submissionId)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            var cmd = new MySqlCommand(@"
                SELECT ReportID FROM report
                WHERE TeacherID = @teacherId
                  AND EvaluationID = @evaluationId
                  AND ReportData LIKE @submissionPattern
                LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@teacherId", teacherId);
            cmd.Parameters.AddWithValue("@evaluationId", evaluationId);
            cmd.Parameters.AddWithValue("@submissionPattern", $"%SubmissionID:{submissionId}%");
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        public static void SaveReport(int teacherId, int evaluationId, int? assignmentId, decimal avgScore, int responseCount, string reportData)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                INSERT INTO report (TeacherID, EvaluationID, AssignmentID, SubmissionDate, AverageScore, ResponseCount, ReportData)
                VALUES (@teacherId, @evaluationId, @assignmentId, NOW(), @avgScore, @responseCount, @reportData)", conn);

            cmd.Parameters.AddWithValue("@teacherId", teacherId);
            cmd.Parameters.AddWithValue("@evaluationId", evaluationId);
            cmd.Parameters.AddWithValue("@assignmentId", assignmentId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@avgScore", avgScore);
            cmd.Parameters.AddWithValue("@responseCount", responseCount);
            var rdParam = cmd.Parameters.Add("@reportData", MySqlDbType.LongText);
            rdParam.Value = reportData ?? string.Empty;

            cmd.ExecuteNonQuery();
        }

        public static List<TeacherReport> GetReportsForTeacher(int teacherId)
        {
            var reports = new List<TeacherReport>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT r.ReportID, r.TeacherID, r.EvaluationID, r.SubmissionDate,
                       r.AverageScore, r.ResponseCount, r.ReportData,
                       COALESCE(ef.Title, '') AS FormTitle,
                       c.Content AS CommentText, c.SystemLevel AS CommentLevel
                FROM report r
                LEFT JOIN EvaluationForm ef ON ef.EvaluationID = r.EvaluationID
                LEFT JOIN comment c ON c.SubmissionID = r.ReportID AND c.Status = 'Approved'
                WHERE r.TeacherID = @teacherId
                ORDER BY r.SubmissionDate DESC", conn);

            cmd.Parameters.AddWithValue("@teacherId", teacherId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var commentLevelStr = reader.IsDBNull(reader.GetOrdinal("CommentLevel")) ? null : reader.GetString("CommentLevel");
                reports.Add(new TeacherReport
                {
                    ReportID = reader.GetInt32("ReportID"),
                    TeacherID = reader.GetInt32("TeacherID"),
                    EvaluationID = reader.GetInt32("EvaluationID"),
                    FormTitle = reader.GetString("FormTitle"),
                    SubmissionDate = reader.GetDateTime("SubmissionDate"),
                    AverageScore = reader.IsDBNull(reader.GetOrdinal("AverageScore")) ? 0 : reader.GetDecimal("AverageScore"),
                    ResponseCount = reader.IsDBNull(reader.GetOrdinal("ResponseCount")) ? 0 : reader.GetInt32("ResponseCount"),
                    ReportData = reader.IsDBNull(reader.GetOrdinal("ReportData")) ? "" : reader.GetString("ReportData"),
                    CommentText = reader.IsDBNull(reader.GetOrdinal("CommentText")) ? null : reader.GetString("CommentText"),
                    CommentLevel = ParseCommentLevel(commentLevelStr)
                });
            }

            return reports;
        }

        private static CommentLevel? ParseCommentLevel(string? level) => level?.ToLower() switch
        {
            "mild" => EvaluaTeach.CommentLevel.Mild,
            "moderate" => EvaluaTeach.CommentLevel.Moderate,
            "severe" => EvaluaTeach.CommentLevel.Severe,
            "normal" => EvaluaTeach.CommentLevel.Normal,
            _ => null
        };

        public static List<TeacherReport> GetAllReports()
        {
            var reports = new List<TeacherReport>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT r.ReportID, r.TeacherID, r.EvaluationID, r.AssignmentID, r.SubmissionDate,
                       r.AverageScore, r.ResponseCount, r.ReportData,
                       COALESCE(ef.Title, '') AS FormTitle,
                       COALESCE(ef.Semester, '') AS Semester,
                       COALESCE(ef.SchoolYear, '') AS SchoolYear,
                       CONCAT(t.FirstName, ' ', t.LastName) AS TeacherName,
                       ta.Subjects, ta.Course, ta.YearLevel, ta.Section
                FROM report r
                LEFT JOIN EvaluationForm ef ON ef.EvaluationID = r.EvaluationID
                LEFT JOIN Teacher t ON t.TeacherID = r.TeacherID
                LEFT JOIN teacher_assignment ta ON ta.AssignmentID = r.AssignmentID
                ORDER BY r.SubmissionDate DESC", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var assignmentId = reader.IsDBNull(reader.GetOrdinal("AssignmentID")) ? (int?)null : reader.GetInt32("AssignmentID");
                var subjects = reader.IsDBNull(reader.GetOrdinal("Subjects")) ? "" : reader.GetString("Subjects");
                var course = reader.IsDBNull(reader.GetOrdinal("Course")) ? "" : reader.GetString("Course");
                var yearLevel = reader.IsDBNull(reader.GetOrdinal("YearLevel")) ? "" : reader.GetString("YearLevel");
                var section = reader.IsDBNull(reader.GetOrdinal("Section")) ? "" : reader.GetString("Section");
                
                reports.Add(new TeacherReport
                {
                    ReportID       = reader.GetInt32("ReportID"),
                    TeacherID      = reader.GetInt32("TeacherID"),
                    TeacherName    = reader.IsDBNull(reader.GetOrdinal("TeacherName")) ? "" : reader.GetString("TeacherName"),
                    EvaluationID   = reader.GetInt32("EvaluationID"),
                    FormTitle      = reader.GetString("FormTitle"),
                    SubmissionDate = reader.GetDateTime("SubmissionDate"),
                    AverageScore   = reader.IsDBNull(reader.GetOrdinal("AverageScore")) ? 0 : reader.GetDecimal("AverageScore"),
                    ResponseCount  = reader.IsDBNull(reader.GetOrdinal("ResponseCount")) ? 0 : reader.GetInt32("ResponseCount"),
                    ReportData     = reader.IsDBNull(reader.GetOrdinal("ReportData")) ? "" : reader.GetString("ReportData"),
                    Semester       = reader.IsDBNull(reader.GetOrdinal("Semester")) ? "" : reader.GetString("Semester"),
                    SchoolYear     = reader.IsDBNull(reader.GetOrdinal("SchoolYear")) ? "" : reader.GetString("SchoolYear"),
                    AssignmentID   = assignmentId,
                    SubjectName    = subjects,
                    Course         = course,
                    YearLevel      = yearLevel,
                    Section        = section
                });
            }

            return reports;
        }

        public static List<(string Label, string Semester, string SchoolYear, decimal AvgScore, int ResponseCount)> GetTeacherPerformanceBySemester(int teacherId)
        {
            var results = new List<(string, string, string, decimal, int)>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT ef.Semester, ef.SchoolYear,
                       AVG(CAST(sr.Answer AS DECIMAL(5,2))) AS AvgScore,
                       COUNT(DISTINCT fs.SubmissionID) AS ResponseCount
                FROM FormSubmission fs
                JOIN SurveyResponse sr ON sr.SubmissionID = fs.SubmissionID
                JOIN SurveyQuestion fq ON fq.QuestionID = sr.QuestionID AND fq.QuestionType = 'rating'
                JOIN EvaluationForm ef ON ef.EvaluationID = fs.EvaluationID
                WHERE fs.TeacherID = @teacherId
                  AND ef.Semester IS NOT NULL AND ef.Semester != ''
                  AND ef.SchoolYear IS NOT NULL AND ef.SchoolYear != ''
                  AND sr.Answer REGEXP '^[0-9]+(\.[0-9]+)?$'
                GROUP BY ef.SchoolYear, ef.Semester
                ORDER BY ef.SchoolYear DESC,
                         FIELD(ef.Semester, '2nd', '1st', 'Summer')", conn);
            cmd.Parameters.AddWithValue("@teacherId", teacherId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string sem = reader.IsDBNull(reader.GetOrdinal("Semester")) ? "" : reader.GetString("Semester");
                string sy = reader.IsDBNull(reader.GetOrdinal("SchoolYear")) ? "" : reader.GetString("SchoolYear");
                decimal avg = reader.IsDBNull(reader.GetOrdinal("AvgScore")) ? 0 : reader.GetDecimal("AvgScore");
                int count = reader.IsDBNull(reader.GetOrdinal("ResponseCount")) ? 0 : reader.GetInt32("ResponseCount");
                string label = $"{sem} Sem {sy}";
                results.Add((label, sem, sy, avg, count));
            }

            return results;
        }

        public static List<(string SubjectLabel, int AssignmentId, string Subjects, string Course, string YearLevel, string Section, decimal AvgScore, int ResponseCount)> 
            GetTeacherPerformanceBySubject(int teacherId)
        {
            var results = new List<(string, int, string, string, string, string, decimal, int)>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT ta.AssignmentID, ta.Subjects, ta.Course, ta.YearLevel, ta.Section,
                       AVG(CAST(sr.Answer AS DECIMAL(5,2))) AS AvgScore,
                       COUNT(DISTINCT fs.SubmissionID) AS ResponseCount
                FROM FormSubmission fs
                JOIN SurveyResponse sr ON sr.SubmissionID = fs.SubmissionID
                JOIN SurveyQuestion fq ON fq.QuestionID = sr.QuestionID AND fq.QuestionType = 'rating'
                JOIN teacher_assignment ta ON ta.AssignmentID = fs.AssignmentID
                WHERE fs.TeacherID = @teacherId
                  AND sr.Answer REGEXP '^[0-9]+(\.[0-9]+)?$'
                GROUP BY ta.AssignmentID, ta.Subjects, ta.Course, ta.YearLevel, ta.Section
                ORDER BY ResponseCount DESC, AvgScore DESC", conn);
            cmd.Parameters.AddWithValue("@teacherId", teacherId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int assignmentId = reader.GetInt32("AssignmentID");
                string subjects = reader.IsDBNull(reader.GetOrdinal("Subjects")) ? "" : reader.GetString("Subjects");
                string course = reader.IsDBNull(reader.GetOrdinal("Course")) ? "" : reader.GetString("Course");
                string yearLevel = reader.IsDBNull(reader.GetOrdinal("YearLevel")) ? "" : reader.GetString("YearLevel");
                string section = reader.IsDBNull(reader.GetOrdinal("Section")) ? "" : reader.GetString("Section");
                decimal avg = reader.IsDBNull(reader.GetOrdinal("AvgScore")) ? 0 : reader.GetDecimal("AvgScore");
                int count = reader.IsDBNull(reader.GetOrdinal("ResponseCount")) ? 0 : reader.GetInt32("ResponseCount");
                string label = $"{subjects} ({course} · Year {yearLevel} · {section})";
                results.Add((label, assignmentId, subjects, course, yearLevel, section, avg, count));
            }

            return results;
        }

        public static void DeleteReport(int reportId)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM report WHERE ReportID = @id", conn);
            cmd.Parameters.AddWithValue("@id", reportId);
            cmd.ExecuteNonQuery();
        }

        public static bool AuthenticateTeacher(string identifier, string hashedPassword, out Teacher? teacher)
        {
            teacher = null;
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT TeacherID, FirstName, LastName, Email, Department, Section, Course, YearLevel, Subjects
                FROM Teacher
                WHERE (Email = @id OR CAST(TeacherID AS CHAR) = @id) AND Password = @password
                LIMIT 1", conn);

            cmd.Parameters.AddWithValue("@id", identifier);
            cmd.Parameters.AddWithValue("@password", hashedPassword);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var subjectsStr = reader.IsDBNull(reader.GetOrdinal("Subjects")) ? "" : reader.GetString("Subjects");
                teacher = new Teacher
                {
                    TeacherID = reader.GetInt32("TeacherID"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email"),
                    Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? "" : reader.GetString("Department"),
                    Section = reader.IsDBNull(reader.GetOrdinal("Section")) ? "" : reader.GetString("Section"),
                    Course = reader.IsDBNull(reader.GetOrdinal("Course")) ? "" : reader.GetString("Course"),
                    YearLevel = reader.IsDBNull(reader.GetOrdinal("YearLevel")) ? "" : reader.GetString("YearLevel"),
                    Subjects = string.IsNullOrEmpty(subjectsStr) ? new List<string>() : subjectsStr.Split(',').Select(s => s.Trim()).ToList()
                };
                return true;
            }
            return false;
        }

        // Filter methods for student dashboard
        public static List<string> GetUniqueCourses()
        {
            var courses = new List<string>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT DISTINCT Course FROM teacher_assignment 
                WHERE Course IS NOT NULL AND Course != '' 
                ORDER BY Course", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                courses.Add(reader.GetString("Course"));
            }
            return courses;
        }

        public static List<string> GetUniqueYearLevels()
        {
            var years = new List<string>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT DISTINCT YearLevel FROM teacher_assignment 
                WHERE YearLevel IS NOT NULL AND YearLevel != '' 
                ORDER BY YearLevel", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                years.Add(reader.GetString("YearLevel"));
            }
            return years;
        }

        public static List<string> GetUniqueSections()
        {
            var sections = new List<string>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT DISTINCT Section FROM teacher_assignment 
                WHERE Section IS NOT NULL AND Section != '' 
                ORDER BY Section", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                sections.Add(reader.GetString("Section"));
            }
            return sections;
        }

        public static List<Teacher> GetFilteredTeachersForStudent(string? courseFilter, string? yearFilter, string? sectionFilter)
        {
            var teachers = new List<Teacher>();
            using var conn = Database.GetConnection();
            conn.Open();

            // Build dynamic query based on filters
            var sql = @"
                SELECT DISTINCT t.TeacherID, t.FirstName, t.LastName, t.Email, t.Department, t.Section, t.Course, t.YearLevel, t.Subjects, t.CreatedAt
                FROM Teacher t
                INNER JOIN teacher_assignment ta ON ta.TeacherID = t.TeacherID
                WHERE 1=1";

            var parameters = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(courseFilter) && courseFilter != "All")
            {
                sql += " AND ta.Course = @course";
                parameters.Add(new MySqlParameter("@course", courseFilter));
            }
            if (!string.IsNullOrEmpty(yearFilter) && yearFilter != "All")
            {
                sql += " AND ta.YearLevel = @yearLevel";
                parameters.Add(new MySqlParameter("@yearLevel", yearFilter));
            }
            if (!string.IsNullOrEmpty(sectionFilter) && sectionFilter != "All")
            {
                sql += " AND ta.Section = @section";
                parameters.Add(new MySqlParameter("@section", sectionFilter));
            }

            sql += " ORDER BY t.LastName, t.FirstName";

            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters.ToArray());

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var subjectsStr = reader.IsDBNull(reader.GetOrdinal("Subjects")) ? "" : reader.GetString("Subjects");
                    var teacherId = reader.GetInt32("TeacherID");
                    teachers.Add(new Teacher
                    {
                        TeacherID = teacherId,
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email"),
                        Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? "" : reader.GetString("Department"),
                        Section = reader.IsDBNull(reader.GetOrdinal("Section")) ? "" : reader.GetString("Section"),
                        Course = reader.IsDBNull(reader.GetOrdinal("Course")) ? "" : reader.GetString("Course"),
                        YearLevel = reader.IsDBNull(reader.GetOrdinal("YearLevel")) ? "" : reader.GetInt32("YearLevel").ToString(),
                        Subjects = string.IsNullOrEmpty(subjectsStr) ? new List<string>() : subjectsStr.Split(',').Select(s => s.Trim()).ToList(),
                        CreatedAt = reader.GetDateTime("CreatedAt"),
                        Assignments = new List<TeacherAssignment>()
                    });
                }
            }

            // Load matching assignments for each teacher based on current filters
            foreach (var teacher in teachers)
            {
                teacher.Assignments = GetMatchingAssignmentsForStudent(teacher.TeacherID, sectionFilter ?? "", courseFilter ?? "", yearFilter ?? "");
            }

            return teachers;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static string HashPasswordPublic(string password) => HashPassword(password);
    }
}
