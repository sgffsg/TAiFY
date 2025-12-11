using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Вызов функции/процедуры.
/// </summary>
public class CallStatement : Statement
{
    public CallStatement(string name, List<Expression> arguments)
    {
        Name = name;
        Arguments = arguments;
    }

    /// <summary>
    /// Имя вызываемой функции/процедуры.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Список аргументов функции/процедуры.
    /// </summary>
    public List<Expression> Arguments { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}