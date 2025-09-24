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
    public void ParseRomanWithSingleDigitNumber(string roman, int expected)
    {
        int result = TextUtil.ParseRoman(roman);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("II", 2)]
    [InlineData("III", 3)]
    [InlineData("XX", 20)]
    [InlineData("XXX", 30)]
    [InlineData("CC", 200)]
    [InlineData("CCC", 300)]
    [InlineData("MM", 2000)]
    [InlineData("MMM", 3000)]
    public void ParseRomanWithRepeatedDigits(string roman, int expected)
    {
        int result = TextUtil.ParseRoman(roman);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("IV", 4)]
    [InlineData("IX", 9)]
    [InlineData("XL", 40)]
    [InlineData("XC", 90)]
    [InlineData("CD", 400)]
    [InlineData("CM", 900)]
    public void ParseRomanWithValidSubtraction(string roman, int expected)
    {
        int result = TextUtil.ParseRoman(roman);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("XIV", 14)]
    [InlineData("XIX", 19)]
    [InlineData("XLII", 42)]
    [InlineData("XCIX", 99)]
    [InlineData("CDXLIV", 444)]
    [InlineData("CMXCIX", 999)]
    [InlineData("MMCMXCIX", 2999)]
    [InlineData("MCMLXXXIV", 1984)]
    [InlineData("MMXXIII", 2023)]
    public void ParseRomanWithCorrectValues(string roman, int expected)
    {
        int result = TextUtil.ParseRoman(roman);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ParseRomanWithEmptyString(string roman)
    {
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(roman));
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Z")]
    [InlineData("1")]
    [InlineData("IVX")]
    [InlineData("Hello")]
    public void ParseRomanWithInvalidCharacters(string roman)
    {
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(roman));
    }

    [Theory]
    [InlineData("VV")]
    [InlineData("LL")]
    [InlineData("DD")]
    [InlineData("VVV")]
    public void ParseRoman_RepeatedVLD_ThrowsArgumentException(string roman)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(roman));
    }

    [Theory]
    [InlineData("IIII")]
    [InlineData("XXXX")]
    [InlineData("CCCC")]
    [InlineData("MMMM")]
    public void ParseRomanWithUnacceptableRepeats(string roman)
    {
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(roman));
    }

    [Theory]
    [InlineData("IL")]
    [InlineData("IC")]
    [InlineData("ID")]
    [InlineData("IM")]
    [InlineData("VX")]
    [InlineData("VL")]
    [InlineData("VC")]
    [InlineData("VD")]
    [InlineData("VM")]
    [InlineData("XD")]
    [InlineData("XM")]
    [InlineData("LC")]
    [InlineData("LD")]
    [InlineData("LM")]
    [InlineData("DM")]
    public void ParseRomanWithInvalidSubstraction(string roman)
    {
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(roman));
    }

    [Theory]
    [InlineData("iv")]
    [InlineData("xix")]
    public void ParseRomanWithLowercaseLetters(string roman)
    {
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(roman));
    }

    [Theory]
    [InlineData("MMCMXCIX", 2999)]
    [InlineData("I", 1)]
    public void ParseRomanWithInBoundaries(string roman, int expected)
    {
        int result = TextUtil.ParseRoman(roman);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("MMMI")]
    public void ParseRomanWithOutOfBoundaries(string roman)
    {
        Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(roman));
    }
}