using Execution;

namespace Parser.UnitTests;

public class ParseTopLevelStatementsTests
{
    [Fact]
    public void Parse_int_variable_declaration()
    {
        string code = @"ЦИФЕРКА x = 5;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();
        Assert.True(context.Exists("x"));
        Assert.Equal(5, context.GetValue("x"));
    }

    [Fact]
    public void Parse_float_variable_declaration()
    {
        string code = @"ПОЛТОРАШКА x = 5.1;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();
        Assert.True(context.Exists("x"));
        Assert.Equal(5.1, context.GetValue("x"));
    }

    [Fact]
    public void Parse_variable_redeclaration()
    {
        string code = @"ЦИФЕРКА x = 5;ПОЛТОРАШКА x = 5.1;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Parse_multiple_variable_declaration()
    {
        string code = @"ЦИФЕРКА x = 5;ПОЛТОРАШКА y = 5.1;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        Assert.True(context.Exists("x"));
        Assert.Equal(5, context.GetValue("x"));

        Assert.True(context.Exists("y"));
        Assert.Equal(5.1, context.GetValue("y"));
    }

    [Fact]
    public void Parse_constant_variable_declaration()
    {
        string code = @"БАЗА ЦИФЕРКА x = 5;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();
        Assert.True(context.Exists("x"));
        Assert.Equal(5, context.GetValue("x"));
    }

    [Fact]
    public void Parse_constant_variable_redeclaration()
    {
        string code = @"БАЗА ЦИФЕРКА x = 5;БАЗА ЦИФЕРКА x = 2;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Parse_constant_variable_with_same_name_variable_declaration()
    {
        string code = @"БАЗА ЦИФЕРКА x = 5;ЦИФЕРКА x = 5;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
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
