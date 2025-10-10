namespace AppointmetScheduler.Entities
{
    public class MedicalHistory
    {

        public int HistoryId { get; set; }
        public int PatientId { get; set; }
        public int? DoctorId { get; set; } 
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public DateTime RecordDate { get; set; }

        //f-keys

        public PatientProfile Patient { get; set; } = null!;
        public DoctorProfile? Doctor { get; set; }
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();


    }
}
