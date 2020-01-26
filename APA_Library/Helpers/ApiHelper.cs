using APA_Library.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace APA_Library.Helpers
{
    public static class ApiHelper
    {
        public static HttpClient ApiClient { get; private set; }
        public static string AirQualityApiEndpoint { get; } = "https://api.openaq.org/v1/";
        public static string GeocodingApiEndpoint { get; } = "http://open.mapquestapi.com/nominatim/v1/search.php?format=json&key=TNEoAE5NkUZjjGXIePZMUCQGdxTcdGEv";

        public static void InitializeClient()
        {
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Data/Pollutants.json");
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                PollutantModel[] pollutants = JsonConvert.DeserializeObject<PollutantModel[]>(json);

                PollutantModel.AllPollutants = pollutants;
            }
        }
    }
}
