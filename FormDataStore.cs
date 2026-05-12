using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace EvaluaTeach
{
    public static class FormDataStore
    {
        public static event Action? FormsUpdated;
        public static event Action? ResponsesUpdated;
        public static event Action? CommentsUpdated;

        // ==================== SEMESTER HELPERS ====================

        public static string GetCurrentSemester()
        {
            int month = DateTime.Now.Month;
            if (month >= 8 && month <= 12) return "1st";
            if (month >= 1 && month <= 5) return "2nd";
            return "Summer";
        }

        public static string GetCurrentSchoolYear()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            if (month >= 8)
                return $"{year}-{year + 1}";
            else
                return $"{year - 1}-{year}";
        }

        // ==================== FORMS ====================

        public static List<EvaluationForm> GetAllForms()
        {
            var forms = new List<EvaluationForm>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT EvaluationID, Title, Description, TargetCourse, 
                       DueDate, IsActive, DateCreated, CreatedBy, Semester, SchoolYear
                FROM EvaluationForm", conn);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    forms.Add(new EvaluationForm
                    {
                        Id = reader.GetInt32("EvaluationID"),
                        Title = reader.GetString("Title"),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                        TargetCourse = reader.IsDBNull(reader.GetOrdinal("TargetCourse")) ? "All" : reader.GetString("TargetCourse"),
                        DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? null : reader.GetDateTime("DueDate"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CreatedAt = reader.GetDateTime("DateCreated"),
                        CreatedById = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetInt32("CreatedBy"),
                        Semester = reader.IsDBNull(reader.GetOrdinal("Semester")) ? "" : reader.GetString("Semester"),
                        SchoolYear = reader.IsDBNull(reader.GetOrdinal("SchoolYear")) ? "" : reader.GetString("SchoolYear"),
                        Questions = new List<FormQuestion>()
                    });
                }
            }

            foreach (var form in forms)
            {
                form.Questions = GetQuestionsForForm(form.Id, conn);
            }

            return forms;
        }

        public static List<EvaluationForm> GetActiveForms()
        {
            return GetAllForms().Where(f => f.IsActive).OrderByDescending(f => f.CreatedAt).ToList();
        }

        public static List<EvaluationForm> GetFormsForStudent(string course)
        {
            string currentSemester = GetCurrentSemester();
            string currentSchoolYear = GetCurrentSchoolYear();
            return GetAllForms()
                .Where(f => f.IsActive &&
                    (f.TargetCourse == "All" || f.TargetCourse == course) &&
                    (string.IsNullOrEmpty(f.Semester) || string.IsNullOrEmpty(f.SchoolYear) ||
                     (f.Semester == currentSemester && f.SchoolYear == currentSchoolYear)))
                .OrderByDescending(f => f.CreatedAt)
                .ToList();
        }

        public static EvaluationForm? GetForm(int id)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT EvaluationID, Title, Description, TargetCourse, 
                       DueDate, IsActive, DateCreated, CreatedBy, Semester, SchoolYear
                FROM EvaluationForm WHERE EvaluationID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            EvaluationForm? form = null;
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    form = new EvaluationForm
                    {
                        Id = reader.GetInt32("EvaluationID"),
                        Title = reader.GetString("Title"),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                        TargetCourse = reader.IsDBNull(reader.GetOrdinal("TargetCourse")) ? "All" : reader.GetString("TargetCourse"),
                        DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? null : reader.GetDateTime("DueDate"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CreatedAt = reader.GetDateTime("DateCreated"),
                        CreatedById = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetInt32("CreatedBy"),
                        Semester = reader.IsDBNull(reader.GetOrdinal("Semester")) ? "" : reader.GetString("Semester"),
                        SchoolYear = reader.IsDBNull(reader.GetOrdinal("SchoolYear")) ? "" : reader.GetString("SchoolYear"),
                        Questions = new List<FormQuestion>()
                    };
                }
            }

            if (form != null)
            {
                form.Questions = GetQuestionsForForm(form.Id, conn);
            }
            return form;
        }

        private static List<FormQuestion> GetQuestionsForForm(int formId, MySqlConnection conn)
        {
            var questions = new List<FormQuestion>();
            var cmd = new MySqlCommand(@"
                SELECT QuestionID, QuestionText, QuestionType, OrderIndex, 
                       IsRequired, MinRating, MaxRating, Category
                FROM SurveyQuestion WHERE EvaluationID = @formId
                ORDER BY OrderIndex", conn);
            cmd.Parameters.AddWithValue("@formId", formId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    questions.Add(new FormQuestion
                    {
                        Id = reader.GetInt32("QuestionID"),
                        Text = reader.GetString("QuestionText"),
                        Type = ParseQuestionType(reader.GetString("QuestionType")),
                        OrderIndex = reader.GetInt32("OrderIndex"),
                        IsRequired = reader.GetBoolean("IsRequired"),
                        MinRating = reader.IsDBNull(reader.GetOrdinal("MinRating")) ? 1 : reader.GetInt32("MinRating"),
                        MaxRating = reader.IsDBNull(reader.GetOrdinal("MaxRating")) ? 5 : reader.GetInt32("MaxRating"),
                        Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? "General" : reader.GetString("Category"),
                        Options = new List<string>()
                    });
                }
            }

            // Load options for multiple choice questions
            foreach (var q in questions)
            {
                if (q.Type == QuestionType.MultipleChoice)
                {
                    q.Options = GetOptionsForQuestion(q.Id, conn);
                }
            }

            return questions;
        }

        private static List<string> GetOptionsForQuestion(int questionId, MySqlConnection conn)
        {
            var options = new List<string>();
            var cmd = new MySqlCommand(@"
                SELECT OptionText FROM QuestionOption 
                WHERE QuestionID = @questionId 
                ORDER BY OrderIndex", conn);
            cmd.Parameters.AddWithValue("@questionId", questionId);
            
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                options.Add(reader.GetString("OptionText"));
            }
            return options;
        }

        public static void AddForm(EvaluationForm form)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // Insert form (EvaluationID is AUTO_INCREMENT)
                var cmd = new MySqlCommand(@"
                    INSERT INTO EvaluationForm (Title, Description, TargetCourse, DueDate, IsActive, CreatedBy, Semester, SchoolYear)
                    VALUES (@title, @desc, @course, @dueDate, @isActive, @createdById, @semester, @schoolYear)", conn, tx);
                cmd.Parameters.AddWithValue("@title", form.Title);
                cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(form.Description) ? (object)DBNull.Value : form.Description);
                cmd.Parameters.AddWithValue("@course", form.TargetCourse ?? "All");
                cmd.Parameters.AddWithValue("@dueDate", form.DueDate.HasValue ? (object)form.DueDate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@isActive", form.IsActive);
                cmd.Parameters.AddWithValue("@createdById", form.CreatedById.HasValue ? (object)form.CreatedById.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@semester", string.IsNullOrEmpty(form.Semester) ? (object)DBNull.Value : form.Semester);
                cmd.Parameters.AddWithValue("@schoolYear", string.IsNullOrEmpty(form.SchoolYear) ? (object)DBNull.Value : form.SchoolYear);
                cmd.ExecuteNonQuery();

                // Get the new form ID
                form.Id = (int)cmd.LastInsertedId;

                // Insert questions
                foreach (var q in form.Questions)
                {
                    var qCmd = new MySqlCommand(@"
                        INSERT INTO SurveyQuestion (EvaluationID, QuestionText, QuestionType, OrderIndex, IsRequired, MinRating, MaxRating, Category)
                        VALUES (@formId, @text, @type, @orderIdx, @isRequired, @minRating, @maxRating, @category)", conn, tx);
                    qCmd.Parameters.AddWithValue("@formId", form.Id);
                    qCmd.Parameters.AddWithValue("@text", q.Text);
                    qCmd.Parameters.AddWithValue("@type", q.Type.ToString().ToLower());
                    qCmd.Parameters.AddWithValue("@orderIdx", q.OrderIndex);
                    qCmd.Parameters.AddWithValue("@isRequired", q.IsRequired);
                    qCmd.Parameters.AddWithValue("@minRating", q.MinRating.HasValue ? (object)q.MinRating.Value : DBNull.Value);
                    qCmd.Parameters.AddWithValue("@maxRating", q.MaxRating.HasValue ? (object)q.MaxRating.Value : DBNull.Value);
                    qCmd.Parameters.AddWithValue("@category", string.IsNullOrEmpty(q.Category) ? "General" : q.Category);
                    qCmd.ExecuteNonQuery();
                    q.Id = (int)qCmd.LastInsertedId;

                    // Insert options for multiple choice questions
                    if (q.Options != null && q.Options.Count > 0)
                    {
                        foreach (var optionText in q.Options)
                        {
                            var optCmd = new MySqlCommand(@"
                                INSERT INTO QuestionOption (QuestionID, OptionText, OrderIndex)
                                VALUES (@questionId, @optionText, @orderIdx)", conn, tx);
                            optCmd.Parameters.AddWithValue("@questionId", q.Id);
                            optCmd.Parameters.AddWithValue("@optionText", optionText);
                            optCmd.Parameters.AddWithValue("@orderIdx", q.Options.IndexOf(optionText));
                            optCmd.ExecuteNonQuery();
                        }
                    }
                }

                tx.Commit();
                FormsUpdated?.Invoke();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public static void UpdateForm(EvaluationForm form)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // Update form metadata
                var cmd = new MySqlCommand(@"
                    UPDATE EvaluationForm 
                    SET Title = @title, Description = @desc, TargetCourse = @course, 
                        DueDate = @dueDate, IsActive = @isActive,
                        Semester = @semester, SchoolYear = @schoolYear
                    WHERE EvaluationID = @id", conn, tx);
                cmd.Parameters.AddWithValue("@id", form.Id);
                cmd.Parameters.AddWithValue("@title", form.Title);
                cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(form.Description) ? (object)DBNull.Value : form.Description);
                cmd.Parameters.AddWithValue("@course", form.TargetCourse ?? "All");
                cmd.Parameters.AddWithValue("@dueDate", form.DueDate.HasValue ? (object)form.DueDate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@isActive", form.IsActive);
                cmd.Parameters.AddWithValue("@semester", string.IsNullOrEmpty(form.Semester) ? (object)DBNull.Value : form.Semester);
                cmd.Parameters.AddWithValue("@schoolYear", string.IsNullOrEmpty(form.SchoolYear) ? (object)DBNull.Value : form.SchoolYear);
                cmd.ExecuteNonQuery();

                // First delete responses and submissions for this form (to satisfy FKs)
                var deleteResponsesCmd = new MySqlCommand(@"
                    DELETE sr FROM SurveyResponse sr
                    INNER JOIN SurveyQuestion sq ON sr.QuestionID = sq.QuestionID
                    WHERE sq.EvaluationID = @formId", conn, tx);
                deleteResponsesCmd.Parameters.AddWithValue("@formId", form.Id);
                deleteResponsesCmd.ExecuteNonQuery();

                // Delete form submissions
                var deleteSubmissionsCmd = new MySqlCommand(@"
                    DELETE FROM FormSubmission WHERE EvaluationID = @formId", conn, tx);
                deleteSubmissionsCmd.Parameters.AddWithValue("@formId", form.Id);
                deleteSubmissionsCmd.ExecuteNonQuery();

                // Delete existing questions (cascades to options via FK)
                var deleteCmd = new MySqlCommand(@"
                    DELETE FROM SurveyQuestion WHERE EvaluationID = @formId", conn, tx);
                deleteCmd.Parameters.AddWithValue("@formId", form.Id);
                deleteCmd.ExecuteNonQuery();

                // Re-insert all questions with new IDs
                foreach (var q in form.Questions)
                {
                    var qCmd = new MySqlCommand(@"
                        INSERT INTO SurveyQuestion (EvaluationID, QuestionText, QuestionType, OrderIndex, IsRequired, MinRating, MaxRating, Category)
                        VALUES (@formId, @text, @type, @orderIdx, @isRequired, @minRating, @maxRating, @category)", conn, tx);
                    qCmd.Parameters.AddWithValue("@formId", form.Id);
                    qCmd.Parameters.AddWithValue("@text", q.Text);
                    qCmd.Parameters.AddWithValue("@type", q.Type.ToString().ToLower());
                    qCmd.Parameters.AddWithValue("@orderIdx", q.OrderIndex);
                    qCmd.Parameters.AddWithValue("@isRequired", q.IsRequired);
                    qCmd.Parameters.AddWithValue("@minRating", q.MinRating.HasValue ? (object)q.MinRating.Value : DBNull.Value);
                    qCmd.Parameters.AddWithValue("@maxRating", q.MaxRating.HasValue ? (object)q.MaxRating.Value : DBNull.Value);
                    qCmd.Parameters.AddWithValue("@category", string.IsNullOrEmpty(q.Category) ? "General" : q.Category);
                    qCmd.ExecuteNonQuery();
                    q.Id = (int)qCmd.LastInsertedId;

                    // Insert options for multiple choice questions
                    if (q.Options != null && q.Options.Count > 0)
                    {
                        foreach (var optionText in q.Options)
                        {
                            var optCmd = new MySqlCommand(@"
                                INSERT INTO QuestionOption (QuestionID, OptionText, OrderIndex)
                                VALUES (@questionId, @optionText, @orderIdx)", conn, tx);
                            optCmd.Parameters.AddWithValue("@questionId", q.Id);
                            optCmd.Parameters.AddWithValue("@optionText", optionText);
                            optCmd.Parameters.AddWithValue("@orderIdx", q.Options.IndexOf(optionText));
                            optCmd.ExecuteNonQuery();
                        }
                    }
                }

                tx.Commit();
                FormsUpdated?.Invoke();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public static void DeleteForm(int id)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // Delete form responses first (they reference form submissions)
                var deleteResponses = new MySqlCommand(@"
                    DELETE sr FROM surveyresponse sr
                    INNER JOIN formsubmission fs ON sr.SubmissionID = fs.SubmissionID
                    WHERE fs.EvaluationID = @id", conn, tx);
                deleteResponses.Parameters.AddWithValue("@id", id);
                deleteResponses.ExecuteNonQuery();

                // Delete form submissions
                var deleteSubmissions = new MySqlCommand(
                    "DELETE FROM FormSubmission WHERE EvaluationID = @id", conn, tx);
                deleteSubmissions.Parameters.AddWithValue("@id", id);
                deleteSubmissions.ExecuteNonQuery();

                // Delete comments referencing this form
                var deleteComments = new MySqlCommand(
                    "DELETE FROM Comment WHERE FormTitle IN (SELECT Title FROM EvaluationForm WHERE EvaluationID = @id)", conn, tx);
                deleteComments.Parameters.AddWithValue("@id", id);
                deleteComments.ExecuteNonQuery();

                // Delete form questions
                var deleteQuestions = new MySqlCommand(
                    "DELETE FROM surveyquestion WHERE EvaluationID = @id", conn, tx);
                deleteQuestions.Parameters.AddWithValue("@id", id);
                deleteQuestions.ExecuteNonQuery();

                // Finally delete the form
                var deleteForm = new MySqlCommand(
                    "DELETE FROM EvaluationForm WHERE EvaluationID = @id", conn, tx);
                deleteForm.Parameters.AddWithValue("@id", id);
                deleteForm.ExecuteNonQuery();

                tx.Commit();
                FormsUpdated?.Invoke();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // ==================== RESPONSES ====================

        public static void AddResponse(FormResponse response)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                var subCmd = new MySqlCommand(@"
                    INSERT INTO FormSubmission (StudentIDNumber, EvaluationID, TeacherID, SubmittedAt)
                    VALUES (@studentId, @formId, @teacherId, @submittedAt)", conn, tx);
                subCmd.Parameters.AddWithValue("@studentId", response.StudentId);
                subCmd.Parameters.AddWithValue("@formId", response.FormId);
                subCmd.Parameters.AddWithValue("@teacherId", response.TeacherId > 0 ? response.TeacherId : (object)DBNull.Value);
                subCmd.Parameters.AddWithValue("@submittedAt", response.SubmittedAt);
                subCmd.ExecuteNonQuery();

                var submissionId = (int)subCmd.LastInsertedId;
                response.Id = submissionId;

                foreach (var answer in response.Answers)
                {
                    var respCmd = new MySqlCommand(@"
                        INSERT INTO SurveyResponse (SubmissionID, QuestionID, Answer)
                        VALUES (@subId, @qid, @answer)", conn, tx);
                    respCmd.Parameters.AddWithValue("@subId", submissionId);
                    respCmd.Parameters.AddWithValue("@qid", answer.Key);
                    respCmd.Parameters.AddWithValue("@answer", answer.Value);
                    respCmd.ExecuteNonQuery();
                }

                tx.Commit();
                ResponsesUpdated?.Invoke();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public static List<FormResponse> GetResponsesForForm(int formId)
        {
            var responses = new List<FormResponse>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT fs.SubmissionID, fs.StudentIDNumber, fs.EvaluationID, fs.TeacherID, fs.SubmittedAt,
                       CONCAT(t.FirstName, ' ', t.LastName) as TeacherName,
                       CONCAT(s.FirstName, ' ', s.LastName) as StudentName,
                       s.Avatar
                FROM FormSubmission fs
                LEFT JOIN Teacher t ON fs.TeacherID = t.TeacherID
                LEFT JOIN student s ON s.IDNumber = fs.StudentIDNumber
                WHERE fs.EvaluationID = @formId", conn);
            cmd.Parameters.AddWithValue("@formId", formId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var teacherId = reader.IsDBNull(reader.GetOrdinal("TeacherID")) ? 0 : reader.GetInt32("TeacherID");
                    var teacherName = reader.IsDBNull(reader.GetOrdinal("TeacherName")) ? "" : reader.GetString("TeacherName");
                    var studentName = reader.IsDBNull(reader.GetOrdinal("StudentName")) ? "" : reader.GetString("StudentName");
                    var avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : (byte[])reader["Avatar"];
                    responses.Add(new FormResponse
                    {
                        Id = reader.GetInt32("SubmissionID"),
                        FormId = reader.GetInt32("EvaluationID"),
                        TeacherId = teacherId,
                        TeacherName = teacherName,
                        StudentId = reader.GetString("StudentIDNumber"),
                        StudentName = studentName,
                        Avatar = avatar,
                        SubmittedAt = reader.GetDateTime("SubmittedAt"),
                        Answers = new Dictionary<int, string>()
                    });
                }
            }

            foreach (var response in responses)
            {
                response.Answers = GetAnswersForSubmission(response.Id, conn);
            }

            return responses;
        }

        private static Dictionary<int, string> GetAnswersForSubmission(int submissionId, MySqlConnection conn)
        {
            var answers = new Dictionary<int, string>();
            var cmd = new MySqlCommand(@"
                SELECT QuestionID, Answer
                FROM SurveyResponse
                WHERE SubmissionID = @subId", conn);
            cmd.Parameters.AddWithValue("@subId", submissionId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var qid = reader.GetInt32("QuestionID");
                var answer = reader.IsDBNull(reader.GetOrdinal("Answer")) ? "" : reader.GetString("Answer");
                answers[qid] = answer;
            }
            return answers;
        }

        public static List<FormResponse> GetResponsesByStudent(string studentId)
        {
            var responses = new List<FormResponse>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT SubmissionID, EvaluationID, SubmittedAt
                FROM FormSubmission
                WHERE StudentIDNumber = @studentId", conn);
            cmd.Parameters.AddWithValue("@studentId", studentId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                responses.Add(new FormResponse
                {
                    Id = reader.GetInt32("SubmissionID"),
                    FormId = reader.GetInt32("EvaluationID"),
                    StudentId = studentId,
                    SubmittedAt = reader.GetDateTime("SubmittedAt"),
                    Answers = new Dictionary<int, string>()
                });
            }
            return responses;
        }

        public static bool HasStudentSubmitted(int formId, string studentId, int teacherId = 0)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string sql;
            MySqlCommand cmd;
            
            if (teacherId > 0)
            {
                // When checking for a specific teacher, match that teacher OR match submissions
                // where the form submission was made before teacher tracking was implemented (NULL)
                // BUT we need to also check if there's a specific submission for this teacher
                sql = @"
                    SELECT COUNT(*) FROM FormSubmission 
                    WHERE EvaluationID = @formId AND StudentIDNumber = @studentId
                    AND (TeacherID = @teacherId OR TeacherID IS NULL)
                    AND NOT EXISTS (
                        SELECT 1 FROM FormSubmission fs2 
                        WHERE fs2.EvaluationID = @formId 
                        AND fs2.StudentIDNumber = @studentId 
                        AND fs2.TeacherID = @teacherId
                    )";
                
                // Actually simpler: if there's a submission for this specific teacher, it's completed
                // If there's a NULL submission and no specific teacher submission, consider it completed for backward compat
                sql = @"
                    SELECT COUNT(*) FROM FormSubmission 
                    WHERE EvaluationID = @formId AND StudentIDNumber = @studentId
                    AND (
                        TeacherID = @teacherId 
                        OR (TeacherID IS NULL AND NOT EXISTS (
                            SELECT 1 FROM FormSubmission fs2 
                            WHERE fs2.EvaluationID = @formId 
                            AND fs2.StudentIDNumber = @studentId 
                            AND fs2.TeacherID = @teacherId
                        ))
                    )";
            }
            else
            {
                // No teacher specified - check for any submission by this student for this form
                sql = @"
                    SELECT COUNT(*) FROM FormSubmission 
                    WHERE EvaluationID = @formId AND StudentIDNumber = @studentId";
            }

            cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@formId", formId);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            if (teacherId > 0)
            {
                cmd.Parameters.AddWithValue("@teacherId", teacherId);
            }

            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public static int GetSubmissionCount(int formId)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM FormSubmission WHERE EvaluationID = @formId", conn);
            cmd.Parameters.AddWithValue("@formId", formId);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ==================== COMMENTS ====================

        public static void AddComment(FormComment comment)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            // Check if comment already exists for this submission
            // Note: A student can evaluate the same form for different teachers,
            // so we check by SubmissionID (which is unique per student+form+teacher)
            int? existingCommentId = null;
            if (comment.SubmissionId > 0)
            {
                var checkCmd = new MySqlCommand(
                    "SELECT CommentID FROM comment WHERE SubmissionID = @submissionId LIMIT 1", conn);
                checkCmd.Parameters.AddWithValue("@submissionId", comment.SubmissionId);
                var result = checkCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    existingCommentId = Convert.ToInt32(result);
                }
            }
            
            // If no comment found by SubmissionID, also check by StudentID + FormTitle
            // This handles cases where submission ID might not be set correctly
            if (!existingCommentId.HasValue && comment.StudentDbId.HasValue && !string.IsNullOrEmpty(comment.FormTitle))
            {
                var checkByFormCmd = new MySqlCommand(@"
                    SELECT c.CommentID 
                    FROM comment c
                    INNER JOIN FormSubmission fs ON fs.SubmissionID = c.SubmissionID
                    WHERE c.StudentID = @studentId 
                    AND c.FormTitle = @formTitle
                    AND fs.TeacherID = @teacherId
                    LIMIT 1", conn);
                checkByFormCmd.Parameters.AddWithValue("@studentId", comment.StudentDbId.Value);
                checkByFormCmd.Parameters.AddWithValue("@formTitle", comment.FormTitle);
                checkByFormCmd.Parameters.AddWithValue("@teacherId", 
                    comment.TeacherId > 0 ? comment.TeacherId : (object)DBNull.Value);
                var result2 = checkByFormCmd.ExecuteScalar();
                if (result2 != null && result2 != DBNull.Value)
                {
                    existingCommentId = Convert.ToInt32(result2);
                }
            }

            if (existingCommentId.HasValue)
            {
                // Update existing comment
                var updateCmd = new MySqlCommand(@"
                    UPDATE comment
                    SET Content = @content,
                        DateSubmitted = @dateSubmitted,
                        Status = @status,
                        SystemLevel = @systemLevel,
                        FormTitle = @formTitle
                    WHERE CommentID = @commentId", conn);
                updateCmd.Parameters.AddWithValue("@commentId", existingCommentId.Value);
                updateCmd.Parameters.AddWithValue("@content", comment.CommentText);
                updateCmd.Parameters.AddWithValue("@dateSubmitted", comment.SubmittedAt);
                updateCmd.Parameters.AddWithValue("@status", comment.Status.ToString());
                updateCmd.Parameters.AddWithValue("@systemLevel", comment.SystemLevel.ToString());
                updateCmd.Parameters.AddWithValue("@formTitle", (object?)comment.FormTitle ?? DBNull.Value);
                updateCmd.ExecuteNonQuery();

                comment.Id = existingCommentId.Value;
            }
            else
            {
                // Insert new comment
                var cmd = new MySqlCommand(@"
                    INSERT INTO comment
                        (StudentID, TeacherID, Content, DateSubmitted, Status,
                         SubmissionID, FormTitle, SystemLevel)
                    VALUES
                        (@studentId, @teacherId, @content, @dateSubmitted, @status,
                         @submissionId, @formTitle, @systemLevel)", conn);
                cmd.Parameters.AddWithValue("@studentId",
                    comment.StudentDbId.HasValue ? (object)comment.StudentDbId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@teacherId", 
                    comment.TeacherId > 0 ? (object)comment.TeacherId : DBNull.Value);
                cmd.Parameters.AddWithValue("@content", comment.CommentText);
                cmd.Parameters.AddWithValue("@dateSubmitted", comment.SubmittedAt);
                cmd.Parameters.AddWithValue("@status", comment.Status.ToString());
                cmd.Parameters.AddWithValue("@submissionId",
                    comment.SubmissionId > 0 ? (object)comment.SubmissionId : DBNull.Value);
                cmd.Parameters.AddWithValue("@formTitle", (object?)comment.FormTitle ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@systemLevel", comment.SystemLevel.ToString());
                cmd.ExecuteNonQuery();

                comment.Id = (int)cmd.LastInsertedId;
            }

            CommentsUpdated?.Invoke();
        }

        public static void UpdateCommentStatus(int commentId, CommentStatus status, CommentLevel adminLevel, string reviewedBy)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                UPDATE comment
                SET Status = @status, AdminLevel = @adminLevel,
                    ReviewedAt = @reviewedAt, ReviewedBy = @reviewedBy
                WHERE CommentID = @id", conn);
            cmd.Parameters.AddWithValue("@id", commentId);
            cmd.Parameters.AddWithValue("@status", status.ToString());
            cmd.Parameters.AddWithValue("@adminLevel", adminLevel.ToString());
            cmd.Parameters.AddWithValue("@reviewedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@reviewedBy", reviewedBy);
            cmd.ExecuteNonQuery();

            CommentsUpdated?.Invoke();
        }

        public static FormComment? GetCommentForSubmission(int submissionId)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT c.CommentID, c.StudentID, c.Content, c.DateSubmitted, c.Status,
                       c.SubmissionID, c.FormTitle, c.SystemLevel, c.AdminLevel,
                       c.ReviewedAt, c.ReviewedBy,
                       s.IDNumber, s.FirstName, s.LastName, s.Email
                FROM comment c
                LEFT JOIN student s ON s.StudentID = c.StudentID
                WHERE c.SubmissionID = @subId
                LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@subId", submissionId);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var adminLevelStr = reader.IsDBNull(reader.GetOrdinal("AdminLevel")) ? null : reader.GetString("AdminLevel");
            var firstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? "" : reader.GetString("FirstName");
            var lastName  = reader.IsDBNull(reader.GetOrdinal("LastName"))  ? "" : reader.GetString("LastName");
            var idNumber  = reader.IsDBNull(reader.GetOrdinal("IDNumber"))  ? "" : reader.GetString("IDNumber");
            var email     = reader.IsDBNull(reader.GetOrdinal("Email"))     ? "" : reader.GetString("Email");
            int? dbStudentId = reader.IsDBNull(reader.GetOrdinal("StudentID")) ? null : reader.GetInt32("StudentID");

            return new FormComment
            {
                Id = reader.GetInt32("CommentID"),
                StudentDbId = dbStudentId,
                SubmissionId = submissionId,
                StudentId = idNumber,
                StudentName = $"{firstName} {lastName}".Trim(),
                StudentEmail = email,
                FormTitle = reader.IsDBNull(reader.GetOrdinal("FormTitle")) ? "" : reader.GetString("FormTitle"),
                CommentText = reader.IsDBNull(reader.GetOrdinal("Content")) ? "" : reader.GetString("Content"),
                SystemLevel = reader.IsDBNull(reader.GetOrdinal("SystemLevel")) ? CommentLevel.Normal : ParseCommentLevel(reader.GetString("SystemLevel")),
                AdminLevel = adminLevelStr == null ? null : ParseCommentLevel(adminLevelStr),
                Status = ParseCommentStatus(reader.GetString("Status")),
                SubmittedAt = reader.IsDBNull(reader.GetOrdinal("DateSubmitted")) ? DateTime.Now : reader.GetDateTime("DateSubmitted"),
                ReviewedAt = reader.IsDBNull(reader.GetOrdinal("ReviewedAt")) ? null : reader.GetDateTime("ReviewedAt"),
                ReviewedBy = reader.IsDBNull(reader.GetOrdinal("ReviewedBy")) ? "" : reader.GetString("ReviewedBy")
            };
        }

        public static List<FormComment> GetApprovedCommentsForSubmissions(IEnumerable<int> submissionIds)
        {
            var ids = submissionIds.ToList();
            if (!ids.Any()) return new List<FormComment>();

            var comments = new List<FormComment>();
            using var conn = Database.GetConnection();
            conn.Open();

            var inClause = string.Join(",", ids.Select((_, i) => $"@id{i}"));
            var cmd = new MySqlCommand($@"
                SELECT c.CommentID, c.StudentID, c.Content, c.DateSubmitted, c.Status,
                       c.SubmissionID, c.FormTitle, c.SystemLevel, c.AdminLevel,
                       c.ReviewedAt, c.ReviewedBy,
                       s.IDNumber, s.FirstName, s.LastName
                FROM comment c
                LEFT JOIN student s ON s.StudentID = c.StudentID
                WHERE c.SubmissionID IN ({inClause})
                  AND c.Status = 'Approved'
                ORDER BY c.DateSubmitted DESC", conn);

            for (int i = 0; i < ids.Count; i++)
                cmd.Parameters.AddWithValue($"@id{i}", ids[i]);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var adminLevelStr = reader.IsDBNull(reader.GetOrdinal("AdminLevel")) ? null : reader.GetString("AdminLevel");
                var firstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? "" : reader.GetString("FirstName");
                var lastName  = reader.IsDBNull(reader.GetOrdinal("LastName"))  ? "" : reader.GetString("LastName");
                var idNumber  = reader.IsDBNull(reader.GetOrdinal("IDNumber"))  ? "" : reader.GetString("IDNumber");
                comments.Add(new FormComment
                {
                    Id           = reader.GetInt32("CommentID"),
                    SubmissionId = reader.GetInt32("SubmissionID"),
                    StudentId    = idNumber,
                    StudentName  = $"{firstName} {lastName}".Trim(),
                    FormTitle    = reader.IsDBNull(reader.GetOrdinal("FormTitle")) ? "" : reader.GetString("FormTitle"),
                    CommentText  = reader.IsDBNull(reader.GetOrdinal("Content")) ? "" : reader.GetString("Content"),
                    SystemLevel  = reader.IsDBNull(reader.GetOrdinal("SystemLevel")) ? CommentLevel.Normal : ParseCommentLevel(reader.GetString("SystemLevel")),
                    AdminLevel   = adminLevelStr == null ? null : ParseCommentLevel(adminLevelStr),
                    Status       = CommentStatus.Approved,
                    SubmittedAt  = reader.IsDBNull(reader.GetOrdinal("DateSubmitted")) ? DateTime.Now : reader.GetDateTime("DateSubmitted")
                });
            }
            return comments;
        }

        public static List<FormComment> GetPendingComments()
        {
            return GetCommentsByStatus("Pending");
        }

        public static List<FormComment> GetAllComments()
        {
            return GetCommentsByStatus(null);
        }

        private static List<FormComment> GetCommentsByStatus(string? status)
        {
            var comments = new List<FormComment>();
            using var conn = Database.GetConnection();
            conn.Open();

            string sql = @"
                SELECT c.CommentID, c.StudentID, c.Content, c.DateSubmitted, c.Status,
                       c.SubmissionID, c.FormTitle, c.SystemLevel, c.AdminLevel,
                       c.ReviewedAt, c.ReviewedBy,
                       s.IDNumber, s.FirstName, s.LastName, s.Email
                FROM comment c
                LEFT JOIN student s ON s.StudentID = c.StudentID";

            if (status != null)
                sql += " WHERE c.Status = @status";

            sql += " ORDER BY c.DateSubmitted DESC";

            var cmd = new MySqlCommand(sql, conn);
            if (status != null)
                cmd.Parameters.AddWithValue("@status", status);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var adminLevelStr = reader.IsDBNull(reader.GetOrdinal("AdminLevel")) ? null : reader.GetString("AdminLevel");
                var firstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? "" : reader.GetString("FirstName");
                var lastName  = reader.IsDBNull(reader.GetOrdinal("LastName"))  ? "" : reader.GetString("LastName");
                var idNumber  = reader.IsDBNull(reader.GetOrdinal("IDNumber"))  ? "" : reader.GetString("IDNumber");
                var email     = reader.IsDBNull(reader.GetOrdinal("Email"))     ? "" : reader.GetString("Email");
                int? dbStudentId = reader.IsDBNull(reader.GetOrdinal("StudentID")) ? null : reader.GetInt32("StudentID");

                comments.Add(new FormComment
                {
                    Id = reader.GetInt32("CommentID"),
                    StudentDbId = dbStudentId,
                    SubmissionId = reader.IsDBNull(reader.GetOrdinal("SubmissionID")) ? 0 : reader.GetInt32("SubmissionID"),
                    StudentId = idNumber,
                    StudentName = $"{firstName} {lastName}".Trim(),
                    StudentEmail = email,
                    FormTitle = reader.IsDBNull(reader.GetOrdinal("FormTitle")) ? "" : reader.GetString("FormTitle"),
                    CommentText = reader.IsDBNull(reader.GetOrdinal("Content")) ? "" : reader.GetString("Content"),
                    SystemLevel = reader.IsDBNull(reader.GetOrdinal("SystemLevel")) ? CommentLevel.Normal : ParseCommentLevel(reader.GetString("SystemLevel")),
                    AdminLevel = adminLevelStr == null ? null : ParseCommentLevel(adminLevelStr),
                    Status = ParseCommentStatus(reader.GetString("Status")),
                    SubmittedAt = reader.IsDBNull(reader.GetOrdinal("DateSubmitted")) ? DateTime.Now : reader.GetDateTime("DateSubmitted"),
                    ReviewedAt = reader.IsDBNull(reader.GetOrdinal("ReviewedAt")) ? null : reader.GetDateTime("ReviewedAt"),
                    ReviewedBy = reader.IsDBNull(reader.GetOrdinal("ReviewedBy")) ? "" : reader.GetString("ReviewedBy")
                });
            }
            return comments;
        }

        public static int GetPendingCommentCount()
        {
            using var conn = Database.GetConnection();
            conn.Open();
            var cmd = new MySqlCommand("SELECT COUNT(*) FROM comment WHERE Status = 'Pending'", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private static CommentLevel ParseCommentLevel(string level) => level?.ToLower() switch
        {
            "mild"     => CommentLevel.Mild,
            "moderate" => CommentLevel.Moderate,
            "severe"   => CommentLevel.Severe,
            _          => CommentLevel.Normal
        };

        private static CommentStatus ParseCommentStatus(string status) => status?.ToLower() switch
        {
            "approved" => CommentStatus.Approved,
            "rejected" => CommentStatus.Rejected,
            _          => CommentStatus.Pending
        };

        // ==================== HELPERS ====================

        private static QuestionType ParseQuestionType(string type)
        {
            return type?.ToLower() switch
            {
                "text" => QuestionType.Text,
                "yesno" => QuestionType.YesNo,
                "multiplechoice" => QuestionType.MultipleChoice,
                _ => QuestionType.Rating
            };
        }

    }
}
