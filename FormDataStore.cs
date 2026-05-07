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

        // ==================== FORMS ====================

        public static List<EvaluationForm> GetAllForms()
        {
            var forms = new List<EvaluationForm>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT EvaluationID, Title, Description, TargetCourse, 
                       DueDate, IsActive, DateCreated, CreatedBy
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
            return GetAllForms()
                .Where(f => f.IsActive &&
                    (f.TargetCourse == "All" || f.TargetCourse == course))
                .OrderByDescending(f => f.CreatedAt)
                .ToList();
        }

        public static EvaluationForm? GetForm(int id)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT EvaluationID, Title, Description, TargetCourse, 
                       DueDate, IsActive, DateCreated, CreatedBy
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
                       IsRequired, MinRating, MaxRating
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
                    INSERT INTO EvaluationForm (Title, Description, TargetCourse, DueDate, IsActive, CreatedBy)
                    VALUES (@title, @desc, @course, @dueDate, @isActive, @createdById)", conn, tx);
                cmd.Parameters.AddWithValue("@title", form.Title);
                cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(form.Description) ? (object)DBNull.Value : form.Description);
                cmd.Parameters.AddWithValue("@course", form.TargetCourse ?? "All");
                cmd.Parameters.AddWithValue("@dueDate", form.DueDate.HasValue ? (object)form.DueDate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@isActive", form.IsActive);
                cmd.Parameters.AddWithValue("@createdById", form.CreatedById.HasValue ? (object)form.CreatedById.Value : DBNull.Value);
                cmd.ExecuteNonQuery();

                // Get the new form ID
                form.Id = (int)cmd.LastInsertedId;

                // Insert questions
                foreach (var q in form.Questions)
                {
                    var qCmd = new MySqlCommand(@"
                        INSERT INTO SurveyQuestion (EvaluationID, QuestionText, QuestionType, OrderIndex, IsRequired, MinRating, MaxRating)
                        VALUES (@formId, @text, @type, @orderIdx, @isRequired, @minRating, @maxRating)", conn, tx);
                    qCmd.Parameters.AddWithValue("@formId", form.Id);
                    qCmd.Parameters.AddWithValue("@text", q.Text);
                    qCmd.Parameters.AddWithValue("@type", q.Type.ToString().ToLower());
                    qCmd.Parameters.AddWithValue("@orderIdx", q.OrderIndex);
                    qCmd.Parameters.AddWithValue("@isRequired", q.IsRequired);
                    qCmd.Parameters.AddWithValue("@minRating", q.MinRating.HasValue ? (object)q.MinRating.Value : DBNull.Value);
                    qCmd.Parameters.AddWithValue("@maxRating", q.MaxRating.HasValue ? (object)q.MaxRating.Value : DBNull.Value);
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
                        DueDate = @dueDate, IsActive = @isActive
                    WHERE EvaluationID = @id", conn, tx);
                cmd.Parameters.AddWithValue("@id", form.Id);
                cmd.Parameters.AddWithValue("@title", form.Title);
                cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(form.Description) ? (object)DBNull.Value : form.Description);
                cmd.Parameters.AddWithValue("@course", form.TargetCourse ?? "All");
                cmd.Parameters.AddWithValue("@dueDate", form.DueDate.HasValue ? (object)form.DueDate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@isActive", form.IsActive);
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
                        INSERT INTO SurveyQuestion (EvaluationID, QuestionText, QuestionType, OrderIndex, IsRequired, MinRating, MaxRating)
                        VALUES (@formId, @text, @type, @orderIdx, @isRequired, @minRating, @maxRating)", conn, tx);
                    qCmd.Parameters.AddWithValue("@formId", form.Id);
                    qCmd.Parameters.AddWithValue("@text", q.Text);
                    qCmd.Parameters.AddWithValue("@type", q.Type.ToString().ToLower());
                    qCmd.Parameters.AddWithValue("@orderIdx", q.OrderIndex);
                    qCmd.Parameters.AddWithValue("@isRequired", q.IsRequired);
                    qCmd.Parameters.AddWithValue("@minRating", q.MinRating.HasValue ? (object)q.MinRating.Value : DBNull.Value);
                    qCmd.Parameters.AddWithValue("@maxRating", q.MaxRating.HasValue ? (object)q.MaxRating.Value : DBNull.Value);
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

            var cmd = new MySqlCommand("DELETE FROM EvaluationForm WHERE EvaluationID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            FormsUpdated?.Invoke();
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
                    INSERT INTO FormSubmission (StudentIDNumber, EvaluationID, SubmittedAt)
                    VALUES (@studentId, @formId, @submittedAt)", conn, tx);
                subCmd.Parameters.AddWithValue("@studentId", response.StudentId);
                subCmd.Parameters.AddWithValue("@formId", response.FormId);
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
                SELECT SubmissionID, StudentIDNumber, EvaluationID, SubmittedAt
                FROM FormSubmission
                WHERE EvaluationID = @formId", conn);
            cmd.Parameters.AddWithValue("@formId", formId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    responses.Add(new FormResponse
                    {
                        Id = reader.GetInt32("SubmissionID"),
                        FormId = reader.GetInt32("EvaluationID"),
                        StudentId = reader.GetString("StudentIDNumber"),
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

        public static bool HasStudentSubmitted(int formId, string studentId)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT COUNT(*) FROM FormSubmission 
                WHERE EvaluationID = @formId AND StudentIDNumber = @studentId", conn);
            cmd.Parameters.AddWithValue("@formId", formId);
            cmd.Parameters.AddWithValue("@studentId", studentId);

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

        public static void SeedSampleData()
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand("SELECT COUNT(*) FROM EvaluationForm", conn);
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count > 0) return;

            // Get default admin ID
            var adminCmd = new MySqlCommand("SELECT AdminID FROM Admin LIMIT 1", conn);
            var adminResult = adminCmd.ExecuteScalar();
            int? adminId = adminResult != null ? Convert.ToInt32(adminResult) : null;

            var sampleForm = new EvaluationForm
            {
                Title = "Teacher Performance Evaluation",
                Description = "Please evaluate your teacher's performance this semester",
                TargetCourse = "BSIT",
                CreatedAt = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                IsActive = true,
                CreatedById = adminId,
                Questions = new List<FormQuestion>
                {
                    new() { Text = "Knowledge of the subject matter", Type = QuestionType.Rating, IsRequired = true, MinRating = 1, MaxRating = 5, OrderIndex = 0 },
                    new() { Text = "Teaching methodology and presentation skills", Type = QuestionType.Rating, IsRequired = true, MinRating = 1, MaxRating = 5, OrderIndex = 1 },
                    new() { Text = "Classroom management and discipline", Type = QuestionType.Rating, IsRequired = true, MinRating = 1, MaxRating = 5, OrderIndex = 2 },
                    new() { Text = "Additional comments or suggestions", Type = QuestionType.Text, IsRequired = false, OrderIndex = 3 }
                }
            };

            AddForm(sampleForm);
        }
    }
}
