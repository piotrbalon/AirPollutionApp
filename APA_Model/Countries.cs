using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace APA_Model
{
    public class Countries
    {
        private static string apiEndpoint = ApiHelper.apiEndpoint + "countries";

        public static async Task<string[]> LoadCountries()
        {
            string url = apiEndpoint;

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if(response.IsSuccessStatusCode)
                {
                    GetResultModel<CountryModel[]> data = await response.Content.ReadAsAsync<GetResultModel<CountryModel[]>>();

                    string[] countries = new string[data.results.GetLength(0)];

                    int i = 0;
                    foreach(CountryModel country in data.results)
                    {
                        countries[i] = $"{country.name} ({country.code})";
                        i++;
                    }

                    return countries;
                }

                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
