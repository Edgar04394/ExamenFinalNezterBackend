using Microsoft.AspNetCore.Mvc;
using ApiExamen.Models;
using ApiExamen.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ApiExamen.Controllers
{
    [Authorize(Roles = "Empleado, Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionesController : ControllerBase
    {
        private readonly IAsignacionService _asignacionService;

        public AsignacionesController(IAsignacionService asignacionService)
        {
            _asignacionService = asignacionService;
        }

        [HttpPost("AsignarExamenAEmpleado")]
        public async Task<IActionResult> AsignarExamen([FromBody] Asignacion asignacion)
        {
            await _asignacionService.Asignar(asignacion);
            return Ok(new { mensaje = "Examen asignado" });
        }

        [HttpPost("eliminarAsignacion")]
        public async Task<IActionResult> EliminarAsignacion([FromBody] int idAsignacion)
        {
            try
            {
                Console.WriteLine($"Eliminando asignación: {idAsignacion}");
                await _asignacionService.EliminarAsignacion(idAsignacion);
                return Ok(new { mensaje = "Asignación eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar asignación: {ex.Message}");
                return BadRequest(new { mensaje = $"Error al eliminar asignación: {ex.Message}" });
            }
        }

        [HttpPost("VisualizarAsignacionesPorEmpleado")]
        public async Task<IActionResult> ListarAsignacionesPorEmpleado([FromBody] Empleado empleado)
        {
            Console.WriteLine($"codigoEmpleado recibido: {empleado.codigoEmpleado}");
            var asignaciones = await _asignacionService.ConsultarPorEmpleado(empleado.codigoEmpleado);
            return Ok(asignaciones);
        }
    }
}
