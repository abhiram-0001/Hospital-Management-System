using AppointmetScheduler.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmetScheduler.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<DoctorProfile>DoctorProfiles { get; set; }
        public DbSet<PatientProfile>PaitentProfiles { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Appointments> Appointment { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Notifications> Notification { get; set; }
        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
            b.Entity<User>(e =>
            {
                e.HasKey(x => x.UserId);
                e.Property(x=>x.Email).IsRequired();
                e.Property(x => x.Role).HasConversion<int>();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x=>x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(u => u.PaitentProfile)
                    .WithOne(p => p.Paitent)
                    .HasForeignKey<PatientProfile>(p=>p.PaitentId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(u=>u.DoctorProfile)
                    .WithOne(d=>d.Doctor)
                    .HasForeignKey<DoctorProfile>(d=>d.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            b.Entity<PatientProfile>(e =>
            {
                e.HasKey(p=>p.PaitentId);
            });
            b.Entity<DoctorProfile>(e =>
            {
                e.HasKey(d => d.DoctorId);
            });
            b.Entity<Service>(e=>{
                e.HasKey(s=>s.ServiceId);
                e.Property(s => s.Name).IsRequired();
                e.Property(s=>s.IsActive).HasDefaultValue(true);
                e.Property(s => s.DefaultDurationMinutes).HasDefaultValue(15);
            });
            b.Entity<DoctorSchedule>(e =>
            {
                e.HasKey(ds=>ds.ScheduleId);
                e.Property(ds => ds.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(ds => ds.doctor)
                    .WithMany(d => d.DoctorSchedules)
                    .HasForeignKey(d=>d.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            b.Entity<Appointments>(e =>
            {
                e.HasKey(a => a.AppointmentId);
                e.Property(a => a.Status).HasConversion<int>();
                e.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(a => a.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(a => a.Paitent)
                    .WithMany()
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(a=>a.Doctor)
                    .WithMany()
                    .HasForeignKey(a=>a.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(a => a.Service)
                   .WithMany()
                   .HasForeignKey(a => a.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict);
            });
            b.Entity<MedicalHistory>(e =>
            {
                e.HasKey(m=>m.HistoryId);
                e.Property(m => m.RecordDate).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(m=>m.Patient)
                    .WithMany()
                    .HasForeignKey(m=>m.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(m=>m.Doctor)
                    .WithMany()
                    .HasForeignKey(m=>m.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            b.Entity<Medication>(e =>
            {
                e.HasKey(md => md.MedId);
                e.Property(md=>md.DrugName).IsRequired();
                e.HasOne(md=>md.History)
                    .WithMany(m=>m.Medications)
                    .HasForeignKey(md=>md.HistoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            b.Entity<Notifications>(e =>
            {
                
                e.HasKey(x => x.NotificationId);
                e.Property(x => x.Type).HasConversion<int>();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(n => n.User)
                  .WithMany()
                  .HasForeignKey(n => n.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}
