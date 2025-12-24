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
                "ПОКА", TokenType.Poka
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

            if (char.IsAsciiDigit(ch))
            {
                return ParseNumericLiteral();
            }

            return ch == '"'
                ? ParseStringLiteral()
                : ParseRemainTokens();
        }

        private Token ParseIdentifierOrKeyword()
        {
            StringBuilder value = new StringBuilder();
            value.Append(scanner.Peek());
            scanner.Advance();

            while (!scanner.IsEnd())
            {
                char c = scanner.Peek();
                if (char.IsLetterOrDigit(c) || c == '_')
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

            if (double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double resultNumber))
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
            StringBuilder content = new();
            bool hasError = false;
            scanner.Advance();

            while (!scanner.IsEnd() && scanner.Peek() != '"')
            {
                if (scanner.Peek() == '\n' || scanner.Peek() == '\r')
                {
                    return new Token(TokenType.Error, new TokenValue("The string cannot contain newline characters."));
                }

                if (scanner.Peek() == '\\')
                {
                    if (!DecodeEscapeSequence(content))
                    {
                        hasError = true;
                        content.Append('\\');
                    }
                }
                else
                {
                    content.Append(scanner.Peek());
                    scanner.Advance();
                }
            }

            if (hasError)
            {
                return new Token(TokenType.Error, new TokenValue(content.ToString()));
            }

            scanner.Advance();
            return new Token(TokenType.StringLiteral, new TokenValue(content.ToString()));
        }

        private bool DecodeEscapeSequence(StringBuilder valueBuilder)
        {
            // Предполагаем, что первый символ — обратный слеш "\".
            scanner.Advance();

            char ch1 = scanner.Peek();

            // Разбор простой escape-последовательности: "\n", "\"" и так далее.
            if (SimpleEscapes.TryGetValue(ch1, out char unescaped1))
            {
                scanner.Advance();
                valueBuilder.Append(unescaped1);
                return true;
            }

            // Разбор escape-последовательности в каретной нотации: "\^@", "\^C" и так далее.
            if (ch1 == '^')
            {
                char ch2 = scanner.Peek(1);
                if (CaretEscapes.TryGetValue(ch2, out char unescaped2))
                {
                    scanner.Advance();
                    scanner.Advance();
                    valueBuilder.Append(unescaped2);
                    return true;
                }
            }

            // Разбор escape-последовательности в нотации "\DDD", где D — десятичная цифра,
            //  а DDD — код символа ASCII (число от 0 до 127 включительно).
            if (char.IsAsciiDigit(ch1))
            {
                char ch2 = scanner.Peek(1);
                if (char.IsAsciiDigit(ch2))
                {
                    char ch3 = scanner.Peek(2);
                    if (char.IsAsciiDigit(ch3))
                    {
                        int code = (ch1 - '0') * 100 + (ch2 - '0') * 10 + (ch3 - '0');
                        if (code <= 127)
                        {
                            scanner.Advance();
                            scanner.Advance();
                            scanner.Advance();
                            valueBuilder.Append((char)code);
                            return true;
                        }
                    }
                }
            }

            // Пропуск любой последовательности пробельных символов между двумя обратными слэшами '\', например: "\   \"
            if (!char.IsWhiteSpace(ch1))
            {
                return false;
            }

            int skipCount = 1;
            while (char.IsWhiteSpace(scanner.Peek(skipCount)))
            {
                ++skipCount;
            }

            if (scanner.Peek(skipCount) != '\\')
            {
                return false;
            }

            for (int i = 0; i <= skipCount; ++i)
            {
                scanner.Advance();
            }

            return true;
        }

        private Token ParseRemainTokens()
        {
            char ch = scanner.Peek();

            scanner.Advance();
            switch (ch)
            {
                case ';':
                    return new Token(TokenType.Semicolon);
                case ',':
                    return new Token(TokenType.Comma);
                case '.':
                    return new Token(TokenType.DotFieldAccess);
                case ':':
                    return new Token(TokenType.ColonTypeIndication);
                case '(':
                    return new Token(TokenType.OpenParenthesis);
                case ')':
                    return new Token(TokenType.CloseParenthesis);
                case '{':
                    return new Token(TokenType.OpenCurlyBrace);
                case '}':
                    return new Token(TokenType.CloseCurlyBrace);
                case '[':
                    return new Token(TokenType.OpenSquareBracket);
                case ']':
                    return new Token(TokenType.CloseSquareBracket);
                case '+':
                    return new Token(TokenType.Plus);
                case '-':
                    return new Token(TokenType.Minus);
                case '*':
                    return new Token(TokenType.Multiplication);
                case '/':
                    return new Token(TokenType.Division);
                case '%':
                    return new Token(TokenType.Remainder);
                case '=':
                    if (scanner.Peek() != '=')
                    {
                        return new Token(TokenType.Assignment);
                    }

                    scanner.Advance();
                    return new Token(TokenType.Equal);

                case '!':
                    if (scanner.Peek() != '=')
                    {
                        return new Token(TokenType.Error, new TokenValue("Unknown operator: '!'"));
                    }

                    scanner.Advance();
                    return new Token(TokenType.NotEqual);

                case '<':
                    if (scanner.Peek() != '=')
                    {
                        return new Token(TokenType.LessThan);
                    }

                    scanner.Advance();
                    return new Token(TokenType.LessThanOrEqual);

                case '>':
                    if (scanner.Peek() != '=')
                    {
                        return new Token(TokenType.GreaterThan);
                    }

                    scanner.Advance();
                    return new Token(TokenType.GreaterThanOrEqual);

                default:
                    return new Token(TokenType.Error, new TokenValue($"Unknown symbol: '{ch.ToString()}'"));
            }
        }

        /// <summary>
        ///  Пропускает пробельные символы, пока не встретит иной символ.
        /// </summary>
        private void SkipWhiteSpaces()
        {
            while (!scanner.IsEnd() && char.IsWhiteSpace(scanner.Peek()))
            {
                scanner.Advance();
            }
        }

        private void SkipWhiteSpacesAndComments()
        {
            while (true)
            {
                SkipWhiteSpaces();

                if (!TryParseComment())
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Формат комментария: (ПОЯСНИТЕЛЬНАЯ-БРИГАДА: ...)
        /// </summary>
        private bool TryParseComment()
        {
            int startPos = scanner.GetPosition();

            if (scanner.Peek() != '(')
            {
                return false;
            }

            scanner.Advance();

            string commentStart = "ПОЯСНИТЕЛЬНАЯ-БРИГАДА:";
            for (int i = 0; i < commentStart.Length; i++)
            {
                if (scanner.IsEnd() || scanner.Peek() != commentStart[i])
                {
                    scanner.SetPosition(startPos);
                    return false;
                }

                scanner.Advance();
            }

            while (!scanner.IsEnd())
            {
                if (scanner.Peek() == ')')
                {
                    scanner.Advance();
                    return true;
                }

                scanner.Advance();
            }

            return true;
        }
    }
}
