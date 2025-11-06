using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public class TextScanner(string text)
    {
        private readonly string text = text;
        private int position;

        public char Peek(int n = 0)
        {
            int peekPosition = this.position + n;
            return peekPosition < text.Length ? text[peekPosition] : '\0';
        }

        public void Advance() => position++;

        public bool IsEnd() => position >= text.Length;

        public int GetPosition() => position;

        public void SetPosition(int newPosition) => position = newPosition;
    }
}
