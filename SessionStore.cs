using System;

namespace EvaluaTeach
{
    public enum UserRole
    {
        Student,
        Admin,
        Teacher
    }

    public static class SessionStore
    {
        public static string UserId { get; private set; } = string.Empty;
        public static int? UserIdNumeric { get; private set; } = null;  // Database AdminID or StudentID
        public static string UserName { get; private set; } = string.Empty;
        public static string Email { get; private set; } = string.Empty;
        public static UserRole Role { get; private set; } = UserRole.Student;
        public static bool IsLoggedIn { get; private set; } = false;

        public static event Action? SessionUpdated;

        public static void Login(string userId, int? userIdNumeric, string userName, string email, UserRole role)
        {
            UserId = userId;
            UserIdNumeric = userIdNumeric;
            UserName = userName;
            Email = email;
            Role = role;
            IsLoggedIn = true;

            string roleMeta = role == UserRole.Admin ? "Administrator" : role == UserRole.Teacher ? "Teacher" : "Student";
            ProfileStore.UpdateProfile(userName, roleMeta, email, userId);
            SessionUpdated?.Invoke();
        }

        public static void Logout()
        {
            UserId = string.Empty;
            UserName = string.Empty;
            Email = string.Empty;
            Role = UserRole.Student;
            IsLoggedIn = false;
            SessionUpdated?.Invoke();
        }
    }
}
