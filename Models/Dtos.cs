using System.ComponentModel.DataAnnotations;

namespace ExoApi.Models;

/// <summary>Dados para criar ou substituir um projeto (POST e PUT).</summary>
public class ProjetoDto
{
    [Required(ErrorMessage = "O titulo e obrigatorio.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "O titulo deve ter de 3 a 120 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O status e obrigatorio.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "O status deve ter de 3 a 30 caracteres.")]
    public string Status { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de inicio e obrigatoria.")]
    public DateTime? DataInicio { get; set; }

    [Required(ErrorMessage = "As tecnologias sao obrigatorias.")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "As tecnologias devem ter de 2 a 255 caracteres.")]
    public string Tecnologias { get; set; } = string.Empty;
}

/// <summary>Dados para atualizar apenas alguns campos de um projeto (PATCH).</summary>
public class ProjetoPatchDto
{
    [StringLength(120, MinimumLength = 3, ErrorMessage = "O titulo deve ter de 3 a 120 caracteres.")]
    public string? Titulo { get; set; }

    [StringLength(30, MinimumLength = 3, ErrorMessage = "O status deve ter de 3 a 30 caracteres.")]
    public string? Status { get; set; }

    public DateTime? DataInicio { get; set; }

    [StringLength(255, MinimumLength = 2, ErrorMessage = "As tecnologias devem ter de 2 a 255 caracteres.")]
    public string? Tecnologias { get; set; }
}

/// <summary>Credenciais para autenticacao.</summary>
public class LoginDto
{
    [Required(ErrorMessage = "O login e obrigatorio.")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha e obrigatoria.")]
    public string Senha { get; set; } = string.Empty;
}

/// <summary>Resposta do login com o token JWT.</summary>
public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
}
