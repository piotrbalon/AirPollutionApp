using System;
using System.Collections.Generic;
using System.Text;

namespace APA_Library.Models
{
    public class LatestMeasurementsModel
    {
        public string Location { get; set; }
        public string Country { get; set; }

        public string City { get; set; }

        public class MeasurementModel
        {
            public string Parameter { get; set; }
            public double Value { get; set; }
            public DateTime LastUpdated { get; set; }
            public string Unit { get; set; }

            public class AveragingPeriodModel
            {
                public string Unit { get; set; }
                public double Value { get; set; }
            }

            public AveragingPeriodModel AveragingPeriod { get; set; }
        }

        public MeasurementModel[] Measurements { get; set; }
    }
}
