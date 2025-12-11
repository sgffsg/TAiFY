using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Объявление переменной внутри блока (как statement).
/// </summary>
public sealed class VariableDeclarationStatement : Statement
{
    public VariableDeclarationStatement(string typeName, string name, Expression? value)
    {
        TypeName = typeName;
        Name = name;
        Value = value;
    }

    public string TypeName { get; }

    public string Name { get; }

    public Expression? Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}