using Ast.Attributes;
using Ast.Declarations;

namespace Ast.Expressions;

/// <summary>
/// Вызов функции/процедуры.
/// </summary>
public class FunctionCallExpression : Expression
{
    private AstAttribute<AbstractFunctionDeclaration> function;

    public FunctionCallExpression(string functionName, List<Expression> arguments)
    {
        FunctionName = functionName;
        Arguments = arguments;
    }

    public string FunctionName { get; }

    public List<Expression> Arguments { get; }

    public AbstractFunctionDeclaration Function
    {
        get => function.Get();
        set => function.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}