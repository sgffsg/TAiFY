namespace Execution.Exceptions;

public class ContinueException : Exception
{
    public ContinueException()
        : base()
    {
    }

    public ContinueException(string? message)
        : base(message)
    {
    }

    public ContinueException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}