using System.Collections;
using System.Collections.Generic;
using WedSite.Data;

namespace WedSite.Database
{
    public interface IDatabase
    {
        // Setup
        int AddGuest(string reservationCode, string partyName);

        // Login
        Guest GetGuest(string reservationCode); // Returns null if the reservation code isn't found.

        // Visiting
        long AddGuestLogin(GuestLogin login);
        void AddGuestVisit(GuestVisit guestVisit);
        void AddAnonymousVisit(AnonymousVisit anonymousVisit);

        // Updating
        void UpdateGuest(Guest guest);
        IEnumerable<Guest> GetGuests();

        // IP Tracking
        void SaveIpForLookup(string ip);
        IEnumerable<string> GetIpsToLookup();
        void CacheIpLocation(string ip, string location);
    }
}
