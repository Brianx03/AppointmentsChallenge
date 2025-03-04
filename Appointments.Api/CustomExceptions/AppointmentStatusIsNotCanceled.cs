namespace Appointments.Api.CustomExceptions
{
    public class AppointmentStatusIsNotCanceled : Exception
    {
        public AppointmentStatusIsNotCanceled() : base("Appointment status is not 'Canceled' and cannot be deleted.") { }
    }
}
