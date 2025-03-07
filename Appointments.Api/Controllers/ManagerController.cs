using Appointments.Api.CustomExceptions;
using Appointments.Api.Models;
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
        private readonly IUserService _userService;

        public ManagerController(IAppointmentService appointmentService, IUserService userService)
        {
            _appointmentService = appointmentService;
            _userService = userService;
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
            catch (Exception)
            {
                return StatusCode(500, "An internal server error occurred.");
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
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AppointmentNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (AppointmentStatusIsCanceled ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An internal server error occurred.");
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
            catch (Exception)
            {
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost("user")]
        public async Task<IActionResult> AddUser([FromBody] UserDto user)
        {
            try
            {
                if (user == null)
                    throw new UserIsNullException();

                var newUser = new User
                {
                    Name = user.Name,
                };

                await _userService.AddUserAsync(newUser);
                return NoContent();
            }
            catch(UserIsNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An internal server error occurred.");
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
            catch (Exception)
            {
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
