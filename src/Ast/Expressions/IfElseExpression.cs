using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ast.Statements;

namespace Ast.Expressions;

public sealed class IfElseExpression : Expression
{
    public IfElseExpression(Expression condition, Expression thenBranch, Expression? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public Expression Condition { get; }

    public Expression ThenBranch { get; }

    public Expression? ElseBranch { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}