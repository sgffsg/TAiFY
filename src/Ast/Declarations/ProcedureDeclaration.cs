using Ast.Expressions;

namespace Ast.Declarations;

/// <summary>
/// Объявление процедуры.
/// </summary>
public sealed class ProcedureDeclaration : Declaration
{
    public ProcedureDeclaration(string name, List<string> parameters, Expression body)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
    }

    /// <summary>
    /// Название процедуры.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Список параметров процедуры.
    /// </summary>
    public List<string> Parameters { get; }

    /// <summary>
    /// Тело процедуры.
    /// </summary>
    public Expression Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}