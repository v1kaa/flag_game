using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfApp6
{
    public partial class first_one : UserControl
    {
        private string connectionString = "Data Source=LAPTOP-FQBEUBB7\\VIKA;Initial Catalog=users;Integrated Security=True";
        private string filePath = @"C:\Users\Acer\Downloads\Country.json";
        private int uncorrect_answers = 0;
        private JObject country;
        private int score = 0;
        private DispatcherTimer timer_object;
        private DateTime startTime;
        private string endTimeResult;
        private int userId;
        private string user_name;
        private string pass;// Assuming you have the user ID available in this class

        public first_one(string user, int user_id)
        {
            startTime = DateTime.Now;
            InitializeComponent();
            user_label.Content = user;
            userId = user_id;
            user_name = user;


            





            // Start the timer when the user control is initialized
           // startTime = DateTime.Now;
            timer_object = new DispatcherTimer();
            timer_object.Interval = TimeSpan.FromSeconds(1);
            timer_object.Tick += Timer_Tick;
            timer_object.Start();

            choose_new_flag_data();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Calculate the elapsed time since the start time
            TimeSpan elapsedTime = DateTime.Now - startTime;

            // Update the label with the elapsed time formatted as HH:mm:ss
            timer.Content = elapsedTime.ToString(@"hh\:mm\:ss");
            endTimeResult = elapsedTime.ToString(@"hh\:mm\:ss");
        }

        public void choose_new_flag_data()
        {
            string jsonData = System.IO.File.ReadAllText(filePath);
            JArray countries = JArray.Parse(jsonData);
            int numberOfCountries = countries.Count;
            Random rand = new Random();

            // Wylosuj indeks przycisku dla poprawnej odpowiedzi
            int correctButtonIndex = rand.Next(0, 2);

            int index1 = rand.Next(0, numberOfCountries);
            country = (JObject)countries[index1];
            string flagLink = country["flags"]["png"].ToString();

            Flag.Source = new BitmapImage(new Uri(flagLink));

            // Ustaw odpowiedni przycisk dla poprawnej odpowiedzi
            if (correctButtonIndex == 0)
            {
                button_one.Content = country["name"]["common"].ToString();
            }
            else
            {
                button_second.Content = country["name"]["common"].ToString();
            }

            // Wybierz indeks dla drugiego kraju różny od indeksu pierwszego kraju
            int index2;
            do
            {
                index2 = rand.Next(0, numberOfCountries);
            } while (index2 == index1);

            JObject country2 = (JObject)countries[index2];

            // Ustaw drugi przycisk dla drugiego kraju
            if (correctButtonIndex == 0)
            {
                button_second.Content = country2["name"]["common"].ToString();
            }
            else
            {
                button_one.Content = country2["name"]["common"].ToString();
            }
        }

        private void Check_if_button_one_is_correct_Click(object sender, RoutedEventArgs e)
        {
            CheckAnswer(button_one.Content.ToString());
        }

        private void Check_if_button_second_is_correct_Click(object sender, RoutedEventArgs e)
        {
            CheckAnswer(button_second.Content.ToString());
        }

        private void CheckAnswer(string selectedAnswer)
        {
            if (country["name"]["common"].ToString() == selectedAnswer)
            {
                score++;
                your_score.Text = "Your score: " + score + "/20";
                choose_new_flag_data();
            }
            else
            {
                MessageBox.Show("Incorrect answer");
                uncorrect_answers++;
                choose_new_flag_data();
            }

            if (score == 3)
            {
                // Stop the timer
                timer_object.Stop();

                // Show win message
                MessageBox.Show("You won!");

                // Add a new row to the Table table with the result of the timer and user ID
                AddRowToTable();

                // Reset score
                score = 0;
                your_score.Text = "Your score: " + score + "/20";
            }
        }



        private string GetPasswordFromDatabase(int userId)
        {
            string password = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT user_pass FROM users WHERE Id = @userId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        connection.Open();
                        password = (string)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while retrieving password: " + ex.Message);
            }

            return password;
        }
        private void AddRowToTable()
        {
            string insertQuery = "INSERT INTO [dbo].[Table] (user_id, time, date, uncorrect,type_of_game) VALUES (@userId, @time, @date, @uncorrect, @game)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@uncorrect", uncorrect_answers);
                    command.Parameters.AddWithValue("@time", endTimeResult); // Using the result of the timer
                    command.Parameters.AddWithValue("@date", DateTime.Now.Date.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@game", "flag to coutnrieS");

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("New row added to the table.");
                            Window w = Window.GetWindow(this);
                            w.Content = new user_page(user_name,GetPasswordFromDatabase(userId) );
                        }
                        else
                        {
                            MessageBox.Show("Failed to add a new row to the table.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
        }
    }
}
