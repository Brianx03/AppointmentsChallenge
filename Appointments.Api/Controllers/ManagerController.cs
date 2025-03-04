using Appointments.Api.CustomExceptions;
using Appointments.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.Api.Controllers
{
    [Route("api/manager")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public ManagerController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
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
    }
}
