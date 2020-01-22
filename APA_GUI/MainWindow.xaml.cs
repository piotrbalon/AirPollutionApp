using APA_Model;
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

namespace APA_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ApiHelper.InitializeClient();
        }

        private async Task LoadCountries()
        {
            countries.ItemsSource = await Countries.LoadCountries();
        }

        private async Task LoadCities(string country)
        {
            cities.ItemsSource = await Cities.LoadCities(country);
        }

        private async void Country_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCountry = countries.SelectedItem.ToString();
            countryTextbox.Text = selectedCountry;

            await LoadCities(selectedCountry);
        }

        private void City_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCity = cities.SelectedItem.ToString();
            cityTextbox.Text = selectedCity;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCountries();
        }
    }
}
