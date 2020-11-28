namespace WedSite.Data.Metrics
{
    public class LocationTimeVisit
    {
        public string IP { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public bool VisitedPages { get; set; }
        public bool HackAttempt { get; set; }
        public bool IsRobot { get; set; }
    }
}
