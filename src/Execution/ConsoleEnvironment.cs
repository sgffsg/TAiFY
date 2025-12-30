using Runtime;
using ValueType = Runtime.ValueType;

namespace Execution;

/// <summary>
/// Реализация IEnvironment для работы с консолью.
/// </summary>
public class ConsoleEnvironment : IEnvironment
{
    public Value Read(ValueType expectedType)
    {
        while (true)
        {
            string? input = Console.ReadLine();
            try
            {
                return expectedType switch
                {
                    ValueType.ЦИФЕРКА => new Value(int.Parse(input ?? "0")),
                    ValueType.ПОЛТОРАШКА => new Value(double.Parse(input ?? "0.0")),
                    ValueType.РАСКЛАД => input?.ToUpper() == "ХАЙП" ? new Value(true) :
                                         input?.ToUpper() == "КРИНЖ" ? new Value(false) :
                                         throw new Exception(),
                    ValueType.ЦИТАТА => new Value(input ?? ""),
                    _ => throw new ArgumentException("Неподдерживаемый тип для чтения")
                };
            }
            catch
            {
                Console.WriteLine($"Ошибка: Ожидался тип {expectedType}. Попробуйте еще раз:");
            }
        }
    }

    public void Write(string message)
    {
        Console.Write(message);
    }

    public void AddResult(Value value)
    {
        Console.WriteLine($"Результат: {value}");
    }
}
