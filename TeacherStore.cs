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

            return teachers;
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

            var cmd = new MySqlCommand("DELETE FROM Teacher WHERE TeacherID = @id", conn);
            cmd.Parameters.AddWithValue("@id", teacherId);
            cmd.ExecuteNonQuery();

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

            var cmd = new MySqlCommand(@"
                SELECT TeacherID, FirstName, LastName, Email, Department, Section, Course, YearLevel, Subjects, CreatedAt
                FROM Teacher
                WHERE (Section = @section OR Section IS NULL OR Section = '') 
                  AND (Course = @course OR Course IS NULL OR Course = '')
                  AND (YearLevel = @yearLevel OR YearLevel IS NULL OR YearLevel = '')
                ORDER BY LastName, FirstName", conn);

            cmd.Parameters.AddWithValue("@section", section);
            cmd.Parameters.AddWithValue("@course", course);
            cmd.Parameters.AddWithValue("@yearLevel", yearLevel);

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

            return teachers;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
