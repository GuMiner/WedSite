using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WedSite.Data;
using WedSite.Database;

namespace WedSite.Tracker
{
    public class TrackerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IDatabase database;

        private static readonly HashSet<string> requestPathsToSkip = new HashSet<string>
        {
            "/css",
            "/lib",
            "/js",
            "/img",
            "/favicon",
            "/apple-touch",
        };

        public TrackerMiddleware(RequestDelegate next, IDatabase database)
        {
            this.next = next;
            this.database = database;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string requestPath = context.Request.Path.Value;
            if (requestPathsToSkip.Any(path => requestPath.StartsWith(path)))
            {
                await next(context);
                return;
            }

            var IP = Utilities.GetIp(context.Request);

            database.SaveIpForLookup(IP);
            if (context.User.Identity.IsAuthenticated)
            {
                var loginId = long.Parse(context.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value);
                database.AddGuestVisit(new GuestVisit(loginId, requestPath));
            }
            else
            {
                database.AddAnonymousVisit(new AnonymousVisit(IP, requestPath));
            }

            await next(context);
        }
    }
}
