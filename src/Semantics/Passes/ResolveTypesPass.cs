using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Runtime;

using Semantics.Exceptions;

using ValueType = Runtime.ValueType;

namespace Semantics.Passes;

public sealed class ResolveTypesPass : AbstractPass
{
    private ValueType? expectedReturnType;
    private bool hasGuaranteedReturn;

    /// <summary>
    /// Литерал всегда имеет определённый тип.
    /// </summary>
    public override void Visit(LiteralExpression e)
    {
        base.Visit(e);
        e.ResultType = e.Value.GetValueType();
    }

    /// <summary>
    /// Выполняет проверки типов для бинарных операций:
    /// 1. Арифметические и логические операции выполняются над целыми числами и возвращают число.
    /// 2. Операции сравнения выполняются над двумя числами либо двумя строками и возвращают тот же тип.
    /// </summary>
    public override void Visit(BinaryOperationExpression e)
    {
        base.Visit(e);

        ValueType left = e.Left.ResultType;
        ValueType right = e.Right.ResultType;

        if (left != right)
        {
            throw new TypeErrorException($"Ошибка: Несоответствие типов {left} и {right} в операции {e.Operation}.");
        }

        ValueType? resultType = GetBinaryOperationResultType(e.Operation, left, right);
        if (resultType == null)
        {
            throw new TypeErrorException($"Ошибка: Операция {e.Operation} не поддерживается для типа {left}.");
        }

        e.ResultType = resultType.Value;
    }

    /// <summary>
    /// Выполняет проверки типов для унарного минуса.
    /// Унарный минус применяется только к числам и возвращает число.
    /// </summary>
    public override void Visit(UnaryOperationExpression e)
    {
        base.Visit(e);

        ValueType operandType = e.Operand.ResultType;
        switch (e.Operation)
        {
            case UnaryOperation.Not:
                if (operandType != ValueType.РАСКЛАД)
                {
                    throw new TypeErrorException(
                        $"Оператор 'НЕ' применим только к типу РАСКЛАД, но получен {operandType}."
                    );
                }

                e.ResultType = ValueType.РАСКЛАД;
                break;

            case UnaryOperation.Minus:
            case UnaryOperation.Plus:
                if (operandType != ValueType.ЦИФЕРКА && operandType != ValueType.ПОЛТОРАШКА)
                {
                    throw new TypeErrorException(
                        $"Унарный оператор '{e.Operation}' применим только к числам, получен {operandType}."
                    );
                }

                e.ResultType = operandType;
                break;

            default:
                throw new ArgumentException($"Неизвестная унарная операция: {e.Operation}");
        }
    }

    public override void Visit(VariableExpression e)
    {
        base.Visit(e);
        e.ResultType = e.Variable.ResultType;
    }

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);

        ValueType valueType = d.InitialValue.ResultType;
        ValueType declaredType = d.ResultType;

        if (declaredType != valueType)
        {
            throw new TypeErrorException($"Ошибка инициализации: переменная '{d.Name}' объявлена как {declaredType}, но ей присваивается значение типа {valueType}.");
        }
    }

    public override void Visit(ConstantDeclaration d)
    {
        base.Visit(d);

        ValueType valueType = d.Value.ResultType;
        ValueType declaredType = d.ResultType;

        if (declaredType != valueType)
        {
            throw new TypeErrorException($"Ошибка инициализации: константа '{d.Name}' объявлена как {declaredType}, но ей присваивается значение типа {valueType}.");
        }
    }

    public override void Visit(AssignmentExpression e)
    {
        base.Visit(e);
        ValueType variableResultType = e.Variable.ResultType;
        ValueType valueResultType = e.Value.ResultType;

        if (variableResultType != valueResultType)
        {
            throw new TypeErrorException($"Нельзя присвоить значение типа {valueResultType} переменной типа {variableResultType}.");
        }

        e.ResultType = ValueType.Void;
    }

    public override void Visit(IfStatement s)
    {
        base.Visit(s);

        if (s.Condition.ResultType != ValueType.РАСКЛАД)
        {
            throw new TypeErrorException("Условие в 'ЕСЛИ' должно иметь тип РАСКЛАД.");
        }
    }

    public override void Visit(FunctionDeclaration d)
    {
        hasGuaranteedReturn = false;
        expectedReturnType = d.ResultType;

        base.Visit(d);

        if (expectedReturnType != ValueType.Void && !hasGuaranteedReturn && !IsReturnGuaranteed(d.Body))
        {
            throw new TypeErrorException($"Функция '{d.Name}' обязана возвращать {expectedReturnType} на всех путях выполнения.");
        }

        expectedReturnType = null;
    }

    public override void Visit(WhileStatement s)
    {
        base.Visit(s);
        if (s.Condition.ResultType != ValueType.РАСКЛАД)
        {
            throw new TypeErrorException("Условие в 'ПОКА' должно иметь тип РАСКЛАД.");
        }
    }

    public override void Visit(ReturnStatement e)
    {
        base.Visit(e);
        ValueType actual = e.Value?.ResultType ?? ValueType.Void;

        if (expectedReturnType == null)
        {
            throw new TypeErrorException("Инструкция 'ДРАТУТИ' использована вне функции.");
        }

        if (actual != expectedReturnType)
        {
            throw new TypeErrorException($"Попытка вернуть {actual} из функции, возвращающей {expectedReturnType}.");
        }

        hasGuaranteedReturn = true;
    }

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        IReadOnlyList<AbstractParameterDeclaration> parameters = e.Function.Parameters;

        if (e.Arguments.Count != parameters.Count)
        {
            throw new TypeErrorException($"Неверное количество аргументов для функции {e.FunctionName}.");
        }

        for (int i = 0; i < parameters.Count; i++)
        {
            if (e.Arguments[i].ResultType != parameters[i].ResultType)
            {
                throw new TypeErrorException(
                    $"Аргумент #{i + 1} функции {e.FunctionName} должен быть {parameters[i].ResultType}, а не {e.Arguments[i].ResultType}.");
            }
        }

        e.ResultType = e.Function.ResultType;
    }

    private static ValueType? GetBinaryOperationResultType(BinaryOperation operation, ValueType left, ValueType right)
    {
        if (left != right)
        {
            return null;
        }

        switch (operation)
        {
            case BinaryOperation.Plus:
                if (left == ValueType.ЦИФЕРКА || left == ValueType.ПОЛТОРАШКА || left == ValueType.ЦИТАТА)
                {
                    return left;
                }

                return null;
            case BinaryOperation.Minus:
            case BinaryOperation.Multiplication:
            case BinaryOperation.Division:
                if (left == ValueType.ЦИФЕРКА || left == ValueType.ПОЛТОРАШКА)
                {
                    return left;
                }

                return null;
            case BinaryOperation.Remainder:
                if (left == ValueType.ЦИФЕРКА)
                {
                    return ValueType.ЦИФЕРКА;
                }

                return null;

            case BinaryOperation.LessThan:
            case BinaryOperation.GreaterThan:
            case BinaryOperation.LessThanOrEqual:
            case BinaryOperation.GreaterThanOrEqual:
            case BinaryOperation.Equal:
            case BinaryOperation.NotEqual:
                return ValueType.РАСКЛАД;

            case BinaryOperation.And:
            case BinaryOperation.Or:
                if (left == ValueType.РАСКЛАД)
                {
                    return ValueType.РАСКЛАД;
                }

                return null;

            default:
                throw new ArgumentException($"Недопустимая бинарная операция {operation}");
        }
    }

    private bool IsReturnGuaranteed(AstNode node)
    {
        if (node is ReturnStatement)
        {
            return true;
        }

        if (node is BlockStatement block)
        {
            return HasGuaranteedReturnInList(block.Statements);
        }

        if (node is IfStatement ifs && ifs.ElseBranch != null)
        {
            if (IsReturnGuaranteed(ifs.ThenBranch) && IsReturnGuaranteed(ifs.ElseBranch))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasGuaranteedReturnInList(List<AstNode> nodes)
    {
        foreach (AstNode node in nodes)
        {
            if (IsReturnGuaranteed(node))
            {
                return true;
            }
        }

        return false;
    }
}
