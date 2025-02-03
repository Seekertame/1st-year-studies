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
                    "\n5: Фильтрация JSON объекта" +
                    "\n6: Сортировка JSON объекта" +
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

                if (commandFunction is "3" or "4" or "5" or "6")
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

                    if (commandFunction is "5" or "6")
                    {
                        try
                        {
                            // Выводим верхнеуровневые поля JSON (обычно это будет только "verbs")
                            Console.WriteLine("Доступные поля верхнего уровня:");
                            foreach (string field in jsonObject.GetAllFields())
                            {
                                Console.WriteLine($"- {field}");
                            }
                            
                            // Определяем ключ коллекции
                            string collectionKey = "verbs";
                            
                            // Если jsonObject является GenericJsonObject, пытаемся извлечь коллекцию
                            if (jsonObject is GenericJsonObject gjo)
                            {
                                List<object>? collection = gjo.GetCollection(collectionKey);
                                
                                if (collection == null || collection.Count == 0)
                                {
                                    Console.WriteLine($"Коллекция '{collectionKey}' пуста или имеет некорректную структуру.");
                                    continue;
                                }

                                // Собираем уникальные ключи всех записей коллекции
                                HashSet<string> uniqueKeys = new(StringComparer.OrdinalIgnoreCase);
                                foreach (object item in collection)
                                {
                                    if (item is Dictionary<string, object> dict)
                                    {
                                        foreach (string key in dict.Keys)
                                        {
                                            uniqueKeys.Add(key);
                                        }
                                    }
                                }

                                // Выводим пользователю уникальные ключи (без повторений, регистр не важен)
                                Console.WriteLine($"Доступные поля для фильтрации внутри '{collectionKey}':");
                                foreach (string key in uniqueKeys)
                                {
                                    Console.WriteLine($"- {key}");
                                }

                                if (commandFunction.Equals("5"))
                                {
                                    // Запрашиваем у пользователя выбор поля для фильтрации
                                    string filterField =
                                        app.StringChecker(
                                            "Введите название одного поля для фильтрации (например: category): ");


                                    // Запрашиваем список разрешённых значений (через запятую)
                                    string allowedValuesInput =
                                        app.StringChecker("Введите список значений для фильтрации (через запятую): ");


                                    // Преобразуем введённую строку в список значений, удаляя лишние пробелы
                                    List<string> allowedValues = allowedValuesInput
                                        .Split(',')
                                        .Select(s => s.Trim())
                                        .Where(s => !string.IsNullOrEmpty(s))
                                        .ToList();

                                    // Выполняем фильтрацию коллекции
                                    gjo.FilterCollection(collectionKey, filterField, allowedValues);
                                    Console.WriteLine("Данные успешно отфильтрованы. Отформатированный JSON:");
                                    Console.WriteLine(gjo.PrettyPrint());
                                }

                                if (commandFunction.Equals("6"))
                                {
                                    // Запрашиваем у пользователя название поля для сортировки
                                    string sortField = app.StringChecker("Введите название поля для сортировки (например: id или category): ");

                                    // Запрашиваем направление сортировки: asc или desc
                                    string direction = app.StringChecker("Введите направление сортировки (asc для возрастания, desc для убывания): ");
                                    bool ascending = true;
                                    
                                    if (direction.Trim().ToLower() == "desc")
                                    {
                                        ascending = false;
                                    }
                                    else if (direction.Trim().ToLower() != "asc")
                                    {
                                        Console.WriteLine("Направление сортировки не указано. По умолчанию: asc.");
                                    }

                                    // Выполняем сортировку коллекции
                                    gjo.SortCollection(collectionKey, sortField, ascending);
                                    Console.WriteLine("Данные успешно отсортированы. Отформатированный JSON:");
                                    Console.WriteLine(gjo.PrettyPrint());
                                    
                                }
                            }
                            else
                            {
                                Console.WriteLine("Невозможно выполнить операцию: объект не является GenericJsonObject.");
                            }
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