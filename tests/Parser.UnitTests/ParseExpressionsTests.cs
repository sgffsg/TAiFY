using Ast;
using Ast.Expressions;
using Execution;
using Runtime;
using Xunit;

namespace Parser.UnitTests;

public class ParseExpressionsTests
{
    [Theory]
    [MemberData(nameof(GetExpressionTests))]
    public void Can_parse_expressions_data(string code, object expected)
    {
        FakeEnvironment environment = new();
        Context context = new();
        AstEvaluator evaluator = new(context, environment);
        Parser parser = new(code);

        Expression result = parser.EvaluateExpression();
        Value value = evaluator.Evaluate(result);

        object actual = value.GetValueType() switch
        {
            Runtime.ValueType.ЦИФЕРКА => value.AsInt(),
            Runtime.ValueType.ПОЛТОРАШКА => value.AsDouble(),
            Runtime.ValueType.ЦИТАТА => value.AsString(),
            Runtime.ValueType.РАСКЛАД => value.AsBool(),
            _ => throw new NotSupportedException()
        };

        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, object> GetExpressionTests()
    {
        return new TheoryData<string, object>
        {
            { "42;", 42 },
            { "-42;", -42 },
            { "3.14;", 3.14 },
            { "-3.14;", -3.14 },
            { "0;", 0 },
            { "ХАЙП;", true },
            { "КРИНЖ;", false },
            { "\"Hello\";", "Hello" },
            { "2 + 3;", 5 },
            { "5 - 2;", 3 },
            { "3 * 4;", 12 },
            { "10 / 2;", 5 },
            { "7 % 3;", 1 },
            { "2 + 3 * 4;", 14 },
            { "ХАЙП И ХАЙП;", true },
            { "ХАЙП И КРИНЖ;", false },
            { "ХАЙП ИЛИ КРИНЖ;", true },
            { "КРИНЖ ИЛИ КРИНЖ;", false },
            { "НЕ ХАЙП;", false },
            { "НЕ КРИНЖ;", true },
            { "5 == 5;", true },
            { "5 == 3;", false },
            { "5 != 3;", true },
            { "5 != 5;", false },
            { "3 < 5;", true },
            { "5 <= 5;", true },
            { "5 > 3;", true },
            { "5 >= 5;", true },
            { "10 - 4 / 2;", 8 },
            { "-3 + 5;", 2 },
            { "2 < 3 И 4 > 1;", true },
            { "10 - 5 - 2;", 3 },
            { "12 / 3 / 2;", 2 },
            { "+5;", 5 },
            { "НЕ НЕ ХАЙП;", true },
            { "-5 + 3;", -2 },
            { "- + -5;", 5 },
            { "-2 * 3;", -6 },
            { "-(2 + 3);", -5 },
            { "(2 + 3) * 4;", 20 },
            { "2 + (3 * 4);", 14 },
            { "((2 + 3) * (4 - 1));", 15 },
            { "МОДУЛЬ(-5.0);", 5.0 },
            { "СТЕПЕНЬ(2.0, 3.0);", 8.0 },
            { "МИНИМУМ(3.0, 1.0, 2.0);", 1.0 },
            { "МОДУЛЬ(МИНИМУМ(-1.0, 5.0));", 1.0 },
        };
    }
}