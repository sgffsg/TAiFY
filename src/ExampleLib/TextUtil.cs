using System;
using System.Numerics;
using System.Text;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExampleLib;

public static class TextUtil
{
    private const char RomanSymbolOne = 'I';
    private const char RomanSymbolFive = 'V';
    private const char RomanSymbolTen = 'X';
    private const char RomanSymbolFivty = 'L';
    private const char RomanSymbolHundred = 'C';
    private const char RomanSymbolFiveHundred = 'D';
    private const char RomanSymbolThousand = 'M';

    private const int TenModifier = 10;
    private const int HundredModifier = 100;
    private const int ThousandModifier = 1000;

    private static readonly RomanProcessData UnitsProcessData = new(RomanSymbolOne, RomanSymbolFive, RomanSymbolTen);
    private static readonly RomanProcessData TensProcessData = new(RomanSymbolTen, RomanSymbolFivty, RomanSymbolHundred);
    private static readonly RomanProcessData HundredsProcessData = new(RomanSymbolHundred, RomanSymbolFiveHundred, RomanSymbolThousand);

    // Символы Unicode, которые мы принимаем как дефис.
    private static readonly Rune[] Hyphens = [new Rune('‐'), new Rune('-')];

    // Символы Unicode, которые мы принимаем как апостроф.
    private static readonly Rune[] Apostrophes = [new Rune('\''), new Rune('`')];

    // Состояния распознавателя слов.
    private enum WordState
    {
        NoWord,
        Letter,
        Hyphen,
        Apostrophe,
    }

    /// <summary>
    ///  Распознаёт слова в тексте. Поддерживает Unicode, в том числе английский и русский языки.
    ///  Слово состоит из букв, может содержать дефис в середине и апостроф в середине либо в конце.
    /// </summary>
    /// <remarks>
    ///  Функция использует автомат-распознаватель с четырьмя состояниями:
    ///   1. NoWord — автомат находится вне слова;
    ///   2. Letter — автомат находится в буквенной части слова;
    ///   3. Hyphen — автомат обработал дефис;
    ///   4. Apostrophe — автомат обработал апостроф.
    ///
    ///  Переходы между состояниями:
    ///   - NoWord → Letter — при получении буквы;
    ///   - Letter → Hyphen — при получении дефиса;
    ///   - Letter → Apostrophe — при получении апострофа;
    ///   - Letter → NoWord — при получении любого символа, кроме буквы, дефиса или апострофа;
    ///   - Hyphen → Letter — при получении буквы;
    ///   - Hyphen → NoWord — при получении любого символа, кроме буквы;
    ///   - Apostrophe → Letter — при получении буквы;
    ///   - Apostrophe → NoWord — при получении любого символа, кроме буквы.
    ///
    ///  Разница между состояниями Hyphen и Apostrophe в том, что дефис не может стоять в конце слова.
    /// </remarks>
    public static List<string> ExtractWords(string text)
    {
        WordState state = WordState.NoWord;

        List<string> results = [];
        StringBuilder currentWord = new();
        foreach (Rune ch in text.EnumerateRunes())
        {
            switch (state)
            {
                case WordState.NoWord:
                    if (Rune.IsLetter(ch))
                    {
                        PushCurrentWord();
                        currentWord.Append(ch);
                        state = WordState.Letter;
                    }

                    break;

                case WordState.Letter:
                    if (Rune.IsLetter(ch))
                    {
                        currentWord.Append(ch);
                    }
                    else if (Hyphens.Contains(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Hyphen;
                    }
                    else if (Apostrophes.Contains(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Apostrophe;
                    }
                    else
                    {
                        state = WordState.NoWord;
                    }

                    break;

                case WordState.Hyphen:
                    if (Rune.IsLetter(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Letter;
                    }
                    else
                    {
                        // Убираем дефис, которого не должно быть в конце слова.
                        currentWord.Remove(currentWord.Length - 1, 1);
                        state = WordState.NoWord;
                    }

                    break;

                case WordState.Apostrophe:
                    if (Rune.IsLetter(ch))
                    {
                        currentWord.Append(ch);
                        state = WordState.Letter;
                    }
                    else
                    {
                        state = WordState.NoWord;
                    }

                    break;
            }
        }

        PushCurrentWord();

        return results;

        void PushCurrentWord()
        {
            if (currentWord.Length > 0)
            {
                results.Add(currentWord.ToString());
                currentWord.Clear();
            }
        }
    }

    public static string FormatRoman(int value)
    {
        StringBuilder result = new StringBuilder();

        if (value < 0 || value > 3000)
        {
            throw new ArgumentOutOfRangeException("Число должно быть в диапазоне от 0 до 3000");
        }

        if (value == 0)
        {
            return "N";
        }

        int thousands = value / ThousandModifier;
        result.Append('M', thousands);
        value %= ThousandModifier;

        int hundreds = value / HundredModifier;
        ProcessRomanDigit(result, hundreds, HundredsProcessData);
        value %= HundredModifier;

        int tens = value / TenModifier;
        ProcessRomanDigit(result, tens, TensProcessData);
        value %= TenModifier;

        ProcessRomanDigit(result, value, UnitsProcessData);

        return result.ToString();
    }

    private static void ProcessRomanDigit(StringBuilder result, int digit, RomanProcessData processData)
    {
        switch (digit)
        {
            case 1:
            case 2:
            case 3:
                result.Append(processData.UnitSymbol, digit);
                break;
            case 4:
                result.Append(processData.UnitSymbol);
                result.Append(processData.FiveSymbol);
                break;
            case 5:
                result.Append(processData.FiveSymbol);
                break;
            case 6:
            case 7:
            case 8:
                result.Append(processData.FiveSymbol);
                result.Append(processData.UnitSymbol, digit - 5);
                break;
            case 9:
                result.Append(processData.UnitSymbol);
                result.Append(processData.TenSymbol);
                break;
        }
    }

    public struct RomanProcessData
    {
        public char UnitSymbol;
        public char FiveSymbol;
        public char TenSymbol;

        public RomanProcessData(char unitSymbol, char fiveSymbol, char tenSymbol)
        {
            this.UnitSymbol = unitSymbol;
            this.FiveSymbol = fiveSymbol;
            this.TenSymbol = tenSymbol;
        }
    }
}