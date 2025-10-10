namespace AppointmetScheduler.Entities
{
    public class PatientProfile
    {
        public int PaitentId { get; set; }
        public string? Adress { get; set; }
        public string? BloodGroup { get; set; }
        public string? EmergencyContact { get; set; }

        //f-keys
        public User Paitent { get; set; }
    }
}
