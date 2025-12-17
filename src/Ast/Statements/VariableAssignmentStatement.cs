using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ast.Expressions;

namespace Ast.Statements;

public class VariableAssignmentStatement : Statement
{
    public VariableAssignmentStatement(string variableName, Expression value)
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
