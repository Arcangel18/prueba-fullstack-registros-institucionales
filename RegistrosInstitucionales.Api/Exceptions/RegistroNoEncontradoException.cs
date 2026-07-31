namespace RegistrosInstitucionales.Api.Exceptions;

public class RegistroNoEncontradoException : Exception
{
    public RegistroNoEncontradoException(string message) : base(message)
    {
    }
}