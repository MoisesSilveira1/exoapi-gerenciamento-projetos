using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ExoApi.Exceptions;
using ExoApi.Models;
using ExoApi.Repositories;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ExoApi.Services;

/// <summary>Geracao e verificacao de hash de senha (PBKDF2 + salt).</summary>
public static class SenhaHasher
{
    private const int Iteracoes = 100_000;

    public static string Gerar(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, HashAlgorithmName.SHA256, 32);
        return $"{Iteracoes}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verificar(string senha, string hashArmazenado)
    {
        var partes = hashArmazenado.Split('.');
        if (partes.Length != 3 || !int.TryParse(partes[0], out var iteracoes)) return false;

        var salt = Convert.FromBase64String(partes[1]);
        var esperado = Convert.FromBase64String(partes[2]);
        var calculado = Rfc2898DeriveBytes.Pbkdf2(senha, salt, iteracoes, HashAlgorithmName.SHA256, esperado.Length);
        return CryptographicOperations.FixedTimeEquals(calculado, esperado);
    }
}

public interface IAuthService
{
    Task<TokenDto> AutenticarAsync(LoginDto dto);
}

/// <summary>Autentica o usuario e emite o token JWT.</summary>
public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IConfiguration _configuracao;

    public AuthService(IUsuarioRepository usuarios, IConfiguration configuracao)
    {
        _usuarios = usuarios;
        _configuracao = configuracao;
    }

    public async Task<TokenDto> AutenticarAsync(LoginDto dto)
    {
        var usuario = await _usuarios.ObterPorLoginAsync(dto.Login);
        if (usuario is null || !SenhaHasher.Verificar(dto.Senha, usuario.SenhaHash))
            throw new NaoAutorizadoException("Login ou senha invalidos.");

        var expiraEm = DateTime.UtcNow.AddMinutes(_configuracao.GetValue("Jwt:ExpiraMinutos", 60));
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracao["Jwt:Key"]!));

        var descritor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("sub", usuario.Id.ToString()),
                new Claim("login", usuario.Login)
            }),
            Issuer = _configuracao["Jwt:Issuer"],
            Audience = _configuracao["Jwt:Audience"],
            Expires = expiraEm,
            SigningCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descritor);
        return new TokenDto { Token = token, ExpiraEm = expiraEm };
    }
}
