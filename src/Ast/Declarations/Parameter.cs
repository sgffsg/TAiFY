namespace Ast.Declarations;

public sealed class Parameter
{
    public Parameter(string type, string name)
    {
        Type = type;
        Name = name;
    }

    public string Type { get; }

    public string Name { get; }
}