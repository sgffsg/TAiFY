namespace Ast.Expressions;

public class AssignmentExpression : Expression
{
    public AssignmentExpression(string variableName, Expression value)
    {
        VariableName = variableName;
        Value = value;
    }

    public string VariableName { get; }

    public Expression Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
