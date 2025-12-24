namespace Ast.Statements;

public sealed class BlockStatement : Statement
{
    public BlockStatement(List<Statement> statements)
    {
        Statements = statements;
    }

    public List<Statement> Statements { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
