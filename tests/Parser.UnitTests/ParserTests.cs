using Xunit;

namespace Parser.UnitTests;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(GetLiterals))]
    public void ParseLiteralsTest(string code, decimal[] expected)
    {
        Row row = Parser.ExecuteExpression(code);
        Assert.Equal(expected.Length, row.ColumnCount);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], row[i]);
        }
    }

    public static TheoryData<string, decimal[]> GetLiterals()
    {
        return new TheoryData<string, decimal[]>
        {
            { "42", [42] },
            { "-42", [-42] },
            { "3.14", [3.14m] },
            { "-3.14", [-3.14m] },
            { "+5", [5] },
            { "0", [0] },
            { "ХАЙП", [1] },
            { "КРИНЖ", [0] },
            { "ПИ", [3.1415926535m] },
            { "ЕШКА", [2.7182818284m] },
            { "\"Hello, World!\"", [13] },
            { "1, 2, 3", [1, 2, 3] },
            { "2 + 3, 5 - 1", [5, 4] },
        };
    }

    [Theory]
    [MemberData(nameof(GetArithmeticOperations))]
    public void ParseArithmeticOperationsTest(string code, decimal[] expected)
    {
        Row row = Parser.ExecuteExpression(code);
        Assert.Equal(expected.Length, row.ColumnCount);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], row[i]);
        }
    }

    public static TheoryData<string, decimal[]> GetArithmeticOperations()
    {
        return new TheoryData<string, decimal[]>
        {
            { "2 + 3", [5] },
            { "5 - 2", [3] },
            { "3 * 4", [12] },
            { "10 / 2", [5] },
            { "7 % 3", [1] },
            { "2 + 3 * 4", [14] },
        };
    }

    [Theory]
    [MemberData(nameof(GetLogicalOperations))]
    public void ParseLogicalOperationsTest(string code, decimal[] expected)
    {
        Row row = Parser.ExecuteExpression(code);
        Assert.Equal(expected.Length, row.ColumnCount);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], row[i]);
        }
    }

    public static TheoryData<string, decimal[]> GetLogicalOperations()
    {
        return new TheoryData<string, decimal[]>
        {
            { "ХАЙП И ХАЙП", [1] },
            { "ХАЙП И КРИНЖ", [0] },
            { "ХАЙП ИЛИ КРИНЖ", [1] },
            { "КРИНЖ ИЛИ КРИНЖ", [0] },
            { "НЕ ХАЙП", [0] },
            { "НЕ КРИНЖ", [1] },
        };
    }

    [Theory]
    [MemberData(nameof(GetComparisonOperations))]
    public void ParseComparisonOperationsTest(string code, decimal[] expected)
    {
        Row row = Parser.ExecuteExpression(code);
        Assert.Equal(expected.Length, row.ColumnCount);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], row[i]);
        }
    }

    public static TheoryData<string, decimal[]> GetComparisonOperations()
    {
        return new TheoryData<string, decimal[]>
        {
            { "5 == 5", [1] },
            { "5 == 3", [0] },
            { "5 != 3", [1] },
            { "5 != 5", [0] },
            { "3 < 5", [1] },
            { "5 <= 5", [1] },
            { "5 > 3", [1] },
            { "5 >= 5", [1] },
        };
    }

    [Theory]
    [MemberData(nameof(GetOperatorPrecedence))]
    public void ParseOperatorPrecedenceTest(string code, decimal[] expected)
    {
        Row row = Parser.ExecuteExpression(code);
        Assert.Equal(expected.Length, row.ColumnCount);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], row[i]);
        }
    }

    public static TheoryData<string, decimal[]> GetOperatorPrecedence()
    {
        return new TheoryData<string, decimal[]>
        {
            { "10 - 4 / 2", [8] },
            { "-3 + 5", [2] },
            { "2 < 3 И 4 > 1", [1] },
        };
    }

    [Theory]
    [MemberData(nameof(GetAssociativity))]
    public void ParseAssociativityTest(string code, decimal[] expected)
    {
        Row row = Parser.ExecuteExpression(code);
        Assert.Equal(expected.Length, row.ColumnCount);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], row[i]);
        }
    }

    public static TheoryData<string, decimal[]> GetAssociativity()
    {
        return new TheoryData<string, decimal[]>
        {
            { "10 - 5 - 2", [3] },
            { "12 / 3 / 2", [2] },
            { "+5", [5] },
            { "-3.14", [-3.14m] },
            { "НЕ НЕ ХАЙП", [1] },
            { "-5 + 3", [-2] },
            { "- + -5", [5] },
            { "-2 * 3", [-6] },
            { "-(2 + 3)", [-5] },
        };
    }

    // Группировка скобок
    [Theory]
    [MemberData(nameof(GetParenthesesGrouping))]
    public void ParseParenthesesGroupingTest(string code, decimal[] expected)
    {
        Row row = Parser.ExecuteExpression(code);
        Assert.Equal(expected.Length, row.ColumnCount);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], row[i]);
        }
    }

    public static TheoryData<string, decimal[]> GetParenthesesGrouping()
    {
        return new TheoryData<string, decimal[]>
        {
            { "(2 + 3) * 4", [20] },
            { "2 + (3 * 4)", [14] },
            { "((2 + 3) * (4 - 1))", [15] },
        };
    }

    [Theory]
    [MemberData(nameof(GetBuiltInFunctions))]
    public void ParseBuiltInFunctionsTest(string code, decimal[] expected)
    {
        Row row = Parser.ExecuteExpression(code);
        Assert.Equal(expected.Length, row.ColumnCount);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], row[i]);
        }
    }

    public static TheoryData<string, decimal[]> GetBuiltInFunctions()
    {
        return new TheoryData<string, decimal[]>
        {
            { "МОДУЛЬ(-5)", [5] },
            { "СТЕПЕНЬ(2, 3)", [8] },
            { "МИНИМУМ(3, 1, 2)", [1] },
            { "МОДУЛЬ(МИНИМУМ(-1, 5))", [1] },
        };
    }

    [Theory]
    [MemberData(nameof(GetErrorTheory))]
    public void ParseErrorTest(string code, Type expectedExceptionType)
    {
        Assert.Throws(expectedExceptionType, () => Parser.ExecuteExpression(code));
    }

    public static TheoryData<string, Type> GetErrorTheory()
    {
        return new TheoryData<string, Type>
        {
            { "(2 + 3", typeof(UnexpectedLexemeException) },
            { "2 + 3)", typeof(UnexpectedLexemeException) },
            { "2 & 3", typeof(UnexpectedLexemeException) },
            { "", typeof(UnexpectedLexemeException) },
            { "5 / 0", typeof(DivideByZeroException) },
            { "МОДУЛЬ(1, 2)", typeof(ArgumentException) },
            { "МОЯ_ФУНКЦИЯ()", typeof(NotImplementedException) },
            { "переменная", typeof(NotImplementedException) },
        };
    }
}
