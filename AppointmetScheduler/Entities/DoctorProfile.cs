namespace AppointmetScheduler.Entities
{
    public class DoctorProfile
    {

        public int DoctorId { get; set; }
        public string ?Description { get; set; }
        public string ?Specialization { get; set; } 
        public int? Experience { get; set; }
        public string? Qualifications { get; set; }
        public int? rating { get; set; }
        //f-keys
        public User Doctor { get; set; }
        public ICollection<DoctorSchedule>DoctorSchedules { get; set; }
        
    }
}
