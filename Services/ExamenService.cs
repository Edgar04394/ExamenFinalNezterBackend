using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ApiExamen.Models;

namespace ApiExamen.Services
{
    public class ExamenService
    {
        private readonly string _connectionString;
        public ExamenService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<Examen>> Consultar()
        {
            using var con = new SqlConnection(_connectionString);
            return await con.QueryAsync<Examen>("spConsultarExamenes", commandType: CommandType.StoredProcedure);
        }

        public async Task Crear(Examen examen)
        {
            using var con = new SqlConnection(_connectionString);
            var parametros = new
            {
                titulo = examen.titulo,
                descripcion = examen.descripcion,
                tiempoLimite = examen.tiempoLimite
            };

            await con.ExecuteAsync("spInsertarExamen", parametros, commandType: CommandType.StoredProcedure);
        }


        public async Task Actualizar(Examen examen)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spActualizarExamen", examen, commandType: CommandType.StoredProcedure);
        }


        public async Task Eliminar(int idExamen)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spEliminarExamen", new { idExamen }, commandType: CommandType.StoredProcedure);
        }
    }
}
