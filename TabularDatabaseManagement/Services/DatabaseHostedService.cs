using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace TabularDatabaseManagement.Services
{
    public class DatabaseHostedService : IHostedService
    {
        private readonly DatabaseService _databaseService;

        public DatabaseHostedService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Нічого не робимо при старті
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Викликаємо Dispose при зупинці
            _databaseService.Dispose();
            return Task.CompletedTask;
        }
    }
}
