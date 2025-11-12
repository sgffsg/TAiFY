# Грамматика выражений языка Vaibik

## Синтаксис выражений

Выражения в языке `Vaibik` могут содержать:

* Литералы чисел (целые `ЦИФЕРКА` и вещественные `ПОЛТОРАШКА`)
* Логические литералы (`ХАЙП`, `КРИНЖ`)
* Идентификаторы (переменные)
* Арифметические операторы (бинарные и унарные)
* Операторы сравнения
* Логические операторы (`И`, `ИЛИ`, `НЕ`)
* Встроенные константы
* Круглые скобки `()` для группировки
* Вызовы встроенных функций (напр. `abs(x)`)

## Операторы

### Арифметические операторы

| Символы / Слово | Операция                  | Ассоциативность |
|:----------------|:--------------------------|:----------------|
| `+`             | Сложение или унарный "+"  | Левая           |
| `-`             | Вычитание или унарный "-" | Левая           |
| `*`             | Умножение                 | Левая           |
| `/`             | Вещественное деление      | Левая           |
| `%`             | Остаток от деления        | Левая           |

### Операторы сравнения

| Символы | Операция         |
|:--------|:-----------------|
| `==`    | Равенство        |
| `!=`    | Неравенство      |
| `>`     | Строгое больше   |
| `>=`    | Нестрогое больше |
| `<`     | Строгое меньше   |
| `<=`    | Нестрогое меньше |

### Логические операторы

| Слово | Операция       | Ассоциативность  |
|:------|:---------------|:-----------------|
| `И`   | Логическое И   | Левая            |
| `ИЛИ` | Логическое ИЛИ | Левая            |
| `НЕ`  | Логическое НЕ  | Правая (унарный) |

## Приоритет операторов

| Приоритет (по убыванию) | Операторы                        | Ассоциативность          |
|:------------------------|:---------------------------------|:-------------------------|
| 7 (Высший)              | `( ... )`, `f(...)`              | Группировка, вызов       |
| 6                       | `+`, `-`, `НЕ`                   | Унарные (право)          |
| 5                       | `*`, `/`, `%`,                   | Мультипликативные (лево) |
| 4                       | `+`, `-`                         | Аддитивные (лево)        |
| 3                       | `==`, `!=`, `<`, `>`, `<=`, `>=` | Сравнение (не-ассоц.)    |
| 2                       | `И`                              | Логическое И (лево)      |
| 1                       | `ИЛИ`                            | Логическое ИЛИ (лево)    |

## Встроенные константы

| Константа    | Обозначение | Значение (приблизительное) |
|:-------------|:------------|:---------------------------|
| Число «пи»   | `ПИ`        | `3.1415926535`             |
| Число Эйлера | `ЕШКА`      | `2.7182818284`             |

## Встроенные  функции для чисел

* `МОДУЛЬ(x)` — возвращает модуль (абсолютное значение) числа `x`.
* `МИНИМУМ(x, y, ...)` — возвращает наименьшее из переданных чисел.
* `МАКСИМУМ(x, y, ...)` — возвращает наибольшее из переданных чисел.

## Встроенные тригонометрические функции

* `СИНУС(x)` — возвращает синус угла.
* `КОСИНУС(x)` — возвращает косинус угла.
* `ТАНГЕНС(x)` — возвращает тангенс угла.

## Грамматика в нотации EBNF

### Базовые символы

```
digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;

underscore = "_" ;

cyrillicLetter = 
    "а" | "б" | "в" | "г" | "д" | "е" | "ё" | "ж" | "з" | "и" | "й" | "к" | "л" | "м" | "н" | "о" | "п" |
    "р" | "с" | "т" | "у" | "ф" | "х" | "ц" | "ч" | "ш" | "щ" | "ъ" | "ы" | "ь" | "э" | "ю" | "я" |
    "А" | "Б" | "В" | "Г" | "Д" | "Е" | "Ё" | "Ж" | "З" | "И" | "Й" | "К" | "Л" | "М" | "Н" | "О" | "П" |
    "Р" | "С" | "Т" | "У" | "Ф" | "Х" | "Ц" | "Ч" | "Ш" | "Щ" | "Ъ" | "Ы" | "Ь" | "Э" | "Ю" | "Я" ;
    
anyChar = ? "Любой символ Unicode" ? ;

escapeSequence = "\" , ( "n" | "t" | '"' | "\" ) ;
```

### Лексемы и Синтаксис Инструкций

#### Лексемы (Токены)

```
numericLiteral = realLiteral | integerLiteral ;
integerLiteral = [ "-" | "+" ], digit, { digit } ;
realLiteral = [ "-" | "+" ], digit, { digit }, ".", digit, { digit } ;
stringLiteral = '"', { anyChar - '"' | escapeSequence }, '"' ;
logicalLiteral = "ХАЙП" | "КРИНЖ" ;
constant = "ПИ" | "ЕШКА" ;

identifier = ( cyrillicLetter | underscore ), { cyrillicLetter | digit | underscore } ;

typeName = "ЦИФЕРКА" | "ПОЛТОРАШКА" | "ЦИТАТА" | "РАСКЛАД" | "ПШИК" ;
```

#### Базовые конструкции

```
functionCall = identifier, "(", [ argumentList ], ")" ;
argumentList = expression, { ",", expression } ;
```

#### Инструкции

```
program = "ПОЕХАЛИ", statementList, "ФИНАЛОЧКА" ;
statementList = { statement } ;
block = "ПОЕХАЛИ", statementList, "ФИНАЛОЧКА" ;
blockOrStatement = statement | block ;
statement = 
      ( variableDeclaration | assignmentStatement | ifStatement | loopStatement 
      | returnStatement | breakStatement | ioStatement | functionCallStatement
      | functionDeclaration | procedureDeclaration )
    ;
variableDeclaration = 
      ( typeName, identifier, "=", expression, ";" )
    | ( "БАЗА", typeName, identifier, "=", expression, ";" )
    ;
assignmentStatement = identifier, "=", expression, ";" ;

ifStatement = 
    "ЕСЛИ", "(", expression, ")", "ТО", blockOrStatement, 
    [ "ИНАЧE", blockOrStatement ] ;

loopStatement = 
    "ЦИКЛ", "(", 
      ( assignmentStatement, ";", expression, ";", assignmentStatement ) (* 'for' стиль *)
      | expression (* 'while' стиль *)
    , ")", block ;

returnStatement = "ДРАТУТИ", expression, ";" ;

breakStatement = "ХВАТИТ", ";" ;

ioStatement = 
      ( "ВЫБРОС", "(", argumentList, ")", ";" )
    | ( "ВБРОС", "(", identifier, { ",", identifier } , ")", ";" )
    ;

functionCallStatement = functionCall, ";" ;

functionDeclaration = 
    typeName, identifier, "(", [ parameterList ], ")", block ;

procedureDeclaration = 
    "ПРОКРАСТИНИРУЕМ", identifier, "(", [ parameterList ], ")", block ;

parameterList = 
    identifier, ":", typeName, { ",", identifier, ":", typeName } ;
```

#### Грамматика Выражений

```
expression = logicalOrExpression ;
logicalOrExpression = 
    logicalAndExpression, { "ИЛИ", logicalAndExpression } ;

logicalAndExpression = 
    comparisonExpression, { "И", comparisonExpression } ;

comparisonExpression = 
    additiveExpression, 
    [ ( "==" | "!=" | "<" | ">" | "<=" | ">=" ), additiveExpression ] ;

additiveExpression = 
    multiplicativeExpression, 
    { ( "+" | "-" ), multiplicativeExpression } ;

multiplicativeExpression =
    unaryExpression, { ( "*" | "/" | "%" ), unaryExpression } ;

unaryExpression = 
    { "+" | "-" | "НЕ" } , primary ;

primary = 
      numericLiteral
    | stringLiteral
    | logicalLiteral
    | constant
    | functionCall
    | identifier
    | "(", expression, ")"
    ;
```