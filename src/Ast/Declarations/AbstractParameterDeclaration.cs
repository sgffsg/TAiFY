namespace Ast.Declarations;

public abstract class AbstractParameterDeclaration : AbstractVariableDeclaration
{
    protected AbstractParameterDeclaration(string name)
        : base(name)
    {
    }
}
