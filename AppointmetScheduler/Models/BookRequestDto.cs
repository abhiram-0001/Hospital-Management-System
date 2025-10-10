namespace AppointmetScheduler.Models
{
    public class BookRequestDto
    {
        public int PaitientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime StartLocal { get; set; }
        public int DurationMinutes { get; set; }
        public string TimeZoneId { get; set; }
        
    }
}
