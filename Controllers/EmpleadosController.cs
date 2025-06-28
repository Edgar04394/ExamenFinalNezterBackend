using Microsoft.AspNetCore.Mvc;
using ApiExamen.Models;
using ApiExamen.Services;
using Microsoft.AspNetCore.Authorization;

namespace ApiExamen.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly EmpleadoService _empleadoService;

        public EmpleadosController(EmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }

        [AllowAnonymous]
        [HttpPost("CrearEmpleadoConUsuario")]
        public async Task<IActionResult> CrearEmpleadoConUsuario([FromBody] EmpleadoUsuarioDTO dto)
        {
            try
            {
                await _empleadoService.CrearEmpleadoConUsuario(dto);
                return Ok(new { mensaje = "Empleado y usuario creados correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al crear empleado y usuario", error = ex.Message });
            }
        }

        [HttpPost("visualizarEmpleado")]
        public async Task<IActionResult> VisualizarEmpleados()
        {
            var empleados = await _empleadoService.Consultar();
            return Ok(empleados);
        }

        [HttpPost("CrearEmpleado")]
        public async Task<IActionResult> CrearEmpleado([FromBody] Empleado empleado)
        {
            await _empleadoService.Crear(empleado); // ← nombre correcto
            return Ok(new { mensaje = "Empleado creado" });
        }

        [HttpPost("ActualizarEmpleado/{id}")]
        public async Task<IActionResult> ActualizarEmpleado(int id, [FromBody] Empleado empleado)
        {
            empleado.codigoEmpleado = id;
            await _empleadoService.Actualizar(empleado);
            return Ok(new { mensaje = "Empleado actualizado correctamente" });
        }

        [HttpPost("EliminarEmpleado/{id}")]
        public async Task<IActionResult> EliminarEmpleado(int id)
        {
            await _empleadoService.Eliminar(id);
            return Ok(new { mensaje = "Empleado eliminado correctamente" });
        }
    }
}
