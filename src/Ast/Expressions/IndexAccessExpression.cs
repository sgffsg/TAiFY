namespace Ast.Expressions;

public sealed class IndexAccessExpression : Expression
{
    public IndexAccessExpression(Expression target, Expression index)
    {
        Target = target;
        Index = index;
    }

    /// <summary>
    /// Выражение, к которому применяется индексация (должно иметь тип ЦИТАТА).
    /// </summary>
    public Expression Target { get; }

    /// <summary>
    /// Выражение индекса (должно иметь тип ЦИФЕРКА).
    /// </summary>
    public Expression Index { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
