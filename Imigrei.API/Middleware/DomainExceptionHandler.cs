using Imigrei.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Imigrei.API.Middleware;

/// <summary>
/// Traduz exceções do domínio em respostas HTTP, mantendo os controllers
/// livres de blocos try/catch repetidos.
/// </summary>
public class DomainExceptionHandler : IExceptionHandler
{
    private readonly ILogger<DomainExceptionHandler> _logger;

    public DomainExceptionHandler(ILogger<DomainExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, titulo) = exception switch
        {
            IntegridadeArquivoException => (StatusCodes.Status500InternalServerError, "Falha de integridade do arquivo"),
            CredenciaisInvalidasException => (StatusCodes.Status401Unauthorized, "Falha de autenticação"),
            DomainException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
            _ => (0, string.Empty),
        };

        // Exceções inesperadas seguem para o tratamento padrão do pipeline.
        if (status == 0)
            return false;

        // 5xx aqui significa problema no servidor (arquivo adulterado/corrompido), não erro do cliente.
        if (status >= StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Requisição falhou com {Status}.", status);
        else
            _logger.LogInformation(exception, "Requisição rejeitada com {Status}.", status);

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problema, cancellationToken);

        return true;
    }
}
