using System;

namespace Kursach.Class
{
    public static class CurrentUser
    {
        public static APPUser User { get; set; } // Объект текущего пользователя
    }

    // Класс для представления пользователя
    public class APPUser
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public bool IsAdmin { get; set; }

        public static implicit operator APPUser(Autorizaehtion.APPUser v)
        {
            throw new NotImplementedException();
        }
        // Другие свойства, если необходимо
    }
}