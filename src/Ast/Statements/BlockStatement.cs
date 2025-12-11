namespace Ast.Statements;

/// <summary>
/// Блок инструкций с областью видимости.
/// </summary>
public sealed class BlockStatement : Statement
{
    private readonly List<Statement> statements;

    public BlockStatement(List<Statement> statements)
    {
        this.statements = statements;
    }

    /// <summary>
    /// Список инструкций в блоке.
    /// </summary>
    public IReadOnlyList<Statement> Statements => statements;

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}