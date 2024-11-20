using System.Security;
using System.Text;

namespace Project2_2
{
    public class DownloadData
    {
        // Приватное поле для хранения пути к файлу
        private readonly string _filePath;
        
        // Приватное поле для хранения данных CSV
        private string[][]? _csvData;
        
        // Публичный конструктор для инициализации пути к файлу
        public DownloadData(string filePath)
        {
            _filePath = filePath;
        }
        
        // Публичный метод для чтения данных из CSV файла
        public void ReadCsvData()
        {
            Encoding encoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            
            try
            {
                // Проверка типа файла
                if (!Path.GetExtension(_filePath).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    // _csvData = null;
                    throw new Exception("Тип файла некорректен. Введите путь к CSV файлу.");
                }
                
                string[] lines = File.ReadAllLines(_filePath, encoding);
                
                _csvData = new string[lines.Length][];

                for (int i = 0; i < lines.Length; i++)
                {
                    _csvData[i] = i == 0 
                        ? lines[i].Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries) 
                        : lines[i].Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)[1..];

                }

                if (!(_csvData != null && _csvData.Length != 0 && ValidateCsvStructure()))
                {
                    // _csvData = null;
                    throw new Exception("CSV файл не соотвествует структуре.");
                }
                
                UploadFileFromUser();
                
                Console.WriteLine("Файл успешно открыт, проверен и загружен.");
            }
            
            // Исключения для метода File.ReadAllLines
            catch (ArgumentException)
            {
                Console.WriteLine(
                    "Path строка нулевой длины, содержит только пробелы или содержит один или несколько недопустимых символов.");
                _csvData = null;
            }
            catch (PathTooLongException)
            {
                Console.WriteLine(
                    "Указанный путь, имя файла или оба значения превышают максимальную длину, заданную в системе.");
                _csvData = null;
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Указан недопустимый путь.");
                _csvData = null;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл, заданный параметром path, не найден.");
                _csvData = null;
            }
            /*
            catch (IOException)
            {
                Console.WriteLine("При открытии файла произошла ошибка ввода-вывода.");
                _csvData = null;
            }
            */
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Параметр path указывает файл, доступный только для чтения.");
                _csvData = null;
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("Параметр path задан в недопустимом формате.");
                _csvData = null;
            }
            catch (SecurityException)
            {
                Console.WriteLine("У вызывающего объекта отсутствует необходимое разрешение.");
                _csvData = null;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{exception.Message}");
            }
        }

        private bool ValidateCsvStructure()
        {
            string[] columns = { "Country", "Port Name", "UN Code", "Vessels in Port", 
                "Departures(Last 24 Hours)", "Arrivals(Last 24 Hours)", "Expected Arrivals", 
                "Type", "Area Local", "Area Global", "Also known as" };
            
            if (columns.Length != _csvData[0].Length)
            {
                throw new Exception("CSV файл не соотвествует структуре.");
            }

            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i] != _csvData[0][i])
                {
                    return false;
                }
            }

            return true;
        }

        private void UploadFileFromUser(bool fileFromUserExist)
        {
            // Определяем директорию назначения "fileFromUser" в папке проекта
            string destinationDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fileFromUser");

            // Создаем директорию, если её нет
            Directory.CreateDirectory(destinationDirectory);

            // Формируем полный путь к файлу в директории "fileFromUser"
            string destinationFilePath = Path.Combine(destinationDirectory, Path.GetFileName(_filePath));

            if (!fileFromUserExist)
            {
                // Копируем файл в директорию
                File.Copy(_filePath, destinationFilePath, overwrite: true);
                Console.WriteLine($"Файл успешно загружен в: {destinationFilePath}");
            }
            else
            {
                // Получаем список файлов в директории
                string[] files = Directory.GetFiles(destinationDirectory);
                
                // Удаляем предыдущий файл
                File.Delete(files[0]);
                
                // Копируем новый файл в директорию
                File.Copy(_filePath, destinationFilePath, overwrite: true);
                
                Console.WriteLine($"Файл успешно обновлен. Новый файл: {destinationFilePath}");
            }
            
        }
        
        // Публичный метод для получения данных CSV
        public string[][] GetCsvData()
        {
            return _csvData ?? Array.Empty<string[]>();
        }
    }
}


