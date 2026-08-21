using Microsoft.EntityFrameworkCore;
using WhatsAppMediaVault.Core.Data;

namespace WhatsAppMediaVault.Worker.Services
{
    public class OldMediaCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OldMediaCleanupService> _logger;
        private readonly int _maxAgeDays = 30;

        public OldMediaCleanupService(IServiceScopeFactory scopeFactory, ILogger<OldMediaCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupOldMediaAsync();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task CleanupOldMediaAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MediaDbContext>();

            var cutoffDate = DateTime.Now.AddDays(-_maxAgeDays);

            var itemsToDelete = await db.MediaItems
                .Where(m => m.DateReceived < cutoffDate && !m.IsStarred)
                .ToListAsync();

            int deletedCount = 0;
            long spaceFreed = 0;

            foreach (var item in itemsToDelete)
            {
                try
                {
                    if (File.Exists(item.FilePath))
                    {
                        spaceFreed += item.FileSizeBytes;
                        File.Delete(item.FilePath);
                    }

                    db.MediaItems.Remove(item);
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Could not delete {item.FilePath}: {ex.Message}");
                }
            }

            await db.SaveChangesAsync();
            _logger.LogInformation($"Cleanup done: {deletedCount} files deleted, {spaceFreed / 1024 / 1024} MB freed.");
        }
    }
}