using Ast.Expressions;

namespace Ast.Statements;

public sealed class ContinueStatement : Expression
{
    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}