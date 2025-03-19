using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Kursach
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>

    public partial class App : Application
    {
        // Статическое свойство для хранения текущего пользователя
        public static users CurrentUser { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Пример инициализации текущего пользователя
            // Обычно это делается после успешного входа в систему
            CurrentUser = new users { user_id = 1, login = "ИмяПользователя" }; // Замените на реальные данные
        }
    }
}