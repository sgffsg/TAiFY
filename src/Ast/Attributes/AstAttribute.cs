using System.Runtime.CompilerServices;

namespace Ast.Attributes;

public struct AstAttribute<T>
{
    private T value;
    private bool initialized;

    public T Get([CallerMemberName] string? memberName = null)
    {
        if (!initialized)
        {
            throw new InvalidOperationException($"Attribute {memberName} of type {typeof(T)} is not set");
        }

        return value;
    }

    public void Set(T value, [CallerMemberName] string? memberName = null)
    {
        if (initialized)
        {
            throw new InvalidOperationException($"Attribute {memberName} already has a value");
        }

        this.value = value;
        this.initialized = true;
    }
}
