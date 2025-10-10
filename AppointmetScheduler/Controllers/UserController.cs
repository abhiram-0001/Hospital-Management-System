using AppointmetScheduler.Data;
using AppointmetScheduler.Entities;
using AppointmetScheduler.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AppointmetScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {
        private readonly AppDbContext _db;
        public UserController(AppDbContext db)
        {
            _db = db;
        }
        [HttpGet("getall")]
        public async Task<IActionResult> Getall([FromQuery] string? role, [FromQuery] bool? active)
        {
            var q =  _db.Users.AsQueryable();
            if(!string.IsNullOrWhiteSpace(role)&&Enum.TryParse<UserRole>(role,true,out var r))
            {
                q = q.Where(u => u.Role == r);
            }
            if (active.HasValue)
            {
                q=q.Where(u => u.IsActive==active);
            }
            var allusers = await q.Select(u =>new
            {
                Id=u.UserId,
                Name=u.FirstName+" "+u.LastName,
                u.Email,
                Role=u.Role.ToString(),
                u.IsActive
            }).ToListAsync();
            return Ok(allusers);
        }
        public record UseRoleDto(string Role);
        [HttpPost("{id:int}/role")]
        public async Task<ActionResult> SetRole(int id, [FromBody] UseRoleDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u=>u.UserId==id);
            if (user == null)
            {
                return BadRequest("No User Found");
            }
            if(!Enum.TryParse<UserRole>(dto.Role,true,out var role))
            {
                return BadRequest("Invalid Role");
            }
            user.Role= role;
            if (role == UserRole.Patient)
            {
                var patientprofile=new PatientProfile();
                user.PaitentProfile= patientprofile;
                //_db.PaitentProfiles.Add(patientprofile);
            }
            else if (role==UserRole.Doctor)
            {
                var doctorprofile=new DoctorProfile();
                user.DoctorProfile= doctorprofile;
                //_db.DoctorProfiles.Add(doctorprofile);
            }
                await _db.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("{id:int}/activate")]
        public async Task<ActionResult> Activate(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u=>u.UserId==id);
            if (user == null)
            {
                return BadRequest("No User Found");
            }
            user.IsActive = true;
            await _db.SaveChangesAsync();
            return NoContent(); 
        }
        [HttpPost("{id:int}/deactivate")]
        public async Task<ActionResult> Deactivate(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return BadRequest("No User Found");
            }
            user.IsActive = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
