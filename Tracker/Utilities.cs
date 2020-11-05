using Microsoft.AspNetCore.Http;

namespace WedSite.Tracker
{
    public class Utilities
    {
        public static string GetIp(HttpRequest httpRequest)
        {
            var ip = httpRequest.HttpContext.Connection.RemoteIpAddress;
            if (ip.IsIPv4MappedToIPv6)
            {
                return ip.MapToIPv4().ToString();
            }

            return ip.ToString();
        }
    }
}
