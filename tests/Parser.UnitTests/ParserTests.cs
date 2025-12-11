using Execution;

using Xunit;

namespace Parser.UnitTests;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(LiteralsData))]
    public void TestLiterals(string code, decimal[] expected)
    {
        RunBaseTest(code, expected);
    }

    [Theory]
    [MemberData(nameof(ArithmeticOperationsData))]
    public void TestArithmeticOperations(string code, decimal[] expected)
    {
        RunBaseTest(code, expected);
    }

    [Theory]
    [MemberData(nameof(ComparisionOperationsData))]
    public void TestComparisionOperations(string code, decimal[] expected)
    {
        RunBaseTest(code, expected);
    }

    [Theory]
    [MemberData(nameof(LogicalOperationsData))]
    public void TestLogicalOperations(string code, decimal[] expected)
    {
        RunBaseTest(code, expected);
    }

    [Theory]
    [MemberData(nameof(UnaryOperationsData))]
    public void TestUnaryOperations(string code, decimal[] expected)
    {
        RunBaseTest(code, expected);
    }

    [Theory]
    [MemberData(nameof(AssociativeOperationsData))]
    public void TestAssociativeOperations(string code, decimal[] expected)
    {
        RunBaseTest(code, expected);
    }

    [Theory]
    [MemberData(nameof(VariableAssignmentsData))]
    public void TestVariableAssignments(string code, decimal[] expected)
    {
        RunBaseTest(code, expected);
    }

    [Theory]
    [MemberData(nameof(BuiltinFunctionsData))]
    public void TestBuiltInFunctions(string code, decimal[] expected)
    {
        RunBaseTest(code, expected);
    }

    [Theory]
    [MemberData(nameof(ContolSequencesData))]
    public void TestControlSequences(string code, decimal[] expected)
    {
        RunBaseTest(code, expected);
    }

    [Theory]
    [MemberData(nameof(ComplexExpressionsData))]
    public void TestComplexExpressions(string code, decimal[] expected)
    {
        RunBaseTest(code, expected);
    }

    [Theory]
    [MemberData(nameof(ErrorCasesData))]
    public void TestErrorCases(string code, Type expectedExceptionType)
    {
        FakeEnvironment environment = new FakeEnvironment();
        Parser parser = new Parser(environment, code);

        Assert.Throws(expectedExceptionType, () => parser.ParseProgram());
    }

    public static TheoryData<string, decimal[]> LiteralsData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "42;", [42] },
            { "-42;", [-42] },
            { "3.14;", [3.14m] },
            { "-3.14;", [-3.14m] },
            { "+5;", [5] },
            { "0;", [0] },
            { "ХАЙП;", [1] },
            { "КРИНЖ;", [0] },
            { "ПИ;", [3.1415926535m] },
            { "ЕШКА;", [2.7182818284m] },
            { "\"Hello\";", [5] },
            { "\"\";", [0] },
            { "\"Привет мир!\";", [11] },
        };
    }

    public static TheoryData<string, decimal[]> ArithmeticOperationsData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "2 + 3;", [5] },
            { "5 - 2;", [3] },
            { "3 * 4;", [12] },
            { "10 / 2;", [5] },
            { "7 % 3;", [1] },
            { "2 + 3 * 4;", [14] },
            { "(2 + 3) * 4;", [20] },
            { "10 - 4 / 2;", [8] },
            { "2 + 3; 5 - 1;", [5, 4] },
        };
    }

    public static TheoryData<string, decimal[]> ComparisionOperationsData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "5 == 5;", [1] },
            { "5 == 3;", [0] },
            { "5 != 3;", [1] },
            { "5 != 5;", [0] },
            { "3 < 5;", [1] },
            { "5 < 3;", [0] },
            { "5 <= 5;", [1] },
            { "3 <= 5;", [1] },
            { "5 > 3;", [1] },
            { "3 > 5;", [0] },
            { "5 >= 5;", [1] },
            { "5 >= 3;", [1] },
        };
    }

    public static TheoryData<string, decimal[]> LogicalOperationsData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "ХАЙП И ХАЙП;", [1] },
            { "ХАЙП И КРИНЖ;", [0] },
            { "КРИНЖ И ХАЙП;", [0] },
            { "КРИНЖ И КРИНЖ;", [0] },
            { "ХАЙП ИЛИ ХАЙП;", [1] },
            { "ХАЙП ИЛИ КРИНЖ;", [1] },
            { "КРИНЖ ИЛИ ХАЙП;", [1] },
            { "КРИНЖ ИЛИ КРИНЖ;", [0] },
            { "НЕ ХАЙП;", [0] },
            { "НЕ КРИНЖ;", [1] },
            { "НЕ НЕ ХАЙП;", [1] },
            { "2 < 3 И 4 > 1;", [1] },
            { "2 < 3 И 4 < 1;", [0] },
            { "2 > 3 ИЛИ 4 > 1;", [1] },
            { "2 > 3 ИЛИ 4 < 1;", [0] },
        };
    }

    public static TheoryData<string, decimal[]> UnaryOperationsData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "+5;", [5] },
            { "-3;", [-3] },
            { "-3.14;", [-3.14m] },
            { "- + -5;", [5] },
            { "-2 * 3;", [-6] },
            { "-(2 + 3);", [-5] },
        };
    }

    public static TheoryData<string, decimal[]> AssociativeOperationsData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "10 - 5 - 2;", [3] },
            { "12 / 3 / 2;", [2] },
            { "- + -5;", [5] },
            { "НЕ НЕ ХАЙП;", [1] },
        };
    }

    public static TheoryData<string, decimal[]> BuiltinFunctionsData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "МОДУЛЬ(-5);", [5] },
            { "МОДУЛЬ(5);", [5] },
            { "МОДУЛЬ(0);", [0] },
            { "КОРЕНЬ(16);", [4] },
            { "СТЕПЕНЬ(2, 3);", [8] },
            { "СТЕПЕНЬ(3, 2);", [9] },
            { "МИНИМУМ(3, 1, 2);", [1] },
            { "МИНИМУМ(-5, 0, 5);", [-5] },
            { "МАКСИМУМ(3, 1, 2);", [3] },
            { "МАКСИМУМ(-5, 0, 5);", [5] },
            { "СИНУС(0);", [0] },
            { "КОСИНУС(0);", [1] },
            { "ТАНГЕНС(0);", [0] },
        };
    }

    public static TheoryData<string, decimal[]> VariableAssignmentsData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "ЦИФЕРКА x = 5;", [5] },
            { "ПОЛТОРАШКА y = 3.14;", [3.14m] },
            { "ЦИФЕРКА a = 10; ЦИФЕРКА b = 20; ЦИФЕРКА c = a + b;", [10, 20, 30] },
            { "ЦИФЕРКА x = 5; x = x + 1;", [5, 6] },
            { "БАЗА ЦИФЕРКА МАКС = 100; ЦИФЕРКА результат = МАКС;", [100, 100] },
            { "ЦИФЕРКА радиус = 5; ПОЛТОРАШКА результат = радиус * радиус * ПИ;", [5, 78.5398163375m] },
        };
    }

    public static TheoryData<string, decimal[], decimal[]> OutputData()
    {
        return new TheoryData<string, decimal[], decimal[]>
        {
            { "ВЫБРОС(\"Привет\");", [], [6] },
            { "ВЫБРОС(1, 2, 3);", [], [1, 2, 3] },
        };
    }

    public static TheoryData<string, decimal[]> ContolSequencesData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "ЕСЛИ (ХАЙП) ТО 5;", [5] },
            { "ЕСЛИ (КРИНЖ) ТО 5;", [] },
            { "ЕСЛИ (5 > 3) ТО 10;", [10] },
            { "ЕСЛИ (5 < 3) ТО 10;", [] },
            { "ЕСЛИ (ХАЙП) ТО 10; ИНАЧЕ 20;", [10] },
            { "ЕСЛИ (КРИНЖ) ТО 10; ИНАЧЕ 20;", [20] },
            { "ПОЕХАЛИ 1; 2; 3; ФИНАЛОЧКА", [1, 2, 3] },
            { "ЦИФЕРКА x = 0; ПОЕХАЛИ x = 5; ФИНАЛОЧКА x = 6;", [0, 5, 6] },
        };
    }

    public static TheoryData<string, Type> ErrorCasesData()
    {
        return new TheoryData<string, Type>
        {
            { "(2 + 3", typeof(UnexpectedLexemeException) },
            { "2 + 3)", typeof(UnexpectedLexemeException) },
            { "2 & 3;", typeof(UnexpectedLexemeException) },
            { "5 / 0;", typeof(DivideByZeroException) },
            { "10 % 0;", typeof(DivideByZeroException) },
            { "x = 5;", typeof(Exception) },
            { "МОДУЛЬ();", typeof(ArgumentException) },
            { "МОДУЛЬ(1, 2, 3);", typeof(ArgumentException) },
        };
    }

    public static TheoryData<string, decimal[]> ComplexExpressionsData()
    {
        return new TheoryData<string, decimal[]>
        {
            { "МОДУЛЬ(МИНИМУМ(-10, 5, 20));", [10] },
            { "СТЕПЕНЬ(КОРЕНЬ(16), 2);", [16] },
            { "ЦИФЕРКА x = 5; ЦИФЕРКА y = 10; (x + y) * МОДУЛЬ(-2);", [5, 10, 30] },
            { "ЕСЛИ (5 > 3 И 10 < 20) ТО 100; ИНАЧЕ 200;", [100] },
            { "ЕСЛИ (5 > 10 ИЛИ 10 < 20) ТО 100; ИНАЧЕ 200;", [100] },
        };
    }

    private void RunBaseTest(string code, decimal[] expected)
    {
        FakeEnvironment environment = RunParser(code);
        IReadOnlyList<decimal> results = environment.GetResults();

        Assert.Equal(expected.Length, results.Count);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], results[i]);
        }
    }

    private FakeEnvironment RunParser(string code, params decimal[] inputs)
    {
        FakeEnvironment env = new FakeEnvironment(inputs);
        Parser parser = new(env, code);
        parser.ParseProgram();
        return env;
    }
}
