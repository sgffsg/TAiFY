using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;
using Lexer;

using Runtime;

namespace Parser;

public class Parser
{
    private readonly TokenStream tokens;

    public Parser(string code)
    {
        tokens = new TokenStream(code);
    }

    /// <summary>
    /// Основная точка входа. Собирает программу в список узлов AST.
    /// </summary>
    public List<AstNode> ParseProgram()
    {
        List<AstNode> nodes = new();
        while (tokens.Peek().Type != TokenType.EOF)
        {
            nodes.Add(ParseTopLevelItem());

            if (tokens.Peek().Type == TokenType.Semicolon)
            {
                tokens.Advance();
            }
        }

        return nodes;
    }

    /// <summary>
    /// topLevelItem =
    ///     procedureDeclaration | constantDeclaration | typedDeclaration.
    /// </summary>
    private AstNode ParseTopLevelItem()
    {
        Token token = tokens.Peek();
        return token.Type switch
        {
            TokenType.Baza => ParseConstantDeclaration(),
            TokenType.Prokrastiniryem => ParseProcedureDeclaration(),
            _ when IsTypeName(token) => ParseTypedDeclaration(),
            _ => ParseStatement()
        };
    }

    /// <summary>
    /// constantDeclaration =
    ///     "БАЗА", typeName, identifier, "=", expression, ";".
    /// </summary>
    private ConstantDeclaration ParseConstantDeclaration()
    {
        Match(TokenType.Baza);
        string typeName = ParseTypeName();
        string name = ParseIdentifier();
        Match(TokenType.Assignment);
        Expression value = ParseExpression();
        Match(TokenType.Semicolon);

        return new ConstantDeclaration(typeName, name, value);
    }

    /// <summary>
    /// typedDeclaration =
    ///     typeName, identifier, (functionTail | variableTail ) ;.
    /// </summary>
    private AstNode ParseTypedDeclaration()
    {
        string typeName = ParseTypeName();
        string identifier = ParseIdentifier();

        if (tokens.Peek().Type == TokenType.OpenParenthesis)
        {
            return ParseFunctionTail(typeName, identifier);
        }

        return ParseVariableTail(typeName, identifier);
    }

    /// <summary>
    /// functionTail =
    ///     "(", [parameterList], ")", block ;.
    /// </summary>
    private FunctionDeclaration ParseFunctionTail(string returnType, string functionName)
    {
        Match(TokenType.OpenParenthesis);
        List<ParameterDeclaration> parameters = ParseParameterList();
        Match(TokenType.CloseParenthesis);
        BlockStatement body = ParseBlock();

        return new FunctionDeclaration(functionName, parameters, returnType, body);
    }

    /// <summary>
    /// variableTail =
    ///     "=", expression, ";".
    /// </summary>
    private VariableDeclaration ParseVariableTail(string typeName, string variableName)
    {
        Match(TokenType.Assignment);
        Expression initialValue = ParseExpression();
        Match(TokenType.Semicolon);

        return new VariableDeclaration(typeName, variableName, initialValue);
    }

    /// <summary>
    /// procedureDeclaration =
    ///     "ПРОКРАСТИНИРУЕМ", identifier, "(", [parameterList], ")", block.
    /// </summary>
    private FunctionDeclaration ParseProcedureDeclaration()
    {
        Match(TokenType.Prokrastiniryem);
        string name = ParseIdentifier();
        Match(TokenType.OpenParenthesis);
        List<ParameterDeclaration> parameters = ParseParameterList();
        Match(TokenType.CloseParenthesis);
        BlockStatement body = ParseBlock();

        return new FunctionDeclaration(name, parameters, null, body);
    }

    /// <summary>
    /// parameterList =
    ///     identifier, ":", typeName, { ",", identifier, ":", typeName };.
    /// </summary>
    private List<ParameterDeclaration> ParseParameterList()
    {
        List<ParameterDeclaration> parameters = new();
        if (tokens.Peek().Type == TokenType.CloseParenthesis)
        {
            return parameters;
        }

        do
        {
            string name = ParseIdentifier();
            Match(TokenType.ColonTypeIndication);
            string type = ParseTypeName();

            parameters.Add(new ParameterDeclaration(type, name));
            if (tokens.Peek().Type != TokenType.Comma)
            {
                break;
            }

            tokens.Advance();
        }
        while (true);

        return parameters;
    }

    /// <summary>
    /// block = "ПОЕХАЛИ", { statement }, "ФИНАЛОЧКА" ;.
    /// </summary>
    private BlockStatement ParseBlock()
    {
        Match(TokenType.Poehali);
        List<AstNode> statements = new();

        while (tokens.Peek().Type != TokenType.Finalochka &&
               tokens.Peek().Type != TokenType.EOF)
        {
            statements.Add(ParseStatement());
        }

        Match(TokenType.Finalochka);
        return new BlockStatement(statements);
    }

    /// <summary>
    /// statement =
    ///     variableDeclaration | ifStatement | whileStatement | forStatement | returnStatement | breakStatement
    ///      | continueStatement | ioStatement | block | sideEffectStatement | ";" ;.
    /// </summary>
    private AstNode ParseStatement()
    {
        Token token = tokens.Peek();
        switch (token.Type)
        {
            case TokenType.Ciferka:
            case TokenType.Poltorashka:
            case TokenType.Citata:
            case TokenType.Rasklad:
                return ParseVariableDeclaration();

            case TokenType.Esli:
                return ParseIfStatement();

            case TokenType.Cikl:
                return ParseForStatement();

            case TokenType.Poka:
                return ParseWhileStatement();

            case TokenType.Dratuti:
                return ParseReturnStatement();

            case TokenType.Hvatit:
                return ParseBreakStatement();

            case TokenType.Prodolzhaem:
                return ParseContinueStatement();

            case TokenType.Vbros:
                return ParseInputStatement();

            case TokenType.Vybros:
                return ParseOutputStatement();

            case TokenType.Poehali:
                return ParseBlock();

            case TokenType.Identifier:
                return ParseSideEffectStatement();

            default:
                ExpressionStatement result = new ExpressionStatement(ParseExpression());
                Match(TokenType.Semicolon);
                return result;
        }
    }

    /// <summary>
    /// variableDeclaration =
    ///     typeName, identifier, "=", expression, ";" ;.
    /// </summary>
    private VariableDeclaration ParseVariableDeclaration()
    {
        string type = ParseTypeName();
        string name = ParseIdentifier();
        Match(TokenType.Assignment);
        Expression value = ParseExpression();
        Match(TokenType.Semicolon);

        return new VariableDeclaration(type, name, value);
    }

    /// <summary>
    /// variableAssignment = identifier, "=", expression ;.
    /// </summary>
    private AssignmentExpression ParseAssignmentExpression()
    {
        string identifier = ParseIdentifier();
        Match(TokenType.Assignment);
        Expression value = ParseExpression();

        return new AssignmentExpression(identifier, value);
    }

    /// <summary>
    /// ifStatement =
    ///     "ЕСЛИ", "(", expression, ")", "ТО", statement,
    ///     ["ИНАЧЕ", statement] ;.
    /// </summary>
    private IfStatement ParseIfStatement()
    {
        Match(TokenType.Esli);
        Match(TokenType.OpenParenthesis);
        Expression condition = ParseExpression();
        Match(TokenType.CloseParenthesis);
        Match(TokenType.To);

        AstNode thenBranch = ParseStatement();
        AstNode? elseBranch = null;
        if (tokens.Peek().Type == TokenType.Inache)
        {
            Match(TokenType.Inache);
            elseBranch = ParseStatement();
        }

        return new IfStatement(condition, thenBranch, elseBranch);
    }

    /// <summary>
    /// whileStatement =
    ///     "ПОКА", "(", expression, ")", block ;.
    /// </summary>
    private WhileStatement ParseWhileStatement()
    {
        Match(TokenType.Poka);
        Match(TokenType.OpenParenthesis);
        Expression condition = ParseExpression();
        Match(TokenType.CloseParenthesis);

        BlockStatement body = tokens.Peek().Type == TokenType.Poehali
            ? ParseBlock()
            : new BlockStatement(new List<AstNode> { ParseStatement() });

        return new WhileStatement(condition, body);
    }

    /// <summary>
    /// forStatement =
    ///     "ЦИКЛ", "(", variableAssignment, ";", expression, ";", variableAssignment, ")", block ;.
    /// </summary>
    private ForStatement ParseForStatement()
    {
        Match(TokenType.Cikl);
        Match(TokenType.OpenParenthesis);

        AssignmentExpression init = ParseAssignmentExpression();
        Match(TokenType.Semicolon);

        Expression condition = ParseExpression();
        Match(TokenType.Semicolon);

        AssignmentExpression iterator = ParseAssignmentExpression();
        Match(TokenType.CloseParenthesis);

        BlockStatement body = tokens.Peek().Type == TokenType.Poehali
            ? ParseBlock()
            : new BlockStatement(new List<AstNode> { ParseStatement() });
        return new ForStatement(init, condition, iterator, body);
    }

    /// <summary>
    /// returnStatement = "ДРАТУТИ", [ expression ], ";" ;.
    /// </summary>
    private ReturnStatement ParseReturnStatement()
    {
        Match(TokenType.Dratuti);
        Expression? returnValue = null;
        if (tokens.Peek().Type != TokenType.Semicolon)
        {
            returnValue = ParseExpression();
        }

        Match(TokenType.Semicolon);
        return new ReturnStatement(returnValue);
    }

    /// <summary>
    /// breakStatement = "ХВАТИТ", ";" ;.
    /// </summary>
    private BreakStatement ParseBreakStatement()
    {
        Match(TokenType.Hvatit);
        Match(TokenType.Semicolon);

        return new BreakStatement();
    }

    /// <summary>
    /// continueStatement = "ПРОДОЛЖАЕМ", ";" ;.
    /// </summary>
    private ContinueStatement ParseContinueStatement()
    {
        Match(TokenType.Prodolzhaem);
        Match(TokenType.Semicolon);

        return new ContinueStatement();
    }

    /// <summary>
    /// inputStatement = "ВБРОС", "(", identifier, { ",", identifier }, ")", ";".
    /// </summary>
    private InputStatement ParseInputStatement()
    {
        Match(TokenType.Vbros);
        Match(TokenType.OpenParenthesis);

        List<string> names = new() { ParseIdentifier() };
        while (tokens.Peek().Type == TokenType.Comma)
        {
            Match(TokenType.Comma);
            names.Add(ParseIdentifier());
        }

        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);
        return new InputStatement(names);
    }

    /// <summary>
    /// outputStatement = "ВЫБРОС", "(", [ argumentList ], ")", ";".
    /// </summary>
    private OutputStatement ParseOutputStatement()
    {
        Match(TokenType.Vybros);
        Match(TokenType.OpenParenthesis);

        List<Expression> args = new();
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            args.Add(ParseExpression());
            while (tokens.Peek().Type == TokenType.Comma)
            {
                Match(TokenType.Comma);
                args.Add(ParseExpression());
            }
        }

        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);
        return new OutputStatement(args);
    }

    /// <summary>
    /// sideEffectStatement = identifier, ( assignmentTail | callTail ), ";" ;.
    /// </summary>
    private Expression ParseSideEffectStatement()
    {
        string identifier = ParseIdentifier();
        if (tokens.Peek().Type == TokenType.Assignment)
        {
            return new ExpressionStatement(ParseAssignmentTail(identifier));
        }
        else if (tokens.Peek().Type == TokenType.OpenParenthesis)
        {
            return new ExpressionStatement(ParseCallTail(identifier));
        }
        else
        {
            throw new UnexpectedLexemeException("ожидается '=' или '('", tokens.Peek());
        }
    }

    /// <summary>
    /// assignmentTail = "=", expression ;.
    /// </summary>
    private AssignmentExpression ParseAssignmentTail(string identifier)
    {
        Match(TokenType.Assignment);
        Expression value = ParseExpression();

        return new AssignmentExpression(identifier, value);
    }

    /// <summary>
    /// callTail = "(", [ argumentList ], ")" ;.
    /// </summary>
    private FunctionCallExpression ParseCallTail(string functionName)
    {
        Match(TokenType.OpenParenthesis);
        List<Expression> args = new();

        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            args.Add(ParseExpression());
            while (tokens.Peek().Type == TokenType.Comma)
            {
                Match(TokenType.Comma);
                args.Add(ParseExpression());
            }
        }

        Match(TokenType.CloseParenthesis);
        return new FunctionCallExpression(functionName, args);
    }

    /// <summary>
    /// Парсит список значений, разделенных запятыми.
    /// </summary>
    private List<Expression> ParseArgumentList()
    {
        List<Expression> values = new() { ParseExpression() };
        while (tokens.Peek().Type == TokenType.Comma)
        {
            tokens.Advance();
            values.Add(ParseExpression());
        }

        return values;
    }

    /// <summary>
    /// Выполняет парсинг одного выражения:
    ///     expression = logicalOrExpression.
    /// </summary>
    private Expression ParseExpression()
    {
        return ParseLogicalOrExpression();
    }

    /// <summary>
    /// Выполняет парсинг выражения ИЛИ:
    ///     logicalOrExpression = logicalAndExpression, { "ИЛИ", logicalAndExpression }.
    /// </summary>
    private Expression ParseLogicalOrExpression()
    {
        Expression left = ParseLogicalAndExpression();
        while (tokens.Peek().Type == TokenType.Or)
        {
            tokens.Advance();
            left = new BinaryOperationExpression(left, BinaryOperation.Or, ParseLogicalAndExpression());
        }

        return left;
    }

    /// <summary>
    /// Выполняет парсинг выражения И:
    ///     logicalAndExpression = comparisonExpression, { "И", comparisonExpression }.
    /// </summary>
    private Expression ParseLogicalAndExpression()
    {
        Expression left = ParseComparisonExpression();
        while (tokens.Peek().Type == TokenType.And)
        {
            tokens.Advance();
            left = new BinaryOperationExpression(left, BinaryOperation.And, ParseComparisonExpression());
        }

        return left;
    }

    /// <summary>
    /// Выполняет парсинг сравнения выражений:
    ///     comparisonExpression = additiveExpression, [ ( "==" | "!=" | ".<" | ">" | "<=" | ">=" ), additiveExpression ].
    /// </summary>
    private Expression ParseComparisonExpression()
    {
        Expression left = ParseAdditiveExpression();
        TokenType type = tokens.Peek().Type;

        BinaryOperation? op = type switch
        {
            TokenType.Equal => BinaryOperation.Equal,
            TokenType.NotEqual => BinaryOperation.NotEqual,
            TokenType.LessThan => BinaryOperation.LessThan,
            TokenType.GreaterThan => BinaryOperation.GreaterThan,
            TokenType.LessThanOrEqual => BinaryOperation.LessThanOrEqual,
            TokenType.GreaterThanOrEqual => BinaryOperation.GreaterThanOrEqual,
            _ => null,
        };

        if (op.HasValue)
        {
            tokens.Advance();
            return new BinaryOperationExpression(left, op.Value, ParseAdditiveExpression());
        }

        return left;
    }

    /// <summary>
    /// Выполняет парсинг сложения/вычитания:
    ///     additiveExpression = multiplicativeExpression, { ("+" | "-"), multiplicativeExpression }.
    /// </summary>
    private Expression ParseAdditiveExpression()
    {
        Expression left = ParseMultiplicativeExpression();

        while (true)
        {
            TokenType type = tokens.Peek().Type;
            BinaryOperation? op = type switch
            {
                TokenType.Plus => BinaryOperation.Plus,
                TokenType.Minus => BinaryOperation.Minus,
                _ => null,
            };

            if (op.HasValue)
            {
                tokens.Advance();
                left = new BinaryOperationExpression(left, op.Value, ParseMultiplicativeExpression());
            }
            else
            {
                return left;
            }
        }
    }

    /// <summary>
    /// Разбирает умножение/деление/остаток:
    ///     multiplicativeExpression = unaryExpression, { ("*" | "/" | "%"), unaryExpression }.
    /// </summary>
    private Expression ParseMultiplicativeExpression()
    {
        Expression left = ParseUnaryExpression();
        while (true)
        {
            TokenType type = tokens.Peek().Type;

            BinaryOperation? op = type switch
            {
                TokenType.Multiplication => BinaryOperation.Multiplication,
                TokenType.Division => BinaryOperation.Division,
                TokenType.Remainder => BinaryOperation.Remainder,
                _ => null,
            };

            if (op.HasValue)
            {
                tokens.Advance();
                left = new BinaryOperationExpression(left, op.Value, ParseUnaryExpression());
            }
            else
            {
                return left;
            }
        }
    }

    /// <summary>
    /// Разбирает унарную операцию:
    ///     unaryExpression = { "+" | "-" | "НЕ" } , primary.
    /// </summary>
    private Expression ParseUnaryExpression()
    {
        while (true)
        {
            TokenType type = tokens.Peek().Type;
            UnaryOperation? op = type switch
            {
                TokenType.Plus => UnaryOperation.Plus,
                TokenType.Minus => UnaryOperation.Minus,
                TokenType.Not => UnaryOperation.Not,
                _ => null,
            };

            if (op.HasValue)
            {
                tokens.Advance();
                return new UnaryOperationExpression(op.Value, ParseUnaryExpression());
            }

            return ParsePrimaryExpression();
        }
    }

    /// <summary>
    /// Парсинг основного выражения:
    ///     primary = numericLiteral | stringLiteral | logicalLiteral | constant | functionCall | identifier | "(", expression, ")".
    /// </summary>
    private Expression ParsePrimaryExpression()
    {
        Token t = tokens.Peek();
        switch (t.Type)
        {
            case TokenType.NumericLiteral:
            case TokenType.StringLiteral:
            case TokenType.Hype:
            case TokenType.Cringe:
                return ParseLiteral();

            case TokenType.Identifier:
                string identifierName = t.Value?.ToString() ?? "";
                tokens.Advance();

                if (tokens.Peek().Type == TokenType.OpenParenthesis)
                {
                    return ParseCallTail(identifierName);
                }
                else
                {
                    return new VariableExpression(identifierName);
                }

            case TokenType.OpenParenthesis:
                tokens.Advance();
                Expression value = ParseExpression();
                Match(TokenType.CloseParenthesis);
                return value;

            default:
                throw new UnexpectedLexemeException(t.Type, t);
        }
    }

    /// <summary>
    /// Парсинг литералов.
    /// </summary>
    private Expression ParseLiteral()
    {
        Token t = tokens.Peek();
        switch (t.Type)
        {
            case TokenType.NumericLiteral:
                return ParseNumericLiteral();
            case TokenType.StringLiteral:
                return ParseStringLiteral();
            case TokenType.Hype:
            case TokenType.Cringe:
                return ParseLogicalLiteral();
            default:
                throw new UnexpectedLexemeException(t.Type, t);
        }
    }

    /// <summary>
    /// Парсинг числового литерала:
    ///     numericLiteral = realLiteral | integerLiteral.
    /// </summary>
    private LiteralExpression ParseNumericLiteral()
    {
        Token t = tokens.Peek();
        if (t.Type == TokenType.NumericLiteral)
        {
            string rawValue = t.Value!.ToString()!;
            double value = double.Parse(rawValue.Replace(',', '.'));
            tokens.Advance();

            Runtime.ValueType type = rawValue.Contains('.') || rawValue.Contains(',')
                ? Runtime.ValueType.Poltorashka
                : Runtime.ValueType.Ciferka;

            LiteralExpression literal = new LiteralExpression(new Value(value));
            literal.ResultType = type;

            return literal;
        }

        throw new UnexpectedLexemeException(t.Type, t);
    }

    /// <summary>
    /// Парсинг строкового литерала:
    ///     stringLiteral = '"', { anyChar - '"' | escapeSequence }, '"'.
    /// </summary>
    private LiteralExpression ParseStringLiteral()
    {
        Token t = tokens.Peek();
        string val = t.Value!.ToString()!;
        tokens.Advance();

        LiteralExpression literal = new LiteralExpression(new Value(val));
        literal.ResultType = Runtime.ValueType.Citata;
        return literal;
    }

    /// <summary>
    /// Парсинг логического значения:
    ///     logicalLiteral = "ХАЙП" | "КРИНЖ".
    /// </summary>
    private LiteralExpression ParseLogicalLiteral()
    {
        Token t = tokens.Peek();
        bool val = t.Type == TokenType.Hype;
        tokens.Advance();

        LiteralExpression literal = new LiteralExpression(new Value(val));
        literal.ResultType = Runtime.ValueType.Rasklad;
        return literal;
    }

    /// <summary>
    /// Пропускает ожидаемую лексему либо бросает исключение, если встретит иную лексему.
    /// </summary>
    private void Match(TokenType expected)
    {
        if (tokens.Peek().Type != expected)
        {
            throw new UnexpectedLexemeException(expected, tokens.Peek());
        }

        tokens.Advance();
    }

    private bool IsTypeName(Token token) => token.Type switch
    {
        TokenType.Ciferka => true,
        TokenType.Poltorashka => true,
        TokenType.Citata => true,
        TokenType.Rasklad => true,
        _ => false
    };

    private string ParseTypeName()
    {
        Token token = tokens.Peek();
        switch (token.Type)
        {
            case TokenType.Ciferka:
            case TokenType.Poltorashka:
            case TokenType.Citata:
            case TokenType.Rasklad:
                tokens.Advance();
                return token.Type.ToString().ToLower();
            default:
                throw new UnexpectedLexemeException("ожидается имя типа", token);
        }
    }

    private string ParseIdentifier()
    {
        Token token = tokens.Peek();
        if (token.Type == TokenType.Identifier)
        {
            tokens.Advance();
            return token.Value?.ToString() ?? "";
        }

        throw new UnexpectedLexemeException(TokenType.Identifier, token);
    }
}