using Ast.Declarations;

using Semantics.Exceptions;

namespace Semantics.Symbols;

public class SymbolsTable
{
    private readonly SymbolsTable? parent;

    private readonly Dictionary<string, Declaration> symbols;

    public SymbolsTable(SymbolsTable? parent)
    {
        parent = parent;
        symbols = [];
    }

    public SymbolsTable? Parent => parent;

    public Declaration GetSymbol(string name)
    {
        if (symbols.TryGetValue(name, out Declaration? symbol))
        {
            return symbol;
        }

        if (parent != null)
        {
            return parent.GetSymbol(name);
        }

        throw new UnknownSymbolException(name);
    }

    public void DefineSymbol(string name, Declaration symbol)
    {
        if (!symbols.TryAdd(name, symbol))
        {
            throw new DuplicateSymbolException(name);
        }
    }
}