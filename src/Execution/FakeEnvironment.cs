namespace Execution;

/// <summary>
/// Симулирует ввод-вывод в тестах.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly Queue<double> inputQueue = new();
    private readonly List<double> outputList = new();

    public FakeEnvironment()
    {
    }

    public FakeEnvironment(params double[] inputs)
    {
        foreach (double input in inputs)
        {
            inputQueue.Enqueue(input);
        }
    }

    /// <summary>
    /// Добавляет значение в очередь ввода.
    /// </summary>
    public void AddInput(double value)
    {
        inputQueue.Enqueue(value);
    }

    /// <summary>
    /// Читает число из очереди ввода.
    /// </summary>
    public double ReadNumber()
    {
        if (inputQueue.Count == 0)
        {
            return 0;
        }

        return inputQueue.Dequeue();
    }

    /// <summary>
    /// Записывает число в список вывода.
    /// </summary>
    public void WriteNumber(double value)
    {
        outputList.Add(value);
    }

    /// <summary>
    /// Получает список всех выведенных значений.
    /// </summary>
    public IReadOnlyList<double> GetOutput()
    {
        return outputList.AsReadOnly();
    }

    /// <summary>
    /// Очищает список вывода.
    /// </summary>
    public void ClearOutput()
    {
        outputList.Clear();
    }

    /// <summary>
    /// Очищает очередь ввода.
    /// </summary>
    public void ClearInput()
    {
        inputQueue.Clear();
    }

    public string ReadString()
    {
        throw new NotImplementedException();
    }

    public void Write(double value)
    {
        outputList.Add(value);
    }
}