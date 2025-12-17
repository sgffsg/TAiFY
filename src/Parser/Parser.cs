using Execution;
using Lexer;

namespace Parser;

public class Parser
{
    private readonly TokenStream tokens;
    private readonly Context context;
    private readonly IEnvironment environment;
    private readonly Stack<LoopContext> loopStack = new();
    private readonly Stack<FunctionContext> functionStack = new();

    public Parser(Context context, IEnvironment environment, string code)
    {
        this.context = context;
        this.environment = environment;
        tokens = new TokenStream(code);
    }

    public void ParseProgram()
    {
        do
        {
            ParseTopLevelItem();
        }
        while (tokens.Peek().Type != TokenType.EOF);
    }

    public List<double> ExecuteExpressionToList()
    {
        List<double> result = new();

        do
        {
            result.Add(ParseExpression());
        }
        while (tokens.Peek().Type != TokenType.EOF);

        return result;
    }

    /// <summary>
    /// topLevelItem =
    ///     procedureDeclaration | constantDeclaration | typedDeclaration.
    /// </summary>
    private void ParseTopLevelItem()
    {
        Token token = tokens.Peek();
        switch(token.Type)
        {
            case TokenType.Baza:
                ParseConstantDeclaration();
                break;
            case TokenType.Prokrastiniryem:
                ParseProcedureDeclaration();
                break;
            default:
                if (IsTypeName(token))
                {
                    ParseTypedDeclaration();
                }
                else
                {
                    ParseStatement();
                }

                break;
        }
    }

    /// <summary>
    /// constantDeclaration =
    ///     "БАЗА", typeName, identifier, "=", expression, ";".
    /// </summary>
    private void ParseConstantDeclaration()
    {
        Match(TokenType.Baza);

        string typeName = ParseTypeName();
        string identifier = ParseIdentifier();

        Match(TokenType.Assignment);
        double value = ParseExpression();
        Match(TokenType.Semicolon);

        context.DefineConstant(identifier, value);
    }

    /// <summary>
    /// typedDeclaration =
    ///     typeName, identifier, (functionTail | variableTail ) ;.
    /// </summary>
    private void ParseTypedDeclaration()
    {
        string typeName = ParseTypeName();
        string identifier = ParseIdentifier();

        if (tokens.Peek().Type == TokenType.OpenParenthesis)
        {
            ParseFunctionTail(typeName, identifier);
        }
        else
        {
            ParseVariableTail(typeName, identifier);
        }
    }


    /// <summary>
    /// functionTail =
    ///     "(", [parameterList], ")", block ;.
    /// </summary>
    private void ParseFunctionTail(string returnType, string functionName)
    {
        context.DefineFunction(functionName);

        Scope functionScope = new Scope();
        context.PushScope(functionScope);

        functionStack.Push(new FunctionContext(functionName, returnType));

        Match(TokenType.OpenParenthesis);
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            ParseParameterList();
        }

        Match(TokenType.CloseParenthesis);

        ParseBlock();

        context.PopScope();
        functionStack.Pop();
    }

    /// <summary>
    /// variableTail =
    ///     "=", expression, ";".
    /// </summary>
    private void ParseVariableTail(string typeName, string variableName)
    {
        Match(TokenType.Assignment);
        double value = ParseExpression();
        Match(TokenType.Semicolon);

        context.DefineVariable(variableName, value);
    }

    /// <summary>
    /// procedureDeclaration =
    ///     "ПРОКРАСТИНИРУЕМ", identifier, "(", [parameterList], ")", block.
    /// </summary>
    private void ParseProcedureDeclaration()
    {
        Match(TokenType.Prokrastiniryem);
        string procedureName = ParseIdentifier();

        context.RegisterProcedure(procedureName);

        Scope procedureScope = new Scope();
        context.PushScope(procedureScope);

        Match(TokenType.OpenParenthesis);
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            ParseParameterList();
        }

        Match(TokenType.CloseParenthesis);
        ParseBlock();

        context.PopScope();
    }

    /// <summary>
    /// parameterList =
    ///     identifier, ":", typeName, { ",", identifier, ":", typeName };.
    /// </summary>
    private void ParseParameterList()
    {
        do
        {
            string paramName = ParseIdentifier();
            Match(TokenType.ColonTypeIndication);
            string paramType = ParseTypeName();

            context.DefineVariable(paramName, 0);

            if (tokens.Peek().Type != TokenType.Comma)
            {
                break;
            }

            Match(TokenType.Comma);
        }
        while (true);
    }

    /// <summary>
    /// block = "ПОЕХАЛИ", { statement }, "ФИНАЛОЧКА" ;.
    /// </summary>
    private void ParseBlock()
    {
        Scope blockScope = new Scope();
        context.PushScope(blockScope);

        Match(TokenType.Poehali);

        while (tokens.Peek().Type != TokenType.Finalochka &&
               tokens.Peek().Type != TokenType.EOF)
        {
            ParseStatement();
        }

        Match(TokenType.Finalochka);
        context.PopScope();
    }

    /// <summary>
    /// statement =
    ///     variableDeclaration | ifStatement | whileStatement | forStatement | returnStatement | breakStatement
    ///      | continueStatement | ioStatement | block | sideEffectStatement | ";" ;.
    /// </summary>
    private void ParseStatement()
    {
        Token token = tokens.Peek();
        switch (token.Type)
        {
            case TokenType.Semicolon:
                tokens.Advance();
                break;
            case TokenType.Ciferka:
            case TokenType.Poltorashka:
            case TokenType.Citata:
            case TokenType.Rasklad:
            case TokenType.Pachka:
                ParseVariableDeclaration();
                break;
            case TokenType.Esli:
                ParseIfStatement();
                break;
            case TokenType.Cikl:
                ParseForStatement();
                break;
            case TokenType.Poka:
                ParseWhileStatement();
                break;
            case TokenType.Dratuti:
                ParseReturnStatement();
                break;
            case TokenType.Hvatit:
                ParseBreakStatement();
                break;
            case TokenType.Prodolzhaem:
                ParseContinueStatement();
                break;
            case TokenType.Vbros:
                ParseInputStatement();
                break;
            case TokenType.Vybros:
                ParseOutputStatement();
                break;
            case TokenType.Poehali:
                ParseBlock();
                break;
            case TokenType.Identifier:
                ParseSideEffectStatement();
                break;
            default:
                double result = ParseExpression();
                Match(TokenType.Semicolon);
                environment.WriteNumber(result);
                break;
        }
    }

    /// <summary>
    /// variableDeclaration =
    ///     typeName, identifier, "=", expression, ";" ;.
    /// </summary>
    private void ParseVariableDeclaration()
    {
        string typeName = ParseTypeName();
        string identifier = ParseIdentifier();
        Match(TokenType.Assignment);
        double value = ParseExpression();
        Match(TokenType.Semicolon);

        context.DefineVariable(identifier, value);
    }

    /// <summary>
    /// variableAssignment = identifier, "=", expression ;.
    /// </summary>
    private void ParseVariableAssignment()
    {
        string identifier = ParseIdentifier();
        Match(TokenType.Assignment);
        double value = ParseExpression();
        Match(TokenType.Semicolon);

        context.AssignVariable(identifier, value);
    }

    /// <summary>
    /// ifStatement =
    ///     "ЕСЛИ", "(", expression, ")", "ТО", statement,
    ///     ["ИНАЧЕ", statement] ;.
    /// </summary>
    private void ParseIfStatement()
    {
        Match(TokenType.Esli);
        Match(TokenType.OpenParenthesis);
        double condition = ParseExpression();
        Match(TokenType.CloseParenthesis);
        Match(TokenType.To);

        if (condition != 0)
        {
            ParseStatement();

            if (tokens.Peek().Type == TokenType.Inache)
            {
                Match(TokenType.Inache);
                SkipStatement();
            }
        }
        else
        {
            SkipStatement();

            if (tokens.Peek().Type == TokenType.Inache)
            {
                Match(TokenType.Inache);
                ParseStatement();
            }
        }
    }

    /// <summary>
    /// whileStatement =
    ///     "ПОКА", "(", expression, ")", block ;.
    /// </summary>
    private void ParseWhileStatement()
    {
        Match(TokenType.Poka);
        Match(TokenType.OpenParenthesis);

        double condition = ParseExpression();

        Match(TokenType.CloseParenthesis);

        loopStack.Push(new LoopContext(LoopType.While));

        if (condition != 0)
        {
            ParseBlockOrStatement();
        }
        else
        {
            SkipBlockOrStatement();
        }

        loopStack.Pop();
    }

    /// <summary>
    /// forStatement =
    ///     "ЦИКЛ", "(", variableAssignment, ";", expression, ";", variableAssignment, ")", block ;.
    /// </summary>
    private void ParseForStatement()
    {
        Match(TokenType.Cikl);
        Match(TokenType.OpenParenthesis);

        ParseVariableAssignment();
        Match(TokenType.Semicolon);

        double condition = ParseExpression();
        Match(TokenType.Semicolon);

        SkipVariableAssignment();
        Match(TokenType.CloseParenthesis);

        loopStack.Push(new LoopContext(LoopType.For));

        if (condition != 0)
        {
            ParseBlockOrStatement();
        }
        else
        {
            SkipBlockOrStatement();
        }

        loopStack.Pop();
    }

    /// <summary>
    /// Пропускает блок или одиночный statement.
    /// </summary>
    private void ParseBlockOrStatement()
    {
        if (tokens.Peek().Type == TokenType.Poehali)
        {
            ParseBlock();
        }
        else
        {
            ParseStatement();
        }
    }

    /// <summary>
    /// returnStatement = "ДРАТУТИ", [ expression ], ";" ;.
    /// </summary>
    private void ParseReturnStatement()
    {
        if (functionStack.Count == 0)
        {
            throw new Exception("Оператор 'ДРАТУТИ' может использоваться только внутри функции");
        }

        Match(TokenType.Dratuti);

        double returnValue = 0;
        if (tokens.Peek().Type != TokenType.Semicolon)
        {
            returnValue = ParseExpression();
        }

        Match(TokenType.Semicolon);

        FunctionContext currentFunction = functionStack.Peek();
        currentFunction.ReturnValue = returnValue;
        currentFunction.HasReturned = true;
    }

    /// <summary>
    /// breakStatement = "ХВАТИТ", ";" ;.
    /// </summary>
    private void ParseBreakStatement()
    {
        if (loopStack.Count == 0)
        {
            throw new Exception("Оператор 'ХВАТИТ' может использоваться только внутри цикла");
        }

        Match(TokenType.Hvatit);
        Match(TokenType.Semicolon);
    }

    /// <summary>
    /// continueStatement = "ПРОДОЛЖАЕМ", ";" ;.
    /// </summary>
    private void ParseContinueStatement()
    {
        if (loopStack.Count == 0)
        {
            throw new Exception("Оператор 'ПРОДОЛЖАЕМ' может использоваться только внутри цикла");
        }

        Match(TokenType.Prodolzhaem);
        Match(TokenType.Semicolon);
    }

    /// <summary>
    /// inputStatement = "ВБРОС", "(", identifier, { ",", identifier }, ")", ";".
    /// </summary>
    private void ParseInputStatement()
    {
        Match(TokenType.Vbros);
        Match(TokenType.OpenParenthesis);

        List<string> variables = new List<string>();
        variables.Add(ParseIdentifier());

        while (tokens.Peek().Type == TokenType.Comma)
        {
            Match(TokenType.Comma);
            variables.Add(ParseIdentifier());
        }

        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);

        foreach (string variable in variables)
        {
            if (!context.Exists(variable))
            {
                context.DefineVariable(variable, 0);
            }

            double value = environment.ReadNumber();
            context.AssignVariable(variable, value);
        }
    }

    /// <summary>
    /// outputStatement = "ВЫБРОС", "(", [ argumentList ], ")", ";".
    /// </summary>
    private void ParseOutputStatement()
    {
        Match(TokenType.Vybros);
        Match(TokenType.OpenParenthesis);

        List<double> arguments = new List<double>();
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments = ParseArgumentList();
        }

        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);

        foreach (double arg in arguments)
        {
            environment.WriteNumber(arg);
        }
    }

    /// <summary>
    /// sideEffectStatement = identifier, ( assignmentTail | callTail ), ";" ;.
    /// </summary>
    private void ParseSideEffectStatement()
    {
        string identifier = ParseIdentifier();

        if (tokens.Peek().Type == TokenType.Assignment)
        {
            ParseAssignment(identifier);
        }
        else if (tokens.Peek().Type == TokenType.OpenParenthesis)
        {
            ParseCallTail(identifier);
        }
        else
        {
            throw new UnexpectedLexemeException("ожидается '=' или '('", tokens.Peek());
        }
    }

    /// <summary>
    /// assignmentTail = "=", expression ;.
    /// </summary>
    private void ParseAssignment(string variableName)
    {
        Match(TokenType.Assignment);
        double value = ParseExpression();
        Match(TokenType.Semicolon);

        if (!context.Exists(variableName))
        {
            throw new Exception($"Необъявленная переменная: {variableName}");
        }

        context.AssignVariable(variableName, value);
    }

    /// <summary>
    /// callTail = "(", [ argumentList ], ")" ;.
    /// </summary>
    private void ParseCallTail(string functionName)
    {
        if (!context.Exists(functionName))
        {
            throw new Exception($"Необъявленная функция: {functionName}");
        }

        Match(TokenType.OpenParenthesis);

        List<double> arguments = new();
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments = ParseArgumentList();
        }

        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);

        environment.WriteNumber(arguments.Count);
    }

    /// <summary>
    /// Парсит список значений, разделенных запятыми.
    /// </summary>
    private List<double> ParseArgumentList()
    {
        List<double> values = new List<double> { ParseExpression() };
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
    private double ParseExpression()
    {
        return ParseLogicalOrExpression();
    }

    /// <summary>
    /// Выполняет парсинг выражения ИЛИ:
    ///     logicalOrExpression = logicalAndExpression, { "ИЛИ", logicalAndExpression }.
    /// </summary>
    private double ParseLogicalOrExpression()
    {
        double left = ParseLogicalAndExpression();
        while (tokens.Peek().Type == TokenType.Or)
        {
            tokens.Advance();
            double right = ParseLogicalAndExpression();
            left = (left != 0 || right != 0) ? 1 : 0;
        }

        return left;
    }

    /// <summary>
    /// Выполняет парсинг выражения И:
    ///     logicalAndExpression = comparisonExpression, { "И", comparisonExpression }.
    /// </summary>
    private double ParseLogicalAndExpression()
    {
        double left = ParseComparisonExpression();
        while (tokens.Peek().Type == TokenType.And)
        {
            tokens.Advance();
            double right = ParseComparisonExpression();
            left = (left != 0 && right != 0) ? 1 : 0;
        }

        return left;
    }

    /// <summary>
    /// Выполняет парсинг сравнения выражений:
    ///     comparisonExpression = additiveExpression, [ ( "==" | "!=" | "<" | ">" | "<=" | ">=" ), additiveExpression ].
    /// </summary>
    private double ParseComparisonExpression()
    {
        double left = ParseAdditiveExpression();

        switch (tokens.Peek().Type)
        {
            case TokenType.Equal:
                tokens.Advance();
                left = (left == ParseAdditiveExpression()) ? 1 : 0;
                break;
            case TokenType.NotEqual:
                tokens.Advance();
                left = (left != ParseAdditiveExpression()) ? 1 : 0;
                break;
            case TokenType.LessThan:
                tokens.Advance();
                left = (left < ParseAdditiveExpression()) ? 1 : 0;
                break;
            case TokenType.GreaterThan:
                tokens.Advance();
                left = (left > ParseAdditiveExpression()) ? 1 : 0;
                break;
            case TokenType.LessThanOrEqual:
                tokens.Advance();
                left = (left <= ParseAdditiveExpression()) ? 1 : 0;
                break;
            case TokenType.GreaterThanOrEqual:
                tokens.Advance();
                left = (left >= ParseAdditiveExpression()) ? 1 : 0;
                break;
            default:
                return left;
        }

        return left;
    }

    /// <summary>
    /// Выполняет парсинг сложения/вычитания:
    ///     additiveExpression = multiplicativeExpression, { ("+" | "-"), multiplicativeExpression }.
    /// </summary>
    private double ParseAdditiveExpression()
    {
        double left = ParseMultiplicativeExpression();

        while (true)
        {
            switch (tokens.Peek().Type)
            {
                case TokenType.Plus:
                    tokens.Advance();
                    left += ParseMultiplicativeExpression();
                    break;
                case TokenType.Minus:
                    tokens.Advance();
                    left -= ParseMultiplicativeExpression();
                    break;
                default:
                    return left;
            }
        }
    }

    /// <summary>
    /// Разбирает умножение/деление/остаток:
    ///     multiplicativeExpression = unaryExpression, { ("*" | "/" | "%"), unaryExpression }.
    /// </summary>
    private double ParseMultiplicativeExpression()
    {
        double left = ParseUnaryExpression();
        while (true)
        {
            switch (tokens.Peek().Type)
            {
                case TokenType.Multiplication:
                    tokens.Advance();
                    left *= ParseUnaryExpression();
                    break;
                case TokenType.Division:
                    tokens.Advance();
                    {
                        double divisor = ParseUnaryExpression();
                        if (divisor == 0)
                        {
                            throw new DivideByZeroException();
                        }

                        left /= divisor;
                    }

                    break;
                case TokenType.Remainder:
                    tokens.Advance();
                    {
                        double divisor = ParseUnaryExpression();
                        if (divisor == 0)
                        {
                            throw new DivideByZeroException();
                        }

                        left %= divisor;
                    }

                    break;
                default:
                    return left;
            }
        }
    }

    /// <summary>
    /// Разбирает унарную операцию:
    ///     unaryExpression = { "+" | "-" | "НЕ" } , primary.
    /// </summary>
    private double ParseUnaryExpression()
    {
        while (true)
        {
            TokenType type = tokens.Peek().Type;
            if (type == TokenType.Plus || type == TokenType.Minus || type == TokenType.Not)
            {
                tokens.Advance();
                double value = ParseUnaryExpression();

                return type switch
                {
                    TokenType.Plus => +value,
                    TokenType.Minus => -value,
                    TokenType.Not => (value == 0) ? 1 : 0,
                    _ => value
                };
            }

            return ParsePrimaryExpression();
        }
    }

    /// <summary>
    /// Парсинг основного выражения:
    ///     primary = numericLiteral | stringLiteral | logicalLiteral | constant | functionCall | identifier | "(", expression, ")".
    /// </summary>
    private double ParsePrimaryExpression()
    {
        Token t = tokens.Peek();
        switch (t.Type)
        {
            case TokenType.NumericLiteral:
            case TokenType.StringLiteral:
            case TokenType.Hype:
            case TokenType.Cringe:
                return ParseLiteral();

            case TokenType.PI:
            case TokenType.EULER:
                return ParseDefaultConstant();

            case TokenType.Module:
            case TokenType.Minimum:
            case TokenType.Maximum:
            case TokenType.Pow:
            case TokenType.Sqrt:
            case TokenType.Sinus:
            case TokenType.Cosinus:
            case TokenType.Tangens:
                return ParseDefaultFunctionCall();

            case TokenType.Identifier:
                string identifierName = t.Value?.ToString() ?? "";
                tokens.Advance();

                if (tokens.Peek().Type == TokenType.OpenParenthesis)
                {
                    return ParseIdentifierAsFunctionCall(identifierName);
                }
                else
                {
                    return ParseVariableReference(identifierName);
                }

            case TokenType.OpenParenthesis:
                tokens.Advance();
                double value = ParseExpression();
                Match(TokenType.CloseParenthesis);
                return value;

            default:
                throw new UnexpectedLexemeException(t.Type, t);
        }
    }

    /// <summary>
    /// Парсинг литералов.
    /// </summary>
    private double ParseLiteral()
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
    private double ParseNumericLiteral()
    {
        Token t = tokens.Peek();
        if (t.Type == TokenType.NumericLiteral)
        {
            double value = t.Value!.ToDouble();
            tokens.Advance();
            return value;
        }

        throw new UnexpectedLexemeException(TokenType.NumericLiteral, t);
    }

    /// <summary>
    /// Парсинг строкового литерала:
    ///     stringLiteral = '"', { anyChar - '"' | escapeSequence }, '"'.
    /// </summary>
    private double ParseStringLiteral()
    {
        // В данной реализации строки не поддерживаются для числовых вычислений
        Token t = tokens.Peek();
        tokens.Advance();

        return t.Value!.ToString().Length;
    }

    /// <summary>
    /// Парсинг логического значения:
    ///     logicalLiteral = "ХАЙП" | "КРИНЖ".
    /// </summary>
    private double ParseLogicalLiteral()
    {
        Token t = tokens.Peek();
        tokens.Advance();
        return t.Type switch
        {
            TokenType.Hype => 1,
            TokenType.Cringe => 0,
            _ => throw new UnexpectedLexemeException(t.Type, t),
        };
    }

    /// <summary>
    /// Парсинг констант:
    ///     constant = "ПИ" | "ЕШКА".
    /// </summary>
    private double ParseDefaultConstant()
    {
        Token t = tokens.Peek();
        tokens.Advance();
        return t.Type switch
        {
            TokenType.EULER => 2.7182818284,
            TokenType.PI => 3.1415926535,
            _ => throw new UnexpectedLexemeException(t.Type, t),
        };
    }

    /// <summary>
    /// Парсинг вызова встроенной функции:
    ///     functionCall = identifier, "(", [ argumentList ], ")".
    /// </summary>
    private double ParseDefaultFunctionCall()
    {
        Token defaultFunctionToken = tokens.Peek();
        tokens.Advance();

        Match(TokenType.OpenParenthesis);

        List<double> arguments = new();
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments = ParseArgumentList();
        }

        Match(TokenType.CloseParenthesis);
        return BuiltinFunctions.Instance.Invoke(defaultFunctionToken.Type, arguments);
    }

    /// <summary>
    /// Парсинг идентификатора как вызов пользовательской функции:
    ///     function_call = function_name, "(", [ expression_list ], ")" ;
    /// </summary>
    private double ParseIdentifierAsFunctionCall(string functionName)
    {
        Match(TokenType.OpenParenthesis);

        List<double> arguments = new List<double>();
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments = ParseArgumentList();
        }

        Match(TokenType.CloseParenthesis);

        return arguments.Count;
    }

    /// <summary>
    /// Парсинг идентификатора как обращения к переменной:
    ///     variableDeclaration = (typeName, identifier, "=", expression, ";" ) | ( "БАЗА", typeName, identifier, "=", expression, ";" )
    /// </summary>
    private double ParseVariableReference(string variableName)
    {
        try
        {
            return context.GetValue(variableName);
        }
        catch (ArgumentException ex)
        {
            throw new Exception($"Необъявленная переменная: {variableName}", ex);
        }
    }

    /// <summary>
    /// Парсинг массива:
    ///     array_literal = "[", [ expression_list ], "]".
    /// </summary>
    private List<double> ParseArrayLiteral()
    {
        Match(TokenType.OpenSquareBracket);
        List<double> values = ParseArgumentList();
        Match(TokenType.CloseSquareBracket);
        return values;
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
        TokenType.Pachka => true,
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
            case TokenType.Pachka:
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

    private void SkipBlock()
    {
        Match(TokenType.Poehali);

        int braceCount = 1;
        while (braceCount > 0 && tokens.Peek().Type != TokenType.EOF)
        {
            if (tokens.Peek().Type == TokenType.Poehali)
            {
                braceCount++;
            }
            else if (tokens.Peek().Type == TokenType.Finalochka)
            {
                braceCount--;
            }

            tokens.Advance();
        }
    }

    private void SkipStatement()
    {
        int depth = 0;

        while (tokens.Peek().Type != TokenType.EOF)
        {
            Token token = tokens.Peek();

            if (token.Type == TokenType.Semicolon && depth == 0)
            {
                tokens.Advance();
                break;
            }

            if (token.Type == TokenType.Poehali)
            {
                depth++;
            }
            else if (token.Type == TokenType.Finalochka)
            {
                depth--;
            }

            tokens.Advance();
        }
    }

    private void SkipBlockOrStatement()
    {
        if (tokens.Peek().Type == TokenType.Poehali)
        {
            SkipBlock();
        }
        else
        {
            SkipStatement();
        }
    }

    /// <summary>
    /// Пропускает присваивание переменной.
    /// </summary>
    private void SkipVariableAssignment()
    {
        ParseIdentifier();
        Match(TokenType.Assignment);
        while (tokens.Peek().Type != TokenType.Semicolon &&
               tokens.Peek().Type != TokenType.CloseParenthesis &&
               tokens.Peek().Type != TokenType.EOF)
        {
            tokens.Advance();
        }
    }
}

/// <summary>
/// Контекст цикла для управления break/continue.
/// </summary>
public class LoopContext
{
    public LoopContext(LoopType type)
    {
        Type = type;
    }

    public LoopType Type { get; }
}

/// <summary>
/// Тип цикла.
/// </summary>
public enum LoopType
{
    /// <summary>
    /// Цикл While.
    /// </summary>
    While,

    /// <summary>
    /// Цикл For.
    /// </summary>
    For,
}

/// <summary>
/// Контекст функции для управления возвратом значений.
/// </summary>
public class FunctionContext
{
    public FunctionContext(string name, string returnType)
    {
        Name = name;
        ReturnType = returnType;
        ReturnValue = 0;
        HasReturned = false;
    }

    public string Name { get; }

    public string ReturnType { get; }

    public double ReturnValue { get; set; }

    public bool HasReturned { get; set; }
}