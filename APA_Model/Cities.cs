using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace APA_Model
{
    public class Cities
    {
        private static string apiEndpoint = ApiHelper.apiEndpoint + "cities";

        public static async Task<string[]> LoadCities(string country)
        {
            string countryCode = Regex.Match(country, @"\(([^)]*)\)").Groups[1].Value;
            string url = apiEndpoint + "?country=" + countryCode;

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    GetResultModel<CityModel[]> data = await response.Content.ReadAsAsync<GetResultModel<CityModel[]>>();

                    string[] cities = new string[data.results.GetLength(0)];

                    int i = 0;
                    foreach (CityModel city in data.results)
                    {
                        cities[i] = city.name;
                        i++;
                    }

                    return cities;
                }

                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
