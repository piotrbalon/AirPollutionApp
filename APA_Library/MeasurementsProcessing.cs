﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using APA_Library.Helpers;
using System.Threading.Tasks;
using APA_Library.Models;

namespace APA_Library
{
    public class MeasurementsProcessing
    {
        private static string apiEndpoint = ApiHelper.ApiEndpoint + "measurements";

        public static async Task<List<LastestMeasurementsModel>> LoadMeasurements(
            CountryModel country = null,
            CityModel city = null,
            PollutantModel pollutant = null,
            StationModel station = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null
        )
        {
            string url = $"{apiEndpoint}?has_geo=true";

            if (!(country is null))
            {
                url += $"&country={country.Code}";
            }

            if (!(city is null))
                url += "&city=" + city.Name;

            if (!(pollutant is null))
            {
                url += $"&parameter={pollutant.Id}";
            }

            if (!(station is null))
            {
                url += $"&location={station.Location}";
            }

            if (!(dateFrom is null))
            {
                url += $"&date_from={dateFrom.Value.ToString("s", CultureInfo.InvariantCulture)}";
            }

            if (!(dateTo is null))
            {
                url += $"&date_to={dateTo.Value.ToString("s", CultureInfo.InvariantCulture)}";
            }

            List<LastestMeasurementsModel> measurements = new List<LastestMeasurementsModel>();

            // first call to api to find out how many measurements are there
            int totalMeasurements = await FetchAndDeserializeMeasurements(measurements, url, 1);
            int pages = totalMeasurements / 10000;

            // fetch all pages
            Task[] tasks = new Task[pages];
            for (int page = 2; page <= pages; page++)
            {
                tasks[page] = FetchAndDeserializeMeasurements(measurements, url, page);
            }

            // wait until all pages are fetched
            await Task.WhenAll(tasks);

            return measurements;
        }

        private static async Task<int> FetchAndDeserializeMeasurements(List<LastestMeasurementsModel> measurements, string url, int page = 1)
        {
            string pagedUrl = $"{url}&page={page}&limit=10000";

            HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(pagedUrl);
            if (response.IsSuccessStatusCode)
            {
                GetResultModel<LastestMeasurementsModel[]> data = await response.Content.ReadAsAsync<GetResultModel<LastestMeasurementsModel[]>>();
                measurements.AddRange(data.Results);
                return data.Meta.Found;
            }

            throw new Exception(response.ReasonPhrase);
        }
    }
}
