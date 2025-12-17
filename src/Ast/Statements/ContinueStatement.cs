namespace Ast.Statements;

/// <summary>
/// Переход к след. итерации цикла.
/// </summary>
public sealed class ContinueStatement : Statement
{
    public ContinueStatement()
    {
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}