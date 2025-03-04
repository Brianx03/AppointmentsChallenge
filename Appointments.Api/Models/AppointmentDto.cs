using Appointments.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Appointments.Api.Models
{
    public class AppointmentDto
    {
        [Required]
        public int UserId { get; set; }
        public int AppointmentId { get; set; }

        [Required]
        [MaxLength(250)]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
