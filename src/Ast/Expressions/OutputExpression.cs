using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ast.Expressions;

public sealed class OutputExpression : Expression
{
    public OutputExpression(List<Expression> arguments)
    {
        Arguments = arguments;
    }

    /// <summary>
    /// Список выражений для вывода.
    /// </summary>
    public List<Expression> Arguments { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}