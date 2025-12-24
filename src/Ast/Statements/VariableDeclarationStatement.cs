using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Выражение переменной.
/// </summary>
public sealed class VariableDeclarationStatement : Statement
{
    public VariableDeclarationStatement(string type, string name, Expression? initialValue)
    {
        Type = type;
        Name = name;
        InitialValue = initialValue;
    }

    public string Type { get; }

    public string Name { get; }

    public Expression? InitialValue { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
