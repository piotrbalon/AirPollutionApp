using System;
using System.Collections.Generic;
using System.Text;

namespace APA_Model
{
    public class CityModel
    {
        public string name { get; set; }

        public string country { get; set; }

        // Number of measurements for this country
        public int count { get; set; }

        // Number of stations in this country
        public int locations { get; set; }
    }
}
