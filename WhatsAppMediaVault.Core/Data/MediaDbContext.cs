using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WhatsAppMediaVault.Core.Models;

namespace WhatsAppMediaVault.Core.Data
{
    public class MediaDbContext : DbContext
    {
        public MediaDbContext(DbContextOptions<MediaDbContext> options) : base(options) { }

        public DbSet<MediaItem> MediaItems { get; set; }
    }
}
