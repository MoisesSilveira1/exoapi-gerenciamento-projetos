namespace ExoApi.Exceptions;

/// <summary>Recurso nao encontrado (HTTP 404).</summary>
public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string mensagem) : base(mensagem) { }
}

/// <summary>Requisicao invalida segundo as regras de negocio (HTTP 400).</summary>
public class RequisicaoInvalidaException : Exception
{
    public RequisicaoInvalidaException(string mensagem) : base(mensagem) { }
}

/// <summary>Credenciais invalidas (HTTP 401).</summary>
public class NaoAutorizadoException : Exception
{
    public NaoAutorizadoException(string mensagem) : base(mensagem) { }
}
