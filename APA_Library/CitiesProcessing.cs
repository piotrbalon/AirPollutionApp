using APA_Library.Helpers;
using APA_Library.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace APA_Library
{
    public class CitiesProcessing
    {
        private static string apiEndpoint = ApiHelper.AirQualityApiEndpoint + "cities";

        public static async Task<GetResultModel<CityModel[]>> LoadCities(CountryModel country = null)
        {
            if (country is null)
                throw new ArgumentNullException();

            string url = $"{apiEndpoint}?country={country.Code}&limit=10000";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    GetResultModel<CityModel[]> data = await response.Content.ReadAsAsync<GetResultModel<CityModel[]>>();

                    return data;
                }

                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
