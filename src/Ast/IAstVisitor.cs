using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

namespace Ast;

public interface IAstVisitor
{
    void Visit(BinaryOperationExpression e);

    void Visit(UnaryOperationExpression e);

    void Visit(LiteralExpression e);

    void Visit(IdentifierExpression e);

    void Visit(AssignmentExpression e);

    void Visit(ContinueStatement s);

    void Visit(IfStatement s);

    void Visit(WhileStatement s);

    void Visit(ForStatement s);

    void Visit(ReturnStatement s);

    void Visit(BreakStatement s);

    void Visit(CallStatement e);

    void Visit(InputStatement s);

    void Visit(OutputStatement s);

    void Visit(BlockStatement s);

    void Visit(VariableDeclarationStatement s);

    void Visit(ParameterDeclaration d);

    void Visit(FunctionDeclaration d);

    void Visit(ProcedureDeclaration d);

    void Visit(VariableDeclaration d);

    void Visit(ConstantDeclaration d);
}
