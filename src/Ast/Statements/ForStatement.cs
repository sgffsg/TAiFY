using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Цикл for.
/// </summary>
public sealed class ForStatement : Statement
{
    public ForStatement(Statement? initialization, Expression? condition, Statement? increment, Statement body)
    {
        Initialization = initialization;
        Condition = condition;
        Increment = increment;
        Body = body;
    }

    /// <summary>
    /// Опциональная инициализация цикла.
    /// </summary>
    public Statement? Initialization { get; }

    /// <summary>
    /// Опциональное условие цикла.
    /// </summary>
    public Expression? Condition { get; }

    /// <summary>
    /// Опциональное выражение инкремента.
    /// </summary>
    public Statement? Increment { get; }

    /// <summary>
    /// Тело цикла.
    /// </summary>
    public Statement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}