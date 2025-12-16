namespace Lexer
{
    public class TokenValue
    {
        private readonly object value;

        public TokenValue(object value) => this.value = value;

        public override string ToString() =>
            value switch
            {
                string s => s,
                int i => i.ToString(),
                double d => d.ToString(),
                decimal d => d.ToString(),
                bool b => b.ToString(),
                _ => throw new NotImplementedException()
            };

        public double ToDouble() =>
            value switch
            {
                string s => double.Parse(s),
                int i => i,
                double d => d,
                decimal d => Convert.ToDouble(d),
                bool b => Convert.ToDouble(b),
                _ => throw new NotImplementedException()
            };

        public decimal ToDecimal() =>
            value switch
            {
                string s => decimal.Parse(s),
                int i => i,
                double d => Convert.ToDecimal(d),
                decimal d => d,
                bool b => Convert.ToDecimal(b),
                _ => throw new NotImplementedException()
            };

        public override bool Equals(object? obj) =>
            obj is TokenValue other && Equals(value, other.value);

        public override int GetHashCode() => value.GetHashCode();
    }
}
