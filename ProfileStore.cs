using System;
using System.Drawing;

namespace EvaluaTeach
{
    // Simple in-memory profile store for sharing profile data across forms.
    public static class ProfileStore
    {
        private static Image? avatar;

        public static string Name { get; private set; } = "Mang Juan";
        public static string Meta { get; private set; } = "Student BSIT";
        public static string Email { get; private set; } = "mangjuan@student.edu";
        public static string StudentId { get; private set; } = "2024-000123";

        public static Image? Avatar
        {
            get => avatar;
            private set
            {
                avatar = value;
                ProfileUpdated?.Invoke();
            }
        }

        public static event Action? ProfileUpdated;

        public static void UpdateProfile(string? name, string? meta, string? email = null, string? studentId = null)
        {
            if (!string.IsNullOrWhiteSpace(name)) Name = name!;
            if (!string.IsNullOrWhiteSpace(meta)) Meta = meta!;
            if (!string.IsNullOrWhiteSpace(email)) Email = email!;
            if (!string.IsNullOrWhiteSpace(studentId)) StudentId = studentId!;

            ProfileUpdated?.Invoke();
        }

        public static void SetAvatar(Image? image)
        {
            if (image == null)
                return;

            // store a copy to avoid disposing issues
            Avatar = new Bitmap(image);
        }
    }
}
