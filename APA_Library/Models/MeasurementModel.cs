using System;

namespace APA_Library.Models
{

    public class LastestMeasurementsModel
    {
        // Pollution type of measurement
        public string Parameter { get; set; }

        // Unit of measurement
        public string Unit { get; set; }

        public double Value { get; set; }

        // UTC timestamp of measurement
        public MeasurementDateModel Date { get; set; }

        public string Location { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public Coordinates Coordinates { get; set; }

        public class MeasurementDateModel
        {
            public DateTime Utc { get; set; }
            public DateTime Local { get; set; }
        }
    }
}
