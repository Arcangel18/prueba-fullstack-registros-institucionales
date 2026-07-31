namespace RegistrosInstitucionales.Api.Exceptions;

public class CuotaExcedidaException : Exception
{
    public CuotaExcedidaException(string message) : base(message)
    {
    }
}