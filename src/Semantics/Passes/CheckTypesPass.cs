using Ast.Expressions;

namespace Semantics.Passes;

public class CheckTypesPass : AbstractPass
{
    public override void Visit(BinaryOperationExpression e)
    {
        base.Visit(e);
        if (e.Left.ResultType != e.Right.ResultType)
        {
            throw new Exception($"Несовпадение типов в операции: {e.Left.ResultType} и {e.Right.ResultType}");
        }

        e.ResultType = IsComparison(e.Operation) ? Runtime.ValueType.Rasklad : e.Left.ResultType;
    }

    private bool IsComparison(BinaryOperation op) => op >= BinaryOperation.Equal && op <= BinaryOperation.GreaterThanOrEqual;
}
