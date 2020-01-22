using System;
using System.Collections.Generic;
using System.Text;

namespace APA_Model
{
    public class CountryModel
    {
        public string code { get; set; }

        public string name { get; set; }

        // Number of measurements for this country
        public int count { get; set; }

        // Number of cities in this country
        public int cities { get; set; }

        // Number of stations in this country
        public int locations { get; set; }
    }
}
