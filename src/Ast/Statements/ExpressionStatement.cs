using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Инструкция для выполнения.
/// </summary>
public sealed class ExpressionStatement : Statement
{
    public ExpressionStatement(Expression expression)
    {
        Expression = expression;
    }

    /// <summary>
    /// Выражение для выполнения.
    /// </summary>
    public Expression Expression { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}