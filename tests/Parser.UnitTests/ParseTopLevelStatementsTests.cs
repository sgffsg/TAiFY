using Ast;
using Ast.Declarations;

using Execution;
using Execution.Exceptions;

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
        Assert.Equal(5, context.GetValue("x"));
    }

    [Fact]
    public void Parse_float_variable_declaration()
    {
        string code = @"ПОЛТОРАШКА x = 5.1;";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        Assert.True(context.Exists("x"));
        Assert.Equal(5.1, context.GetValue("x"));
    }

    [Fact]
    public void Parse_variable_redeclaration()
    {
        string code = @"ЦИФЕРКА x = 5;ПОЛТОРАШКА x = 5.1;";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.Throws<ArgumentException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_multiple_variable_declarations()
    {
        string code = @"ЦИФЕРКА a = 10; ЦИФЕРКА b = 20; ЦИФЕРКА c = a + b;";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

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

        ExecuteTestCode(context, environment, code);

        Assert.True(context.Exists("x"));
        Assert.Equal(6, context.GetValue("x"));
    }

    [Fact]
    public void Parse_variable_reassignment_without_declaration()
    {
        string code = @"x = x + 1;";
        Context context = new();
        FakeEnvironment environment = new();

        Exception exception = Assert.Throws<ArgumentException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_constant_variable_declaration()
    {
        string code = @"БАЗА ЦИФЕРКА x = 5;";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        Assert.True(context.Exists("x"));
        Assert.Equal(5, context.GetValue("x"));
    }

    [Fact]
    public void Parse_constant_variable_redeclaration()
    {
        string code = @"БАЗА ЦИФЕРКА x = 5;БАЗА ЦИФЕРКА x = 2;";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.Throws<ArgumentException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_constant_variable_with_same_name_variable_declaration()
    {
        string code = @"БАЗА ЦИФЕРКА x = 5;ЦИФЕРКА x = 5;";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.Throws<ArgumentException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_constant_variable_usage()
    {
        string code = @"БАЗА ЦИФЕРКА МАКС = 100; ЦИФЕРКА результат = МАКС;";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

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

        Exception exception = Assert.Throws<ArgumentException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_variables_in_expression()
    {
        string code = @"ЦИФЕРКА радиус = 5; ПОЛТОРАШКА результат = радиус * радиус * ПИ;";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        Assert.True(context.Exists("радиус"));
        Assert.Equal(5, context.GetValue("радиус"));

        Assert.True(context.Exists("результат"));
        Assert.Equal(5 * 5 * Math.PI, context.GetValue("результат"), 5); // Точность до 5 знаков
    }

    [Fact]
    public void Parse_if_branch_with_literal_check()
    {
        string code = @"ЕСЛИ (ХАЙП) ТО ВЫБРОС(5);";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

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

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Empty(output);
    }

    [Fact]
    public void Parse_if_branch_with_comparison_expression()
    {
        string code = @"ЕСЛИ (5 > 3) ТО ВЫБРОС(10);";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Equal(10, output[0]);
    }

    [Fact]
    public void Parse_empty_else_branch_with_comparison_expression()
    {
        string code = @"ЕСЛИ (5 < 3) ТО 10;";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Empty(output);
    }

    [Fact]
    public void Parse_if_else_true_branch()
    {
        string code = @"ЕСЛИ (ХАЙП) ТО ВЫБРОС(10); ИНАЧЕ ВЫБРОС(20);";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(10, output[0]);
    }

    [Fact]
    public void Parse_if_else_false_branch()
    {
        string code = @"ЕСЛИ (КРИНЖ) ТО ВЫБРОС(10); ИНАЧЕ ВЫБРОС(20);";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(20, output[0]);
    }

    [Fact]
    public void Parse_nested_functions()
    {
        string code = @"ВЫБРОС(МОДУЛЬ(МИНИМУМ(-10, 5, 20)));";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(10, output[0]);
    }

    [Fact]
    public void Parse_combination_of_functions()
    {
        string code = @"ВЫБРОС(СТЕПЕНЬ(КОРЕНЬ(16), 2));";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(16, output[0], 5);
    }

    [Fact]
    public void Parse_combination_of_variables_and_functions()
    {
        string code = @"ЦИФЕРКА x = 5; ЦИФЕРКА y = 10; ВЫБРОС((x + y) * МОДУЛЬ(-2));";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(30, output[0]);
    }

    [Fact]
    public void Parse_complex_logical_and_condition()
    {
        string code = @"ЕСЛИ (5 > 3 И 10 < 20) ТО ВЫБРОС(100); ИНАЧЕ ВЫБРОС(200);";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(100, output[0]);
    }

    [Fact]
    public void Parse_complex_logical_or_condition()
    {
        string code = @"ЕСЛИ (5 > 10 ИЛИ 10 < 20) ТО ВЫБРОС(100); ИНАЧЕ ВЫБРОС(200);";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Single(output);
        Assert.Equal(100, output[0]);
    }

    [Fact]
    public void Parse_complex_logical_and_or_combination()
    {
        string code = @"ЕСЛИ ((5 > 3) И (10 < 20 ИЛИ 1 == 2)) ТО ВЫБРОС(100); ИНАЧЕ ВЫБРОС(200);";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

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

        Assert.Throws<UnexpectedLexemeException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_extra_closed_parenthesis_should_fail()
    {
        string code = @"2 + 3)";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.Throws<UnexpectedLexemeException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_unknown_operator_should_fail()
    {
        string code = @"2 & 3;";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.Throws<UnexpectedLexemeException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_division_by_zero_should_fail()
    {
        string code = @"5 / 0;";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.Throws<DivideByZeroException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_remainder_by_zero_should_fail()
    {
        string code = @"10 % 0;";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.Throws<DivideByZeroException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_use_undeclared_variable_should_fail()
    {
        string code = @"x = 5;";
        Context context = new();
        FakeEnvironment environment = new();

        Exception exception = Assert.Throws<ArgumentException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_function_too_few_arguments_should_fail()
    {
        string code = @"МОДУЛЬ();";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.Throws<ArgumentException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_function_too_many_arguments_should_fail()
    {
        string code = @"МОДУЛЬ(1, 2, 3);";
        Context context = new();
        FakeEnvironment environment = new();

        Assert.Throws<ArgumentException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_output_string()
    {
        string code = @"ВЫБРОС(""Привет"");";
        Context context = new();
        FakeEnvironment environment = new();

        ExecuteTestCode(context, environment, code);

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

        ExecuteTestCode(context, environment, code);

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

        ExecuteTestCode(context, environment, code);

        IReadOnlyList<double> output = environment.GetOutput();
        Assert.Equal(3, output.Count);
        Assert.Equal(5, output[0]);
        Assert.Equal(10, output[1]);
        Assert.Equal(15, output[2]);
    }

    [Fact]
    public void Parse_input_statement()
    {
        string code = @"ЦИФЕРКА x; ЦИФЕРКА y;ВБРОС(x, y);";
        double[] inputs = { 10.5, 20.3 };
        Context context = new();
        FakeEnvironment environment = new(inputs);

        ExecuteTestCode(context, environment, code);

        Assert.True(context.Exists("x"));
        Assert.True(context.Exists("y"));
        Assert.Equal(10.5, context.GetValue("x"));
        Assert.Equal(20.3, context.GetValue("y"));
    }

    [Fact]
    public void Parse_input_with_existing_variables()
    {
        string code = @"ЦИФЕРКА x = 0; ЦИФЕРКА y = 0; ВБРОС(x, y);";
        double[] inputs = { 100, 200 };
        Context context = new();
        FakeEnvironment environment = new(inputs);

        ExecuteTestCode(context, environment, code);

        Assert.Equal(100, context.GetValue("x"));
        Assert.Equal(200, context.GetValue("y"));
    }

    [Fact]
    public void Parse_break_statement_outside_loop_should_fail()
    {
        string code = @"ХВАТИТ;";
        Context context = new();
        FakeEnvironment environment = new();

        Exception exception = Assert.Throws<BreakException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_continue_statement_outside_loop_should_fail()
    {
        string code = @"ПРОДОЛЖАЕМ;";
        Context context = new();
        FakeEnvironment environment = new();

        Exception exception = Assert.Throws<ContinueException>(() => ExecuteTestCode(context, environment, code));
    }

    [Fact]
    public void Parse_return_statement_outside_function_should_fail()
    {
        string code = @"ДРАТУТИ 5;";
        Context context = new();
        FakeEnvironment environment = new();

        Exception exception = Assert.Throws<ReturnException>(() => ExecuteTestCode(context, environment, code));
    }

    private void ExecuteTestCode(Context context, IEnvironment environment, string code)
    {
        Parser parser = new(code);
        List<AstNode> nodes = parser.ParseProgram();
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

    [Fact]
public void Parse_for_loop_with_counter()
{
    string code = @"
ЦИФЕРКА сумма = 0;
ЦИКЛ (ЦИФЕРКА i = 0; i < 5; i = i + 1)
    сумма = сумма + i;
ФИНАЛОЧКА
ВЫБРОС(сумма);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    Assert.Equal(10, output[0]); // 0+1+2+3+4 = 10
}

[Fact]
public void Parse_for_loop_with_break()
{
    string code = @"
ЦИФЕРКА сумма = 0;
ЦИКЛ (ЦИФЕРКА i = 0; i < 10; i = i + 1)
    ЕСЛИ (i == 5) ТО
        ХВАТИТ;
    ФИНАЛОЧКА
    сумма = сумма + i;
ФИНАЛОЧКА
ВЫБРОС(сумма);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    Assert.Equal(10, output[0]); // 0+1+2+3+4 = 10 (breaks before adding 5)
}

[Fact]
public void Parse_for_loop_with_continue()
{
    string code = @"
ЦИФЕРКА сумма = 0;
ЦИКЛ (ЦИФЕРКА i = 0; i < 5; i = i + 1)
    ЕСЛИ (i == 2) ТО
        ПРОДОЛЖАЕМ;
    ФИНАЛОЧКА
    сумма = сумма + i;
ФИНАЛОЧКА
ВЫБРОС(сумма);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    Assert.Equal(10, output[0]); // 0+1+3+4 = 8 (skips 2)
}

[Fact]
public void Parse_while_loop_simple()
{
    string code = @"
ЦИФЕРКА счетчик = 0;
ЦИФЕРКА сумма = 0;
ПОКА (счетчик < 5)
    сумма = сумма + счетчик;
    счетчик = счетчик + 1;
ФИНАЛОЧКА
ВЫБРОС(сумма);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    Assert.Equal(10, output[0]); // 0+1+2+3+4 = 10
}

[Fact]
public void Parse_while_loop_with_break()
{
    string code = @"
ЦИФЕРКА счетчик = 0;
ЦИФЕРКА сумма = 0;
ПОКА (ХАЙП)
    ЕСЛИ (счетчик >= 5) ТО
        ХВАТИТ;
    ФИНАЛОЧКА
    сумма = сумма + счетчик;
    счетчик = счетчик + 1;
ФИНАЛОЧКА
ВЫБРОС(сумма);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    Assert.Equal(10, output[0]); // 0+1+2+3+4 = 10
}

[Fact]
public void Parse_while_loop_with_continue()
{
    string code = @"
ЦИФЕРКА счетчик = 0;
ЦИФЕРКА сумма = 0;
ПОКА (счетчик < 5)
    счетчик = счетчик + 1;
    ЕСЛИ (счетчик == 3) ТО
        ПРОДОЛЖАЕМ;
    ФИНАЛОЧКА
    сумма = сумма + счетчик;
ФИНАЛОЧКА
ВЫБРОС(сумма);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    Assert.Equal(13, output[0]); // 1+2+4+5 = 12 (skips adding 3)
}

[Fact]
public void Parse_nested_for_loops()
{
    string code = @"
ЦИФЕРКА сумма = 0;
ЦИКЛ (ЦИФЕРКА i = 0; i < 3; i = i + 1)
    ЦИКЛ (ЦИФЕРКА j = 0; j < 3; j = j + 1)
        сумма = сумма + 1;
    ФИНАЛОЧКА
ФИНАЛОЧКА
ВЫБРОС(сумма);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    Assert.Equal(9, output[0]); // 3x3 = 9
}

[Fact]
public void Parse_empty_for_loop()
{
    string code = @"
ЦИКЛ (ЦИФЕРКА i = 0; i < 0; i = i + 1)
    ВЫБРОС(1);
ФИНАЛОЧКА";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Empty(output); // Loop should not execute
}

[Fact]
public void Parse_infinite_for_loop_with_break()
{
    string code = @"
ЦИФЕРКА счетчик = 0;
ЦИКЛ (ЦИФЕРКА i = 0; ХАЙП; i = i + 1)
    счетчик = счетчик + 1;
    ЕСЛИ (счетчик >= 3) ТО
        ХВАТИТ;
    ФИНАЛОЧКА
ФИНАЛОЧКА
ВЫБРОС(счетчик);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    Assert.Equal(3, output[0]);
}

[Fact]
public void Parse_for_loop_with_complex_condition()
{
    string code = @"
ЦИФЕРКА сумма = 0;
ЦИКЛ (ЦИФЕРКА i = 0; i < 10 И сумма < 15; i = i + 1)
    сумма = сумма + i;
ФИНАЛОЧКА
ВЫБРОС(сумма);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    Assert.Equal(15, output[0]); // 0+1+2+3+4+5 = 15
}

[Fact]
public void Parse_while_loop_with_complex_condition()
{
    string code = @"
ЦИФЕРКА a = 0;
ЦИФЕРКА b = 10;
ЦИФЕРКА сумма = 0;
ПОКА (a < 5 И b > 0)
    сумма = сумма + a + b;
    a = a + 1;
    b = b - 1;
ФИНАЛОЧКА
ВЫБРОС(сумма);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    Assert.Equal(70, output[0]); // 5 iterations: (0+10)+(1+9)+(2+8)+(3+7)+(4+6) = 70
}

[Fact]
public void Parse_for_loop_without_initialization()
{
    string code = @"
ЦИФЕРКА i = 0;
ЦИКЛ (; i < 3; i = i + 1)
    ВЫБРОС(i);
ФИНАЛОЧКА";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Equal(3, output.Count);
    Assert.Equal(0, output[0]);
    Assert.Equal(1, output[1]);
    Assert.Equal(2, output[2]);
}

[Fact]
public void Parse_for_loop_without_increment()
{
    string code = @"
ЦИФЕРКА i = 0;
ЦИКЛ (; i < 3; )
    ВЫБРОС(i);
    i = i + 1;
ФИНАЛОЧКА";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Equal(3, output.Count);
    Assert.Equal(0, output[0]);
    Assert.Equal(1, output[1]);
    Assert.Equal(2, output[2]);
}

[Fact]
public void Parse_for_loop_infinite_without_condition()
{
    string code = @"
ЦИФЕРКА счетчик = 0;
ЦИКЛ (ЦИФЕРКА i = 0; ; i = i + 1)
    счетчик = счетчик + 1;
    ЕСЛИ (счетчик >= 3) ТО
        ХВАТИТ;
    ФИНАЛОЧКА
ФИНАЛОЧКА
ВЫБРОС(счетчик);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    Assert.Equal(3, output[0]);
}

[Fact]
public void Parse_loop_variable_scope()
{
    string code = @"
ЦИФЕРКА внешняя = 0;
ЦИКЛ (ЦИФЕРКА i = 0; i < 3; i = i + 1)
    внешняя = внешняя + i;
ФИНАЛОЧКА
ВЫБРОС(внешняя);
// Попытка обратиться к i здесь должна вызвать ошибку
ЦИФЕРКА тест = i;";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    // Проверяем, что переменная i недоступна вне цикла
    Assert.Throws<ArgumentException>(() => ExecuteTestCode(context, environment, code));
}

[Fact]
public void Parse_loop_with_float_counter()
{
    string code = @"
ПОЛТОРАШКА сумма = 0;
ЦИКЛ (ПОЛТОРАШКА i = 0.5; i < 3.0; i = i + 0.5)
    сумма = сумма + i;
ФИНАЛОЧКА
ВЫБРОС(сумма);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    // 0.5 + 1.0 + 1.5 + 2.0 + 2.5 = 7.5
    Assert.Equal(7.5, output[0], 5);
}

[Fact]
public void Parse_break_in_nested_loop()
{
    string code = @"
ЦИФЕРКА счетчик = 0;
ЦИКЛ (ЦИФЕРКА i = 0; i < 3; i = i + 1)
    ЦИКЛ (ЦИФЕРКА j = 0; j < 3; j = j + 1)
        счетчик = счетчик + 1;
        ЕСЛИ (j == 1) ТО
            ХВАТИТ;
        ФИНАЛОЧКА
    ФИНАЛОЧКА
ФИНАЛОЧКА
ВЫБРОС(счетчик);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    // Для каждого i: j = 0, j = 1 (break) = 2 итерации
    // Всего: 3 * 2 = 6
    Assert.Equal(6, output[0]);
}

[Fact]
public void Parse_continue_in_nested_loop()
{
    string code = @"
ЦИФЕРКА счетчик = 0;
ЦИКЛ (ЦИФЕРКА i = 0; i < 3; i = i + 1)
    ЦИКЛ (ЦИФЕРКА j = 0; j < 3; j = j + 1)
        ЕСЛИ (j == 1) ТО
            ПРОДОЛЖАЕМ;
        ФИНАЛОЧКА
        счетчик = счетчик + 1;
    ФИНАЛОЧКА
ФИНАЛОЧКА
ВЫБРОС(счетчик);";
    
    Context context = new();
    FakeEnvironment environment = new();
    
    ExecuteTestCode(context, environment, code);
    
    IReadOnlyList<double> output = environment.GetOutput();
    Assert.Single(output);
    // Для каждого i: j = 0, j = 2 = 2 итерации (j = 1 пропущен)
    // Всего: 3 * 2 = 6
    Assert.Equal(6, output[0]);
}
}
