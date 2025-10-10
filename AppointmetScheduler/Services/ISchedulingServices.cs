using AppointmetScheduler.Entities;
using AppointmetScheduler.Models;

namespace AppointmetScheduler.Services
{
    public interface ISchedulingServices
    {
        Task<bool> CanBook(BookRequestDto request);
        Task<Appointments>BookAsync(BookRequestDto request);
    }
}
