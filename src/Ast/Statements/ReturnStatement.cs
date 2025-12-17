using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Возврат значения из функции.
/// </summary>
public sealed class ReturnStatement : Statement
{
    public ReturnStatement(Expression value)
    {
        Value = value;
    }

    /// <summary>
    /// Возвращаемое значение функции.
    /// </summary>
    public Expression Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}