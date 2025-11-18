namespace Lexer
{
    public enum TokenType
    {
        // Ключевые слова языка

        /// <summary>
        /// Начало блока кода ПОЕХАЛИ
        /// </summary>
        Poehali,

        /// <summary>
        /// Конец блока кода ФИНАЛОЧКА
        /// </summary>
        Finalochka,

        /// <summary>
        /// Объявление процедуры ПРОКРАСТИНИРУЕМ
        /// </summary>
        Prokrastiniryem,

        /// <summary>
        /// Ввод значения ВБРОС
        /// </summary>
        Vbros,

        /// <summary>
        /// Вывод значения ВЫБРОС
        /// </summary>
        Vybros,

        /// <summary>
        /// Прерывание цикла ХВАТИТ
        /// </summary>
        Hvatit,

        /// <summary>
        /// Возврат из функции ДРАТУТИ
        /// </summary>
        Dratuti,

        // Типы данных

        /// <summary>
        /// Целочисленный тип ЦИФЕРКА
        /// </summary>
        Ciferka,

        /// <summary>
        /// Число с плавающей точкой ПОЛТОРАШКА
        /// </summary>
        Poltorashka,

        /// <summary>
        /// Строковый тип ЦИТАТА
        /// </summary>
        Citata,

        /// <summary>
        /// Логический тип РАСКЛАД
        /// </summary>
        Rasklad,

        /// <summary>
        /// Массив ПАЧКА
        /// </summary>
        Pachka,

        // Литералы и значения

        /// <summary>
        /// Идентификатор (имя переменной, функции)
        /// </summary>
        Identifier,

        /// <summary>
        /// Строковый литерал
        /// </summary>
        StringLiteral,

        /// <summary>
        /// Числовой литерал float/int
        /// </summary>
        NumericLiteral,

        /// <summary>
        /// Логическое "истина" ХАЙП
        /// </summary>
        Hype,

        /// <summary>
        /// Логическое "ложь" КРИНЖ
        /// </summary>
        Cringe,

        /// <summary>
        /// Null-значение ПШИК
        /// </summary>
        Pshik,

        /// <summary>
        /// Константа БАЗА
        /// </summary>
        Baza,

        // Операторы сравнения

        /// <summary>
        /// Оператор сравнения равно "==".
        /// </summary>
        Equal,

        /// <summary>
        /// Оператор сравнения не равно "!=".
        /// </summary>
        NotEqual,

        /// <summary>
        /// Оператор сравнения меньше "<".
        /// </summary>
        LessThan,

        /// <summary>
        /// Оператор сравнения меньше или равно "<=".
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Оператор сравнения больше ">".
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Оператор сравнения больше или равно ">=".
        /// </summary>
        GreaterThanOrEqual,

        // Арифметические операторы

        /// <summary>
        /// Арифметический оператор плюс "+"
        /// </summary>
        Plus,

        /// <summary>
        /// Арифметический оператор минус "-"
        /// </summary>
        Minus,

        /// <summary>
        /// Арифметический оператор умножения "*"
        /// </summary>
        Multiplication,

        /// <summary>
        /// Арифметический оператор деления "/"
        /// </summary>
        Division,

        /// <summary>
        /// Арифметический оператор остаток "%"
        /// </summary>
        Remainder,

        // Логические операторы

        /// <summary>
        /// Логический оператор И
        /// </summary>
        And,

        /// <summary>
        /// Логический оператор ИЛИ
        /// </summary>
        Or,

        /// <summary>
        /// Логический оператор НЕ
        /// </summary>
        Not,

        // Управляющие структуры

        /// <summary>
        /// Условный оператор ЕСЛИ
        /// </summary>
        Esli,

        /// <summary>
        /// Часть условного оператора ТО
        /// </summary>
        To,

        /// <summary>
        /// Альтернативная ветка ИНАЧЕ
        /// </summary>
        Inache,

        /// <summary>
        /// Объявление цикла ЦИКЛ
        /// </summary>
        Cikl,

        // Разделители и скобки

        /// <summary>
        /// Оператор присваивания равно "=".
        /// </summary>
        Assignment,

        /// <summary>
        /// Оператор указания типа (разделитель типа) ":"
        /// </summary>
        ColonTypeIndication,

        /// <summary>
        /// Оператор доступа к полю структуры "."
        /// </summary>
        DotFieldAccess,

        /// <summary>
        /// Открывающая круглая скобка '('.
        /// </summary>
        OpenParenthesis,

        /// <summary>
        ///  Закрывающая круглая скобка ')'.
        /// </summary>
        CloseParenthesis,

        /// <summary>
        ///  Открывающая квадратная скобка '['.
        /// </summary>
        OpenSquareBracket,

        /// <summary>
        ///  Закрывающая квадратная скобка ']'.
        /// </summary>
        CloseSquareBracket,

        /// <summary>
        /// Открывающая фигурная скобка '{'.
        /// </summary>
        OpenCurlyBrace,

        /// <summary>
        /// Закрывающая фигурная скобка '}'.
        /// </summary>
        CloseCurlyBrace,

        /// <summary>
        /// Разделитель элементов ','
        /// </summary>
        Comma,

        /// <summary>
        /// Конец инструкции ';'
        /// </summary>
        Semicolon,

        /// <summary>
        /// Строка '"'
        /// </summary>
        DoubleQuote,

        // Комментарии

        /// <summary>
        /// Однострочные комментарии (ПОЯСНИТЕЛЬНАЯ-БРИГАДА: ...)
        /// </summary>
        SingleLineComment,

        /// <summary>
        /// Многострочные комментарии (ПОЯСНИТЕЛЬНАЯ-БРИГАДА: ... ФИНАЛОЧКА-КОММЕНТАРИЯ)
        /// </summary>
        MultiLineComment,

        // Встроенные константы

        /// <summary>
        /// Константа 3.14159265358
        /// </summary>
        PI,

        /// <summary>
        /// Константа 2.71828182846
        /// </summary>
        EULER,

        // Встроенные функции

        /// <summary>
        /// Модуль числа МОДУЛЬ(X)
        /// </summary>
        Module,

        /// <summary>
        /// Минимум числа МИНИМУМ(x, y, ...)
        /// </summary>
        Minimum,

        /// <summary>
        /// Максимум числа МАКСИМУМ(x, y, ...)
        /// </summary>
        Maximum,

        /// <summary>
        /// Возведение числа в степень СТЕПЕНЬ()
        /// </summary>
        Pow,

        /// <summary>
        /// Извлечение корня из числа Корень(X)
        /// </summary>
        Sqrt,

        /// <summary>
        /// Синус угла СИНУС(X)
        /// </summary>
        Sinus,

        /// <summary>
        /// Косинус угла КОСИНУС(X)
        /// </summary>
        Cosinus,

        /// <summary>
        /// Тангенс угла ТАНГЕНС(X)
        /// </summary>
        Tangens,

        // Служебные слова

        /// <summary>
        /// Конец файла.
        /// </summary>
        EOF,

        /// <summary>
        /// Недопустимая лексема.
        /// </summary>
        Error,
    }
}
