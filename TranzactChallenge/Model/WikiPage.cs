using System;
using System.Collections.Generic;

namespace TranzactChallenge.Model
{
    public class WikiPage
    {
        // Properties
        public int ID 
        { get; set; }

        public int PeriodHour
        { get; set; }

        public string PageLanguage
        { get; private set; }

        public string PageDomain
        { get; private set; }

        public string PageDesign
        { get; private set; }

        public string PageTitle
        { get; private set; }

        public int CountViews
        { get; private set; }

        public int TotalResponseSize
        { get; private set; }

        // Constructor

        public WikiPage(int hour, string language, string domain, string pageDesign, string pageTitle, int countViews, int totalResponseSize)
        {
            this.PeriodHour = hour;
            this.PageLanguage = language;
            this.PageDomain = domain;
            this.PageDesign = pageDesign;
            this.PageTitle = pageTitle;
            this.CountViews = countViews;
            this.TotalResponseSize = totalResponseSize;
        }

    }
}
