using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary1
{
    /// <summary>
    /// Структура "Слот", представляющая данные одного слота рабочего места.
    /// Реализует интерфейс IJSONObject.
    /// </summary>
    /// <remarks>
    /// Конструктор, заполняющий данные слота из словаря.
    /// Если какого-либо поля нет, используется пустой словарь.
    /// </remarks>
    public readonly struct Slot(Dictionary<string, object> dict) : IJSONObject
    {
        public string Id { get; } = dict.TryGetValue("id", out object? value) ? value?.ToString() ?? "" : "";
        public string Label { get; } = dict.TryGetValue("label", out object? value) ? value?.ToString() ?? "" : "";
        public string Description { get; } = dict.TryGetValue("description", out object? value) ? value?.ToString() ?? "" : "";
        public IReadOnlyDictionary<string, int> Essential { get; } = ExtractIntDictionary(dict, "essential");
        public IReadOnlyDictionary<string, int> Required { get; } = ExtractIntDictionary(dict, "required");
        public IReadOnlyDictionary<string, int> Forbidden { get; } = ExtractIntDictionary(dict, "forbidden");

        private static IReadOnlyDictionary<string, int> ExtractIntDictionary(Dictionary<string, object> dict, string key)
        {
            Dictionary<string, int> result = [];
            if (dict.TryGetValue(key, out object? value) && value is Dictionary<string, object> inner)
            {
                foreach (KeyValuePair<string, object> kv in inner)
                {
                    if (int.TryParse(kv.Value?.ToString(), out int val))
                    {
                        result[kv.Key] = val;
                    }
                }
            }
            return result;
        }

        // Реализация IJSONObject

        public readonly IEnumerable<string> GetAllFields()
        {
            return ["id", "label", "description", "essential", "required", "forbidden"];
        }

        public readonly string GetField(string fieldName)
        {
            return fieldName.ToLowerInvariant() switch
            {
                "id" => Id,
                "label" => Label,
                "description" => Description,
                "essential" => string.Join(", ", Essential.Select(kvp => $"{kvp.Key}:{kvp.Value}")),
                "required" => string.Join(", ", Required.Select(kvp => $"{kvp.Key}:{kvp.Value}")),
                "forbidden" => string.Join(", ", Forbidden.Select(kvp => $"{kvp.Key}:{kvp.Value}")),
                _ => "",
            };
        }

        public void SetField(string fieldName, string value)
        {
            throw new NotSupportedException("Поля структуры Slot доступны только для чтения.");
        }
    }
}
