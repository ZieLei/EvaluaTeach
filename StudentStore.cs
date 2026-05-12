using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

                // Look up IDNumber (string) for this StudentID (int)
                var idNumCmd = new MySqlCommand("SELECT IDNumber FROM student WHERE StudentID = @studentId", conn);
                idNumCmd.Parameters.AddWithValue("@studentId", studentId);
                var idNumber = idNumCmd.ExecuteScalar()?.ToString() ?? "";

                // Remove comments first (FK: comment.StudentID → student.StudentID)
                var delComments = new MySqlCommand("DELETE FROM comment WHERE StudentID = @id", conn);
                delComments.Parameters.AddWithValue("@id", studentId);
                delComments.ExecuteNonQuery();

                if (!string.IsNullOrEmpty(idNumber))
                {
                    // Remove survey responses for this student's submissions
                    var delAnswers = new MySqlCommand(@"
                        DELETE sr FROM SurveyResponse sr
                        INNER JOIN FormSubmission fs ON fs.SubmissionID = sr.SubmissionID
                        WHERE fs.StudentIDNumber = @idNum", conn);
                    delAnswers.Parameters.AddWithValue("@idNum", idNumber);
                    delAnswers.ExecuteNonQuery();

                    // Remove submissions
                    var delSubs = new MySqlCommand("DELETE FROM FormSubmission WHERE StudentIDNumber = @idNum", conn);
                    delSubs.Parameters.AddWithValue("@idNum", idNumber);
                    delSubs.ExecuteNonQuery();
                }

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

        public static Task<(int inserted, int updated)> SyncFromRegistry()
            => Task.Run(() => RunSyncFromRegistry());

        private static (int inserted, int updated) RunSyncFromRegistry()
        {
            using var conn = Database.GetConnection();
            conn.Open();

            // 1. Read all registry rows in one query
            var rows = new List<(string IdNumber, string FirstName, string LastName,
                                  string Email, string Course, int YearLevel, string RawPassword)>();
            using (var rdr = new MySqlCommand(@"
                SELECT id_number, first_name, last_name, email_address,
                       course, year_level, contact_number
                FROM studentslcc", conn).ExecuteReader())
            {
                while (rdr.Read())
                {
                    string id = rdr.IsDBNull(0) ? "" : rdr.GetString(0).Trim();
                    if (string.IsNullOrWhiteSpace(id)) continue;
                    rows.Add((
                        id,
                        rdr.IsDBNull(1) ? "" : rdr.GetString(1).Trim(),
                        rdr.IsDBNull(2) ? "" : rdr.GetString(2).Trim(),
                        rdr.IsDBNull(3) ? "" : rdr.GetString(3).Trim(),
                        rdr.IsDBNull(4) ? "" : rdr.GetString(4).Trim(),
                        ParseYearLevel(rdr.IsDBNull(5) ? "" : rdr.GetString(5).Trim()),
                        rdr.IsDBNull(6) ? "" : rdr.GetString(6).Trim()
                    ));
                }
            }

            if (rows.Count == 0) return (0, 0);

            // 2. Load existing students (IDNumber + Email) in one query
            var existingIds   = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var existingEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var rdr = new MySqlCommand(
                "SELECT IDNumber, IFNULL(Email,'') FROM student", conn).ExecuteReader())
            {
                while (rdr.Read())
                {
                    existingIds.Add(rdr.GetString(0));
                    var em = rdr.GetString(1);
                    if (!string.IsNullOrWhiteSpace(em)) existingEmails.Add(em);
                }
            }

            // 3. Resolve email conflicts in-memory: track emails already seen in this batch
            var batchEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var pwCache     = new Dictionary<string, string>(StringComparer.Ordinal);

            var toInsert = new List<(string IdNumber, string FirstName, string LastName,
                                     string Email, string Course, int YearLevel, string HashedPw)>();
            var toUpdate = new List<(string IdNumber, string FirstName, string LastName,
                                     string Email, string Course, int YearLevel)>();

            foreach (var row in rows)
            {
                // Determine safe email: blank if claimed by another student or duplicate in this batch
                string safeEmail = row.Email;
                if (!string.IsNullOrWhiteSpace(safeEmail))
                {
                    bool claimedByOther = existingEmails.Contains(safeEmail)
                        && !existingIds.Contains(row.IdNumber); // existing student with same id owns it — allow update
                    bool duplicateInBatch = batchEmails.Contains(safeEmail);
                    if (claimedByOther || duplicateInBatch)
                        safeEmail = "";
                }
                if (!string.IsNullOrWhiteSpace(safeEmail))
                    batchEmails.Add(safeEmail);

                if (existingIds.Contains(row.IdNumber))
                {
                    toUpdate.Add((row.IdNumber, row.FirstName, row.LastName, safeEmail, row.Course, row.YearLevel));
                }
                else
                {
                    string pwKey = string.IsNullOrWhiteSpace(row.RawPassword) ? row.IdNumber : row.RawPassword;
                    if (!pwCache.TryGetValue(pwKey, out string? hashedPw))
                    {
                        hashedPw = HashPassword(pwKey);
                        pwCache[pwKey] = hashedPw;
                    }
                    toInsert.Add((row.IdNumber, row.FirstName, row.LastName, safeEmail, row.Course, row.YearLevel, hashedPw));
                    existingIds.Add(row.IdNumber); // prevent duplicate inserts within same batch
                }
            }

            // 4. Merge toInsert + toUpdate into a single unified list for INSERT ... ON DUPLICATE KEY UPDATE
            //    IDNumber is the UNIQUE key — insert sets Password, update ignores it.
            var allRows = new List<(string IdNumber, string FirstName, string LastName,
                                    string Email, string Course, int YearLevel, string HashedPw, bool IsNew)>();
            foreach (var r in toInsert)
                allRows.Add((r.IdNumber, r.FirstName, r.LastName, r.Email, r.Course, r.YearLevel, r.HashedPw, true));
            foreach (var r in toUpdate)
                allRows.Add((r.IdNumber, r.FirstName, r.LastName, r.Email, r.Course, r.YearLevel, "", false));

            const int chunkSize = 100;
            using var tx = conn.BeginTransaction();
            try
            {
                for (int i = 0; i < allRows.Count; i += chunkSize)
                {
                    var chunk = allRows.Skip(i).Take(chunkSize).ToList();
                    var sb = new System.Text.StringBuilder(
                        "INSERT INTO student (IDNumber, FirstName, LastName, Email, Course, YearLevel, Section, Password) VALUES ");
                    var cmd = new MySqlCommand { Connection = conn, Transaction = tx };

                    for (int j = 0; j < chunk.Count; j++)
                    {
                        if (j > 0) sb.Append(',');
                        // For updates the password placeholder is unused in ON DUPLICATE KEY, so pass empty string safely
                        sb.Append($"(@id{j},@fn{j},@ln{j},@em{j},@co{j},@yr{j},NULL,@pw{j})");
                        var r = chunk[j];
                        cmd.Parameters.AddWithValue($"@id{j}", r.IdNumber);
                        cmd.Parameters.AddWithValue($"@fn{j}", r.FirstName);
                        cmd.Parameters.AddWithValue($"@ln{j}", r.LastName);
                        cmd.Parameters.AddWithValue($"@em{j}", r.Email);
                        cmd.Parameters.AddWithValue($"@co{j}", r.Course);
                        cmd.Parameters.AddWithValue($"@yr{j}", r.YearLevel);
                        cmd.Parameters.AddWithValue($"@pw{j}", r.HashedPw);
                    }

                    sb.Append(@"
                        ON DUPLICATE KEY UPDATE
                            FirstName = VALUES(FirstName),
                            LastName  = VALUES(LastName),
                            Email     = VALUES(Email),
                            Course    = VALUES(Course),
                            YearLevel = VALUES(YearLevel)");

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }

            return (toInsert.Count, toUpdate.Count);
        }

        private static int ParseYearLevel(string yearStr)
        {
            if (string.IsNullOrWhiteSpace(yearStr)) return 1;
            var s = yearStr.Trim().ToLowerInvariant();
            if (s.StartsWith("1") || s.StartsWith("fir") || s.StartsWith("1st")) return 1;
            if (s.StartsWith("2") || s.StartsWith("sec") || s.StartsWith("2nd")) return 2;
            if (s.StartsWith("3") || s.StartsWith("thi") || s.StartsWith("3rd")) return 3;
            if (s.StartsWith("4") || s.StartsWith("fou") || s.StartsWith("4th")) return 4;
            if (s.StartsWith("5") || s.StartsWith("fif") || s.StartsWith("5th")) return 5;
            return 1;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
