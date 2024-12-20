using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;

namespace Project1
{
    public partial class EditScheduleWindow : Window
    {
        private string userEmail;

        // Строка подключения
        static readonly string _connectionString = "Server=sql7.freemysqlhosting.net;Database=sql7752662;User=sql7752662;Password=q4QaT7UgZR;Port=3306;CharSet=utf8mb4;";

        // Получение подключения к базе данных
        private MySqlConnection GetDatabaseConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        // Получение списка преподавателей
        private List<string> GetTeachers()
        {
            List<string> teachers = new List<string>();

            try
            {
                using (var connection = GetDatabaseConnection())
                {
                    connection.Open();
                    string query = "SELECT Name FROM Teachers";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                teachers.Add(reader["Name"].ToString());
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при получении преподавателей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return teachers;
        }

        // Получение списка всех предметов
        private List<string> GetSubjects()
        {
            List<string> subjects = new List<string>();

            try
            {
                using (var connection = GetDatabaseConnection())
                {
                    connection.Open();
                    string query = "SELECT Name FROM Subjects";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                subjects.Add(reader["Name"].ToString());
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при получении предметов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return subjects;
        }

        // Конструктор окна
        public EditScheduleWindow(string userem)
        {
            userEmail = userem;
            InitializeComponent();
            TeacherListBox.ItemsSource = GetTeachers();
            SubjectListBox.ItemsSource = GetSubjects();
        }

        // Сохранение расписания
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string teacherName = TeacherListBox.SelectedItem?.ToString();
            string subjectName = SubjectListBox.SelectedItem?.ToString();
            int scheduleTime = -1;

            // Получение выбранного номера пары
            if (LessonListBox.SelectedItem is ListBoxItem selectedItem &&
                int.TryParse(selectedItem.Content.ToString(), out scheduleTime))
            {
                DateTime? selectedDate = ScheduleDatePicker.SelectedDate;

                if (string.IsNullOrEmpty(teacherName) || string.IsNullOrEmpty(subjectName) || !selectedDate.HasValue)
                {
                    MessageBox.Show("Заполните все поля перед сохранением.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    int teacherId = GetTeacherId(teacherName);
                    int subjectId = GetSubjectId(subjectName);

                    string query = @"
                        INSERT INTO Schedules (TeacherId, SubjectId, PairNumber, ScheduleDate)
                        VALUES (@TeacherId, @SubjectId, @PairNumber, @ScheduleDate);";

                    using (var connection = GetDatabaseConnection())
                    {
                        connection.Open();
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@TeacherId", teacherId);
                            command.Parameters.AddWithValue("@SubjectId", subjectId);
                            command.Parameters.AddWithValue("@PairNumber", scheduleTime);
                            command.Parameters.AddWithValue("@ScheduleDate", selectedDate.Value.ToString("yyyy-MM-dd"));

                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Расписание успешно сохранено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Ошибка при сохранении расписания: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите корректный номер пары.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Удаление расписания
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string teacherName = TeacherListBox.SelectedItem?.ToString();
            string subjectName = SubjectListBox.SelectedItem?.ToString();

            if (LessonListBox.SelectedItem is ListBoxItem selectedItem &&
                int.TryParse(selectedItem.Content.ToString(), out int scheduleTime))
            {
                DateTime? selectedDate = ScheduleDatePicker.SelectedDate;

                if (string.IsNullOrEmpty(teacherName) || string.IsNullOrEmpty(subjectName) || !selectedDate.HasValue)
                {
                    MessageBox.Show("Заполните все поля перед удалением.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    int teacherId = GetTeacherId(teacherName);
                    int subjectId = GetSubjectId(subjectName);

                    string query = @"
                        DELETE FROM Schedules 
                        WHERE TeacherId = @TeacherId AND SubjectId = @SubjectId 
                        AND PairNumber = @PairNumber AND ScheduleDate = @ScheduleDate";

                    using (var connection = GetDatabaseConnection())
                    {
                        connection.Open();
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@TeacherId", teacherId);
                            command.Parameters.AddWithValue("@SubjectId", subjectId);
                            command.Parameters.AddWithValue("@PairNumber", scheduleTime);
                            command.Parameters.AddWithValue("@ScheduleDate", selectedDate.Value.ToString("yyyy-MM-dd"));

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                                MessageBox.Show("Расписание успешно удалено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            else
                                MessageBox.Show("Запись не найдена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Ошибка при удалении расписания: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите корректный номер пары.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Получение Id преподавателя
        public int GetTeacherId(string teacherName)
        {
            // Проверка на пустое или null значение

            // Убираем лишние пробелы и приводим к нижнему регистру
            teacherName = teacherName.Trim();


            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    // SQL-запрос с учетом регистра и пробелов (LOWER используется для нечувствительности к регистру)
                    string query = "SELECT Id FROM Teachers WHERE BINARY Name = @TeacherName";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Добавляем параметр и значение
                        command.Parameters.Add("@TeacherName", MySqlDbType.VarChar).Value = teacherName;

                        // Выполняем запрос и получаем результат
                        var result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int teacherId))
                        {
                            return teacherId;
                        }
                        return 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }




        // Получение Id предмета
        private int GetSubjectId(string subjectName)
        {
            // Убираем пробелы и приводим имя к нижнему регистру
            subjectName = subjectName.Trim();


            try
            {
                using (var connection = GetDatabaseConnection())
                {
                    connection.Open();

                    // SQL-запрос для поиска ID предмета (с учетом регистра и пробелов)
                    string query = "SELECT Id FROM Subjects WHERE BINARY Name = @SubjectName";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Добавляем параметр и значение
                        command.Parameters.Add("@SubjectName", MySqlDbType.VarChar).Value = subjectName;

 

                        // Выполняем запрос и получаем результат
                        var result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int subjectId))
                        {
                            return subjectId;
                        }
                        return 0;
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            BasicWindow basicWindow = new BasicWindow("Admin", userEmail);
            basicWindow.Show();
            this.Close();
        }
    }
}
