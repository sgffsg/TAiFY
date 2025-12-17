using Ast.Declarations;
using Ast.Expressions;

namespace Ast;

public interface IAstVisitor
{
    void Visit(ConstantDeclaration d);

    void Visit(VariableDeclaration d);

    void Visit(FunctionDeclaration d);

    void Visit(ProcedureDeclaration d);

    void Visit(AssignmentExpression e);

    void Visit(BinaryOperationExpression e);

    void Visit(UnaryOperationExpression e);

    void Visit(CallExpression e);

    void Visit(LiteralExpression e);

    void Visit(BreakExpression e);

    void Visit(ContinueExpression e);

    void Visit(ReturnExpression e);

    void Visit(SequenceExpression e);

    void Visit(InputExpression e);

    void Visit(OutputExpression e);

    void Visit(VariableScopeExpression e);

    void Visit(WhileLoopExpression e);

    void Visit(ForLoopExpression e);

    void Visit(IfElseExpression e);
}