using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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

namespace WpfApp6
{
    /// <summary>
    /// Interaction logic for user_page.xaml
    /// </summary>
    public partial class user_page : UserControl
    {
        private string connectionString = "Data Source=LAPTOP-FQBEUBB7\\VIKA;Initial Catalog=users;Integrated Security=True";
        private string name_user;
        private int user_id;
        private string password_user;
        private string help;
        public user_page(string user, string pass)
        {
            this.name_user = user;
            password_user = pass;

            InitializeComponent();
            DisplayTableData(name_user);
            hello_user.Content = " hello there " + user + "\n dont forget your pass: "+ pass;
        }

        private void DisplayTableData(string username)
        {
            // DataTable to hold the results from the database
            DataTable dataTable = new DataTable();

            try
            {
                // Open connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // SQL query to retrieve the user ID based on the username
                    string getUserIdQuery = "SELECT Id FROM users WHERE user_name = @username";

                    // Create a SqlCommand object for retrieving the user ID
                    using (SqlCommand getUserIdCommand = new SqlCommand(getUserIdQuery, connection))
                    {
                        getUserIdCommand.Parameters.AddWithValue("@username", username);

                        // Open the connection
                        connection.Open();

                        // Execute the command to retrieve the user ID
                        object result = getUserIdCommand.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            int userId = Convert.ToInt32(result);

                            // Przypisanie wartości user_id
                            this.user_id = userId;

                            // SQL query to retrieve data from the Table table using the user ID
                            // SQL query to retrieve data from the Table table using the user ID
                            string query = "SELECT time, date, uncorrect FROM [dbo].[Table] WHERE user_id = @userId";


                            // Create a SqlCommand object for retrieving data from the Table table
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@userId", userId);

                                // Create a SqlDataAdapter to fill the DataTable
                                SqlDataAdapter adapter = new SqlDataAdapter(command);

                                // Fill the DataTable with data from the database
                                adapter.Fill(dataTable);
                            }
                        }
                        else
                        {
                            MessageBox.Show("User ID not found for the given username.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

            // Assign the DataTable as the ItemsSource for the DataGrid
            tableDataGrid.ItemsSource = dataTable.DefaultView;
        }


        private void start_Click(object sender, RoutedEventArgs e)
        {
        
            Window w = Window.GetWindow(this);
            if (help == "1") { w.Content = new first_one(name_user, user_id); }
            else if(help=="2") { w.Content = new country_to_flags(name_user, user_id); } else
            {
                MessageBox.Show("choose a type of game  !!!");
            }
        }


        private void learning_Click(object sender, RoutedEventArgs e)
        {
            Window w = Window.GetWindow(this);
            w.Content = new all_flags(name_user, password_user);

        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            Window w= Window.GetWindow(this);
            w.Content = new login();
        }

        private void selectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)selectionComboBox.SelectedItem;

            if (selectedItem != null)
            {
                string selectedTag = selectedItem.Tag.ToString();
                switch (selectedTag)
                {
                    case "FlagToCountries":
                        help = "1";
                         break;
                    case "CountryToFlags":
                        help = "2";                      
                        break;
                }
            }
        }

    }
}