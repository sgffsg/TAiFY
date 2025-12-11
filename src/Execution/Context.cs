using Ast.Declarations;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Execution;

/// <summary>
/// Контекст выполнения программы (все переменные, константы, функции).
/// </summary>
public class Context
{
    private readonly Stack<Scope> scopes = new();
    private readonly Dictionary<string, double> constants = new();
    private readonly Dictionary<string, FunctionDeclaration> functions = new();
    private readonly Dictionary<string, ProcedureDeclaration> procedures = new();

    public Context()
    {
        PushScope(new Scope());
    }

    /// <summary>
    /// Возвращает текущую область видимости.
    /// </summary>
    public Scope CurrentScope => scopes.Peek();

    public void PushScope(Scope scope)
    {
        scopes.Push(scope);
    }

    public Scope CreateAndPushScope()
    {
        Scope scope = new Scope();
        PushScope(scope);
        return scope;
    }

    public void PopScope()
    {
        if (scopes.Count <= 1)
        {
            throw new InvalidOperationException("Невозможно удалить глобальную область видимости");
        }
        scopes.Pop();
    }

    /// <summary>
    /// Возвращает значение переменной или константы.
    /// </summary>
    public double GetVariable(string name)
    {
        foreach (Scope scope in scopes)
        {
            if (scope.TryGetVariable(name, out double value))
            {
                return value;
            }
        }

        if (constants.TryGetValue(name, out double constantValue))
        {
            return constantValue;
        }

        throw new ArgumentException($"Переменная или константа '{name}' не определена");
    }

    /// <summary>
    /// Проверяет, определена ли переменная или константа.
    /// </summary>
    public bool HasVariable(string name)
    {
        // Проверяем области видимости
        foreach (Scope scope in scopes)
        {
            if (scope.ContainsVariable(name))
                return true;
        }

        // Проверяем константы
        return constants.ContainsKey(name);
    }

    /// <summary>
    /// Присваивает (изменяет) значение переменной.
    /// </summary>
    public void AssignVariable(string name, double value)
    {
        Scope[] scopesArray = scopes.ToArray();
        foreach (Scope scope in scopesArray)
        {
            if (scope.TryAssignVariable(name, value))
            {
                return;
            }
        }

        throw new ArgumentException($"Переменная '{name}' не определена");
    }

    /// <summary>
    /// Определяет переменную в текущей области видимости.
    /// </summary>
    public void DeclareVariable(string name, double value)
    {
        if (!CurrentScope.TryDefineVariable(name, value))
        {
            throw new ArgumentException($"Переменная '{name}' уже определена в текущей области видимости");
        }
    }

    /// <summary>
    /// Определяет константу в глобальной области видимости.
    /// </summary>
    public void DeclareConstant(string name, double value)
    {
        if (!constants.TryAdd(name, value))
        {
            throw new ArgumentException($"Константа '{name}' уже определена");
        }
    }

    /// <summary>
    /// Получает объявление функции.
    /// </summary>
    public FunctionDeclaration GetFunction(string name)
    {
        if (functions.TryGetValue(name, out FunctionDeclaration? function))
        {
            return function;
        }

        throw new ArgumentException($"Функция '{name}' не определена");
    }

    /// <summary>
    /// Определяет функцию.
    /// </summary>
    public void DeclareFunction(FunctionDeclaration function)
    {
        if (!functions.TryAdd(function.Name, function))
        {
            throw new ArgumentException($"Функция '{function.Name}' уже определена");
        }
    }

    /// <summary>
    /// Получает объявление процедуры.
    /// </summary>
    public ProcedureDeclaration GetProcedure(string name)
    {
        if (procedures.TryGetValue(name, out ProcedureDeclaration? procedure))
        {
            return procedure;
        }

        throw new ArgumentException($"Процедура '{name}' не определена");
    }

    /// <summary>
    /// Определяет процедуру.
    /// </summary>
    public void DeclareProcedure(ProcedureDeclaration procedure)
    {
        if (!procedures.TryAdd(procedure.Name, procedure))
        {
            throw new ArgumentException($"Процедура '{procedure.Name}' уже определена");
        }
    }

    /// <summary>
    /// Проверяет, определена ли функция.
    /// </summary>
    public bool HasFunction(string name) => functions.ContainsKey(name);

    /// <summary>
    /// Проверяет, определена ли процедура.
    /// </summary>
    public bool HasProcedure(string name) => procedures.ContainsKey(name);

    
}