using Ast.Statements;

namespace Ast.Declarations;

public sealed class FunctionDeclaration : Declaration
{
    public FunctionDeclaration(string returnType, string name, List<Parameter> parameters, BlockStatement body)
    {
        ReturnType = returnType;
        Name = name;
        Parameters = parameters;
        Body = body;
    }

    public string ReturnType { get; }

    public string Name { get; }

    public List<Parameter> Parameters { get; }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}