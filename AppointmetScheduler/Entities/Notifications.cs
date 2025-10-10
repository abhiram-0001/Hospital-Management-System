using AppointmetScheduler.Enums;

namespace AppointmetScheduler.Entities
{
    public class Notifications
    {

        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = null!;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? MetadataJson { get; set; } // store JSON (optional ISJSON check)
        public DateTime CreatedAt { get; set; } // GETUTCDATE()

        //f-key
        public User User { get; set; } = null!;
    }
}
