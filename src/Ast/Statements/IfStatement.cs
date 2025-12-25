using Ast.Expressions;

namespace Ast.Statements;

public sealed class IfStatement : Expression
{
    public IfStatement(Expression condition, AstNode thenBranch, AstNode? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public Expression Condition { get; }

    public AstNode ThenBranch { get; }

    public AstNode? ElseBranch { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}