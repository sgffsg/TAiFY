using Ast.Expressions;

namespace Ast.Statements;
public sealed class WhileStatement : Statement
{
    public WhileStatement(Expression condition, BlockStatement body)
    {
        Condition = condition;
        Body = body;
    }

    public Expression Condition { get; }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}