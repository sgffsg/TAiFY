using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;
using Semantics.Symbols;

namespace Semantics.Passes;

public sealed class ResolveNamesPass : AbstractPass
{
    private readonly SymbolsTable symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        this.symbols = globalSymbols;
    }

    public override void Visit(ConstantDeclaration d)
    {
        base.Visit(d);
        d.ResultType = ConvertTypeNameToValueType(d.Type);
        symbols.DefineSymbol(d.Name, d);
    }

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);
        d.ResultType = ConvertTypeNameToValueType(d.Type);
        symbols.DefineSymbol(d.Name, d);
    }

    public override void Visit(VariableExpression e)
    {
        base.Visit(e);
        AbstractVariableDeclaration decl = ResolveVariable(e.Name);
        e.Variable = decl;
        e.ResultType = decl.ResultType;
    }

    private AbstractVariableDeclaration ResolveVariable(string name)
    {
        Declaration symbol = symbols.GetSymbol(name);
        if (symbol is AbstractVariableDeclaration variable)
        {
            return variable;
        }

        throw new InvalidSymbolException(
            $"Name {name} does not refer to a variable"
        );
    }

    private AbstractTypeDeclaration ResolveType(string name)
    {
        Declaration symbol = symbols.GetSymbol(name);
        if (symbol is AbstractTypeDeclaration type)
        {
            return type;
        }

        throw new InvalidSymbolException(
            $"Name {name} does not refer to a type"
        );
    }

    private Runtime.ValueType ConvertTypeNameToValueType(string typeName)
    {
        return typeName.ToLower() switch
        {
            "ЦИФЕРКА" => Runtime.ValueType.Ciferka,
            "ПОЛТОРАШКА" => Runtime.ValueType.Poltorashka,
            "ЦИТАТА" => Runtime.ValueType.Citata,
            "РАСКЛАД" => Runtime.ValueType.Rasklad,
            _ => throw new Exception($"Неизвестный тип: {typeName}"),
        };
    }
}
