using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatsAppMediaVault.Core.Data;

namespace WhatsAppMediaVault.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly MediaDbContext _db;

        public MediaController(MediaDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _db.MediaItems
                .OrderByDescending(m => m.DateReceived)
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost("{id}/star")]
        public async Task<IActionResult> Star(int id)
        {
            var item = await _db.MediaItems.FindAsync(id);
            if (item == null) return NotFound();

            item.IsStarred = true;
            await _db.SaveChangesAsync();

            return Ok(item);
        }

        [HttpPost("{id}/unstar")]
        public async Task<IActionResult> Unstar(int id)
        {
            var item = await _db.MediaItems.FindAsync(id);
            if (item == null) return NotFound();

            item.IsStarred = false;
            await _db.SaveChangesAsync();

            return Ok(item);
        }
    }
}