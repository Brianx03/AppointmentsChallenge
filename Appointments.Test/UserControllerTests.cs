using Appointments.Api.Controllers;
using Appointments.Api.CustomExceptions;
using Appointments.Api.Enums;
using Appointments.Api.Models;
using Appointments.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Xunit;

namespace Appointments.Test
{
    public class UserControllerTests
    {
        private readonly Mock<IAppointmentService> _appointmentServiceMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _appointmentServiceMock = new Mock<IAppointmentService>();
            _userController = new UserController(_appointmentServiceMock.Object);
        }

        [Fact]
        public async Task GetUserAppointments_ReturnsOkResult_WithAppointments()
        {
            var appointments = new List<Appointment> { new Appointment() };
            _appointmentServiceMock.Setup(s => s.GetUserAppointmentsAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(appointments);

            var result = await _userController.GetUserAppointments(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(appointments, okResult.Value);
        }

        [Fact]
        public async Task GetUserAppointments_ReturnsBadRequest_WhenValidationExceptionThrown()
        {
            _appointmentServiceMock.Setup(s => s.GetUserAppointmentsAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ThrowsAsync(new ValidationException("Invalid user ID"));

            var result = await _userController.GetUserAppointments(0);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid user ID", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAppointment_ReturnsNoContent_WhenSuccessful()
        {
            var appointmentDto = new AppointmentDto
            {
                UserId = 1,
                Description = "Test appointment",
                Date = DateTime.UtcNow.AddDays(1)
            };

            var result = await _userController.CreateAppointment(appointmentDto);

            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task CreateAppointment_ReturnsBadRequest_WhenValidationExceptionThrown()
        {
            _appointmentServiceMock.Setup(s => s.AddAppointmentAsync(It.IsAny<Appointment>()))
                .ThrowsAsync(new ValidationException("Appointment date must be in the future."));

            var appointmentDto = new AppointmentDto
            {
                UserId = 1,
                Description = "Test appointment",
                Date = DateTime.UtcNow.AddDays(-1)
            };

            var result = await _userController.CreateAppointment(appointmentDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Appointment date must be in the future.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAppointment_ReturnsNoContent()
        {
            var appointmentDto = new AppointmentDto
            {
                UserId = 1,
                AppointmentId = 1,
                Description = "Updated appointment",
                Date = DateTime.UtcNow.AddDays(1)
            };

            var result = await _userController.UpdateAppointment(appointmentDto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAppointment_ReturnsBadRequest_WhenValidationExceptionThrown()
        {
            _appointmentServiceMock.Setup(s => s.UpdateAppointmentAsync(It.IsAny<Appointment>()))
                .ThrowsAsync(new ValidationException("Invalid data"));

            var appointmentDto = new AppointmentDto
            {
                UserId = 1,
                AppointmentId = 1,
                Description = "Updated appointment",
                Date = DateTime.UtcNow.AddDays(1)
            };

            var result = await _userController.UpdateAppointment(appointmentDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid data", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteAppointment_ReturnsNoContent()
        {
            var result = await _userController.DeleteAppointment(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAppointment_ReturnsNotFound_WhenAppointmentNotFoundExceptionThrown()
        {
            _appointmentServiceMock.Setup(s => s.DeleteAppointmentAsync(It.IsAny<int>()))
                .ThrowsAsync(new AppointmentNotFoundException());

            var result = await _userController.DeleteAppointment(1);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Appointment not found.", notFoundResult.Value);
        }
    }
}
