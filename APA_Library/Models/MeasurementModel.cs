using System;

namespace APA_Library.Models
{
    class MeasurementModel
    {
        // Pollution type of measurement
        public string Parameter { get; set; }

        // Unit of measurement
        public string Unit { get; set; }

        public double Value { get; set; }

        // UTC timestamp of measurement
        public DateTime Date { get; set; }

        public string Location { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public Coordinates Coordinates { get; set; }
    }
}
