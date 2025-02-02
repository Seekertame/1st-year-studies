using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1
{
    /// <summary>
    /// Универсальный класс JSON‑объекта, реализующий интерфейс IJSONObject.
    /// Хранит данные в виде словаря (ключ – строка, значение – object) и предоставляет
    /// методы доступа только для чтения.
    /// Кроме того, переопределён метод ToString() для корректной сериализации в JSON‑формат.
    /// </summary>
    public class GenericJsonObject : IJSONObject
    {
        private readonly Dictionary<string, object?> _data;

        /// <summary>
        /// Конструктор без параметров (требование задания) – создаёт пустой объект.
        /// </summary>
        public GenericJsonObject()
        {
            _data = [];
        }

        /// <summary>
        /// Конструктор, принимающий готовый словарь.
        /// </summary>
        public GenericJsonObject(Dictionary<string, object> data)
        {
            // Если в разобранном JSON отсутствуют некоторые ключи, их можно заполнить значениями по умолчанию.
            // Здесь просто сохраняем полученный словарь.
            _data = data;
        }

        public IEnumerable<string> GetAllFields()
        {
            return _data.Keys;
        }

        /// <summary>
        /// Возвращает строковое представление значения поля.
        /// Если значение является составным, то возвращается его сериализованное представление.
        /// Если поле не найдено или его значение равно null, возвращается пустая строка.
        /// </summary>
        public string GetField(string fieldName)
        {
            // Если поле отсутствует или его значение null, возвращаем пустую строку.
            return !_data.TryGetValue(fieldName, out object? value) || value == null ? string.Empty : SerializeValue(value);
        }

        /// <summary>
        /// Метод изменения полей не поддерживается после парсинга.
        /// </summary>
        public void SetField(string fieldName, string? value)
        {
            // Позволяем устанавливать поле только если его ещё нет.
            if (_data.ContainsKey(fieldName))
            {
                throw new KeyNotFoundException("Поле уже установлено и недоступно для изменения.");
            }
            _data[fieldName] = value;
        }

        /// <summary>
        /// Переопределённый метод ToString() для сериализации объекта в строку в формате JSON.
        /// </summary>
        public override string ToString()
        {
            return SerializeValue(_data);
        }

        /// <summary>
        /// Рекурсивно сериализует значение в JSON‑строку.
        /// </summary>
        private string SerializeValue(object value)
        {
            if (value == null)
            {
                // Возвращаем "null", если значение равно null.
                return "null";
            }

            if (value is string strValue)
            {
                return $"\"{EscapeString(strValue)}\"";
            }

            if (value is bool boolValue)
            {
                return boolValue ? "true" : "false";
            }

            if (value is Dictionary<string, object?> dict)
            {
                StringBuilder sb = new();
                _ = sb.Append("{");
                _ = sb.Append(string.Join(",", dict.Select(kvp =>
                    $"\"{EscapeString(kvp.Key)}\":{SerializeValue(kvp.Value!)}")));
                _ = sb.Append("}");
                return sb.ToString();
            }

            if (value is List<object> list)
            {
                StringBuilder sb = new();
                _ = sb.Append("[");
                _ = sb.Append(string.Join(",", list.Select(SerializeValue)));
                _ = sb.Append("]");
                return sb.ToString();
            }

            // Для чисел и прочих типов используем инвариантное представление
            if (value is double or float or int or long or decimal)
            {
                return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // Если тип не распознан, вызываем ToString() и заключаем в кавычки.
            return $"\"{EscapeString(value.ToString() ?? string.Empty)}\"";
        }

        /// <summary>
        /// Экранирует управляющие символы в строке.
        /// </summary>
        private string EscapeString(string s)
        {
            return s?.Replace("\\", "\\\\")
                     .Replace("\"", "\\\"")
                     .Replace("\n", "\\n")
                     .Replace("\r", "\\r")
                     .Replace("\t", "\\t") ?? string.Empty;
        }
    }
}
