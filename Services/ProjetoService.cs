using ExoApi.Exceptions;
using ExoApi.Models;
using ExoApi.Repositories;

namespace ExoApi.Services;

public interface IProjetoService
{
    Task<IEnumerable<Projeto>> ListarAsync();
    Task<Projeto> ObterAsync(int id);
    Task<Projeto> CriarAsync(ProjetoDto dto);
    Task<Projeto> SubstituirAsync(int id, ProjetoDto dto);
    Task<Projeto> AtualizarParcialAsync(int id, ProjetoPatchDto dto);
    Task ExcluirAsync(int id);
}

/// <summary>Regras de negocio relacionadas aos projetos (camada de servico).</summary>
public class ProjetoService : IProjetoService
{
    private readonly IProjetoRepository _repositorio;

    public ProjetoService(IProjetoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<Projeto>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<Projeto> ObterAsync(int id)
    {
        return await _repositorio.ObterPorIdAsync(id)
            ?? throw new NaoEncontradoException($"Projeto com id {id} nao encontrado.");
    }

    public async Task<Projeto> CriarAsync(ProjetoDto dto)
    {
        var projeto = new Projeto
        {
            Titulo = dto.Titulo.Trim(),
            Status = dto.Status.Trim(),
            DataInicio = dto.DataInicio!.Value.Date,
            Tecnologias = dto.Tecnologias.Trim()
        };
        projeto.Id = await _repositorio.InserirAsync(projeto);
        return projeto;
    }

    public async Task<Projeto> SubstituirAsync(int id, ProjetoDto dto)
    {
        var projeto = await ObterAsync(id);
        projeto.Titulo = dto.Titulo.Trim();
        projeto.Status = dto.Status.Trim();
        projeto.DataInicio = dto.DataInicio!.Value.Date;
        projeto.Tecnologias = dto.Tecnologias.Trim();
        await _repositorio.AtualizarAsync(projeto);
        return projeto;
    }

    public async Task<Projeto> AtualizarParcialAsync(int id, ProjetoPatchDto dto)
    {
        if (dto.Titulo is null && dto.Status is null && dto.DataInicio is null && dto.Tecnologias is null)
            throw new RequisicaoInvalidaException("Informe ao menos um campo para atualizar.");

        var projeto = await ObterAsync(id);
        if (dto.Titulo is not null) projeto.Titulo = dto.Titulo.Trim();
        if (dto.Status is not null) projeto.Status = dto.Status.Trim();
        if (dto.DataInicio is not null) projeto.DataInicio = dto.DataInicio.Value.Date;
        if (dto.Tecnologias is not null) projeto.Tecnologias = dto.Tecnologias.Trim();
        await _repositorio.AtualizarAsync(projeto);
        return projeto;
    }

    public async Task ExcluirAsync(int id)
    {
        var excluiu = await _repositorio.ExcluirAsync(id);
        if (!excluiu)
            throw new NaoEncontradoException($"Projeto com id {id} nao encontrado.");
    }
}
