namespace Ast.Expressions;

/// <summary>
/// Идентификатор переменной или функции.
/// </summary>
public sealed class IdentifierExpression : Expression
{
    public IdentifierExpression(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Имя идентификатора.
    /// </summary>
    public string Name { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}