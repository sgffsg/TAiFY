using System.IO;

using ExampleLib.UnitTests.Helpers;

using Xunit;

namespace ExampleLib.UnitTests;

public class FileUtilTests
{
    [Fact]
    public void CanSortTextFile()
    {
        const string unsorted = """
                                Играют волны — ветер свищет,
                                И мачта гнется и скрыпит…
                                Увы! он счастия не ищет
                                И не от счастия бежит!
                                """;
        const string sorted = """
                              И мачта гнется и скрыпит…
                              И не от счастия бежит!
                              Играют волны — ветер свищет,
                              Увы! он счастия не ищет
                              """;

        using TempFile file = TempFile.Create(unsorted);
        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal(sorted.Replace("\r\n", "\n"), actual);
    }

    [Fact]
    public void CanSortOneLineFile()
    {
        using TempFile file = TempFile.Create("Играют волны — ветер свищет,");
        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal("Играют волны — ветер свищет,", actual);
    }

    [Fact]
    public void CanSortEmptyFile()
    {
        using TempFile file = TempFile.Create("");

        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal("", actual);
    }

    [Fact]
    public void CanSetLineNumbersToEmptyFile()
    {
        using TempFile file = TempFile.Create(string.Empty);
        FileUtil.AddLineNumbers(file.Path);

        string result = File.ReadAllText(file.Path);
        Assert.Empty(result);
    }

    [Fact]
    public void CanSetNumbersToSingleLineFile()
    {
        const string baseText = "Second line";
        using TempFile file = TempFile.Create(baseText);
        FileUtil.AddLineNumbers(file.Path);

        string[] result = File.ReadAllLines(file.Path);
        Assert.Single(result);
        Assert.Equal("1. Second line", result[0]);
    }


    [Fact]
    public void CanSetNumbersToMultiLineFile()
    {
        const string baseText = "First line\nSecond line\nThird line";
        using TempFile file = TempFile.Create(baseText);
        FileUtil.AddLineNumbers(file.Path);

        string[] result = File.ReadAllLines(file.Path);
        Assert.Equal(3, result.Length);
        Assert.Equal("1. First line", result[0]);
        Assert.Equal("2. Second line", result[1]);
        Assert.Equal("3. Third line", result[2]);
    }

    [Fact]
    public void CanSetNumbersToFileWithEmptyLines()
    {
        const string baseText = "First line\n\nThird line";
        using TempFile file = TempFile.Create(baseText);
        FileUtil.AddLineNumbers(file.Path);

        string[] result = File.ReadAllLines(file.Path);
        Assert.Equal(3, result.Length);
        Assert.Equal("1. First line", result[0]);
        Assert.Equal("2. ", result[1]);
        Assert.Equal("3. Third line", result[2]);
    }
}