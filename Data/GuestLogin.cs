namespace WedSite.Data
{
    public class GuestLogin
    {
        public GuestLogin()
        {
        }

        public GuestLogin(long guestId, string ip)
        {
            GuestId = guestId;
            IP = ip;
        }

        public long Id { get; set; }
        public long GuestId { get; set; }
        public string IP { get; set; }
    }
}
