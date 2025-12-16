using System.Globalization;

namespace Execution;

/// <summary>
/// Поддельное окружение.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly Queue<decimal> inputQueue = new();
    private readonly List<decimal> results = new();
    private readonly List<string> output = new();

    public FakeEnvironment()
    {
    }

    public FakeEnvironment(params decimal[] inputs)
    {
        foreach (decimal input in inputs)
        {
            inputQueue.Enqueue(input);
        }
    }

    public void AddResult(decimal result)
    {
        results.Add(result);
        output.Add(result.ToString("0.#####", CultureInfo.InvariantCulture));
    }

    public decimal ReadNumber()
    {
        if (inputQueue.Count > 0)
        {
            return inputQueue.Dequeue();
        }

        return 0;
    }

    public IReadOnlyList<decimal> GetResults() => results;

    public IReadOnlyList<string> GetOutput() => output;

    public void AddInput(decimal value) => inputQueue.Enqueue(value);

    public void Clear()
    {
        inputQueue.Clear();
        results.Clear();
        output.Clear();
    }
}