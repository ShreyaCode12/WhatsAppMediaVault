using Microsoft.EntityFrameworkCore;
using WhatsAppMediaVault.Core.Data;
using WhatsAppMediaVault.Core.Models;

namespace WhatsAppMediaVault.Worker.Services
{
    public class MediaFolderScannerService : BackgroundService
    {
        private readonly string _syncedFolder = @"C:\SyncthingData\WhatsApp Images";
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MediaFolderScannerService> _logger;

        public MediaFolderScannerService(IServiceScopeFactory scopeFactory, ILogger<MediaFolderScannerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ScanFolderAsync();
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }

        private async Task ScanFolderAsync()
        {
            if (!Directory.Exists(_syncedFolder))
            {
                _logger.LogWarning($"Synced folder not found: {_syncedFolder}");
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MediaDbContext>();

            var files = Directory.GetFiles(_syncedFolder, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png"));

            var existingPaths = db.MediaItems.Select(m => m.FilePath).ToHashSet();

            int added = 0;
            foreach (var file in files)
            {
                if (existingPaths.Contains(file)) continue;

                var info = new FileInfo(file);
                db.MediaItems.Add(new MediaItem
                {
                    FilePath = file,
                    FileName = info.Name,
                    DateReceived = info.CreationTime,
                    FileSizeBytes = info.Length,
                    IsStarred = false
                });
                added++;
            }

            if (added > 0)
            {
                await db.SaveChangesAsync();
                _logger.LogInformation($"Scanned folder: {added} new files added to DB.");
            }
        }
    }
}