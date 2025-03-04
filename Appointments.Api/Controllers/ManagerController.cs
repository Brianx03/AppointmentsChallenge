using Appointments.Api.CustomExceptions;
using Appointments.Api.Models;
using Appointments.Api.Repositories.Interfaces;
using Appointments.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

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

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAllAppointments(
            [FromQuery] string sortBy = "date",
            [FromQuery] bool ascending = true)
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsAsync(sortBy, ascending);
                return Ok(appointments);
            }
            catch (AppointmentNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
