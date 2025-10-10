namespace AppointmetScheduler.Entities
{
    public class Service
    {

        public int ServiceId { get; set; }
        public string Name { get; set; } = null!;
        public int DefaultDurationMinutes { get; set; } = 15;
        public bool IsActive { get; set; } = true;

    }
}
