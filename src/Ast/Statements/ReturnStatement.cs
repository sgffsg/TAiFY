using Ast.Expressions;

namespace Ast.Statements;

public sealed class ReturnStatement : Expression
{
    public ReturnStatement(Expression? value)
    {
        Value = value;
    }

    public Expression? Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}