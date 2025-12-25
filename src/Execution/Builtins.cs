using Ast.Declarations;
using Ast.Expressions;

using Runtime;

using ValueType = Runtime.ValueType;

namespace Execution;

public class Builtins
{
    public Builtins(IEnvironment environment)
    {
        Types = [
            new("ЦИФЕРКА", ValueType.Ciferka),
            new("ПОЛТОРАШКА", ValueType.Poltorashka),
            new("ЦИТАТА", ValueType.Citata),
            new("РАСКЛАД", ValueType.Rasklad)
        ];

        Constants = [
            new ConstantDeclaration("ПОЛТОРАШКА", "ПИ", new LiteralExpression(new Value(3.1415926535))),
            new ConstantDeclaration("ПОЛТОРАШКА", "ЕШКА", new LiteralExpression(new Value(2.7182818284)))
        ];

        Functions = [
            new(
                "МОДУЛЬ",
                [new BuiltinFunctionParameter("x", ValueType.Poltorashka)],
                ValueType.Poltorashka,
                args => new Value(Math.Abs(args[0].AsDouble()))
            ),
            new(
                "МИНИМУМ",
                [new BuiltinFunctionParameter("x", ValueType.Poltorashka), new BuiltinFunctionParameter("y", ValueType.Poltorashka)],
                ValueType.Poltorashka,
                args => new Value(Math.Min(args[0].AsDouble(), args[1].AsDouble()))
            ),
            new(
                "МАКСИМУМ",
                [new BuiltinFunctionParameter("x", ValueType.Poltorashka), new BuiltinFunctionParameter("y", ValueType.Poltorashka)],
                ValueType.Poltorashka,
                args => new Value(Math.Max(args[0].AsDouble(), args[1].AsDouble()))
            ),
            new(
                "СИНУС",
                [new BuiltinFunctionParameter("x", ValueType.Poltorashka)],
                ValueType.Poltorashka,
                args => new Value(Math.Sin(args[0].AsDouble()))
            ),
            new(
                "КОСИНУС",
                [new BuiltinFunctionParameter("x", ValueType.Poltorashka)],
                ValueType.Poltorashka,
                args => new Value(Math.Cos(args[0].AsDouble()))
            ),
            new(
                "ТАНГЕНС",
                [new BuiltinFunctionParameter("x", ValueType.Poltorashka)],
                ValueType.Poltorashka,
                args => new Value(Math.Tan(args[0].AsDouble()))
            ),
            new(
                "ДЛИНА",
                [new BuiltinFunctionParameter("строка", ValueType.Citata)],
                ValueType.Ciferka,
                args => new Value(args[0].AsString().Length)
            ),
        ];
    }

    /// <summary>
    /// Список встроенных функций языка.
    /// </summary>
    public IReadOnlyList<BuiltinFunction> Functions { get; }

    /// <summary>
    /// Список встроенных типов языка.
    /// </summary>
    public IReadOnlyList<BuiltinType> Types { get; }

    /// <summary>
    /// Список встроенных констант языка.
    /// </summary>
    public IReadOnlyList<ConstantDeclaration> Constants { get; }
}