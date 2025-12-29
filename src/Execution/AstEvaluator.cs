using System;

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
        node.Accept(this);

        return values.Count > 0
            ? values.Pop()
            : Value.Void;
    }

    /// <summary>
    /// Объявления (Declarations).
    /// </summary>
    public void Visit(ConstantDeclaration d)
    {
        context.DefineConstant(d.Name, Evaluate(d.Value));
        values.Push(Value.Void);
    }

    public void Visit(VariableDeclaration d)
    {
        context.DefineVariable(d.Name, Evaluate(d.InitialValue));
        values.Push(Value.Void);
    }

    public void Visit(FunctionDeclaration d)
    {
        context.DefineFunction(d);
        values.Push(Value.Void);
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
        List<Value> arguments = e.Arguments.Select(arg => Evaluate(arg)).ToList();

        BuiltinFunction? builtins = Builtins.Functions.FirstOrDefault(f => f.Name == e.FunctionName);
        if (builtins != null)
        {
            values.Push(builtins.Invoke(arguments));
            return;
        }

        FunctionDeclaration function = context.GetCallable(e.FunctionName);
        context.PushScope(new Scope());
        try
        {
            for (int i = 0; i < function.Parameters.Count; i++)
            {
                context.DefineVariable(function.Parameters[i].Name, arguments[i]);
            }

            function.Body.Accept(this);
            values.Push(Value.Void);
        }
        catch (ReturnException ret)
        {
            values.Push(ret.ReturnValue);
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(BinaryOperationExpression e)
    {
        Value left = Evaluate(e.Left);
        Value right = Evaluate(e.Right);

        if (left.GetValueType() != right.GetValueType())
        {
            throw new Exception($"Несоответствие типов: {left.GetValueType()} и {right.GetValueType()}");
        }

        values.Push(ExecuteBinaryExpression(e.Operation, left, right));
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
        Value value = values.Pop();

        values.Push(e.Operation switch
        {
            UnaryOperation.Plus => value,
            UnaryOperation.Minus => value.GetValueType() == ValueType.ЦИФЕРКА
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

    public void Visit(IndexExpression e)
    {
        Value target = Evaluate(e.Target);
        Value index = Evaluate(e.Index);

        string str = target.AsString();
        int idx = index.AsInt();
        values.Push(new Value(str[idx].ToString()));
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
        if (Evaluate(s.Condition).AsBool())
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
        while (Evaluate(s.Condition).AsBool())
        {
            try
            {
                s.Body.Accept(this);
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
            Evaluate(s.Initializer);
            while (Evaluate(s.Condition).AsBool())
            {
                try
                {
                    s.Body.Accept(this);
                }
                catch (BreakException)
                {
                    break;
                }
                catch (ContinueException)
                {
                    continue;
                }

                Evaluate(s.Iterator);
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
        IEnumerable<string> outputs = s.Arguments.Select(a => Evaluate(a).ToString());
        environment.Write(string.Join("", outputs));
        values.Push(Value.Void);
    }

    public void Visit(InputStatement s)
    {
        foreach (string name in s.VariableNames)
        {
            ValueType variableType = context.GetValue(name).GetValueType();
            context.AssignVariable(name, environment.Read(variableType));
        }

        values.Push(Value.Void);
    }

    public void Visit(ExpressionStatement s)
    {
        Evaluate(s.Expression);
        values.Push(Value.Void);
    }

    private Value ExecuteBinaryExpression(BinaryOperation operation, Value left, Value right)
    {
        ValueType type = left.GetValueType();

        return operation switch
        {
            BinaryOperation.Plus when type == ValueType.ЦИТАТА => new Value(left.AsString() + right.AsString()),
            BinaryOperation.Plus => ProcessNumericOperation(left, right, operation),
            BinaryOperation.Minus => ProcessNumericOperation(left, right, operation),
            BinaryOperation.Multiplication => ProcessNumericOperation(left, right, operation),
            BinaryOperation.Division => ProcessNumericOperation(left, right, operation),
            BinaryOperation.Remainder => ProcessNumericOperation(left, right, operation),
            BinaryOperation.And => new Value(left.AsBool() && right.AsBool()),
            BinaryOperation.Or => new Value(left.AsBool() || right.AsBool()),
            BinaryOperation.Equal => new Value(ProcessLogicalOperation(left, right) == 0),
            BinaryOperation.NotEqual => new Value(ProcessLogicalOperation(left, right) != 0),
            BinaryOperation.LessThan => new Value(ProcessLogicalOperation(left, right) < 0),
            BinaryOperation.GreaterThan => new Value(ProcessLogicalOperation(left, right) > 0),
            BinaryOperation.LessThanOrEqual => new Value(ProcessLogicalOperation(left, right) <= 0),
            BinaryOperation.GreaterThanOrEqual => new Value(ProcessLogicalOperation(left, right) >= 0),

            _ => throw new NotImplementedException()
        };
    }

    private Value ProcessNumericOperation(Value l, Value r, BinaryOperation operation)
    {
        ValueType type = l.GetValueType();
        if (type == ValueType.ЦИФЕРКА)
        {
            int lValue = l.AsInt();
            int rValue = r.AsInt();

            switch (operation)
            {
                case BinaryOperation.Plus:
                    return new Value(lValue + rValue);

                case BinaryOperation.Minus:
                    return new Value(lValue - rValue);

                case BinaryOperation.Multiplication:
                    return new Value(lValue * rValue);

                case BinaryOperation.Division:
                    if (rValue == 0)
                    {
                        throw new DivideByZeroException("Division by zero");
                    }

                    return new Value(lValue / rValue);

                case BinaryOperation.Remainder:
                    if (rValue == 0)
                    {
                        throw new DivideByZeroException("Remainder by zero");
                    }

                    return new Value(lValue % rValue);
            }
        }
        else if (type == ValueType.ПОЛТОРАШКА)
        {
            double lValue = l.AsDouble();
            double rValue = r.AsDouble();

            switch (operation)
            {
                case BinaryOperation.Plus:
                    return new Value(lValue + rValue);

                case BinaryOperation.Minus:
                    return new Value(lValue - rValue);

                case BinaryOperation.Multiplication:
                    return new Value(lValue * rValue);

                case BinaryOperation.Division:
                    if (rValue == 0)
                    {
                        throw new DivideByZeroException("Division by zero");
                    }

                    return new Value(lValue / rValue);

                case BinaryOperation.Remainder:
                    throw new NotImplementedException($"Оператор % запрещен для типа {type}");
            }
        }

        throw new NotImplementedException($"Неопределенное выполнение арифметической операции для типа {type}.");
    }

    private int ProcessLogicalOperation(Value l, Value r)
    {
        ValueType type = l.GetValueType();
        return type switch
        {
            ValueType.ЦИФЕРКА => l.AsInt().CompareTo(r.AsInt()),
            ValueType.ПОЛТОРАШКА => l.AsDouble().CompareTo(r.AsDouble()),
            ValueType.ЦИТАТА => string.CompareOrdinal(l.AsString(), r.AsString()),
            _ => throw new InvalidOperationException($"Неопределенное выполнение арифметической операции для типа {type}.")
        };
    }
}
