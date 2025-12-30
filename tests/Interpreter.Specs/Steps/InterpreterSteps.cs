using System.Globalization;
using Execution;
using Reqnroll;
using Runtime;

namespace Interpreter.Specs.Steps;

[Binding]
public class InterpreterSteps
{
    private string? programCode;
    private Context? context;
    private FakeEnvironment? fakeEnvironment;
    private Exception? executionException;

    [Given(@"я подготовил код программы:")]
    public void GivenЯПодготовилКодПрограммы(string multilineText)
    {
        context = new Context();
        fakeEnvironment = new FakeEnvironment();
        programCode = multilineText;
        executionException = null;
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
            string rawValue = row["Число"];

            if (int.TryParse(rawValue, out int intVal))
            {
                fakeEnvironment.EnqueueInput(new Value(intVal));
            }
            else if (double.TryParse(rawValue, CultureInfo.InvariantCulture, out double dblVal))
            {
                fakeEnvironment.EnqueueInput(new Value(dblVal));
            }
            else
            {
                fakeEnvironment.EnqueueInput(new Value(rawValue));
            }
        }
    }

    [When(@"я выполняю программу")]
    public void WhenЯВыполняюПрограмму()
    {
        try
        {
            VaibikiInterpreter interpreter = new(fakeEnvironment!);
            interpreter.Execute(programCode!);
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

        IReadOnlyList<string> actualResults = fakeEnvironment!.GetOutputHistory();
        if (actualResults.Count != table.Rows.Count)
        {
            Assert.Fail($"Количество строк вывода не совпадает. Ожидалось: {table.Rows.Count}, Получено: {actualResults.Count}. " +
                        $"\nВывод программы: {string.Join(" | ", actualResults)}");
        }

        for (int i = 0; i < actualResults.Count; i++)
        {
            string expectedStr = table.Rows[i]["Результат"];
            string actualStr = actualResults[i];

            if (double.TryParse(expectedStr, CultureInfo.InvariantCulture, out double expDouble) && double.TryParse(actualStr, CultureInfo.InvariantCulture, out double actDouble))
            {
                Assert.Equal(expDouble, actDouble);
            }
            else if (long.TryParse(expectedStr, out long expInt) && long.TryParse(actualStr, out long actInt))
            {
                Assert.Equal(expInt, actInt);
            }
            else
            {
                Assert.Equal(expectedStr, actualStr);
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
        executionException = null;
    }
}
