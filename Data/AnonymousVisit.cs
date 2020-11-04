using System;

namespace WedSite.Data
{
    public class AnonymousVisit
    {
        public AnonymousVisit()
        {
        }

        public AnonymousVisit(string ip, string pageName)
        {
            IP = ip;
            PageName = pageName;

            VisitTime = DateTime.UtcNow;
        }

        public long Id { get; set; }
        public DateTime VisitTime { get; set; }
        public string PageName { get; set; }
        public string IP { get; set; }
    }
}
