using System;
using System.Collections.Generic;
using System.Linq;

using Ast;
using Ast.Expressions;

using Execution;

using Runtime;

using Xunit;

namespace Parser.UnitTests;

public class ParseExpressionsTests
{
    private const double Precision = 1e-10;

    [Theory]
    [MemberData(nameof(GetLiteralsTheory))]
    public void Can_parse_literals_expression(string code, List<double> expected)
    {
        RunBaseTest(code, expected);
    }

    public static TheoryData<string, List<double>> GetLiteralsTheory()
    {
        return new TheoryData<string, List<double>>
        {
            { "42;", [42] },
            { "-42;", [-42] },
            { "3.14;", [3.14] },
            { "-3.14;", [-3.14] },
            { "+5;", [5] },
            { "0;", [0] },
            { "ХАЙП;", [1] },
            { "КРИНЖ;", [0] },
            { "ПИ;", [3.1415926535] },
            { "ЕШКА;", [2.7182818284] },
            { "\"Hello\";", [5] },
            { "\"\";", [0] },
            { "\"Привет мир!\";", [11] },
        };
    }

    [Theory]
    [MemberData(nameof(GetArithmeticOperationsTheory))]
    public void Can_parse_arithmetic_operation_expression(string code, List<double> expected)
    {
        RunBaseTest(code, expected);
    }

    public static TheoryData<string, List<double>> GetArithmeticOperationsTheory()
    {
        return new TheoryData<string, List<double>>
        {
            { "2 + 3;", [5] },
            { "5 - 2;", [3] },
            { "3 * 4;", [12] },
            { "10 / 2;", [5] },
            { "7 % 3;", [1] },
            { "2 + 3 * 4;", [14] },
        };
    }

    [Theory]
    [MemberData(nameof(GetLogicalOperationsData))]
    public void Can_parse_logical_operation_expression(string code, List<double> expected)
    {
        RunBaseTest(code, expected);
    }

    public static TheoryData<string, List<double>> GetLogicalOperationsData()
    {
        return new TheoryData<string, List<double>>
        {
            { "ХАЙП И ХАЙП;", [1] },
            { "ХАЙП И КРИНЖ;", [0] },
            { "ХАЙП ИЛИ КРИНЖ;", [1] },
            { "КРИНЖ ИЛИ КРИНЖ;", [0] },
            { "НЕ ХАЙП;", [0] },
            { "НЕ КРИНЖ;", [1] },
        };
    }

    [Theory]
    [MemberData(nameof(GetComparisonOperationsTheory))]
    public void Can_parse_comparision_operation_expression(string code, List<double> expected)
    {
        RunBaseTest(code, expected);
    }

    public static TheoryData<string, List<double>> GetComparisonOperationsTheory()
    {
        return new TheoryData<string, List<double>>
        {
            { "5 == 5;", [1] },
            { "5 == 3;", [0] },
            { "5 != 3;", [1] },
            { "5 != 5;", [0] },
            { "3 < 5;", [1] },
            { "5 <= 5;", [1] },
            { "5 > 3;", [1] },
            { "5 >= 5;", [1] },
        };
    }

    [Theory]
    [MemberData(nameof(GetOperatorPrecedenceTheory))]
    public void Can_parse_operator_precedence_expression(string code, List<double> expected)
    {
        RunBaseTest(code, expected);
    }

    public static TheoryData<string, List<double>> GetOperatorPrecedenceTheory()
    {
        return new TheoryData<string, List<double>>
        {
            { "10 - 4 / 2;", [8] },
            { "-3 + 5;", [2] },
            { "2 < 3 И 4 > 1;", [1] },
        };
    }

    [Theory]
    [MemberData(nameof(GetOperatorAssociativityTheory))]
    public void Can_parse_operator_associativity_expression(string code, List<double> expected)
    {
        RunBaseTest(code, expected);
    }

    public static TheoryData<string, List<double>> GetOperatorAssociativityTheory()
    {
        return new TheoryData<string, List<double>>
        {
            { "10 - 5 - 2;", [3] },
            { "12 / 3 / 2;", [2] },
            { "+5;", [5] },
            { "-3.14;", [-3.14] },
            { "НЕ НЕ ХАЙП;", [1] },
            { "-5 + 3;", [-2] },
            { "- + -5;", [5] },
            { "-2 * 3;", [-6] },
            { "-(2 + 3);", [-5] },
        };
    }

    [Theory]
    [MemberData(nameof(GetParenthesesGroupingTheory))]
    public void Can_parse_parentheses_grouping_expression(string code, List<double> expected)
    {
        RunBaseTest(code, expected);
    }

    public static TheoryData<string, List<double>> GetParenthesesGroupingTheory()
    {
        return new TheoryData<string, List<double>>
        {
            { "(2 + 3) * 4;", [20] },
            { "2 + (3 * 4);", [14] },
            { "((2 + 3) * (4 - 1));", [15] },
        };
    }

    [Theory]
    [MemberData(nameof(GetBuiltInFunctionTheory))]
    public void Can_parse_builtin_function_expression(string code, List<double> expected)
    {
        RunBaseTest(code, expected);
    }

    public static TheoryData<string, List<double>> GetBuiltInFunctionTheory()
    {
        return new TheoryData<string, List<double>>
        {
            { "МОДУЛЬ(-5);", [5] },
            { "СТЕПЕНЬ(2, 3);", [8] },
            { "МИНИМУМ(3, 1, 2);", [1] },
            { "МОДУЛЬ(МИНИМУМ(-1, 5));", [1] },
        };
    }

    [Theory]
    [MemberData(nameof(GetExceptionThrowTheory))]
    public void Cannot_parse_expression_with_exception_throw(string code, Type expectedExceptionType)
    {
        Context context = new();
        FakeEnvironment environment = new();

        Parser parser = new(code);
        AstEvaluator evaluator = new(context, environment);

        Assert.Throws(expectedExceptionType, () =>
        {
            List<AstNode> nodes = parser.ParseProgram();
            foreach (AstNode node in nodes)
            {
                evaluator.Evaluate(node);
            }
        });
    }

    public static TheoryData<string, Type> GetExceptionThrowTheory()
    {
        return new TheoryData<string, Type>
        {
            { "(2 + 3;", typeof(UnexpectedLexemeException) },
            { "2 + 3);", typeof(UnexpectedLexemeException) },
            { "5 / 0;", typeof(DivideByZeroException) },
        };
    }

    private void RunBaseTest(string code, List<double> expected)
    {
        FakeEnvironment environment = new();

        Parser parser = new(code);
        Context context = new();
        AstEvaluator evaluator = new(context, environment);

        List<double> results = new();
        List<AstNode> nodes = parser.ParseProgram();

        foreach (AstNode node in nodes)
        {
            Value val = evaluator.Evaluate(node);
            results.Add(ValueToDouble(val));
        }

        MatchResults(expected, results);
    }

    private double ValueToDouble(Value val)
    {
        return val.GetValueType() switch
        {
            Runtime.ValueType.ЦИФЕРКА => val.AsInt(),
            Runtime.ValueType.ПОЛТОРАШКА => val.AsDouble(),
            Runtime.ValueType.РАСКЛАД => val.AsBool() ? 1.0 : 0.0,
            Runtime.ValueType.ЦИТАТА => val.AsString().Length,
            _ => 0.0
        };
    }

    private void MatchResults(List<double> expected, List<double> results)
    {
        Assert.Equal(expected.Count, results.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i], results[i]);
        }
    }
}