using ClassLibrary1;
using System;

using System.Text;
using System.IO;
namespace Project_3rd_module
{

    public class Program
    {
        public static void Main()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            ConsoleApp app = new();

            while (true)
            {
                string commandFunction = app.StringChecker(
                    "Введите номер команды" +
                    "\n1: Загрузить JSON из файла: " +
                    "\n8: Выход"
                );

                if (commandFunction.Equals("8"))
                {
                    Console.WriteLine("Завершение работы программы...");
                    break;
                }

                if (commandFunction.Equals("1"))
                {
                    // В моём понимании, относительный путь - путь от Solution Root.
                    // Следовательно, путь должен быть либо абсолютным, либо от solution root.
                    // Только тогда программа правильно построит путь к файлу. 
                    string? filePath = app.GetFilePath();

                    if (filePath == null) { continue; }

                    try
                    {
                        IJSONObject? jsonObject = JsonParser.ReadJson(filePath);
                        JsonParser.WriteJson(jsonObject);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }
                }
            }

        }
    }
}