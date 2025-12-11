namespace Ast.Expressions;

/// <summary>
/// Литеральное значение (число, строка, булевое значение, null).
/// </summary>
public sealed class LiteralExpression : Expression
{
    public LiteralExpression(object value, string type)
    {
        Value = value;
        Type = type;
    }

    /// <summary>
    /// Значение литерала.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Тип литерала.
    /// </summary>
    public string Type { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
