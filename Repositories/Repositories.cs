using Dapper;
using ExoApi.Models;
using Microsoft.Data.SqlClient;

namespace ExoApi.Repositories;

public interface IProjetoRepository
{
    Task<IEnumerable<Projeto>> ListarAsync();
    Task<Projeto?> ObterPorIdAsync(int id);
    Task<int> InserirAsync(Projeto projeto);
    Task<bool> AtualizarAsync(Projeto projeto);
    Task<bool> ExcluirAsync(int id);
}

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorLoginAsync(string login);
    Task InserirAsync(Usuario usuario);
}

/// <summary>Acesso a tabela Projetos (camada de dados).</summary>
public class ProjetoRepository : IProjetoRepository
{
    private readonly string _connectionString;

    public ProjetoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' nao configurada.");
    }

    public async Task<IEnumerable<Projeto>> ListarAsync()
    {
        using var conexao = new SqlConnection(_connectionString);
        return await conexao.QueryAsync<Projeto>(
            "SELECT Id, Titulo, Status, DataInicio, Tecnologias FROM Projetos ORDER BY Id");
    }

    public async Task<Projeto?> ObterPorIdAsync(int id)
    {
        using var conexao = new SqlConnection(_connectionString);
        return await conexao.QuerySingleOrDefaultAsync<Projeto>(
            "SELECT Id, Titulo, Status, DataInicio, Tecnologias FROM Projetos WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<int> InserirAsync(Projeto projeto)
    {
        using var conexao = new SqlConnection(_connectionString);
        return await conexao.ExecuteScalarAsync<int>(
            @"INSERT INTO Projetos (Titulo, Status, DataInicio, Tecnologias)
              OUTPUT INSERTED.Id
              VALUES (@Titulo, @Status, @DataInicio, @Tecnologias)",
            projeto);
    }

    public async Task<bool> AtualizarAsync(Projeto projeto)
    {
        using var conexao = new SqlConnection(_connectionString);
        var linhas = await conexao.ExecuteAsync(
            @"UPDATE Projetos
                 SET Titulo = @Titulo, Status = @Status,
                     DataInicio = @DataInicio, Tecnologias = @Tecnologias
               WHERE Id = @Id",
            projeto);
        return linhas > 0;
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        using var conexao = new SqlConnection(_connectionString);
        var linhas = await conexao.ExecuteAsync("DELETE FROM Projetos WHERE Id = @Id", new { Id = id });
        return linhas > 0;
    }
}

/// <summary>Acesso a tabela Usuarios (camada de dados).</summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly string _connectionString;

    public UsuarioRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' nao configurada.");
    }

    public async Task<Usuario?> ObterPorLoginAsync(string login)
    {
        using var conexao = new SqlConnection(_connectionString);
        return await conexao.QuerySingleOrDefaultAsync<Usuario>(
            "SELECT Id, Login, SenhaHash FROM Usuarios WHERE Login = @Login",
            new { Login = login });
    }

    public async Task InserirAsync(Usuario usuario)
    {
        using var conexao = new SqlConnection(_connectionString);
        await conexao.ExecuteAsync(
            "INSERT INTO Usuarios (Login, SenhaHash) VALUES (@Login, @SenhaHash)", usuario);
    }
}
