using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

namespace Semantics.Passes;

public class CheckContextSensitiveRulesPass : AbstractPass
{
    private int loopDepth = 0;
    private readonly bool inFunction = false;

    public override void Visit(AssignmentExpression s)
    {
        base.Visit(s);
        if (s.Variable is ConstantDeclaration)
        {
            throw new Exception($"Ошибка: Нельзя изменять константу '{s.Name}' (БАЗА)");
        }
    }

    public override void Visit(WhileStatement s)
    {
        loopDepth++;
        base.Visit(s);
        loopDepth--;
    }

    public override void Visit(BreakStatement s)
    {
        if (loopDepth == 0)
        {
            throw new Exception("ХВАТИТ (break) можно использовать только внутри цикла");
        }
    }

    public override void Visit(ReturnStatement s)
    {
        if (!inFunction)
        {
            throw new Exception("ДРАТУТИ (return) нельзя использовать вне функции");
        }

        base.Visit(s);
    }
}