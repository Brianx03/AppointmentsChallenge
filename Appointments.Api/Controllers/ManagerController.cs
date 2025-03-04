using Appointments.Api.CustomExceptions;
using Appointments.Api.Models;
using Appointments.Api.Repositories.Interfaces;
using Appointments.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Appointments.Api.Controllers
{
    [Route("api/manager")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserRepository _userService;

        public ManagerController(IAppointmentService appointmentService, IUserRepository userRepository)
        {
            _appointmentService = appointmentService;
            _userService = userRepository;
        }

        [HttpPut("appointment/approve")]
        public async Task<IActionResult> ApproveAppointment([FromQuery] int appointmentId)
        {
            try
            {
                await _appointmentService.ApproveAppointmentAsync(appointmentId);
                return NoContent();
            }
            catch (AppointmentNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (AppointmentStatusIsCanceled ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("appointment/cancel")]
        public async Task<IActionResult> CancelAppointment([FromQuery] int appointmentId)
        {
            try
            {
                await _appointmentService.CancelAppointmentAsync(appointmentId);
                return NoContent();
            }
            catch (AppointmentNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (AppointmentStatusIsCanceled ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("user")]
        public async Task<IActionResult> AddUser([FromBody] UserDto user)
        {
            try
            {
                var newUser = new User
                {
                    Name = user.Name,
                };

                await _userService.AddUserAsync(newUser);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
