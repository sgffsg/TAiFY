namespace Ast.Statements;

/// <summary>
/// Прерывание выполнения цикла.
/// </summary>
public sealed class BreakStatement : Statement
{
    public BreakStatement()
    {
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}