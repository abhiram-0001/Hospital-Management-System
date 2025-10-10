namespace AppointmetScheduler.Entities
{
    public class DoctorSchedule
    {
        public int ScheduleId { get; set; }
        public int DoctorId { get; set; }
        public DateOnly? AvailableDate {  get; set; }   
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get;  set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAvailable { get; set; }=true;

        //f-key to doctorprofile
        public DoctorProfile doctor { get; set; }
    }
}
