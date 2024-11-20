using System.Text;

namespace Project2_2
{

    public class Program
    {
        public static void Main()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            int userChoice;
            do
            {
                Console.WriteLine("Меню. Выберите номер : " +
                                  "\n1. Загрузить данные из файла." +
                                  "\n5. Выйти из программы.");

                string? answer = Console.ReadLine();

                if (!int.TryParse(answer, out userChoice))
                {
                    Console.WriteLine("Ошибка: Введите, пожалуйста, номер функции (цифру).");
                }

                switch (userChoice)
                {
                    case 1:
                        Console.WriteLine("Введите путь к CSV файлу, который вы хотите загрузить:");
                        
                        string? filePath = Console.ReadLine();
                        
                        // Создаем объект класса DownloadData и загружаем данные
                        DownloadData downloadData = new DownloadData(filePath); // ############################ Не рассмотрен null value.
                        
                        downloadData.ReadCsvData();

                        string[][] csvData = downloadData.GetCsvData();
                        
                        if (csvData != null)
                        {
                            // Выводим данные на консоль
                            foreach (string[] row in csvData)
                            {
                                foreach (string value in row)
                                {
                                    Console.Write($"!{value}! ");
                                }
                                Console.WriteLine();
                            }
                        }
                        break;

                    case 5:
                        Console.WriteLine("Выход из программы...");
                        break;

                    default:
                        Console.WriteLine("Неверный номер функции. Попробуйте еще раз.");
                        break;
                }
            } while (userChoice != 5);
        }
    }
}
/*
using System;
using System.IO;


class Program
{
    static void Main(string[] args)
    {
        // Указываем путь к вашему CSV файлу
        string filePath = @"C:\path\to\your\file.csv";

        // Создаем экземпляр класса DownloadData
        DownloadData downloadData = new DownloadData(filePath);

        // Читаем данные из CSV файла
        downloadData.ReadCsvData();

        // Получаем данные для дальнейшего использования
        string[][] csvData = downloadData.GetCsvData();

        if (csvData != null)
        {
            // Выводим данные на консоль
            foreach (string[] row in csvData)
            {
                foreach (string value in row)
                {
                    Console.Write(value + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
*/