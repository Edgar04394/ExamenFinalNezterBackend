using Microsoft.AspNetCore.Mvc;
using ApiExamen.Models;
using ApiExamen.Services;
using Microsoft.AspNetCore.Authorization;

namespace ApiExamen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.Login(request);

            if (token == null)
                return Unauthorized("Credenciales incorrectas");

            return Ok(new { token }); // Retorna solo el token JWT
        }
    }
}
