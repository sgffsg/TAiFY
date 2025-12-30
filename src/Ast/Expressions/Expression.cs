using Ast.Attributes;
using ValueType = Runtime.ValueType;

namespace Ast.Expressions;

/// <summary>
/// Абстрактный класс всех выражений (expressions).
/// </summary>
public abstract class Expression : AstNode
{
    private AstAttribute<ValueType> resultType;

    /// <summary>
    /// Тип результата инструкции.
    /// </summary>
    public ValueType ResultType
    {
        get => resultType.Get();
        set => resultType.Set(value);
    }
}