using Appointments.Api.Models;

namespace Appointments.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task AddUserAsync(User user);
    }
}
