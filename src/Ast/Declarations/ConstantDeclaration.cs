using Ast.Expressions;

namespace Ast.Declarations;

/// <summary>
/// Объявление константы.
/// </summary>
public sealed class ConstantDeclaration : Declaration
{
    public ConstantDeclaration(string typeName, string name, Expression value)
    {
        TypeName = typeName;
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Тип константы.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Имя константы.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Значение константы.
    /// </summary>
    public Expression Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
