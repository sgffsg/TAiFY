parser grammar VaibikParser;

options { tokenVocab=VaibikLexer; }

program: POEHALI statementList FINALOCHKA;

statementList: statement*;

block: POEHALI statementList FINALOCHKA;

blockOrStatement: statement | block;

statement:
    variableDeclaration
    | assignmentStatement
    | ifStatement
    | loopStatement
    | returnStatement
    | breakStatement
    | ioStatement
    | functionCallStatement
    | functionDeclaration
    | procedureDeclaration
    ;

variableDeclaration:
    (typeName ID ASSIGN expression SEMI)
    | (BAZA typeName ID ASSIGN expression SEMI)
    ;

assignmentStatement: ID ASSIGN expression SEMI;

ifStatement:
    ESLI LPAREN expression RPAREN TO blockOrStatement (INACHE blockOrStatement)?;

loopStatement:
    CIKL LPAREN
        (assignmentStatement SEMI expression SEMI assignmentStatement
        | expression)
    RPAREN block;

returnStatement: DRATUTI expression SEMI;

breakStatement: HVATIT SEMI;

ioStatement:
    (VYBROS LPAREN argumentList RPAREN SEMI)
    | (VBROS LPAREN ID (COMMA ID)* RPAREN SEMI)
    ;

functionCallStatement: functionCall SEMI;

functionDeclaration:
    typeName ID LPAREN parameterList? RPAREN block;

procedureDeclaration:
    PROKRASTINIRUEM ID LPAREN parameterList? RPAREN block;

parameterList:
    ID COLON typeName (COMMA ID COLON typeName)*;

typeName: CIFERKA | POLTORASHKA | CITATA | RASKLAD | PSHIK;

argumentList: expression (COMMA expression)*;

expression: logicalOrExpression;

logicalOrExpression:
    logicalAndExpression (ILI logicalAndExpression)*;

logicalAndExpression:
    comparisonExpression (I comparisonExpression)*;

comparisonExpression:
    additiveExpression ((EQ | NEQ | LT | GT | LTE | GTE) additiveExpression)?;

additiveExpression:
    multiplicativeExpression ((PLUS | MINUS) multiplicativeExpression)*;

multiplicativeExpression:
    unaryExpression ((MULT | DIV | MOD) unaryExpression)*;

unaryExpression:
    (PLUS | MINUS | NE)* primary;

primary:
    INTEGER
    | REAL
    | STRING
    | logicalLiteral
    | constant
    | functionCall
    | ID
    | LPAREN expression RPAREN
    ;

logicalLiteral: HAIP | KRINZH;

constant: PI | ESHKA;

functionCall: ID LPAREN argumentList? RPAREN;