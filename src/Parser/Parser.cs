using Execution;
using Lexer;

namespace Parser;

public class Parser
{
    private readonly TokenStream tokens;
    private readonly IEnvironment environment;
    private readonly Dictionary<string, object> symbols = new();

    
    public Parser(IEnvironment environment, string code)
    {
        this.environment = environment;
        tokens = new TokenStream(code);
    }

    /// <summary>
    /// program = { topLevelItem } , EOF.
    /// </summary>
    public void ParseProgram()
    {
        do
        {
            ParseTopLevelItem();
        }
        while (tokens.Peek().Type != TokenType.EOF);
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
        decimal value = ParseExpression();
        Match(TokenType.Semicolon);

        symbols[identifier] = value;
        environment.AddResult(value);
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
            ParseFunctionTail(identifier);
        }
        else
        {
            ParseVariableTail(identifier);
        }
    }

    /// <summary>
    /// functionTail =
    ///     "(", [parameterList], ")", block ;.
    /// </summary>
    private void ParseFunctionTail(string functionName)
    {
        Match(TokenType.OpenParenthesis);
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            ParseParameterList();
        }

        Match(TokenType.CloseParenthesis);

        SkipBlock();
    }

    /// <summary>
    /// variableTail =
    ///     "=", expression, ";".
    /// </summary>
    private void ParseVariableTail(string variableName)
    {
        Match(TokenType.Assignment);
        decimal value = ParseExpression();
        Match(TokenType.Semicolon);

        symbols[variableName] = value;
        environment.AddResult(value);
    }

    /// <summary>
    /// procedureDeclaration =
    ///     "ПРОКРАСТИНИРУЕМ", identifier, "(", [parameterList], ")", block.
    /// </summary>
    private void ParseProcedureDeclaration()
    {
        Match(TokenType.Prokrastiniryem);
        string procedureName = ParseIdentifier();
        Match(TokenType.OpenParenthesis);

        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            ParseParameterList();
        }

        Match(TokenType.CloseParenthesis);
        ParseBlock();
    }

    /// <summary>
    /// parameterList =
    ///     identifier, ":", typeName, { ",", identifier, ":", typeName };.
    /// </summary>
    private void ParseParameterList()
    {
        do
        {
            ParseIdentifier();
            Match(TokenType.ColonTypeIndication);
            ParseTypeName();

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
        Match(TokenType.Poehali);

        while (tokens.Peek().Type != TokenType.Finalochka &&
               tokens.Peek().Type != TokenType.EOF)
        {
            ParseStatement();
        }

        Match(TokenType.Finalochka);
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
                decimal result = ParseExpression();
                Match(TokenType.Semicolon);
                environment.AddResult(result);
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
        decimal value = ParseExpression();
        Match(TokenType.Semicolon);

        symbols[identifier] = value;
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
        decimal condition = ParseExpression();
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

        while (tokens.Peek().Type != TokenType.CloseParenthesis &&
               tokens.Peek().Type != TokenType.EOF)
        {
            tokens.Advance();
        }

        Match(TokenType.CloseParenthesis);

        SkipBlockOrStatement();
    }

    /// <summary>
    /// forStatement =
    ///     "ЦИКЛ", "(", variableAssignment, ";", expression, ";", variableAssignment, ")", block ;.
    /// </summary>
    private void ParseForStatement()
    {
        Match(TokenType.Cikl);
        Match(TokenType.OpenParenthesis);

        while (tokens.Peek().Type != TokenType.Semicolon &&
               tokens.Peek().Type != TokenType.EOF)
        {
            tokens.Advance();
        }

        Match(TokenType.Semicolon);

        while (tokens.Peek().Type != TokenType.Semicolon &&
               tokens.Peek().Type != TokenType.EOF)
        {
            tokens.Advance();
        }

        Match(TokenType.Semicolon);

        while (tokens.Peek().Type != TokenType.CloseParenthesis &&
               tokens.Peek().Type != TokenType.EOF)
        {
            tokens.Advance();
        }

        Match(TokenType.CloseParenthesis);

        SkipBlockOrStatement();
    }

    /// <summary>
    /// Пропускает блок или одиночный statement.
    /// </summary>
    private void SkipBlockOrStatement()
    {
        if (tokens.Peek().Type == TokenType.Poehali)
        {
            // Пропускаем блок
            SkipBlock();
        }
        else
        {
            // Пропускаем одиночный statement
            SkipStatement();
        }
    }

    /// <summary>
    /// returnStatement = "ДРАТУТИ", [ expression ], ";" ;.
    /// </summary>
    private void ParseReturnStatement()
    {
        Match(TokenType.Dratuti);

        decimal returnValue = 0;
        if (tokens.Peek().Type != TokenType.Semicolon)
        {
            returnValue = ParseExpression();
        }

        Match(TokenType.Semicolon);
        environment.AddResult(returnValue);
    }

    /// <summary>
    /// breakStatement = "ХВАТИТ", ";" ;.
    /// </summary>
    private void ParseBreakStatement()
    {
        Match(TokenType.Hvatit);
        Match(TokenType.Semicolon);
    }

    /// <summary>
    /// continueStatement = "ПРОДОЛЖАЕМ", ";" ;.
    /// </summary>
    private void ParseContinueStatement()
    {
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
            symbols[variable] = environment.ReadNumber();
        }
    }

    /// <summary>
    /// outputStatement = "ВЫБРОС", "(", [ argumentList ], ")", ";".
    /// </summary>
    private void ParseOutputStatement()
    {
        Match(TokenType.Vybros);
        Match(TokenType.OpenParenthesis);

        List<decimal> arguments = new List<decimal>();
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments = ParseArgumentList();
        }
        else
        {
        }

        Match(TokenType.CloseParenthesis);

        Match(TokenType.Semicolon);
        foreach (decimal arg in arguments)
        {
            environment.AddResult(arg);
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
        decimal value = ParseExpression();
        Match(TokenType.Semicolon);

        if (!symbols.ContainsKey(variableName))
        {
            throw new Exception($"Необъявленная переменная: {variableName}");
        }

        // symbols[variableName] = value;
        environment.AddResult(value);
    }

    /// <summary>
    /// callTail       = "(", [ argumentList ], ")" ;.
    /// </summary>
    private void ParseCallTail(string functionName)
    {
        Match(TokenType.OpenParenthesis);

        List<decimal> arguments = new();
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments = ParseArgumentList();
        }

        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);

        environment.AddResult(arguments.Count);
    }

    /// <summary>
    /// variableAssignment = identifier, "=", expression ;.
    /// </summary>
    private void ParseVariableAssignment()
    {
        string identifier = ParseIdentifier();
        Match(TokenType.Assignment);
        while (tokens.Peek().Type != TokenType.Semicolon &&
               tokens.Peek().Type != TokenType.CloseParenthesis &&
               tokens.Peek().Type != TokenType.EOF)
        {
            tokens.Advance();
        }
    }

    /// <summary>
    /// Парсит список значений, разделенных запятыми.
    /// </summary>
    private List<decimal> ParseArgumentList()
    {
        List<decimal> values = new List<decimal> { ParseExpression() };
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
    private decimal ParseExpression()
    {
        return ParseLogicalOrExpression();
    }

    /// <summary>
    /// Выполняет парсинг выражения ИЛИ:
    ///     logicalOrExpression = logicalAndExpression, { "ИЛИ", logicalAndExpression }.
    /// </summary>
    private decimal ParseLogicalOrExpression()
    {
        decimal left = ParseLogicalAndExpression();
        while (tokens.Peek().Type == TokenType.Or)
        {
            tokens.Advance();
            decimal right = ParseLogicalAndExpression();
            left = (left != 0 || right != 0) ? 1 : 0;
        }

        return left;
    }

    /// <summary>
    /// Выполняет парсинг выражения И:
    ///     logicalAndExpression = comparisonExpression, { "И", comparisonExpression }.
    /// </summary>
    private decimal ParseLogicalAndExpression()
    {
        decimal left = ParseComparisonExpression();
        while (tokens.Peek().Type == TokenType.And)
        {
            tokens.Advance();
            decimal right = ParseComparisonExpression();
            left = (left != 0 && right != 0) ? 1 : 0;
        }

        return left;
    }

    /// <summary>
    /// Выполняет парсинг сравнения выражений:
    ///     comparisonExpression = additiveExpression, [ ( "==" | "!=" | ".<" | ">" | "<=" | ">=" ), additiveExpression ].
    /// </summary>
    private decimal ParseComparisonExpression()
    {
        decimal left = ParseAdditiveExpression();

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
        }

        return left;
    }

    /// <summary>
    /// Выполняет парсинг сложения/вычитания:
    ///     additiveExpression = multiplicativeExpression, { ("+" | "-"), multiplicativeExpression }.
    /// </summary>
    private decimal ParseAdditiveExpression()
    {
        decimal left = ParseMultiplicativeExpression();

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
    private decimal ParseMultiplicativeExpression()
    {
        decimal left = ParseUnaryExpression();
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
                        decimal divisor = ParseUnaryExpression();
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
                        decimal divisor = ParseUnaryExpression();
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
    private decimal ParseUnaryExpression()
    {
        while (true)
        {
            TokenType type = tokens.Peek().Type;
            if (type == TokenType.Plus || type == TokenType.Minus || type == TokenType.Not)
            {
                tokens.Advance();
                decimal value = ParseUnaryExpression();

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
    private decimal ParsePrimaryExpression()
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
                decimal value = ParseExpression();
                Match(TokenType.CloseParenthesis);
                return value;

            default:
                throw new UnexpectedLexemeException(t.Type, t);
        }
    }

    /// <summary>
    /// Парсинг литералов.
    /// </summary>
    private decimal ParseLiteral()
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
    private decimal ParseNumericLiteral()
    {
        Token t = tokens.Peek();
        if (t.Type == TokenType.NumericLiteral)
        {
            decimal value = t.Value!.ToDecimal();
            tokens.Advance();
            return value;
        }

        throw new UnexpectedLexemeException(TokenType.NumericLiteral, t);
    }

    /// <summary>
    /// Парсинг строкового литерала:
    ///     stringLiteral = '"', { anyChar - '"' | escapeSequence }, '"'.
    /// </summary>
    private decimal ParseStringLiteral()
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
    private decimal ParseLogicalLiteral()
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
    private decimal ParseDefaultConstant()
    {
        Token t = tokens.Peek();
        tokens.Advance();
        return t.Type switch
        {
            TokenType.EULER => 2.7182818284M,
            TokenType.PI => 3.1415926535M,
            _ => throw new UnexpectedLexemeException(t.Type, t),
        };
    }

    /// <summary>
    /// Парсинг вызова встроенной функции:
    ///     functionCall = identifier, "(", [ argumentList ], ")".
    /// </summary>
    private decimal ParseDefaultFunctionCall()
    {
        Token defaultFunctionToken = tokens.Peek();
        tokens.Advance();

        Match(TokenType.OpenParenthesis);

        List<decimal> args = new List<decimal>();
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            args = ParseArgumentList();
        }

        Match(TokenType.CloseParenthesis);
        return BuiltinFunctions.Instance.Invoke(defaultFunctionToken.Type, args);
    }

    /// <summary>
    /// Парсинг идентификатора как вызов пользовательской функции:
    ///     function_call = function_name, "(", [ expression_list ], ")" ;.
    /// </summary>
    private decimal ParseIdentifierAsFunctionCall(string functionName)
    {
        Match(TokenType.OpenParenthesis);

        List<decimal> arguments = new List<decimal>();
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments = ParseArgumentList();
        }

        Match(TokenType.CloseParenthesis);
        return arguments.Count;
    }

    /// <summary>
    /// Парсинг идентификатора как обращения к переменной:
    ///     variableDeclaration = (typeName, identifier, "=", expression, ";" ) | ( "БАЗА", typeName, identifier, "=", expression, ";" ).
    /// </summary>
    private decimal ParseVariableReference(string variableName)
    {
        if (symbols.TryGetValue(variableName, out object? value))
        {
            if (value is decimal decimalValue)
            {
                return decimalValue;
            }

            return Convert.ToDecimal(value);
        }

        throw new Exception($"Необъявленная переменная: {variableName}");
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
}