namespace EvaluaTeach
{
    public class Teacher
    {
        public int TeacherID { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Department { get; set; } = "";

        // Legacy single assignment fields - kept for backwards compatibility
        public string Section { get; set; } = "";
        public string Course { get; set; } = "";
        public string YearLevel { get; set; } = "";
        public List<string> Subjects { get; set; } = new();

        // New: Multiple assignments support
        public List<TeacherAssignment> Assignments { get; set; } = new();

        public DateTime CreatedAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public string SubjectsDisplay => Subjects.Count > 0 ? string.Join(", ", Subjects) : "N/A";
        public string DisplaySection => string.IsNullOrEmpty(Section) ? "N/A" : Section;
        public string DisplayCourse => string.IsNullOrEmpty(Course) ? "N/A" : Course;
        public string DisplayYearLevel => string.IsNullOrEmpty(YearLevel) ? "N/A" : YearLevel;

        // Helper to get all unique subjects across assignments
        public string GetAllSubjectsDisplay()
        {
            var allSubjects = new List<string>();
            allSubjects.AddRange(Subjects);
            foreach (var assignment in Assignments)
            {
                allSubjects.AddRange(assignment.Subjects);
            }
            var unique = allSubjects.Distinct().ToList();
            return unique.Count > 0 ? string.Join(", ", unique) : "N/A";
        }
    }
}
