using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Data.SqlClient;
using System;

namespace WpfApp6
{
    /// <summary>
    /// Interaction logic for login.xaml
    /// </summary>
    /// String str;

    public partial class login : UserControl
    {
        string connectionString = "Data Source=LAPTOP-FQBEUBB7\\VIKA;Initial Catalog=users;Integrated Security=True";
        public login()
        {
            InitializeComponent();

        }
        private void login_user_name_GotFocus(object sender, RoutedEventArgs e)
        {
            login_user_name.Clear();
        }

        private void login_user_password_GotFocus(object sender, RoutedEventArgs e)
        {
            login_user_password.Clear();
        }

        private void signin_user_name_GotFocus(object sender, RoutedEventArgs e)
        {
            signin_user_name.Clear();
        }

        private void signin_user_password_GotFocus(object sender, RoutedEventArgs e)
        {
            signin_user_password.Clear();
        }




        // Obsługa kliknięcia przycisku Zaloguj
        // Obsługa kliknięcia przycisku Zaloguj
        private void log_in_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM users WHERE user_name = @UserName AND user_pass = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", login_user_name.Text);
                    command.Parameters.AddWithValue("@Password", login_user_password.Password);

                    try
                    {
                        connection.Open();
                        int userCount = (int)command.ExecuteScalar();

                        if (userCount > 0)
                        {
                            Window w = Window.GetWindow(this);
                            w.Content = new user_page(login_user_name.Text, login_user_password.Password);
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
        }


        // Obsługa kliknięcia przycisku Zarejestruj
        private void sign_in_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string checkQuery = "SELECT COUNT(*) FROM users WHERE user_name = @UserName";

                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@UserName", signin_user_name.Text);

                    try
                    {
                        connection.Open();
                        int userCount = (int)checkCommand.ExecuteScalar();

                        if (userCount > 0)
                        {
                            MessageBox.Show("Username already exists. Please choose a different username.");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while checking username: " + ex.Message);
                        return;
                    }
                    connection.Close();
                }

                string insertQuery = "INSERT INTO users (user_name, user_pass) VALUES (@UserName, @Password)";

                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@UserName", signin_user_name.Text);
                    insertCommand.Parameters.AddWithValue("@Password", signin_user_password.Text);

                    try
                    {
                        connection.Open();
                        int rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User signed in successfully!");
                            // Close the current window
                            Window w = Window.GetWindow(this);

                            // Open the user page
                            w.Content = new user_page(signin_user_name.Text, signin_user_password.Text);
                            
                        }
                        else
                        {
                            MessageBox.Show("Failed to sign in user.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while signing in: " + ex.Message);
                    }
                }
            }
        }




    }
}
