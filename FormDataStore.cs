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

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                forms.Add(new EvaluationForm
                {
                    Id = Guid.Parse(reader.GetString("EvaluationID")),
                    Title = reader.GetString("Title"),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                    TargetCourse = reader.IsDBNull(reader.GetOrdinal("TargetCourse")) ? "All" : reader.GetString("TargetCourse"),
                    DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? null : reader.GetDateTime("DueDate"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedAt = reader.GetDateTime("DateCreated"),
                    CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? "" : reader.GetString("CreatedBy"),
                    Questions = new List<FormQuestion>()
                });
            }
            reader.Close();

            // Load questions for each form
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

        public static EvaluationForm? GetForm(Guid id)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT EvaluationID, Title, Description, TargetCourse, 
                       DueDate, IsActive, DateCreated, CreatedBy
                FROM EvaluationForm WHERE EvaluationID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id.ToString());

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var form = new EvaluationForm
            {
                Id = Guid.Parse(reader.GetString("EvaluationID")),
                Title = reader.GetString("Title"),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                TargetCourse = reader.IsDBNull(reader.GetOrdinal("TargetCourse")) ? "All" : reader.GetString("TargetCourse"),
                DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? null : reader.GetDateTime("DueDate"),
                IsActive = reader.GetBoolean("IsActive"),
                CreatedAt = reader.GetDateTime("DateCreated"),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? "" : reader.GetString("CreatedBy"),
                Questions = new List<FormQuestion>()
            };
            reader.Close();

            form.Questions = GetQuestionsForForm(form.Id, conn);
            return form;
        }

        private static List<FormQuestion> GetQuestionsForForm(Guid formId, MySqlConnection conn)
        {
            var questions = new List<FormQuestion>();
            var cmd = new MySqlCommand(@"
                SELECT QuestionID, QuestionText, QuestionType, OrderIndex, 
                       IsRequired, MinRating, MaxRating
                FROM SurveyQuestion WHERE EvaluationID = @formId
                ORDER BY OrderIndex", conn);
            cmd.Parameters.AddWithValue("@formId", formId.ToString());

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                questions.Add(new FormQuestion
                {
                    Id = Guid.Parse(reader.GetString("QuestionID")),
                    Text = reader.GetString("QuestionText"),
                    Type = ParseQuestionType(reader.GetString("QuestionType")),
                    OrderIndex = reader.GetInt32("OrderIndex"),
                    IsRequired = reader.GetBoolean("IsRequired"),
                    MinRating = reader.IsDBNull(reader.GetOrdinal("MinRating")) ? 1 : reader.GetInt32("MinRating"),
                    MaxRating = reader.IsDBNull(reader.GetOrdinal("MaxRating")) ? 5 : reader.GetInt32("MaxRating")
                });
            }
            return questions;
        }

        public static void AddForm(EvaluationForm form)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // Insert form
                var cmd = new MySqlCommand(@"
                    INSERT INTO EvaluationForm (EvaluationID, Title, Description, TargetCourse, DueDate, IsActive, CreatedBy)
                    VALUES (@id, @title, @desc, @course, @dueDate, @isActive, @createdById)", conn, tx);
                cmd.Parameters.AddWithValue("@id", form.Id.ToString());
                cmd.Parameters.AddWithValue("@title", form.Title);
                cmd.Parameters.AddWithValue("@desc", form.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@course", form.TargetCourse ?? "All");
                cmd.Parameters.AddWithValue("@dueDate", form.DueDate.HasValue ? form.DueDate.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@isActive", form.IsActive);
                cmd.Parameters.AddWithValue("@createdById", form.CreatedById.HasValue ? form.CreatedById.Value : (object)DBNull.Value);
                cmd.ExecuteNonQuery();

                // Insert questions
                foreach (var q in form.Questions)
                {
                    var qCmd = new MySqlCommand(@"
                        INSERT INTO SurveyQuestion (QuestionID, EvaluationID, QuestionText, QuestionType, OrderIndex, IsRequired, MinRating, MaxRating)
                        VALUES (@qid, @formId, @text, @type, @orderIdx, @isRequired, @minRating, @maxRating)", conn, tx);
                    qCmd.Parameters.AddWithValue("@qid", q.Id.ToString());
                    qCmd.Parameters.AddWithValue("@formId", form.Id.ToString());
                    qCmd.Parameters.AddWithValue("@text", q.Text);
                    qCmd.Parameters.AddWithValue("@type", q.Type.ToString().ToLower());
                    qCmd.Parameters.AddWithValue("@orderIdx", q.OrderIndex);
                    qCmd.Parameters.AddWithValue("@isRequired", q.IsRequired);
                    qCmd.Parameters.AddWithValue("@minRating", q.MinRating ?? (object)DBNull.Value);
                    qCmd.Parameters.AddWithValue("@maxRating", q.MaxRating ?? (object)DBNull.Value);
                    qCmd.ExecuteNonQuery();
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

            var cmd = new MySqlCommand(@"
                UPDATE EvaluationForm 
                SET Title = @title, Description = @desc, TargetCourse = @course, 
                    DueDate = @dueDate, IsActive = @isActive
                WHERE EvaluationID = @id", conn);
            cmd.Parameters.AddWithValue("@id", form.Id.ToString());
            cmd.Parameters.AddWithValue("@title", form.Title);
            cmd.Parameters.AddWithValue("@desc", form.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@course", form.TargetCourse ?? "All");
            cmd.Parameters.AddWithValue("@dueDate", form.DueDate.HasValue ? form.DueDate.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@isActive", form.IsActive);
            cmd.ExecuteNonQuery();

            FormsUpdated?.Invoke();
        }

        public static void DeleteForm(Guid id)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            // ON DELETE CASCADE will handle related questions and responses
            var cmd = new MySqlCommand("DELETE FROM EvaluationForm WHERE EvaluationID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id.ToString());
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
                // Create submission record
                var subCmd = new MySqlCommand(@"
                    INSERT INTO FormSubmission (StudentID, EvaluationID, SubmittedAt)
                    VALUES (@studentId, @formId, @submittedAt)", conn, tx);
                subCmd.Parameters.AddWithValue("@studentId", response.StudentId);
                subCmd.Parameters.AddWithValue("@formId", response.FormId.ToString());
                subCmd.Parameters.AddWithValue("@submittedAt", response.SubmittedAt);
                subCmd.ExecuteNonQuery();

                var submissionId = (int)subCmd.LastInsertedId;

                // Insert individual responses
                foreach (var answer in response.Answers)
                {
                    var respCmd = new MySqlCommand(@"
                        INSERT INTO SurveyResponse (SubmissionID, QuestionID, Answer)
                        VALUES (@subId, @qid, @answer)", conn, tx);
                    respCmd.Parameters.AddWithValue("@subId", submissionId);
                    respCmd.Parameters.AddWithValue("@qid", answer.Key.ToString());
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

        public static List<FormResponse> GetResponsesForForm(Guid formId)
        {
            var responses = new List<FormResponse>();
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT fs.SubmissionID, fs.StudentID, fs.EvaluationID, fs.SubmittedAt
                FROM FormSubmission fs
                WHERE fs.EvaluationID = @formId", conn);
            cmd.Parameters.AddWithValue("@formId", formId.ToString());

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var submissionId = reader.GetInt32("SubmissionID");
                var response = new FormResponse
                {
                    Id = Guid.NewGuid(),
                    FormId = Guid.Parse(reader.GetString("EvaluationID")),
                    StudentId = reader.GetString("StudentID"),
                    SubmittedAt = reader.GetDateTime("SubmittedAt"),
                    Answers = new Dictionary<Guid, string>()
                };
                responses.Add(response);
            }
            reader.Close();

            // Load answers for each response
            foreach (var response in responses)
            {
                response.Answers = GetAnswersForSubmission(response.Id, conn);
            }

            return responses;
        }

        private static Dictionary<Guid, string> GetAnswersForSubmission(Guid submissionId, MySqlConnection conn)
        {
            var answers = new Dictionary<Guid, string>();
            var cmd = new MySqlCommand(@"
                SELECT QuestionID, Answer
                FROM SurveyResponse sr
                JOIN FormSubmission fs ON sr.SubmissionID = fs.SubmissionID
                WHERE fs.EvaluationID = @subId", conn);
            cmd.Parameters.AddWithValue("@subId", submissionId.ToString());

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var qid = Guid.Parse(reader.GetString("QuestionID"));
                var answer = reader.GetString("Answer");
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
                SELECT fs.SubmissionID, fs.EvaluationID, fs.SubmittedAt
                FROM FormSubmission fs
                WHERE fs.StudentID = @studentId", conn);
            cmd.Parameters.AddWithValue("@studentId", studentId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                responses.Add(new FormResponse
                {
                    Id = Guid.NewGuid(),
                    FormId = Guid.Parse(reader.GetString("EvaluationID")),
                    StudentId = studentId,
                    SubmittedAt = reader.GetDateTime("SubmittedAt"),
                    Answers = new Dictionary<Guid, string>()
                });
            }
            return responses;
        }

        public static bool HasStudentSubmitted(Guid formId, string studentId)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT COUNT(*) FROM FormSubmission 
                WHERE EvaluationID = @formId AND StudentID = @studentId", conn);
            cmd.Parameters.AddWithValue("@formId", formId.ToString());
            cmd.Parameters.AddWithValue("@studentId", studentId);

            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public static int GetSubmissionCount(Guid formId)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
                SELECT COUNT(*) FROM FormSubmission WHERE EvaluationID = @formId", conn);
            cmd.Parameters.AddWithValue("@formId", formId.ToString());

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
            // Check if data already exists
            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand("SELECT COUNT(*) FROM EvaluationForm", conn);
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count > 0) return;

            var sampleForm = new EvaluationForm
            {
                Id = Guid.NewGuid(),
                Title = "Teacher Performance Evaluation",
                Description = "Please evaluate your teacher's performance this semester",
                TargetCourse = "BSIT",
                CreatedAt = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                IsActive = true,
                CreatedBy = "Admin",
                Questions = new List<FormQuestion>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Knowledge of the subject matter",
                        Type = QuestionType.Rating,
                        IsRequired = true,
                        MinRating = 1,
                        MaxRating = 5,
                        OrderIndex = 0
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Teaching methodology and presentation skills",
                        Type = QuestionType.Rating,
                        IsRequired = true,
                        MinRating = 1,
                        MaxRating = 5,
                        OrderIndex = 1
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Classroom management and discipline",
                        Type = QuestionType.Rating,
                        IsRequired = true,
                        MinRating = 1,
                        MaxRating = 5,
                        OrderIndex = 2
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = "Additional comments or suggestions",
                        Type = QuestionType.Text,
                        IsRequired = false,
                        OrderIndex = 3
                    }
                }
            };

            AddForm(sampleForm);
        }
    }
}

