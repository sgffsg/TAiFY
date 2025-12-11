using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

namespace Execution;

public class AstEvaluator : IAstVisitor
{
    public void Visit(BinaryOperationExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(UnaryOperationExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(LiteralExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(AssignmentExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(ContinueStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(IfStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(WhileStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(ForStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(ReturnStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(BreakStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(CallStatement e)
    {
        throw new NotImplementedException();
    }

    public void Visit(InputStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(OutputStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(BlockStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(ParameterDeclaration d)
    {
        throw new NotImplementedException();
    }

    public void Visit(FunctionDeclaration d)
    {
        throw new NotImplementedException();
    }

    public void Visit(ProcedureDeclaration d)
    {
        throw new NotImplementedException();
    }

    public void Visit(VariableDeclaration d)
    {
        throw new NotImplementedException();
    }

    public void Visit(ConstantDeclaration d)
    {
        throw new NotImplementedException();
    }
}
