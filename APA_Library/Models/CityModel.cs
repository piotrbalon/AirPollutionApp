namespace APA_Library.Models
{
    public class CityModel
    {
        public string Name { get; set; }

        public string Country { get; set; }

        // Number of measurements for this country
        public int Count { get; set; }

        // Number of stations in this country
        public int Locations { get; set; }
    }
}
