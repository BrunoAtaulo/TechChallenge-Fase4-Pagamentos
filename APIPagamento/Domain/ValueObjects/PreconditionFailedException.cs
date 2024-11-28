namespace Domain.ValueObjects
{

    public class PreconditionFailedException(string message, string paramName) : ArgumentException(message, paramName)
    {
    }
}
