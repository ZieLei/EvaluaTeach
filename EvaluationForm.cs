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
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; } = QuestionType.Rating;
        public bool IsRequired { get; set; } = true;
        public int? MinRating { get; set; } = 1;
        public int? MaxRating { get; set; } = 5;
        public List<string> Options { get; set; } = new();
        public int OrderIndex { get; set; } = 0;
        public string Category { get; set; } = "General";
    }

    public class EvaluationForm
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TargetTeacher { get; set; } = string.Empty;
        public string TargetDepartment { get; set; } = string.Empty;
        public string TargetCourse { get; set; } = "All";
        public List<FormQuestion> Questions { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string Semester { get; set; } = string.Empty;
        public string SchoolYear { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public int? CreatedById { get; set; }  // Foreign key to Admin table
    }

    public class FormResponse
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public byte[]? Avatar { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public Dictionary<int, string> Answers { get; set; } = new();
        public int? AssignmentId { get; set; }  // Links to teacher_assignment for per-subject tracking
        
        // Subject context (populated when AssignmentId is set)
        public string SubjectName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        
        public string SubjectDisplay => !string.IsNullOrEmpty(SubjectName) 
            ? $"📚 {SubjectName} ({Course} · Year {YearLevel} · {Section})" 
            : "General Evaluation";
    }

    public enum CommentLevel
    {
        Normal,
        Mild,
        Moderate,
        Severe
    }

    public enum CommentStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class TeacherReport
    {
        public int ReportID { get; set; }
        public int TeacherID { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int EvaluationID { get; set; }
        public string FormTitle { get; set; } = string.Empty;
        public DateTime SubmissionDate { get; set; }
        public decimal AverageScore { get; set; }
        public int ResponseCount { get; set; }
        public string ReportData { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string SchoolYear { get; set; } = string.Empty;
        public string? CommentText { get; set; }
        public CommentLevel? CommentLevel { get; set; }
        
        // Subject context for per-subject reports
        public int? AssignmentID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        
        public string SubjectDisplay => !string.IsNullOrEmpty(SubjectName)
            ? $"📚 {SubjectName} ({Course} · Year {YearLevel} · {Section})"
            : "";
    }

    public class TeacherAssignment
    {
        public int AssignmentID { get; set; }
        public int TeacherID { get; set; }
        public string Section { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public List<string> Subjects { get; set; } = new();
        public string SubjectsDisplay => Subjects.Count > 0 ? string.Join(", ", Subjects) : "N/A";
        
        public override string ToString()
        {
            return $"{SubjectsDisplay} ({Course} · Year {YearLevel} · {Section})";
        }
    }

    public class FormComment
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public int? StudentDbId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public string FormTitle { get; set; } = string.Empty;
        public string CommentText { get; set; } = string.Empty;
        public CommentLevel SystemLevel { get; set; } = CommentLevel.Normal;
        public CommentLevel? AdminLevel { get; set; }
        public CommentStatus Status { get; set; } = CommentStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public DateTime? ReviewedAt { get; set; }
        public string ReviewedBy { get; set; } = string.Empty;
    }
}
