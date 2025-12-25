using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Execution.Exceptions;

using Runtime;

using ValueType = Runtime.ValueType;

namespace Execution;

public class AstEvaluator(Context context, IEnvironment environment) : IAstVisitor
{
    private readonly Stack<Value> values = new();

    public Value Evaluate(AstNode node)
    {
        if (values.Count > 0)
        {
            throw new InvalidOperationException("Стек вычислений должен быть пуст.");
        }

        node.Accept(this);

        return values.Count switch
        {
            0 => Value.Void,
            1 => values.Pop(),
            _ => throw new InvalidOperationException("Ошибка логики: на стеке осталось более одного значения.")
        };
    }

    /// <summary>
    /// Объявления (Declarations).
    /// </summary>
    public void Visit(ConstantDeclaration d)
    {
        context.DefineConstant(d.Name, Evaluate(d.Value));
    }

    public void Visit(VariableDeclaration d)
    {
        context.DefineVariable(d.Name, d.InitialValue != null ? Evaluate(d.InitialValue) : Value.Void);
    }

    public void Visit(FunctionDeclaration d)
    {
        context.DefineFunction(d);
    }

    public void Visit(ParameterDeclaration d)
    {
    }

    /// <summary>
    /// Выражения (Expressions).
    /// </summary>
    public void Visit(LiteralExpression e)
    {
        values.Push(e.Value);
    }

    public void Visit(VariableExpression e)
    {
        values.Push(context.GetValue(e.Name));
    }

    public void Visit(FunctionCallExpression e)
    {
        List<Value> argValues = e.Arguments.Select(arg => Evaluate(arg)).ToList();
        FunctionDeclaration function = context.GetCallable(e.FunctionName);

        context.PushScope(new Scope());
        try
        {
            for (int i = 0; i < function.Parameters.Count; i++)
            {
                context.DefineVariable(function.Parameters[i].Name, argValues[i]);
            }

            try
            {
                foreach (AstNode statement in function.Body.Statements)
                {
                    statement.Accept(this);
                    if (values.Count > 0)
                    {
                        values.Pop();
                    }
                }
            }
            catch (ReturnException ret)
            {
                values.Push(ret.ReturnValue);
                return;
            }

            values.Push(Value.Void);
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        Value left = values.Pop();
        e.Right.Accept(this);
        Value right = values.Pop();

        if (left.GetValueType() != right.GetValueType())
        {
            throw new Exception($"Типы не совпадают: {left.GetValueType()} и {right.GetValueType()}");
        }

        values.Push(e.Operation switch
        {
            BinaryOperation.Plus => left.GetValueType() == ValueType.Citata
                ? new Value(left.AsString() + right.AsString())
                : NumericOp(left, right, (a, b) => a + b),
            BinaryOperation.Minus => NumericOp(left, right, (a, b) => a - b),
            BinaryOperation.Multiplication => NumericOp(left, right, (a, b) => a * b),
            BinaryOperation.Division => IsZero(right)
                ? throw new DivideByZeroException("Division by zero")
                : NumericOp(left, right, (a, b) => a / b),
            BinaryOperation.Remainder => IsZero(right)
                ? throw new DivideByZeroException("Division by zero")
                : NumericOp(left, right, (a, b) => a % b),
            BinaryOperation.Equal => new Value(Compare(left, right) == 0),
            BinaryOperation.NotEqual => new Value(Compare(left, right) != 0),
            BinaryOperation.LessThan => new Value(Compare(left, right) < 0),
            BinaryOperation.GreaterThan => new Value(Compare(left, right) > 0),
            BinaryOperation.LessThanOrEqual => new Value(Compare(left, right) <= 0),
            BinaryOperation.GreaterThanOrEqual => new Value(Compare(left, right) >= 0),
            BinaryOperation.And => new Value(IsTruthy(left) && IsTruthy(right)),
            BinaryOperation.Or => new Value(IsTruthy(left) || IsTruthy(right)),
            _ => throw new NotImplementedException()
        });
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
        Value value = values.Pop();

        values.Push(e.Operation switch
        {
            UnaryOperation.Plus => value,
            UnaryOperation.Minus => value.GetValueType() == ValueType.Ciferka
                ? new Value(-value.AsInt())
                : new Value(-value.AsDouble()),
            UnaryOperation.Not => new Value(!value.AsBool()),
            _ => throw new NotImplementedException()
        });
    }

    public void Visit(AssignmentExpression e)
    {
        e.Value.Accept(this);
        Value value = values.Peek();
        context.AssignVariable(e.Name, value);
    }

    /// <summary>
    /// Инструкции (Statements).
    /// </summary>
    public void Visit(BlockStatement s)
    {
        context.PushScope(new Scope());
        try
        {
            foreach (AstNode statement in s.Statements)
            {
                statement.Accept(this);
                if (values.Count > 0)
                {
                    values.Pop();
                }
            }
        }
        finally
        {
            context.PopScope();
        }

        values.Push(Value.Void);
    }

    public void Visit(IfStatement s)
    {
        s.Condition.Accept(this);
        if (values.Pop().AsBool())
        {
            s.ThenBranch.Accept(this);
        }
        else
        {
            s.ElseBranch?.Accept(this);
        }

        if (values.Count > 0)
        {
            values.Pop();
        }

        values.Push(Value.Void);
    }

    public void Visit(WhileStatement s)
    {
        while (true)
        {
            s.Condition.Accept(this);
            if (!values.Pop().AsBool())
            {
                break;
            }

            try
            {
                s.Body.Accept(this);
                if (values.Count > 0)
                {
                    values.Pop();
                }
            }
            catch (BreakException)
            {
                break;
            }
            catch (ContinueException)
            {
                continue;
            }
        }

        values.Push(Value.Void);
    }

    public void Visit(ForStatement s)
    {
        context.PushScope(new Scope());
        try
        {
            s.Initializer.Accept(this);
            if (values.Count > 0)
            {
                values.Pop();
            }

            while (true)
            {
                s.Condition.Accept(this);
                if (!values.Pop().AsBool())
                {
                    break;
                }

                try
                {
                    s.Body.Accept(this);
                    if (values.Count > 0)
                    {
                        values.Pop();
                    }
                }
                catch (BreakException)
                {
                    break;
                }
                catch (ContinueException)
                {
                }

                s.Iterator.Accept(this);
                if (values.Count > 0)
                {
                    values.Pop();
                }
            }
        }
        finally
        {
            context.PopScope();
        }

        values.Push(Value.Void);
    }

    public void Visit(ReturnStatement s)
    {
        throw new ReturnException(s.Value != null ? Evaluate(s.Value) : Value.Void);
    }

    public void Visit(BreakStatement s)
    {
        throw new BreakException();
    }

    public void Visit(ContinueStatement s)
    {
        throw new ContinueException();
    }

    public void Visit(OutputStatement s)
    {
        IEnumerable<string?> messages = s.Arguments.Select(arg =>
        {
            Value val = Evaluate(arg);
            return val.GetValueType() == ValueType.Rasklad
                ? (val.AsBool() ? "ХАЙП" : "КРИНЖ")
                : val.ToString();
        });

        environment.Write(string.Join(" ", messages));
        values.Push(Value.Void);
    }

    public void Visit(InputStatement s)
    {
        foreach (string name in s.VariableNames)
        {
            ValueType targetType = context.GetValue(name).GetValueType();
            context.AssignVariable(name, environment.Read(targetType));
        }

        values.Push(Value.Void);
    }

    public void Visit(ExpressionStatement s)
    {
        s.Expression.Accept(this);
    }

    private int Compare(Value l, Value r)
    {
        return l.GetValueType() switch
        {
            ValueType.Ciferka => l.AsInt().CompareTo(r.AsInt()),
            ValueType.Poltorashka => l.AsDouble().CompareTo(r.AsDouble()),
            ValueType.Citata => string.CompareOrdinal(l.AsString(), r.AsString()),
            _ => throw new InvalidOperationException("Тип не поддерживает сравнение.")
        };
    }

    private Value NumericOp(Value l, Value r, Func<double, double, double> op)
    {
        if (l.GetValueType() == ValueType.Ciferka)
        {
            return new Value((int)op(l.AsInt(), r.AsInt()));
        }

        return new Value(op(l.AsDouble(), r.AsDouble()));
    }

    private bool IsZero(Value value)
    {
        return value.GetValueType() == ValueType.Ciferka
            ? value.AsInt() == 0
            : value.AsDouble() == 0;
    }

    private bool IsTruthy(Value value)
    {
        return value.GetValueType() == ValueType.Ciferka
            ? value.AsInt() > 0
            : value.AsDouble() > 0;
    }
}
