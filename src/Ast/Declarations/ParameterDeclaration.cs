namespace Ast.Declarations;

/// <summary>
/// Объявление параметра функции.
/// </summary>
public class ParameterDeclaration : Declaration
{
    public ParameterDeclaration(string name, string type)
    {
        Name = name;
        Type = type;
    }

    /// <summary>
    /// Имя параметра.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Тип параметра.
    /// </summary>
    public string Type { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}