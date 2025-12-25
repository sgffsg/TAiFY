using Ast.Attributes;
using Ast.Expressions;

namespace Ast.Declarations;

public class VariableDeclaration : AbstractVariableDeclaration
{
    private AstAttribute<AbstractTypeDeclaration?> declaredType;

    public VariableDeclaration(string type, string name, Expression? value)
        : base(name)
    {
        Type = type;
        InitialValue = value;
    }

    public string Type { get; }

    public Expression? InitialValue { get; }

    public AbstractTypeDeclaration? DeclaredType
    {
        get => declaredType.Get();
        set => declaredType.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
