using AppointmetScheduler.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AppointmetScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Doctor")]
    public class DoctorProfileController : BaseApiController
    {
        private AppDbContext _context;
        public DoctorProfileController(AppDbContext context) =>_context = context;

        [HttpGet("get/{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            if (id != CurrentUserId) return Forbid();
            var doc = await _context.Users
                    .Where(x => x.UserId == id)
                    .Select(x => new {
                        x.FirstName,
                        x.LastName,
                        x.Email,
                        x.Gender,
                        x.DOB,
                        x.PhoneNumber,
                        pp = _context.DoctorProfiles.Where(d=>d.DoctorId== id)
                                                    .Select(d => new
                                                    {
                                                       d.Qualifications,
                                                       d.Description,
                                                       d.rating,
                                                       d.Experience,
                                                       d.Specialization
                                                    }).FirstOrDefault()
                    }).FirstOrDefaultAsync();

            return  doc==null?NotFound(): Ok(doc);
        }
        public record UpdateDtoDoc(string? PhoneNumber, string? Gender, DateOnly? DOB,
            string? Specilization,string? Qualifications,int? Expirence);
        [HttpPut("update/{id:int}")]
        public async Task<ActionResult>Update(int id, [FromBody] UpdateDtoDoc dto)
        {
            if (id != CurrentUserId) return Forbid();
            var user=await _context.Users.FirstOrDefaultAsync(u=>u.UserId==id);
            var doc = await _context.DoctorProfiles.FindAsync(id);

            if(user==null) return NotFound();
            if(doc==null) return NotFound();

            if(dto.PhoneNumber!=null) user.PhoneNumber=dto.PhoneNumber;
            if(dto.Gender!=null) user.Gender=dto.Gender;
            if(dto.DOB!=null)user.DOB=dto.DOB;

            if(dto.Specilization!=null) doc.Specialization = dto.Specilization;
            if(dto.Qualifications!=null) doc.Qualifications = dto.Qualifications;
            if(dto.Expirence!=null) doc.Experience=dto.Expirence;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
