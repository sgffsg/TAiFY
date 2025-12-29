using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;
using Semantics.Exceptions;
using Semantics.Symbols;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST, устанавливающий соответствие имён и символов (объявлений).
/// </summary>
public sealed class ResolveNamesPass : AbstractPass
{
    /// <summary>
    /// В таблицу символов складываются объявления.
    /// </summary>
    private SymbolsTable symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        this.symbols = globalSymbols;
    }

    public override void Visit(ConstantDeclaration d)
    {
        base.Visit(d);

        d.DeclaredType = ResolveType(d.Type);
        d.ResultType = ConvertTypeNameToValueType(d.Type);
        DeclareSymbol(d.Name, d);
    }

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);

        d.DeclaredType = ResolveType(d.Type);
        d.ResultType = ConvertTypeNameToValueType(d.Type);
        DeclareSymbol(d.Name, d);
    }

    public override void Visit(VariableExpression e)
    {
        base.Visit(e);

        e.Variable = ResolveVariable(e.Name);
    }

    public override void Visit(ParameterDeclaration d)
    {
        base.Visit(d);

        d.Type = ResolveType(d.TypeName);
        d.ResultType = ConvertTypeNameToValueType(d.TypeName);
        DeclareSymbol(d.Name, d);
    }

    public override void Visit(ForStatement s)
    {
        base.Visit(s);
    }

    public override void Visit(FunctionDeclaration d)
    {
        DeclareSymbol(d.Name, d);
        if (d.TypeName != null)
        {
            d.DeclaredType = ResolveType(d.TypeName);
            d.ResultType = ConvertTypeNameToValueType(d.TypeName);
        }
        else
        {
            d.ResultType = Runtime.ValueType.Void;
        }

        symbols = new SymbolsTable(symbols);
        try
        {
            base.Visit(d);
        }
        finally
        {
            symbols = symbols.Parent!;
        }
    }

    public override void Visit(AssignmentExpression e)
    {
        Declaration symbol = symbols.GetSymbol(e.Name);

        if (symbol is AbstractVariableDeclaration decl)
        {
            e.Variable = decl;
        }
        else
        {
            throw new Exception($"Попытка присвоить значение '{e.Name}', но это не переменная (БАЗА).");
        }

        // 3. Не забываем посетить правую часть присваивания (само выражение)
        e.Value.Accept(this);
    }

    public override void Visit(BlockStatement s)
    {
        symbols = new SymbolsTable(symbols);
        try
        {
            base.Visit(s);
        }
        finally
        {
            symbols = symbols.Parent!;
        }
    }

    public override void Visit(FunctionCallExpression e)
    {
        Declaration symbol = symbols.GetSymbol(e.FunctionName);

        if (symbol is AbstractFunctionDeclaration funcDecl)
        {
            e.Function = funcDecl;
        }
        else
        {
            throw new Exception($"Ошибка: функция '{e.FunctionName}' не найдена в БАЗЕ.");
        }

        foreach (Expression arg in e.Arguments)
        {
            arg.Accept(this);
        }
    }

    private AbstractFunctionDeclaration ResolveFunction(string name)
    {
        Declaration symbol = symbols.GetSymbol(name);
        if (symbol is AbstractFunctionDeclaration function)
        {
            return function;
        }

        throw new InvalidSymbolException(
            $"Name {name} does not refer to a function"
        );
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

    private void DeclareSymbol(string name, Declaration declaration)
    {
        try
        {
            Declaration existing = symbols.GetSymbol(name);
            if (existing != null)
            {
                throw new DuplicateSymbolException($"Имя '{name}' уже занято. Скрытие имен запрещено (БАЗА).");
            }
        }
        catch (UnknownSymbolException)
        {
        }

        symbols.DefineSymbol(name, declaration);
    }

    private Runtime.ValueType ConvertTypeNameToValueType(string typeName)
    {
        return typeName switch
        {
            "ЦИФЕРКА" => Runtime.ValueType.ЦИФЕРКА,
            "ПОЛТОРАШКА" => Runtime.ValueType.ПОЛТОРАШКА,
            "ЦИТАТА" => Runtime.ValueType.ЦИТАТА,
            "РАСКЛАД" => Runtime.ValueType.РАСКЛАД,
            _ => throw new Exception($"Неизвестный тип: {typeName}"),
        };
    }
}
