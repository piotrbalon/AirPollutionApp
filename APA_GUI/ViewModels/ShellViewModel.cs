using APA_Library;
using APA_Library.Helpers;
using APA_Library.Models;
using Caliburn.Micro;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Microsoft.Toolkit.Wpf.UI.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace APA_GUI.ViewModels
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class ShellViewModel : Conductor<object>
    {
        private CountryModel selectedCountry { get; set; }
        private CityModel selectedCity { get; set; }
        private StationModel selectedStation { get; set; }
        private PollutantModel selectedPollutant { get; set; }
        private DateTime selectedDateFrom { get; set; }
        private DateTime selectedDateTo { get; set; }

        public BindableCollection<CountryModel> Countries { get; set; }
        public BindableCollection<CityModel> Cities { get; set; }
        public BindableCollection<StationModel> Stations { get; set; }
        public BindableCollection<PollutantModel> Pollutants { get; set; }

        public int CountriesCount { get; set; }
        public int CitiesCount { get; set; }
        public int StationsCount { get; set; }

        public readonly DateTime MinimumFromDate = DateTime.UtcNow.AddDays(-90);
        public readonly DateTime MaximumToDate = DateTime.UtcNow;

        public CountryModel SelectedCountry
        {
            get => selectedCountry;
            set
            {
                selectedCountry = value;
                _ = LoadCities();
                _ = LoadStations();
            }
        }
        public CityModel SelectedCity
        {
            get => selectedCity;
            set
            {
                selectedCity = value;
                _ = LoadStations();
            }
        }
        public StationModel SelectedStation { get => selectedStation; set => selectedStation = value; }
        public PollutantModel SelectedPollutant
        {
            get => selectedPollutant;
            set
            {
                selectedPollutant = value;
                _ = LoadStations();
            }
        }

        public DateTime SelectedDateFrom
        {
            get => selectedDateFrom;
            set
            {
                selectedDateFrom = value;
                NotifyOfPropertyChange(() => SelectedDateFrom);
            }
        }

        public DateTime SelectedDateTo
        {
            get => selectedDateTo;
            set
            {
                selectedDateTo = value;
                NotifyOfPropertyChange(() => SelectedDateTo);
            }
        }

        public ShellViewModel()
        {
            ApiHelper.InitializeClient();
            Pollutants = new BindableCollection<PollutantModel>(PollutantModel.AllPollutants);
            Countries = new BindableCollection<CountryModel>();
            Cities = new BindableCollection<CityModel>();
            Stations = new BindableCollection<StationModel>();
            SelectedDateFrom = MinimumFromDate;
            SelectedDateTo = MaximumToDate;
            _ = LoadCountries();
        }

        private async Task LoadCountries()
        {
            GetResultModel<CountryModel[]> countriesData = await CountriesProcessing.LoadCountries();
            Countries = new BindableCollection<CountryModel>(countriesData.Results);
            NotifyOfPropertyChange(() => Countries);
            CountriesCount = countriesData.Meta.Found;
        }

        private async Task LoadCities()
        {
            GetResultModel<CityModel[]> citiesData = await CitiesProcessing.LoadCities(selectedCountry);
            Cities = new BindableCollection<CityModel>(citiesData.Results);
            NotifyOfPropertyChange(() => Cities);
            CitiesCount = citiesData.Meta.Found;
        }

        private async Task LoadStations()
        {
            GetResultModel<StationModel[]> stationsData = await StationsProcessing.LoadStations(selectedCountry, selectedCity, selectedPollutant);
            Stations = new BindableCollection<StationModel>(stationsData.Results);
            NotifyOfPropertyChange(() => Stations);
            StationsCount = stationsData.Meta.Found;
            NotifyOfPropertyChange(() => StationsCount);
        }

        private async void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Specify a known location.
            BasicGeoposition cityPosition = new BasicGeoposition() { Latitude = 47.604, Longitude = -122.329 };
            var cityCenter = new Geopoint(cityPosition);

            // Set the map location.
            await ((MapControl)sender).TrySetViewAsync(cityCenter, 12);
        }
    }
}
