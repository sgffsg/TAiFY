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

    [Fact]
    public void Parse_if_branch_with_literal_check()
    {
        string code = @"ЕСЛИ (ХАЙП) ТО 5;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(5, output[0]);
    }

    [Fact]
    public void Parse_empty_else_branch_with_literal_check()
    {
        string code = @"ЕСЛИ (КРИНЖ) ТО 5;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Empty(output);
    }

    [Fact]
    public void Parse_if_branch_with_comparison_expression()
    {
        string code = @"ЕСЛИ (5 > 3) ТО 10;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(10, output[0]);
    }

    [Fact]
    public void Parse_empty_else_branch_with_comparison_expression()
    {
        string code = @"ЕСЛИ (5 < 3) ТО 10;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Empty(output);
    }

    [Fact]
    public void Parse_if_else_true_branch()
    {
        string code = @"ЕСЛИ (ХАЙП) ТО 10; ИНАЧЕ 20;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(10, output[0]);
    }

    [Fact]
    public void Parse_if_else_false_branch()
    {
        string code = @"ЕСЛИ (КРИНЖ) ТО 10; ИНАЧЕ 20;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(20, output[0]);
    }

    [Fact]
    public void Parse_block_with_multiple_statements()
    {
        string code = @"ПОЕХАЛИ 1; 2; 3; ФИНАЛОЧКА";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Equal(3, output.Count);
        Assert.Equal(1, output[0]);
        Assert.Equal(2, output[1]);
        Assert.Equal(3, output[2]);
    }

    [Fact]
    public void Parse_nested_functions()
    {
        string code = @"МОДУЛЬ(МИНИМУМ(-10, 5, 20));";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(10, output[0]);
    }

    [Fact]
    public void Parse_combination_of_functions()
    {
        string code = @"СТЕПЕНЬ(КОРЕНЬ(16), 2);";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(16, output[0], 5);
    }

    [Fact]
    public void Parse_combination_of_variables_and_functions()
    {
        string code = @"ЦИФЕРКА x = 5; ЦИФЕРКА y = 10; (x + y) * МОДУЛЬ(-2);";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(30, output[0]);
    }

    [Fact]
    public void Parse_complex_logical_and_condition()
    {
        string code = @"ЕСЛИ (5 > 3 И 10 < 20) ТО 100; ИНАЧЕ 200;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(100, output[0]);
    }

    [Fact]
    public void Parse_complex_logical_or_condition()
    {
        string code = @"ЕСЛИ (5 > 10 ИЛИ 10 < 20) ТО 100; ИНАЧЕ 200;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(100, output[0]);
    }

    [Fact]
    public void Parse_complex_logical_and_or_combination()
    {
        string code = @"ЕСЛИ ((5 > 3) И (10 < 20 ИЛИ 1 == 2)) ТО 100; ИНАЧЕ 200;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(100, output[0]);
    }

    [Fact]
    public void Parse_unclosed_parenthesis_should_fail()
    {
        string code = @"(2 + 3";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Parse_extra_closed_parenthesis_should_fail()
    {
        string code = @"2 + 3)";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Parse_unknown_operator_should_fail()
    {
        string code = @"2 & 3;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Parse_division_by_zero_should_fail()
    {
        string code = @"5 / 0;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Assert.Throws<DivideByZeroException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Parse_remainder_by_zero_should_fail()
    {
        string code = @"10 % 0;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Assert.Throws<DivideByZeroException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Parse_use_undeclared_variable_should_fail()
    {
        string code = @"x = 5;";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Exception exception = Assert.Throws<Exception>(() => parser.ParseProgram());
        Assert.Contains("Необъявленная переменная", exception.Message);
    }

    [Fact]
    public void Parse_function_too_few_arguments_should_fail()
    {
        string code = @"МОДУЛЬ();";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Parse_function_too_many_arguments_should_fail()
    {
        string code = @"МОДУЛЬ(1, 2, 3);";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Parse_output_string()
    {
        string code = @"ВЫБРОС(""Привет"");";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(6, output[0]); // Длина строки "Привет" = 6 символов
    }

    [Fact]
    public void Parse_output_multiple_values()
    {
        string code = @"ВЫБРОС(1, 2, 3);";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Equal(3, output.Count);
        Assert.Equal(1, output[0]);
        Assert.Equal(2, output[1]);
        Assert.Equal(3, output[2]);
    }

    [Fact]
    public void Parse_output_with_expressions()
    {
        string code = @"ЦИФЕРКА x = 5; ВЫБРОС(x, x * 2, x + 10);";
        Context context = new();
        FakeEnvironment environment = new();
        Parser parser = new(context, environment, code);

        parser.ParseProgram();

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Equal(3, output.Count);
        Assert.Equal(5, output[0]);
        Assert.Equal(10, output[1]);
        Assert.Equal(15, output[2]);
    }

    [Fact]
    public void Parse_input_statement()
    {
        string code = @"ВБРОС(x, y);";
        double[] inputs = { 10.5, 20.3 };
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
