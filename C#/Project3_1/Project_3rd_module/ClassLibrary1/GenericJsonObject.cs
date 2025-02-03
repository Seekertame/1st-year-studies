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
        /// Позволяет получить коллекцию (List<object>) из объекта по заданному ключу.
        /// Если ключ отсутствует или значение не является коллекцией, возвращает null.
        /// </summary>
        /// <param name="key">Ключ, по которому хранится коллекция (например, "verbs")</param>
        public List<object>? GetCollection(string key)
        {
            if (_data.TryGetValue(key, out object? collectionObj) && collectionObj is List<object> collection)
            {
                return collection;
            }
            return null;
        }
        
        /// <summary>
        /// Фильтрует коллекцию объектов внутри JSON по заданному полю и списку разрешённых значений.
        /// Если значение поля отсутствует, объект исключается.
        /// Если значение поля равно null, объект оставляется.
        /// После фильтрации коллекция перезаписывается.
        /// </summary>
        /// <param name="collectionKey">Ключ коллекции в JSON (например, "verbs")</param>
        /// <param name="fieldName">
        /// Поле внутри каждого объекта коллекции, по которому производится фильтрация (например, "category").
        /// Если у объекта нет такого поля, элемент не включается в отфильтрованный результат.
        /// </param>
        /// <param name="allowedValues">Список значений, которые должны остаться в фильтруемом поле</param>
        public void FilterCollection(string collectionKey, string fieldName, List<string> allowedValues)
        {
            // Проверяем, существует ли коллекция по заданному ключу.
            if (!_data.TryGetValue(collectionKey, out object? collectionObj))
            {
                Console.WriteLine($"Ключ '{collectionKey}' не найден в JSON-объекте.");
                return;
            }

            if (!(collectionObj is List<object> collection))
            {
                Console.WriteLine($"Значение по ключу '{collectionKey}' не является коллекцией.");
                return;
            }

            List<object> filtered = new List<object>();

            foreach (object item in collection)
            {
                // Если элемент коллекции – это словарь (JSON-объект)
                if (item is Dictionary<string, object> dict)
                {
                    // Если объект не содержит нужное поле, исключаем его из результатов фильтрации.
                    if (!dict.TryGetValue(fieldName, out object? fieldValue))
                    {
                        continue; // Пропускаем этот элемент
                    }
                    else
                    {
                        // Если значение поля равно null, оставляем объект.
                        if (fieldValue == null)
                        {
                            filtered.Add(item);
                        }
                        else
                        {
                            // Приводим значение к строке.
                            string strVal = fieldValue.ToString() ?? "";
                            // Если значение содержится в списке разрешённых, оставляем объект.
                            if (allowedValues.Contains(strVal))
                            {
                                filtered.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    // Если элемент коллекции не является объектом, оставляем его без фильтрации.
                    filtered.Add(item);
                }
            }

            // Перезаписываем коллекцию в исходном объекте.
            _data[collectionKey] = filtered;
        }
        
        
        /// <summary>
        /// Сортирует коллекцию объектов внутри JSON по значению указанного поля.
        /// Null-значения и отсутствующие поля не исключаются, их сортировка проводится согласно стандартным правилам.
        /// После сортировки коллекция перезаписывается.
        /// </summary>
        /// <param name="collectionKey">Ключ коллекции в JSON (например, "verbs")</param>
        /// <param name="sortField">Поле внутри каждого объекта коллекции, по которому производится сортировка</param>
        /// <param name="ascending">Направление сортировки: true - по возрастанию, false - по убыванию</param>
        public void SortCollection(string collectionKey, string sortField, bool ascending)
        {
            // Проверяем, существует ли коллекция по заданному ключу.
            if (!_data.TryGetValue(collectionKey, out object? collectionObj))
            {
                Console.WriteLine($"Ключ '{collectionKey}' не найден.");
                return;
            }
            if (!(collectionObj is List<object> collection))
            {
                Console.WriteLine($"Значение по ключу '{collectionKey}' не является коллекцией.");
                return;
            }

            // Локальный метод для сравнения значений поля.
            int CompareField(object? a, object? b)
            {
                if (a == null && b == null) { return 0; }
                if (a == null) { return -1; }
                if (b == null) { return 1; }
                // Попробуем числовое сравнение, если возможно.
                if (double.TryParse(a.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double da) &&
                    double.TryParse(b.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double db))
                {
                    return da.CompareTo(db);
                }
                // Иначе сравниваем как строки.
                return StringComparer.InvariantCulture.Compare(a.ToString(), b.ToString());
            }

            // Локальный метод для сравнения двух элементов коллекции.
            int CompareItems(object o1, object o2)
            {
                // Если оба элемента являются словарями (JSON-объектами)
                if (o1 is Dictionary<string, object> d1 && o2 is Dictionary<string, object> d2)
                {
                    // Извлекаем значение для sortField (если поле отсутствует, считаем его null).
                    d1.TryGetValue(sortField, out object? v1);
                    d2.TryGetValue(sortField, out object? v2);
                    int cmp = CompareField(v1, v2);
                    return ascending ? cmp : -cmp;
                }
                // Если элементы не словари, оставляем порядок без изменений.
                return 0;
            }

            // Сортируем коллекцию с помощью лямбда-выражения.
            collection.Sort((o1, o2) => CompareItems(o1, o2));

            // Перезаписываем коллекцию в исходном объекте.
            _data[collectionKey] = collection;
        }



        #region Сериализация JSON

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
        #endregion
    }
}
