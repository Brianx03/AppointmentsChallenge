namespace Appointments.Api.CustomExceptions
{
    public class AppointmentStatusIsNotPending : Exception
    {
        public AppointmentStatusIsNotPending() : base("Appointment status is not 'Pending' and cannot be updated.") { }
    }
}
