using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ApiExamen.Models;

namespace ApiExamen.Services
{
    public class RespuestaService
    {
        private readonly string _connectionString;

        public RespuestaService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<Respuesta>> ConsultarPorPregunta(int idPregunta)
        {
            using var con = new SqlConnection(_connectionString);
            return await con.QueryAsync<Respuesta>("spConsultarRespuestasPorPregunta", new { idPregunta }, commandType: CommandType.StoredProcedure);
        }

        public async Task Crear(Respuesta r)
        {
            using var con = new SqlConnection(_connectionString);

            var parametros = new
            {
                textoRespuesta = r.textoRespuesta,
                valor = r.valor,
                idPregunta = r.idPregunta,
                idClasificacion = r.idClasificacion
            };

            await con.ExecuteAsync("spInsertarRespuesta", parametros, commandType: CommandType.StoredProcedure);
        }

        public async Task Actualizar(Respuesta respuesta)
        {
            using var con = new SqlConnection(_connectionString);

            var parametros = new
            {
                idRespuesta = respuesta.idRespuesta,
                textoRespuesta = respuesta.textoRespuesta,
                valor = respuesta.valor,
                idClasificacion = respuesta.idClasificacion
            };

            await con.ExecuteAsync("spActualizarRespuesta", parametros, commandType: CommandType.StoredProcedure);
        }

        /*public async Task EliminarTodasLasRespuestasPorPregunta(int idPregunta)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spEliminarRespuestasPorPregunta", new { idPregunta }, commandType: CommandType.StoredProcedure);
        }*/

        public async Task Eliminar(int idRespuesta)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spEliminarRespuesta", new { idRespuesta }, commandType: CommandType.StoredProcedure);
        }
    }
}
