using System;
using Terminal.Gui;

namespace TUIApp
{
    public class ProgramTui
    {
        public static void Main()
        {
            // Инициализируем Terminal.Gui
            Application.Init();
            Toplevel top = Application.Top;

            // Меню бар согласно новому заданию:
            MenuBar menu = new(
            [
                new MenuBarItem("_JSON файл:",
                [
                    new MenuBarItem("Импортировать",
                    [
                        new MenuItem("Через консоль", "", ImportDataManual),
                        new MenuItem("Через путь", "", ImportDataByPath)
                    ]),
                    new MenuBarItem("_Экспортировать",
                    [
                    new MenuItem("В консоль", "", ExportDataToConsole),
                    new MenuItem("Сохранить в файле", "", ExportDataToFile)
                    ])
                ]),
                new MenuBarItem("_Фильтр",
                [
                    new MenuItem("Фильтрация данных", "", FilterData)
                ]),
                new MenuBarItem("_Сортировка",
                [
                    new MenuItem("Сортировка данных", "", SortData)
                ]),
                new MenuBarItem("_Основная задача",
                [
                    new MenuItem("Основная задача", "", ExecuteMainTask)
                ]),
                new MenuBarItem("_О программе",
                [
                    new MenuItem("О программе", "", ShowAbout)
                ]),
                new MenuBarItem("_Выход", "", () => Application.RequestStop())
            ]);
            top.Add(menu);

            // Создаём основное окно приложения
            Window win = new("TUI-приложение")
            {
                X = 0,
                Y = 1, // Отступ для MenuBar
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            // Добавляем на окно стартовую информацию или инструкцию
            Label instructions = new("Добро пожаловать!\nВыберите нужное действие из меню.")
            {
                X = Pos.Center(),
                Y = Pos.Center()
            };
            win.Add(instructions);
            top.Add(win);

            Application.Run();
            Application.Shutdown();
        }

        // Заглушки для загрузки JSON файла

        // Импорт JSON через консоль
        public static void ImportDataManual()
        {
            _ = MessageBox.Query(50, 7, "Загрузить JSON файл", "Импорт JSON через консоль не реализован.", "OK");
        }

        // Импорт JSON через указание пути
        public static void ImportDataByPath()
        {
            _ = MessageBox.Query(50, 7, "Загрузить JSON файл", "Импорт JSON через путь не реализован.", "OK");
        }

        // Заглушки для экспорта JSON файла

        // Экспорт JSON — вывод в консоль
        public static void ExportDataToConsole()
        {
            _ = MessageBox.Query(50, 7, "Экспортировать JSON файл", "Экспорт JSON в консоль не реализован.", "OK");
        }

        // Экспорт JSON — сохранение в файл
        public static void ExportDataToFile()
        {
            _ = MessageBox.Query(50, 7, "Экспортировать JSON файл", "Экспорт JSON в файл не реализован.", "OK");
        }

        // Заглушка для фильтрации данных
        public static void FilterData()
        {
            _ = MessageBox.Query(50, 7, "Фильтр", "Функция фильтрации не реализована.", "OK");
        }

        // Заглушка для сортировки данных
        public static void SortData()
        {
            _ = MessageBox.Query(50, 7, "Сортировка", "Функция сортировки не реализована.", "OK");
        }

        // Заглушка для основной задачи
        public static void ExecuteMainTask()
        {
            _ = MessageBox.Query(50, 7, "Основная задача", "Основная задача не реализована.", "OK");
        }

        // Метод отображения информации "О программе"
        public static void ShowAbout()
        {
            _ = MessageBox.Query(50, 7, "О программе", "Автор: Тимур Бюрчиев\nИндивидуальный вариант: 9", "OK");
        }
    }
}
