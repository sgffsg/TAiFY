using Ast.Expressions;

namespace Ast.Declarations;

/// <summary>
/// Выражение переменной.
/// </summary>
public sealed class VariableDeclaration : Declaration
{
    public VariableDeclaration(string typeName, string name, Expression? value)
    {
        TypeName = typeName;
        Name = name;
        Value = value;
    }

    public string TypeName { get; }

    /// <summary>
    /// Имя переменной.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Базовое значение переменной.
    /// </summary>
    public Expression? Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
