namespace ExoApi.Models;

/// <summary>Projeto realizado pela empresa ExoApi.</summary>
public class Projeto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public string Tecnologias { get; set; } = string.Empty;
}

/// <summary>Usuario autorizado a acessar recursos protegidos.</summary>
public class Usuario
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
}
