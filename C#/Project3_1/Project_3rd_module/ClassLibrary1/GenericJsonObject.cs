using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly Dictionary<string, object> _data;

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
        /// </summary>
        public string GetField(string fieldName)
        {
            return !_data.TryGetValue(fieldName, out object? value) ? string.Empty : SerializeValue(value); // Даёт пустую строку, если поле не найдено или равно null. 
        }

        /// <summary>
        /// Метод изменения полей не поддерживается, т.к. поля доступны только для чтения.
        /// </summary>
        public void SetField(string fieldName, string? value)
        {
            throw new KeyNotFoundException("Поле недоступно для изменения.");
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

            if (value is Dictionary<string, object> dict)
            {
                StringBuilder sb = new();
                return sb.Append("{")
                    .Append(string.Join(",", dict.Select(kvp =>
                        $"\"{EscapeString(kvp.Key)}\":{SerializeValue(kvp.Value)}")))
                    .Append("}")
                    .ToString();
            }

            if (value is List<object> list)
            {
                StringBuilder sb = new();
                return sb.Append("[")
                    .Append(string.Join(",", list.Select(SerializeValue)))
                    .Append("]")
                    .ToString();
            }

            // Для чисел и прочих типов используем инвариантное представление
            if (value is double or float or int or long or decimal)
            {
                return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture); 
                // Какой тут null, когда выше рассмотрен случай, что value == null
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