using Ast.Expressions;

namespace Ast.Statements;

public sealed class OutputStatement : Expression
{
    public OutputStatement(List<Expression> arguments)
    {
        Arguments = arguments;
    }

    public List<Expression> Arguments { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}