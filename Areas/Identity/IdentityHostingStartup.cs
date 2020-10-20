using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(WedSite.Areas.Identity.IdentityHostingStartup))]
namespace WedSite.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}