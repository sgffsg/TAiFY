lexer grammar VaibikLexer;

// Ключевые слова (регистрозависимые)
POEHALI: 'ПОЕХАЛИ';
FINALOCHKA: 'ФИНАЛОЧКА';
PROKRASTINIRUEM: 'ПРОКРАСТИНИРУЕМ';
VBROS: 'ВБРОС';
VYBROS: 'ВЫБРОС';
PACHKA: 'ПАЧКА';
CITATA: 'ЦИТАТА';
HAIP: 'ХАЙП';
KRINZH: 'КРИНЖ';
RASKLAD: 'РАСКЛАД';
POLTORASHKA: 'ПОЛТОРАШКА';
DRATUTI: 'ДРАТУТИ';
CIFERKA: 'ЦИФЕРКА';
PSHIK: 'ПШИК';
HVATIT: 'ХВАТИТ';
BAZA: 'БАЗА';
ESLI: 'ЕСЛИ';
TO: 'ТО';
INACHE: 'ИНАЧЕ';
CIKL: 'ЦИКЛ';
I: 'И';
ILI: 'ИЛИ';
NE: 'НЕ';
POYASNITELNAYA_BRIGADA: '(ПОЯСНИТЕЛЬНАЯ-БРИГАДА:';
FINALOCHKA_KOMMENTARIYA: 'ФИНАЛОЧКА-КОММЕНТАРИЯ)';

// Встроенные константы
PI: 'ПИ';
ESHKA: 'ЕШКА';

// Встроенные функции
MODUL: 'МОДУЛЬ';
MINIMUM: 'МИНИМУМ';
MAXIMUM: 'МАКСИМУМ';
SINUS: 'СИНУС';
KOSINUS: 'КОСИНУС';
TANGENS: 'ТАНГЕНС';

// Операторы
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

// Разделители и скобки
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

// Идентификаторы и литералы
ID: (CYRILLIC_LETTER | '_') (CYRILLIC_LETTER | DIGIT | '_')*;
INTEGER: [+-]? DIGIT+;
REAL: [+-]? DIGIT+ '.' DIGIT+;
STRING: '"' ( ~["] | ESCAPE_SEQUENCE )* '"';

// Пропуск пробелов и комментариев
WS: [ \t\r\n]+ -> skip;
COMMENT: POYASNITELNAYA_BRIGADA (~[Ф] | 'Ф' ~[И] | 'ФИ' ~[Н] | 'ФИН' ~[А] | 'ФИНА' ~[Л] | 'ФИНАЛ' ~[О] | 'ФИНАЛО' ~[Ч] | 'ФИНАЛОЧ' ~[К] | 'ФИНАЛОЧК' ~[А] | 'ФИНАЛОЧКА' ~[-] | 'ФИНАЛОЧКА-' ~[К] | 'ФИНАЛОЧКА-К' ~[О] | 'ФИНАЛОЧКА-КО' ~[М] | 'ФИНАЛОЧКА-КОМ' ~[М] | 'ФИНАЛОЧКА-КОММ' ~[Е] | 'ФИНАЛОЧКА-КОММЕ' ~[Н] | 'ФИНАЛОЧКА-КОММЕН' ~[Т] | 'ФИНАЛОЧКА-КОММЕНТ' ~[А] | 'ФИНАЛОЧКА-КОММЕНТА' ~[Р] | 'ФИНАЛОЧКА-КОММЕНТАР' ~[И] | 'ФИНАЛОЧКА-КОММЕНТАРИ' ~[Я] | 'ФИНАЛОЧКА-КОММЕНТАРИЯ' ~[)])* FINALOCHKA_KOMMENTARIYA -> skip;
SINGLE_LINE_COMMENT: POYASNITELNAYA_BRIGADA ~[\r\n]* '\r'? '\n' -> skip;

// Фрагменты
fragment DIGIT: '0'..'9';
fragment CYRILLIC_LETTER: [\u0410-\u042F\u0430-\u044F];
fragment ESCAPE_SEQUENCE: '\\' ('n' | 't' | '"' | '\\');