using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WedSite.Database;

namespace WedSite
{
    public class AutoBackup : IHostedService
    {
        private int executionCount = 0;
        private readonly ILogger<AutoBackup> logger;
        private readonly IDatabase database;
        private Timer timer;

        public AutoBackup(ILogger<AutoBackup> logger, IDatabase database)
        {
            this.logger = logger;
            this.database = database;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("AutoBackup is starting.");

            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(79));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            logger.LogInformation($"AutoBackup is working. Count: {count}");
            if (!Directory.Exists("backups"))
                Directory.CreateDirectory("backups");

            string guestJson = JsonConvert.SerializeObject(database.GetGuests());
            File.WriteAllText($"backups/backup{count % 20}.json", guestJson);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("AutoBackup is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
