using Appointments.Api.Models;
using Appointments.Api.Repositories.Interfaces;
using Appointments.Api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace Appointments.Api.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AddUserAsync(User user)
        {
            if (user.Name.IsNullOrEmpty())
                throw new ValidationException("User Name cannot be empty");

            var existingUsers = await _userRepository.GetAllUsersAsync();
            if (existingUsers.Any(u => u.Name == user.Name))
                throw new ValidationException("A user with the same name already exists.");

            await _userRepository.AddUserAsync(user);
        }


        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }
    }
}
