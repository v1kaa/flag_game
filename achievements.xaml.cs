using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp6
{
    public partial class achievements : UserControl
    {
        private string connectionString = "Data Source=LAPTOP-FQBEUBB7\\VIKA;Initial Catalog=users;Integrated Security=True";
        private string user_name;
        private string user_pass;

        public achievements(string name, string pass)
        {
            InitializeComponent();
            user_name = name;
            user_pass = pass;
            display_achievements();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window w = Window.GetWindow(this);
            w.Content = new user_page(user_name, user_pass);
        }

        private void display_achievements()
        {
            try
            {
                DataTable achievementsTable = new DataTable();
                achievementsTable.Columns.Add("Description", typeof(string));
                achievementsTable.Columns.Add("Count", typeof(int));

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT descriptions.description, ISNULL(COUNT(achievements.user_name), 0) as count
                FROM (
                    SELECT DISTINCT description
                    FROM [dbo].achievements
                ) descriptions
                LEFT JOIN [dbo].achievements ON descriptions.description = achievements.description AND achievements.user_name = @user_name
                GROUP BY descriptions.description";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_name", user_name);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string description = reader["description"].ToString();
                                int count = Convert.ToInt32(reader["count"]);
                                achievementsTable.Rows.Add(description, count);
                            }
                        }
                    }
                }

                tableDataGrid.ItemsSource = achievementsTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


    }
}

