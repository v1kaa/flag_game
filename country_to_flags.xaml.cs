using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp6
{
    /// <summary>
    /// Interaction logic for country_to_flags.xaml
    /// </summary>
    public partial class country_to_flags : UserControl
    {
        private string connectionString = "Data Source=LAPTOP-FQBEUBB7\\VIKA;Initial Catalog=users;Integrated Security=True";
        private string filePath = @"C:\Users\Acer\Downloads\Country.json";
        private int score = 0;
        private int uncorrect_answer = 0;
        private DispatcherTimer timer_object;
        private DateTime startTime;

        private JObject country1;

        private string endTimeResult;
        private int userId;
        private string user_name;
        private string pass;
        string flagLink1;
        string flagLink2;
        public country_to_flags(string user, int user_id)
        {

            InitializeComponent();
            user_name = user;
            userId= user_id;
            startTime = DateTime.Now;
            timer_object = new DispatcherTimer();
            timer_object.Interval = TimeSpan.FromSeconds(1);
            timer_object.Tick += Timer_Tick;
            timer_object.Start();
            choose_new_country();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Calculate the elapsed time since the start time
            TimeSpan elapsedTime = DateTime.Now - startTime;

            // Update the label with the elapsed time formatted as HH:mm:ss
            timer.Content = elapsedTime.ToString(@"hh\:mm\:ss");
            endTimeResult = elapsedTime.ToString(@"hh\:mm\:ss");
        }

        public void choose_new_country()
        {
            try
            {
                string jsonData = System.IO.File.ReadAllText(filePath);
                JArray countries = JArray.Parse(jsonData);
                int numberOfCountries = countries.Count;

                Random rand = new Random();

                // Randomly select which button will display the correct flag
                bool flag1IsCorrect = rand.Next(0, 2) == 0;

                int index1 = rand.Next(0, numberOfCountries);
                JObject country1 = (JObject)countries[index1];

                // Get the name of the first country
                string countryName = country1["name"]["common"].ToString();
                country.Content = countryName;

                // Randomly select the index for the second country (different from the first)
                int index2;
                do
                {
                    index2 = rand.Next(0, numberOfCountries);
                } while (index2 == index1);
                JObject country2 = (JObject)countries[index2];

                // Get the URLs of the flags of the two countries
                flagLink1 = country1["flags"]["png"].ToString();
                flagLink2 = country2["flags"]["png"].ToString();

                // Assign the flags to the buttons based on the random selection
                if (flag1IsCorrect)
                {
                    Flag1Button.Content = new Image { Source = new BitmapImage(new Uri(flagLink1)) };
                    Flag21Button.Content = new Image { Source = new BitmapImage(new Uri(flagLink2)) };
                }
                else
                {
                    Flag1Button.Content = new Image { Source = new BitmapImage(new Uri(flagLink2)) };
                    Flag21Button.Content = new Image { Source = new BitmapImage(new Uri(flagLink1)) };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while choosing new flag data: " + ex.Message);
            }
        }





        private void check(string selectedPhotoUrl)
        {
            if (selectedPhotoUrl == flagLink1)
            {
                MessageBox.Show("Correct");
                score++;
                score_label.Content = "score: " + score + "/20";
                choose_new_country();

                if (score == 5)
                {
                    timer_object.Stop();
                    MessageBox.Show("You won!");
                    AddRowToTable();
                    score = 0;
                    score_label.Content = "score: 0/20";

                }
            }
            else
            {
                uncorrect_answer++;
                MessageBox.Show("Incorrect");
                choose_new_country();
            }
        }

        private void Flag1Button_Click(object sender, RoutedEventArgs e)
        {
            // Extract the URL of the flag image from the button's content
            var selectedImage = (Image)Flag1Button.Content;
            var selectedPhotoUrl = ((BitmapImage)selectedImage.Source).UriSource.ToString();
            check(selectedPhotoUrl);
        }

        private void Flag21Button_Click(object sender, RoutedEventArgs e)
        {
            // Extract the URL of the flag image from the button's content
            var selectedImage = (Image)Flag21Button.Content;
            var selectedPhotoUrl = ((BitmapImage)selectedImage.Source).UriSource.ToString();
            check(selectedPhotoUrl);
        }

        private void AddRowToTable()
        {
            string insertQuery = "INSERT INTO [dbo].[Table] (user_id, time, date, uncorrect, type_of_game) VALUES (@userId, @time, @date, @uncorrect,@game)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@uncorrect", uncorrect_answer);
                    command.Parameters.AddWithValue("@time", endTimeResult); // Using the result of the timer
                    command.Parameters.AddWithValue("@date", DateTime.Now.Date.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@game", "country to flagS");

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("New row added to the table.");
                            Window w = Window.GetWindow(this);
                            w.Content = new user_page(user_name, GetPasswordFromDatabase(userId));
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




    }
}
