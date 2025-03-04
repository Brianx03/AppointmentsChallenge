using Appointments.Api.CustomExceptions;
using Appointments.Api.Enums;
using Appointments.Api.Models;
using Appointments.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Appointments.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public UserController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetUserAppointments(
            [FromQuery] int userId,
            [FromQuery] string sortBy = "date",
            [FromQuery] bool ascending = true)
        {
            try
            {
                var appointments = await _appointmentService.GetUserAppointmentsAsync(userId, sortBy, ascending);
                return Ok(appointments);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
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

        [HttpPost("appointments")]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentDto appointmentDto)
        {
            try
            {
                var appointment = new Appointment
                {
                    UserId = appointmentDto.UserId,
                    Description = appointmentDto.Description,
                    Date = appointmentDto.Date,
                    Status = AppointmentStatus.Pending
                };

                await _appointmentService.AddAppointmentAsync(appointment);

                return CreatedAtAction(nameof(GetUserAppointments), new { userId = appointment.UserId }, appointment);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }


        [HttpPut("appointment")]
        public async Task<IActionResult> UpdateAppointment([FromBody] AppointmentDto appointment)
        {
            try
            {
                var updatedAppointment = new Appointment()
                {
                    UserId = appointment.UserId,
                    AppointmentId = appointment.AppointmentId,
                    Description = appointment.Description,
                    Date = appointment.Date
                };

                await _appointmentService.UpdateAppointmentAsync(updatedAppointment);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AppointmentForbiddenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AppointmentStatusIsNotPending ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AppointmentNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("appointment")]
        public async Task<IActionResult> DeleteAppointment([FromQuery] int appointmentId)
        {
            try
            {
                await _appointmentService.DeleteAppointmentAsync(appointmentId);
                return NoContent();
            }
            catch (AppointmentNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (AppointmentStatusIsNotCanceled ex)
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
