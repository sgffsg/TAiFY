using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Цикл с предусловием.
/// </summary>
public sealed class WhileStatement : Statement
{
    public WhileStatement(Expression condition, Statement body)
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
    public Statement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}