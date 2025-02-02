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
            _data = new Dictionary<string, object>();
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
            if (!_data.ContainsKey(fieldName))
            {
                return null;
            }
            return SerializeValue(_data[fieldName]);
        }

        /// <summary>
        /// Метод изменения полей не поддерживается, т.к. поля доступны только для чтения.
        /// </summary>
        public void SetField(string fieldName, string value)
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
                StringBuilder sb = new StringBuilder();
                sb.Append("{");

                bool first = true;
                foreach (KeyValuePair<string, object> kvp in dict)
                {
                    if (!first)
                    {
                        sb.Append(",");
                    }

                    sb.Append($"\"{EscapeString(kvp.Key)}\":");
                    sb.Append(SerializeValue(kvp.Value));

                    first = false;
                }

                sb.Append("}");
                return sb.ToString();
            }

            if (value is List<object> list)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");

                bool first = true;
                foreach (object item in list)
                {
                    if (!first)
                    {
                        sb.Append(",");
                    }

                    sb.Append(SerializeValue(item));
                    first = false;
                }

                sb.Append("]");
                return sb.ToString();
            }

            // Для чисел и прочих типов используем инвариантное представление
            if (value is double || value is float || value is int || value is long || value is decimal)
            {
                return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
            }

            // Если тип не распознан, вызываем ToString() и заключаем в кавычки.
            return $"\"{EscapeString(value.ToString())}\"";
        }


        /// <summary>
        /// Экранирует управляющие символы в строке.
        /// </summary>
        private string EscapeString(string s)
        {
            return s.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t");
        }
    }
}