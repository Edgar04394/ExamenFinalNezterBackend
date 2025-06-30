using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ApiExamen.Models;
using ApiExamen.Interfaces;

namespace ApiExamen.Services
{
    public class ResultadoService : IResultadoService
    {
        private readonly string _connectionString;

        public ResultadoService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task GuardarRespuestaEmpleado(RespuestaEmpleado r)
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

        public async Task<List<object>> ObtenerResultadosHistoricos(int codigoEmpleado)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                
                // Primero verificar si el empleado existe
                var empleado = await con.QueryFirstOrDefaultAsync<object>(
                    "SELECT * FROM Empleados WHERE codigoEmpleado = @codigoEmpleado",
                    new { codigoEmpleado }
                );
                
                if (empleado == null)
                {
                    throw new Exception($"No se encontró el empleado con código {codigoEmpleado}");
                }
                
                // Verificar si hay datos en el historial
                var historialCount = await con.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(*) FROM HistorialResultados WHERE codigoEmpleado = @codigoEmpleado",
                    new { codigoEmpleado }
                );
                
                Console.WriteLine($"DEBUG: Empleado {codigoEmpleado} tiene {historialCount} registros en historial");
                
                if (historialCount == 0)
                {
                    Console.WriteLine($"DEBUG: No hay registros en historial para empleado {codigoEmpleado}");
                    return new List<object>();
                }
                
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
                
                Console.WriteLine($"DEBUG: Se obtuvieron {resultados.Count()} resultados del historial");
                foreach (var resultado in resultados.Take(3)) // Mostrar los primeros 3 para debug
                {
                    var result = resultado as dynamic;
                    Console.WriteLine($"DEBUG: Resultado - Examen: {result?.NombreExamen}, Competencia: {result?.Competencia}, Promedio: {result?.Promedio}");
                }
                
                return resultados.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en ObtenerResultadosHistoricos: {ex.Message}");
                throw new Exception($"Error al obtener resultados históricos para empleado {codigoEmpleado}: {ex.Message}", ex);
            }
        }

        public async Task GuardarEnHistorial(int idAsignacion)
        {
            try
            {
                Console.WriteLine($"DEBUG: Iniciando GuardarEnHistorial para asignación {idAsignacion}");
                
                using var con = new SqlConnection(_connectionString);
                
                // Primero verificar si la asignación existe y tiene respuestas
                var asignacion = await con.QueryFirstOrDefaultAsync<object>(
                    "SELECT * FROM Asignaciones WHERE idAsignacion = @idAsignacion",
                    new { idAsignacion }
                );
                
                if (asignacion == null)
                {
                    Console.WriteLine($"ERROR: No se encontró la asignación con ID {idAsignacion}");
                    throw new Exception($"No se encontró la asignación con ID {idAsignacion}");
                }
                
                Console.WriteLine($"DEBUG: Asignación {idAsignacion} encontrada");
                
                var respuestasCount = await con.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(*) FROM RespuestasEmpleado WHERE idAsignacion = @idAsignacion",
                    new { idAsignacion }
                );
                
                Console.WriteLine($"DEBUG: Asignación {idAsignacion} tiene {respuestasCount} respuestas");
                
                if (respuestasCount == 0)
                {
                    Console.WriteLine($"ERROR: No se encontraron respuestas para la asignación {idAsignacion}");
                    throw new Exception($"No se encontraron respuestas para la asignación {idAsignacion}");
                }
                
                // Ejecutar el stored procedure
                Console.WriteLine($"DEBUG: Ejecutando spGuardarResultadosEnHistorial para asignación {idAsignacion}");
                await con.ExecuteAsync("spGuardarResultadosEnHistorial", 
                    new { idAsignacion }, 
                    commandType: CommandType.StoredProcedure);
                
                Console.WriteLine($"DEBUG: spGuardarResultadosEnHistorial ejecutado exitosamente para asignación {idAsignacion}");
                
                // El stored procedure debería manejar todo el proceso de guardado
                // No necesitamos verificar manualmente si se guardó correctamente
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en GuardarEnHistorial: {ex.Message}");
                throw new Exception($"Error al guardar en historial para asignación {idAsignacion}: {ex.Message}", ex);
            }
        }

        public async Task<List<ReportePromedioCompetencia>> ReportePromedioPorCompetencia(int idAsignacion)
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
                
                return resultado.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<object>> ObtenerResultadosPorExamen(int idExamen, int codigoEmpleado)
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
                
                return resultados.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
