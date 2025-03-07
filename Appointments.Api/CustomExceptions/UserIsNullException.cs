namespace Appointments.Api.CustomExceptions
{
    public class UserIsNullException : Exception
    {
        public UserIsNullException() : base("User data cannot be null.") { }
    }
}
