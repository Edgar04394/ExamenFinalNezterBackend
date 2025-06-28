using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ApiExamen.Models;

namespace ApiExamen.Services
{
    public class ResultadoService
    {
        private readonly string _connectionString;

        public ResultadoService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task GuardarRespuesta(RespuestaEmpleado r)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                await con.ExecuteAsync("spInsertarRespuestaEmpleado", new
                {
                    r.idAsignacion,
                    r.idPregunta,
                    r.idRespuesta
                }, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ReportePromedioCompetencia>> ObtenerReporte(int idAsignacion)
        {
            try
            {
                // Primero verificamos si existen respuestas para esta asignación
                using var con = new SqlConnection(_connectionString);
                
                // Verificar si hay respuestas para esta asignación
                var respuestasCount = await con.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(*) FROM RespuestasEmpleado WHERE idAsignacion = @idAsignacion",
                    new { idAsignacion }
                );
                
                if (respuestasCount == 0)
                {
                    return new List<ReportePromedioCompetencia>();
                }
                
                // Obtener el reporte
                var resultado = await con.QueryAsync<ReportePromedioCompetencia>(
                    "spReportePromedioPorCompetencia", 
                    new { idAsignacion }, 
                    commandType: CommandType.StoredProcedure
                );
                
                return resultado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<object>> ObtenerResultadosHistoricos(int codigoEmpleado)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                
                // Consulta para obtener todos los resultados históricos del empleado desde la tabla de historial
                var query = @"
                    SELECT 
                        nombreExamen AS NombreExamen,
                        descripcionExamen AS DescripcionExamen,
                        CAST(tiempoLimite AS VARCHAR(8)) AS TiempoLimite,
                        fechaRealizacion AS fechaAsignacion,
                        competencia AS Competencia,
                        promedio AS Promedio
                    FROM HistorialResultados 
                    WHERE codigoEmpleado = @codigoEmpleado
                    ORDER BY fechaRealizacion DESC, nombreExamen, competencia";
                
                var resultados = await con.QueryAsync<object>(query, new { codigoEmpleado });
                
                return resultados;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<object>> ObtenerResultadosPorExamen(int idExamen, int codigoEmpleado)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                
                // Consulta para obtener resultados específicos de un examen
                var query = @"
                    SELECT 
                        e.titulo AS NombreExamen,
                        e.descripcion AS DescripcionExamen,
                        CAST(e.tiempoLimite AS VARCHAR(8)) AS TiempoLimite,
                        a.fechaAsignacion,
                        c.nombre AS Competencia,
                        c.color AS ColorCompetencia,
                        AVG(r.valor) AS Promedio
                    FROM RespuestasEmpleado re
                    JOIN Asignaciones a ON re.idAsignacion = a.idAsignacion
                    JOIN Examenes e ON a.idExamen = e.idExamen
                    JOIN Respuestas r ON re.idRespuesta = r.idRespuesta
                    JOIN Clasificaciones c ON r.idClasificacion = c.idClasificacion
                    WHERE a.codigoEmpleado = @codigoEmpleado AND e.idExamen = @idExamen
                    GROUP BY e.titulo, e.descripcion, e.tiempoLimite, a.fechaAsignacion, c.nombre, c.color
                    ORDER BY c.nombre";
                
                var resultados = await con.QueryAsync<object>(query, new { codigoEmpleado, idExamen });
                
                return resultados;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task GuardarResultadosEnHistorial(int idAsignacion)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                
                await con.ExecuteAsync("spGuardarResultadosEnHistorial", 
                    new { idAsignacion }, 
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
