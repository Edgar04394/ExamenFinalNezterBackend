using Dapper;
using Microsoft.Data.SqlClient;
using ApiExamen.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiExamen.Interfaces;

namespace ApiExamen.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _config;

        public AuthService(IUsuarioRepository usuarioRepository, IConfiguration config)
        {
            _usuarioRepository = usuarioRepository;
            _config = config;
        }

        public async Task<string?> Login(LoginRequest request)
        {
            var usuario = await _usuarioRepository.ObtenerUsuarioPorCredenciales(request.Usuario, request.Contrasena);

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
