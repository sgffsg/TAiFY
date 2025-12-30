using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ast.Expressions;

namespace Ast.Statements;

public sealed class ForStatement : Expression
{
    public ForStatement(AssignmentExpression initializer, Expression condition, AssignmentExpression iterator, BlockStatement body)
    {
        Initializer = initializer;
        Condition = condition;
        Iterator = iterator;
        Body = body;
    }

    public AssignmentExpression Initializer { get; }

    public Expression Condition { get; }

    public AssignmentExpression Iterator { get; }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}