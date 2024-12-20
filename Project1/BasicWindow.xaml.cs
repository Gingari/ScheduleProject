using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MySql.Data.MySqlClient;

namespace Project1
{
    public partial class BasicWindow : Window
    {
        private DateTime StartOfWeek;
        private MediaPlayer player;
        private string userRole;   // Роль пользователя
        private string userEmail;  // Email пользователя

        public BasicWindow(string role, string email)
        {
            InitializeComponent();
            userRole = role;
            userEmail = email;

            SetCurrentDate();
            SetCurrentWeek(DateTime.Now);
            LoadScheduleForUser(); // Загружаем пары
            player = new MediaPlayer();
            SetRoleBasedUI();
        }

        // Устанавливаем текущую дату
        public void SetCurrentDate()
        {
            Data.Text = DateTime.Now.ToString("dd-MM-yyyy");
        }

        // Устанавливаем текущую неделю
        public void SetCurrentWeek(DateTime date)
        {
            StartOfWeek = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);  // Определяем начало недели (Понедельник)
            DateTime EndOfWeek = StartOfWeek.AddDays(6);  // Конец недели (Воскресенье)

            WeekTextBlock.Text = $"{StartOfWeek:dd-MM-yyyy} - {EndOfWeek:dd-MM-yyyy}";  // Обновляем текст на UI

            LoadScheduleForUser();  // Загружаем расписание для новой недели
        }


        // Переход к следующей неделе
        private void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentWeek(StartOfWeek.AddDays(7));
        }

        // Переход к предыдущей неделе
        private void LastWeek_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentWeek(StartOfWeek.AddDays(-7));
        }

        // Кнопка воспроизведения музыки
        private void GoidaButton_Click(object sender, RoutedEventArgs e)
        {
            // Определяем путь к папке "MusicDirectory", которая будет рядом с приложением
            string musicDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

            if (Directory.Exists(musicDirectoryPath))
            {
                var files = Directory.GetFiles(musicDirectoryPath, "*.mp3");
                if (files.Length > 0)
                {
                    Random random = new Random();
                    string randomFile = files[random.Next(files.Length)];

                    if (player.CanPause || player.Position.TotalSeconds > 0)
                    {
                        player.Stop();
                    }

                    try
                    {
                        player.Open(new Uri(randomFile));
                        player.Play();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при воспроизведении: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("В указанной директории нет музыкальных файлов.");
                }
            }
            else
            {
                MessageBox.Show("Указанная директория не существует: " + musicDirectoryPath);
            }
        }


        // Настройка интерфейса в зависимости от роли
        private void SetRoleBasedUI()
        {
            if (userRole == "Admin")
            {
                EditButton.Visibility = Visibility.Visible;
                AddNewButton.Visibility = Visibility.Visible;
            }
            else if (userRole == "Teacher")
            {
                EditButton.Visibility = Visibility.Collapsed;
                AddNewButton.Visibility = Visibility.Collapsed;
            }
        }

        // Обработчик клика по кнопке редактирования расписания
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditScheduleWindow editScheduleWindow = new EditScheduleWindow(userEmail);
                editScheduleWindow.ShowDialog();
                this.Hide();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Получение расписания для текущего пользователя
        private string GetScheduleForUser(string email)
        {
            string connectionString = "Server=sql7.freemysqlhosting.net ;Database=sql7752662;User=sql7752662;Password=q4QaT7UgZR;Port=3306;";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Изменяем запрос на использование поля Login (именно это поле хранит email пользователя в базе)
                string query = "SELECT ScheduleText FROM Schedules WHERE Login = @Email;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    var result = command.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }

        private void LoadScheduleForUser()
        {
            string connectionString = "Server=sql7.freemysqlhosting.net;Database=sql7752662;User=sql7752662;Password=q4QaT7UgZR;Port=3306;";

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                SELECT 
                    Schedules.PairNumber, 
                    Subjects.Name AS SubjectName, 
                    Schedules.ScheduleDate
                FROM 
                    Users
                JOIN 
                    Teachers ON Users.Id = Teachers.UserId
                JOIN 
                    Schedules ON Teachers.Id = Schedules.TeacherId
                JOIN 
                    Subjects ON Schedules.SubjectId = Subjects.Id
                WHERE 
                    Users.Login = @UserEmail
                    AND Schedules.ScheduleDate BETWEEN @StartOfWeek AND @EndOfWeek
                ORDER BY 
                    Schedules.ScheduleDate, Schedules.PairNumber;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Параметры для запроса
                        command.Parameters.AddWithValue("@UserEmail", userEmail);
                        command.Parameters.AddWithValue("@StartOfWeek", StartOfWeek.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@EndOfWeek", StartOfWeek.AddDays(6).ToString("yyyy-MM-dd"));

                        using (var reader = command.ExecuteReader())
                        {
                            ClearScheduleUI(); // Очищаем UI перед обновлением

                            while (reader.Read())
                            {
                                int pairNumber = reader.GetInt32("PairNumber");
                                string subjectName = reader.GetString("SubjectName");
                                DateTime scheduleDate = reader.GetDateTime("ScheduleDate");

                                // Определяем день недели
                                string dayOfWeek = scheduleDate.DayOfWeek.ToString();

                                // Заполняем UI
                                UpdateScheduleUI(dayOfWeek, pairNumber, subjectName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки расписания: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void ClearScheduleUI()
        {
            // Понедельник
            MondayPair1.Content = MondayPair2.Content = MondayPair3.Content =
            MondayPair4.Content = MondayPair5.Content = MondayPair6.Content =
            MondayPair7.Content = MondayPair8.Content = "";

            // Вторник
            TuesdayPair1.Content = TuesdayPair2.Content = TuesdayPair3.Content =
            TuesdayPair4.Content = TuesdayPair5.Content = TuesdayPair6.Content =
            TuesdayPair7.Content = TuesdayPair8.Content = "";

            // Среда
            WednesdayPair1.Content = WednesdayPair2.Content = WednesdayPair3.Content =
            WednesdayPair4.Content = WednesdayPair5.Content = WednesdayPair6.Content =
            WednesdayPair7.Content = WednesdayPair8.Content = "";

            // Четверг
            ThursdayPair1.Content = ThursdayPair2.Content = ThursdayPair3.Content =
            ThursdayPair4.Content = ThursdayPair5.Content = ThursdayPair6.Content =
            ThursdayPair7.Content = ThursdayPair8.Content = "";

            // Пятница
            FridayPair1.Content = FridayPair2.Content = FridayPair3.Content =
            FridayPair4.Content = FridayPair5.Content = FridayPair6.Content =
            FridayPair7.Content = FridayPair8.Content = "";

            // Суббота
            SaturdayPair1.Content = SaturdayPair2.Content = SaturdayPair3.Content =
            SaturdayPair4.Content = SaturdayPair5.Content = SaturdayPair6.Content =
            SaturdayPair7.Content = SaturdayPair8.Content = "";

            // Воскресенье
            SundayPair1.Content = SundayPair2.Content = SundayPair3.Content =
            SundayPair4.Content = SundayPair5.Content = SundayPair6.Content =
            SundayPair7.Content = SundayPair8.Content = "";
        }



        // Обновляем UI для конкретного дня недели и пары
        private void UpdateScheduleUI(string dayOfWeek, int pairNumber, string subject)
        {
            switch (dayOfWeek)
            {
                case "Monday":
                    if (pairNumber == 1) MondayPair1.Content = subject;
                    if (pairNumber == 2) MondayPair2.Content = subject;
                    if (pairNumber == 3) MondayPair3.Content = subject;
                    if (pairNumber == 4) MondayPair4.Content = subject;
                    if (pairNumber == 5) MondayPair5.Content = subject;
                    if (pairNumber == 6) MondayPair6.Content = subject;
                    if (pairNumber == 7) MondayPair7.Content = subject;
                    if (pairNumber == 8) MondayPair8.Content = subject;
                    break;
                case "Tuesday":
                    if (pairNumber == 1) TuesdayPair1.Content = subject;
                    if (pairNumber == 2) TuesdayPair2.Content = subject;
                    if (pairNumber == 3) TuesdayPair3.Content = subject;
                    if (pairNumber == 4) TuesdayPair4.Content = subject;
                    if (pairNumber == 5) TuesdayPair5.Content = subject;
                    if (pairNumber == 6) TuesdayPair6.Content = subject;
                    if (pairNumber == 7) TuesdayPair7.Content = subject;
                    if (pairNumber == 8) TuesdayPair8.Content = subject;
                    break;
                case "Wednesday":
                    if (pairNumber == 1) WednesdayPair1.Content = subject;
                    if (pairNumber == 2) WednesdayPair2.Content = subject;
                    if (pairNumber == 3) WednesdayPair3.Content = subject;
                    if (pairNumber == 4) WednesdayPair4.Content = subject;
                    if (pairNumber == 5) WednesdayPair5.Content = subject;
                    if (pairNumber == 6) WednesdayPair6.Content = subject;
                    if (pairNumber == 7) WednesdayPair7.Content = subject;
                    if (pairNumber == 8) WednesdayPair8.Content = subject;
                    break;
                case "Thursday":
                    if (pairNumber == 1) ThursdayPair1.Content = subject;
                    if (pairNumber == 2) ThursdayPair2.Content = subject;
                    if (pairNumber == 3) ThursdayPair3.Content = subject;
                    if (pairNumber == 4) ThursdayPair4.Content = subject;
                    if (pairNumber == 5) ThursdayPair5.Content = subject;
                    if (pairNumber == 6) ThursdayPair6.Content = subject;
                    if (pairNumber == 7) ThursdayPair7.Content = subject;
                    if (pairNumber == 8) ThursdayPair8.Content = subject;
                    break;
                case "Friday":
                    if (pairNumber == 1) FridayPair1.Content = subject;
                    if (pairNumber == 2) FridayPair2.Content = subject;
                    if (pairNumber == 3) FridayPair3.Content = subject;
                    if (pairNumber == 4) FridayPair4.Content = subject;
                    if (pairNumber == 5) FridayPair5.Content = subject;
                    if (pairNumber == 6) FridayPair6.Content = subject;
                    if (pairNumber == 7) FridayPair7.Content = subject;
                    if (pairNumber == 8) FridayPair8.Content = subject;
                    break;
                case "Saturday":
                    if (pairNumber == 1) SaturdayPair1.Content = subject;
                    if (pairNumber == 2) SaturdayPair2.Content = subject;
                    if (pairNumber == 3) SaturdayPair3.Content = subject;
                    if (pairNumber == 4) SaturdayPair4.Content = subject;
                    if (pairNumber == 5) SaturdayPair5.Content = subject;
                    if (pairNumber == 6) SaturdayPair6.Content = subject;
                    if (pairNumber == 7) SaturdayPair7.Content = subject;
                    if (pairNumber == 8) SaturdayPair8.Content = subject;
                    break;
                case "Sunday":
                    if (pairNumber == 1) SundayPair1.Content = subject;
                    if (pairNumber == 2) SundayPair2.Content = subject;
                    if (pairNumber == 3) SundayPair3.Content = subject;
                    if (pairNumber == 4) SundayPair4.Content = subject;
                    if (pairNumber == 5) SundayPair5.Content = subject;
                    if (pairNumber == 6) SundayPair6.Content = subject;
                    if (pairNumber == 7) SundayPair7.Content = subject;
                    if (pairNumber == 8) SundayPair8.Content = subject;
                    break;
            }
        }

        // Обработчик клика по кнопке добавления пользователя
        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            var email = userEmail;
            var newWindow = new AddUser(email);
            this.Hide(); // Скрываем текущее окно
            newWindow.ShowDialog(); // Открываем модальное окно
            
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
