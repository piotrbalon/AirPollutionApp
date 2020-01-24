namespace APA_Library.Models
{
    public class PollutantModel
    {
        public enum Parameter
        {
            pm25,
            pm10,
            so2,
            no2,
            o3,
            co,
            bc
        }

        public enum Period
        {
            hour,
            day,
            year
        }

        public class Limit
        {
            public string Unit;
            public Period Period;
            public double Value;
            public Limit(string unit, Period period, double value)
            {
                Unit = unit;
                Period = period;
                Value = value;
            }
        }

        public Parameter Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Limit[] Limits { get; set; }

        public static PollutantModel[] AllPollutants { get; set; }
    }
}
