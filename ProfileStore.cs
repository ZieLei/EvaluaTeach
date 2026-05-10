using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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

        public static bool SetAvatar(Image? image)
        {
            if (image == null)
                return false;

            // Resize to smaller size for database (max 300x300 to stay under packet limit)
            const int maxSize = 300;
            Bitmap resizedImage;
            if (image.Width > maxSize || image.Height > maxSize)
            {
                int newWidth, newHeight;
                if (image.Width > image.Height)
                {
                    newWidth = maxSize;
                    newHeight = (int)(image.Height * ((double)maxSize / image.Width));
                }
                else
                {
                    newHeight = maxSize;
                    newWidth = (int)(image.Width * ((double)maxSize / image.Height));
                }
                resizedImage = new Bitmap(image, new Size(newWidth, newHeight));
            }
            else
            {
                resizedImage = new Bitmap(image);
            }

            // Save to database first - only update UI if successful
            if (DatabaseStudentId.HasValue)
            {
                if (!SaveAvatarToDatabase(resizedImage))
                {
                    resizedImage.Dispose();
                    return false; // Save failed, don't update UI
                }
            }

            // Only update UI after successful save
            FullSizeAvatar?.Dispose();
            FullSizeAvatar = new Bitmap(resizedImage);
            
            Avatar?.Dispose();
            Avatar = new Bitmap(resizedImage, new Size(84, 84));
            
            resizedImage.Dispose();
            return true;
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

        private static bool SaveAvatarToDatabase(Image image)
        {
            if (!DatabaseStudentId.HasValue)
            {
                MessageBox.Show("Cannot save avatar: No student ID available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                EnsureAvatarColumnExists();

                using var ms = new MemoryStream();
                // Use JPEG with lower quality for smaller size
                var encoder = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()
                    .FirstOrDefault(c => c.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid);
                if (encoder != null)
                {
                    var encoderParams = new System.Drawing.Imaging.EncoderParameters(1);
                    encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(
                        System.Drawing.Imaging.Encoder.Quality, 85L);
                    image.Save(ms, encoder, encoderParams);
                }
                else
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                }
                var avatarBytes = ms.ToArray();

                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = new MySqlCommand(
                    "UPDATE student SET Avatar = @avatar WHERE StudentID = @studentId",
                    conn);
                cmd.Parameters.AddWithValue("@avatar", avatarBytes);
                cmd.Parameters.AddWithValue("@studentId", DatabaseStudentId.Value);
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected == 0)
                {
                    MessageBox.Show("Avatar was not saved: No matching student found in database.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving avatar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool ChangePassword(string oldPassword, string newPassword)
        {
            if (!SessionStore.IsLoggedIn || !SessionStore.UserIdNumeric.HasValue)
                return false;

            try
            {
                string oldHash = HashPassword(oldPassword);
                string newHash = HashPassword(newPassword);

                using var conn = Database.GetConnection();
                conn.Open();

                if (SessionStore.Role == UserRole.Admin)
                {
                    var cmd = new MySqlCommand(
                        "UPDATE Admin SET Password = @newHash WHERE AdminID = @id AND Password = @oldHash",
                        conn);
                    cmd.Parameters.AddWithValue("@newHash", newHash);
                    cmd.Parameters.AddWithValue("@id", SessionStore.UserIdNumeric.Value);
                    cmd.Parameters.AddWithValue("@oldHash", oldHash);
                    return cmd.ExecuteNonQuery() > 0;
                }
                else
                {
                    var cmd = new MySqlCommand(
                        "UPDATE Student SET Password = @newHash WHERE StudentID = @id AND Password = @oldHash",
                        conn);
                    cmd.Parameters.AddWithValue("@newHash", newHash);
                    cmd.Parameters.AddWithValue("@id", SessionStore.UserIdNumeric.Value);
                    cmd.Parameters.AddWithValue("@oldHash", oldHash);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
