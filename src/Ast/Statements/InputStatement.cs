namespace Ast.Statements;

/// <summary>
/// Ввод значений с консоли в переменные.
/// </summary>
public sealed class InputStatement : Statement
{
    public InputStatement(List<string> variables)
    {
        Variables = variables;
    }

    /// <summary>
    /// Список переменных для ввода значений.
    /// </summary>
    public List<string> Variables { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}