namespace Appointments.Api.CustomExceptions
{
    public class AppointmentForbiddenException : Exception
    {
        public AppointmentForbiddenException() : base("You are not authorized to update this appointment.") { }
    }
}
