using System.Text;
namespace Project_3rd_module
{
    public class ConsoleApp
    {
        public string StringChecker(string s = "")
        {
            while (true)
            {
                if (s != "") {Console.WriteLine(s);}
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Input is null or empty. Try again.");
                    continue;
                }
                return input;
            }

        }

        public string? GetFilePath()
        {
            string userPath = StringChecker("Введите путь к файлу JSON: ");

            // baseDir указывает на папку bin/Debug/net8.0/
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Чтобы добраться до корня решения, поднимаемся на 4 уровня вверх:
            //  1) net8.0
            //  2) Debug
            //  3) bin
            //  4) Project_3rd_module (если на этом уровне лежит .sln)
            string solutionRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));

            // Если пользователь ввёл абсолютный путь (C:\...), используем его напрямую
            // Иначе считаем, что это путь от корня решения
            string filePath = Path.IsPathRooted(userPath)
                ? userPath
                : Path.Combine(solutionRoot, userPath);

            Console.WriteLine($"Пытаемся открыть файл по пути: {filePath}");

            // Проверяем, существует ли файл
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Ошибка: файл не найден!");
                return null;
            }

            return filePath;
        }
    }
}