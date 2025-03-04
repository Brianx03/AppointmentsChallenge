using Appointments.Api.CustomExceptions;
using Appointments.Api.Enums;
using Appointments.Api.Models;
using Appointments.Api.Repositories.Interfaces;
using Appointments.Api.Services.Implementation;
using Appointments.Api.Services.Interfaces;
using Moq;
using System.ComponentModel.DataAnnotations;


namespace Appointments.Test
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
        private readonly IAppointmentService _appointmentService;

        public AppointmentServiceTests()
        {
            _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            _appointmentService = new AppointmentService(_appointmentRepositoryMock.Object);
        }

        [Fact]
        public async Task GetUserAppointmentsAsync_InvalidUserId_ThrowsValidationException()
        {
            await Assert.ThrowsAsync<ValidationException>(() => _appointmentService.GetUserAppointmentsAsync(0));
        }

        [Fact]
        public async Task GetUserAppointmentsAsync_NoAppointments_ThrowsAppointmentNotFoundException()
        {
            _appointmentRepositoryMock.Setup(a => a.GetUserAppointmentsAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((List<Appointment>)null);

            await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _appointmentService.GetUserAppointmentsAsync(1));
        }

        [Fact]
        public async Task GetUserAppointmentsAsync_ReturnsAppointments()
        {
            var appointments = new List<Appointment> { new Appointment() };
            _appointmentRepositoryMock.Setup(a => a.GetUserAppointmentsAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(appointments);

            var result = await _appointmentService.GetUserAppointmentsAsync(1);

            Assert.Equal(appointments, result);
        }

        [Fact]
        public async Task GetAllAppointmentsAsync_NoAppointments_ThrowsAppointmentNotFoundException()
        {
            _appointmentRepositoryMock.Setup(a => a.GetAllAppointmentsAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((List<Appointment>)null);

            await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _appointmentService.GetAllAppointmentsAsync());
        }

        [Fact]
        public async Task GetAllAppointmentsAsync_ReturnsAppointments()
        {
            var appointments = new List<Appointment> { new Appointment() };
            _appointmentRepositoryMock.Setup(a => a.GetAllAppointmentsAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(appointments);

            var result = await _appointmentService.GetAllAppointmentsAsync();

            Assert.Equal(appointments, result);
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_InvalidAppointmentId_ThrowsValidationException()
        {
            await Assert.ThrowsAsync<ValidationException>(() => _appointmentService.GetAppointmentByIdAsync(0));
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_NoAppointment_ThrowsAppointmentNotFoundException()
        {
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Appointment)null);

            await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _appointmentService.GetAppointmentByIdAsync(1));
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_ReturnsAppointment()
        {
            var appointment = new Appointment();
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(appointment);

            var result = await _appointmentService.GetAppointmentByIdAsync(1);

            Assert.Equal(appointment, result);
        }

        [Fact]
        public async Task AddAppointmentAsync_UserNotFound_ThrowsUserNotFoundException()
        {
            _appointmentRepositoryMock.Setup(a => a.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null);

            var appointment = new Appointment { UserId = 1, Date = DateTime.UtcNow.AddDays(1) };

            await Assert.ThrowsAsync<UserNotFoundException>(() => _appointmentService.AddAppointmentAsync(appointment));
        }

        [Fact]
        public async Task AddAppointmentAsync_PastDate_ThrowsValidationException()
        {
            var user = new User();
            _appointmentRepositoryMock.Setup(a => a.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(user);

            var appointment = new Appointment { UserId = 1, Date = DateTime.UtcNow.AddDays(-1) };

            await Assert.ThrowsAsync<ValidationException>(() => _appointmentService.AddAppointmentAsync(appointment));
        }

        [Fact]
        public async Task AddAppointmentAsync_AddsAppointment()
        {
            var user = new User();
            _appointmentRepositoryMock.Setup(a => a.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(user);

            var appointment = new Appointment { UserId = 1, Date = DateTime.UtcNow.AddDays(1) };

            await _appointmentService.AddAppointmentAsync(appointment);

            _appointmentRepositoryMock.Verify(repo => repo.AddAppointmentAsync(appointment), Times.Once);
        }

        [Fact]
        public async Task UpdateAppointmentAsync_AppointmentNotFound_ThrowsAppointmentNotFoundException()
        {
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Appointment)null);

            var appointment = new Appointment { AppointmentId = 1, UserId = 1, Date = DateTime.UtcNow.AddDays(1) };

            await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _appointmentService.UpdateAppointmentAsync(appointment));
        }

        [Fact]
        public async Task UpdateAppointmentAsync_UserMismatch_ThrowsAppointmentForbiddenException()
        {
            var existingAppointment = new Appointment { AppointmentId = 1, UserId = 2, Date = DateTime.UtcNow.AddDays(1) };
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(existingAppointment);

            var appointment = new Appointment { AppointmentId = 1, UserId = 1, Date = DateTime.UtcNow.AddDays(1) };

            await Assert.ThrowsAsync<AppointmentForbiddenException>(() => _appointmentService.UpdateAppointmentAsync(appointment));
        }

        [Fact]
        public async Task UpdateAppointmentAsync_StatusNotPending_ThrowsAppointmentStatusIsNotPending()
        {
            var existingAppointment = new Appointment { AppointmentId = 1, UserId = 1, Date = DateTime.UtcNow.AddDays(1), Status = AppointmentStatus.Approved };
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(existingAppointment);

            var appointment = new Appointment { AppointmentId = 1, UserId = 1, Date = DateTime.UtcNow.AddDays(1) };

            await Assert.ThrowsAsync<AppointmentStatusIsNotPending>(() => _appointmentService.UpdateAppointmentAsync(appointment));
        }

        [Fact]
        public async Task UpdateAppointmentAsync_PastDate_ThrowsValidationException()
        {
            var existingAppointment = new Appointment { AppointmentId = 1, UserId = 1, Date = DateTime.UtcNow.AddDays(1), Status = AppointmentStatus.Pending };
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(existingAppointment);

            var appointment = new Appointment { AppointmentId = 1, UserId = 1, Date = DateTime.UtcNow.AddDays(-1) };

            await Assert.ThrowsAsync<ValidationException>(() => _appointmentService.UpdateAppointmentAsync(appointment));
        }

        [Fact]
        public async Task UpdateAppointmentAsync_UpdatesAppointment()
        {
            var existingAppointment = new Appointment { AppointmentId = 1, UserId = 1, Date = DateTime.UtcNow.AddDays(1), Status = AppointmentStatus.Pending };
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(existingAppointment);

            var appointment = new Appointment { AppointmentId = 1, UserId = 1, Date = DateTime.UtcNow.AddDays(2), Description = "Updated" };

            await _appointmentService.UpdateAppointmentAsync(appointment);

            _appointmentRepositoryMock.Verify(a => a.UpdateAppointmentAsync(It.Is<Appointment>(a => a.Date == appointment.Date && a.Description == appointment.Description)), Times.Once);
        }

        [Fact]
        public async Task DeleteAppointmentAsync_AppointmentNotFound_ThrowsAppointmentNotFoundException()
        {
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Appointment)null);

            await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _appointmentService.DeleteAppointmentAsync(1));
        }

        [Fact]
        public async Task DeleteAppointmentAsync_StatusNotCanceled_ThrowsAppointmentStatusIsNotCanceled()
        {
            var existingAppointment = new Appointment { AppointmentId = 1, Status = AppointmentStatus.Approved };
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(existingAppointment);

            await Assert.ThrowsAsync<AppointmentStatusIsNotCanceled>(() => _appointmentService.DeleteAppointmentAsync(1));
        }

        [Fact]
        public async Task DeleteAppointmentAsync_DeletesAppointment()
        {
            var existingAppointment = new Appointment { AppointmentId = 1, Status = AppointmentStatus.Canceled };
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(existingAppointment);

            await _appointmentService.DeleteAppointmentAsync(1);

            _appointmentRepositoryMock.Verify(repo => repo.DeleteAppointmentAsync(existingAppointment), Times.Once);
        }

        [Fact]
        public async Task ApproveAppointmentAsync_AppointmentNotFound_ThrowsAppointmentNotFoundException()
        {
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Appointment)null);

            await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _appointmentService.ApproveAppointmentAsync(1));
        }

        [Fact]
        public async Task ApproveAppointmentAsync_StatusCanceled_ThrowsAppointmentStatusIsCanceled()
        {
            var appointment = new Appointment { AppointmentId = 1, Status = AppointmentStatus.Canceled };
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(appointment);

            await Assert.ThrowsAsync<AppointmentStatusIsCanceled>(() => _appointmentService.ApproveAppointmentAsync(1));
        }

        [Fact]
        public async Task ApproveAppointmentAsync_ApprovesAppointment()
        {
            var appointment = new Appointment { AppointmentId = 1, Status = AppointmentStatus.Pending };
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(appointment);

            await _appointmentService.ApproveAppointmentAsync(1);

            _appointmentRepositoryMock.Verify(a => a.UpdateAppointmentAsync(It.Is<Appointment>(a => a.Status == AppointmentStatus.Approved)), Times.Once);
        }

        [Fact]
        public async Task CancelAppointmentAsync_AppointmentNotFound_ThrowsAppointmentNotFoundException()
        {
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Appointment)null);

            await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _appointmentService.CancelAppointmentAsync(1));
        }

        [Fact]
        public async Task CancelAppointmentAsync_CancelsAppointment()
        {
            var appointment = new Appointment { AppointmentId = 1, Status = AppointmentStatus.Pending };
            _appointmentRepositoryMock.Setup(a => a.GetAppointmentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(appointment);

            await _appointmentService.CancelAppointmentAsync(1);

            _appointmentRepositoryMock.Verify(a => a.UpdateAppointmentAsync(It.Is<Appointment>(a => a.Status == AppointmentStatus.Canceled)), Times.Once);
        }
    }
}