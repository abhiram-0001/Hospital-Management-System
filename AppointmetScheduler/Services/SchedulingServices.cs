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
                PatientId=req.PaitientId,
                DoctorId=req.DoctorId,
                StartUtc=startUtc,
                EndUtc=endUtc,
                Status=Enums.AppointmentStatus.Scheduled
                
            };
            _dbContext.Appointment.Add(ap);
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

            var withinwindow = await _dbContext.DoctorSchedules
                                    .Where(s => s.DoctorId == req.DoctorId && s.IsAvailable && s.AvailableDate == date)
                                    .AnyAsync(s => localstart.TimeOfDay >= s.StartTime.Value.ToTimeSpan()&& localend.TimeOfDay <= s.EndTime.Value.ToTimeSpan());

            if (!withinwindow) return false;
            var overlap = await _dbContext.Appointment.AnyAsync(a=>a.DoctorId==req.DoctorId&&a.Status!=Enums.AppointmentStatus.Cancelled&&startUtc>=a.StartUtc&&endUtc<=a.EndUtc);
            return !overlap;
            
        }
    }
}
