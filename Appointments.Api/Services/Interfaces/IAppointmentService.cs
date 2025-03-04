using Appointments.Api.Models;

namespace Appointments.Api.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetUserAppointmentsAsync(int userId, string sortBy = "date", bool ascending = false);
        Task<List<Appointment>> GetAllAppointmentsAsync(string sortBy = "date", bool ascending = false);
        Task<Appointment> GetAppointmentByIdAsync(int appointmentId);
        Task AddAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);       
        Task DeleteAppointmentAsync(int appointmentId);
        Task ApproveAppointmentAsync(int appointmentId);
        Task CancelAppointmentAsync(int appointmentId);
    }
}
