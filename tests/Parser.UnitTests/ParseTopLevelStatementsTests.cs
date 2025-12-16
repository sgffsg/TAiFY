using Execution;

namespace Parser.UnitTests;

public class ParseTopLevelStatementsTests
{
    [Fact]
    public void Parse_variable_declaration()
    {
        string code = @"ЦИФЕРКА x = 5;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();
        Assert.True(context.Exists("x"));
        Assert.Equal(5, context.GetValue("x"));
    }

    private void RunBaseTest(string code, double[] inputs, double[] expected)
    {
        Context context = new();
        FakeEnvironment environment = new(inputs);
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> results = environment.GetOutput();
        MatchResults(expected, results);
    }

    private void MatchResults(double[] expected, IReadOnlyList<double> results)
    {
        Assert.Equal(expected.Length, results.Count);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], results[i]);
        }
    }
}
