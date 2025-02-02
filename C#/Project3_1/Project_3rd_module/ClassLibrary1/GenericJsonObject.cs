using System;
using System.Collections.Generic;
using System.Globalization;
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
            if (!_data.TryGetValue(fieldName, out object? value) || value == null)
            {
                // Комментарий: возвращается пустая строка, если значение поля не найдено или равно null.
                return string.Empty;
            }
            return SerializeValue(value);
        }

        /// <summary>
        /// Метод изменения полей не поддерживается после парсинга.
        /// Позволяет установить значение только для нового поля.
        /// </summary>
        public void SetField(string fieldName, string? value)
        {
            // Если поле уже установлено, выбрасываем исключение.
            if (_data.ContainsKey(fieldName))
            {
                throw new KeyNotFoundException("Поле уже установлено и недоступно для изменения.");
            }
            _data[fieldName] = value;
        }

        /// <summary>
        /// Переопределённый метод ToString() для сериализации объекта в строку в формате JSON.
        /// Здесь используется компактное представление без отступов.
        /// </summary>
        public override string ToString()
        {
            return SerializeValue(_data);
        }

        /// <summary>
        /// Возвращает человекочитаемое (pretty printed) представление JSON-объекта.
        /// </summary>
        public string PrettyPrint()
        {
            return SerializeValuePretty(_data, 0);
        }

        /// <summary>
        /// Рекурсивно сериализует значение в JSON‑строку без форматирования.
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
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append(string.Join(",", dict.Select(kvp =>
                    $"\"{EscapeString(kvp.Key)}\":{SerializeValue(kvp.Value!)}")));
                sb.Append("}");
                return sb.ToString();
            }

            if (value is List<object> list)
            {
                StringBuilder sb = new StringBuilder();
                _ = sb.Append("[");
                _ = sb.Append(string.Join(",", list.Select(SerializeValue)));
                _ = sb.Append("]");
                return sb.ToString();
            }

            // Для чисел и прочих типов используем инвариантное представление
            if (value is double or float or int or long or decimal)
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }

            // Если тип не распознан, вызываем ToString() и заключаем в кавычки.
            return $"\"{EscapeString(value.ToString() ?? string.Empty)}\"";
        }

        /// <summary>
        /// Рекурсивно сериализует значение в человекочитаемую JSON‑строку с отступами.
        /// </summary>
        /// <param name="value">Объект для сериализации.</param>
        /// <param name="indentLevel">Уровень отступа (0 для корневого элемента).</param>
        private string SerializeValuePretty(object value, int indentLevel)
        {
            string indent = new string(' ', indentLevel * 4);
            string indentChild = new string(' ', (indentLevel + 1) * 4);

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

            if (value is Dictionary<string, object?> dict)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("{");
                bool first = true;
                foreach (KeyValuePair<string, object?> kvp in dict)
                {
                    if (!first)
                    {
                        sb.AppendLine(",");
                    }
                    else
                    {
                        first = false;
                    }
                    sb.Append(indentChild);
                    sb.Append($"\"{EscapeString(kvp.Key)}\": ");
                    sb.Append(SerializeValuePretty(kvp.Value!, indentLevel + 1));
                }
                sb.AppendLine();
                sb.Append(indent);
                sb.Append("}");
                return sb.ToString();
            }

            if (value is List<object> list)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[");
                bool first = true;
                foreach (object item in list)
                {
                    if (!first)
                    {
                        sb.AppendLine(",");
                    }
                    else
                    {
                        first = false;
                    }
                    sb.Append(indentChild);
                    sb.Append(SerializeValuePretty(item, indentLevel + 1));
                }
                sb.AppendLine();
                sb.Append(indent);
                sb.Append("]");
                return sb.ToString();
            }

            if (value is double or float or int or long or decimal)
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture);
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
