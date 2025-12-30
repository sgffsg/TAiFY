using Ast.Declarations;
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
        new(
            "ПОДЦИТАТА",
            [new BuiltinFunctionParameter("строка", ValueType.ЦИТАТА), new BuiltinFunctionParameter("от", ValueType.ЦИФЕРКА), new BuiltinFunctionParameter("длина", ValueType.ЦИФЕРКА),],
            ValueType.ЦИТАТА,
            args =>
            {
                string text = args[0].AsString();
                int fromIndex = args[1].AsInt();
                int length = args[2].AsInt();

                int safeLength = Math.Min(length, Math.Max(0, text.Length - fromIndex));

                if (fromIndex < 0 || fromIndex >= text.Length)
                {
                    return new Value("");
                }

                return new Value(text.Substring(fromIndex, safeLength));
            }
        ),
        new(
            "ПОИСК",
            [new BuiltinFunctionParameter("где", ValueType.ЦИТАТА), new BuiltinFunctionParameter("что", ValueType.ЦИТАТА)],
            ValueType.ЦИФЕРКА,
            args => new Value(args[0].AsString().IndexOf(args[1].AsString()))
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
}