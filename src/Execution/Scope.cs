using Runtime;

namespace Execution;

public class Scope
{
    private readonly Dictionary<string, Value> variables = new();
    private readonly HashSet<string> constants = new();

    public void Define(string name, Value value, bool isConstant)
    {
        variables[name] = value;
        if (isConstant)
        {
            constants.Add(name);
        }
    }

    public bool IsConstant(string name) => constants.Contains(name);

    public bool TryGetValue(string name, out Value? value) => variables.TryGetValue(name, out value);

    public void Assign(string name, Value value) => variables[name] = value;

    public bool Contains(string name) => variables.ContainsKey(name);
}