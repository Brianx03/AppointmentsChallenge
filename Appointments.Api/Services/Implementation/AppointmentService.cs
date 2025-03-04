using Appointments.Api.CustomExceptions;
using Appointments.Api.Enums;
using Appointments.Api.Models;
using Appointments.Api.Repositories.Interfaces;
using Appointments.Api.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Appointments.Api.Services.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(int userId, string sortBy = "date", bool ascending = false)
        {
            if (userId <= 0)
                throw new ValidationException("Invalid User ID.");

            var appointments = await _appointmentRepository.GetUserAppointmentsAsync(userId, sortBy, ascending);

            if (appointments == null || !appointments.Any())
                throw new AppointmentNotFoundException();

            return appointments;
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync(string sortBy = "date", bool ascending = false)
        {
            var appointments = await _appointmentRepository.GetAllAppointmentsAsync(sortBy, ascending);

            if (appointments == null || !appointments.Any())
                throw new AppointmentNotFoundException();

            return appointments;
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int appointmentId)
        {
            if (appointmentId <= 0)
                throw new ValidationException("Invalid User ID.");

            if (appointmentId <= 0)
                throw new ValidationException("Invalid Appointment ID.");

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);

            if (appointment == null)
                throw new AppointmentNotFoundException();

            return appointment;
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            var userExists = await _appointmentRepository.GetUserByIdAsync(appointment.UserId);
            if (userExists == null)
                throw new UserNotFoundException(appointment.UserId);

            if (appointment.Date < DateTime.UtcNow)
                throw new ValidationException("Appointment date must be in the future.");

            await _appointmentRepository.AddAppointmentAsync(appointment);
        }
        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            var existingAppointment = await _appointmentRepository.GetAppointmentByIdAsync(appointment.AppointmentId);

            if (existingAppointment == null)
                throw new AppointmentNotFoundException();

            if (existingAppointment.UserId != appointment.UserId)
                throw new AppointmentForbiddenException();

            if (existingAppointment.Status != AppointmentStatus.Pending)
                throw new AppointmentStatusIsNotPending();

            if (appointment.Date < DateTime.UtcNow)
                throw new ValidationException("Appointment date must be in the future.");

            existingAppointment.Date = appointment.Date;
            existingAppointment.Description = appointment.Description;    

            await _appointmentRepository.UpdateAppointmentAsync(existingAppointment);
        }

        public async Task DeleteAppointmentAsync(int appointmentId)
        {
            var existingAppointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);

            if (existingAppointment == null)
                throw new AppointmentNotFoundException();

            if (existingAppointment.Status != AppointmentStatus.Canceled)
                throw new AppointmentStatusIsNotCanceled(); 

            await _appointmentRepository.DeleteAppointmentAsync(existingAppointment);
        }

        public async Task<bool> ApproveAppointmentAsync(int userId, int appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null || appointment.Status == AppointmentStatus.Canceled)
                return false;

            appointment.Status = AppointmentStatus.Approved;
            await _appointmentRepository.UpdateAppointmentAsync(appointment);
            return true;
        }

        public async Task<bool> CancelAppointmentAsync(int userId, int appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return false;

            appointment.Status = AppointmentStatus.Canceled;
            await _appointmentRepository.UpdateAppointmentAsync(appointment);
            return true;
        }

        public async Task ApproveAppointmentAsync(int appointmentId)
        {
            var existingAppointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);

            if (existingAppointment == null)
                throw new AppointmentNotFoundException();

            if (existingAppointment.Status == AppointmentStatus.Canceled)
                throw new AppointmentStatusIsCanceled();

            existingAppointment.Status = AppointmentStatus.Approved;

            await _appointmentRepository.UpdateAppointmentAsync(existingAppointment);
        }

        public async Task CancelAppointmentAsync(int appointmentId)
        {
            var existingAppointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);

            if (existingAppointment == null)
                throw new AppointmentNotFoundException();

            existingAppointment.Status = AppointmentStatus.Canceled;

            await _appointmentRepository.UpdateAppointmentAsync(existingAppointment);
        }
    }
}
