namespace Appointments.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Appointments.Api;
    using Appointments.Api.Enums;
    using Appointments.Api.Models;
    using Appointments.Api.Repositories.Implementation;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class AppointmentRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly AppointmentRepository _repository;

        public AppointmentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Appointment")
                .Options;

            _context = new AppDbContext(options);
            _repository = new AppointmentRepository(_context);
        }

        [Fact]
        public async Task GetAllAppointmentsAsync_ShouldReturnSortedAppointments()
        {
            //arrange
            var userId = 1;
            _context.Appointments.RemoveRange(_context.Appointments);
            var appointments = new List<Appointment>
            {
                new Appointment { UserId = userId, Date = new DateTime(2025, 1, 1), Status = AppointmentStatus.Pending, Description = "Test 1" },
                new Appointment { UserId = userId, Date = new DateTime(2025, 1, 2), Status = AppointmentStatus.Canceled, Description = "Test 2" }
            };

            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAppointmentsAsync("date", true);

            // Assert
            Assert.Equal(new DateTime(2025, 1, 1), result[0].Date);
        }

        [Fact]
        public async Task GetUserAppointmentsAsync_ShouldReturnUserAppointments()
        {
            // Arrange
            var userId = 1;
            _context.Appointments.RemoveRange(_context.Appointments);
            var appointments = new List<Appointment>
            {
                new Appointment { UserId = userId, Date = new DateTime(2025, 1, 1), Status = AppointmentStatus.Pending, Description = "Test 1" },
                new Appointment { UserId = userId, Date = new DateTime(2025, 1, 2), Status = AppointmentStatus.Canceled, Description = "Test 2" }
            };

            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserAppointmentsAsync(userId, "date", true);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, a => Assert.Equal(userId, a.UserId));
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_ShouldReturnAppointment()
        {
            // Arrange
            _context.Appointments.RemoveRange(_context.Appointments);
            var appointment = new Appointment { AppointmentId = 4, Date = new DateTime(2025, 1, 1), Status = AppointmentStatus.Pending, Description = "Test GetAppointmentByIdAsync" };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAppointmentByIdAsync(appointment.AppointmentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointment.AppointmentId, result.AppointmentId);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser()
        {
            // Arrange
            _context.Appointments.RemoveRange(_context.Appointments);
            var user = new User { UserId = 1, Name = "Brian" };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserByIdAsync(user.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
        }

        [Fact]
        public async Task AddAppointmentAsync_ShouldAddAppointment()
        {
            // Arrange
            _context.Appointments.RemoveRange(_context.Appointments);
            var appointment = new Appointment { Date = new DateTime(2025, 1, 1), Status = AppointmentStatus.Pending, Description = "Test" };

            // Act
            await _repository.AddAppointmentAsync(appointment);
            var savedAppointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Description == "Test");

            // Assert
            Assert.NotNull(savedAppointment);
            Assert.Equal(AppointmentStatus.Pending, savedAppointment.Status);
        }

        [Fact]
        public async Task UpdateAppointmentAsync_ShouldUpdateAppointment()
        {
            // Arrange
            _context.Appointments.RemoveRange(_context.Appointments);
            var appointment = new Appointment { AppointmentId = 1, Date = new DateTime(2025, 1, 1), Status = AppointmentStatus.Pending, Description = "test updateAppointmentAsync" };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            appointment.Status = AppointmentStatus.Approved;
            await _repository.UpdateAppointmentAsync(appointment);
            var updatedAppointment = await _context.Appointments.FindAsync(appointment.AppointmentId);

            // Assert
            Assert.NotNull(updatedAppointment);
            Assert.Equal(AppointmentStatus.Approved, updatedAppointment.Status);
        }

        [Fact]
        public async Task DeleteAppointmentAsync_ShouldDeleteAppointment()
        {
            // Arrange
            _context.Appointments.RemoveRange(_context.Appointments);
            var appointment = new Appointment { AppointmentId = 1, Date = new DateTime(2025, 1, 1), Status = AppointmentStatus.Pending, Description = "Test 1" };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAppointmentAsync(appointment);
            var deletedAppointment = await _context.Appointments.FindAsync(appointment.AppointmentId);

            // Assert
            Assert.Null(deletedAppointment);
        }
    }
}