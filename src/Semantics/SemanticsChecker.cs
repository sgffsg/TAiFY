using Ast;
using Ast.Declarations;
using Semantics.Passes;
using Semantics.Symbols;

namespace Semantics;

public class SemanticsChecker
{
    private readonly AbstractPass[] passes;

    public SemanticsChecker(
        IReadOnlyList<BuiltinFunction> builtins,
        IReadOnlyList<ConstantDeclaration> builtinConstants,
        IReadOnlyList<AbstractTypeDeclaration> builtinTypes
    )
    {
        SymbolsTable globalSymbols = new(parent: null);

        foreach (AbstractTypeDeclaration type in builtinTypes)
        {
            globalSymbols.DefineSymbol(type.Name, type);
        }

        foreach (ConstantDeclaration constant in builtinConstants)
        {
            globalSymbols.DefineSymbol(constant.Name, constant);
        }

        foreach (BuiltinFunction function in builtins)
        {
            globalSymbols.DefineSymbol(function.Name, function);
        }

        passes = [
            new ResolveNamesPass(globalSymbols),
            new CheckContextSensitiveRulesPass(),
            new ResolveTypesPass(),
            new CheckTypesPass(),
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