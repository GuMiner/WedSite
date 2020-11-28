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

        private readonly HashSet<string> hackerPages = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "/.env",
            "//2018/wp-includes/wlwmanifest.xml",
            "//2019/wp-includes/wlwmanifest.xml",
            "//blog/wp-includes/wlwmanifest.xml",
            "//news/wp-includes/wlwmanifest.xml",
            "//site/wp-includes/wlwmanifest.xml",
            "//sito/wp-includes/wlwmanifest.xml",
            "//sito/wp-includes/wlwmanifest.xml",
            "//test/wp-includes/wlwmanifest.xml",
            "//website/wp-includes/wlwmanifest.xml",
            "//wordpress/wp-includes/wlwmanifest.xml",
            "//wp1/wp-includes/wlwmanifest.xml",
            "//wp2/wp-includes/wlwmanifest.xml",
            "//wp-includes/wlwmanifest.xml",
            "//xmlrpc.php",
            "/admin/includes/general.js",
            "/admin/view/javascript/common.js",
            "/administrator/",
            "/administrator/help/en-GB/toc.json",
            "/administrator/language/en-GB/install.xml",
            "/fckeditor/editor/filemanager/connectors/php/upload.php",
            "/images/editor/separator.gif",
            "/misc/ajax.js",
            "/plugins/system/debug/debug.xml",
            "/vendor/phpunit/phpunit/build.xml",
            "/wp-admin/install.php",
            "/wp-admin/setup-config.php",
            "/wp-includes/js/jquery/jquery.js",
        };

        private readonly HashSet<string> robotPages = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "/robots.txt",
            "//robots.txt",
            "/ads.txt",
            "/BingSiteAuth.xml",
            "/google67faf96cdb2f5ffb.html",
            "/google7e6ac75d41af09d5.html",
        };

        private readonly HashSet<string> defaultPages = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "/",
            "//",
        };

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

            var anonymousCollection = db.GetCollection<AnonymousVisit>("anonymousVisits");
            anonymousCollection.EnsureIndex(v => v.IP);
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

        public IEnumerable<Guest> GetGuests()
        {
            var collection = database.GetCollection<Guest>("guests");
            return collection.FindAll().ToList();
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

        public IEnumerable<CachedIp> GetVisitedIps()
        {
            var collection = database.GetCollection<CachedIp>("ipCache");
            return collection.FindAll().ToList();
        }

        public bool IsHacker(string ip)
        {
            ip = ip.ToLowerInvariant();

            var collection = database.GetCollection<AnonymousVisit>("anonymousVisits");
            var result = collection.Query()
                .Where(v => v.IP.Equals(ip))
                .Where(v => hackerPages.Contains(v.PageName))
                .FirstOrDefault();
            return result != null;
        }

        public bool IsRobot(string ip)
        {
            ip = ip.ToLowerInvariant();

            var collection = database.GetCollection<AnonymousVisit>("anonymousVisits");
            var result = collection.Query()
                .Where(v => v.IP.Equals(ip))
                .Where(v => robotPages.Contains(v.PageName))
                .FirstOrDefault();
            return result != null;
        }

        public bool VisitedPages(string ip)
        {
            ip = ip.ToLowerInvariant();

            var collection = database.GetCollection<AnonymousVisit>("anonymousVisits");
            var result = collection.Query()
                .Where(v => v.IP.Equals(ip))
                .Where(v => !defaultPages.Contains(v.PageName))
                .FirstOrDefault();
            return result != null;
        }

        public void Dispose()
        {
            database?.Dispose();
        }
    }
}
