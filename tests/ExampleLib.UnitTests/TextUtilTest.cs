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

    [Theory]
    [InlineData(1, "I")]
    [InlineData(2, "II")]
    [InlineData(3, "III")]
    [InlineData(4, "IV")]
    [InlineData(5, "V")]
    [InlineData(6, "VI")]
    [InlineData(7, "VII")]
    [InlineData(8, "VIII")]
    [InlineData(9, "IX")]
    public void FormatRomanWithBasicDigits(int input, string expected)
    {
        string result = TextUtil.FormatRoman(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, "N")]
    [InlineData(3000, "MMM")]
    public void FormatRomanWithBoundaryValues(int input, string expected)
    {
        string result = TextUtil.FormatRoman(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(3001)]
    [InlineData(5000)]
    public void FormatRomanWithOutOfBounds(int input)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TextUtil.FormatRoman(input));
    }

    [Theory]
    [InlineData(1, "I")]
    [InlineData(4, "IV")]
    [InlineData(5, "V")]
    [InlineData(9, "IX")]
    [InlineData(10, "X")]
    [InlineData(40, "XL")]
    [InlineData(50, "L")]
    [InlineData(90, "XC")]
    [InlineData(100, "C")]
    [InlineData(400, "CD")]
    [InlineData(500, "D")]
    [InlineData(900, "CM")]
    [InlineData(1000, "M")]
    [InlineData(3000, "MMM")]
    public void FormatRomanWithRankTransition(int input, string expected)
    {
        string result = TextUtil.FormatRoman(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(44, "XLIV")]
    [InlineData(49, "XLIX")]
    [InlineData(94, "XCIV")]
    [InlineData(99, "XCIX")]
    [InlineData(444, "CDXLIV")]
    [InlineData(499, "CDXCIX")]
    [InlineData(888, "DCCCLXXXVIII")]
    [InlineData(999, "CMXCIX")]
    [InlineData(1494, "MCDXCIV")]
    [InlineData(1977, "MCMLXXVII")]
    [InlineData(2008, "MMVIII")]
    [InlineData(2019, "MMXIX")]
    [InlineData(2423, "MMCDXXIII")]
    public void FormatRomanWithComplexValuesTest(int input, string expected)
    {
        string result = TextUtil.FormatRoman(input);
        Assert.Equal(expected, result);
    }
}