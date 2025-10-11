using AppointmetScheduler.Data;
using AppointmetScheduler.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmetScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalHistoryController : BaseApiController
    {
        private readonly AppDbContext _context;
        public MedicalHistoryController(AppDbContext context) => _context = context;
        [HttpGet("get/{id:int}")]
        public async Task<ActionResult> ListForPatient(int id)
        {
            if (CurrentUserRole == "Patient" && CurrentUserId != id) return Forbid();
            var data = await _context.MedicalHistories.AsNoTracking()
                            .Where(h => h.PatientId == id)
                            .OrderByDescending(h => h.RecordDate)
                            .Select(h => new
                            {
                                h.HistoryId,
                                h.PatientId,
                                h.DoctorId,
                                h.Diagnosis,
                                h.Treatment,
                                h.RecordDate
                            }).ToListAsync();
            return Ok(data);
        }
        public record UpsertHistoryDto(int PatientId, int? DoctorId, 
            string? Diagnosis, string? Treatment);
        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody]UpsertHistoryDto dto)
        {
            if (CurrentUserRole == "Doctor" && CurrentUserId != dto.DoctorId) return Forbid();
            var h = new MedicalHistory
            {
                PatientId=dto.PatientId,
                DoctorId=dto.DoctorId,
                Diagnosis=dto.Diagnosis,
                Treatment=dto.Treatment
            };
            _context.MedicalHistories.Add(h);
            await _context.SaveChangesAsync();
            return Ok(h);
        }
        [HttpGet("getbyid/{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var h=await _context.MedicalHistories.AsNoTracking()
                        .Where(h=>h.HistoryId==id)
                        .Select(x=>new
                        {
                            x.HistoryId,
                            x.PatientId,
                            x.DoctorId,
                            x.Diagnosis,
                            x.Treatment,
                            x.RecordDate
                        }).ToListAsync();
            if (h == null) return NotFound();
            return Ok(h);
        }
        [HttpPut("{id:int}")]

        public async Task<IActionResult> Update(int id, [FromBody] UpsertHistoryDto dto, CancellationToken ct = default)
        {
            var h = await _context.MedicalHistories.FindAsync(new object?[] { id }, ct);
            if (h == null) return NotFound();
            if (CurrentUserRole == "Doctor" && CurrentUserId != h.DoctorId) return Forbid();

            h.Diagnosis = dto.Diagnosis; h.Treatment = dto.Treatment;
            await _context.SaveChangesAsync(ct);
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var h = await _context.MedicalHistories.FindAsync(id);
            if (h == null) return NotFound();
            _context.MedicalHistories.Remove(h);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
