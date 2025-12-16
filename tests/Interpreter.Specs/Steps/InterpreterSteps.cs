using System.Globalization;

using Execution;

using Reqnroll;

namespace Interpreter.Specs.Steps;

[Binding]
public class InterpreterSteps
{
    private string? programCode;
    private FakeEnvironment? fakeEnvironment;
    private Parser.Parser? parser;
    private Exception? executionException;

    [Given(@"я запустил программу:")]
    public void GivenЯЗапустилПрограмму(string multilineText)
    {
        programCode = multilineText;
        fakeEnvironment = new FakeEnvironment();

        try
        {
            parser = new Parser.Parser(fakeEnvironment, programCode);
        }
        catch (Exception ex)
        {
            executionException = ex;
            throw;
        }
    }

    [Given(@"я установил входные данные:")]
    public void GivenЯУстановилВходныеДанные(Table table)
    {
        if (fakeEnvironment == null)
        {
            throw new InvalidOperationException("Сначала нужно запустить программу");
        }

        foreach (DataTableRow? row in table.Rows)
        {
            if (row.TryGetValue("Число", out string? valueStr))
            {
                if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                {
                    fakeEnvironment.AddInput(value);
                }
                else
                {
                    throw new FormatException($"Неверный формат числа: '{valueStr}'");
                }
            }
        }
    }

    [When(@"я выполняю программу")]
    public void WhenЯВыполняюПрограмму()
    {
        if (parser == null)
        {
            throw new InvalidOperationException("Парсер не инициализирован");
        }

        try
        {
            parser.ParseProgram();
            executionException = null;
        }
        catch (Exception ex)
        {
            executionException = ex;
        }
    }

    [Then(@"я получаю результаты:")]
    public void ThenЯПолучаюРезультаты(Table table)
    {
        if (executionException != null)
        {
            throw new InvalidOperationException(
                $"Программа завершилась с ошибкой: {executionException.Message}",
                executionException);
        }

        if (fakeEnvironment == null)
        {
            throw new InvalidOperationException("Окружение не инициализировано");
        }

        IReadOnlyList<double> results = fakeEnvironment.GetOutput();
        List<double> expectedResults = table.Rows
            .Select(row => row.TryGetValue("Результат", out string? valueStr) ? valueStr : null)
            .Where(valueStr => valueStr != null)
            .Select(valueStr => double.Parse(valueStr!, CultureInfo.InvariantCulture))
            .ToList();

        if (results.Count != expectedResults.Count)
        {
            throw new Exception(
                $"Количество результатов не совпадает. Ожидалось: {expectedResults.Count}, Получено: {results.Count}");
        }

        for (int i = 0; i < results.Count; i++)
        {
            if (Math.Abs(results[i] - expectedResults[i]) > 0.0000000001)
            {
                throw new Exception($"Результат [{i}] не совпадает. Ожидалось: {expectedResults[i]}, Получено: {results[i]}");
            }
        }
    }

    [Then(@"программа завершается с ошибкой")]
    public void ThenПрограммаЗавершаетсяСОшибкой()
    {
        if (executionException == null)
        {
            throw new Exception("Ожидалась ошибка выполнения, но программа завершилась успешно");
        }
    }

    [AfterScenario]
    public void Cleanup()
    {
        programCode = null;
        fakeEnvironment = null;
        parser = null;
        executionException = null;
    }
}
