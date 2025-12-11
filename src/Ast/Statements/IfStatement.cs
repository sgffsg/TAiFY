using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Условный оператор ветвления.
/// </summary>
public sealed class IfStatement : Statement
{
    public IfStatement(Expression condition, Statement thenBranch, Statement? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    /// <summary>
    /// Условие выполнения.
    /// </summary>
    public Expression Condition { get; }

    /// <summary>
    /// Ветка для выполнения при истинном условии.
    /// </summary>
    public Statement ThenBranch { get; }

    /// <summary>
    /// Опциональная ветка для выполнения при ложном условии.
    /// </summary>
    public Statement? ElseBranch { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
