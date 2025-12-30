using Ast.Attributes;
using Ast.Declarations;

namespace Ast.Expressions;

public sealed class AssignmentExpression : Expression
{
    private AstAttribute<AbstractVariableDeclaration> variable;

    public AssignmentExpression(string name, Expression value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public Expression Value { get; }

    public AbstractVariableDeclaration Variable
    {
        get => variable.Get();
        set => variable.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}