using System;
using System.Numerics;
using System.Text;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExampleLib;

public static class TextUtil
{
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

    private const char ROMAN_SYMBOL_ONE = 'I';
    private const char ROMAN_SYMBOL_FIVE = 'V';
    private const char ROMAN_SYMBOL_TEN = 'X';
    private const char ROMAN_SYMBOL_FIFTY = 'L';
    private const char ROMAN_SYMBOL_HUNDRED = 'C';
    private const char ROMAN_SYMBOL_FIVE_HUNDRED = 'D';
    private const char ROMAN_SYMBOL_THOUSAND = 'M';

    private const int TEN_MODIFIER = 10;
    private const int HUNDRED_MODIFIER = 100;
    private const int THOUSAND_MODIFIER = 1000;

    private static readonly RomanProcessData UNITS_PROCESS_DATA = new(ROMAN_SYMBOL_ONE, ROMAN_SYMBOL_FIVE, ROMAN_SYMBOL_TEN);
    private static readonly RomanProcessData TENS_PROCESS_DATA = new(ROMAN_SYMBOL_TEN, ROMAN_SYMBOL_FIFTY, ROMAN_SYMBOL_HUNDRED);
    private static readonly RomanProcessData HUNDREDS_PROCESS_DATA = new(ROMAN_SYMBOL_HUNDRED, ROMAN_SYMBOL_FIVE_HUNDRED, ROMAN_SYMBOL_THOUSAND);

    /// <summary>
    ///   Преобразует число из десятичной системы счисления в римскую
    /// </summary>
    /// <param name="value"> Принимает число от 0 до 3000 в десятичной системе счисления. </param>
    /// <returns> Возвращает стоку в римской системе счисления. </returns>
    /// <exception cref="ArgumentOutOfRangeException"> При выходе за пределы диапазона 0-3000 выбрасывается исключение. </exception>

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

        int thousands = value / THOUSAND_MODIFIER;
        result.Append('M', thousands);
        value %= THOUSAND_MODIFIER;

        int hundreds = value / HUNDRED_MODIFIER;
        ProcessRomanDigit(result, hundreds, HUNDREDS_PROCESS_DATA);
        value %= HUNDRED_MODIFIER;

        int tens = value / TEN_MODIFIER;
        ProcessRomanDigit(result, tens, TENS_PROCESS_DATA);
        value %= TEN_MODIFIER;

        ProcessRomanDigit(result, value, UNITS_PROCESS_DATA);

        return result.ToString();
    }

    /// <summary>
    ///   Обрабатывает цифру, в соответствии с правилами римской системы счисления.
    /// </summary>
    /// <param name="result"> Построитель строки для создания результата. </param>
    /// <param name="digit"> Преобразуемая в римскую систему счисления цифра. </param>
    /// <param name="processData"> Структура, содержащя значения единицы, середины и следующего разряда числа. </param>
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
}