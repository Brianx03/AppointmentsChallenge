using Appointments.Api.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appointments.Api.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
        
        [Required]
        public DateTime Date { get; set; }

        public AppointmentStatus Status { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
