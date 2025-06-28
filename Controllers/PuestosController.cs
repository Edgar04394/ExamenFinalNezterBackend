using Microsoft.AspNetCore.Mvc;
using ApiExamen.Models;
using ApiExamen.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

namespace ApiExamen.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class PuestosController : ControllerBase
    {
        private readonly PuestoService _puestoService;

        public PuestosController(PuestoService puestoService)
        {
            _puestoService = puestoService;
        }

        [HttpPost("visualizarPuesto")]
        public async Task<IActionResult> VisualizarPuestos()
        {
            var puestos = await _puestoService.Consultar();
            return Ok(puestos);
        }

        [HttpPost("CrearPuesto")]
        public async Task<IActionResult> CrearPuesto([FromBody] Puesto puesto)
        {
            await _puestoService.Crear(puesto);
            return Ok(new { mensaje = "Puesto creado exitosamente" });
        }

        [HttpPost("ActualizarPuesto/{id}")]
        public async Task<IActionResult> ActualizarPuesto(int id, [FromBody] Puesto puesto)
        {
            puesto.idPuesto = id;
            await _puestoService.Actualizar(puesto);
            return Ok(new { mensaje = "Puesto actualizado" });
        }

        [HttpPost("EliminarPuesto/{id}")]
        public async Task<IActionResult> EliminarPuesto(int id)
        {
            try
            {
                await _puestoService.Eliminar(id);
                return Ok(new { mensaje = "Puesto eliminado" });
            }
            catch (SqlException)
            {
                return StatusCode(400, new { mensaje = "No se puede eliminar el puesto porque está en uso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno: " + ex.Message });
            }
        }
    }
}
