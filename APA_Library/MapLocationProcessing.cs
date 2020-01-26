using APA_Library.Helpers;
using APA_Library.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace APA_Library
{
    public class MapLocationProcessing
    {
        private static string apiEndpoint = ApiHelper.GeocodingApiEndpoint;

        public static async Task<MapLocationModel> LoadMapLocation(CountryModel country = null, CityModel city = null)
        {
            if (country is null && city is null)
                throw new ArgumentNullException();

            string url = apiEndpoint;
            if (!(country is null))
            {
                url = $"{url}&country={country.Name}";
            }

            if (!(city is null))
            {
                url = $"{url}&city={city.Name}";
            }


            using HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                MapLocationModel[] data = await response.Content.ReadAsAsync<MapLocationModel[]>();

                return data[0];
            }
            throw new Exception(response.ReasonPhrase);
        }
    }
}
