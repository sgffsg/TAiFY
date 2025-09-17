using System.Text;

namespace ExampleLib;

public static class TextUtil
{
    // Символы Unicode, которые мы принимаем как дефис.
    private static readonly Rune[] Hyphens = [new Rune('‐'), new Rune('-')];

    // Символы Unicode, которые мы принимаем как апостроф.
    private static readonly Rune[] Apostrophes = [new Rune('\''), new Rune('`')];

    private static Dictionary<Rune, int> RomanValues = new Dictionary<Rune, int>
    {
        { new Rune('I'), 1 },
        { new Rune('V'), 5 },
        { new Rune('X'), 10 },
        { new Rune('L'), 50 },
        { new Rune('C'), 100 },
        { new Rune('D'), 500 },
        { new Rune('M'), 1000 }
    };

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


    public static int ParseRoman(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Пустая строка не является допустимым римским числом");
        }


        int result = 0;
        int prevValue = 0;
        Rune prevRune = default;
        int repeatCount = 1; // Начинаем с 1 для первого символа

        foreach (Rune currentRune in text.EnumerateRunes())
        {
            if (!RomanValues.ContainsKey(currentRune))
            {
                throw new ArgumentException($"Недопустимый символ '{currentRune}' в римском числе");
            }

            int currentValue = RomanValues[currentRune];

            // Проверяем повторение символов
            if (currentRune.Equals(prevRune))
            {
                repeatCount++;

                // Проверяем правила повторения
                Rune v = new Rune('V');
                Rune l = new Rune('L');
                Rune d = new Rune('D');

                if ((currentRune.Equals(v) || currentRune.Equals(l) || currentRune.Equals(d)) && repeatCount > 1)
                {
                    throw new ArgumentException($"Цифра '{currentRune}' не может повторяться");
                }

                if (repeatCount > 3)
                {
                    throw new ArgumentException($"Цифра '{currentRune}' не может повторяться более 3 раз");
                }
            }
            else
            {
                repeatCount = 1; // Сбрасываем счетчик для нового символа
            }

            // Обрабатываем вычитательную нотацию
            if (prevValue < currentValue)
            {
                if (!IsValidSubtraction(prevRune, currentRune))
                {
                    throw new ArgumentException($"Недопустимая комбинация '{prevRune}{currentRune}' для вычитания");
                }

                // Вычитаем дважды предыдущее значение (так как мы его уже добавили)
                result += currentValue - 2 * prevValue;
            }
            else
            {
                result += currentValue;
            }

            prevValue = currentValue;
            prevRune = currentRune;
        }

        if (result < 0 || result > 3000)
        {
            throw new ArgumentException($"Число {result} выходит за допустимый диапазон 0-3000");
        }

        return result;
    }

    private static bool IsValidSubtraction(Rune smaller, Rune larger)
    {
        // Допустимые комбинации для вычитания
        Rune i = new Rune('I');
        Rune v = new Rune('V');
        Rune x = new Rune('X');
        Rune l = new Rune('L');
        Rune c = new Rune('C');
        Rune d = new Rune('D');
        Rune m = new Rune('M');

        return (smaller.Equals(i) && (larger.Equals(v) || larger.Equals(x))) ||
               (smaller.Equals(x) && (larger.Equals(l) || larger.Equals(c))) ||
               (smaller.Equals(c) && (larger.Equals(d) || larger.Equals(m)));
    }
}