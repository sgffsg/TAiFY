using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public struct LexemStats
    {
        public int Keywords { get; set; }

        public int Identifiers { get; set; }

        public int NumberLiterals { get; set; }

        public int StringLiterals { get; set; }

        public int Operators { get; set; }

        public int OtherLexemes { get; set; }

        public override readonly string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"keywords: {Keywords}\n");
            sb.Append($"identifiers: {Identifiers}\n");
            sb.Append($"number literals: {NumberLiterals}\n");
            sb.Append($"string literals: {StringLiterals}\n");
            sb.Append($"operators: {Operators}\n");
            sb.Append($"other lexemes: {OtherLexemes}\n");

            return sb.ToString();
        }
    }
}
