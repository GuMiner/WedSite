using LiteDB;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WedSite.Data;

namespace WedSite.Database
{
    public class LiteDbDatabase : IDatabase, IDisposable
    {
        private readonly ILiteDatabase database;
        private readonly ILogger<LiteDbDatabase> logger;
        
        public LiteDbDatabase(ILogger<LiteDbDatabase> logger)
        {
            database = new LiteDatabase("Filename=./WedSite.db");
            this.logger = logger;
        }

        internal static void Initialize()
        {
            using var db = new LiteDatabase("Filename=./WedSite.db");
            var guestCollection = db.GetCollection<Guest>("guests");
            guestCollection.EnsureIndex(g => g.ReservationCode);

            var ipCollection = db.GetCollection<CachedIp>("ipCache");
            ipCollection.EnsureIndex(ip => ip.IP);

            var ipLookupCollection = db.GetCollection<CachedIp>("ipsToLookup");
            ipLookupCollection.EnsureIndex(ip => ip.IP);
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

        public void SaveIpForLookup(string ip)
        {
            ip = ip.ToLowerInvariant();

            var collection = database.GetCollection<CachedIp>("ipCache");
            CachedIp cachedIp = collection.Query().Where(i => i.IP.Equals(ip)).FirstOrDefault();
            if (cachedIp == null)
            {
                var lookupCollection = database.GetCollection<CachedIp>("ipsToLookup");
                cachedIp = lookupCollection.Query().Where(i => i.IP.Equals(ip)).FirstOrDefault();
                if (cachedIp == null)
                {
                    lookupCollection.Insert(new CachedIp(ip, null));
                }
            }
        }
        
        public IEnumerable<string> GetIpsToLookup()
        {
            var collection = database.GetCollection<CachedIp>("ipsToLookup");
            return collection.FindAll().Select(cachedIp => cachedIp.IP).ToList();
        }

        public void CacheIpLocation(string ip, string location)
        {
            ip = ip.ToLowerInvariant();

            var collection = database.GetCollection<CachedIp>("ipCache");
            var lookupCollection = database.GetCollection<CachedIp>("ipsToLookup");
            CachedIp cachedIp = collection.Query().Where(i => i.IP.Equals(ip)).FirstOrDefault();
            if (cachedIp == null)
            {
                collection.Insert(new CachedIp(ip, location));
            }

            cachedIp = lookupCollection.Query().Where(i => i.IP.Equals(ip)).FirstOrDefault();
            if (cachedIp != null)
            {
                lookupCollection.Delete(cachedIp.Id);
            }
        }

        public void Dispose()
        {
            database?.Dispose();
        }
    }
}
