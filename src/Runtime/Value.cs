using System.Globalization;

namespace Runtime;

public class Value : IEquatable<Value>
{
    public static readonly Value Void = new(VaibikVoid.Value);
    private readonly object value;

    public Value(string value) => this.value = value;

    public Value(double value) => this.value = value;

    public Value(int value) => this.value = value;

    public Value(bool value) => this.value = value;

    private Value(VaibikVoid value) => this.value = value;

    public ValueType GetValueType() => value switch
    {
        string => ValueType.ЦИТАТА,
        int => ValueType.ЦИФЕРКА,
        double => ValueType.ПОЛТОРАШКА,
        bool => ValueType.РАСКЛАД,
        VaibikVoid => ValueType.Void,
        _ => throw new InvalidOperationException("Неизвестный тип данных")
    };

    /// <summary>
    /// Возвращает значение как строку (Цитата) либо бросает исключение.
    /// </summary>
    public string AsString()
    {
        return value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value {value} is not a string"),
        };
    }

    /// <summary>
    /// Возвращает значение как целое число (Циферка) либо бросает исключение.
    /// </summary>
    public int AsInt()
    {
        return value switch
        {
            int i => i,
            double d => (int)d,
            _ => throw new InvalidOperationException($"Value {value} is not an integer"),
        };
    }

    /// <summary>
    /// Возвращает значение как число с плавающей (Полторашка) либо бросает исключение.
    /// </summary>
    public double AsDouble()
    {
        return value switch
        {
            double i => i,
            int i => (double)i,
            _ => throw new InvalidOperationException($"Value {value} is not an integer"),
        };
    }

    /// <summary>
    /// Возвращает значение как логическое (Rasklad) либо бросает исключение.
    /// </summary>
    public bool AsBool()
    {
        return value switch
        {
            bool b => b,
            _ => throw new InvalidOperationException($"Value {value} is not a boolean (Rasklad)"),
        };
    }

    /// <summary>
    /// Печатает значение для отладки.
    /// </summary>
    public override string ToString()
    {
        return value switch
        {
            string s => s,
            int i => i.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),
            bool b => b ? "ХАЙП" : "КРИНЖ",
            VoidType v => v.ToString(),
            _ => throw new InvalidOperationException($"Unexpected value {value} of type {value.GetType()}"),
        };
    }

    /// <summary>
    /// Сравнивает на равенство два значения.
    /// </summary>
    public bool Equals(Value? other)
    {
        if (other is null)
        {
            return false;
        }

        if (GetValueType() != other.GetValueType())
        {
            return false;
        }

        return value switch
        {
            string s => other.AsString() == s,
            int i => other.AsInt() == i,
            VoidType => true,
            _ => throw new NotImplementedException(),
        };
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Value);
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }
}

internal record struct VaibikVoid { public static readonly VaibikVoid Value = default; }