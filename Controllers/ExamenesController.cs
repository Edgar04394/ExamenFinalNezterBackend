using Microsoft.AspNetCore.Mvc;
using ApiExamen.Models;
using ApiExamen.Services;
using Microsoft.AspNetCore.Authorization;

namespace ApiExamen.Controllers
{
    [Authorize(Roles = "Administrador, Empleado")]
    [ApiController]
    [Route("api/[controller]")]
    public class ExamenesController : ControllerBase
    {
        private readonly ExamenService _examenService;

        public ExamenesController(ExamenService examenService)
        {
            _examenService = examenService;
        }

        [Authorize(Roles = "Administrador, Empleado")]
        [HttpPost("VisualizarExamen")]
        public async Task<IActionResult> VisualizarExamenes()
        {
            var examenes = await _examenService.Consultar();
            return Ok(examenes);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("CrearExamen")]
        public async Task<IActionResult> CrearExamen([FromBody] Examen examen)
        {
            await _examenService.Crear(examen);
            return Ok(new { mensaje = "Examen creado" });
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("ActualizarExamen/{id}")]
        public async Task<IActionResult> ActualizarExamen(int id, [FromBody] Examen examen)
        {
            examen.idExamen = id;
            await _examenService.Actualizar(examen);
            return Ok(new { mensaje = "Examen actualizado" });
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("EliminarExamen/{id}")]
        public async Task<IActionResult> EliminarExamen(int id)
        {
            await _examenService.Eliminar(id);
            return Ok(new { mensaje = "Examen eliminado" });
        }
    }
}
