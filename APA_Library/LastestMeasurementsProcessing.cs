using APA_Library.Helpers;
using APA_Library.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace APA_Library
{
    public class LastestMeasurementsProcessing
    {
        private static string apiEndpoint = ApiHelper.AirQualityApiEndpoint + "latest?limit=10000";

        public static async Task<List<LatestMeasurementsModel>> LoadLatestMeasurements(
            PollutantModel pollutant,
            CountryModel country = null,
            CityModel city = null
        )
        {
            if (pollutant is null)
                throw new ArgumentNullException("Air pollutant cannot be null.");

            string url = $"{apiEndpoint}?has_geo=true&parameter={pollutant.Id}";

            if (!(country is null))
                url += $"&country={country.Code}";

            if (!(city is null))
                url += "&city=" + city.Name;

            List<LatestMeasurementsModel> measurements = new List<LatestMeasurementsModel>();

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

        private static async Task<int> FetchAndDeserializeMeasurements(List<LatestMeasurementsModel> measurements, string url, int page = 1)
        {
            string pagedUrl = $"{url}&page={page}&limit=10000";

            HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(pagedUrl);
            if (response.IsSuccessStatusCode)
            {
                GetResultModel<LatestMeasurementsModel[]> data = await response.Content.ReadAsAsync<GetResultModel<LatestMeasurementsModel[]>>();
                measurements.AddRange(data.Results);
                return data.Meta.Found;
            }

            throw new Exception(response.ReasonPhrase);
        }
    }
}
