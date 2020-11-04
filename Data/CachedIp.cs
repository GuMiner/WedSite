namespace WedSite.Data
{
    public class CachedIp
    {
        public CachedIp()
        {

        }

        public CachedIp(string ip, string location)
        {
            IP = ip;
            Location = location;
        }

        public long Id { get; set; }
        public string IP { get; set; }
        public string Location { get; set; }
    }
}
