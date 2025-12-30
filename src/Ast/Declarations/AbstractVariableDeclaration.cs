namespace Ast.Declarations;
public abstract class AbstractVariableDeclaration : Declaration
{
    protected AbstractVariableDeclaration(string name)
    {
        this.Name = name;
    }

    public string Name { get; }
}