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
    public class MedicationsController : BaseApiController
    {
        private readonly AppDbContext _db;
        public MedicationsController(AppDbContext db) =>_db = db;
        [HttpGet("history/{historyId:int}")]
        public async Task<ActionResult> List(int historyId)
        {
            var meds = await _db.Medications.AsNoTracking().
                            Where(m => m.HistoryId == historyId)
                            .Select(x => new
                            {
                                x.MedId,
                                x.HistoryId,
                                x.DrugName,
                                x.Dosage,
                                x.Duration

                            }).ToListAsync();
            return Ok(meds);
        }
        public record UpsertMedicationDto(string DrugName, 
            string? Dosage, string? Duration);

        [HttpPost("history/{historyId:int}")]
        public async Task<IActionResult> Create(int historyId, [FromBody] UpsertMedicationDto dto, CancellationToken ct = default)
        {
            var h = await _db.MedicalHistories.FindAsync(new object?[] { historyId }, ct);
            if (h is null) return NotFound("History not found.");

            
            if (CurrentUserRole == "Doctor" && CurrentUserId != h.DoctorId) return Forbid();

            var m = new Medication { HistoryId = historyId, DrugName = dto.DrugName, Dosage = dto.Dosage, Duration = dto.Duration };
            _db.Medications.Add(m);
            await _db.SaveChangesAsync(ct);
            return Ok(m);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id, CancellationToken ct = default)
        {
            var m = await _db.Medications.AsNoTracking().FirstOrDefaultAsync(x => x.MedId == id, ct);
            return m is null ? NotFound() : Ok(m);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpsertMedicationDto dto, CancellationToken ct = default)
        {
            var m = await _db.Medications.FindAsync(new object?[] { id }, ct);
            if (m is null) return NotFound();
            var h = await _db.MedicalHistories.FindAsync(new object?[] { m.HistoryId }, ct);
            if (h is null) return NotFound("History not found.");
            if (CurrentUserRole == "Doctor" && CurrentUserId != h.DoctorId) return Forbid();

            m.DrugName = dto.DrugName; m.Dosage = dto.Dosage; m.Duration = dto.Duration;
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var m = await _db.Medications.FindAsync(new object?[] { id }, ct);
            if (m is null) return NotFound();
            var h = await _db.MedicalHistories.FindAsync(new object?[] { m.HistoryId }, ct);
            if (h is null) return NotFound("History not found.");
            if (CurrentUserRole == "Doctor" && CurrentUserId != h.DoctorId) return Forbid();

            _db.Medications.Remove(m);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

    }
}
