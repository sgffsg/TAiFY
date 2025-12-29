using Ast;
using Ast.Declarations;
using Execution;
using Runtime;
using Semantics;
using Semantics.Exceptions;

namespace Parser.UnitTests;

public class ParseTopLevelStatementsTests
{
    [Fact]
    public void Parse_int_variable_declaration()
    {
        string code = @"ЦИФЕРКА x = 5;";
        Context context = new();
        FakeEnvironment environment = new();
        ExecuteTestCode(context, environment, code);

        Assert.True(context.Exists("x"));
        Assert.Equal(5, context.GetValue("x").AsInt());
    }

    [Fact]
    public void Parse_float_variable_declaration()
    {
        string code = @"ПОЛТОРАШКА x = 5.1;";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        Assert.True(context.Exists("x"));
        Assert.Equal(5.1, context.GetValue("x").AsDouble());
    }

    [Fact]
    public void Parse_variable_redeclaration()
    {
        string code = @"ЦИФЕРКА x = 5;ПОЛТОРАШКА x = 5.1;";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.ThrowsAny<Exception>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_multiple_variable_declarations()
    {
        string code = @"ЦИФЕРКА a = 10; ЦИФЕРКА b = 20; ЦИФЕРКА c = a + b;";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        Assert.Equal(10, context.GetValue("a").AsInt());
        Assert.Equal(20, context.GetValue("b").AsInt());
        Assert.Equal(30, context.GetValue("c").AsInt());
    }

    [Fact]
    public void Parse_variable_reassignment()
    {
        string code = @"ЦИФЕРКА x = 5;x = x + 1;";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        Assert.Equal(6, context.GetValue("x").AsInt());
    }

    [Fact]
    public void Parse_variable_reassignment_without_declaration()
    {
        string code = @"x = x + 1;";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.ThrowsAny<Exception>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_constant_variable_declaration()
    {
        string code = @"БАЗА ЦИФЕРКА x = 5;";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        Assert.Equal(5, context.GetValue("x").AsInt());
    }

    [Fact]
    public void Parse_constant_reassignment_failed()
    {
        string code = @"БАЗА ЦИФЕРКА МАКС = 100; МАКС = 50;";
        Context context = new();
        FakeEnvironment environment = new();
        Assert.ThrowsAny<Exception>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_variables_in_expression()
    {
        string code = @"ПОЛТОРАШКА радиус = 5.0; ПОЛТОРАШКА результат = радиус * радиус;";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        double expected = 5 * 5;
        Assert.Equal(expected, context.GetValue("результат").AsDouble(), 5);
    }

    [Fact]
    public void Parse_if_branch_with_literal_check()
    {
        string code = @"ЕСЛИ (ХАЙП) ТО ВЫБРОС(5);";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<string> output = environment.GetOutputHistory();
        Assert.Single(output);
        Assert.Equal("5", output[0]);
    }

    [Fact]
    public void Parse_if_else_false_branch()
    {
        string code = @"ЕСЛИ (КРИНЖ) ТО ВЫБРОС(10); ИНАЧЕ ВЫБРОС(20);";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<string> output = environment.GetOutputHistory();
        Assert.Equal("20", output[0]);
    }

    [Fact]
    public void Parse_nested_functions()
    {
        string code = @"ВЫБРОС(МОДУЛЬ(МИНИМУМ(-10.0, 5.0)));";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        Assert.Equal("10", environment.GetOutputHistory()[0]);
    }

    [Fact]
    public void Parse_output_multiple_values()
    {
        string code = @"ВЫБРОС(1, 2, 3);";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<string> output = environment.GetOutputHistory();
        Assert.Equal("123", output[0]);
    }

    [Fact]
    public void Parse_input_statement()
    {
        string code = @"ЦИФЕРКА x = 0; ЦИФЕРКА y = 0; ВБРОС(x, y);";

        Context context = new();
        FakeEnvironment environment = new();
        environment.EnqueueInput(new Value(10));
        environment.EnqueueInput(new Value(20));

        ExecuteTestCode(context, environment, code);

        Assert.Equal(10, context.GetValue("x").AsInt());
        Assert.Equal(20, context.GetValue("y").AsInt());
    }

    [Fact]
    public void Parse_return_statement_outside_function_should_fail()
    {
        string code = @"ДРАТУТИ 5;";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.Throws<InvalidExpressionException>(() => ExecuteTestCode(context, environment, code));
    }

    private void ExecuteTestCode(Context context, IEnvironment environment, string code)
    {
        Parser parser = new(code);
        List<AstNode> nodes = parser.ParseProgram();

        SemanticsChecker checker = new(
            Builtins.Functions,
            Builtins.Constants,
            Builtins.Types
        );
        checker.Check(nodes);

        AstEvaluator evaluator = new(context, environment);
        foreach (Declaration node in nodes.OfType<Declaration>())
        {
            node.Accept(evaluator);
        }

        foreach (AstNode? node in nodes.Where(n => n is not Declaration))
        {
            evaluator.Evaluate(node);
        }
    }
}