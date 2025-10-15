using AppointmetScheduler.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace AppointmetScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Doctor")]
    public class CalenderController : BaseApiController
    {
        private readonly AppDbContext _context;
        public CalenderController(AppDbContext context)=>_context = context;

        [HttpGet("events")]
        public async Task<ActionResult> Events([FromQuery]int docId, 
            [FromQuery]DateTime startUtc, [FromQuery]DateTime endUtc, [FromQuery]string timezoneId)
        {
            if (docId != CurrentUserId) return Forbid();
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            var events = await _context.Appointment.Where(a => a.DoctorId == docId && a.StartUtc < endUtc &&
                                a.EndUtc > startUtc && a.Status != Enums.AppointmentStatus.Cancelled).Select(x => new
                                {
                                    id=x.AppointmentId,
                                    title="Appointment",
                                    start=TimeZoneInfo.ConvertTimeFromUtc(x.StartUtc,tz).ToString("O"),
                                    end=TimeZoneInfo.ConvertTimeFromUtc(x.EndUtc,tz).ToString("O"),
                                    status=x.Status.ToString()
                                }).ToListAsync();
            return Ok(events);
        }
    }
}
