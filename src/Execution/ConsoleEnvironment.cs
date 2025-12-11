using System.Globalization;

namespace Execution;

public class ConsoleEnvironment : IEnvironment
{
    public void AddResult(decimal result)
    {
        Console.WriteLine("Result: " + result.ToString("0.#####", CultureInfo.InvariantCulture));
    }

    public decimal ReadNumber()
    {
        string input = Console.ReadLine() ?? "0";
        if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
        {
            return result;
        }

        throw new FormatException($"Некорректный ввод: '{input}'");
    }
}