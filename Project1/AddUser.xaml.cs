using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Project1
{
    /// <summary>
    /// Логика взаимодействия для AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        static readonly string _connectionString = "Server=sql7.freemysqlhosting.net ;Database=sql7752662;User=sql7752662;Password=q4QaT7UgZR;Port=3306;";

        string usem;
        public AddUser(string userem)
        {
            InitializeComponent();
            usem = userem; // Сохраняем значение
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = EmailTextBox.Text.Trim();
            string password = PasswordBox.Text.Trim();
            string fullName = TeacherNameTextBox.Text.Trim();

            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Выберите роль.");
                return;
            }

            role = role == "Администратор" ? "Admin" : "Teacher";

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Введите логин, пароль и ФИО.");
                return;
            }

            string userQuery = "INSERT INTO Users(Login, Password, Role) VALUES (@Login, @Password, @Role); SELECT LAST_INSERT_ID();";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    long userId;
                    using (MySqlCommand command = new MySqlCommand(userQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Login", login);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@Role", role);

                        // Получаем ID последнего вставленного пользователя
                        userId = Convert.ToInt64(command.ExecuteScalar());
                    }

                    if (userId > 0)
                    {
                        string teacherQuery = "INSERT INTO Teachers(UserId, Name) VALUES (@UserId, @Name)";
                        using (MySqlCommand teacherCommand = new MySqlCommand(teacherQuery, connection))
                        {
                            teacherCommand.Parameters.AddWithValue("@UserId", userId);
                            teacherCommand.Parameters.AddWithValue("@Name", fullName);

                            int rowsAffectedTeacher = teacherCommand.ExecuteNonQuery();
                            if (rowsAffectedTeacher > 0)
                            {
                                MessageBox.Show("Пользователь успешно добавлен.");
                            }
                            else
                            {
                                MessageBox.Show("Не удалось добавить преподавателя.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось добавить пользователя.");
                        return;
                    }

                    // Переход на BasicWindow
                    BasicWindow basicWindow = new BasicWindow("Admin", usem);
                    basicWindow.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            BasicWindow basicWindow = new BasicWindow("Admin", usem);
            basicWindow.Show();
            this.Close();
        }
    }
}
