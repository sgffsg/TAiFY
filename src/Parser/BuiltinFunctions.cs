using Lexer;

namespace Parser
{
    public class BuiltinFunctions
    {
        private readonly Dictionary<TokenType, Func<List<double>, double>> functions = new()
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

        public double Invoke(TokenType type, List<double> arguments)
        {
            if (!functions.TryGetValue(type, out Func<List<double>, double>? function))
            {
                throw new ArgumentException($"Unknown builtin function {type}");
            }

            return function(arguments);
        }

        private static double Module(List<double> arguments)
        {
            if (arguments.Count != 1)
            {
                throw new ArgumentException($"Встроенная функция МОДУЛЬ требует 1 аргумент, получено: {arguments.Count}");
            }

            return Math.Abs(arguments[0]);
        }

        private static double Minimum(List<double> arguments)
        {
            return arguments.Min();
        }

        private static double Maximum(List<double> arguments)
        {
            return arguments.Max();
        }

        private static double Pow(List<double> arguments)
        {
            if (arguments.Count != 2)
            {
                throw new ArgumentException($"Встроенная функция СТЕПЕНЬ требует 2 аргумента, получено: {arguments.Count}");
            }

            return Math.Pow(arguments[0], arguments[1]);
        }

        private static double Sqrt(List<double> arguments)
        {
            if (arguments.Count != 1)
            {
                throw new ArgumentException($"Встроенная функция КОРЕНЬ требует 1 аргумент, получено: {arguments.Count}");
            }

            return Math.Sqrt(arguments[0]);
        }

        private static double Sinus(List<double> arguments)
        {
            if (arguments.Count != 1)
            {
                throw new ArgumentException($"Встроенная функция СИНУС требует 1 аргумент, получено: {arguments.Count}");
            }

            return Math.Sin(arguments[0]);
        }

        private static double Cosinus(List<double> arguments)
        {
            if (arguments.Count != 1)
            {
                throw new ArgumentException($"Встроенная функция КОСИНУС требует 1 аргумент, получено: {arguments.Count}");
            }

            return Math.Cos(arguments[0]);
        }

        private static double Tangens(List<double> arguments)
        {
            if (arguments.Count != 1)
            {
                throw new ArgumentException($"Встроенная функция ТАНГЕНС требует 1 аргумент, получено: {arguments.Count}");
            }

            return Math.Tan(arguments[0]);
        }
    }
}
