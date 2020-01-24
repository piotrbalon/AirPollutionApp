using System;

namespace APA_Library.Models
{
    public struct Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class StationModel
    {
        private string[] parameters { get; set; }

        // Unique ID
        public string Location { get; set; }

        public string SourceName { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        // Pollutant type measure device available
        public string[] Parameters
        {
            get => parameters;
            set
            {
                parameters = value;
                ParametersString = String.Join(", ", parameters);
            }
        }

        public Coordinates Coordinates { get; set; }

        // UTC timestamp of last measurement
        public DateTime LastUpdated { get; set; }

        // Number of measurements for this station
        public int Count { get; set; }

        public string ParametersString { get; set; }
    }
}
