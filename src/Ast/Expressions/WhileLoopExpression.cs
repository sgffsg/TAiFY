namespace Ast.Expressions;

public sealed class WhileLoopExpression : Expression
{
    public WhileLoopExpression(Expression condition, Expression body)
    {
        Condition = condition;
        Body = body;
    }

    /// <summary>
    /// Условие продолжения цикла.
    /// </summary>
    public Expression Condition { get; }

    /// <summary>
    /// Тело цикла.
    /// </summary>
    public Expression Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}