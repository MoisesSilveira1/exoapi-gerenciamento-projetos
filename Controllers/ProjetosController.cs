using ExoApi.Models;
using ExoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExoApi.Controllers;

/// <summary>Gerenciamento dos projetos da ExoApi.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjetosController : ControllerBase
{
    private readonly IProjetoService _servico;

    public ProjetosController(IProjetoService servico)
    {
        _servico = servico;
    }

    /// <summary>Lista todos os projetos.</summary>
    /// <response code="200">Lista de projetos.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Projeto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Projeto>>> Listar()
    {
        return Ok(await _servico.ListarAsync());
    }

    /// <summary>Busca um projeto pelo id.</summary>
    /// <response code="200">Projeto encontrado.</response>
    /// <response code="404">Projeto inexistente.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Projeto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Projeto>> Obter(int id)
    {
        return Ok(await _servico.ObterAsync(id));
    }

    /// <summary>Cria um novo projeto.</summary>
    /// <response code="201">Projeto criado.</response>
    /// <response code="400">Dados invalidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Projeto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Projeto>> Criar([FromBody] ProjetoDto dto)
    {
        var criado = await _servico.CriarAsync(dto);
        return CreatedAtAction(nameof(Obter), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza todas as informacoes de um projeto.</summary>
    /// <response code="200">Projeto atualizado.</response>
    /// <response code="400">Dados invalidos.</response>
    /// <response code="404">Projeto inexistente.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Projeto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Projeto>> Substituir(int id, [FromBody] ProjetoDto dto)
    {
        return Ok(await _servico.SubstituirAsync(id, dto));
    }

    /// <summary>Atualiza somente os campos informados de um projeto.</summary>
    /// <response code="200">Projeto atualizado.</response>
    /// <response code="400">Nenhum campo informado ou dados invalidos.</response>
    /// <response code="404">Projeto inexistente.</response>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(Projeto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Projeto>> AtualizarParcial(int id, [FromBody] ProjetoPatchDto dto)
    {
        return Ok(await _servico.AtualizarParcialAsync(id, dto));
    }

    /// <summary>Exclui um projeto (somente usuarios autenticados).</summary>
    /// <response code="204">Projeto excluido.</response>
    /// <response code="401">Token ausente ou invalido.</response>
    /// <response code="404">Projeto inexistente.</response>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id)
    {
        await _servico.ExcluirAsync(id);
        return NoContent();
    }
}
