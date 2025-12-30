# Грамматика программы языка Vaibik

## 1. Пример кода

Программа на языке Vaibik состоит из набора функций. Точкой входа является процедура с именем `ПОГНАЛИ`.

```
(ПОЯСНИТЕЛЬНАЯ-БРИГАДА: Глобальная константа)
БАЗА ЦИФЕРКА МАКСИМУМ = 100;

(ПОЯСНИТЕЛЬНАЯ-БРИГАДА: Вспомогательная функция)
ЦИФЕРКА сложить(а: ЦИФЕРКА, б: ЦИФЕРКА) 
ПОЕХАЛИ
    ДРАТУТИ а + б;
ФИНАЛОЧКА

(ПОЯСНИТЕЛЬНАЯ-БРИГАДА: Точка входа)
ПРОКРАСТИНИРУЕМ ПОГНАЛИ() 
ПОЕХАЛИ
    ЦИФЕРКА счетчик = 0;
    ЦИФЕРКА ввод = 0;
    
    ВЫБРОС("Введите число: ");
    ВБРОС(ввод);

    ЕСЛИ (ввод > МАКСИМУМ) ТО
        ВЫБРОС("Слишком много!");
    ИНАЧЕ ПОЕХАЛИ
        счетчик = сложить(ввод, 10);
        ВЫБРОС("Результат: ", счетчик);
    ФИНАЛОЧКА;
ФИНАЛОЧКА
```

## 2. Архитектурные решения

### 2.1. Ввод и вывод

Для взаимодействия с консолью выбраны специальные инструкции, а не встроенные функции. Это позволяет компилятору
жестко контролировать типы аргументов.

* `ВЫБРОС(...)` — инструкция вывода. Может принимать список аргументов.
* `ВБРОС(...)` — инструкция ввода. Принимает переменные, в которые нужно записать данные.

### 2.2. Виды инструкций

Чтобы исключить бессмысленный код, введено правило: не любое выражение является инструкцией.
Инструкцией может быть только выражение, имеющее побочный эффект:

* Присваивание (`x = 10;`)
* Вызов функции (`foo();`)
  Простое сложение (`5 + 5;`) инструкцией не является и вызовет ошибку компиляции.

### 2.3. Разделитель инструкций

Используется символ **«;»** (точка с запятой).
Это упрощает разбор и позволяет писать несколько инструкций в одну строку, игнорируя переносы строк.

## 4. Грамматика EBNF

```
program = { topLevelItem } , EOF ;

topLevelItem 
    = procedureDeclaration 
    | constantDeclaration
    | typedDeclaration 
    ;

constantDeclaration = 
    "БАЗА", typeName, identifier, "=", expression, ";" ;

typedDeclaration = 
    typeName, identifier, ( functionTail | variableTail ) ;

functionTail = 
    "(", [ parameterList ], ")", block ;

variableTail = 
    "=", expression, ";" ;

procedureDeclaration = 
    "ПРОКРАСТИНИРУЕМ", identifier, "(", [ parameterList ], ")", block ;

parameterList = 
    identifier, ":", typeName, { ",", identifier, ":", typeName } ;

block = "ПОЕХАЛИ", { statement }, "ФИНАЛОЧКА" ;

statement 
    = variableDeclaration
    | ifStatement
    | whileStatement
    | forStatement
    | returnStatement
    | breakStatement
    | continueStatement
    | ioStatement
    | block       
    | sideEffectStatement
    | ";"
    ;

variableDeclaration = 
    typeName, identifier, "=", expression, ";" ;

ifStatement = 
    "ЕСЛИ", "(", expression, ")", "ТО", statement, 
    [ "ИНАЧЕ", statement ] ;

whileStatement = 
    "ПОКА", "(", expression, ")", block ;

forStatement = 
    "ЦИКЛ", "(", variableAssignment, ";", expression, ";", variableAssignment, ")", block ;

returnStatement = "ДРАТУТИ", [ expression ], ";" ;

breakStatement = "ХВАТИТ", ";" ;

continueStatement = "ПРОДОЛЖАЕМ", ";" ;

ioStatement 
    = ( "ВЫБРОС", "(", [ argumentList ], ")", ";" )
    | ( "ВБРОС", "(", identifier, { ",", identifier }, ")", ";" )
    ;
    
sideEffectStatement = identifier, ( assignmentTail | indexAssignmentTail | callTail ), ";" ;

assignmentTail = "=", expression ;

indexAssignmentTail = "[", expression, "]", "=", expression ;

callTail       = "(", [ argumentList ], ")" ;

variableAssignment = identifier, "=", expression ;
```

