using Microsoft.AspNetCore.Mvc;
using ApiExamen.Models;
using ApiExamen.Services;
using Microsoft.AspNetCore.Authorization;

namespace ApiExamen.Controllers
{
    [Authorize(Roles = "Administrador, Empleado")]
    [ApiController]
    [Route("api/[controller]")]
    public class ClasificacionesController : ControllerBase
    {
        private readonly ClasificacionService _clasificacionService;

        public ClasificacionesController(ClasificacionService clasificacionService)
        {
            _clasificacionService = clasificacionService;
        }

        [HttpPost("VisualizarClasificacion")]
        public async Task<IActionResult> VisualizarClasificaciones()
        {
            var clasificaciones = await _clasificacionService.Consultar();
            return Ok(clasificaciones);
        }

        [HttpPost("CrearClasificacion")]
        public async Task<IActionResult> CrearClasificacion([FromBody] Clasificacion clasificacion)
        {
            await _clasificacionService.Crear(clasificacion);
            return Ok(new { mensaje = "Clasificación creada" });
        }

        [HttpPost("ActualizarClasificacion/{id}")]
        public async Task<IActionResult> ActualizarClasificacion(int id, [FromBody] Clasificacion clasificacion)
        {
            clasificacion.idClasificacion = id;
            await _clasificacionService.Actualizar(clasificacion);
            return Ok(new { mensaje = "Clasificación actualizada" });
        }

        [HttpPost("EliminarClasificacion/{id}")]
        public async Task<IActionResult> EliminarClasificacion(int id)
        {
            await _clasificacionService.Eliminar(id);
            return Ok(new { mensaje = "Clasificación eliminada" });
        }
    }
}
