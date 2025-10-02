using Xunit;

namespace ExampleLib.UnitTests;

public class TextUtilTest
{
    [Fact]
    public void Can_extract_russian_words()
    {
        const string text = """
                            Играют волны — ветер свищет,
                            И мачта гнётся и скрыпит…
                            Увы! он счастия не ищет
                            И не от счастия бежит!
                            """;
        List<string> expected =
        [
            "Играют",
            "волны",
            "ветер",
            "свищет",
            "И",
            "мачта",
            "гнётся",
            "и",
            "скрыпит",
            "Увы",
            "он",
            "счастия",
            "не",
            "ищет",
            "И",
            "не",
            "от",
            "счастия",
            "бежит",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Can_extract_words_with_hyphens()
    {
        const string text = "Что-нибудь да как-нибудь, и +/- что- то ещё";
        List<string> expected =
        [
            "Что-нибудь",
            "да",
            "как-нибудь",
            "и",
            "что",
            "то",
            "ещё",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Can_extract_words_with_apostrophes()
    {
        const string text = "Children's toys and three cats' toys";
        List<string> expected =
        [
            "Children's",
            "toys",
            "and",
            "three",
            "cats'",
            "toys",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Can_extract_words_with_grave_accent()
    {
        const string text = "Children`s toys and three cats` toys, all of''them are green";
        List<string> expected =
        [
            "Children`s",
            "toys",
            "and",
            "three",
            "cats`",
            "toys",
            "all",
            "of'",
            "them",
            "are",
            "green",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<int, string> BasicDigitsTestData()
    {
        return new TheoryData<int, string>
            {
                { 1, "I" },
                { 2, "II" },
                { 3, "III" },
                { 4, "IV" },
                { 5, "V" },
                { 6, "VI" },
                { 7, "VII" },
                { 8, "VIII" },
                { 9, "IX" },
            };
    }

    public static TheoryData<int, string> BoundaryValuesTestData()
    {
        return new TheoryData<int, string>
            {
                { 0, "N" },
                { 3000, "MMM" },
            };
    }

    public static TheoryData<int> OutOfBoundsTestData()
    {
        return new TheoryData<int>
            {
                -1,
                -100,
                3001,
                5000,
            };
    }

    public static TheoryData<int, string> RankTransitionTestData()
    {
        return new TheoryData<int, string>
            {
                { 1, "I" },
                { 4, "IV" },
                { 5, "V" },
                { 9, "IX" },
                { 10, "X" },
                { 40, "XL" },
                { 50, "L" },
                { 90, "XC" },
                { 100, "C" },
                { 400, "CD" },
                { 500, "D" },
                { 900, "CM" },
                { 1000, "M" },
                { 3000, "MMM" },
            };
    }

    public static TheoryData<int, string> ComplexValuesTestData()
    {
        return new TheoryData<int, string>
            {
                { 44, "XLIV" },
                { 49, "XLIX" },
                { 94, "XCIV" },
                { 99, "XCIX" },
                { 444, "CDXLIV" },
                { 499, "CDXCIX" },
                { 888, "DCCCLXXXVIII" },
                { 999, "CMXCIX" },
                { 1494, "MCDXCIV" },
                { 1977, "MCMLXXVII" },
                { 2008, "MMVIII" },
                { 2019, "MMXIX" },
                { 2423, "MMCDXXIII" },
            };
    }


    [Theory]
    [MemberData(nameof(BasicDigitsTestData))]
    public void FormatRomanWithBasicDigits(int input, string expected)
    {
        string result = TextUtil.FormatRoman(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(BoundaryValuesTestData))]
    public void FormatRomanWithBoundaryValues(int input, string expected)
    {
        string result = TextUtil.FormatRoman(input);
        Assert.Equal(expected, result);
    }


    [Theory]
    [MemberData(nameof(OutOfBoundsTestData))]
    public void FormatRomanWithOutOfBounds(int input)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TextUtil.FormatRoman(input));
    }

    [Theory]
    [MemberData(nameof(RankTransitionTestData))]
    public void FormatRomanWithRankTransition(int input, string expected)
    {
        string result = TextUtil.FormatRoman(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(ComplexValuesTestData))]
    public void FormatRomanWithComplexValuesTest(int input, string expected)
    {
        string result = TextUtil.FormatRoman(input);
        Assert.Equal(expected, result);
    }
}