using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
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
        private readonly ITracker tracker;

        public TrackerMiddleware(RequestDelegate next, IDatabase database, ITracker tracker)
        {
            this.next = next;
            this.database = database;
            this.tracker = tracker;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var IP = tracker.GetIp(context.Request);
            string requestPath = context.Request.Path.Value;

            database.SaveIpForLookup(IP);
            if (context.User.Identity.IsAuthenticated)
            {
                var loginId = long.Parse(context.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value);
                database.AddGuestVisit(new GuestVisit(loginId, requestPath));
                //context.User.Claims.ser
                //database.AddGuestVisit(new GuestVisit)
            }
            else
            {
                database.AddAnonymousVisit(new AnonymousVisit(IP, requestPath));
            }

            await next(context);
        }
    }
}
