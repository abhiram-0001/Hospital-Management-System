using AppointmetScheduler.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmetScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : BaseApiController
    {
        private readonly AppDbContext _db;
        public NotificationController(AppDbContext db) => _db = db;
        [HttpGet("mynotifications")]
        public async Task<ActionResult> MyNotifications([FromQuery]bool?unread)
        {
            var q =  _db.Notification.AsNoTracking()
                        .Where(n => n.UserId == CurrentUserId);
            if (unread == true) q = q.Where(n=>!n.IsRead);
            var data=await q.OrderByDescending(n=>n.CreatedAt)
                            .Select(n => new {
                                n.NotificationId,
                                n.Type,
                                n.Message,
                                n.IsRead,
                                n.SentAt,
                                n.ReadAt,
                                n.CreatedAt
                            }).ToListAsync();
            return Ok(data);

        }

        [HttpPost("{id:int}/read")]
        public async Task<IActionResult> MarkRead(int id, CancellationToken ct = default)
        {
            if (CurrentUserId is null) return Unauthorized();
            var n = await _db.Notification.FirstOrDefaultAsync(x => x.NotificationId == id && x.UserId == CurrentUserId, ct);
            if (n is null) return NotFound();
            n.IsRead = true; n.ReadAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

    }
}
