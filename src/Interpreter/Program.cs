using Parser;

namespace Interpreter;

public static class Program
{
    public static int Main(string[] args)
    {
        args = new string[1];
        args[0] = "../../../test.вайбик";
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: Interpreter <file-path>");
            return 1;
        }

        string sourcePath = args[0];

        if (!sourcePath.EndsWith(".вайбик"))
        {
            Console.Error.WriteLine($"Error: File '{sourcePath}' has an incorrect extension. Expected: .вайбик");
        }

        try
        {
            if (!File.Exists(sourcePath))
            {
                Console.Error.WriteLine($"Error: File '{sourcePath}' not found.");
                return 1;
            }

            string sourceCode = File.ReadAllText(sourcePath);

            VaibikiInterpreter interpreter = new();
            interpreter.Execute(sourceCode);

            return 0;
        }
        catch (UnexpectedLexemeException ex)
        {
            Console.Error.WriteLine($"Parse error: {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}