using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

namespace Ast;

public interface IAstVisitor
{
    void Visit(ConstantDeclaration d);

    void Visit(VariableDeclaration d);

    void Visit(ParameterDeclaration d);

    void Visit(FunctionDeclaration d);

    void Visit(ProcedureDeclaration d);

    void Visit(BinaryOperationExpression e);

    void Visit(UnaryOperationExpression e);

    void Visit(CallExpression e);

    void Visit(IdentifierExpression e);

    void Visit(LiteralExpression e);

    void Visit(VariableAssignmentStatement s);

    void Visit(ExpressionStatement s);

    void Visit(BlockStatement s);

    void Visit(IfStatement s);

    void Visit(ForStatement s);

    void Visit(WhileStatement s);

    void Visit(BreakStatement s);

    void Visit(ContinueStatement s);

    void Visit(ReturnStatement s);

    void Visit(InputStatement s);

    void Visit(OutputStatement s);
}