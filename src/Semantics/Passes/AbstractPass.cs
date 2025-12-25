using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

namespace Semantics.Passes;

public abstract class AbstractPass : IAstVisitor
{
    /// <summary>
    /// Объявления (Declarations).
    /// </summary>
    public virtual void Visit(ConstantDeclaration d)
    {
        d.Value.Accept(this);
    }

    public virtual void Visit(VariableDeclaration d)
    {
        if (d.InitialValue != null)
        {
            d.InitialValue.Accept(this);
        }
    }

    public virtual void Visit(FunctionDeclaration d)
    {
        foreach (AstNode s in d.Body.Statements)
        {
            s.Accept(this);
        }
    }

    public virtual void Visit(ParameterDeclaration d)
    {
    }

    /// <summary>
    /// Выражения (Expressions).
    /// </summary>
    public virtual void Visit(LiteralExpression e)
    {
    }

    public virtual void Visit(VariableExpression e)
    {
    }

    public virtual void Visit(FunctionCallExpression e)
    {
        e.Arguments.ForEach(a => a.Accept(this));
    }

    public virtual void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
    }

    public virtual void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
    }

    public virtual void Visit(AssignmentExpression e)
    {
        e.Value.Accept(this);
    }

    /// <summary>
    /// Инструкции (Statements).
    /// </summary>
    public virtual void Visit(BlockStatement s)
    {
        s.Statements.ForEach(node => node.Accept(this));
    }

    public virtual void Visit(IfStatement s)
    {
        s.Condition.Accept(this);
        s.ThenBranch.Accept(this);
        s.ElseBranch?.Accept(this);
    }

    public virtual void Visit(WhileStatement s)
    {
        s.Condition.Accept(this);
        s.Body.Accept(this);
    }

    public virtual void Visit(ForStatement s)
    {
        s.Initializer.Accept(this);
        s.Condition.Accept(this);
        s.Iterator.Accept(this);
        s.Body.Accept(this);

        foreach (AstNode node in s.Body.Statements)
        {
            node.Accept(this);
        }
    }

    public virtual void Visit(ReturnStatement s)
    {
        s.Value?.Accept(this);
    }

    public virtual void Visit(BreakStatement s)
    {
    }

    public virtual void Visit(ContinueStatement s)
    {
    }

    public virtual void Visit(OutputStatement s)
    {
        s.Arguments.ForEach(a => a.Accept(this));
    }

    public virtual void Visit(InputStatement s)
    {
    }

    public virtual void Visit(ExpressionStatement s)
    {
        s.Expression.Accept(this);
    }
}
