using Ast.Declarations;

namespace Ast.Expressions;

public sealed class IndexAssignmentExpression : Expression
{
    public IndexAssignmentExpression(string identifier, Expression index, Expression value)
    {
        Identifier = identifier;
        IndexExpression = index;
        Value = value;
    }

    public string Identifier { get; }

    public Expression IndexExpression { get; }

    public Expression Value { get; }

    public AbstractVariableDeclaration Variable { get; set; } = null!;

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
