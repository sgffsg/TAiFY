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

        double[] result = parser.ParseExpression();
        MatchResults(expected, result);
    }

    private void MatchResults(double[] expected, double[] results)
    {
        Assert.Equal(expected.Length, results.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], results[i]);
        }
    }
}
