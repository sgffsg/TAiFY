using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Execution;

public class Scope
{
    private readonly Dictionary<string, double> variables = [];

    public bool TryGetVariable(string name, out double value)
    {
        if (variables.TryGetValue(name, out double v))
        {
            value = v;
            return true;
        }

        value = 0.0;
        return false;
    }

    public bool TryAssignVariable(string name, double value)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
            return true;
        }

        return false;
    }

    public bool TryDefineVariable(string name, double value)
    {
        return variables.TryAdd(name, value);
    }
}