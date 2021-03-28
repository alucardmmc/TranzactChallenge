using System;
using System.Collections.Generic;
using System.Linq;
using TranzactChallenge.Model;

namespace TranzactChallenge.Application
{
    public class WikiDump
    {
        // Methods
        public WikiPage CreateWikiPage(int hour, string[] arrString)
        {
            List<string> listMedia = Helper.GetWikimedia();
            Dictionary<string, string> dicString = Helper.GetDomainDictionary();
            List<string> domainCode = arrString[0].Split(".").ToList();

            // Case 1:
            if (domainCode.ElementAtOrDefault(1) == null)
            {
                domainCode.Add("Wikipedia");
                domainCode.Add("Desktop");
            }
            else if (domainCode.ElementAtOrDefault(1) != null && domainCode.ElementAtOrDefault(2) == null)
            {
                // Case 2.3:
                if (listMedia.Contains(domainCode[0]))
                {
                    domainCode[0] = "en";
                    domainCode[1] = "Wikimedia";
                    domainCode.Add("Desktop");
                }
                // Case 2.2:
                else if (domainCode[1] != "m")
                {
                    domainCode[1] = dicString[domainCode[1]];
                    domainCode.Add("Desktop");
                }
                // Case 2.1:
                else
                {
                    domainCode[1] = "Wikipedia";
                    domainCode.Add("Mobile");
                }
            }
            else if (domainCode.ElementAtOrDefault(2) != null)
            {
                // Case 3.2:
                if (listMedia.Contains(domainCode[0]))
                {
                    domainCode[0] = "en";
                    domainCode[1] = "Wikimedia";
                    domainCode[2] = "Mobile";
                }
                // Case 3.1:
                else
                {
                    domainCode[1] = dicString[domainCode[2]];
                    domainCode[2] = "Desktop";
                }
            }

            WikiPage wiki = new WikiPage(hour, domainCode[0], domainCode[1], domainCode[2],
                                         arrString[1], Int32.Parse(arrString[2]), Int32.Parse(arrString[3]));
            return wiki;
        }
    }
}
