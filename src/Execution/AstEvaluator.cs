using Ast;
using Ast.Declarations;
using Ast.Expressions;

namespace Execution;

public class AstEvaluator : IAstVisitor
{
    public void Visit(ConstantDeclaration d)
    {
        throw new NotImplementedException();
    }

    public void Visit(VariableDeclaration d)
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

    public void Visit(AssignmentExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(BinaryOperationExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(UnaryOperationExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(CallExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(LiteralExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(BreakExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(ContinueExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(ReturnExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(SequenceExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(InputExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(OutputExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(VariableScopeExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(WhileLoopExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(ForLoopExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(IfElseExpression e)
    {
        throw new NotImplementedException();
    }
}
