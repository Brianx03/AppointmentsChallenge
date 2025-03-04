namespace Appointments.Api.CustomExceptions
{
    public class AppointmentNotFoundException : Exception
    {
        public AppointmentNotFoundException() : base("Appointment not found.") { }
    }
}
