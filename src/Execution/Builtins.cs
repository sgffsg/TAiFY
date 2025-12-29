using Ast.Declarations;
using Ast.Expressions;
using Runtime;

using ValueType = Runtime.ValueType;

namespace Execution;

public static class Builtins
{
    /// <summary>
    /// Список встроенных функций языка.
    /// </summary>
    public static IReadOnlyList<BuiltinFunction> Functions { get; } = [
        new(
            "МОДУЛЬ",
            [new BuiltinFunctionParameter("x", ValueType.ПОЛТОРАШКА)],
            ValueType.ПОЛТОРАШКА,
            args => new Value(Math.Abs(args[0].AsDouble()))
        ),
        new(
            "МИНИМУМ",
            [new BuiltinFunctionParameter("x", ValueType.ПОЛТОРАШКА), new BuiltinFunctionParameter("y", ValueType.ПОЛТОРАШКА)],
            ValueType.ПОЛТОРАШКА,
            args => new Value(Math.Min(args[0].AsDouble(), args[1].AsDouble()))
        ),
        new(
            "МАКСИМУМ",
            [new BuiltinFunctionParameter("x", ValueType.ПОЛТОРАШКА), new BuiltinFunctionParameter("y", ValueType.ПОЛТОРАШКА)],
            ValueType.ПОЛТОРАШКА,
            args => new Value(Math.Max(args[0].AsDouble(), args[1].AsDouble()))
        ),
        new(
            "СИНУС",
            [new BuiltinFunctionParameter("x", ValueType.ПОЛТОРАШКА)],
            ValueType.ПОЛТОРАШКА,
            args => new Value(Math.Sin(args[0].AsDouble()))
        ),
        new(
            "КОСИНУС",
            [new BuiltinFunctionParameter("x", ValueType.ПОЛТОРАШКА)],
            ValueType.ПОЛТОРАШКА,
            args => new Value(Math.Cos(args[0].AsDouble()))
        ),
        new(
            "ТАНГЕНС",
            [new BuiltinFunctionParameter("x", ValueType.ПОЛТОРАШКА)],
            ValueType.ПОЛТОРАШКА,
            args => new Value(Math.Tan(args[0].AsDouble()))
        ),
        new(
            "ДЛИНА",
            [new BuiltinFunctionParameter("строка", ValueType.ЦИТАТА)],
            ValueType.ЦИФЕРКА,
            args => new Value(args[0].AsString().Length)
        ),
    ];

    /// <summary>
    /// Список встроенных типов языка.
    /// </summary>
    public static IReadOnlyList<BuiltinType> Types { get; } = [
        new("ЦИФЕРКА", ValueType.ЦИФЕРКА),
        new("ПОЛТОРАШКА", ValueType.ПОЛТОРАШКА),
        new("ЦИТАТА", ValueType.ЦИТАТА),
        new("РАСКЛАД", ValueType.РАСКЛАД)
    ];

    /// <summary>
    /// Список встроенных констант языка.
    /// </summary>
    public static IReadOnlyList<ConstantDeclaration> Constants { get; } = [
        new ConstantDeclaration("ПОЛТОРАШКА", "ПИ", new LiteralExpression(new Value(3.1415926535))),
        new ConstantDeclaration("ПОЛТОРАШКА", "ЕШКА", new LiteralExpression(new Value(2.7182818284)))
    ];
}