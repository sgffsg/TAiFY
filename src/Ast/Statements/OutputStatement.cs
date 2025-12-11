using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Вывод значений выражений в консоль.
/// </summary>
public sealed class OutputStatement : Statement
{
    public OutputStatement(List<Expression> arguments)
    {
        Arguments = arguments;
    }

    /// <summary>
    /// Список выражений для вывода.
    /// </summary>
    public List<Expression> Arguments { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}