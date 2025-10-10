namespace AppointmetScheduler.Entities
{
    public class Medication
    {

        public int MedId { get; set; }
        public int HistoryId { get; set; }
        public string DrugName { get; set; } = null!;
        public string? Dosage { get; set; }
        public string? Duration { get; set; }
        //f-key
        public MedicalHistory History { get; set; } = null!;
    }
}
