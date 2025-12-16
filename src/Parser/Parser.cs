using Lexer;

namespace Parser;

public class Parser
{
    private readonly TokenStream tokens;

    private Parser(string code)
    {
        tokens = new TokenStream(code);
    }

    /// <summary>
    /// Парсинг строкового выражения.
    /// </summary>
    public static Row ExecuteExpression(string expression)
    {
        Parser p = new(expression);
        return p.ParseRowWithDelimiter();
    }

    /// <summary>
    /// Парсит строку с выражением до разделителя строк.
    /// </summary>
    private Row ParseRowWithDelimiter()
    {
        Row result = ParseRow();
        ConsumeRowDelimiter();

        return result;
    }

    /// <summary>
    /// Парсит строку в Row.
    /// </summary>
    private Row ParseRow()
    {
        List<decimal> values = ParseValueList();
        return new Row(values.ToArray());
    }

    /// <summary>
    /// Проверяет разделитель строки (точку с запятой или конец файла).
    /// </summary>
    private void ConsumeRowDelimiter()
    {
        Token t = tokens.Peek();
        switch (t.Type)
        {
            case TokenType.Semicolon:
                tokens.Advance();
                break;
            case TokenType.EOF:
                break;
            default:
                throw new UnexpectedLexemeException(TokenType.Semicolon, t);
        }
    }

    /// <summary>
    /// Парсит список значений, разделенных запятыми.
    /// </summary>
    private List<decimal> ParseValueList()
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
    ///     comparisonExpression = additiveExpression, [ ( "==" | "!=" | "<" | ">" | "<=" | ">=" ), additiveExpression ].
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
            args = ParseValueList();
        }

        Match(TokenType.CloseParenthesis);
        return BuiltinFunctions.Instance.Invoke(defaultFunctionToken.Type, args);
    }

    /// <summary>
    /// Парсинг идентификатора как вызов пользовательской функции:
    ///     function_call = function_name, "(", [ expression_list ], ")" ;
    /// </summary>
    private decimal ParseIdentifierAsFunctionCall(string functionName)
    {
        Match(TokenType.OpenParenthesis);

        List<decimal> arguments = new List<decimal>();
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments = ParseValueList();
        }

        Match(TokenType.CloseParenthesis);
        throw new NotImplementedException($"User function '{functionName}' is not implemented yet");
    }

    /// <summary>
    /// Парсинг идентификатора как обращения к переменной:
    ///     variableDeclaration = (typeName, identifier, "=", expression, ";" ) | ( "БАЗА", typeName, identifier, "=", expression, ";" )
    /// </summary>
    private decimal ParseVariableReference(string variableName)
    {
        throw new NotImplementedException($"Variable '{variableName}' is not implemented yet");
    }

    /// <summary>
    /// Парсинг массива:
    ///     array_literal = "[", [ expression_list ], "]".
    /// </summary>
    private List<decimal> ParseArrayLiteral()
    {
        Match(TokenType.OpenSquareBracket);
        List<decimal> values = ParseValueList();
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
}