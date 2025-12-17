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

    public string Name { get; }

    public List<Expression> Arguments { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}