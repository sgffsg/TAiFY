namespace Ast.Expressions;

public sealed class ForLoopExpression : Expression
{
    public ForLoopExpression(Expression initialization, Expression condition, Expression increment, Expression body)
    {
        Initialization = initialization;
        Condition = condition;
        Increment = increment;
        Body = body;
    }

    public Expression Initialization { get; }

    public Expression Condition { get; }

    public Expression Increment { get; }

    public Expression Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}