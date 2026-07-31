namespace RegistrosInstitucionales.Api.Exceptions;

public class ApiKeyInvalidaException : Exception
{
    public ApiKeyInvalidaException(string message) : base(message)
    {
    }
}