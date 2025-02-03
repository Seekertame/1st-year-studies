using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary1
{
    /// <summary>
    /// Структура "Рабочее место", представляющая данные из JSON.
    /// Реализует интерфейс IJSONObject.
    /// </summary>
    public readonly struct Workstation : IJSONObject
    {
        public string Id { get; }
        public string Label { get; }
        public string Desc { get; }
        public string Audio { get; }
        public IReadOnlyList<string> Hints { get; }
        public IReadOnlyList<Slot> Slots { get; }
        public IReadOnlyDictionary<string, string> XTriggers { get; }
        public string Category { get; }

        /// <summary>
        /// Конструктор, заполняющий поля рабочего места из словаря.
        /// Если какого-либо поля нет, используются значения по умолчанию.
        /// </summary>
        public Workstation(Dictionary<string, object> dict)
        {
            Id = dict.TryGetValue("id", out object? idVal) ? idVal?.ToString() ?? "" : "";
            Label = dict.TryGetValue("label", out object? labelVal) ? labelVal?.ToString() ?? "" : "";
            Desc = dict.TryGetValue("desc", out object? descVal) ? descVal?.ToString() ?? "" : "";
            Audio = dict.TryGetValue("audio", out object? audioVal) ? audioVal?.ToString() ?? "" : "";
            Category = dict.TryGetValue("category", out object? catVal) ? catVal?.ToString() ?? "" : "";

            // Извлекаем hints как список строк (если присутствуют)
            if (dict.TryGetValue("hints", out object? hintsObj) && hintsObj is List<object> hintsList)
            {
                List<string> hints = hintsList.Select(h => h?.ToString() ?? "").ToList();
                Hints = hints.AsReadOnly();
            }
            else
            {
                Hints = new List<string>().AsReadOnly();
            }

            // Извлекаем слоты как коллекцию структур Slot
            if (dict.TryGetValue("slots", out object? slotsObj) && slotsObj is List<object> slotsList)
            {
                List<Slot> slots = [];
                foreach (object s in slotsList)
                {
                    if (s is Dictionary<string, object> slotDict)
                    {
                        slots.Add(new Slot(slotDict));
                    }
                }
                Slots = slots.AsReadOnly();
            }
            else
            {
                Slots = new List<Slot>().AsReadOnly();
            }

            // Извлекаем xtriggers как словарь строк
            if (dict.TryGetValue("xtriggers", out object? xtObj) && xtObj is Dictionary<string, object> xt)
            {
                Dictionary<string, string> triggers = [];
                foreach (KeyValuePair<string, object> kvp in xt)
                {
                    triggers[kvp.Key] = kvp.Value?.ToString() ?? "";
                }
                XTriggers = triggers;
            }
            else
            {
                XTriggers = new Dictionary<string, string>();
            }
        }

        // Реализация IJSONObject

        public IEnumerable<string> GetAllFields()
        {
            return ["id", "label", "desc", "audio", "hints", "slots", "xtriggers", "category"];
        }

        public string GetField(string fieldName)
        {
            return fieldName.ToLowerInvariant() switch
            {
                "id" => Id,
                "label" => Label,
                "desc" => Desc,
                "audio" => Audio,
                "hints" => string.Join(", ", Hints),
                "slots" => $"Количество слотов: {Slots.Count}",
                "xtriggers" => string.Join(", ", XTriggers.Select(kvp => $"{kvp.Key}:{kvp.Value}")),
                "category" => Category,
                _ => "",
            };
        }

        public void SetField(string fieldName, string value)
        {
            throw new NotSupportedException("Поля структуры Workstation доступны только для чтения.");
        }
    }
}
