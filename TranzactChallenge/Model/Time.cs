using System;
using System.Collections.Generic;

namespace TranzactChallenge.Model
{
    public class Time
    {
        public string Year
        { get; private set; }

        public string Month
        { get; private set; }

        public string Day
        { get; private set; }

        public string Hour
        { get; private set; }

        public Time (string year, string month, string day, string hour)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.Hour = hour;
        }
    }
}
