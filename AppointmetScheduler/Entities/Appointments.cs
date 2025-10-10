using AppointmetScheduler.Enums;

namespace AppointmetScheduler.Entities
{
    public class Appointments
    {

        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int? ServiceId { get; set; }

        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }   
        public DateTime UpdatedAt { get; set; }

        //f-keys

        public PatientProfile Paitent { get; set; } = null!;
        public DoctorProfile Doctor { get; set; } = null!;
        public Service? Service { get; set; }
    }
}
