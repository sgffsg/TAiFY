using Ast.Statements;

namespace Ast.Declarations;

/// <summary>
/// Объявление функции.
/// </summary>
public sealed class FunctionDeclaration : Declaration
{
    public FunctionDeclaration(string returnType, string name, List<ParameterDeclaration> parameters, BlockStatement body)
    {
        ReturnType = returnType;
        Name = name;
        Parameters = parameters;
        Body = body;
    }

    /// <summary>
    /// Возвращаемый тип.
    /// </summary>
    public string ReturnType { get; }

    /// <summary>
    /// Название функции.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Список параметров функции.
    /// </summary>
    public List<ParameterDeclaration> Parameters { get; }

    /// <summary>
    /// Тело функции.
    /// </summary>
    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}