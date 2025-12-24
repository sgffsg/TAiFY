using Ast.Statements;

namespace Ast.Declarations;

public sealed class ProcedureDeclaration : Declaration
{
    public ProcedureDeclaration(string name, List<Parameter> parameters, BlockStatement body)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
    }

    public string Name { get; }

    public List<Parameter> Parameters { get; }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}