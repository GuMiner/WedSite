using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using WedSite.Data;
using WedSite.Data.Metrics;
using WedSite.Database;

namespace WedSite.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminVisitorsModel : PageModel
    {
        private readonly ILogger<AdminVisitorsModel> _logger;
        private readonly IDatabase database;

        public AdminVisitorsModel(ILogger<AdminVisitorsModel> logger, IDatabase database)
        {
            _logger = logger;
            this.database = database;
        }

        public IEnumerable<LocationTimeVisit> LocationTimeVisits => this.GetLocationTimeVisits();

        public void OnGet()
        {
        }

        private IEnumerable<LocationTimeVisit> GetLocationTimeVisits()
        {
            var visitedIps = database.GetVisitedIps();
            foreach (CachedIp ip in visitedIps)
            {
                var parsedLocation = JObject.Parse(ip.Location);
                var latitudeString = parsedLocation.Value<string>("latitude");
                var longitudeString = parsedLocation.Value<string>("longitude");

                if (float.TryParse(latitudeString, out float latitude) && float.TryParse(longitudeString, out float longitude))
                {
                    // Valid location. See what they did.
                    if (database.IsHacker(ip.IP))
                    {
                        yield return new LocationTimeVisit() { IP = ip.IP, Latitude = latitude, Longitude = longitude, HackAttempt = true, VisitedPages = false, IsRobot = true };
                    }
                    else if (database.IsRobot(ip.IP))
                    {
                        yield return new LocationTimeVisit() { IP = ip.IP, Latitude = latitude, Longitude = longitude, HackAttempt = false, VisitedPages = false, IsRobot = true };
                    }
                    else if (!database.VisitedPages(ip.IP))
                    {
                        yield return new LocationTimeVisit() { IP = ip.IP, Latitude = latitude, Longitude = longitude, HackAttempt = false, VisitedPages = false, IsRobot = false };
                    }
                    else
                    {
                        yield return new LocationTimeVisit() { IP = ip.IP, Latitude = latitude, Longitude = longitude, HackAttempt = false, VisitedPages = true, IsRobot = false };
                    }
                }
            }
        }
    }
}
