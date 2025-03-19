using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Kursach.Autorizaehtion;
using Microsoft.Win32; // Для работы с реестром

namespace Kursach
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация таймера для обновления даты и времени
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Отображение текущего языка системы
            SetLanguageFromRegistry();

            // Установка хука на клавиатуру
            _hookID = SetHook(_proc);

            // Установка начальной страницы
            MainFrame.Navigate(new Login());
        }

        // Хук для перехвата событий клавиатуры
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // Обработчик событий клавиатуры
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // Блокировка Shift + Alt
                if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0 && (Keyboard.Modifiers & ModifierKeys.Alt) > 0)
                {
                    return (IntPtr)1; // Блокируем событие
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        // Обновление даты и времени
        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTimeTextBlock.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        // Получение текущего языка системы из реестра
        private void SetLanguageFromRegistry()
        {
            try
            {
                // Открываем ключ реестра
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\International"))
                {
                    if (key != null)
                    {
                        // Получаем LocaleName (например, "ru-RU")
                        string localeName = key.GetValue("LocaleName")?.ToString();
                        if (!string.IsNullOrEmpty(localeName))
                        {
                            // Преобразуем LocaleName в название языка
                            CultureInfo culture = new CultureInfo(localeName);
                            LanguageTextBlock.Text = culture.NativeName;
                        }
                        else
                        {
                            // Если LocaleName недоступен, используем Locale (например, "0419")
                            string locale = key.GetValue("Locale")?.ToString();
                            if (!string.IsNullOrEmpty(locale))
                            {
                                int localeId = int.Parse(locale, System.Globalization.NumberStyles.HexNumber);
                                CultureInfo culture = new CultureInfo(localeId);
                                LanguageTextBlock.Text = culture.NativeName;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении языка из реестра: " + ex.Message);
                LanguageTextBlock.Text = "Unknown";
            }
        }

        // Обработчик кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        // Обработчик кнопки уменьшения окна
        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        // Обработчик кнопки сворачивания окна
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Обработчик кнопки закрытия приложения
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Освобождение ресурсов при закрытии окна
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            UnhookWindowsHookEx(_hookID); // Удаление хука
        }

        // Импорт функций WinAPI для хука на клавиатуру
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
