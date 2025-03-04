using Appointments.Api.Models;
using Appointments.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Api.Repositories.Implementation
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync(string sortBy, bool ascending)
        {
            var query = _context.Appointments.AsQueryable();

            query = sortBy.ToLower() switch
            {
                "date" => ascending ? query.OrderBy(a => a.Date) : query.OrderByDescending(a => a.Date),
                "status" => ascending ? query.OrderBy(a => a.Status) : query.OrderByDescending(a => a.Status),
                "description" => ascending ? query.OrderBy(a => a.Description) : query.OrderByDescending(a => a.Description),
                _ => query.OrderBy(a => a.Date),
            };

            var appointments = await query.ToListAsync();
            return appointments;
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(int userId, string sortBy, bool ascending)
        {
            var query =_context.Appointments
                .Where(appoinment => appoinment.UserId == userId);

            query = sortBy.ToLower() switch
            {
                "date" => ascending ? query.OrderBy(a => a.Date) : query.OrderByDescending(a => a.Date),
                "status" => ascending ? query.OrderBy(a => a.Status) : query.OrderByDescending(a => a.Status),
                "description" => ascending ? query.OrderBy(a => a.Description) : query.OrderByDescending(a => a.Description),
                _ => query.OrderBy(a => a.Date),
            };

            return await query.ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.Appointments.FindAsync(appointmentId);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.Where(a => a.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }
}

