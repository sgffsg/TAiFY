using Ast.Attributes;
using Ast.Declarations;
using Ast.Expressions;

namespace Ast.Statements;

public sealed class InputStatement : Expression
{
    private readonly List<AstAttribute<AbstractVariableDeclaration>> variables = new();

    public InputStatement(List<string> variableNames)
    {
        VariableNames = variableNames;
    }

    public List<string> VariableNames { get; }

    public void SetVariable(int index, AbstractVariableDeclaration decl) => variables[index].Set(decl);

    public AbstractVariableDeclaration GetVariable(int index) => variables[index].Get();

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}