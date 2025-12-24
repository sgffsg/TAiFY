using System.Xml.Linq;

using Ast.Declarations;

namespace Execution;

/// <summary>
/// Контекст выполнения программы (все переменные, константы и другие символы).
/// </summary>
public class Context
{
    private readonly Stack<Scope> scopes = new();
    private readonly Dictionary<string, double> constants = [];
    private readonly Dictionary<string, FunctionDeclaration> functions = [];
    private readonly Dictionary<string, ProcedureDeclaration> procedures = [];

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

    public double GetValue(string name)
    {
        foreach (Scope scope in scopes)
        {
            if (scope.TryGetVariable(name, out double variable))
            {
                return variable;
            }
        }

        if (constants.TryGetValue(name, out double constant))
        {
            return constant;
        }

        throw new ArgumentException($"Переменная {name} не определена");
    }

    public void AssignVariable(string name, double value)
    {
        foreach (Scope s in scopes)
        {
            if (s.TryAssignVariable(name, value))
            {
                return;
            }
        }

        throw new ArgumentException($"Переменная {name} не определена");
    }

    public Declaration GetCallable(string name)
    {
        if (functions.TryGetValue(name, out FunctionDeclaration? function))
        {
            return function;
        }

        if (procedures.TryGetValue(name, out ProcedureDeclaration? procedure))
        {
            return procedure;
        }

        throw new Exception($"Функция или процедура с именем '{name}' не определена");
    }

    public FunctionDeclaration GetFunction(string name)
    {
        if (functions.TryGetValue(name, out FunctionDeclaration? function))
        {
            return function;
        }

        throw new Exception($"Function '{name}' is not defined");
    }

    public ProcedureDeclaration GetProcedure(string name)
    {
        if (procedures.TryGetValue(name, out ProcedureDeclaration? procedure))
        {
            return procedure;
        }

        throw new Exception($"Procedure '{name}' is not defined");
    }

    public void DefineVariable(string name, double value)
    {
        if (!scopes.Peek().TryDefineVariable(name, value) || constants.TryGetValue(name, out _))
        {
            throw new ArgumentException($"Variable '{name}' is already defined in this scope");
        }
    }

    public void DefineConstant(string name, double value)
    {
        if (!constants.TryAdd(name, value))
        {
            throw new ArgumentException($"Constant '{name}' is already defined");
        }
    }

    public void DefineFunction(FunctionDeclaration function)
    {
        if (!functions.TryAdd(function.Name, function))
        {
            throw new ArgumentException($"Function '{function.Name}' is already defined");
        }
    }

    public void DefineProcedure(ProcedureDeclaration procedure)
    {
        if (!procedures.TryAdd(procedure.Name, procedure))
        {
            throw new ArgumentException($"Function '{procedure.Name}' is already defined");
        }
    }

    public bool Exists(string name)
    {
        foreach (Scope scope in scopes)
        {
            if (scope.TryGetVariable(name, out _))
            {
                return true;
            }
        }

        if (constants.ContainsKey(name))
        {
            return true;
        }

        return functions.ContainsKey(name) || procedures.ContainsKey(name);
    }
}