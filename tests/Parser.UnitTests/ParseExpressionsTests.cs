using Execution;

using Xunit;

namespace Parser.UnitTests;

public class ParseExpressionsTests
{
    private void RunBaseTest(string code, double[] expected)
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
