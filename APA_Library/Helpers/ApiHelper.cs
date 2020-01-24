using APA_Library.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace APA_Library.Helpers
{
    public static class ApiHelper
    {
        public static HttpClient ApiClient { get; private set; }
        public static string ApiEndpoint = "https://api.openaq.org/v1/";

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
