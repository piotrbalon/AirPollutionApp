namespace APA_Library.Models
{
    public class CountryModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        // Number of measurements for this country
        public int Count { get; set; }

        // Number of cities in this country
        public int Cities { get; set; }

        // Number of stations in this country
        public int Locations { get; set; }
    }
}
