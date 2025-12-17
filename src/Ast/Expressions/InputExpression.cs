using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ast.Expressions;

public sealed class InputExpression : Expression
{
    public InputExpression(List<string> variableNames)
    {
        VariableNames = variableNames;
    }

    /// <summary>
    /// Список переменных для ввода значений.
    /// </summary>
    public List<string> VariableNames { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}