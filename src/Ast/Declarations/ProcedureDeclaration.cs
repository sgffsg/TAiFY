using Ast.Statements;

namespace Ast.Declarations;

/// <summary>
/// Объявление процедуры.
/// </summary>
public sealed class ProcedureDeclaration : Declaration
{
    public ProcedureDeclaration(string name, List<ParameterDeclaration> parameters, BlockStatement body)
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
    public List<ParameterDeclaration> Parameters { get; }

    /// <summary>
    /// Тело процедуры.
    /// </summary>
    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}