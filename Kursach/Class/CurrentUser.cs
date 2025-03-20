using System;

namespace Kursach.Class
{
    // Класс для хранения информации о текущем пользователе
    public class CurrentUser
    {
        private static CurrentUser _instance; // единственный экземпляр класса

        public static CurrentUser Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CurrentUser();
                }
                return _instance;
            }
        }

        public APPUser User { get; private set; } // Объект текущего пользователя

        // Приватный конструктор для предотвращения создания экземпляров извне
        private CurrentUser() { }

        // Метод для установки текущего пользователя
        public void SetCurrentUser(APPUser user)
        {
            User = user;
        }

        // Метод для проверки, есть ли текущий аутентифицированный пользователь
        public bool IsAuthenticated()
        {
            return User != null;
        }
    }

    // Класс для представления пользователя
    public class APPUser
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public bool IsAdmin { get; set; }

        // Конструктор для удобного создания экземпляров APPUser
        public APPUser(int userId, string login, bool isAdmin)
        {
            UserId = userId;
            Login = login;
            IsAdmin = isAdmin;
        }
    }
}