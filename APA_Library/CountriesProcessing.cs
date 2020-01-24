using APA_Library.Helpers;
using APA_Library.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace APA_Library
{
    public class CountriesProcessing
    {
        private static string apiEndpoint = ApiHelper.ApiEndpoint + "countries";

        public static async Task<GetResultModel<CountryModel[]>> LoadCountries()
        {
            string url = $"{apiEndpoint}?limit=10000";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    GetResultModel<CountryModel[]> data = await response.Content.ReadAsAsync<GetResultModel<CountryModel[]>>();

                    return data;
                }

                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
