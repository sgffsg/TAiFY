namespace Lexer
{
    public enum TokenType
    {
        // Ключевые слова языка

        /// <summary>
        /// Начало блока кода ПОЕХАЛИ
        /// </summary>
        ПОЕХАЛИ,

        /// <summary>
        /// Конец блока кода ФИНАЛОЧКА
        /// </summary>
        ФИНАЛОЧКА,

        /// <summary>
        /// Объявление процедуры ПРОКРАСТИНИРУЕМ
        /// </summary>
        ПРОКРАСТИНИРУЕМ,

        /// <summary>
        /// Ввод значения ВБРОС
        /// </summary>
        ВБРОС,

        /// <summary>
        /// Вывод значения ВЫБРОС
        /// </summary>
        ВЫБРОС,

        /// <summary>
        /// Прерывание цикла ХВАТИТ
        /// </summary>
        ХВАТИТ,

        /// <summary>
        /// Переход к след. итерации цикла ПРОДОЛЖАЕМ
        /// </summary>
        ПРОДОЛЖАЕМ,

        /// <summary>
        /// Возврат из функции ДРАТУТИ
        /// </summary>
        ДРАТУТИ,

        // Типы данных

        /// <summary>
        /// Целочисленный тип ЦИФЕРКА
        /// </summary>
        ЦИФЕРКА,

        /// <summary>
        /// Число с плавающей точкой ПОЛТОРАШКА
        /// </summary>
        ПОЛТОРАШКА,

        /// <summary>
        /// Строковый тип ЦИТАТА
        /// </summary>
        ЦИТАТА,

        /// <summary>
        /// Логический тип РАСКЛАД
        /// </summary>
        РАСКЛАД,

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
        /// Целочисленный числовой литерал
        /// </summary>
        IntegerLiteral,

        /// <summary>
        /// Вещественный числовой литерал
        /// </summary>
        DoubleLiteral,

        /// <summary>
        /// Логическое "истина" ХАЙП
        /// </summary>
        ХАЙП,

        /// <summary>
        /// Логическое "ложь" КРИНЖ
        /// </summary>
        КРИНЖ,

        /// <summary>
        /// Константа БАЗА
        /// </summary>
        БАЗА,

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
        И,

        /// <summary>
        /// Логический оператор ИЛИ
        /// </summary>
        ИЛИ,

        /// <summary>
        /// Логический оператор НЕ
        /// </summary>
        НЕ,

        // Управляющие структуры

        /// <summary>
        /// Условный оператор ЕСЛИ
        /// </summary>
        ЕСЛИ,

        /// <summary>
        /// Часть условного оператора ТО
        /// </summary>
        ТО,

        /// <summary>
        /// Альтернативная ветка ИНАЧЕ
        /// </summary>
        ИНАЧЕ,

        /// <summary>
        /// Объявление цикла for ЦИКЛ
        /// </summary>
        ЦИКЛ,

        /// <summary>
        /// Объявление цикла while ПОКА
        /// </summary>
        ПОКА,

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
