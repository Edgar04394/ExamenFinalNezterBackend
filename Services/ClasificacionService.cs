using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ApiExamen.Models;

namespace ApiExamen.Services
{
    public class ClasificacionService
    {
        private readonly string _connectionString;

        public ClasificacionService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<Clasificacion>> Consultar()
        {
            using var con = new SqlConnection(_connectionString);
            return await con.QueryAsync<Clasificacion>("spConsultarClasificaciones", commandType: CommandType.StoredProcedure);
        }

        public async Task Crear(Clasificacion c)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spInsertarClasificacion", new { c.nombre, c.color }, commandType: CommandType.StoredProcedure);
        }

        public async Task Actualizar(Clasificacion c)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spActualizarClasificacion", new { c.idClasificacion, c.nombre, c.color }, commandType: CommandType.StoredProcedure);
        }

        public async Task Eliminar(int idClasificacion)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spEliminarClasificacion", new { idClasificacion }, commandType: CommandType.StoredProcedure);
        }
    }
}
