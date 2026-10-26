using ExoApi.Models;
using ExoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExoApi.Controllers;

/// <summary>Autenticacao de usuarios.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    /// <summary>Autentica o usuario e devolve um token JWT.</summary>
    /// <remarks>Usuario de exemplo: admin / Admin@123</remarks>
    /// <response code="200">Token JWT gerado.</response>
    /// <response code="401">Login ou senha invalidos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto dto)
    {
        return Ok(await _auth.AutenticarAsync(dto));
    }
}
