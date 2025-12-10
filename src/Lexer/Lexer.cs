using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexer
{
    public class Lexer
    {
        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            {
                "ПОЕХАЛИ", TokenType.Poehali
            },
            {
                "ФИНАЛОЧКА", TokenType.Finalochka
            },
            {
                "ПРОКРАСТИНИРУЕМ", TokenType.Prokrastiniryem
            },
            {
                "ВБРОС", TokenType.Vbros
            },
            {
                "ВЫБРОС", TokenType.Vybros
            },
            {
                "ПАЧКА", TokenType.Pachka
            },
            {
                "ЦИТАТА", TokenType.Citata
            },
            {
                "ХАЙП", TokenType.Hype
            },
            {
                "КРИНЖ", TokenType.Cringe
            },
            {
                "РАСКЛАД", TokenType.Rasklad
            },
            {
                "ПОЛТОРАШКА", TokenType.Poltorashka
            },
            {
                "ДРАТУТИ", TokenType.Dratuti
            },
            {
                "ЦИФЕРКА", TokenType.Ciferka
            },
            {
                "ПШИК", TokenType.Pshik
            },
            {
                "ХВАТИТ", TokenType.Hvatit
            },
            {
                "ПРОДОЛЖАЕМ", TokenType.Prodolzhaem
            },
            {
                "БАЗА", TokenType.Baza
            },
            {
                "ЕСЛИ", TokenType.Esli
            },
            {
                "ТО", TokenType.To
            },
            {
                "ИНАЧЕ", TokenType.Inache
            },
            {
                "ЦИКЛ", TokenType.Cikl
            },
            {
                "И", TokenType.And
            },
            {
                "ИЛИ", TokenType.Or
            },
            {
                "НЕ", TokenType.Not
            },
            {
                "ПИ", TokenType.PI
            },
            {
                "ЕШКА", TokenType.EULER
            },
            {
                "МОДУЛЬ", TokenType.Module
            },
            {
                "МИНИМУМ", TokenType.Minimum
            },
            {
                "МАКСИМУМ", TokenType.Maximum
            },
            {
                "СТЕПЕНЬ", TokenType.Pow
            },
            {
                "КОРЕНЬ", TokenType.Sqrt
            },
            {
                "СИНУС", TokenType.Sinus
            },
            {
                "КОСИНУС", TokenType.Cosinus
            },
            {
                "ТАНГЕНС", TokenType.Tangens
            },
        };

        private static readonly Dictionary<char, char> SimpleEscapes = new()
        {
            {
                'n', '\n'
            },
            {
                't', '\t'
            },
            {
                '"', '\"'
            },
            {
                '\\', '\\'
            },
        };

        private static readonly Dictionary<char, char> CaretEscapes = new()
        {
            {
                '@', '\0'
            },
            {
                'A', '\x01'
            },
            {
                'B', '\x02'
            },
            {
                'C', '\x03'
            },
            {
                'D', '\x04'
            },
            {
                'E', '\x05'
            },
            {
                'F', '\x06'
            },
            {
                'G', '\x07'
            },
            {
                'H', '\x08'
            },
            {
                'I', '\x09'
            },
            {
                'J', '\x0A'
            },
            {
                'K', '\x0B'
            },
            {
                'L', '\x0C'
            },
            {
                'M', '\x0D'
            },
            {
                'N', '\x0E'
            },
            {
                'O', '\x0F'
            },
            {
                'P', '\x10'
            },
            {
                'Q', '\x11'
            },
            {
                'R', '\x12'
            },
            {
                'S', '\x13'
            },
            {
                'T', '\x14'
            },
            {
                'U', '\x15'
            },
            {
                'V', '\x16'
            },
            {
                'W', '\x17'
            },
            {
                'X', '\x18'
            },
            {
                'Y', '\x19'
            },
            {
                'Z', '\x1A'
            },
            {
                '[', '\x1B'
            },
            {
                '\\', '\x1C'
            },
            {
                ']', '\x1D'
            },
            {
                '^', '\x1E'
            },
            {
                '_', '\x1F'
            },
            {
                '?', '\x7F'
            },
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
                return new Token(TokenType.EOF);
            }

            char ch = scanner.Peek();
            if (char.IsLetter(ch) || ch == '_')
            {
                return ParseIdentifierOrKeyword();
            }

            if (char.IsAsciiDigit(ch) || ((ch == '-' || ch == '+' || ch == '.') && char.IsAsciiDigit(scanner.Peek(1))))
            {
                return ParseNumericLiteral();
            }

            if (ch == '"')
            {
                return ParseStringLiteral();
            }
            else
            {
                return ParseRemainTokens();
            }
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
            StringBuilder value = new StringBuilder();
            bool hasDigits = false;

            if (scanner.Peek() == '-')
            {
                value.Append(scanner.Peek());
                scanner.Advance();
            }
            else if (scanner.Peek() == '+')
            {
                scanner.Advance();
            }

            while (char.IsAsciiDigit(scanner.Peek()))
            {
                value.Append(scanner.Peek());
                scanner.Advance();
                hasDigits = true;
            }

            if (scanner.Peek() == '.')
            {
                value.Append(scanner.Peek());
                scanner.Advance();

                bool hasFractionDigits = false;
                while (char.IsAsciiDigit(scanner.Peek()))
                {
                    value.Append(scanner.Peek());
                    scanner.Advance();
                    hasFractionDigits = true;
                    hasDigits = true;
                }

                if (!hasFractionDigits)
                {
                    return new Token(TokenType.Error, new TokenValue("Missing numbers after dot"));
                }
            }

            if (char.IsLetter(scanner.Peek()) || scanner.Peek() == '_')
            {
                int startPos = scanner.GetPosition();
                scanner.SetPosition(startPos);
                StringBuilder errorToken = new StringBuilder();
                errorToken.Append(value);

                while (!scanner.IsEnd() && !char.IsWhiteSpace(scanner.Peek()))
                {
                    errorToken.Append(scanner.Peek());
                    scanner.Advance();
                }

                return new Token(TokenType.Error, new TokenValue($"Incorrect numeric literal: {errorToken}"));
            }

            if (!hasDigits)
            {
                return new Token(TokenType.Error, new TokenValue("No digits in the number"));
            }

            if (decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal resultNumber))
            {
                return new Token(TokenType.NumericLiteral, new TokenValue(resultNumber));
            }
            else
            {
                return new Token(TokenType.Error, new TokenValue($"Не удалось распознать число: {value}"));
            }
        }

        private Token ParseStringLiteral()
        {
            scanner.Advance();
            StringBuilder content = new StringBuilder();

            while(!scanner.IsEnd() && scanner.Peek() != '"')
            {
                if (scanner.Peek() == '\n' || scanner.Peek() == '\r')
                {
                    return new Token(TokenType.Error, new TokenValue("The string cannot contain newline characters."));
                }

                if (scanner.Peek() == '\\')
                {
                    if (TryParseEscapeSequence(out char escapeSequence))
                    {
                        content.Append(escapeSequence);
                    }
                    else
                    {
                        return new Token(TokenType.Error, new TokenValue("Invalid Escape Sequence"));
                    }
                }
                else
                {
                    content.Append(scanner.Peek());
                    scanner.Advance();
                }
            }

            if (scanner.IsEnd())
            {
                return new Token(TokenType.Error, new TokenValue("Unclosed string"));
            }

            scanner.Advance();
            return new Token(TokenType.StringLiteral, new TokenValue(content.ToString()));
        }

        private bool TryParseEscapeSequence(out char escapeSequence)
        {
            scanner.Advance();

            if (scanner.IsEnd())
            {
                escapeSequence = '\0';
                return false;
            }

            char escapeChar = scanner.Peek();
            bool isValid = true;

            switch (escapeChar)
            {
                case 'n':
                    escapeSequence = '\n';
                    break;
                case 't':
                    escapeSequence = '\t';
                    break;
                case '"':
                    escapeSequence = '"';
                    break;
                case '\\':
                    escapeSequence = '\\';
                    break;
                case 'r':
                    escapeSequence = '\r';
                    break;
                default:
                    escapeSequence = '\0';
                    isValid = false;
                    break;
            }

            scanner.Advance();
            return isValid;
        }

        private Token ParseRemainTokens()
        {
            char ch = scanner.Peek();

            switch (ch)
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

                    return new Token(TokenType.Error, new TokenValue("Unknown operator: '!'"));
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
                default:
                    scanner.Advance();
                    return new Token(TokenType.Error, new TokenValue($"Unknown symbol: '{ch.ToString()}'"));
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
