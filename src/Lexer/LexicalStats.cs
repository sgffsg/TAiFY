using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public static class LexicalStats
    {
        private static readonly HashSet<TokenType> Keywords = new()
        {
            TokenType.Poehali, TokenType.Finalochka, TokenType.Prokrastiniryem, TokenType.Vbros,
            TokenType.Vybros, TokenType.Hvatit, TokenType.Dratuti, TokenType.Ciferka, TokenType.Poltorashka,
            TokenType.Citata, TokenType.Rasklad, TokenType.Pachka, TokenType.Baza, TokenType.And,
            TokenType.Or, TokenType.Not, TokenType.Esli, TokenType.To, TokenType.Inache, TokenType.Cikl,
        };

        private static readonly HashSet<TokenType> Operators = new()
        {
            TokenType.Equal, TokenType.NotEqual, TokenType.LessThan, TokenType.LessThanOrEqual,
            TokenType.GreaterThan, TokenType.GreaterThanOrEqual, TokenType.Plus, TokenType.Minus,
            TokenType.Multiplication, TokenType.Division, TokenType.Remainder, TokenType.And,
            TokenType.Not, TokenType.Or, TokenType.Assignment, TokenType.ColonTypeIndication,
            TokenType.DotFieldAccess, TokenType.OpenSquareBracket, TokenType.CloseSquareBracket,
            TokenType.OpenParenthesis, TokenType.CloseParenthesis,
        };

        public static string CollectFromFile(string path)
        {
            string text = File.ReadAllText(path, Encoding.UTF8);
            Lexer lexer = new(text);

            LexemStats lexemData = new();
            for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
            {
                if (t.Type == TokenType.Identifier)
                {
                    lexemData.Identifiers++;
                }
                else if (t.Type == TokenType.NumberLiteral)
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
