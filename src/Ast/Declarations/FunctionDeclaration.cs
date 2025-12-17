using Ast.Expressions;

namespace Ast.Declarations;

/// <summary>
/// Объявление функции.
/// </summary>
public sealed class FunctionDeclaration : Declaration
{
    public FunctionDeclaration(string name, List<string> parameters, Expression body)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
    }

    /// <summary>
    /// Название функции.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Список параметров функции.
    /// </summary>
    public List<string> Parameters { get; }

    /// <summary>
    /// Тело функции.
    /// </summary>
    public Expression Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}