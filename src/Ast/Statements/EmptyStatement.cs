using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Пустая инструкция.
/// </summary>
public sealed class EmptyStatement : Statement
{
    public EmptyStatement()
    {
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}