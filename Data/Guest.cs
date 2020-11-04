using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WedSite.Data
{
    public class Guest
    {
        public Guest()
        {
        }

        public Guest(string reservationCode, string partyName)
        {
            ReservationCode = reservationCode;
            PartyName = partyName;

            ReservationState = "NoSelection";
            ReservationNotes = "";
        }

        public long Id { get; set; }
        public string ReservationCode { get; set; }
        public string PartyName { get; set; }

        public string ReservationState { get; set; }
        public string ReservationNotes { get; set; }

        public string SongRequests { get; set; }
    }
}
