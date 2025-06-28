using Microsoft.AspNetCore.Mvc;
using ApiExamen.Models;
using ApiExamen.Services;
using Microsoft.AspNetCore.Authorization;

namespace ApiExamen.Controllers
{
    [Authorize(Roles = "Administrador, Empleado")]
    [ApiController]
    [Route("api/[controller]")]
    public class RespuestasController : ControllerBase
    {
        private readonly RespuestaService _respuestaService;

        public RespuestasController(RespuestaService respuestaService)
        {
            _respuestaService = respuestaService;
        }

        [HttpPost("VisualizarRespuestaPorPregunta")]
        public async Task<IActionResult> ListarRespuestasPorPregunta([FromBody] Pregunta pregunta)
        {
            var respuestas = await _respuestaService.ConsultarPorPregunta(pregunta.idPregunta);
            return Ok(respuestas);
        }

        [HttpPost("crearRespuesta")]
        public async Task<IActionResult> CrearRespuesta([FromBody] Respuesta respuesta)
        {
            await _respuestaService.Crear(respuesta);
            return Ok(new { mensaje = "Respuesta creada" });
        }

        [HttpPost("ActualizarRespuesta/{id}")]
        public async Task<IActionResult> ActualizarRespuesta(int id, [FromBody] Respuesta respuesta)
        {
            respuesta.idRespuesta = id;
            await _respuestaService.Actualizar(respuesta);
            return Ok(new { mensaje = "Respuesta actualizada" });
        }


        /*[HttpPost("EliminarTodasRespuestasPorPregunta")]
        public async Task<IActionResult> EliminarRespuestasPorPregunta([FromBody] Pregunta pregunta)
        {
            await _respuestaService.EliminarPorPregunta(pregunta.idPregunta);
            return Ok("Respuestas eliminadas");
        }*/

        [HttpPost("EliminarRespuesta/{id}")]
        public async Task<IActionResult> EliminarRespuesta(int id)
        {
            await _respuestaService.Eliminar(id);
            return Ok(new { mensaje = "Respuesta eliminada" });
        }
    }
}
