namespace EvaluaTeach
{
    public class Teacher
    {
        public int TeacherID { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Department { get; set; } = "";
        public List<string> Subjects { get; set; } = new();
        public DateTime CreatedAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public string SubjectsDisplay => Subjects.Count > 0 ? string.Join(", ", Subjects) : "N/A";
    }
}
