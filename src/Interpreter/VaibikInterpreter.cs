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
    /// Выполняет программу на языке Vaibik
    /// </summary>
    /// <param name="sourceCode">Исходный код программы</param>
    public void Execute(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));
        }

        Parser.Parser parser = new Parser.Parser(context, environment, sourceCode);
        parser.ParseProgram();
    }
}