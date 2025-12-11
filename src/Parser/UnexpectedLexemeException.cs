using Lexer;

namespace Parser
{
    #pragma warning disable RCS1194
    public class UnexpectedLexemeException : Exception
    {
        public UnexpectedLexemeException(string message)
            : base(message)
        {
        }

        public UnexpectedLexemeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public UnexpectedLexemeException(TokenType expected, Token actual)
            : base($"Unexpected lexeme {actual} where expected {expected}")
        {
        }

        public UnexpectedLexemeException(string expected, Token actual)
            : base($"Expected {expected}, but got {actual.Type}")
        {
        }
    }
    #pragma warning restore RCS1194
}
