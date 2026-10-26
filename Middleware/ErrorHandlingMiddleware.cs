using ExoApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ExoApi.Middleware;

/// <summary>
/// Captura as excecoes lancadas pela aplicacao e devolve uma resposta JSON padronizada
/// (ProblemDetails) com o codigo de status HTTP adequado.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await TratarAsync(context, ex);
        }
    }

    private async Task TratarAsync(HttpContext context, Exception ex)
    {
        var (status, titulo, detalhe) = ex switch
        {
            NaoEncontradoException => (StatusCodes.Status404NotFound, "Recurso nao encontrado", ex.Message),
            RequisicaoInvalidaException => (StatusCodes.Status400BadRequest, "Requisicao invalida", ex.Message),
            NaoAutorizadoException => (StatusCodes.Status401Unauthorized, "Nao autorizado", ex.Message),
            SqlException => (StatusCodes.Status503ServiceUnavailable, "Banco de dados indisponivel",
                             "Nao foi possivel acessar o banco de dados. Tente novamente mais tarde."),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor",
                  "Ocorreu um erro inesperado ao processar a requisicao.")
        };

        if (status >= 500)
            _logger.LogError(ex, "Erro ao processar {Metodo} {Caminho}", context.Request.Method, context.Request.Path);
        else
            _logger.LogWarning("{Titulo}: {Detalhe}", titulo, detalhe);

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = detalhe,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problema);
    }
}
