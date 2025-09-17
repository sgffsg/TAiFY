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
    [InlineData("I", 1)]
    [InlineData("V", 5)]
    [InlineData("X", 10)]
    [InlineData("L", 50)]
    [InlineData("C", 100)]
    [InlineData("D", 500)]
    [InlineData("M", 1000)]
    [InlineData("II", 2)]
    [InlineData("III", 3)]
    [InlineData("IV", 4)]
    [InlineData("VI", 6)]
    [InlineData("VII", 7)]
    [InlineData("VIII", 8)]
    [InlineData("IX", 9)]
    [InlineData("XIV", 14)]
    [InlineData("XIX", 19)]
    [InlineData("XL", 40)]
    [InlineData("XC", 90)]
    [InlineData("CD", 400)]
    [InlineData("CM", 900)]
    [InlineData("MMXXIV", 2024)]
    [InlineData("MMM", 3000)]
    public void ParseRoman_ValidNumbers_ReturnsCorrectValue(string roman, int expected)
    {
        int result = TextUtil.ParseRoman(roman);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("IIII")]
    [InlineData("VV")]
    [InlineData("LL")]
    [InlineData("DD")]
    [InlineData("IIIIV")]
    [InlineData("VIV")]
    [InlineData("IC")]
    [InlineData("IL")]
    [InlineData("ID")]
    [InlineData("IM")]
    [InlineData("XD")]
    [InlineData("XM")]
    [InlineData("LC")]
    [InlineData("LD")]
    [InlineData("LM")]
    [InlineData("DM")]
    public void ParseRoman_InvalidFormat_ThrowsArgumentException(string input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(input));
    }

    [Fact]
    public void CanParseTooLargeRomanNumber()
    {
        string largeRoman = "MMMI"; // 3001
        ArgumentException exception = Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(largeRoman));
        Assert.Contains("выходит за допустимый диапазон 0-3000", exception.Message);
    }

    [Fact]
    public void CheckParseRoman()
    {
        Assert.Equal(39,    TextUtil.ParseRoman("XXXIX"));
        Assert.Equal(246,   TextUtil.ParseRoman("CCXLVI"));
        Assert.Equal(789,   TextUtil.ParseRoman("DCCLXXXIX"));
        Assert.Equal(2421,  TextUtil.ParseRoman("MMCDXXI"));
        Assert.Equal(160,   TextUtil.ParseRoman("CLX"));
        Assert.Equal(207,   TextUtil.ParseRoman("CCVII"));
        Assert.Equal(1009,  TextUtil.ParseRoman("MIX"));
        Assert.Equal(1066,  TextUtil.ParseRoman("MLXVI"));
    }
}