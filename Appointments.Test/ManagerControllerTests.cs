using Appointments.Api.Controllers;
using Appointments.Api.CustomExceptions;
using Appointments.Api.Models;
using Appointments.Api.Repositories.Interfaces;
using Appointments.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Appointments.Test
{
    public class ManagerControllerTests
    {
        private readonly Mock<IAppointmentService> _mockAppointmentService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly ManagerController _controller;

        public ManagerControllerTests()
        {
            _mockAppointmentService = new Mock<IAppointmentService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new ManagerController(_mockAppointmentService.Object, _mockUserService.Object);
        }

        [Fact]
        public async Task GetAllAppointments_ReturnsOkResult_WithListOfAppointments()
        {
            // Arrange
            var appointments = new List<AppointmentDto> { new AppointmentDto { AppointmentId = 1, Description = "Test" } };
            _mockAppointmentService.Setup(service => service.GetAllAppointmentsAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(appointments);

            // Act
            var result = await _controller.GetAllAppointments();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<AppointmentDto>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task ApproveAppointment_ReturnsNoContentResult()
        {
            // Arrange
            _mockAppointmentService.Setup(service => service.ApproveAppointmentAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ApproveAppointment(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CancelAppointment_ReturnsNoContentResult()
        {
            // Arrange
            _mockAppointmentService.Setup(service => service.CancelAppointmentAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CancelAppointment(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddUser_ReturnsNoContentResult()
        {
            // Arrange
            var userDto = new UserDto { Name = "Brian" };
            _mockUserService.Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddUser(userDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<User> { new User { UserId = 1, Name = "Test User" } };
            _mockUserService.Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<User>>(okResult.Value);
            Assert.Single(returnValue);
        }
    }
}
