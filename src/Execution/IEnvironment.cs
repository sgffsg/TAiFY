namespace Execution;

public interface IEnvironment
{
    double ReadNumber();

    string ReadString();

    void Write(double value);
}
