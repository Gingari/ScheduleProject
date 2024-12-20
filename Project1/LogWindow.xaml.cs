using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Project1
{
    /// <summary>
    /// Логика взаимодействия для LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        static readonly string _connectionString = "Server=sql7.freemysqlhosting.net ;Database=sql7752662;User=sql7752662;Password=q4QaT7UgZR;Port=3306;";
        public LogWindow()
        {
            InitializeComponent();

        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Text.Trim();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    // Проверяем, существует ли email
                    string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Login = @Login;";
                    using (MySqlCommand command = new MySqlCommand(checkEmailQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Login", email);
                        int emailCount = Convert.ToInt32(command.ExecuteScalar());

                        if (emailCount == 0)
                        {
                            MessageBox.Show("Пользователь с таким email не зарегистрирован. Пожалуйста, зарегистрируйтесь.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    // Проверяем пароль
                    string checkPasswordQuery = "SELECT Password FROM Users WHERE Login = @Login;";
                    using (MySqlCommand command = new MySqlCommand(checkPasswordQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Login", email);

                        string storedPassword = command.ExecuteScalar()?.ToString();
                        if (storedPassword != null && storedPassword == password)
                        {
                            // Получаем роль пользователя
                            string getUserRoleQuery = "SELECT Role FROM Users WHERE Login = @Login;";
                            using (MySqlCommand roleCommand = new MySqlCommand(getUserRoleQuery, connection))
                            {
                                roleCommand.Parameters.AddWithValue("@Login", email);
                                string role = roleCommand.ExecuteScalar()?.ToString();

                                BasicWindow basicWindow = new BasicWindow(role, email);
                                basicWindow.Show();
                                this.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверный пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
