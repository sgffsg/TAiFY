#!/usr/bin/env python3
"""
NOTE: Этот скрипт создан ИИ-моделью.

Скрипт для проверки структуры студенческого C# проекта.

Проверяет структуру каталогов, расположение файлов, наличие необходимых
файлов конфигурации и соответствие требованиям к содержимому.
"""

import argparse
import datetime
import logging
import pathlib
import re
import subprocess
import sys
from typing import Dict, List


def parse_arguments() -> argparse.Namespace:
    """Обработка аргументов командной строки.

    Returns:
        Разобранные аргументы командной строки
    """
    parser = argparse.ArgumentParser(description="Проверка структуры студенческого C# проекта")
    parser.add_argument(
        "path", nargs="?", default=".", help="Путь к корню проекта (по умолчанию: текущий каталог)"
    )
    parser.add_argument("--verbose", action="store_true", help="Вывод отладочных сообщений")
    return parser.parse_args()


def setup_logging(verbose: bool) -> None:
    """Настройка логирования.

    Args:
        verbose: Флаг вывода отладочных сообщений
    """
    level = logging.DEBUG if verbose else logging.INFO
    logging.basicConfig(level=level, format="%(levelname)s: %(message)s", datefmt="%H:%M:%S")


def list_git_files(project_path: pathlib.Path) -> List[str]:
    """Получение списка файлов под контролем версий Git.

    Args:
        project_path: Путь к корню проекта

    Returns:
        Список файлов под контролем версий

    Raises:
        RuntimeError: Если не удалось выполнить команду git
    """
    try:
        result = subprocess.run(
            ["git", "ls-files"], cwd=project_path, capture_output=True, text=True, check=True
        )
        return result.stdout.splitlines()
    except subprocess.CalledProcessError as e:
        raise RuntimeError(f"Failed to execute git ls-files: {e.stderr}") from e
    except FileNotFoundError:
        raise RuntimeError("Git is not installed or not in PATH") from None


class ErrorReporter:
    """Центральный отчётчик ошибок: логирует и считает ошибки."""

    def __init__(self) -> None:
        self.error_count = 0

    def error(self, message: str) -> None:
        """Зарегистрировать ошибку."""
        logging.error(message)
        self.error_count += 1

    def report_summary(self) -> int:
        """Вывести итоговое сообщение и вернуть число ошибок."""
        if self.error_count > 0:
            logging.error(f"Проверка проекта не пройдена. Найдено ошибок: {self.error_count}")
        else:
            logging.info("Проверка проекта пройдена успешно. Ошибок не найдено.")
        return self.error_count


import pathlib
import xml.etree.ElementTree as ET


class BuildPropsChecker:
    """Класс для проверки файла Directory.Build.props"""

    FILE_NAME = "Directory.Build.props"

    def check(self, project_path: pathlib.Path, error_reporter: ErrorReporter) -> None:
        file_path = project_path / self.FILE_NAME
        if not file_path.exists():
            error_reporter.error(f"Отсутствует файл {self.FILE_NAME} в корне проекта")
            return

        try:
            root = self._parse_xml(file_path)
            self._check_property_group(root, error_reporter)
            self._check_package_references(root, error_reporter)
        except Exception as e:
            error_reporter.error(f"Ошибка при проверке файла {self.FILE_NAME}: {str(e)}")

    def _parse_xml(self, file_path: pathlib.Path) -> ET.Element:
        try:
            tree = ET.parse(file_path)
            return tree.getroot()
        except ET.ParseError as e:
            raise Exception(f"XML parsing error: {str(e)}") from e

    def _check_property_group(self, root: ET.Element, error_reporter: ErrorReporter) -> None:
        required_properties = [
            "NuGetAudit",
            "WarningsAsErrors",
            "_SkipUpgradeNetAnalyzersNuGetWarning",
        ]

        property_groups = root.findall("PropertyGroup")
        if not property_groups:
            error_reporter.error(f"Отсутствует узел PropertyGroup в {self.FILE_NAME}")
            return

        for prop_group in property_groups:
            for prop in required_properties:
                if prop_group.find(prop) is None:
                    error_reporter.error(
                        f"Отсутствует свойство {prop} в PropertyGroup файла {self.FILE_NAME}"
                    )

    def _check_package_references(self, root: ET.Element, error_reporter: ErrorReporter) -> None:
        required_packages = [
            "Microsoft.CodeAnalysis.NetAnalyzers",
            "Roslynator.Analyzers",
            "StyleCop.Analyzers",
            "xunit.analyzers",
        ]

        item_groups = root.findall("ItemGroup")
        found_packages = []

        for item_group in item_groups:
            package_refs = item_group.findall("PackageReference")
            for package_ref in package_refs:
                include = package_ref.get("Include")
                if include:
                    found_packages.append(include)

        for package in required_packages:
            if package not in found_packages:
                error_reporter.error(
                    f"Отсутствует пакет {package} в PackageReference файла {self.FILE_NAME}"
                )


class DocsDirectoryChecker:
    """Класс для проверки структуры каталога docs/"""

    def check(self, git_files: List[str], error_reporter: ErrorReporter) -> None:
        self._check_allowed_subdirectories(git_files, error_reporter)
        self._check_markdown_only_dirs(git_files, error_reporter)

    def _check_allowed_subdirectories(
        self, git_files: List[str], error_reporter: ErrorReporter
    ) -> None:
        allowed_subdirs = {"specification", "theory", "competitors", "examples"}

        docs_dirs = set()
        for file_path in git_files:
            if file_path.startswith("docs/"):
                parts = file_path.split("/")
                if len(parts) > 2:
                    docs_dirs.add(parts[1])

        for subdir in docs_dirs:
            if subdir not in allowed_subdirs:
                error_reporter.error(f"Недопустимый подкаталог docs/{subdir}")

    def _check_markdown_only_dirs(
        self, git_files: List[str], error_reporter: ErrorReporter
    ) -> None:
        markdown_only_dirs = ["docs/specification"]
        excluded_dirs = ["docs/specification/examples"]

        for file_path in git_files:
            for dir_path in markdown_only_dirs:
                if file_path.startswith(dir_path + "/") and not any(
                    file_path.startswith(e + "/") for e in excluded_dirs
                ):
                    if not file_path.endswith(".md"):
                        error_reporter.error(
                            f"Недопустимый файл {file_path} в каталоге {dir_path}, разрешены только .md файлы"
                        )


class CsProjectChecker:
    """Класс для проверки sln и csproj файлов"""

    def check(
        self, project_path: pathlib.Path, git_files: List[str], error_reporter: ErrorReporter
    ) -> None:
        self._check_csproj_exists(project_path, error_reporter)
        self._check_solution_file(project_path, error_reporter)
        self._check_project_locations(git_files, error_reporter)
        self._check_cs_files_location(git_files, error_reporter)

    def _check_csproj_exists(
        self, project_path: pathlib.Path, error_reporter: ErrorReporter
    ) -> None:
        csproj_files = list(project_path.rglob("*.csproj"))
        if not csproj_files:
            error_reporter.error("В проекте не найдено ни одного файла .csproj")

    def _check_solution_file(
        self, project_path: pathlib.Path, error_reporter: ErrorReporter
    ) -> None:
        sln_files = list(project_path.glob("*.sln"))

        if not sln_files:
            error_reporter.error("В корне проекта отсутствует файл решения (.sln)")
        elif len(sln_files) > 1:
            error_reporter.error("В корне проекта найдено несколько файлов решения (.sln)")

    def _check_project_locations(self, git_files: List[str], error_reporter: ErrorReporter) -> None:
        for file_path in git_files:
            if file_path.endswith(".csproj"):
                if not (file_path.startswith("src/") or file_path.startswith("tests/")):
                    error_reporter.error(
                        f"Файл проекта {file_path} должен находиться в каталогах src/ или tests/"
                    )

    def _check_cs_files_location(self, git_files: List[str], error_reporter: ErrorReporter) -> None:
        for file_path in git_files:
            if file_path.endswith(".cs"):
                if not (
                    file_path.startswith("src/")
                    or file_path.startswith("tests/")
                    or file_path.startswith("docs/examples/")
                    or file_path.startswith("docs/competitors/")
                ):
                    error_reporter.error(
                        f"C# файл {file_path} должен находиться в каталогах src/, tests/, docs/examples/ или docs/competitors/"
                    )


class EditorConfigChecker:
    """Класс для проверки файла .editorconfig"""

    def check(self, project_path: pathlib.Path, error_reporter: ErrorReporter) -> None:
        if not self._check_file_exists(project_path):
            error_reporter.error("Отсутствует файл .editorconfig в корне проекта")
            return

        try:
            file_path = project_path / ".editorconfig"
            content = self._parse_file(file_path)
            self._check_root_setting(content, error_reporter)
            self._check_cs_indentation(content, error_reporter)
            self._check_naming_rules(content, error_reporter)
        except Exception as e:
            error_reporter.error(f"Ошибка при проверке файла .editorconfig: {str(e)}")

    def _check_file_exists(self, project_path: pathlib.Path) -> bool:
        return (project_path / ".editorconfig").exists()

    def _parse_file(self, file_path: pathlib.Path) -> Dict[str, Dict[str, str]]:
        content = {}
        current_section = None

        with open(file_path, "r", encoding="utf-8") as f:
            for line in f:
                line = line.strip()

                if not line or line.startswith("#"):
                    continue

                if line.startswith("[") and line.endswith("]"):
                    current_section = line[1:-1]
                    if current_section not in content:
                        content[current_section] = {}
                elif "=" in line:
                    key, value = line.split("=", 1)
                    key = key.strip()
                    value = value.strip()

                    if current_section is not None:
                        content[current_section][key] = value
                    else:
                        if "root" not in content:
                            content["root"] = {}
                        content["root"][key] = value

        return content

    def _check_root_setting(
        self, content: Dict[str, Dict[str, str]], error_reporter: ErrorReporter
    ) -> None:
        if "root" not in content or content["root"].get("root") != "true":
            error_reporter.error("Отсутствует настройка root = true в файле .editorconfig")

    def _check_cs_indentation(
        self, content: Dict[str, Dict[str, str]], error_reporter: ErrorReporter
    ) -> None:
        cs_settings = {}
        for section in content:
            if "*.cs" in section:
                cs_settings.update(content[section])

        if not cs_settings:
            error_reporter.error("Отсутствует секция для *.cs файлов в .editorconfig")
            return

        if cs_settings.get("indent_size") != "4":
            error_reporter.error("Неверная настройка indent_size для *.cs файлов, должно быть 4")
        if cs_settings.get("tab_width") != "4":
            error_reporter.error("Неверная настройка tab_width для *.cs файлов, должно быть 4")

    def _check_naming_rules(
        self, content: Dict[str, Dict[str, str]], error_reporter: ErrorReporter
    ) -> None:
        all_settings = {}
        for section in content.values():
            all_settings.update(section)

        def check_prefix_exists(prefix: str) -> None:
            if not any(key.startswith(prefix) for key in all_settings):
                error_reporter.error(f"Нет ни одного правила {prefix}* в .editorconfig")

        for prefix in [
            "dotnet_naming_rule.",
            "dotnet_naming_symbols.",
            "dotnet_naming_style.",
            "dotnet_diagnostic.",
        ]:
            check_prefix_exists(prefix)


class IgnoreChecker:
    """Класс для проверки отсутствия файлов под контролем версий"""

    def check(self, git_files: List[str], error_reporter: ErrorReporter) -> None:
        self._check_build_artifacts(git_files, error_reporter)
        self._check_ide_files(git_files, error_reporter)

    def _check_build_artifacts(self, git_files: List[str], error_reporter: ErrorReporter) -> None:
        build_extensions = [".dll", ".exe", ".pdb", ".cache"]

        for file_path in git_files:
            for ext in build_extensions:
                if file_path.endswith(ext):
                    error_reporter.error(
                        f"Файл продукта сборки {file_path} не должен быть под контролем версий"
                    )

    def _check_ide_files(self, git_files: List[str], error_reporter: ErrorReporter) -> None:
        ide_patterns = [".vscode/", ".vs/", ".idea/", ".suo", ".user", ".DotSettings.user"]

        for file_path in git_files:
            for pattern in ide_patterns:
                if pattern in file_path:
                    error_reporter.error(
                        f"Файл настроек IDE {file_path} не должен быть под контролем версий"
                    )


class ReadmeChecker:
    """Класс для проверки файла README.md"""

    def check(self, git_files: List[str], error_reporter: ErrorReporter) -> None:
        """Проверить наличие и содержимое файла README.md.

        Args:
            git_files: Список файлов под контролем версий
            error_reporter: Отчётчик ошибок
        """
        if "README.md" not in git_files:
            error_reporter.error("Отсутствует файл README.md в проекте (должен быть под контролем версий)")
            return

        try:
            file_path = pathlib.Path("README.md")
            content = file_path.read_text(encoding="utf-8").strip()
            if not content:
                error_reporter.error("Файл README.md не содержит описания проекта")
        except FileNotFoundError:
            error_reporter.error("Файл README.md отсутствует в рабочем каталоге (хотя есть в git)")
        except Exception as e:
            error_reporter.error(f"Ошибка при чтении файла README.md: {str(e)}")


class LicenseChecker:
    """Класс для проверки файла LICENSE"""

    # Регулярные выражения для распознавания названий OpenSource лицензий
    LICENSE_PATTERNS = [
        (r"mit license", "MIT License"),
        (r"the mit license", "MIT License"),
        (r"apache license, version 2\.0", "Apache License, Version 2.0"),
        (r"apache license 2\.0", "Apache License 2.0"),
        (r"gnu (general|lesser) public license v?[0-9](\.[0-9])?", "GNU General/Lesser Public License"),
        (r"gnu (affero )?general public license v?[0-9](\.[0-9])?", "GNU Affero General Public License"),
        (r"bsd [23]-clause license", "BSD 2-Clause/3-Clause License"),
        (r"bsd license", "BSD License"),
        (r"mozilla public license 2\.0", "Mozilla Public License 2.0"),
        (r"eclipse public license 2\.0", "Eclipse Public License 2.0"),
        (r"creative commons attribution 4\.0 international", "Creative Commons Attribution 4.0 International"),
        (r"creative commons zero v?1\.0", "Creative Commons Zero v1.0"),
        (r"unlicense", "The Unlicense"),
        (r"isc license", "ISC License"),
    ]

    def check(self, git_files: List[str], error_reporter: ErrorReporter) -> None:
        """Проверить наличие и корректность файла LICENSE.

        Args:
            git_files: Список файлов под контролем версий
            error_reporter: Отчётчик ошибок
        """
        if "LICENSE" not in git_files:
            error_reporter.error("Отсутствует файл LICENSE в проекте (должен быть под контролем версий)")
            return

        try:
            file_path = pathlib.Path("LICENSE")
            lines = file_path.read_text(encoding="utf-8").splitlines()
        except Exception as e:
            error_reporter.error(f"Ошибка при чтении файла LICENSE: {str(e)}")
            return

        # Проверка первой непустой строки на название лицензии
        first_non_empty = None
        for line in lines:
            stripped = line.strip()
            if stripped:
                first_non_empty = stripped
                break

        if first_non_empty is None:
            error_reporter.error("Файл LICENSE пуст")
            return

        if not self._matches_license_pattern(first_non_empty):
            error_reporter.error(
                "Файл LICENSE не содержит узнаваемого названия OpenSource лицензии в первой непустой строке"
            )

        # Проверка наличия строки Copyright (c) ...
        copyright_found = False
        copyright_line = None
        for line in lines:
            lower = line.lower()
            if "copyright" in lower and ("(c)" in lower or "(c)" in lower.replace("(c)", "(c)") or "©" in lower):
                copyright_line = line
                copyright_found = True
                break

        if not copyright_found:
            error_reporter.error("Файл LICENSE не содержит строки с Copyright (c) ...")
            return

        # Проверка года в строке Copyright (безусловная, с разбором через регулярное выражение, захватывающее всю строку)
        # Паттерны для строки копирайта с захватом года
        copyright_patterns = [
            r"(?i)copyright\s*\([cC]\)\s*(\d{4})",
            r"(?i)copyright\s*©\s*(\d{4})",
            r"(?i)copyright\s*(\d{4})\s*\([cC]\)",
            r"(?i)copyright\s*(\d{4})\s*©",
            r"(?i)\([cC]\)\s*(\d{4})",
            r"(?i)©\s*(\d{4})",
            r"(?i)copyright\s*(\d{4})",
            r"(?i)(\d{4})\s*\([cC]\)",
        ]
        year = None
        for pattern in copyright_patterns:
            match = re.search(pattern, copyright_line)
            if match:
                year = int(match.group(1))
                break
        if year is None:
            error_reporter.error(f"В строке Copyright отсутствует четырёхзначный номер года: {copyright_line}")
        else:
            current_year = datetime.datetime.now().year
            if year < 2025 or year > current_year:
                error_reporter.error(
                    f"Год в строке Copyright должен быть числом на отрезке [2025; {current_year}], но указан год {year}"
                )

    def _matches_license_pattern(self, text: str) -> bool:
        """Проверить, соответствует ли текст одному из шаблонов лицензий."""
        text_lower = text.lower()
        for pattern, _ in self.LICENSE_PATTERNS:
            if re.search(pattern, text_lower, re.IGNORECASE):
                return True
        return False


def check_project_structure(project_path: pathlib.Path) -> int:
    """Основная функция проверки структуры проекта"""
    try:
        git_files = list_git_files(project_path)
        logging.debug(f"Найдено файлов под контролем версий: {len(git_files)}")
    except Exception as e:
        logging.error(f"Не удалось получить список файлов под контролем версий: {str(e)}")
        return 1

    error_reporter = ErrorReporter()

    # Создаем экземпляры классов проверки
    csproj_checker = CsProjectChecker()
    build_props_checker = BuildPropsChecker()
    editorconfig_checker = EditorConfigChecker()
    docs_checker = DocsDirectoryChecker()
    ignore_checker = IgnoreChecker()
    readme_checker = ReadmeChecker()
    license_checker = LicenseChecker()

    # Выполняем проверки в указанном порядке
    csproj_checker.check(project_path, git_files, error_reporter)
    build_props_checker.check(project_path, error_reporter)
    editorconfig_checker.check(project_path, error_reporter)
    docs_checker.check(git_files, error_reporter)
    ignore_checker.check(git_files, error_reporter)
    readme_checker.check(git_files, error_reporter)
    license_checker.check(git_files, error_reporter)

    return error_reporter.report_summary()


def main() -> None:
    """Основная функция скрипта"""
    try:
        args = parse_arguments()
        setup_logging(args.verbose)

        project_path = pathlib.Path(args.path).resolve()
        if not project_path.exists():
            raise Exception(f"Path does not exist: {project_path}")

        errors = check_project_structure(project_path)
        sys.exit(0 if errors == 0 else 1)

    except Exception as e:
        raise e


if __name__ == "__main__":
    main()
