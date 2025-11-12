using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public class Lexer
    {
        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            { "ПОЕХАЛИ", TokenType.Poehali },
            { "ФИНАЛОЧКА", TokenType.Finalochka },
            { "ПРОКРАСТИНИРУЕМ", TokenType.Prokrastiniryem },
            { "ВБРОС", TokenType.Vbros },
            { "ВЫБРОС", TokenType.Vybros },
            { "ПАЧКА", TokenType.Pachka },
            { "ЦИТАТА", TokenType.Citata },
            { "ХАЙП", TokenType.Hype },
            { "КРИНЖ", TokenType.Cringe },
            { "РАСКЛАД", TokenType.Rasklad },
            { "ПОЛТОРАШКА", TokenType.Poltorashka },
            { "ДРАТУТИ", TokenType.Dratuti },
            { "ЦИФЕРКА", TokenType.Ciferka },
            { "ПШИК", TokenType.Pshik },
            { "ХВАТИТ", TokenType.Hvatit },
            { "БАЗА", TokenType.Baza },
            { "ЕСЛИ", TokenType.Esli },
            { "ТО", TokenType.To },
            { "ИНАЧЕ", TokenType.Inache },
            { "ЦИКЛ", TokenType.Cikl },
            { "И", TokenType.And },
            { "ИЛИ", TokenType.Or },
            { "НЕ", TokenType.Not },
        };

        private readonly TextScanner scanner;

        public Lexer(string code)
        {
            scanner = new TextScanner(code);
        }

        public Token ParseToken()
        {
            SkipWhiteSpacesAndComments();

            if (scanner.IsEnd())
            {
                return new Token(TokenType.EndOfFile);
            }

            char c = scanner.Peek();

            if (char.IsLetter(c) || c == '_')
            {
                return ParseIdentifierOrKeyword();
            }
            else if (char.IsAsciiDigit(c) || (c == '-' && char.IsAsciiDigit(scanner.Peek(1))))
            {
                int startPos = scanner.GetPosition();
                Token numberToken = ParseNumericLiteral();

                char nextChar = scanner.Peek();
                if (char.IsLetter(nextChar) || nextChar == '_')
                {
                    scanner.SetPosition(startPos);
                    string errorText = "";
                    while (!scanner.IsEnd() && !char.IsWhiteSpace(scanner.Peek()))
                    {
                        errorText += scanner.Peek();
                        scanner.Advance();
                    }

                    return new Token(TokenType.Error, new TokenValue(errorText));
                }

                return numberToken;
            }
            else if (c == '"')
            {
                return ParseStringLiteral();
            }

            switch (c)
            {
                case ';':
                    scanner.Advance();
                    return new Token(TokenType.Semicolon);
                case ',':
                    scanner.Advance();
                    return new Token(TokenType.Comma);
                case '.':
                    scanner.Advance();
                    return new Token(TokenType.DotFieldAccess);
                case ':':
                    scanner.Advance();
                    return new Token(TokenType.ColonTypeIndication);
                case '(':
                    scanner.Advance();
                    return new Token(TokenType.OpenParenthesis);
                case ')':
                    scanner.Advance();
                    return new Token(TokenType.CloseParenthesis);
                case '{':
                    scanner.Advance();
                    return new Token(TokenType.OpenCurlyBrace);
                case '}':
                    scanner.Advance();
                    return new Token(TokenType.CloseCurlyBrace);
                case '[':
                    scanner.Advance();
                    return new Token(TokenType.OpenSquareBracket);
                case ']':
                    scanner.Advance();
                    return new Token(TokenType.CloseSquareBracket);
                case '+':
                    scanner.Advance();
                    return new Token(TokenType.Plus);
                case '-':
                    scanner.Advance();
                    return new Token(TokenType.Minus);
                case '*':
                    scanner.Advance();
                    return new Token(TokenType.Multiplication);
                case '/':
                    scanner.Advance();
                    return new Token(TokenType.Division);
                case '%':
                    scanner.Advance();
                    return new Token(TokenType.Remainder);
                case '=':
                    scanner.Advance();
                    if (scanner.Peek() == '=')
                    {
                        scanner.Advance();
                        return new Token(TokenType.Equal);
                    }

                    return new Token(TokenType.Assignment);
                case '!':
                    scanner.Advance();
                    if (scanner.Peek() == '=')
                    {
                        scanner.Advance();
                        return new Token(TokenType.NotEqual);
                    }

                    return new Token(TokenType.Error, new TokenValue("Неизвестный оператор '!'"));
                case '<':
                    scanner.Advance();
                    if (scanner.Peek() == '=')
                    {
                        scanner.Advance();
                        return new Token(TokenType.LessThanOrEqual);
                    }

                    return new Token(TokenType.LessThan);
                case '>':
                    scanner.Advance();
                    if (scanner.Peek() == '=')
                    {
                        scanner.Advance();
                        return new Token(TokenType.GreaterThanOrEqual);
                    }

                    return new Token(TokenType.GreaterThan);
            }

            scanner.Advance();
            return new Token(TokenType.Error, new TokenValue($"Неизвестный символ: '{c}'"));
        }

        private Token ParseIdentifierOrKeyword()
        {
            StringBuilder value = new StringBuilder();
            value.Append(scanner.Peek());
            scanner.Advance();

            while (!scanner.IsEnd())
            {
                char c = scanner.Peek();
                if (char.IsLetter(c) || char.IsAsciiDigit(c) || c == '_')
                {
                    value.Append(c);
                    scanner.Advance();
                }
                else
                {
                    break;
                }
            }

            string identifier = value.ToString();
            if (Keywords.TryGetValue(identifier, out TokenType type))
            {
                return new Token(type);
            }

            return new Token(TokenType.Identifier, new TokenValue(identifier));
        }

        private Token ParseNumericLiteral()
        {
            string value = "";

            if (scanner.Peek() == '-')
            {
                value += scanner.Peek();
                scanner.Advance();
            }

            while (char.IsAsciiDigit(scanner.Peek()))
            {
                value += scanner.Peek();
                scanner.Advance();
            }

            if (scanner.Peek() == '.')
            {
                value += scanner.Peek();
                scanner.Advance();

                while (char.IsAsciiDigit(scanner.Peek()))
                {
                    value += scanner.Peek();
                    scanner.Advance();
                }
            }

            if (decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal number))
            {
                return new Token(TokenType.NumberLiteral, new TokenValue(number));
            }
            else
            {
                return new Token(TokenType.Error, new TokenValue(value));
            }
        }

        private Token ParseStringLiteral()
        {
            scanner.Advance();

            StringBuilder contents = new StringBuilder();

            while (!scanner.IsEnd() && scanner.Peek() != '"')
            {
                if (scanner.Peek() == '\\')
                {
                    if (TryParseStringLiteralEscapeSequence(out char unescaped))
                    {
                        contents.Append(unescaped);
                    }
                    else
                    {
                        return new Token(TokenType.Error, new TokenValue("Некорректная escape-последовательность"));
                    }
                }
                else
                {
                    contents.Append(scanner.Peek());
                    scanner.Advance();
                }
            }

            if (scanner.IsEnd())
            {
                return new Token(TokenType.Error, new TokenValue("Незакрытая строка"));
            }

            scanner.Advance();

            return new Token(TokenType.StringLiteral, new TokenValue(contents.ToString()));
        }

        private bool TryParseStringLiteralEscapeSequence(out char unescaped)
        {
            scanner.Advance();

            if (scanner.IsEnd())
            {
                unescaped = '\0';
                return false;
            }

            char escapeChar = scanner.Peek();
            scanner.Advance();

            switch (escapeChar)
            {
                case 'n':
                    unescaped = '\n';
                    return true;
                case 't':
                    unescaped = '\t';
                    return true;
                case '"':
                    unescaped = '"';
                    return true;
                case '\\':
                    unescaped = '\\';
                    return true;
                case 'r':
                    unescaped = '\r';
                    return true;
                default:
                    unescaped = '\0';
                    return false;
            }
        }

        private void SkipWhiteSpacesAndComments()
        {
            do
            {
                while (!scanner.IsEnd() && char.IsWhiteSpace(scanner.Peek()))
                {
                    scanner.Advance();
                }
            }
            while (TryParseMultilineComment() || TryParseSingleLineComment());
        }

        private bool TryParseMultilineComment()
        {
            if (scanner.Peek() == '(')
            {
                string potentialStart = "ПОЯСНИТЕЛЬНАЯ-БРИГАДА:";

                for (int i = 0; i < potentialStart.Length; i++)
                {
                    if (scanner.Peek(i) != potentialStart[i])
                    {
                        return false;
                    }
                }

                for (int i = 0; i < potentialStart.Length; i++)
                {
                    scanner.Advance();
                }

                StringBuilder endSequence = new StringBuilder();
                bool inComment = true;

                while (!scanner.IsEnd() && inComment)
                {
                    char c = scanner.Peek();
                    scanner.Advance();
                    endSequence.Append(c);

                    if (endSequence.Length > 20)
                    {
                        endSequence.Remove(0, 1);
                    }

                    if (endSequence.ToString().EndsWith("ФИНАЛОЧКА-КОММЕНТАРИЯ)"))
                    {
                        inComment = false;
                    }
                }

                return true;
            }

            return false;
        }

        private bool TryParseSingleLineComment()
        {
            if (scanner.Peek() == '(')
            {
                string potentialStart = "ПОЯСНИТЕЛЬНАЯ-БРИГАДА:";

                for (int i = 0; i < potentialStart.Length; i++)
                {
                    if (scanner.Peek(i) != potentialStart[i])
                    {
                        return false;
                    }
                }

                while (!scanner.IsEnd() && scanner.Peek() != '\n' && scanner.Peek() != '\r')
                {
                    scanner.Advance();
                }

                return true;
            }

            return false;
        }
    }
}
