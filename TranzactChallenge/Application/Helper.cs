using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using TranzactChallenge.Model;

namespace TranzactChallenge.Application
{
    public static class Helper
    {
        public static string CnnVal(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static string GetDataDumpFolder(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        public static List<string> GetWikimedia()
        {
            List<string> myList = new List<string>
            {
                "commons",
                "meta",
                "incubator",
                "species",
                "strategy",
                "outreach",
                "usability",
                "quality"
            };

            return myList;
        }

        public static Dictionary<string, string> GetDomainDictionary()
        {
            Dictionary<string, string> myDic = new Dictionary<string, string>
            {
                { "b", "WikiBooks" },
                { "d", "Wiktionary" },
                { "f", "WikiMediaFoundation" },
                { "n", "WikiNews" },
                { "q", "WikiQuote" },
                { "s", "WikiSource" },
                { "v", "WikiVersity" },
                { "voy", "WikiVoyage" },
                { "w", "MediaWiki" },
                { "wd", "WikiData" },
                { "m", "WikiMedia" }
            };

            return myDic;
        }

        public static List<Time> GetListTime()
        {
            DateTime dt = DateTime.Now;

            List<Time> time = new List<Time>();

            for (int i = 0; i < 5; i++)
            {
                if (i != 0)
                {
                    dt = dt.AddHours(-1);
                }

                string year = dt.Year.ToString();
                string month = String.Format("{0:00}", dt.Month);
                string day = String.Format("{0:00}", dt.Day);
                string hour = String.Format("{0:00}", dt.Hour); 

                time.Add(new Time(year, month, day, hour));
            }

            return time;
        }
    }
}
