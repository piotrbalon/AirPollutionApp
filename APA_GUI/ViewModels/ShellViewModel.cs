using APA_Library;
using APA_Library.Helpers;
using APA_Library.Models;
using Caliburn.Micro;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        private DateTime? selectedDateFrom { get; set; }
        private DateTime? selectedDateTo { get; set; }
        private Location mapCenter { get; set; } = new Location();
        private int mapZoomLevel { get; set; } = 3;

        private BindableCollection<UIElement> mapElements = new BindableCollection<UIElement>();
        private UIElement pinInfobox = new UIElement();

        public BindableCollection<CountryModel> Countries { get; set; }
        public BindableCollection<CityModel> Cities { get; set; }
        public BindableCollection<StationModel> Stations { get; set; }
        public BindableCollection<PollutantModel> Pollutants { get; set; }
        public BindableCollection<MeasurementsModel> Measurements { get; set; }

        public BindableCollection<UIElement> MapElements
        {
            get => mapElements;
            set
            {
                mapElements = value;
                NotifyOfPropertyChange(() => MapElements);
            }
        }

        public UIElement PinInfobox
        {
            get => pinInfobox;
            set
            {
                pinInfobox = value;
                NotifyOfPropertyChange(() => PinInfobox);
            }
        }

        public string MeasurementsMessage { get; set; }
        public int CountriesCount { get; set; }
        public int CitiesCount { get; set; }
        public int StationsCount { get; set; }
        public int MeasurementsCount { get; set; }

        public DateTime MinimumFromDate { get; } = DateTime.UtcNow.AddDays(-3);
        public DateTime MaximumToDate { get; } = DateTime.UtcNow;

        public Map Map { get; set; } = new Map();

        public CountryModel SelectedCountry
        {
            get => selectedCountry;
            set
            {
                selectedCountry = value;
                mapZoomLevel = 5;
                _ = LoadCities();
                _ = LoadStations();
                _ = LoadMeasurements();
                _ = LoadMapLocation();
                _ = LoadLatestMeasurements();
            }
        }

        public CityModel SelectedCity
        {
            get => selectedCity;
            set
            {
                selectedCity = value;
                mapZoomLevel = 10;
                _ = LoadStations();
                _ = LoadMeasurements();
                _ = LoadMapLocation();
                _ = LoadLatestMeasurements();
            }
        }

        public StationModel SelectedStation
        {
            get => selectedStation;
            set
            {
                selectedStation = value;
                _ = LoadMeasurements();
                _ = LoadMapLocation();
                _ = LoadLatestMeasurements();
            }
        }

        public PollutantModel SelectedPollutant
        {
            get => selectedPollutant;
            set
            {
                selectedPollutant = value;
                _ = LoadStations();
                _ = LoadMeasurements();
                _ = LoadLatestMeasurements();
            }
        }

        public DateTime? SelectedDateFrom
        {
            get => selectedDateFrom;
            set
            {
                selectedDateFrom = value;
                NotifyOfPropertyChange(() => SelectedDateFrom);
                _ = LoadMeasurements();
            }
        }

        public DateTime? SelectedDateTo
        {
            get => selectedDateTo;
            set
            {
                selectedDateTo = value;
                NotifyOfPropertyChange(() => SelectedDateTo);
                _ = LoadMeasurements();
            }
        }

        public Location MapCenter
        {
            get => mapCenter;
            set
            {
                mapCenter = value;
                NotifyOfPropertyChange(() => MapCenter);
            }
        }

        public int MapZoomLevel
        {
            get => mapZoomLevel;
            set
            {
                mapZoomLevel = value;
                NotifyOfPropertyChange(() => MapZoomLevel);
            }
        }

        private BindableCollection<LatestMeasurementsModel> latestMeasurements { get; set; }

        public BindableCollection<LatestMeasurementsModel> LatestMeasurements
        {
            get => latestMeasurements;
            set
            {
                latestMeasurements = value;
                NotifyOfPropertyChange(() => LatestMeasurements);
            }
        }


        public ShellViewModel()
        {
            ApiHelper.InitializeClient();
            Pollutants = new BindableCollection<PollutantModel>(PollutantModel.AllPollutants);
            Countries = new BindableCollection<CountryModel>();
            Cities = new BindableCollection<CityModel>();
            Stations = new BindableCollection<StationModel>();

            SelectedDateTo = MaximumToDate;
            SelectedDateFrom = MinimumFromDate;

            _ = LoadCountries();
        }

        private async Task LoadCountries()
        {
            GetResultModel<CountryModel[]> countriesData = await CountriesProcessing.LoadCountries();
            Countries = new BindableCollection<CountryModel>(countriesData.Results);
            NotifyOfPropertyChange(() => Countries);
            CountriesCount = countriesData.Meta.Found;
        }

        private async Task LoadLatestMeasurements()
        {
            if (selectedPollutant is null)
                throw new NullReferenceException("You need to choose pollutant.");

            MapElements = new BindableCollection<UIElement>();
            NotifyOfPropertyChange(() => MapElements);
            List<LatestMeasurementsModel> data =
                await LastestMeasurementsProcessing.LoadLatestMeasurements(selectedPollutant, selectedCountry,
                    selectedCity);
            latestMeasurements = new BindableCollection<LatestMeasurementsModel>(data);
            DrawLastestMeasurementsOnMap();
        }

        public void DrawLastestMeasurementsOnMap()
        {
            PollutantModel pollutant =
                Pollutants.SingleOrDefault(pollutantModel => pollutantModel.Id == selectedPollutant.Id);
            if (pollutant is null)
                throw new NullReferenceException("could not find pollutant");

            foreach (LatestMeasurementsModel measurement in latestMeasurements)
            {
                Coordinates cords = measurement.Coordinates;
                LatestMeasurementsModel.MeasurementModel m = measurement.Measurements[0];

                if (cords is null)
                {
                    Console.WriteLine($"Null coordinates: {measurement.ToString()}");
                    continue;
                }

                PollutantModel.Limit limit =
                    pollutant.Limits.FirstOrDefault(l =>
                    {
                        if (m.Unit is null)
                        {
                            Console.WriteLine($"Null measurement unit: {measurement.ToString()}");
                            return false;
                        }

                        return m.Unit == l.Unit;
                    });

                if (limit is null)
                {
                    Console.WriteLine($"Could not find pollutant limit: {measurement.ToString()}");
                    continue;
                }

                Pushpin pin = new Pushpin();
                pin.Location = new Location(cords.Latitude, cords.Longitude);
                pin.Height = 5;
                pin.Width = 5;
                pin.Tag = $"{m.Value} {m.Unit} ({m.LastUpdated})";
                pin.Content = $"{m.Value} {m.Unit} ({m.LastUpdated})";
                ToolTipService.SetToolTip(pin, pin.Content);

                double mValue = measurement.Measurements[0].Value;
                if (mValue < limit.Value)
                    pin.Background = Brushes.Green;
                else if (mValue <= (2 * limit.Value))
                    pin.Background = Brushes.Yellow;
                else if (mValue <= (3 * limit.Value))
                    pin.Background = Brushes.Orange;
                else if (mValue <= (4 * limit.Value))
                    pin.Background = Brushes.Red;
                else if (mValue <= (5 * limit.Value))
                    pin.Background = Brushes.DarkRed;
                else if (mValue > (5 * limit.Value))
                    pin.Background = Brushes.Black;


                MapElements.Add(pin);
            }

            NotifyOfPropertyChange(() => MapElements);
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
            GetResultModel<StationModel[]> stationsData =
                await StationsProcessing.LoadStations(selectedCountry, selectedCity, selectedPollutant);
            Stations = new BindableCollection<StationModel>(stationsData.Results);
            NotifyOfPropertyChange(() => Stations);
            StationsCount = stationsData.Meta.Found;
            NotifyOfPropertyChange(() => StationsCount);
        }

        private async Task LoadMapLocation()
        {
            MapLocationModel coords = await MapLocationProcessing.LoadMapLocation(selectedCountry, selectedCity);
            mapCenter = new Location() { Latitude = coords.Lat, Longitude = coords.Lat };
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        class DateModel
        {
            public System.DateTime DateTime { get; set; }
            public double Avg { get; set; }
        }

        private async Task LoadMeasurements()
        {
            if (selectedPollutant is null)
            {
                MeasurementsMessage = "You need to select pollutant and country.";
                return;
            }

            MeasurementsMessage = "";
            List<MeasurementsModel> measurements = await MeasurementsProcessing.LoadMeasurements(selectedPollutant, selectedCountry, selectedCity, selectedStation, selectedDateFrom, selectedDateTo);
            Measurements = new BindableCollection<MeasurementsModel>(measurements);
            NotifyOfPropertyChange(() => Measurements);
            MeasurementsCount = measurements.Count;
            NotifyOfPropertyChange(() => MeasurementsCount);

            //Days
            CartesianMapper<DateModel> dayConfig = Mappers.Xy<DateModel>()
                .X(dateModel => dateModel.DateTime.Ticks / TimeSpan.FromHours(1).Ticks)
                .Y(dateModel => dateModel.Avg);
            //and the formatter
            Formatter = value => new DateTime((long)(value * TimeSpan.FromHours(1).Ticks)).ToString("MM/dd/yyyy HH:mm");
            NotifyOfPropertyChange(() => Formatter);

            IEnumerable<DateModel> values = measurements
                .GroupBy(m => m.Date.Utc)
                .Select(g => new DateModel { DateTime = g.Key, Avg = g.Average(s => s.Value) });


            SeriesCollection = new SeriesCollection(dayConfig);
            SeriesCollection.Add(
                new LineSeries
                {
                    Title = "Average from all stations",
                    Values = values.AsChartValues()
                }
            );

            NotifyOfPropertyChange(() => SeriesCollection);
        }
    }
}
