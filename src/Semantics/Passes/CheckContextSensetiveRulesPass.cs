using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;
using Semantics.Exceptions;

namespace Semantics.Passes;

public class CheckContextSensitiveRulesPass : AbstractPass
{
    private int loopDepth = 0;
    private bool isInsideFunction = false;

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);

        if (e.Arguments.Count != e.Function.Parameters.Count)
        {
            throw new InvalidFunctionCallException($"Ошибка: Функция '{e.FunctionName}' ожидает: {e.Function.Parameters.Count} аргументов, получено: {e.Arguments.Count}.");
        }
    }

    public override void Visit(FunctionDeclaration d)
    {
        isInsideFunction = true;
        base.Visit(d);
        isInsideFunction = false;
    }

    public override void Visit(WhileStatement s)
    {
        loopDepth++;
        try
        {
            base.Visit(s);
        }
        finally
        {
            loopDepth--;
        }
    }

    public override void Visit(ForStatement s)
    {
        loopDepth++;
        try
        {
            base.Visit(s);
        }
        finally
        {
            loopDepth--;
        }
    }

    public override void Visit(BreakStatement s)
    {
        base.Visit(s);

        if (loopDepth <= 0)
        {
            throw new InvalidExpressionException("Ошибка: Инструкция 'ХВАТИТ' допустима только внутри цикла.");
        }
    }

    public override void Visit(ContinueStatement s)
    {
        base.Visit(s);

        if (loopDepth <= 0)
        {
            throw new InvalidExpressionException("Ошибка: Инструкция 'ПРОДОЛЖАЕМ' допустима только внутри цикла.");
        }
    }

    public override void Visit(ReturnStatement e)
    {
        base.Visit(e);

        if (!isInsideFunction)
        {
            throw new InvalidExpressionException("Ошибка: Инструкция 'ДРАТУТИ' (возврат) разрешена только внутри функций.");
        }
    }

    public override void Visit(AssignmentExpression e)
    {
        base.Visit(e);

        if (e.Variable is ConstantDeclaration)
        {
            throw new InvalidAssignmentException(
                $"Ошибка: Попытка присвоить значение константе '{e.Name}'. Константы (БАЗА) неизменяемы.");
        }
    }
}