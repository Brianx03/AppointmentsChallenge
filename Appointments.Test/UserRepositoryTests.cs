using System.Collections.Generic;
using System.Threading.Tasks;
using Appointments.Api;
using Appointments.Api.Models;
using Appointments.Api.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;
namespace Appointments.Api.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly AppDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Users")
                .Options;

            _context = new AppDbContext(options);
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            _context.RemoveRange(_context.Users);
            var user = new User { UserId = 1, Name = "Brian" };

            // Act
            await _repository.AddUserAsync(user);
            var usersInDb = await _context.Users.ToListAsync();

            // Assert
            Assert.Single(usersInDb);
            Assert.Equal("Brian", usersInDb[0].Name);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {

            _context.Users.AddRange(
                new User { UserId = 2, Name = "User 1" },
                new User { UserId = 3, Name = "User 2" }
            );
            await _context.SaveChangesAsync();
           
            // Act
            var users = await _repository.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, users.Count);
            Assert.Contains(users, u => u.Name == "User 1");
            Assert.Contains(users, u => u.Name == "User 2");
        }
    }
}
