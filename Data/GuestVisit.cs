using System;

namespace WedSite.Data
{
    public class GuestVisit
    {
        public GuestVisit()
        {
        }

        public GuestVisit(long guestLoginId, string pageName)
        {
            GuestLoginId = guestLoginId;
            PageName = pageName;
            VisitTime = DateTime.UtcNow;
        }

        public long Id { get; set; }
        public long GuestLoginId { get; set; }
        public DateTime VisitTime { get; set; }
        public string PageName { get; set; }
    }
}
