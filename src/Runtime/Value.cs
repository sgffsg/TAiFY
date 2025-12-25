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
        string => ValueType.Citata,
        int => ValueType.Ciferka,
        double => ValueType.Poltorashka,
        bool => ValueType.Rasklad,
        VaibikVoid => ValueType.Void,
        _ => throw new InvalidOperationException("Неизвестный тип данных")
    };

    public string AsString() => value is string s ? s : throw new InvalidCastException("Ожидалась ЦИТАТА");

    public int AsInt() => value is int i ? i : throw new InvalidCastException("Ожидалась ЦИФЕРКА");

    public double AsDouble() => value is double d ? d : throw new InvalidCastException("Ожидалась ПОЛТОРАШКА");

    public bool AsBool() => value is bool b ? b : throw new InvalidCastException("Ожидался РАСКЛАД");

    public bool Equals(Value? other)
    {
        if (other is null || GetValueType() != other.GetValueType())
        {
            return false;
        }

        return Equals(value, other.value);
    }

    public override bool Equals(object? obj) => Equals(obj as Value);

    public override int GetHashCode() => value.GetHashCode();
}

internal record struct VaibikVoid { public static readonly VaibikVoid Value = default; }