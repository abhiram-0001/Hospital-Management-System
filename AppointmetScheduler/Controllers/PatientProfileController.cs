using AppointmetScheduler.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

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

            var p = await _context.Users
                    .Where(x => x.UserId == id)
                    .Select(x => new { 
                        x.FirstName,
                        x.LastName,
                        x.Email,
                        x.Gender,
                        x.DOB,
                        x.PhoneNumber,
                        pp=_context.PaitentProfiles.Where(p=>p.PaitentId==id)
                                                    .Select(p=>new
                                                    {
                                                        p.Adress,
                                                        p.BloodGroup,
                                                        p.EmergencyContact
                                                    }).FirstOrDefault()
                    }).FirstOrDefaultAsync();
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
        [HttpGet("{id:int}/getalldoctors")]
        public async Task<ActionResult> GetAlldoc(int id)
        {
            if(id!=CurrentUserId) return Forbid();
            var docs = await _context.Users.Where(u => u.Role == Enums.UserRole.Doctor)
                                .Select( u=>new
                                {
                                    u.UserId,
                                    u.FirstName,
                                    u.Email,
                                    dd=_context.DoctorProfiles.Where(d=>d.DoctorId==u.UserId)
                                                .Select(d=>new
                                                {
                                                    d.Specialization,
                                                    d.rating,
                                                    d.Qualifications
                                                }).FirstOrDefault()
                                }).ToListAsync();
            return Ok(docs);
        }
        [HttpGet("getdoc/{docId:int}")]
        public async Task<ActionResult> GetDocById(int docId)
        {
            var doc= await _context.Users.Where(u => u.UserId==docId)
                                .Select(u => new
                                {
                                    u.UserId,
                                    u.FirstName,
                                    u.Email,
                                    dd = _context.DoctorProfiles.Where(d => d.DoctorId == docId)
                                                .Select(d => new
                                                {
                                                    d.Description,
                                                    d.Specialization,
                                                    d.Experience,
                                                    d.rating,
                                                    d.Qualifications
                                                }).FirstOrDefault()
                                }).FirstOrDefaultAsync();
            return Ok(doc);
        }
    }
}
