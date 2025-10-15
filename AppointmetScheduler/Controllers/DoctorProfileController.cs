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
            var doc = await _context.DoctorProfiles
                        .Where(d => d.DoctorId == id)
                        .Select(d => new { d.Specialization, d.Qualifications, d.Experience })
                        .FirstOrDefaultAsync();

            return  doc==null?NotFound(): Ok(doc);
        }
        public record UpdateDtoDoc(string? Specilization,string? Qualifications,int? Expirence);
        [HttpPut("update/{id:int}")]
        public async Task<ActionResult>Update(int id, [FromBody] UpdateDtoDoc dto)
        {
            if (id != CurrentUserId) return Forbid();
            var doc = await _context.DoctorProfiles.FindAsync(id);
            if(doc==null) return NotFound();
            doc.Specialization = dto.Specilization;
            doc.Qualifications = dto.Qualifications;
            doc.Experience=dto.Expirence;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
