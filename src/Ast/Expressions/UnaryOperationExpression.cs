namespace Ast.Expressions;

/// <summary>
/// Унарная операция над выражением.
/// </summary>
public sealed class UnaryOperationExpression : Expression
{
    public UnaryOperationExpression(UnaryOperation operation, Expression operand)
    {
        Operation = operation;
        Operand = operand;
    }

    /// <summary>
    /// Тип унарной операции.
    /// </summary>
    public UnaryOperation Operation { get; }

    /// <summary>
    /// Операнд операции.
    /// </summary>
    public Expression Operand { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public enum UnaryOperation
{
    /// <summary>
    /// Арифметический оператор плюс "+"
    /// </summary>
    Plus,

    /// <summary>
    /// Арифметический оператор минус "-"
    /// </summary>
    Minus,

    /// <summary>
    /// Логический оператор НЕ
    /// </summary>
    Not,
}