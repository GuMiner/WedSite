using System;

namespace WedSite.Data
{
    public enum GuestStates
    {
        NoLogin,
        LoginNoRsvp,
        RsvpCanCome,
        RsvpCannotCome
    }

    public class Guest
    {
        private static Random random = new Random();
        
        public Guest()
        {
        }

        public Guest(string reservationCode, string partyName)
        {
            ReservationCode = reservationCode;
            PartyName = partyName;

            PartyMinSize = 2;
            PartyMaxSize = 2;
            ReservationState = GuestStates.NoLogin;
            ReservationNotes = "";
            SongRequests = "";
        }

        public static string GenerateCode()
        {
            return random.Next(1000000, 9999999).ToString();
        }

        public long Id { get; set; }
        public string ReservationCode { get; set; }
        public string PartyName { get; set; }
        public int PartyMinSize { get; set; }
        public int PartyMaxSize { get; set; }

        public GuestStates ReservationState { get; set; }
        public string ReservationNotes { get; set; }

        public string SongRequests { get; set; }
    }
}
