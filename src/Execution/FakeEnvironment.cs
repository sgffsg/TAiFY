using Runtime;

namespace Execution;

/// <summary>
/// Симулирует ввод-вывод в тестах.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly Queue<Value> inputQueue = new();

    private readonly List<string> outputHistory = new();

    public void EnqueueInput(Value value) => inputQueue.Enqueue(value);

    public IReadOnlyList<string> GetOutputHistory() => outputHistory;

    public void Write(string message)
    {
        outputHistory.Add(message);
    }

    public Value Read(Runtime.ValueType expectedType)
    {
        if (inputQueue.Count == 0)
        {
            throw new InvalidOperationException(
                $"Тестовая ошибка: Попытка чтения (ВБРОС), но очередь ввода пуста. Ожидался тип: {expectedType}"
            );
        }

        Value value = inputQueue.Dequeue();
        if (value.GetValueType() != expectedType)
        {
            throw new InvalidOperationException(
                $"Ошибка типизации в тесте: ВБРОС ожидал {expectedType}, но в очереди было {value.GetValueType()}"
            );
        }

        return value;
    }

    public void Reset()
    {
        inputQueue.Clear();
        outputHistory.Clear();
    }

    public void ClearOutput()
    {
        outputHistory.Clear();
    }

    public void ClearInput()
    {
        inputQueue.Clear();
    }
}