namespace Ast.Statements;

public sealed class InputStatement : Statement
{
    public InputStatement(List<string> variableNames)
    {
        VariableNames = variableNames;
    }

    public List<string> VariableNames { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}