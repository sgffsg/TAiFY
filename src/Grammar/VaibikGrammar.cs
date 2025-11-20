using Antlr4.Runtime;

namespace VaibikGrammar;

public static class VaibikGrammar
{
    public static void ValidateProgram(string vaibikCode)
    {
        AntlrInputStream inputStream = new(vaibikCode);
        VaibikLexer lexer = new(inputStream);
        CommonTokenStream tokenStream = new(lexer);
        VaibikParser parser = new(tokenStream);
        parser.RemoveErrorListeners();
        parser.ErrorHandler = new BailErrorStrategy();
        parser.program();
    }
}