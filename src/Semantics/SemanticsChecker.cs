using Ast;
using Ast.Declarations;
using Semantics.Passes;
using Semantics.Symbols;

namespace Semantics;

public class SemanticsChecker
{
    private readonly AbstractPass[] passes;

    private readonly IReadOnlyList<ConstantDeclaration> builtinConstants;

    public SemanticsChecker(
        IReadOnlyList<BuiltinFunction> builtins,
        IReadOnlyList<ConstantDeclaration> builtinConstants,
        IReadOnlyList<AbstractTypeDeclaration> builtinTypes
    )
    {
        this.builtinConstants = builtinConstants;
        SymbolsTable globalSymbols = new(parent: null);

        foreach (AbstractTypeDeclaration type in builtinTypes)
        {
            globalSymbols.DefineSymbol(type.Name, type);
        }

        foreach (BuiltinFunction function in builtins)
        {
            globalSymbols.DefineSymbol(function.Name, function);
        }

        passes = [
            new ResolveNamesPass(globalSymbols),
            new CheckContextSensitiveRulesPass(),
            new ResolveTypesPass(),
        ];
    }

    public void Check(List<AstNode> nodes)
    {
        foreach (AbstractPass pass in passes)
        {
            foreach (AstNode node in nodes)
            {
                node.Accept(pass);
            }
        }
    }
}