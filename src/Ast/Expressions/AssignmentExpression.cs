namespace Ast.Expressions;

/// <summary>
/// Присваивание значения переменной.
/// </summary>
public sealed class AssignmentExpression : Expression
{
    public AssignmentExpression(string name, Expression value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Имя переменной для присваивания.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Выражение для вычисления значения.
    /// </summary>
    public Expression Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}