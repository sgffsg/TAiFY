using System;

using Ast;
using Ast.Declarations;
using Ast.Expressions;

using Execution;

using Semantics;

namespace Interpreter;

/// <summary>
/// Интерпретатор для выполнения программ.
/// </summary>
public class VaibikiInterpreter
{
    private readonly Builtins builtins;
    private readonly Context context;
    private readonly IEnvironment environment;

    public VaibikiInterpreter(IEnvironment environment)
    {
        this.environment = environment;
        this.builtins = new(this.environment);
        this.context = new();
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

        Parser.Parser parser = new(sourceCode);
        List<AstNode> nodes = parser.ParseProgram();

        SemanticsChecker checker = new(
            builtins.Functions,
            builtins.Constants,
            builtins.Types
        );
        checker.Check(nodes);

        AstEvaluator evaluator = new AstEvaluator(context, environment);

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
                FunctionCallExpression mainCall = new FunctionCallExpression("ПОГНАЛИ", new List<Expression>());
                evaluator.Evaluate(mainCall);
            }
            catch (Exception ex) when (ex.Message.Contains("not defined"))
            {
                // ПОГНАЛИ не найдена
            }
        }
    }
}