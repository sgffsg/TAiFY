namespace Ast.Expressions;

public sealed class BinaryOperationExpression : Expression
{
    public BinaryOperationExpression(Expression left, BinaryOperation operation, Expression right)
    {
        Left = left;
        Operation = operation;
        Right = right;
    }

    public Expression Left { get; }

    public BinaryOperation Operation { get; }

    public Expression Right { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public enum BinaryOperation
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
    /// Арифметический оператор умножения "*"
    /// </summary>
    Multiplication,

    /// <summary>
    /// Арифметический оператор деления "/"
    /// </summary>
    Division,

    /// <summary>
    /// Арифметический оператор остаток "%"
    /// </summary>
    Remainder,

    /// <summary>
    /// Оператор сравнения равно "==".
    /// </summary>
    Equal,

    /// <summary>
    /// Оператор сравнения не равно "!=".
    /// </summary>
    NotEqual,

    /// <summary>
    /// Оператор сравнения меньше "<".
    /// </summary>
    LessThan,

    /// <summary>
    /// Оператор сравнения меньше или равно "<=".
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Оператор сравнения больше ">".
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Оператор сравнения больше или равно ">=".
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Логический оператор И
    /// </summary>
    And,

    /// <summary>
    /// Логический оператор ИЛИ
    /// </summary>
    Or,
}
