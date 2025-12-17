using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ast.Expressions;

public sealed class SequenceExpression : Expression
{
    private readonly List<Expression> sequence;

    public SequenceExpression(List<Expression> sequence)
    {
        this.sequence = sequence;
    }

    public List<Expression> Sequence => sequence;

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
