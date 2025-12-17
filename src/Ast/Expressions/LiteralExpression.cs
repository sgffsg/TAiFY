namespace Ast.Expressions;

/// <summary>
/// Литеральное значение (число, строка, булевое значение, null).
/// </summary>
public sealed class LiteralExpression : Expression
{
    public LiteralExpression(object value)
    {
        Value = value;
    }

    /// <summary>
    /// Значение литерала.
    /// </summary>
    public object Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
