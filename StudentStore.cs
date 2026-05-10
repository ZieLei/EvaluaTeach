using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace EvaluaTeach
{
    public class Student
    {
        public int StudentID { get; set; }
        public string IDNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public byte[]? Avatar { get; set; }
        public string DisplaySection => string.IsNullOrEmpty(Section) ? "N/A" : Section;
    }

    public static class StudentStore
    {
        public static List<Student> GetAllStudents()
        {
            var students = new List<Student>();

            try
            {
                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = new MySqlCommand("SELECT StudentID, IDNumber, FirstName, LastName, Email, Course, Section, YearLevel, Avatar FROM student ORDER BY LastName, FirstName", conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new Student
                    {
                        StudentID = reader.GetInt32("StudentID"),
                        IDNumber = reader.GetString("IDNumber"),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Email = reader.GetString("Email"),
                        Course = reader.GetString("Course"),
                        YearLevel = reader.GetInt32("YearLevel").ToString(),
                        Section = reader.IsDBNull(reader.GetOrdinal("Section")) ? "" : reader.GetString("Section"),
                        Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : (byte[])reader["Avatar"]
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading students: {ex.Message}");
            }

            return students;
        }

        public static void AddStudent(Student student, string password)
        {
            try
            {
                using var conn = Database.GetConnection();
                conn.Open();

                var hashedPassword = HashPassword(password);

                var cmd = new MySqlCommand(
                    "INSERT INTO student (IDNumber, FirstName, LastName, Email, Course, Section, YearLevel, Password) " +
                    "VALUES (@idNumber, @firstName, @lastName, @email, @course, @section, @yearLevel, @password)", conn);

                cmd.Parameters.AddWithValue("@idNumber", student.IDNumber);
                cmd.Parameters.AddWithValue("@firstName", student.FirstName);
                cmd.Parameters.AddWithValue("@lastName", student.LastName);
                cmd.Parameters.AddWithValue("@email", student.Email);
                cmd.Parameters.AddWithValue("@course", student.Course);
                cmd.Parameters.AddWithValue("@section", string.IsNullOrEmpty(student.Section) ? (object)DBNull.Value : student.Section);
                cmd.Parameters.AddWithValue("@yearLevel", student.YearLevel);
                cmd.Parameters.AddWithValue("@password", hashedPassword);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding student: {ex.Message}");
            }
        }

        public static void UpdateStudent(Student student)
        {
            try
            {
                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = new MySqlCommand(
                    "UPDATE student SET FirstName = @firstName, LastName = @lastName, Email = @email, " +
                    "Course = @course, Section = @section, YearLevel = @yearLevel WHERE StudentID = @studentId", conn);

                cmd.Parameters.AddWithValue("@firstName", student.FirstName);
                cmd.Parameters.AddWithValue("@lastName", student.LastName);
                cmd.Parameters.AddWithValue("@email", student.Email);
                cmd.Parameters.AddWithValue("@course", student.Course);
                cmd.Parameters.AddWithValue("@section", string.IsNullOrEmpty(student.Section) ? (object)DBNull.Value : student.Section);
                cmd.Parameters.AddWithValue("@yearLevel", student.YearLevel);
                cmd.Parameters.AddWithValue("@studentId", student.StudentID);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating student: {ex.Message}");
            }
        }

        public static void DeleteStudent(int studentId)
        {
            try
            {
                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = new MySqlCommand("DELETE FROM student WHERE StudentID = @studentId", conn);
                cmd.Parameters.AddWithValue("@studentId", studentId);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting student: {ex.Message}");
            }
        }

        public static bool EmailExists(string email)
        {
            try
            {
                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = new MySqlCommand("SELECT COUNT(*) FROM student WHERE Email = @email", conn);
                cmd.Parameters.AddWithValue("@email", email);

                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking email existence: {ex.Message}");
            }
        }

        public static bool IdExists(string idNumber)
        {
            try
            {
                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = new MySqlCommand("SELECT COUNT(*) FROM student WHERE IDNumber = @idNumber", conn);
                cmd.Parameters.AddWithValue("@idNumber", idNumber);

                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking ID existence: {ex.Message}");
            }
        }

        public static Student? GetStudentById(int studentId)
        {
            try
            {
                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = new MySqlCommand(
                    "SELECT StudentID, IDNumber, FirstName, LastName, Email, Course, Section, YearLevel " +
                    "FROM student WHERE StudentID = @studentId", conn);
                cmd.Parameters.AddWithValue("@studentId", studentId);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Student
                    {
                        StudentID = reader.GetInt32("StudentID"),
                        IDNumber = reader.GetString("IDNumber"),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Email = reader.GetString("Email"),
                        Course = reader.GetString("Course"),
                        YearLevel = reader.GetInt32("YearLevel").ToString(),
                        Section = reader.IsDBNull(reader.GetOrdinal("Section")) ? "" : reader.GetString("Section")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting student: {ex.Message}");
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
