using AppointmetScheduler.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmetScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Patient")]
    public class PatientProfileController : BaseApiController
    {
        private readonly AppDbContext _context;
        public PatientProfileController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("get/{id:int}")]
        public async Task<ActionResult>Get(int id)
        {

            if (id != CurrentUserId) return Forbid();

            var p = await _context.PaitentProfiles
                    .Where(x => x.PaitentId == id)
                    .Select(x => new { x.PaitentId, x.Adress, x.BloodGroup, x.EmergencyContact }).FirstOrDefaultAsync();
            return p == null ? NotFound() : Ok(p);
        }
        public record UpdateDto(string? PhoneNumber, string? Gender, DateOnly? DOB, string? Adress,string? BloodGroup,string? EmergencyContact);
        [HttpPut("update/{id:int}")]
        public async Task<ActionResult>Update(int id, [FromBody] UpdateDto dto)
        {
            if (id != CurrentUserId) return Forbid();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            var paitient = await _context.PaitentProfiles.FirstOrDefaultAsync(x=>x.PaitentId==id);

            if (user == null) return NotFound();
            if (paitient == null) return NotFound();

            user.PhoneNumber = dto.PhoneNumber;
            user.Gender = dto.Gender;
            user.DOB = dto.DOB;
            paitient.Adress = dto.Adress;
            paitient.BloodGroup= dto.BloodGroup;
            paitient.EmergencyContact = dto.EmergencyContact;
            await _context.SaveChangesAsync();
            return Ok(paitient);
        }
    }
}
