using Runtime;
using ValueType = Runtime.ValueType;

namespace Execution;

public interface IEnvironment
{
    Value Read(ValueType expectedType);

    void Write(string message);
}
