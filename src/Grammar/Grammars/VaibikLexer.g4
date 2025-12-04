lexer grammar VaibikLexer;

// --- Ключевые слова (регистрозависимые) ---

// Блоки и структура
POEHALI: 'ПОЕХАЛИ';
FINALOCHKA: 'ФИНАЛОЧКА';
PROKRASTINIRUEM: 'ПРОКРАСТИНИРУЕМ';
DRATUTI: 'ДРАТУТИ';

// Ввод / Вывод
VBROS: 'ВБРОС';
VYBROS: 'ВЫБРОС';

// Типы данных и объявления
PACHKA: 'ПАЧКА';
CITATA_TYPE: 'ЦИТАТА';     // Тип данных
RASKLAD: 'РАСКЛАД';
POLTORASHKA: 'ПОЛТОРАШКА';
CIFERKA: 'ЦИФЕРКА';
PSHIK: 'ПШИК';
BAZA: 'БАЗА';

// Логические литералы
HAIP: 'ХАЙП';
KRINZH: 'КРИНЖ';

// Управление потоком
HVATIT: 'ХВАТИТ';
ESLI: 'ЕСЛИ';
TO: 'ТО';
PRODOLZHAEM: 'ПРОДОЛЖАЕМ';
INACHE: 'ИНАЧЕ';
CIKL: 'ЦИКЛ';
POKA: 'ПОКА';

// Логические операторы (слова)
I: 'И';
ILI: 'ИЛИ';
NE: 'НЕ';

// Встроенные константы
PI: 'ПИ';
ESHKA: 'ЕШКА';

// Комментарии
POYASNITELNAYA_BRIGADA: '(ПОЯСНИТЕЛЬНАЯ-БРИГАДА:';
FINALOCHKA_KOMMENTARIYA: 'ФИНАЛОЧКА-КОММЕНТАРИЯ)';

// Встроенные функции (если нужно парсить их как ключевые слова, иначе как ID)
// В спецификации они выглядят как обычные вызовы, но для подсветки можно выделить.
MODUL: 'МОДУЛЬ';
MINIMUM: 'МИНИМУМ';
MAXIMUM: 'МАКСИМУМ';
SINUS: 'СИНУС';
KOSINUS: 'КОСИНУС';
TANGENS: 'ТАНГЕНС';

// --- Операторы ---
PLUS: '+';
MINUS: '-';
MULT: '*';
DIV: '/';
MOD: '%';
EQ: '==';
NEQ: '!=';
LT: '<';
GT: '>';
LTE: '<=';
GTE: '>=';
ASSIGN: '=';

// --- Разделители и скобки ---
SEMI: ';';
COMMA: ',';
DOT: '.';
LPAREN: '(';
RPAREN: ')';
LBRACE: '{';
RBRACE: '}';
LBRACK: '[';
RBRACK: ']';
COLON: ':';

// --- Идентификаторы и Литералы ---

// Идентификаторы: Кириллица или _, затем кириллица, цифры или _
ID: (CYRILLIC_LETTER | '_') (CYRILLIC_LETTER | DIGIT | '_')*;

// Числа
INTEGER: [0-9]+; // Знак обрабатывается в парсере как унарный оператор
REAL: [0-9]+ '.' [0-9]+;

// Строки
STRING: '"' ( ~["\\] | ESCAPE_SEQUENCE )* '"';

// --- Пропуск пробелов и комментариев ---

WS: [ \t\r\n]+ -> skip;

// Многострочный комментарий (жадный)
COMMENT: POYASNITELNAYA_BRIGADA .*? FINALOCHKA_KOMMENTARIYA -> skip;

// Однострочный комментарий (до конца строки)
// Грамматика гласит: (ПОЯСНИТЕЛЬНАЯ-БРИГАДА: текст)\n
SINGLE_LINE_COMMENT: POYASNITELNAYA_BRIGADA ~[\r\n]* -> skip;

// --- Фрагменты ---
fragment DIGIT: '0'..'9';
fragment CYRILLIC_LETTER: [\u0410-\u042F\u0430-\u044F\u0401\u0451]; // А-Я, а-я, Ё, ё
fragment ESCAPE_SEQUENCE: '\\' ('n' | 't' | '"' | '\\');