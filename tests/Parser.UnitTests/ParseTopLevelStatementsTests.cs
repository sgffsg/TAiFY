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
    public void Parse_multiple_variable_declarations()
    {
        string code = @"ЦИФЕРКА a = 10; ЦИФЕРКА b = 20; ЦИФЕРКА c = a + b;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        Assert.True(context.Exists("a"));
        Assert.Equal(10, context.GetValue("a"));

        Assert.True(context.Exists("b"));
        Assert.Equal(20, context.GetValue("b"));

        Assert.True(context.Exists("c"));
        Assert.Equal(30, context.GetValue("c"));
    }

    [Fact]
    public void Parse_variable_reassignment()
    {
        string code = @"ЦИФЕРКА x = 5;x = x + 1;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        Assert.True(context.Exists("x"));
        Assert.Equal(6, context.GetValue("x"));
    }

    [Fact]
    public void Parse_variable_reassignment_without_declaration()
    {
        string code = @"x = x + 1;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Exception exception = Assert.Throws<Exception>(() => parser.ParseProgram());
        Assert.Contains("Необъявленная переменная", exception.Message);
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

    [Fact]
    public void Parse_constant_variable_usage()
    {
        string code = @"БАЗА ЦИФЕРКА МАКС = 100; ЦИФЕРКА результат = МАКС;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        Assert.True(context.Exists("МАКС"));
        Assert.Equal(100, context.GetValue("МАКС"));

        Assert.True(context.Exists("результат"));
        Assert.Equal(100, context.GetValue("результат"));
    }

    [Fact]
    public void Parse_constant_reassignment_failed()
    {
        string code = @"БАЗА ЦИФЕРКА МАКС = 100; МАКС = 50;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Exception exception = Assert.Throws<InvalidOperationException>(() => parser.ParseProgram());
        Assert.Contains("Невозможно изменить значение константы", exception.Message);
    }

    [Fact]
    public void Parse_variables_in_expression()
    {
        string code = @"ЦИФЕРКА радиус = 5; ПОЛТОРАШКА результат = радиус * радиус * ПИ;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        Assert.True(context.Exists("радиус"));
        Assert.Equal(5, context.GetValue("радиус"));

        Assert.True(context.Exists("результат"));
        Assert.Equal(5 * 5 * Math.PI, context.GetValue("результат"), 5); // Точность до 5 знаков
    }
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
