using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Runtime;

namespace Semantics.Passes;

public sealed class ResolveTypesPass : AbstractPass
{
    public override void Visit(VariableDeclaration d)
    {
        d.DeclaredType = Symbols.Lookup(d.Type) as AbstractTypeDeclaration;
        base.Visit(d);
    }

    public override void Visit(ParameterDeclaration d)
    {
        d.Type = Symbols.Lookup(d.TypeName) as AbstractTypeDeclaration
            ?? throw new Exception($"Тип {d.TypeName} не найден.");
    }
}
