namespace Kursach.Class
{
    public static class CurrentUser
    {
        public static int UserId { get; set; } // Идентификатор текущего пользователя
        public static string Username { get; set; } // Логин текущего пользователя
    }

    public class User
    {
        public int UserId { get; set; } // Идентификатор пользователя
        public string Username { get; set; } // Логин пользователя
    }
}
