using Ast;
using Ast.Declarations;
using Ast.Expressions;

using Execution;

namespace Interpreter;

/// <summary>
/// Интерпретатор для выполнения программ.
/// </summary>
public class VaibikiInterpreter
{
    private readonly Context context;
    private readonly IEnvironment environment;

    public VaibikiInterpreter()
        : this(new Context(), new ConsoleEnvironment())
    {
    }

    public VaibikiInterpreter(Context context, IEnvironment environment)
    {
        this.context = context;
        this.environment = environment;
    }

    /// <summary>
    /// Выполняет программу на языке Vaibik.
    /// </summary>
    /// <param name="sourceCode">Исходный код программы.</param>
    public void Execute(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));
        }

        AstEvaluator evaluator = new AstEvaluator(context, environment);
        Parser.Parser parser = new Parser.Parser(sourceCode);
        List<AstNode> nodes = parser.ParseProgram();

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
            try
            {
                CallExpression mainCall = new CallExpression("ПОГНАЛИ", new List<Expression>());
                evaluator.Evaluate(mainCall);
            }
            catch (Exception ex) when (ex.Message.Contains("not defined"))
            {
                // ПОГНАЛИ не найдена
            }
        }
    }
}