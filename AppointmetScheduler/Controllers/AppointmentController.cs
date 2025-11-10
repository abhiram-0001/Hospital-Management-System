using AppointmetScheduler.Data;
using AppointmetScheduler.Models;
using AppointmetScheduler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmetScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController : BaseApiController
    {
        private readonly ISchedulingServices _schedulingServices;
        private readonly AppDbContext _dbContext;
        public AppointmentController(ISchedulingServices schedulingServices, AppDbContext dbContext)
        {
            _schedulingServices = schedulingServices;
            _dbContext = dbContext;
        }
        [HttpPost("book")]
        public async Task<ActionResult> Book(BookRequestDto req)
        {
            if (CurrentUserRole == "Patient" && CurrentUserId != req.PatientId) return Forbid();
            var appt = await _schedulingServices.BookAsync(req);
            if (appt == null) return NotFound("NO SLOTS");
            return Ok(appt);
        }
        [HttpGet("getall")]
        public async Task<ActionResult> List([FromQuery] int docId, [FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc)
        {
            if (CurrentUserRole == "Doctor" && CurrentUserId != docId) return Forbid();
            var q = _dbContext.Appointment.AsNoTracking().Where(a => a.DoctorId == docId);
            if (fromUtc.HasValue) q = q.Where(a => a.EndUtc >= fromUtc);
            if (toUtc.HasValue) q = q.Where(a => a.StartUtc <= toUtc);

            var list = await q.OrderBy(a => a.StartUtc).Select(a => new { a.AppointmentId, a.PatientId, a.DoctorId, a.StartUtc, a.EndUtc, Status = a.Status.ToString() }).ToListAsync();
            return Ok(list);
        }
        public record RescheduleDto(DateTime newStartLocal, int DurationMinutes, string TimeZoneId);
        [HttpPost("{id:int}/reschedule")]
        public async Task<ActionResult> Reschedule(int id, RescheduleDto dto)
        {
            var appt = await _dbContext.Appointment.FindAsync(id);
            if (appt == null) return NotFound();
            if (CurrentUserRole == "Paitent" && CurrentUserId != appt.PatientId) return Forbid();
            if (CurrentUserRole == "Doctor" && CurrentUserId != appt.DoctorId) return Forbid();

            var canbook = await _schedulingServices.CanBook(new BookRequestDto { PatientId = appt.PatientId,
                DoctorId = appt.DoctorId, StartLocal = dto.newStartLocal, DurationMinutes = dto.DurationMinutes, TimeZoneId = dto.TimeZoneId });
            if (!canbook) return Conflict("Requested time is not available");
            var tz = TimeZoneInfo.FindSystemTimeZoneById(dto.TimeZoneId);
            var newStart = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dto.newStartLocal, DateTimeKind.Unspecified), tz);
            var newEnd = newStart.AddMinutes(dto.DurationMinutes);
            appt.StartUtc = newStart;
            appt.EndUtc = newEnd;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("{id:int}/cancel")]
        public async Task<ActionResult> Cancel(int id)
        {
            var appt=await _dbContext.Appointment.FindAsync(id);
            if (appt == null) return NotFound();
            if (CurrentUserRole == "Patient" && CurrentUserId != appt.PatientId) return NotFound();
            if (CurrentUserRole == "Doctor" && CurrentUserId != appt.DoctorId) return NotFound();
            appt.Status = Enums.AppointmentStatus.Cancelled;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
   
}
