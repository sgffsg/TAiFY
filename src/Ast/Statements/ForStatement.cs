using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ast.Expressions;

namespace Ast.Statements;

public sealed class ForStatement : Statement
{
    public ForStatement(Statement initializer, Expression condition, Statement iterator, BlockStatement body)
    {
        Initializer = initializer;
        Condition = condition;
        Iterator = iterator;
        Body = body;
    }

    public Statement Initializer { get; }

    public Expression Condition { get; }

    public Statement Iterator { get; }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}