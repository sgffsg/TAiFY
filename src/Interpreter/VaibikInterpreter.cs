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
    /// Выполняет программу из переданного кода.
    /// </summary>
    public void Execute(string code)
    {
        Parser.Parser parser = new Parser.Parser(context, environment, code);
        parser.ParseProgram();
    }
}