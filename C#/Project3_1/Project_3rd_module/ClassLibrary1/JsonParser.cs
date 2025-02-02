using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace ClassLibrary1
{
    /// <summary>
    /// Статический класс для обработки JSON‑данных.
    /// Содержит методы для чтения и записи JSON с использованием стандартных потоков Console.
    /// Реализация не использует специализированные библиотеки, рефлексию или атрибуты.
    /// </summary>
    public static class JsonParser
    {
        // Внутренние поля для разбора JSON-текста.
        private static string? _json;
        private static int _index;

        /// <summary>
        /// Считывает данные из файла по указанному пути, разбирает их и возвращает объект, реализующий IJSONObject.
        /// Если корневой элемент не является объектом или после разбора остались лишние символы,
        /// генерируется исключение.
        /// </summary>
        /// <param name="filePath">Путь к JSON-файлу (не должен быть null или пустым)</param>
        public static IJSONObject ReadJson(string filePath)
        {
            try
            {
                // Чтение файла через StreamReader; стандартный ввод перенаправляется на файл
                using StreamReader reader = new(filePath, Encoding.UTF8);
                Console.SetIn(reader);
                _json = Console.In.ReadToEnd();
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("AccessException: Нет доступа к файлу. " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при чтении файла: " + ex.Message);
            }

            _index = 0;
            SkipWhitespace();
            object result = ParseValue();
            SkipWhitespace();

            if (_index != _json.Length)
            {
                throw new Exception("Обнаружены лишние символы после корректного JSON.");
            }

            if (result is Dictionary<string, object> dict)
            {
                // Создаем новый JSON-объект через пустой конструктор
                // и заполняем его поля, вызывая SetField для каждого ключа
                GenericJsonObject jsonObj = new();
                foreach (KeyValuePair<string, object> kvp in dict)
                {
                    if (kvp.Value == null)
                    {
                        // Если значение поля равно null, заполняем его значением null
                        // (возвращается null, так как JSON содержит значение null)
                        jsonObj.SetField(kvp.Key, null);
                    }
                    else
                    {
                        // Для ненулевых значений устанавливаем строковое представление
                        jsonObj.SetField(kvp.Key, kvp.Value.ToString());
                    }
                }
                return jsonObj;
            }

            throw new Exception("Корневой элемент JSON должен быть объектом.");
        }

        /// <summary>
        /// Сериализует переданный объект IJSONObject в строку в JSON‑формате и выводит её в Console.Out.
        /// Для сериализации используется реализация метода ToString() в GenericJsonObject.
        /// </summary>
        public static void WriteJson(IJSONObject obj)
        {
            Console.Out.WriteLine(obj.ToString());
        }

        #region Методы разбора JSON

        /// <summary>
        /// Пропускает пробельные символы в _json.
        /// </summary>
        private static void SkipWhitespace()
        {
            if (_json == null) { throw new Exception("JSON string is null"); }
            
            while (_index < _json.Length && char.IsWhiteSpace(_json[_index]))
            {
                _index++;
            }
        }

        /// <summary>
        /// Разбирает значение JSON: объект, массив, строку, число, true, false или null.
        /// </summary>
        private static object ParseValue()
        {
            SkipWhitespace();

            if (_json == null) { throw new Exception("JSON string is null"); }
            
            if (_index >= _json.Length)
            {
                throw new Exception("Неожиданный конец входных данных.");
            }
            char c = _json[_index];
            if (c == '{')
            {
                return ParseObject();
            }
            if (c == '[')
            {
                return ParseArray();
            }
            if (c == '"')
            {
                return ParseString();
            }
            if (c == '-' || char.IsDigit(c))
            {
                return ParseNumber();
            }
            if (_json[_index..].StartsWith("true"))
            {
                _index += 4;
                return true;
            }
            if (_json[_index..].StartsWith("false"))
            {
                _index += 5;
                return false;
            }
            if (_json[_index..].StartsWith("null"))
            {
                _index += 4;
                // Возвращается null, так как JSON содержит литерал null
                return null!;
            }
            throw new Exception($"Неожиданный символ '{c}' на позиции {_index}.");
        }

        /// <summary>
        /// Разбирает JSON-объект и возвращает его представление в виде словаря.
        /// </summary>
        private static Dictionary<string, object> ParseObject()
        {
            if (_json == null) { throw new Exception("JSON string is null"); }
            
            Dictionary<string, object> dict = [];
            _index++; // пропускаем '{'
            SkipWhitespace();

            if (_index < _json.Length && _json[_index] == '}')
            {
                _index++; // пустой объект, возвращаем пустой словарь
                return dict;
            }

            while (true)
            {
                SkipWhitespace();
                if (_index >= _json.Length || _json[_index] != '"')
                {
                    throw new Exception("Ожидался ключ в кавычках.");
                }
                string key = ParseString();
                SkipWhitespace();
                if (_index >= _json.Length || _json[_index] != ':')
                {
                    throw new Exception("Ожидался символ ':' после ключа.");
                }
                _index++; // пропускаем ':'
                SkipWhitespace();
                object value = ParseValue();
                dict[key] = value;
                SkipWhitespace();
                if (_index < _json.Length && _json[_index] == '}')
                {
                    _index++;
                    break;
                }
                if (_index < _json.Length && _json[_index] == ',')
                {
                    _index++;
                    continue;
                }
                throw new Exception("Ожидался символ ',' или '}' в объекте.");
            }
            return dict;
        }

        /// <summary>
        /// Разбирает JSON-массив и возвращает список его элементов.
        /// </summary>
        private static List<object> ParseArray()
        {
            if (_json == null) { throw new Exception("JSON string is null"); }
            
            List<object> list = [];
            _index++; // пропускаем '['
            SkipWhitespace();
            if (_index < _json.Length && _json[_index] == ']')
            {
                _index++; // пустой массив, возвращаем пустой список
                return list;
            }
            while (true)
            {
                SkipWhitespace();
                object value = ParseValue();
                list.Add(value);
                SkipWhitespace();
                if (_index < _json.Length && _json[_index] == ']')
                {
                    _index++;
                    break;
                }
                if (_index < _json.Length && _json[_index] == ',')
                {
                    _index++;
                    continue;
                }
                throw new Exception("Ожидался символ ',' или ']' в массиве.");
            }
            return list;
        }

        /// <summary>
        /// Разбирает строку в формате JSON.
        /// </summary>
        private static string ParseString()
        {
            if (_json == null) { throw new Exception("JSON string is null"); }

            StringBuilder sb = new();
            _index++; // пропускаем открывающую кавычку
            while (_index < _json.Length)
            {
                char c = _json[_index++];
                if (c == '"')
                {
                    return sb.ToString();
                }
                if (c == '\\' && _index < _json.Length)
                {
                    char next = _json[_index++];
                    _ = next switch
                    {
                        '"' => sb.Append('"'),
                        '\\' => sb.Append('\\'),
                        '/' => sb.Append('/'),
                        'b' => sb.Append('\b'),
                        'f' => sb.Append('\f'),
                        'n' => sb.Append('\n'),
                        'r' => sb.Append('\r'),
                        't' => sb.Append('\t'),
                        _ => sb.Append(next)
                    };
                    continue;
                }
                _ = sb.Append(c);
            }
            throw new Exception("Не закрыта строка.");
        }

        /// <summary>
        /// Разбирает число в формате JSON и возвращает его в виде long или double.
        /// </summary>
        private static object ParseNumber()
        {
            if (_json == null) { throw new Exception("JSON string is null"); }
            
            int start = _index;

            if (_json[_index] == '-')
            {
                _index++;
            }

            while (_index < _json.Length && char.IsDigit(_json[_index]))
            {
                _index++;
            }

            if (_index < _json.Length && _json[_index] == '.')
            {
                _index++;
                while (_index < _json.Length && char.IsDigit(_json[_index]))
                {
                    _index++;
                }
            }

            if (_index < _json.Length && (_json[_index] == 'e' || _json[_index] == 'E'))
            {
                _index++;
                if (_index < _json.Length && (_json[_index] == '+' || _json[_index] == '-'))
                {
                    _index++;
                }
                while (_index < _json.Length && char.IsDigit(_json[_index]))
                {
                    _index++;
                }
            }

            string numStr = _json[start.._index];

            return numStr.IndexOf('.') != -1 || numStr.IndexOf('e') != -1 || numStr.IndexOf('E') != -1
                ? double.TryParse(numStr, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double d)
                    ? (object)d
                    : throw new Exception("Неверный формат числа: " + numStr)
                : long.TryParse(numStr, out long l) ? (object)l : throw new Exception("Неверный формат числа: " + numStr);
        }

        #endregion
    }
}
