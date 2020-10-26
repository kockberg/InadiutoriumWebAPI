using System;
using System.Collections.Generic;

namespace InadiutoriumWebAPI.Models
{
    public class CalendarDateModel
    {
        public DateTime Date { get; set; }

        public string Season { get; set; }

        public int Season_week { get; set; }

        public List<CelebrationDay> Celebrations { get; set; }

        public string Weekday { get; set; }

        public class CelebrationDay
        {
            public string Title { get; set; }

            public string Colour { get; set; }

            public string Rank { get; set; }

            public double Rank_num { get; set; }
        }
    }
}