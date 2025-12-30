using Runtime;
using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

public sealed class BuiltinFunction : AbstractFunctionDeclaration
{
    private readonly Func<IReadOnlyList<Value>, Value> implementation;

    public BuiltinFunction(
        string name,
        IReadOnlyList<BuiltinFunctionParameter> parameters,
        ValueType resultType,
        Func<IReadOnlyList<Value>, Value> implementation
    )
        : base(name, parameters)
    {
        ResultType = resultType;
        this.implementation = implementation;
    }

    public Value Invoke(IReadOnlyList<Value> arguments)
    {
        return implementation(arguments);
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}