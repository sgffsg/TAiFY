using Execution;

using Xunit;

namespace Parser.UnitTests;

public class ParseExpressionsTests
{
    [Theory]
    [MemberData(nameof(LiteralsData))]
    public void ParseSingleNumberLiteral(string code, List<double> expected)
    {
        RunBaseTest(code, expected);
    }

    public static TheoryData<string, List<double>> LiteralsData()
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

    private void RunBaseTest(string code, List<double> expected)
    {
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        List<double> result = parser.ExecuteExpressionToList();
        MatchResults(expected, result);
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
