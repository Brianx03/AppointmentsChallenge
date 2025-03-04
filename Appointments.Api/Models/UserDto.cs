using Appointments.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Appointments.Api.Models
{
    public class UserDto
    {
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
