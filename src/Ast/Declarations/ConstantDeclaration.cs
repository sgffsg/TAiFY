using Ast.Attributes;
using Ast.Expressions;

namespace Ast.Declarations;

public sealed class ConstantDeclaration : AbstractVariableDeclaration
{
    private AstAttribute<AbstractTypeDeclaration?> declaredType;

    public ConstantDeclaration(string type, string name, Expression value)
         : base(name)
    {
        Type = type;
        Value = value;
    }

    public string Type { get; }

    public Expression Value { get; }

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
