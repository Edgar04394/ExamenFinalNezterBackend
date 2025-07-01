using Microsoft.AspNetCore.Mvc;
using ApiExamen.Models;
using ApiExamen.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiExamen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.Login(request);

                if (token == null)
                    return Unauthorized("Credenciales incorrectas");

                return Ok(new { token }); // Retorna solo el token JWT
            }
            catch (Exception ex)
            {
                // Log del error (en producción usarías un logger)
                Console.WriteLine($"Error en login: {ex.Message}");
                
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("create-test-users")]
        public async Task<IActionResult> CreateTestUsers()
        {
            try
            {
                var connectionString = _config.GetConnectionString("DefaultConnection");
                using var connection = new SqlConnection(connectionString);

                // Crear usuario admin
                var adminQuery = @"
                    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE usuario = 'admin')
                    BEGIN
                        INSERT INTO Usuarios (usuario, contrasena, rol, codigoEmpleado)
                        VALUES ('admin', 'admin123', 'Administrador', NULL);
                    END";

                // Crear usuario empleado
                var empleadoQuery = @"
                    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE usuario = 'empleado1')
                    BEGIN
                        INSERT INTO Usuarios (usuario, contrasena, rol, codigoEmpleado)
                        VALUES ('empleado1', 'empleado123', 'Empleado', 1);
                    END";

                await connection.ExecuteAsync(adminQuery);
                await connection.ExecuteAsync(empleadoQuery);

                // Verificar que se crearon
                var usuarios = await connection.QueryAsync<Usuario>(
                    "SELECT idUsuario, usuario, rol FROM Usuarios WHERE usuario IN ('admin', 'empleado1')"
                );

                return Ok(new { 
                    message = "Usuarios de prueba creados exitosamente",
                    usuarios = usuarios.ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error creando usuarios de prueba", message = ex.Message });
            }
        }
    }
}
