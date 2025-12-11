using Ast.Declarations;

namespace Execution;

/// <summary>
/// Контекст выполнения программы (все переменные, константы, функции).
/// </summary>
public class Context
{
    private readonly Stack<Scope> scopes = new();
    private readonly Dictionary<string, decimal> constants = new();
    private readonly Dictionary<string, FunctionDeclaration> functions = new();
    private readonly Dictionary<string, ProcedureDeclaration> procedures = new();


}