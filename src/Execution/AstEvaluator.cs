using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Execution.Exceptions;

namespace Execution;

public class AstEvaluator : IAstVisitor
{
    private readonly Context context;
    private readonly IEnvironment environment;
    private readonly Stack<double> values = new();

    public AstEvaluator(Context context, IEnvironment environment)
    {
        this.context = context;
        this.environment = environment;
    }

    public double Evaluate(AstNode node)
    {
        values.Clear();
        node.Accept(this);

        if (values.Count == 0)
        {
            return 0.0;
        }

        return values.Pop();
    }

    public void Visit(ConstantDeclaration d)
    {
        d.Value.Accept(this);
        context.DefineConstant(d.Name, values.Pop());
    }

    public void Visit(FunctionDeclaration d)
    {
        context.DefineFunction(d);
    }

    public void Visit(ProcedureDeclaration d)
    {
        context.DefineProcedure(d);
    }

    public void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
        double right = values.Pop();
        double left = values.Pop();

        values.Push(e.Operation switch
        {
            BinaryOperation.Plus => left + right,
            BinaryOperation.Minus => left - right,
            BinaryOperation.Multiplication => left * right,
            BinaryOperation.Division => right == 0 ? throw new DivideByZeroException("Division by zero") : left / right,
            BinaryOperation.Remainder => right == 0 ? throw new DivideByZeroException("Remainder by zero") : left % right,
            BinaryOperation.Equal => left == right ? 1.0 : 0.0,
            BinaryOperation.NotEqual => left != right ? 1.0 : 0.0,
            BinaryOperation.LessThan => left < right ? 1.0 : 0.0,
            BinaryOperation.GreaterThan => left > right ? 1.0 : 0.0,
            BinaryOperation.LessThanOrEqual => (left <= right) || (right == left) ? 1.0 : 0.0,
            BinaryOperation.GreaterThanOrEqual => (right <= left) || (right == left) ? 1.0 : 0.0,
            BinaryOperation.And => (left > 0 && right > 0) ? 1.0 : 0.0,
            BinaryOperation.Or => (left > 0 || right > 0) ? 1.0 : 0.0,
            _ => throw new NotImplementedException($"Оператор {e.Operation} не поддерживается")
        });
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
        double value = values.Pop();

        values.Push(e.Operation switch
        {
            UnaryOperation.Plus => value,
            UnaryOperation.Minus => -value,
            UnaryOperation.Not => (value == 0.0) ? 1.0 : 0.0,

            _ => throw new NotImplementedException($"Оператор {e.Operation} не поддерживается")
        });
    }

    public void Visit(CallExpression e)
    {
        List<double> evaluatedArgs = new();
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
            evaluatedArgs.Add(values.Pop());
        }

        try
        {
            double builtinResult = BuiltinFunctions.Instance.Invoke(e.Name, evaluatedArgs);
            values.Push(builtinResult);
            return;
        }
        catch (ArgumentException ex) when (ex.Message.Contains("Unknown builtin function"))
        {
        }

        Declaration declaration = context.GetCallable(e.Name);

        List<Parameter> parameters;
        BlockStatement body;

        if (declaration is FunctionDeclaration func)
        {
            parameters = func.Parameters;
            body = func.Body;
        }
        else if (declaration is ProcedureDeclaration proc)
        {
            parameters = proc.Parameters;
            body = proc.Body;
        }
        else
        {
            throw new InvalidOperationException($"'{e.Name}' не является вызываемым объектом.");
        }

        if (evaluatedArgs.Count != parameters.Count)
        {
            throw new ArgumentException($"'{e.Name}' ожидает {parameters.Count} аргументов, получено {evaluatedArgs.Count}");
        }

        context.PushScope(new Scope());

        try
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                context.DefineVariable(parameters[i].Name, evaluatedArgs[i]);
            }

            try
            {
                body.Accept(this);
                values.Push(0.0);
            }
            catch (ReturnException ex)
            {
                values.Push((double)ex.Value);
            }
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(AssignmentExpression e)
    {
        e.Value.Accept(this);
        double value = values.Peek();
        context.AssignVariable(e.Name, value);
    }

    public void Visit(AssignmentStatement e)
    {
        e.Value.Accept(this);
        double value = values.Pop();
        context.AssignVariable(e.Name, value);
    }

    public void Visit(LiteralExpression e)
    {
        values.Push(e.Value);
    }

    public void Visit(VariableExpression e)
    {
        values.Push(context.GetValue(e.Name));
    }

    public void Visit(BlockStatement e)
    {
        foreach (Statement s in e.Statements)
        {
            s.Accept(this);

            while (values.Count > 0)
            {
                values.Pop();
            }
        }
    }

    public void Visit(BreakStatement e)
    {
        throw new BreakException();
    }

    public void Visit(ContinueStatement e)
    {
        throw new ContinueException();
    }

    public void Visit(ReturnStatement e)
    {
        double result = 0;
        if (e.Value != null)
        {
            e.Value.Accept(this);
            result = values.Pop();
        }

        throw new ReturnException(result);
    }

    public void Visit(ExpressionStatement e)
    {
        e.Expression.Accept(this);
    }

    public void Visit(ForStatement e)
    {
        e.Initializer.Accept(this);
        while(true)
        {
            if (e.Condition != null)
            {
                e.Condition.Accept(this);
                if (values.Pop() <= 0)
                {
                    break;
                }
            }

            try
            {
                e.Body.Accept(this);
            }
            catch (BreakException)
            {
                break;
            }
            catch (ContinueException)
            {
            }

            e.Iterator.Accept(this);
        }
    }

    public void Visit(IfStatement e)
    {
        e.Condition.Accept(this);
        if (values.Pop() > 0)
        {
            e.ThenBranch.Accept(this);
        }
        else
        {
            e.ElseBranch?.Accept(this);
        }
    }

    public void Visit(InputStatement e)
    {
        foreach(string name in e.VariableNames)
        {
            double value = environment.ReadNumber();
            context.AssignVariable(name, value);
        }
    }

    public void Visit(OutputStatement e)
    {
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);

            double value = values.Pop();
            environment.Write(value);
        }
    }

    public void Visit(VariableDeclarationStatement e)
    {
        double initialValue = 0.0;
        if (e.InitialValue != null)
        {
            e.InitialValue.Accept(this);
            initialValue = values.Pop();
        }

        context.DefineVariable(e.Name, initialValue);
    }

    public void Visit(WhileStatement e)
    {
        while (true)
        {
            e.Condition.Accept(this);
            if (values.Pop() <= 0)
            {
                break;
            }

            try
            {
                e.Body.Accept(this);
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
    }
}
