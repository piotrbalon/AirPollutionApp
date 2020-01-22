using System;
using System.Net.Http;
using System.Net.Http.Headers;
namespace APA_Model
{
    public static class ApiHelper
    {
        public static HttpClient ApiClient { get; set; }
        public static string apiEndpoint = "https://api.openaq.org/v1/";

        public static void InitializeClient()
        {
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
