using APA_Library.Helpers;
using APA_Library.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace APA_Library
{
    public class MeasurementsProcessing
    {
        private static string apiEndpoint = ApiHelper.AirQualityApiEndpoint + "measurements";

        public static async Task<List<MeasurementsModel>> LoadMeasurements(
            PollutantModel pollutant,
            CountryModel country,
            CityModel city = null,
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

            List<MeasurementsModel> measurements = new List<MeasurementsModel>();

            // first call to api to find out how many measurements are there

            int totalMeasurements = await FetchAndDeserializeMeasurements(measurements, url, 1);

            if (totalMeasurements > 10000)
            {
                int pages = (int)Math.Ceiling((double)totalMeasurements / 10000);
                // fetch all pages
                Task[] tasks = new Task[pages - 1];
                for (int page = 2, i = 0; page <= pages; page++, i++)
                {
                    tasks[i] = FetchAndDeserializeMeasurements(measurements, url, page);
                }

                // wait until all pages are fetched
                await Task.WhenAll(tasks);
            }
            return measurements;
        }

        private static async Task<int> FetchAndDeserializeMeasurements(List<MeasurementsModel> measurements, string url, int page = 1)
        {
            string pagedUrl = $"{url}&page={page}&limit=10000";

            HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(pagedUrl);
            if (response.IsSuccessStatusCode)
            {
                GetResultModel<MeasurementsModel[]> data = await response.Content.ReadAsAsync<GetResultModel<MeasurementsModel[]>>();
                measurements.AddRange(data.Results);
                return data.Meta.Found;
            }
            Console.WriteLine(response.ReasonPhrase);
            return 0;
        }
    }
}
