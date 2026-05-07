using MySql.Data.MySqlClient;

namespace EvaluaTeach
{
    public static class Database
    {
        private static string connectionString =
            "Server=localhost;Database=EvaluaTeach;Uid=root;Pwd=;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
