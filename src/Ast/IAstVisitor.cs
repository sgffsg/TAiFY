using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

namespace Ast;

public interface IAstVisitor
{
    /// <summary>
    /// Объявления (Declarations).
    /// </summary>
    void Visit(ConstantDeclaration d);

    void Visit(VariableDeclaration d);

    void Visit(FunctionDeclaration d);

    void Visit(ParameterDeclaration d);

    /// <summary>
    /// Выражения (Expressions).
    /// </summary>
    void Visit(LiteralExpression e);

    void Visit(VariableExpression e);

    void Visit(FunctionCallExpression e);

    void Visit(BinaryOperationExpression e);

    void Visit(UnaryOperationExpression e);

    void Visit(AssignmentExpression e);

    void Visit(IndexExpression e);

    /// <summary>
    /// Инструкции (Statements).
    /// </summary>
    void Visit(BlockStatement s);

    void Visit(IfStatement s);

    void Visit(WhileStatement s);

    void Visit(ForStatement s);

    void Visit(ReturnStatement s);

    void Visit(BreakStatement s);

    void Visit(ContinueStatement s);

    void Visit(OutputStatement s);

    void Visit(InputStatement s);

    void Visit(ExpressionStatement s);
}