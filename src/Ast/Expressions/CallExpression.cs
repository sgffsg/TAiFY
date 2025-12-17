namespace Ast.Expressions;

/// <summary>
/// Вызов функции/процедуры.
/// </summary>
public class CallExpression : Expression
{
    public CallExpression(string name, List<Expression> arguments)
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