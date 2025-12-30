using Runtime;

namespace Execution.Exceptions;

public class ReturnException : Exception
{
    public ReturnException() => ReturnValue = Value.Void;

    public ReturnException(string message)
        : base(message)
    {
        ReturnValue = Value.Void;
    }

    public ReturnException(string message, Exception innerException)
        : base(message, innerException)
    {
        ReturnValue = Value.Void;
    }

    public ReturnException(Value returnValue)
    {
        ReturnValue = returnValue;
    }

    public Value ReturnValue { get; }
}