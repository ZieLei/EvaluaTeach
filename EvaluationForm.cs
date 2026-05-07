using System;
using System.Collections.Generic;

namespace EvaluaTeach
{
    public enum QuestionType
    {
        Rating,
        Text,
        MultipleChoice,
        YesNo
    }

    public class FormQuestion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; } = QuestionType.Rating;
        public bool IsRequired { get; set; } = true;
        public int? MinRating { get; set; } = 1;
        public int? MaxRating { get; set; } = 5;
        public List<string> Options { get; set; } = new();
        public int OrderIndex { get; set; } = 0;
    }

    public class EvaluationForm
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TargetTeacher { get; set; } = string.Empty;
        public string TargetDepartment { get; set; } = string.Empty;
        public string TargetCourse { get; set; } = "All";
        public List<FormQuestion> Questions { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string CreatedBy { get; set; } = string.Empty;
        public int? CreatedById { get; set; }  // Foreign key to Admin table
    }

    public class FormResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FormId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public Dictionary<Guid, string> Answers { get; set; } = new();
    }
}
