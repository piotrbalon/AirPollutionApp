using APA_Library.Helpers;
using APA_Library.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace APA_Library
{
    public class StationsProcessing
    {
        private static string apiEndpoint = ApiHelper.AirQualityApiEndpoint + "locations?limit=10000";

        public static async Task<GetResultModel<StationModel[]>> LoadStations(
            CountryModel country = null,
            CityModel city = null,
            PollutantModel pollutant = null
        )
        {
            string url = apiEndpoint;

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

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    GetResultModel<StationModel[]> data = await response.Content.ReadAsAsync<GetResultModel<StationModel[]>>();

                    return data;
                }

                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
