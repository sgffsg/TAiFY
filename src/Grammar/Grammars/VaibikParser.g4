parser grammar VaibikParser;

options { tokenVocab=VaibikLexer; }

// --- Основная структура программы ---
program: topLevelItem* EOF;

topLevelItem:
      functionDeclaration
    | procedureDeclaration
    | globalVariableDeclaration
    ;

// --- Объявления функций и процедур ---

functionDeclaration:
    typeName ID LPAREN parameterList? RPAREN block;

procedureDeclaration:
    PROKRASTINIRUEM ID LPAREN parameterList? RPAREN block;

parameterList:
    parameter (COMMA parameter)*;

parameter:
    ID COLON fullType;

// --- Типы данных ---

// Полный тип может быть простым или массивом
fullType: typeName | arrayType;

typeName:
      CIFERKA
    | POLTORASHKA
    | CITATA_TYPE
    | RASKLAD
    | PSHIK
    ;

// Синтаксис массива: ПАЧКА[10] ЦИФЕРКА
arrayType: PACHKA LBRACK expression RBRACK typeName;

// --- Объявления переменных ---

globalVariableDeclaration:
    BAZA? fullType ID ASSIGN expression SEMI;

// Внутри блока переменная может быть объявлена без БАЗА, но синтаксис схож
variableDeclaration:
    fullType ID (ASSIGN expression)? SEMI;

// --- Блоки и Инструкции ---

block: POEHALI statementList FINALOCHKA;

statementList: statement*;

statement:
      variableDeclaration
    | expressionStatement
    | arrayAssignmentStatement
    | ifStatement
    | loopStatement
    | whileStatement
    | returnStatement
    | ioStatement
    | sideEffectStatement
    | block
    | SEMI
    ;

// Присваивание переменной (теперь как часть выражения)
expressionStatement: expression SEMI;

// Присваивание элементу массива: arr[0] = 1;
arrayAssignmentStatement: ID LBRACK expression RBRACK ASSIGN expression SEMI;

// Вызов функции/процедуры как отдельная инструкция (если результат не нужен)
sideEffectStatement: functionCall SEMI;

// --- Управляющие конструкции ---

ifStatement:
    ESLI LPAREN expression RPAREN TO statement (INACHE statement)?;

// Инструкции, которые могут быть только внутри циклов
loopOnlyStatement:
      breakStatement
    | continueStatement
    ;

// Специальные блоки для циклов, которые разрешают break/continue
loopBody: POEHALI (statement | loopOnlyStatement)* FINALOCHKA;

whileBody: POEHALI (statement | loopOnlyStatement)* FINALOCHKA;

loopStatement:
    CIKL LPAREN assignmentExpr SEMI expression SEMI assignmentExpr RPAREN loopBody;

whileStatement:
    POKA LPAREN expression RPAREN whileBody;

// Вспомогательное правило для выражений присваивания внутри заголовка цикла (без точки с запятой)
assignmentExpr: ID ASSIGN expression;

returnStatement: DRATUTI expression? SEMI;

breakStatement: HVATIT SEMI;

continueStatement: PRODOLZHAEM SEMI;

// --- Ввод / Вывод ---

ioStatement:
      VYBROS LPAREN argumentList? RPAREN SEMI
    | VBROS LPAREN inputList RPAREN SEMI
    ;

inputList: ID (COMMA ID)*;

// --- Выражения ---

// Основное выражение теперь включает присваивания
expression: assignmentExpression;

// Правило для цепочек присваивания: a = b = c = 5
assignmentExpression:
    logicalOrExpression (ASSIGN assignmentExpression)?;

logicalOrExpression:
    logicalAndExpression (ILI logicalAndExpression)*;

logicalAndExpression:
    equalityExpression (I equalityExpression)*;

equalityExpression:
    relationalExpression ((EQ | NEQ) relationalExpression)*;

relationalExpression:
    additiveExpression ((LT | GT | LTE | GTE) additiveExpression)*;

additiveExpression:
    multiplicativeExpression ((PLUS | MINUS) multiplicativeExpression)*;

multiplicativeExpression:
    unaryExpression ((MULT | DIV | MOD) unaryExpression)*;

unaryExpression:
      (PLUS | MINUS | NE) unaryExpression  # unaryOp
    | primary                              # primaryAtom
    ;

primary:
      literal
    | constant
    | arrayAccess
    | ID callSuffix?
    | builtinFunctionCall
    | LPAREN expression RPAREN
    ;

// Доступ к массиву: arr[i]
arrayAccess: ID LBRACK expression RBRACK;

callSuffix: LPAREN argumentList? RPAREN;

functionCall: ID LPAREN argumentList? RPAREN;

builtinFunctionCall:
      (MODUL | SINUS | KOSINUS | TANGENS) LPAREN expression RPAREN
    | (MINIMUM | MAXIMUM) LPAREN argumentList RPAREN
    ;

argumentList: expression (COMMA expression)*;

// --- Литералы и константы ---

literal:
      INTEGER
    | REAL
    | STRING
    | logicalLiteral
    ;

logicalLiteral: HAIP | KRINZH;

constant: PI | ESHKA;