using Appointments.Api.Models;

namespace Appointments.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();

        Task AddUserAsync(User user);
    }
}
