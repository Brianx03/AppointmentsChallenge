using Appointments.Api.CustomExceptions;
using Appointments.Api.Enums;
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

            await _userRepository.AddUserAsync(user);
        }
    }
}
