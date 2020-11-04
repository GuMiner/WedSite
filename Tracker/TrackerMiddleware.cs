using Microsoft.AspNetCore.Http;
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

        public TrackerMiddleware(RequestDelegate next, IDatabase database)
        {
            this.next = next;
            this.database = database;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var IP = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
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
