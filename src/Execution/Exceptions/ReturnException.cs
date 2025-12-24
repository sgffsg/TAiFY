namespace Execution.Exceptions;

public class ReturnException : Exception
{
    public ReturnException()
        : base()
    {
    }

    public ReturnException(double value)
    {
        Value = value;
    }

    public ReturnException(string? message)
        : base(message)
    {
    }

    public ReturnException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public double Value { get; }
}