using Microsoft.AspNetCore.Mvc;
using ApiExamen.Models;
using ApiExamen.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ApiExamen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
    }
}
