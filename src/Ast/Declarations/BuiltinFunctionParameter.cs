using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

public class BuiltinFunctionParameter : AbstractParameterDeclaration
{
    public BuiltinFunctionParameter(string name, ValueType type)
        : base(name)
    {
        ResultType = type;
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}
