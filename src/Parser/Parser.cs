using Execution;
using Lexer;

namespace Parser;

public class Parser
{
    private readonly TokenStream tokens;
    private readonly Context context;
    private readonly IEnvironment environment;

    public Parser(Context context, IEnvironment environment, string code)
    {
        this.context = context;
        this.environment = environment;
        tokens = new TokenStream(code);
    }

    public void ParseProgram()
    {
    }

    public double ExecuteExpression()
    {
        return ParseExpression();
    }

    public List<double> ExecuteExpressionToList()
    {
        return new List<double>() { ParseExpression() };
    }

    /// <summary>
    /// Парсит список значений, разделенных запятыми.
    /// </summary>
    private List<double> ParseValueList()
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

        List<double> args = new List<double>();
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
    private double ParseIdentifierAsFunctionCall(string functionName)
    {
        Match(TokenType.OpenParenthesis);

        List<double> arguments = new List<double>();
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
    private double ParseVariableReference(string variableName)
    {
        throw new NotImplementedException($"Variable '{variableName}' is not implemented yet");
    }

    /// <summary>
    /// Парсинг массива:
    ///     array_literal = "[", [ expression_list ], "]".
    /// </summary>
    private List<double> ParseArrayLiteral()
    {
        Match(TokenType.OpenSquareBracket);
        List<double> values = ParseValueList();
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