using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Appointments.Api.Models;
using Appointments.Api.Repositories.Interfaces;
using Appointments.Api.Services.Implementation;
using Moq;
using Xunit;

namespace Appointments.Api.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task AddUserAsync_ShouldThrowValidationException_WhenUserNameIsEmpty()
        {
            // Arrange
            var user = new User { UserId = 1, Name = "" };

            //Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.AddUserAsync(user));
            Assert.Equal("User Name cannot be empty", exception.Message);
        }

        [Fact]
        public async Task AddUserAsync_ShouldCallRepository_WhenUserIsValid()
        {
            // Arrange
            var user = new User { UserId = 1, Name = "Brian" };

            // Act
            await _userService.AddUserAsync(user);

            // Assert
            _userRepositoryMock.Verify(u => u.AddUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, Name = "User 1" },
                new User { UserId = 2, Name = "User 2" }
            };
            _userRepositoryMock.Setup(u => u.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Equal(users.Count, result.Count);
            Assert.Contains(result, u => u.Name == "User 1");
            Assert.Contains(result, u => u.Name == "User 2");
        }
    }
}
