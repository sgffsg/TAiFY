using System.Text;

namespace Lexer
{
    public static class LexicalStats
    {
        private static readonly HashSet<TokenType> Keywords = new()
        {
            TokenType.ПОЕХАЛИ, TokenType.ФИНАЛОЧКА, TokenType.ПРОКРАСТИНИРУЕМ, TokenType.ВБРОС,
            TokenType.ВЫБРОС, TokenType.ХВАТИТ, TokenType.ПРОДОЛЖАЕМ, TokenType.ДРАТУТИ, TokenType.ЦИФЕРКА,
            TokenType.ПОЛТОРАШКА, TokenType.ЦИТАТА, TokenType.РАСКЛАД, TokenType.БАЗА, TokenType.И,
            TokenType.ИЛИ, TokenType.НЕ, TokenType.ЕСЛИ, TokenType.ТО, TokenType.ИНАЧЕ, TokenType.ЦИКЛ,
        };

        private static readonly HashSet<TokenType> Operators = new()
        {
            TokenType.Equal, TokenType.NotEqual, TokenType.LessThan, TokenType.LessThanOrEqual,
            TokenType.GreaterThan, TokenType.GreaterThanOrEqual, TokenType.Plus, TokenType.Minus,
            TokenType.Multiplication, TokenType.Division, TokenType.Remainder, TokenType.И,
            TokenType.НЕ, TokenType.ИЛИ, TokenType.Assignment, TokenType.ColonTypeIndication,
            TokenType.DotFieldAccess, TokenType.OpenSquareBracket, TokenType.CloseSquareBracket,
            TokenType.OpenParenthesis, TokenType.CloseParenthesis,
        };

        public static string CollectFromFile(string path)
        {
            string text = File.ReadAllText(path, Encoding.UTF8);
            Lexer lexer = new(text);

            LexemStats lexemData = new();
            for (Token t = lexer.ParseToken(); t.Type != TokenType.EOF; t = lexer.ParseToken())
            {
                if (t.Type == TokenType.Identifier)
                {
                    lexemData.Identifiers++;
                }
                else if (t.Type == TokenType.IntegerLiteral)
                {
                    lexemData.NumberLiterals++;
                }
                else if (t.Type == TokenType.StringLiteral)
                {
                    lexemData.StringLiterals++;
                }
                else if (Keywords.Contains(t.Type))
                {
                    lexemData.Keywords++;
                }
                else if (Operators.Contains(t.Type))
                {
                    lexemData.Operators++;
                }
                else
                {
                    lexemData.OtherLexemes++;
                }
            }

            return lexemData.ToString();
        }
    }
}
