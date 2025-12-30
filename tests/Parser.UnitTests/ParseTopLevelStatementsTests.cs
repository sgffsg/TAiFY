using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Execution;
using Runtime;
using Semantics;
using Semantics.Exceptions;

namespace Parser.UnitTests;

public class ParseTopLevelStatementsTests
{
    [Theory]
    [MemberData(nameof(GetSemanticViolationsData))]
    public void Semantic_check_throws_exprexted_exception(string code, Type expectedException)
    {
        Context context = new();
        FakeEnvironment environment = new();
        Assert.Throws(expectedException, () => ExecuteTestCode(context, environment, code));
    }

    public static TheoryData<string, Type> GetSemanticViolationsData()
    {
        return new TheoryData<string, Type>
        {
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ() ПОЕХАЛИ
                    ПОЛТОРАШКА радиус = 0.0;
                    ПОЛТОРАШКА радиус = 1.0;
                ФИНАЛОЧКА
                """,
                typeof(DuplicateSymbolException)
            },
            {
                """
                БАЗА ПОЛТОРАШКА радиус = 3.0;

                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ
                    ПОЛТОРАШКА радиус = 5.0;
                ФИНАЛОЧКА
                """,
                typeof(DuplicateSymbolException)
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ() ПОЕХАЛИ
                    ВЫБРОС(н);
                ФИНАЛОЧКА
                """,
                typeof(UnknownSymbolException)
            },
            {
                """
                БАЗА ПОЛТОРАШКА радиус = 3.0;

                ПРОКРАСТИНИРУЕМ ПОГНАЛИ() 
                ПОЕХАЛИ
                    радиус = 5.0;
                ФИНАЛОЧКА
                """,
                typeof(InvalidAssignmentException)
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ
                    ЦИФЕРКА тест = 5;
                    МИНИМУМ(тест);
                ФИНАЛОЧКА
                """,
                typeof(InvalidFunctionCallException)
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ
                    ЦИФЕРКА тест = 5;
                    ДЛИНА(тест);
                ФИНАЛОЧКА
                """,
                typeof(TypeErrorException)
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ
                    ХВАТИТ;
                ФИНАЛОЧКА
                """,
                typeof(InvalidExpressionException)
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ
                    ВЫБРОС(1 + "привет");
                ФИНАЛОЧКА
                """,
                typeof(TypeErrorException)
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ() ПОЕХАЛИ
                    ВЫБРОС("текст" * 2);
                ФИНАЛОЧКА
                """,
                typeof(TypeErrorException)
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ() ПОЕХАЛИ
                    ВЫБРОС(3.14 == "пи");
                ФИНАЛОЧКА
                """,
                typeof(TypeErrorException)
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ() ПОЕХАЛИ
                    ЦИФЕРКА икс = "текст";
                ФИНАЛОЧКА
                """,
                typeof(TypeErrorException)
            },
            {
                """
                ЦИФЕРКА тест() ПОЕХАЛИ
                    (ПОЯСНИТЕЛЬНАЯ-БРИГАДА: Нет ДРАТУТИ)
                ФИНАЛОЧКА
                """,
                typeof(TypeErrorException)
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetTopLevelTestData))]
    public void Can_top_level_data_parsed(string code, object[] expectedOutput)
    {
        Context context = new();
        FakeEnvironment environment = new();
        ExecuteTestCode(context, environment, code);
        IReadOnlyList<string> actualOutput = environment.GetOutputHistory();

        Assert.Equal(expectedOutput.Length, actualOutput.Count);
        for (int i = 0; i < expectedOutput.Length; i++)
        {
            Assert.Equal(expectedOutput[i].ToString(), actualOutput[i]);
        }
    }

    public static TheoryData<string, object[]> GetTopLevelTestData()
    {
        return new TheoryData<string, object[]>
        {
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ
                    ВЫБРОС(7);
                ФИНАЛОЧКА
                """,
                new object[] { 7 }
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ
                    ЦИФЕРКА х = 5;
                    ВЫБРОС(х);
                ФИНАЛОЧКА
                """,
                new object[] { 5 }
            },
            {
                """
                ЦИФЕРКА х = 10;
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ 
                    ВЫБРОС(х); 
                ФИНАЛОЧКА
                """,
                new object[] { 10 }
            },
            {
                """
                ЦИФЕРКА х = 10;
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ 
                    ВЫБРОС(МИНИМУМ(10.0, 3.0));
                ФИНАЛОЧКА
                """,
                new object[] { 3 }
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ 
                    ЦИФЕРКА и = 0;
                    ЦИКЛ (и = 1; и <= 5; и = и + 1) ПОЕХАЛИ 
                        ВЫБРОС(и);
                    ФИНАЛОЧКА 
                ФИНАЛОЧКА
                """,
                new object[] { 1, 2, 3, 4, 5 }
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ 
                    ЦИФЕРКА х = 1;
                    ПОКА (х < 5) ПОЕХАЛИ 
                        ВЫБРОС(х); 
                        х = х + 1;
                    ФИНАЛОЧКА;
                ФИНАЛОЧКА
                """,
                new object[] { 1, 2, 3, 4 }
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ 
                    ЦИФЕРКА х = 5;
                    ЕСЛИ (х > 2) ТО ВЫБРОС(10); 
                    ИНАЧЕ ВЫБРОС(5); 
                ФИНАЛОЧКА
                """,
                new object[] { 10 }
            },
            {
                """
                ПРОКРАСТИНИРУЕМ ПОГНАЛИ()
                ПОЕХАЛИ 
                    ЦИТАТА привет = "привет"; 
                    ЦИТАТА мир = "мир"; 
                    ВЫБРОС(привет + мир); 
                ФИНАЛОЧКА
                """,
                new object[] { "приветмир" }
            },
        };
    }

    private void ExecuteTestCode(Context context, IEnvironment environment, string code)
    {
        Parser parser = new(code);
        List<AstNode> nodes = parser.ParseProgram();

        SemanticsChecker checker = new(
            Builtins.Functions,
            Builtins.Types
        );
        checker.Check(nodes);

        AstEvaluator evaluator = new(context, environment);
        foreach (Declaration node in nodes.OfType<Declaration>())
        {
            node.Accept(evaluator);
        }

        List<AstNode> topLevelStatements = nodes.Where(n => n is not Declaration).ToList();
        if (topLevelStatements.Count > 0)
        {
            foreach (AstNode node in topLevelStatements)
            {
                evaluator.Evaluate(node);
            }
        }
        else
        {
            FunctionCallExpression mainCall = new FunctionCallExpression("ПОГНАЛИ", new List<Expression>());
            evaluator.Evaluate(mainCall);
        }
    }
}