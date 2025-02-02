using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private static string _json;
        private static int _index;

        /// <summary>
        /// Считывает данные из стандартного потока ввода (Console.In), разбирает их
        /// и возвращает объект, реализующий IJSONObject.
        /// Если корневой элемент не является объектом или после разбора остались лишние символы,
        /// генерируется исключение.
        /// </summary>
        public static IJSONObject ReadJson(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("Ошибка: путь к файлу не должен быть пустым или null.");
            }
            
            using (StreamReader reader = new(filePath, Encoding.UTF8))
            {
                Console.SetIn(reader);
                _json = Console.In.ReadToEnd();
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
                // Оборачиваем разобранный словарь в универсальный JSON‑объект.
                return new GenericJsonObject(dict);
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

        private static void SkipWhitespace()
        {
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
            if (_json.Substring(_index).StartsWith("true"))
            {
                _index += 4;
                return true;
            }
            if (_json.Substring(_index).StartsWith("false"))
            {
                _index += 5;
                return false;
            }
            if (_json.Substring(_index).StartsWith("null"))
            {
                _index += 4;
                return null;
            }
            throw new Exception($"Неожиданный символ '{c}' на позиции {_index}.");
        }

        /// <summary>
        /// Разбирает JSON-объект и возвращает его представление в виде словаря.
        /// </summary>
        private static Dictionary<string, object> ParseObject()
        {
            Dictionary<string, object> dict = new();
            _index++; // пропускаем '{'
            SkipWhitespace();

            if (_index < _json.Length && _json[_index] == '}')
            {
                _index++; // пустой объект
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
            List<object> list = new List<object>();
            _index++; // пропускаем '['
            SkipWhitespace();
            if (_index < _json.Length && _json[_index] == ']')
            {
                _index++; // пустой массив
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
            StringBuilder sb = new StringBuilder();
            _index++; // пропускаем открывающую кавычку
            while (_index < _json.Length)
            {
                char c = _json[_index++];
                if (c == '"')
                {
                    return sb.ToString();
                }
                if (c == '\\')
                {
                    if (_index >= _json.Length)
                    {
                        break;
                    }
                    char next = _json[_index++];
                    switch (next)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        default: sb.Append(next); break;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            throw new Exception("Не закрыта строка.");
        }

        /// <summary>
        /// Разбирает число в формате JSON и возвращает его в виде long или double.
        /// </summary>
        private static object ParseNumber()
        {
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

            string numStr = _json.Substring(start, _index - start);

            if (numStr.IndexOf('.') != -1 || numStr.IndexOf('e') != -1 || numStr.IndexOf('E') != -1)
            {
                if (double.TryParse(numStr, System.Globalization.NumberStyles.Any, 
                        System.Globalization.CultureInfo.InvariantCulture, out double d))
                {
                    return d;
                }
                else
                {
                    throw new Exception("Неверный формат числа: " + numStr);
                }
            }
            else
            {
                if (long.TryParse(numStr, out long l))
                {
                    return l;
                }
                else
                {
                    throw new Exception("Неверный формат числа: " + numStr);
                }
            }
        }


        #endregion
    }
    
}
