using Appointments.Api.Models;

namespace Appointments.Api.CustomExceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(int UserId) : base($"User with ID {UserId} not found.") { }
    }
}
