namespace Execution;

/// <summary>
/// Реализация IEnvironment для работы с консолью.
/// </summary>
public class ConsoleEnvironment : IEnvironment
{
    /// <summary>
    /// Читает число из консоли.
    /// </summary>
    public double ReadNumber()
    {
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            return 0;
        }

        if (double.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double result))
        {
            return result;
        }

        return 0;
    }

    public string ReadString()
    {
        string? input = Console.ReadLine();
        return input ?? "";
    }

    public void Write(double value)
    {
        Console.WriteLine(value.ToString());
    }
}
