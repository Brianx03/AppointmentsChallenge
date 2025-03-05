using Appointments.Api.Models;

namespace Appointments.Api.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetUserAppointmentsAsync(int userId, string sortBy, bool ascending);
        Task<List<AppointmentDto>> GetAllAppointmentsAsync(string sortBy, bool ascending);
        Task<Appointment?> GetAppointmentByIdAsync(int AppointmentId);
        Task<User?> GetUserByIdAsync(int userId);
        Task AddAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(Appointment appointment);
    }
}
