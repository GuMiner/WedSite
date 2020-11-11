using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WedSite.Database;

namespace WedSite.Tracker
{
    public class IpAddressResolver : IHostedService
    {
        private int executionCount = 0;
        private readonly ILogger<IpAddressResolver> logger;
        private readonly IDatabase database;
        private readonly IHttpClientFactory clientFactory;
        private Timer timer;

        public IpAddressResolver(ILogger<IpAddressResolver> logger, IDatabase database, IHttpClientFactory clientFactory)
        {
            this.logger = logger;
            this.database = database;
            this.clientFactory = clientFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("IP Address Resolver is starting.");

            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            logger.LogInformation($"IP Address Resolver is working. Count: {count}");

            var lookupService = new IpStack();
            var ipsToLookup = database.GetIpsToLookup();
            if (ipsToLookup.Any())
            {
                foreach (string ip in ipsToLookup)
                {
                    using var httpClient = clientFactory.CreateClient("ipStack");
                    string location = lookupService.GetLocationAsync(httpClient, ip).GetAwaiter().GetResult();
                    if (!string.IsNullOrWhiteSpace(location))
                    {
                        logger.LogInformation($"Looked up {ip} -> {location}");
                        database.CacheIpLocation(ip, location);
                    }
                    else
                    {
                        logger.LogWarning($"Couldn't lookup {ip}!");
                    }

                    // Avoid accidentally killing IPStack.
                    Thread.Sleep(500);
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("IP Address Resolver is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
