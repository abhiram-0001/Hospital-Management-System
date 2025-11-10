using AppointmetScheduler.Data;
using AppointmetScheduler.Entities;
using AppointmetScheduler.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmetScheduler.Services
{
    public class SchedulingServices : ISchedulingServices
    {
        private AppDbContext _dbContext;
        public SchedulingServices(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Appointments> BookAsync(BookRequestDto req)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(req.TimeZoneId);
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(req.StartLocal,DateTimeKind.Unspecified),tz);
            var endUtc = startUtc.AddMinutes(req.DurationMinutes);
            if (!await CanBook(req)) return null;
            var ap = new Appointments
            {
                PatientId=req.PatientId,
                DoctorId=req.DoctorId,
                StartUtc=startUtc,
                EndUtc=endUtc,
                Status=Enums.AppointmentStatus.Scheduled,
                Notes=req.Notes
            };
            _dbContext.Appointment.Add(ap);
            await _dbContext.SaveChangesAsync();
            return ap;
            
        }

        public async Task<bool> CanBook(BookRequestDto req)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(req.TimeZoneId);
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(req.StartLocal, DateTimeKind.Unspecified), tz);
            var endUtc = startUtc.AddMinutes(req.DurationMinutes);

            var localstart = TimeZoneInfo.ConvertTimeFromUtc(startUtc, tz);
            var localend = TimeZoneInfo.ConvertTimeFromUtc(endUtc, tz);
            var date = DateOnly.FromDateTime(startUtc);

            var startLocalTime = TimeOnly.FromDateTime(localstart);
            var endLocalTime = TimeOnly.FromDateTime(localend);

            var withinwindow = await _dbContext.DoctorSchedules
                                    .Where(s => s.DoctorId == req.DoctorId && s.IsAvailable && s.AvailableDate == date)
                                    .AnyAsync(s => startLocalTime >= s.StartTime&& endLocalTime <= s.EndTime);

            if (!withinwindow) return false;
            var overlap = await _dbContext.Appointment
                                .AnyAsync(a=>a.DoctorId==req.DoctorId&&a.Status!=Enums.AppointmentStatus.Cancelled&&startUtc>=a.StartUtc&&endUtc<=a.EndUtc);

            return !overlap;
        }
    }
}
