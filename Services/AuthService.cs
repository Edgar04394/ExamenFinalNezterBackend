using Dapper;
using Microsoft.Data.SqlClient;
using ApiExamen.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiExamen.Services
{
    public class AuthService
    {
        private readonly string _connectionString;
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _config = config;
        }

        public async Task<string?> Login(LoginRequest request)
        {
            using var con = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM Usuarios WHERE usuario = @usuario AND contrasena = @contrasena";
            var usuario = await con.QueryFirstOrDefaultAsync<Usuario>(sql, new
            {
                usuario = request.Usuario,
                contrasena = request.Contrasena
            });

            if (usuario == null) return null;

            // Generar token JWT con el rol
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.usuario!),
                new Claim(ClaimTypes.Role, usuario.rol!) // Aquí va el rol
            };

            // Agregar el claim de codigoEmpleado si existe
            if (usuario.rol == "Empleado" && usuario.GetType().GetProperty("codigoEmpleado") != null)
            {
                var codigoEmpleadoValue = usuario.GetType().GetProperty("codigoEmpleado")?.GetValue(usuario, null);
                if (codigoEmpleadoValue != null)
                    claims.Add(new Claim("codigoEmpleado", codigoEmpleadoValue.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
