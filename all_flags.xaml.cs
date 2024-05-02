using Newtonsoft.Json.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp6
{
    /// <summary>
    /// Interaction logic for all_flags.xaml
    /// </summary>
    public partial class all_flags : UserControl
    {
        private string filePath = @"C:\Users\Acer\Downloads\Country.json";
        public all_flags()
        {
            InitializeComponent();
            DisplayAllFlags();
        }

        private void DisplayAllFlags()
        {
            try
            {
                string jsonData = System.IO.File.ReadAllText(filePath);
                JArray countries = JArray.Parse(jsonData);

                foreach (JObject country in countries)
                {
                    string flagLink = country["flags"]["png"].ToString();
                    string countryName = country["name"]["common"].ToString();

                    // Tworzenie obiektu Image i ustawienie jego źródła na flagę
                    Image flagImage = new Image();
                    flagImage.Source = new BitmapImage(new Uri(flagLink));
                    flagImage.Width = 50;
                    flagImage.Height = 30;

                    // Tworzenie obiektu TextBlock i ustawienie jego tekstu na nazwę kraju
                    TextBlock countryTextBlock = new TextBlock();
                    countryTextBlock.Text = countryName;

                    // Tworzenie obiektu StackPanel, do którego dodane zostaną obraz i tekst
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Horizontal;
                    stackPanel.Children.Add(flagImage);
                    stackPanel.Children.Add(countryTextBlock);

                    // Dodanie stackPanel do głównego kontenera (np. ListBox lub StackPanel)
                    stackPanel.Margin = new Thickness(60, 30, 30, 10); // Dostosuj margines według potrzeb

                    // Dodanie stackPanel do głównego kontenera (np. ListBox lub StackPanel)
                    mainContainer.Children.Add(stackPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while displaying flags: " + ex.Message);
            }
        }
    }
}
