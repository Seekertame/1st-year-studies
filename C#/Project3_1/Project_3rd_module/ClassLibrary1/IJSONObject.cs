namespace ClassLibrary1
{
    /// <summary>
    /// Интерфейс для JSON‑объектов.
    /// Все классы, описывающие JSON‑объекты, должны реализовывать данный интерфейс.
    /// </summary>
    public interface IJSONObject
    {
        IEnumerable<string> GetAllFields();
        string GetField(string fieldName);
        void SetField(string fieldName, string value);
    }
}