namespace Execution;

/// <summary>
/// Контекст выполнения программы - управляет областями видимости и значениями переменных/констант.
/// </summary>
public class Context
{
    private Scope? currentScope;

    public Context()
    {
        currentScope = new Scope();
    }

    public Context(Scope scope)
    {
        currentScope = scope;
    }

    /// <summary>
    /// Добавляет новую область видимости в стек.
    /// </summary>
    public Scope PushScope()
    {
        currentScope = new Scope(currentScope);
        return currentScope;
    }

    /// <summary>
    /// Добавляет указанную область видимости в стек.
    /// </summary>
    public void PushScope(Scope scope)
    {
        currentScope = scope;
    }

    /// <summary>
    /// Удаляет текущую область видимости из стека (возвращается к родительской).
    /// </summary>
    public void PopScope()
    {
        if (currentScope == null)
        {
            throw new InvalidOperationException("Cannot pop scope - no scope available");
        }

        currentScope = currentScope.GetRoot();
    }

    /// <summary>
    /// Определяет переменную в текущей области видимости.
    /// </summary>
    public void DefineVariable(string name, double value)
    {
        if (currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        currentScope.DefineVariable(name, value);
    }

    /// <summary>
    /// Присваивает значение переменной.
    /// </summary>
    public void AssignVariable(string name, double value)
    {
        if (currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        currentScope.AssignVariable(name, value);
    }

    /// <summary>
    /// Определяет константу в текущей области видимости.
    /// </summary>
    public void DefineConstant(string name, double value)
    {
        if (currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        currentScope.DefineConstant(name, value);
    }

    /// <summary>
    /// Получает значение переменной или константы.
    /// </summary>
    public double GetValue(string name)
    {
        if (currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        return currentScope.GetValue(name);
    }

    /// <summary>
    /// Проверяет, существует ли переменная или константа.
    /// </summary>
    public bool HasValue(string name)
    {
        if (currentScope == null)
        {
            return false;
        }

        return currentScope.HasValue(name);
    }

    /// <summary>
    /// Проверяет, является ли переменная константой.
    /// </summary>
    public bool IsConstant(string name)
    {
        if (currentScope == null)
        {
            return false;
        }

        return currentScope.IsConstant(name);
    }

    /// <summary>
    /// Проверяет, существует ли переменная или константа только в текущей области видимости (без подъема вверх).
    /// </summary>
    public bool HasLocalValue(string name)
    {
        if (currentScope == null)
        {
            return false;
        }

        return currentScope.HasLocalValue(name);
    }

    /// <summary>
    /// Получает текущую область видимости.
    /// </summary>
    public Scope GetCurrentScope()
    {
        if (currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        return currentScope;
    }

    /// <summary>
    /// Определяет функцию в текущей области видимости.
    public void DefineFunction(string name)
    {
        if (currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        currentScope.DefineFunction(name);
    }

    /// <summary>
    /// Проверяет, является ли имя функцией.
    /// </summary>
    public bool IsFunction(string name)
    {
        if (currentScope == null)
        {
            return false;
        }

        return currentScope.IsFunction(name);
    }

    /// <summary>
    /// Проверяет, является ли имя функцией только в текущей области видимости (без подъема вверх).
    /// </summary>
    public bool IsFunctionLocal(string name)
    {
        if (currentScope == null)
        {
            return false;
        }

        return currentScope.IsFunctionLocal(name);
    }
}