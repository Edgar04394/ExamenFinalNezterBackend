using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ApiExamen.Models;

namespace ApiExamen.Services
{
    public class PuestoService
    {
        private readonly string _connectionString;

        public PuestoService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<Puesto>> Consultar()
        {
            using var con = new SqlConnection(_connectionString);
            return await con.QueryAsync<Puesto>("spConsultarPuestos", commandType: CommandType.StoredProcedure);
        }

        public async Task Crear(Puesto p)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spInsertarPuesto", new { p.puesto }, commandType: CommandType.StoredProcedure);
        }

        public async Task Actualizar(Puesto p)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spActualizarPuesto", p, commandType: CommandType.StoredProcedure);
        }

        public async Task Eliminar(int idPuesto)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spEliminarPuesto", new { idPuesto }, commandType: CommandType.StoredProcedure);
        }
    }
}
