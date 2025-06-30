using Microsoft.AspNetCore.Mvc;
using ApiExamen.Models;
using ApiExamen.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ApiExamen.Controllers
{
    [Authorize(Roles = "Administrador, Empleado")]
    [ApiController]
    [Route("api/[controller]")]
    public class ResultadosController : ControllerBase
    {
        private readonly IResultadoService _resultadoService;
        private readonly string _connectionString;

        public ResultadosController(IResultadoService resultadoService, IConfiguration config)
        {
            _resultadoService = resultadoService;
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        [HttpPost("guardarRespuesta")]
        public async Task<IActionResult> GuardarRespuestaEmpleado([FromBody] RespuestaEmpleado respuestaEmpleado)
        {
            try
            {
                await _resultadoService.GuardarRespuestaEmpleado(respuestaEmpleado);
                
                var response = new { mensaje = "Respuesta guardada", success = true };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new { mensaje = $"Error al guardar respuesta: {ex.Message}", success = false };
                return BadRequest(errorResponse);
            }
        }

        [HttpPost("reportePorCompetencia")]
        public async Task<IActionResult> ReportePromedioPorCompetencia([FromBody] Asignacion asignacion)
        {
            try
            {
                var reporte = await _resultadoService.ReportePromedioPorCompetencia(asignacion.idAsignacion);
                
                // Verificar si el reporte está vacío
                if (!reporte.Any())
                {
                    return Ok(new List<ReportePromedioCompetencia>());
                }
                
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al generar reporte: {ex.Message}");
            }
        }

        [HttpPost("diagnostico")]
        public async Task<IActionResult> Diagnostico([FromBody] int idAsignacion)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                
                // 1. Verificar si la asignación existe
                var asignacion = await con.QueryFirstOrDefaultAsync<Asignacion>(
                    "SELECT * FROM Asignaciones WHERE idAsignacion = @idAsignacion",
                    new { idAsignacion }
                );
                
                if (asignacion == null)
                {
                    return BadRequest($"La asignación {idAsignacion} no existe");
                }
                
                // 2. Verificar respuestas del empleado
                var respuestasEmpleado = await con.QueryAsync<RespuestaEmpleado>(
                    "SELECT * FROM RespuestasEmpleado WHERE idAsignacion = @idAsignacion",
                    new { idAsignacion }
                );
                
                // 3. Verificar datos de respuestas
                var respuestas = await con.QueryAsync<Respuesta>(
                    @"SELECT r.* FROM Respuestas r 
                      INNER JOIN RespuestasEmpleado re ON r.idRespuesta = re.idRespuesta 
                      WHERE re.idAsignacion = @idAsignacion",
                    new { idAsignacion }
                );
                
                // 4. Verificar clasificaciones
                var clasificaciones = await con.QueryAsync<Clasificacion>(
                    @"SELECT DISTINCT c.* FROM Clasificaciones c 
                      INNER JOIN Respuestas r ON c.idClasificacion = r.idClasificacion 
                      INNER JOIN RespuestasEmpleado re ON r.idRespuesta = re.idRespuesta 
                      WHERE re.idAsignacion = @idAsignacion",
                    new { idAsignacion }
                );
                
                // 5. Probar el procedimiento almacenado
                var reporte = await con.QueryAsync<ReportePromedioCompetencia>(
                    "spReportePromedioPorCompetencia",
                    new { idAsignacion },
                    commandType: CommandType.StoredProcedure
                );
                
                var diagnostico = new
                {
                    asignacion = asignacion,
                    totalRespuestasEmpleado = respuestasEmpleado.Count(),
                    respuestasEmpleado = respuestasEmpleado.ToList(),
                    totalRespuestas = respuestas.Count(),
                    respuestas = respuestas.ToList(),
                    totalClasificaciones = clasificaciones.Count(),
                    clasificaciones = clasificaciones.ToList(),
                    totalReporte = reporte.Count(),
                    reporte = reporte.ToList()
                };
                
                return Ok(diagnostico);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en diagnóstico: {ex.Message}");
            }
        }

        [HttpPost("resultadosHistoricos")]
        public async Task<IActionResult> ResultadosHistoricos([FromBody] int codigoEmpleado)
        {
            try
            {
                var resultados = await _resultadoService.ObtenerResultadosHistoricos(codigoEmpleado);
                
                return Ok(new {
                    mensaje = "Resultados históricos obtenidos exitosamente",
                    success = true,
                    codigoEmpleado = codigoEmpleado,
                    totalResultados = resultados.Count,
                    resultados = resultados
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    mensaje = $"Error al obtener resultados históricos: {ex.Message}",
                    success = false,
                    codigoEmpleado = codigoEmpleado,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("resultadosPorExamen")]
        public async Task<IActionResult> ResultadosPorExamen([FromBody] object request)
        {
            try
            {
                var idExamen = Convert.ToInt32(request.GetType().GetProperty("idExamen")?.GetValue(request));
                var codigoEmpleado = Convert.ToInt32(request.GetType().GetProperty("codigoEmpleado")?.GetValue(request));
                
                var resultados = await _resultadoService.ObtenerResultadosPorExamen(idExamen, codigoEmpleado);
                
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener resultados por examen: {ex.Message}");
            }
        }

        [HttpPost("guardarEnHistorial/{idAsignacion}")]
        public async Task<IActionResult> GuardarEnHistorial(int idAsignacion)
        {
            try
            {
                await _resultadoService.GuardarEnHistorial(idAsignacion);
                
                return Ok(new { 
                    mensaje = "Resultados guardados en historial exitosamente", 
                    success = true,
                    idAsignacion = idAsignacion
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    mensaje = $"Error al guardar en historial: {ex.Message}", 
                    success = false,
                    idAsignacion = idAsignacion,
                    error = ex.ToString()
                });
            }
        }
    }
}
