using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ApiExamen.Models;

namespace ApiExamen.Services
{
    public class AsignacionService
    {
        private readonly string _connectionString;

        public AsignacionService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task Asignar(Asignacion a)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spAsignarExamen", new
            {
                a.idExamen,
                a.codigoEmpleado
            }, commandType: CommandType.StoredProcedure);

        }

        public async Task<IEnumerable<Asignacion>> ConsultarPorEmpleado(int codigoEmpleado)
        {
            using var con = new SqlConnection(_connectionString);
            return await con.QueryAsync<Asignacion>("spConsultarAsignacionesEmpleado", new { codigoEmpleado }, commandType: CommandType.StoredProcedure);
        }

        public async Task EliminarAsignacion(int idAsignacion)
        {
            try
            {
                Console.WriteLine($"AsignacionService: Eliminando asignación {idAsignacion}");
                
                using var con = new SqlConnection(_connectionString);
                
                // Primero eliminar las respuestas del empleado asociadas a esta asignación
                await con.ExecuteAsync(
                    "DELETE FROM RespuestasEmpleado WHERE idAsignacion = @idAsignacion",
                    new { idAsignacion }
                );
                
                Console.WriteLine($"AsignacionService: Respuestas del empleado eliminadas para asignación {idAsignacion}");
                
                // Luego eliminar la asignación
                await con.ExecuteAsync(
                    "DELETE FROM Asignaciones WHERE idAsignacion = @idAsignacion",
                    new { idAsignacion }
                );
                
                Console.WriteLine($"AsignacionService: Asignación {idAsignacion} eliminada exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AsignacionService: Error al eliminar asignación {idAsignacion}: {ex.Message}");
                throw;
            }
        }
    }
}
