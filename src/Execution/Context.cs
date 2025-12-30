using Ast.Declarations;
using Runtime;

namespace Execution;

/// <summary>
/// Контекст выполнения программы (все переменные, константы и другие символы).
/// </summary>
public class Context
{
    private readonly Stack<Scope> scopes = new();
    private readonly Dictionary<string, FunctionDeclaration> functions = [];

    public Context()
    {
        scopes.Push(new Scope());
    }

    public void PushScope(Scope scope)
    {
        scopes.Push(scope);
    }

    public void PopScope()
    {
        scopes.Pop();
    }

    public void DefineVariable(string name, Value value)
    {
        if (IsNameAlreadyTaken(name))
        {
            throw new Exception($"Семантическая ошибка: Имя '{name}' уже используется.");
        }

        scopes.Peek().Define(name, value, isConstant: false);
    }

    public void DefineConstant(string name, Value value)
    {
        if (IsNameAlreadyTaken(name))
        {
            throw new Exception($"Семантическая ошибка: Нельзя создать константу '{name}', имя занято.");
        }

        scopes.Peek().Define(name, value, isConstant: true);
    }

    public void DefineFunction(FunctionDeclaration func)
    {
        if (IsNameAlreadyTaken(func.Name))
        {
            throw new Exception($"Семантическая ошибка: Функция '{func.Name}' конфликтует с существующим именем.");
        }

        functions[func.Name] = func;
    }

    public void AssignVariable(string name, Value value)
    {
        foreach (Scope scope in scopes)
        {
            if (scope.Contains(name))
            {
                if (scope.IsConstant(name))
                {
                    throw new Exception($"Ошибка: Попытка изменить константу '{name}' (БАЗА).");
                }

                scope.Assign(name, value);
                return;
            }
        }

        throw new Exception($"Ошибка выполнения: Переменная '{name}' не найдена.");
    }

    public Value GetValue(string name)
    {
        foreach (Scope scope in scopes)
        {
            if (scope.TryGetValue(name, out Value? value))
            {
                return value!;
            }
        }

        throw new Exception($"Ошибка выполнения: Идентификатор '{name}' не определен.");
    }

    public FunctionDeclaration GetCallable(string name)
    {
        return functions.TryGetValue(name, out FunctionDeclaration? func)
            ? func
            : throw new Exception($"Функция '{name}' не найдена.");
    }

    public bool Exists(string name)
    {
        if (functions.ContainsKey(name))
        {
            return true;
        }

        return scopes.Any(s => s.Contains(name));
    }

    private bool IsNameAlreadyTaken(string name)
    {
        return functions.ContainsKey(name) || scopes.Any(s => s.Contains(name));
    }
}