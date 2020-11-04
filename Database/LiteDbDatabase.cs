using LiteDB;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using WedSite.Data;

namespace WedSite.Database
{
    public class LiteDbDatabase : IDatabase, IDisposable
    {
        private ILiteDatabase database;
        private ILogger<LiteDbDatabase> logger;
        
        public LiteDbDatabase(ILogger<LiteDbDatabase> logger)
        {
            database = new LiteDatabase("Filename=./WedSite.db; Connection=shared");
            this.logger = logger;
        }

        internal static void Initialize()
        {
            using var db = new LiteDatabase("Filename=./WedSite.db; Connection=shared");
            var guestCollection = db.GetCollection<Guest>("guests");
            guestCollection.EnsureIndex(g => g.ReservationCode);

            var ipCollection = db.GetCollection<CachedIp>("ipCache");
            ipCollection.EnsureIndex(ip => ip.IP);
        }

        public void AddAnonymousVisit(AnonymousVisit anonymousVisit)
        {
            var collection = database.GetCollection<AnonymousVisit>("anonymousVisits");
            collection.Insert(anonymousVisit);
        }

        public int AddGuest(string reservationCode, string partyName)
        {
            logger.LogInformation($"GuestAdd: {reservationCode}, {partyName}");

            Guest guest = new Guest(reservationCode, partyName);
            var collection = database.GetCollection<Guest>("guests");
            return collection.Insert(guest).AsInt32;
        }

        public Guest GetGuest(string reservationCode)
        {
            var collection = database.GetCollection<Guest>("guests");
            return collection.Query()
                .Where(g => g.ReservationCode.Equals(reservationCode))
                .FirstOrDefault();
        }

        public void UpdateGuest(Guest guest)
        {
            logger.LogInformation($"GuestUpdate: {JsonConvert.SerializeObject(guest)}");

            var collection = database.GetCollection<Guest>("guests");
            collection.Update(guest);
        }

        public long AddGuestLogin(GuestLogin login)
        {
            var collection = database.GetCollection<GuestLogin>("guestLogins");
            return collection.Insert(login).AsInt64;
        }

        public void AddGuestVisit(GuestVisit guestVisit)
        {
            var collection = database.GetCollection<GuestVisit>("guestVisits");
            collection.Insert(guestVisit);
        }

        public void Dispose()
        {
            database?.Dispose();
        }

        public string GetCachedLocation(string ip)
        {
            ip = ip.ToLowerInvariant();

            var collection = database.GetCollection<CachedIp>("ipCache");
            CachedIp cachedIp = collection.Query().Where(i => i.IP.Equals(ip)).FirstOrDefault();
            return cachedIp?.Location;
        }

        public void CacheLocation(string ip, string location)
        {
            ip = ip.ToLowerInvariant();

            var collection = database.GetCollection<CachedIp>("ipCache");
            collection.Insert(new CachedIp(ip, location));
        }
    }
}
