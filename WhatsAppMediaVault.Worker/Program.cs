using Microsoft.EntityFrameworkCore;
using WhatsAppMediaVault.Core.Data;
using WhatsAppMediaVault.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<MediaDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("WhatsAppMediaVault.Worker")
    ));

builder.Services.AddHostedService<MediaFolderScannerService>();
builder.Services.AddHostedService<OldMediaCleanupService>();

var host = builder.Build();
host.Run();