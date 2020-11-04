using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WedSite.Tracker
{
    public class IpStack
    {
        private readonly string apiKey = "1b15632d020adf7e13d164626ea760bd";

        public IpStack()
        {
            // TODO how do I save credentials.
        }

        public async Task<string> GetLocationAsync(HttpClient client, string ip)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, FormUrl(ip, apiKey));
                client.Timeout = TimeSpan.FromSeconds(10);
                HttpResponseMessage response = await client.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"For IP: {ip}");
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }

        private string FormUrl(string ip, string accessKey)
        {
            return $"http://api.ipstack.com/{ip}?access_key={accessKey}&fields=city,region_name,country_name,zip,latitude,longitude";
        }

    }
}
