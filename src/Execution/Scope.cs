namespace Execution;

/// <summary>
/// Представляет область видимости для выполнения программы.
/// Хранит значения переменных и констант в текущей области видимости.
/// </summary>
public class Scope
{
    private readonly Dictionary<string, double> variables = new();
    private readonly HashSet<string> constants = new();
    private readonly HashSet<string> functions = new();
    private readonly Scope? root;

    public Scope(Scope? root = null)
    {
        this.root = root;
    }

    /// <summary>
    /// Определяет переменную с начальным значением в текущей области видимости.
    /// </summary>
    public void DefineVariable(string name, double value)
    {
        if (variables.ContainsKey(name))
        {
            throw new InvalidOperationException($"Variable '{name}' is already declared in this scope");
        }

        variables[name] = value;
    }

    /// <summary>
    /// Присваивает значение переменной. Ищет переменную в текущей области и родительских областях.
    /// </summary>
    public void AssignVariable(string name, double value)
    {
        if (constants.Contains(name))
        {
            throw new InvalidOperationException($"Cannot assign to constant '{name}'");
        }

        if (variables.ContainsKey(name))
        {
            variables[name] = value;
            return;
        }

        if (root != null)
        {
            root.AssignVariable(name, value);
            return;
        }

        throw new InvalidOperationException($"Variable '{name}' is not declared");
    }

    /// <summary>
    /// Определяет константу со значением в текущей области видимости.
    /// </summary>
    public void DefineConstant(string name, double value)
    {
        if (variables.ContainsKey(name) || constants.Contains(name))
        {
            throw new InvalidOperationException($"Constant '{name}' is already declared in this scope");
        }

        constants.Add(name);
        variables[name] = value;
    }

    /// <summary>
    /// Получает значение переменной или константы. Ищет в текущей области и родительских областях.
    /// </summary>
    public double GetValue(string name)
    {
        if (variables.TryGetValue(name, out double value))
        {
            return value;
        }

        if (root != null)
        {
            return root.GetValue(name);
        }

        throw new InvalidOperationException($"Variable or constant '{name}' is not declared");
    }

    /// <summary>
    /// Проверяет, существует ли переменная или константа в текущей области или родительских областях.
    /// </summary>
    public bool HasValue(string name)
    {
        if (variables.ContainsKey(name))
        {
            return true;
        }

        return root?.HasValue(name) ?? false;
    }

    /// <summary>
    /// Получает родительскую область видимости.
    /// </summary>
    public Scope? GetRoot()
    {
        return root;
    }

    /// <summary>
    /// Проверяет, является ли переменная константой.
    /// </summary>
    public bool IsConstant(string name)
    {
        if (constants.Contains(name))
        {
            return true;
        }

        return root?.IsConstant(name) ?? false;
    }

    /// <summary>
    /// Проверяет, существует ли переменная или константа только в текущей области видимости (без подъема вверх).
    /// </summary>
    public bool HasLocalValue(string name)
    {
        return variables.ContainsKey(name);
    }

    /// <summary>
    /// Определяет функцию в текущей области видимости.
    /// </summary>
    public void DefineFunction(string name)
    {
        if (variables.ContainsKey(name) || constants.Contains(name) || functions.Contains(name))
        {
            throw new InvalidOperationException($"Function '{name}' is already declared");
        }

        functions.Add(name);
    }

    /// <summary>
    /// Проверяет, является ли имя функцией.
    /// </summary>
    public bool IsFunction(string name)
    {
        if (functions.Contains(name))
        {
            return true;
        }

        return root?.IsFunction(name) ?? false;
    }

    /// <summary>
    /// Проверяет, является ли имя функцией только в текущей области видимости (без подъема вверх).
    /// </summary>
    public bool IsFunctionLocal(string name)
    {
        return functions.Contains(name);
    }
}
