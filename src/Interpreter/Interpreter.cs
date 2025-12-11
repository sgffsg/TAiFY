using Execution;

namespace Interpreter;

public class Interpreter
{
    public Interpreter()
    {
    }

    /// <summary>
    /// Выполняет программу на языке Vaibik.
    /// </summary>
    /// <param name="sourceCode">Исходный код программы.</param>
    public static void Execute(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));
        }

        Parser.Parser parser = new(new ConsoleEnvironment(), sourceCode);
        parser.ParseProgram();
    }
}