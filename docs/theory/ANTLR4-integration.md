# Интеграции ANTLR4 в проект на C#

### Добавление пакетов

1. Создание проекта:(``опционально``)
    ```bash
    dotnet new console -n projectName -f net8.0
    cd projectName
    ```
2. Добавление необходимых пакетов
    ```bash
    dotnet add package Antlr4.Runtime.Standard --version 4.13.0
    dotnet add package Antlr4BuildTasks --version 12.11.0
    ```

3. Проверить, что выполнилось успешнго:
    ```bash
    dotnet restore
    ```

    Ожидается что в папке проекта появтся:
    
    * obj/project.assets.json 
    * obj/AntlrSample.csproj.nuget.g.props
    * obj/AntlrSample.csproj.nuget.g.targets

    В консоли: `Восстановление завершено. Restore completed in X.XX sec for C:\path\to\AntlrSample\AntlrSample.csproj.`
    

4. Настроить проект(файл `.csproj`):
    ```xml
    <Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net8.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Antlr4.Runtime.Standard" Version="4.13.0" />
        <PackageReference Include="Antlr4BuildTasks" Version="12.11.0" PrivateAssets="all" />
    </ItemGroup>

    <ItemGroup>
        <Antlr4 Include="Expr.g4">
        <Visitor>True</Visitor>
        <Listener>True</Listener>
        </Antlr4>
    </ItemGroup>
    </Project>
    ```
    `Примечание:` В теге `<Antlr4>` укажи свой граматический файл, если он называется не `Expr.g4`

## Тестирование

1. Создать в папке проекта файл - `Expr.g4`
    ```antlr4
    grammar Expr;
    prog:   expr+ ;
    expr:   expr ('*'|'/') expr
        |   expr ('+'|'-') expr
        |   INT
        |   '(' expr ')'
        ;
    INT :   [0-9]+ ;
    WS  :   [ \t\r\n]+ -> skip ;
    ```
    `Примечание:` Файл должен быть сохранен в кодировке *UTF-8* без сегнатуры (*BOM*)

2. Выполнить сборку проекта
    ```bash
    dotnet build
    ```
    `Примечание 1:` Убедиться, что сборка прошла без ошибок. Если код не обновился, попробовать выполнить `dotnet clean`, затем `dotnet build`
    `Примечание 2:` Для работы ANTLR4 требуется также установить `Java`, которую можно поставить с официального сайта: `https://www.oracle.com/java/technologies/downloads/#jdk25-windows`

3. Использовать сгенерированный парсер:
    
    3.1 Program.cs

    ```C#
    using Antlr4.Runtime;
    using System.Linq;

    var input = "2 + 3 * (4 - 1)\n";
    var inputStream = new AntlrInputStream(input);
    var lexer = new ExprLexer(inputStream);
    var commonTokenStream = new CommonTokenStream(lexer);
    var parser = new ExprParser(commonTokenStream);

    // Запуск разбора, начиная с корневого правила 'prog'
    var tree = parser.prog();

    // Вывод дерева в виде LISP-подобной строки
    Console.WriteLine(tree.ToStringTree(parser));
    ```
            
4. Запуск
    ```bash
    dotnet run
    ```

5. Ожидаемый результат в консоли: `(prog (expr (expr 2) + (expr (expr 3) * (expr ( (expr (expr 4) - (expr 1)) )))) )`
        



## Справка:

1. Файл сохранен в кодировке UTF-8 без сигнатуры (BOM) означает, что файл не имеет специальных байтов (EF BB BF) в начале, которые обозначают эту кодировку. Это стандартный формат для совместимости с различными системами. Для сохранения или проверки этого формата можно использовать текстовые редакторы, такие как Notepad++, где в настройках кодировки есть опция "UTF-8" (именно без BOM, а не "UTF-8 с BOM")
2. Тег `<Visitor>` - При True генерирует интерфейс и базовый класс посетителя
3. Тег `<Listener>` - При True генерирует интерфейс и базовый класс слушателя (включен по умолчанию)
4. Тег `<Abstract>` - При True сгенерированные классы лексера и парсера будут `abstract`