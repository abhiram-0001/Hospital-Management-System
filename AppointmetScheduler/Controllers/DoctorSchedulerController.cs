using AppointmetScheduler.Data;
using AppointmetScheduler.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AppointmetScheduler.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class DoctorSchedulerController : BaseApiController
    {
        private AppDbContext _context;
        public DoctorSchedulerController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("get/{id:int}")]
        public async Task<ActionResult> Get(int id, [FromQuery]DateOnly?fromdate,DateOnly?todate)
        {
            if (id != CurrentUserId&&CurrentUserRole=="Doctor") return Forbid();

            var q = _context.DoctorSchedules.Where(ds=>ds.DoctorId==id);
            if (fromdate.HasValue) q = q.Where(s => s.AvailableDate >= fromdate);
            if(todate.HasValue) q=q.Where(s => s.AvailableDate <=todate);

            var data = await q.OrderBy(s => s.AvailableDate).ThenBy(s => s.StartTime)
                .Select(x => new { x.ScheduleId, x.DoctorId, x.AvailableDate, x.StartTime, x.EndTime, x.IsAvailable }).ToListAsync();

            return Ok(data);
        }
        public record docScheduleDto(DateOnly AvailableDate, TimeOnly StartTime, TimeOnly EndTime, bool IsAvailable);
        [HttpPost("{docId:int}/create")]
        public async Task<ActionResult> Create(int docId, [FromBody] docScheduleDto dto)
        {
            if (CurrentUserId != docId&&CurrentUserRole=="Doctor") return Forbid();
            var schedule = new DoctorSchedule();
            schedule.AvailableDate = dto.AvailableDate;
            schedule.StartTime = dto.StartTime;
            schedule.EndTime = dto.EndTime;
            schedule.IsAvailable = dto.IsAvailable;
            schedule.DoctorId = docId;
            _context.DoctorSchedules.Add(schedule);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), 
                new { docId, fromDate = dto.AvailableDate, toDate = dto.AvailableDate }, 
                new { schedule.ScheduleId });
        }
        [HttpPut("{scheduleId:int}/update")]
        public async Task<ActionResult> Update(int scheduleId, [FromBody] docScheduleDto dto)
        {
            var schedule = await _context.DoctorSchedules.FindAsync(scheduleId);
            if (scheduleId == null) return NotFound();
            if(CurrentUserId!=schedule.DoctorId) return Forbid();

            schedule.AvailableDate = dto.AvailableDate;
            schedule.StartTime = dto.StartTime; 
            schedule.EndTime = dto.EndTime;
            schedule.IsAvailable = dto.IsAvailable;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{scheduleId:int}/delete")]
        public async Task<ActionResult>Delete(int scheduleId)
        {
            var s = await _context.DoctorSchedules.FindAsync(scheduleId);

            if (s == null) return NotFound();
            if(CurrentUserId!=s.DoctorId) return Forbid();
            _context.DoctorSchedules.Remove(s);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
