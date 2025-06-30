using ApiExamen.Interfaces;
using ApiExamen.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace ApiExamen.Services
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<Usuario?> ObtenerUsuarioPorCredenciales(string usuario, string contrasena)
        {
            using var con = new SqlConnection(_connectionString);
            var query = "SELECT * FROM Usuarios WHERE usuario = @usuario AND contrasena = @contrasena";
            return await con.QueryFirstOrDefaultAsync<Usuario>(query, new { usuario, contrasena });
        }
    }
} 