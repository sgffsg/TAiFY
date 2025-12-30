using Ast.Attributes;
using Ast.Statements;

namespace Ast.Declarations;

public sealed class FunctionDeclaration : AbstractFunctionDeclaration
{
    private AstAttribute<AbstractTypeDeclaration?> declaredType;

    public FunctionDeclaration(string name, IReadOnlyList<ParameterDeclaration> parameters, string? typeName, BlockStatement body)
        : base(name, parameters)
    {
        TypeName = typeName;
        Body = body;
    }

    public string? TypeName { get; }

    public BlockStatement Body { get; }

    public AbstractTypeDeclaration? DeclaredType
    {
        get => declaredType.Get();
        set => declaredType.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}