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

    /// <summary>
    /// Записывает число в консоль.
    /// </summary>
    public void WriteNumber(double value)
    {
        Console.WriteLine(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }
}
