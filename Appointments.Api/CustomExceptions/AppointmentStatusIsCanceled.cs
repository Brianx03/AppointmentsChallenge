namespace Appointments.Api.CustomExceptions
{
    public class AppointmentStatusIsCanceled : Exception
    {
        public AppointmentStatusIsCanceled() : base("Appointment status is 'Canceled' and cannot be Approved.") { }
    }
}
