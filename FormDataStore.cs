using System;
using System.Collections.Generic;
using System.Linq;

namespace EvaluaTeach
{
    public static class FormDataStore
    {
        private static readonly List<EvaluationForm> forms = new();
        private static readonly List<FormResponse> responses = new();

        public static event Action? FormsUpdated;
        public static event Action? ResponsesUpdated;

        public static IReadOnlyList<EvaluationForm> GetAllForms()
        {
            return forms.AsReadOnly();
        }

        public static IReadOnlyList<EvaluationForm> GetActiveForms()
        {
            return forms.Where(f => f.IsActive).OrderByDescending(f => f.CreatedAt).ToList().AsReadOnly();
        }

        public static IReadOnlyList<EvaluationForm> GetFormsForStudent(string department)
        {
            return forms
                .Where(f => f.IsActive &&
                    (string.IsNullOrEmpty(f.TargetDepartment) || f.TargetDepartment == department))
                .OrderByDescending(f => f.CreatedAt)
                .ToList()
                .AsReadOnly();
        }

        public static EvaluationForm? GetForm(Guid id)
        {
            return forms.FirstOrDefault(f => f.Id == id);
        }

        public static void AddForm(EvaluationForm form)
        {
            forms.Add(form);
            FormsUpdated?.Invoke();
        }

        public static void UpdateForm(EvaluationForm form)
        {
            var index = forms.FindIndex(f => f.Id == form.Id);
            if (index >= 0)
            {
                forms[index] = form;
                FormsUpdated?.Invoke();
            }
        }

        public static void DeleteForm(Guid id)
        {
            forms.RemoveAll(f => f.Id == id);
            FormsUpdated?.Invoke();
        }

        public static void AddResponse(FormResponse response)
        {
            responses.Add(response);
            ResponsesUpdated?.Invoke();
        }

        public static IReadOnlyList<FormResponse> GetResponsesForForm(Guid formId)
        {
            return responses.Where(r => r.FormId == formId).ToList().AsReadOnly();
        }

        public static IReadOnlyList<FormResponse> GetResponsesByStudent(string studentId)
        {
            return responses.Where(r => r.StudentId == studentId).ToList().AsReadOnly();
        }

        public static bool HasStudentSubmitted(Guid formId, string studentId)
        {
            return responses.Any(r => r.FormId == formId && r.StudentId == studentId);
        }

        public static int GetSubmissionCount(Guid formId)
        {
            return responses.Count(r => r.FormId == formId);
        }

        public static void SeedSampleData()
        {
            if (forms.Count > 0) return;

            var sampleForm = new EvaluationForm
            {
                Id = Guid.NewGuid(),
                Title = "Teacher Performance Evaluation",
                Description = "Please evaluate your teacher's performance this semester",
                TargetTeacher = "All Teachers",
                TargetDepartment = "BSIT",
                CreatedAt = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                IsActive = true,
                CreatedBy = "Admin",
                Questions = new List<FormQuestion>
                {
                    new()
                    {
                        Text = "Knowledge of the subject matter",
                        Type = QuestionType.Rating,
                        IsRequired = true,
                        MinRating = 1,
                        MaxRating = 5,
                        OrderIndex = 0
                    },
                    new()
                    {
                        Text = "Teaching methodology and presentation skills",
                        Type = QuestionType.Rating,
                        IsRequired = true,
                        MinRating = 1,
                        MaxRating = 5,
                        OrderIndex = 1
                    },
                    new()
                    {
                        Text = "Classroom management and discipline",
                        Type = QuestionType.Rating,
                        IsRequired = true,
                        MinRating = 1,
                        MaxRating = 5,
                        OrderIndex = 2
                    },
                    new()
                    {
                        Text = "Additional comments or suggestions",
                        Type = QuestionType.Text,
                        IsRequired = false,
                        OrderIndex = 3
                    }
                }
            };

            forms.Add(sampleForm);
        }
    }
}
