using System;
using System.Drawing;
using System.IO;
using MySql.Data.MySqlClient;

namespace EvaluaTeach
{
    // Profile store with database persistence for avatar
    public static class ProfileStore
    {
        private static Image? avatar;
        private static Image? fullSizeAvatar;

        public static string Name { get; private set; } = "Mang Juan";
        public static string Meta { get; private set; } = "Student BSIT";
        public static string Email { get; private set; } = "mangjuan@student.edu";
        public static string StudentId { get; private set; } = "2024-000123";
        public static int? DatabaseStudentId { get; private set; }

        // Thumbnail for small display (84x84)
        public static Image? Avatar
        {
            get => avatar;
            private set
            {
                avatar = value;
                ProfileUpdated?.Invoke();
            }
        }

        // Full size for viewing (400x400)
        public static Image? FullSizeAvatar
        {
            get => fullSizeAvatar;
            private set
            {
                fullSizeAvatar = value;
            }
        }

        public static event Action? ProfileUpdated;

        public static void UpdateProfile(string? name, string? meta, string? email = null, string? studentId = null, int? databaseId = null)
        {
            if (!string.IsNullOrWhiteSpace(name)) Name = name!;
            if (!string.IsNullOrWhiteSpace(meta)) Meta = meta!;
            if (!string.IsNullOrWhiteSpace(email)) Email = email!;
            if (!string.IsNullOrWhiteSpace(studentId)) StudentId = studentId!;
            if (databaseId.HasValue) DatabaseStudentId = databaseId.Value;

            ProfileUpdated?.Invoke();
        }

        public static void SetAvatar(Image? image)
        {
            if (image == null)
                return;

            // Store full-size version (capped at 2000px to prevent huge files)
            const int maxSize = 2000;
            if (image.Width > maxSize || image.Height > maxSize)
            {
                int fullWidth, fullHeight;
                if (image.Width > image.Height)
                {
                    fullWidth = maxSize;
                    fullHeight = (int)(image.Height * ((double)maxSize / image.Width));
                }
                else
                {
                    fullHeight = maxSize;
                    fullWidth = (int)(image.Width * ((double)maxSize / image.Height));
                }
                FullSizeAvatar = new Bitmap(image, new Size(fullWidth, fullHeight));
            }
            else
            {
                FullSizeAvatar = new Bitmap(image);
            }

            // Create 84x84 thumbnail for profile display
            Avatar = new Bitmap(image, new Size(84, 84));

            // Save to database if we have a student ID
            if (DatabaseStudentId.HasValue)
            {
                SaveAvatarToDatabase(FullSizeAvatar);
            }
        }

        public static void EnsureAvatarColumnExists()
        {
            try
            {
                using var conn = Database.GetConnection();
                conn.Open();

                // Check if Avatar column exists
                var checkCmd = new MySqlCommand(@"
                    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'Student' AND COLUMN_NAME = 'Avatar' AND TABLE_SCHEMA = DATABASE()", conn);
                var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                if (!exists)
                {
                    // Add Avatar column as MEDIUMBLOB
                    var alterCmd = new MySqlCommand("ALTER TABLE Student ADD COLUMN Avatar MEDIUMBLOB", conn);
                    alterCmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Silently fail if we can't check/modify schema
            }
        }

        public static void LoadAvatarFromDatabase(int studentId)
        {
            try
            {
                EnsureAvatarColumnExists();

                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = new MySqlCommand(
                    "SELECT Avatar FROM Student WHERE StudentID = @studentId AND Avatar IS NOT NULL",
                    conn);
                cmd.Parameters.AddWithValue("@studentId", studentId);

                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    var avatarBytes = (byte[])result;
                    using var ms = new MemoryStream(avatarBytes);
                    // Load image and create a copy so we can dispose the stream
                    var loadedImage = Image.FromStream(ms);
                    FullSizeAvatar = new Bitmap(loadedImage);
                    loadedImage.Dispose();
                    // Create 84x84 thumbnail for profile display
                    Avatar = new Bitmap(FullSizeAvatar, new Size(84, 84));
                }
            }
            catch
            {
                // Silently fail if avatar can't be loaded
            }
        }

        private static void SaveAvatarToDatabase(Image image)
        {
            if (!DatabaseStudentId.HasValue) return;

            try
            {
                EnsureAvatarColumnExists();

                using var ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                var avatarBytes = ms.ToArray();

                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = new MySqlCommand(
                    "UPDATE Student SET Avatar = @avatar WHERE StudentID = @studentId",
                    conn);
                cmd.Parameters.AddWithValue("@avatar", avatarBytes);
                cmd.Parameters.AddWithValue("@studentId", DatabaseStudentId.Value);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // Silently fail if avatar can't be saved
            }
        }
    }
}
