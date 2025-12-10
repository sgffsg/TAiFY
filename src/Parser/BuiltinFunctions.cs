using Lexer;

namespace Parser
{
    public class BuiltinFunctions
    {
        private readonly Dictionary<TokenType, Func<List<decimal>, decimal>> functions = new()
        {
            { TokenType.Module, Module },
            { TokenType.Minimum, Minimum },
            { TokenType.Maximum, Maximum },
            { TokenType.Pow, Pow },
            { TokenType.Sqrt, Sqrt },
            { TokenType.Sinus, Sinus },
            { TokenType.Cosinus, Cosinus },
            { TokenType.Tangens, Tangens },
        };

        private static readonly BuiltinFunctions InstanceValue = new();

        public static BuiltinFunctions Instance => InstanceValue;

        public decimal Invoke(TokenType type, List<decimal> arguments)
        {
            if (!functions.TryGetValue(type, out Func<List<decimal>, decimal>? function))
            {
                throw new ArgumentException($"Unknown builtin function {type}");
            }

            return function(arguments);
        }

        private static decimal Module(List<decimal> arguments)
        {
            if (arguments.Count != 1)
            {
                throw new ArgumentException($"Встроенная функция МОДУЛЬ требует 1 аргумент, получено: {arguments.Count}");
            }

            return Math.Abs(arguments[0]);
        }

        private static decimal Minimum(List<decimal> arguments)
        {
            if (arguments.Count == 0)
            {
                throw new ArgumentException($"Builtin function МИНИМУМ requires at least 1 argument, received: {arguments.Count}");
            }

            return arguments.Min();
        }

        private static decimal Maximum(List<decimal> arguments)
        {
            if (arguments.Count == 0)
            {
                throw new ArgumentException($"Builtin function МАКСИМУМ requires at least 1 argument, received: {arguments.Count}");
            }

            return arguments.Max();
        }

        private static decimal Pow(List<decimal> arguments)
        {
            if (arguments.Count != 2)
            {
                throw new ArgumentException($"Builtin function СТЕПЕНЬ requires exactly 2 arguments, received: {arguments.Count}");
            }

            return (decimal)Math.Pow((double)arguments[0], (double)arguments[1]);
        }

        private static decimal Sqrt(List<decimal> arguments)
        {
            if (arguments.Count != 1)
            {
                throw new ArgumentException($"Builtin function КОРЕНЬ requires exactly 1 argument, received: {arguments.Count}");
            }

            return (decimal)Math.Sqrt((double)arguments[0]);
        }

        private static decimal Sinus(List<decimal> arguments)
        {
            if (arguments.Count != 1)
            {
                throw new ArgumentException($"Builtin function СИНУС requires exactly 1 argument, received: {arguments.Count}");
            }

            return (decimal)Math.Sin((double)arguments[0]);
        }

        private static decimal Cosinus(List<decimal> arguments)
        {
            if (arguments.Count != 1)
            {
                throw new ArgumentException($"Builtin function КОСИНУС requires exactly 1 argument, received: {arguments.Count}");
            }

            return (decimal)Math.Cos((double)arguments[0]);
        }

        private static decimal Tangens(List<decimal> arguments)
        {
            if (arguments.Count != 1)
            {
                throw new ArgumentException($"Builtin function ТАНГЕНС requires exactly 1 argument, received: {arguments.Count}");
            }

            return (decimal)Math.Tan((double)arguments[0]);
        }
    }
}
