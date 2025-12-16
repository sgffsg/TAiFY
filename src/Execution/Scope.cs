using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Execution;

/// <summary>
/// Представляет область видимости для выполнения программы.
/// Хранит значения переменных и констант в текущей области видимости.
/// </summary>
public class Scope
{
    private readonly Dictionary<string, double> variables = new();
    private readonly Dictionary<string, double> constants = new();
    private readonly HashSet<string> functions = new();
    private readonly HashSet<string> procedures = new();

    public Scope()
    {
    }

    public void DefineVariable(string name, double initialValue = 0)
    {
        if (variables.ContainsKey(name) && constants.ContainsKey(name))
        {
            throw new InvalidOperationException($"Идентификатор '{name}' уже существует.");
        }

        variables[name] = initialValue;
    }

    public double GetVariable(string name)
    {
        if (variables.TryGetValue(name, out double value))
        {
            return value;
        }

        throw new KeyNotFoundException($"Переменная '{name}' не найдена.");
    }

    public bool TryGetVariable(string name, out double value)
        => variables.TryGetValue(name, out value);

    public bool HasVariable(string name) => variables.ContainsKey(name);

    public void DeclareConstant(string name, double value)
    {
        if (constants.ContainsKey(name) || variables.ContainsKey(name))
        {
            throw new InvalidOperationException($"Идентификатор '{name}' уже существует");
        }

        constants[name] = value;
    }

    public double GetConstant(string name)
    {
        if (constants.TryGetValue(name, out double value))
        {
            return value;
        }

        throw new KeyNotFoundException($"Константа '{name}' не найдена");
    }

    public bool TryGetConstant(string name, out double value)
        => constants.TryGetValue(name, out value);

    public bool HasConstant(string name) => constants.ContainsKey(name);

    public void DefineFunction(string name)
    {
        if (functions.Contains(name) || procedures.Contains(name))
        {
            throw new InvalidOperationException($"Идентификатор '{name}' уже существует");
        }

        functions.Add(name);
    }

    public bool HasFunction(string name) => functions.Contains(name);

    public void RegisterProcedure(string name)
    {
        if (procedures.Contains(name) || functions.Contains(name))
        {
            throw new InvalidOperationException($"Идентификатор '{name}' уже существует");
        }

        procedures.Add(name);
    }

    public bool HasProcedure(string name) => procedures.Contains(name);

    public bool Exists(string name)
    {
        return variables.ContainsKey(name) ||
               constants.ContainsKey(name) ||
               functions.Contains(name) ||
               procedures.Contains(name);
    }

    public void Clean()
    {
        variables.Clear();
        constants.Clear();
        procedures.Clear();
        functions.Clear();
    }
}
