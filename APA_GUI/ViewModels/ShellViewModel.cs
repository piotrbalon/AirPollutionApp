﻿using APA_Library;
using APA_Library.Helpers;
using APA_Library.Models;
using Caliburn.Micro;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Microsoft.Toolkit.Wpf.UI.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using LiveCharts;
using LiveCharts.Wpf;

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

        public BindableCollection<CountryModel> Countries { get; set; }
        public BindableCollection<CityModel> Cities { get; set; }
        public BindableCollection<StationModel> Stations { get; set; }
        public BindableCollection<PollutantModel> Pollutants { get; set; }
        public BindableCollection<LastestMeasurementsModel> Measurements { get; set; }

        public string MeasurementsMessage { get; set; }
        public int CountriesCount { get; set; }
        public int CitiesCount { get; set; }
        public int StationsCount { get; set; }
        public int MeasurementsCount { get; set; }

        public DateTime MinimumFromDate { get; } = DateTime.UtcNow.AddDays(-90);
        public DateTime MaximumToDate { get; } = DateTime.UtcNow;

        public CountryModel SelectedCountry
        {
            get => selectedCountry;
            set
            {
                selectedCountry = value;
                _ = LoadCities();
                _ = LoadStations();
                _ = LoadMeasurements();
            }
        }
        public CityModel SelectedCity
        {
            get => selectedCity;
            set
            {
                selectedCity = value;
                _ = LoadStations();
                _ = LoadMeasurements();
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
                _ = LoadMeasurements();
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

        private async Task LoadMeasurements()
        {
            if (selectedCountry is null || selectedPollutant is null)
            {
                MeasurementsMessage = "You need to select pollutant and country.";
                return;
            }

            MeasurementsMessage = "";
            List<LastestMeasurementsModel> measurements = await MeasurementsProcessing.LoadMeasurements(selectedCountry, selectedCity, selectedPollutant, selectedStation, selectedDateFrom, selectedDateTo);
            Measurements = new BindableCollection<LastestMeasurementsModel>(measurements);
            NotifyOfPropertyChange(() => Measurements);
            MeasurementsCount = measurements.Count;
            NotifyOfPropertyChange(() => MeasurementsCount);
        }

        private async void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Specify a known location.
            BasicGeoposition cityPosition = new BasicGeoposition() { Latitude = 50.061, Longitude = 19.936 };
            var cityCenter = new Geopoint(cityPosition);

            // Set the map location.
            await ((MapControl)sender).TrySetViewAsync(cityCenter, 12);
        }
    }
}
