using Ast.Expressions;

namespace Ast.Statements;

public sealed class BlockStatement : Expression
{
    public BlockStatement(List<AstNode> statements)
    {
        Statements = statements;
    }

    /// <summary>
    /// Список инструкций, объявлений или вложенных блоков.
    /// </summary>
    public List<AstNode> Statements { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
