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
            IJSONObject jsonObject = new GenericJsonObject();

            while (true)
            {
                string commandFunction = app.StringChecker(
                    "Введите номер команды" +
                    "\n1: Загрузить JSON из файла: " +
                    "\n2: Ввести данные JSON файла вручную:" +
                    "\n3: Вывести данные JSON объекта в консоль" +
                    "\n4: Сохранить данные JSON объекта в файл" +
                    "\n8: Выход"
                );

                if (commandFunction.Equals("8"))
                {
                    Console.WriteLine("Завершение работы программы...");
                    break;
                }
                
                if (commandFunction is "1" or "2")
                {
                    string? filePathToDownloadJson = "";
                    
                    if (commandFunction.Equals("1"))
                    {
                        // В моём понимании, относительный путь - путь от Solution Root.
                        // Следовательно, путь должен быть либо абсолютным, либо от solution root.
                        // Только тогда программа правильно построит путь к файлу. 
                        filePathToDownloadJson = app.GetFilePath();

                        if (filePathToDownloadJson == null) { continue; }
                    }

                    try
                    {
                        jsonObject = JsonParser.ReadJson(filePathToDownloadJson);
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                }

                if (commandFunction is "3" or "4")
                {
                    if (!jsonObject.GetAllFields().Any())
                    {
                        Console.WriteLine("Ошибка: JSON-объект пуст. Сначала загрузите данные");
                        continue;
                    }

                    if (commandFunction.Equals("3"))
                    {
                        JsonParser.PrintJson(jsonObject);
                    }

                    if (commandFunction.Equals("4"))
                    {
                        string? filePathToUploadJson = app.FilePathToUploadJson();
                        
                        if (filePathToUploadJson == null) { continue; }
                        
                        try
                        {
                            JsonParser.WriteJson(jsonObject, filePathToUploadJson);
                            
                            Console.WriteLine("Файл успешно загружен.");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
                
            }

        }
    }
}