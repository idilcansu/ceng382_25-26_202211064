using System.Text.Json;

namespace MyRazorApp.Helpers
{
    public class Utils
    {
        private static readonly Lazy<Utils> _instance = new Lazy<Utils>(() => new Utils());

        public static Utils Instance => _instance.Value;

        private Utils() { }

        public string ExportToJson<T>(IEnumerable<T> data, List<string>? selectedColumns = null)
{
    if (selectedColumns != null && selectedColumns.Any())
    {
        var filteredData = data.Select(item =>
        {
            var dict = new Dictionary<string, object?>();
            foreach (var prop in typeof(T).GetProperties())
            {
                if (selectedColumns.Contains(prop.Name))
                {
                    dict[prop.Name] = prop.GetValue(item)!; // null-forgiving operator burada
                }
            }
            return dict;
        });
        return JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });
    }

    return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
}

    }
}