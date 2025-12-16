namespace Execution;

/// <summary>
/// Контекст выполнения программы - управляет областями видимости и значениями переменных/констант.
/// </summary>
public class Context
{
    private readonly Stack<Scope> scopes = new();
    private readonly Scope globalScope = new();

    public Context()
    {
        scopes.Push(globalScope);
    }

    /// <summary>
    /// Добавляет новую область видимости в стек.
    /// </summary>
    public void PushScope(Scope scope)
    {
        scopes.Push(scope);
    }

    public void PopScope()
    {
        if (scopes.Count > 1)
        {
            scopes.Pop();
        }
    }

    /// <summary>
    /// Возвращает текущую область видимости.
    /// </summary>
    public Scope GetCurrentScope()
    {
        return scopes.Peek();
    }

    /// <summary>
    /// Возвращает значение переменной или константы.
    /// </summary>
    public double GetValue(string name)
    {
        foreach (Scope scope in scopes)
        {
            if (scope.TryGetVariable(name, out double variable))
            {
                return variable;
            }

            if (scope.TryGetConstant(name, out double constant))
            {
                return constant;
            }
        }

        throw new ArgumentException($"Переменная или константа '{name}' не определена");
    }

    /// <summary>
    /// Присваивает (изменяет) значение существующей переменной.
    /// Поиск ведется от текущей области к глобальной.
    /// </summary>
    public void AssignVariable(string name, double value)
    {
        foreach (Scope scope in scopes)
        {
            if (scope.HasVariable(name))
            {
                scope.DefineVariable(name, value);
                return;
            }

            if (scope.HasConstant(name))
            {
                throw new InvalidOperationException($"Невозможно изменить значение константы '{name}'");
            }
        }

        throw new ArgumentException($"Переменная '{name}' не определена");
    }

    /// <summary>
    /// Определяет переменную в текущей области видимости.
    /// </summary>
    public void DefineVariable(string name, double value = 0)
    {
        Scope currentScope = scopes.Peek();
        if (currentScope.Exists(name))
        {
            throw new ArgumentException($"Идентификатор '{name}' уже определен в текущей области видимости");
        }

        currentScope.DefineVariable(name, value);
    }

    /// <summary>
    /// Определяет константу в текущей области видимости.
    /// </summary>
    public void DefineConstant(string name, double value)
    {
        Scope currentScope = scopes.Peek();
        if (currentScope.Exists(name))
        {
            throw new ArgumentException($"Идентификатор '{name}' уже определен в текущей области видимости");
        }

        currentScope.DeclareConstant(name, value);
    }

    /// <summary>
    /// Определяет константу в глобальной области видимости.
    /// </summary>
    public void DefineGlobalConstant(string name, double value)
    {
        if (globalScope.Exists(name))
        {
            throw new ArgumentException($"Константа '{name}' уже определена в глобальной области");
        }

        globalScope.DeclareConstant(name, value);
    }

    /// <summary>
    /// Определяет функцию в текущей области видимости.
    /// </summary>
    public void DefineFunction(string name)
    {
        Scope currentScope = scopes.Peek();

        if (currentScope.Exists(name))
        {
            throw new ArgumentException($"Идентификатор '{name}' уже определен в текущей области видимости");
        }

        currentScope.DefineFunction(name);
    }

    /// <summary>
    /// Регистрирует процедуру в текущей области видимости.
    /// </summary>
    public void RegisterProcedure(string name)
    {
        Scope currentScope = scopes.Peek();

        if (currentScope.Exists(name))
        {
            throw new ArgumentException($"Идентификатор '{name}' уже определен в текущей области видимости");
        }

        currentScope.RegisterProcedure(name);
    }

    /// <summary>
    /// Проверяет существование идентификатора в текущей и родительских областях.
    /// </summary>
    public bool Exists(string name)
    {
        foreach (Scope scope in scopes)
        {
            if (scope.Exists(name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Проверяет существование идентификатора только в текущей области видимости.
    /// </summary>
    public bool ExistsInCurrentScope(string name)
    {
        return scopes.Peek().Exists(name);
    }

    /// <summary>
    /// Очищает текущую область видимости (кроме глобальной).
    /// </summary>
    public void CleanCurrentScope()
    {
        if (scopes.Count > 1)
        {
            scopes.Peek().Clean();
        }
    }
}