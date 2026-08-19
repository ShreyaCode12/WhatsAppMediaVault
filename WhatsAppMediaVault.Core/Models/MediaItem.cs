using System;
using System.Collections.Generic;
using System.Text;

namespace WhatsAppMediaVault.Core.Models
{
    
        public class MediaItem
        {
            public int Id { get; set; }
            public string FilePath { get; set; } = string.Empty;
            public string FileName { get; set; } = string.Empty;
            public DateTime DateReceived { get; set; }
            public long FileSizeBytes { get; set; }
            public bool IsStarred { get; set; } = false;
        }
    
}
