using Xunit;

namespace Lexer.UnitTests;

public class LexerTest
{
    private const string FileContent =
            """
            ПОЕХАЛИ
            
            БАЗА имя: ЦИТАТА = "Алиса";
            БАЗА возраст: ЦИФЕРКА = 25;
            
            ЕСЛИ возраст > 18 ТО
                ВЫБРОС "Совершеннолетний: " + имя;
            ИНАЧЕ
                ВЫБРОС "Несовершеннолетний: " + имя;
            
            ПРОКРАСТИНИРУЕМ приветствие(имя: ЦИТАТА)
            {
                ВЫБРОС "Привет, " + имя + "!";
                ДРАТУТИ;
            }
            
            приветствие(имя);
            ФИНАЛОЧКА
            """;

    private const string ExpectedResult =
        """
        keywords: 15
        identifiers: 10
        number literals: 2
        string literals: 5
        operators: 14
        other lexemes: 9
        """;

    [Theory]
    [MemberData(nameof(GetTokenizeLexerKeywordData))]
    public void Check_Lexer_Keywords(string text, List<Token> expected)
    {
        List<Token> actual = Tokenize(text);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, List<Token>> GetTokenizeLexerKeywordData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "ПОЕХАЛИ", [
                    new Token(TokenType.Poehali)
                ]
            },
            {
                "ФИНАЛОЧКА", [
                    new Token(TokenType.Finalochka)
                ]
            },
            {
                "ПРОКРАСТИНИРУЕМ", [
                    new Token(TokenType.Prokrastiniryem)
                ]
            },
            {
                "ВБРОС", [
                    new Token(TokenType.Vbros)
                ]
            },
            {
                "ВЫБРОС", [
                    new Token(TokenType.Vybros)
                ]
            },
            {
                "ХВАТИТ", [
                    new Token(TokenType.Hvatit)
                ]
            },
            {
                "ДРАТУТИ", [
                    new Token(TokenType.Dratuti)
                ]
            },
            {
                "ЦИФЕРКА", [
                    new Token(TokenType.Ciferka)
                ]
            },
            {
                "ПОЛТОРАШКА", [
                    new Token(TokenType.Poltorashka)
                ]
            },
            {
                "ЦИТАТА", [
                    new Token(TokenType.Citata)
                ]
            },
            {
                "РАСКЛАД", [
                    new Token(TokenType.Rasklad)
                ]
            },
            {
                "ПАЧКА", [
                    new Token(TokenType.Pachka)
                ]
            },
            {
                "ХАЙП", [
                    new Token(TokenType.Hype)
                ]
            },
            {
                "КРИНЖ", [
                    new Token(TokenType.Cringe)
                ]
            },
            {
                "ПШИК", [
                    new Token(TokenType.Pshik)
                ]
            },
            {
                "БАЗА", [
                    new Token(TokenType.Baza)
                ]
            },
            {
                "ЕСЛИ", [
                    new Token(TokenType.Esli)
                ]
            },
            {
                "ТО", [
                    new Token(TokenType.To)
                ]
            },
            {
                "ИНАЧЕ", [
                    new Token(TokenType.Inache)
                ]
            },
            {
                "ЦИКЛ", [
                    new Token(TokenType.Cikl)
                ]
            },
            {
                "И", [
                    new Token(TokenType.And)
                ]
            },
            {
                "ИЛИ", [
                    new Token(TokenType.Or)
                ]
            },
            {
                "НЕ", [
                    new Token(TokenType.Not)
                ]
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetIdentifierData))]
    public void Check_Lexer_Identifier(string text, List<Token> expected)
    {
        List<Token> actual = Tokenize(text);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, List<Token>> GetIdentifierData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "поехали", [
                    new Token(TokenType.Identifier, new TokenValue("поехали"))
                ]
            },
            {
                "ПоЕхАлИ", [
                    new Token(TokenType.Identifier, new TokenValue("ПоЕхАлИ"))
                ]
            },
            {
                "переменная", [
                    new Token(TokenType.Identifier, new TokenValue("переменная"))
                ]
            },
            {
                "переменная_1", [
                    new Token(TokenType.Identifier, new TokenValue("переменная_1"))
                ]
            },
            {
                "_переменная", [
                    new Token(TokenType.Identifier, new TokenValue("_переменная"))
                ]
            },
            {
                "пере-менная", [
                    new Token(TokenType.Identifier, new TokenValue("пере")),
                    new Token(TokenType.Minus),
                    new Token(TokenType.Identifier, new TokenValue("менная"))
                ]
            },
            {
                "- идентификатор", [
                    new Token(TokenType.Minus),
                    new Token(TokenType.Identifier, new TokenValue("идентификатор"))
                ]
            },
            {
                "123идентификатор", [
                    new Token(TokenType.Error, new TokenValue("Incorrect numeric literal: 123идентификатор"))
                ]
            },
            {
                "спец@символ", [
                    new Token(TokenType.Identifier, new TokenValue("спец")),
                    new Token(TokenType.Error, new TokenValue("Unknown symbol: '@'")),
                    new Token(TokenType.Identifier, new TokenValue("символ"))
                ]
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetTokenizeLiteralsData))]
    public void Check_Lexer_Numeric(string text, List<Token> expected)
    {
        List<Token> actual = Tokenize(text);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, List<Token>> GetTokenizeLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "123", [
                    new Token(TokenType.NumericLiteral, new TokenValue(123m))
                ]
            },
            {
                "-456", [
                    new Token(TokenType.NumericLiteral, new TokenValue(-456))
                ]
            },
            {
                "0", [
                    new Token(TokenType.NumericLiteral, new TokenValue(0))
                ]
            },
            {
                "3.14", [
                    new Token(TokenType.NumericLiteral, new TokenValue(3.14m))
                ]
            },
            {
                "-2.5", [
                    new Token(TokenType.NumericLiteral, new TokenValue(-2.5m))
                ]
            },
            {
                "0.0", [
                    new Token(TokenType.NumericLiteral, new TokenValue(0.0m))
                ]
            },
            {
                "\"простая строка\"", [
                    new Token(TokenType.StringLiteral, new TokenValue("простая строка"))
                ]
            },
            {
                "\"строка с \\n переносом\"", [
                    new Token(TokenType.StringLiteral, new TokenValue("строка с \n переносом"))
                ]
            },
            {
                "\"строка с \\t табуляцией\"", [
                    new Token(TokenType.StringLiteral, new TokenValue("строка с \t табуляцией"))
                ]
            },
            {
                "\"строка с \\\" кавычкой\"", [
                    new Token(TokenType.StringLiteral, new TokenValue("строка с \" кавычкой"))
                ]
            },
            {
                "\"строка с \\\\ слешем\"", [
                    new Token(TokenType.StringLiteral, new TokenValue("строка с \\ слешем"))
                ]
            },
            {
                "\"\"", [
                    new Token(TokenType.StringLiteral, new TokenValue(""))
                ]
            },
            {
                "ХАЙП", [
                    new Token(TokenType.Hype)
                ]
            },
            {
                "КРИНЖ", [
                    new Token(TokenType.Cringe)
                ]
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetTokenizeOperatorsData))]
    public void Check_Lexer_Operators(string text, List<Token> expected)
    {
        List<Token> actual = Tokenize(text);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, List<Token>> GetTokenizeOperatorsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "+", [
                    new Token(TokenType.Plus)
                ]
            },
            {
                "-", [
                    new Token(TokenType.Minus)
                ]
            },
            {
                "*", [
                    new Token(TokenType.Multiplication)
                ]
            },
            {
                "/", [
                    new Token(TokenType.Division)
                ]
            },
            {
                "%", [
                    new Token(TokenType.Remainder)
                ]
            },
            {
                "==", [
                    new Token(TokenType.Equal)
                ]
            },
            {
                "!=", [
                    new Token(TokenType.NotEqual)
                ]
            },
            {
                "<", [
                    new Token(TokenType.LessThan)
                ]
            },
            {
                ">", [
                    new Token(TokenType.GreaterThan)
                ]
            },
            {
                "<=", [
                    new Token(TokenType.LessThanOrEqual)
                ]
            },
            {
                ">=", [
                    new Token(TokenType.GreaterThanOrEqual)
                ]
            },
            {
                "И", [
                    new Token(TokenType.And)
                ]
            },
            {
                "ИЛИ", [
                    new Token(TokenType.Or)
                ]
            },
            {
                "НЕ", [
                    new Token(TokenType.Not)
                ]
            },
            {
                "=", [
                    new Token(TokenType.Assignment)
                ]
            },
            {
                "+5", [
                    new Token(TokenType.NumericLiteral, new TokenValue(5m))
                ]
            },
            {
                "-10", [
                    new Token(TokenType.NumericLiteral, new TokenValue(-10m))
                ]
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetTokenizeSeparatorsData))]
    public void Check_Lexer_Separators(string text, List<Token> expected)
    {
        List<Token> actual = Tokenize(text);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, List<Token>> GetTokenizeSeparatorsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                ";", [
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                ",", [
                    new Token(TokenType.Comma)
                ]
            },
            {
                ".", [
                    new Token(TokenType.DotFieldAccess)
                ]
            },
            {
                "()", [
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.CloseParenthesis)
                ]
            },
            {
                "{}", [
                    new Token(TokenType.OpenCurlyBrace),
                    new Token(TokenType.CloseCurlyBrace)
                ]
            },
            {
                "[]", [
                    new Token(TokenType.OpenSquareBracket),
                    new Token(TokenType.CloseSquareBracket)
                ]
            },
            {
                ":", [
                    new Token(TokenType.ColonTypeIndication)
                ]
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetTokenizeComplexData))]
    public void Complex_Lexer_Tests(string text, List<Token> expected)
    {
        List<Token> actual = Tokenize(text);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, List<Token>> GetTokenizeComplexData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "ПОЕХАЛИ\n    ВЫБРОС \"Привет мир\";\nФИНАЛОЧКА", [
                    new Token(TokenType.Poehali),
                    new Token(TokenType.Vybros),
                    new Token(TokenType.StringLiteral, new TokenValue("Привет мир")),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.Finalochka)
                ]
            },
            {
                "переменная = 42 + переменная2;", [
                    new Token(TokenType.Identifier, new TokenValue("переменная")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.NumericLiteral, new TokenValue(42m)),
                    new Token(TokenType.Plus),
                    new Token(TokenType.Identifier, new TokenValue("переменная2")),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "  \t\n  переменная  \t\n  ", [
                    new Token(TokenType.Identifier, new TokenValue("переменная"))
                ]
            },
            {
                "строка1\nстрока2", [
                    new Token(TokenType.Identifier, new TokenValue("строка1")),
                    new Token(TokenType.Identifier, new TokenValue("строка2"))
                ]
            },
            {
                "@", [
                    new Token(TokenType.Error, new TokenValue("Unknown symbol: '@'"))
                ]
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetTokenizeControlStructuresData))]
    public void Check_Lexer_Control_Structures(string text, List<Token> expected)
    {
        List<Token> actual = Tokenize(text);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, List<Token>> GetTokenizeControlStructuresData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "ПАЧКА[ЦИФЕРКА]", [
                    new Token(TokenType.Pachka),
                    new Token(TokenType.OpenSquareBracket),
                    new Token(TokenType.Ciferka),
                    new Token(TokenType.CloseSquareBracket)
                ]
            },
            {
                "массив[5]", [
                    new Token(TokenType.Identifier, new TokenValue("массив")),
                    new Token(TokenType.OpenSquareBracket),
                    new Token(TokenType.NumericLiteral, new TokenValue(5m)),
                    new Token(TokenType.CloseSquareBracket)
                ]
            },
            {
                "ПРОКРАСТИНИРУЕМ функция() {}", [
                    new Token(TokenType.Prokrastiniryem),
                    new Token(TokenType.Identifier, new TokenValue("функция")),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.OpenCurlyBrace),
                    new Token(TokenType.CloseCurlyBrace)
                ]
            },
            {
                "ЕСЛИ условие ТО действие ИНАЧЕ другое_действие", [
                    new Token(TokenType.Esli),
                    new Token(TokenType.Identifier, new TokenValue("условие")),
                    new Token(TokenType.To),
                    new Token(TokenType.Identifier, new TokenValue("действие")),
                    new Token(TokenType.Inache),
                    new Token(TokenType.Identifier, new TokenValue("другое_действие"))
                ]
            },
            {
                "ЦИКЛ (счетчик < 10) {}", [
                    new Token(TokenType.Cikl),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, new TokenValue("счетчик")),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.NumericLiteral, new TokenValue(10m)),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.OpenCurlyBrace),
                    new Token(TokenType.CloseCurlyBrace)
                ]
            },
            {
                "БАЗА константа = 100;", [
                    new Token(TokenType.Baza),
                    new Token(TokenType.Identifier, new TokenValue("константа")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.NumericLiteral, new TokenValue(100m)),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "переменная: ЦИФЕРКА = 5;", [
                    new Token(TokenType.Identifier, new TokenValue("переменная")),
                    new Token(TokenType.ColonTypeIndication),
                    new Token(TokenType.Ciferka),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.NumericLiteral, new TokenValue(5m)),
                    new Token(TokenType.Semicolon)
                ]
            },
        };
    }

    [Fact]
    public void LexicalStatsTestTheory()
    {
        string tempFilePath = Path.GetTempFileName();
        File.WriteAllText(tempFilePath, FileContent);
        string actual = LexicalStats.CollectFromFile(tempFilePath);
        Assert.Equal(Normalize(ExpectedResult), Normalize(actual));
    }

    private static string Normalize(string s) => string.Concat(s.Where(c => !char.IsWhiteSpace(c)));

    private static List<Token> Tokenize(string text)
    {
        List<Token> results = [];
        Lexer lexer = new(text);

        for (Token t = lexer.ParseToken(); t.Type != TokenType.EOF; t = lexer.ParseToken())
        {
            results.Add(t);
        }

        return results;
    }
}