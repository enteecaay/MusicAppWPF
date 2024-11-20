using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public static class JsonDatabase
{
    private static readonly string BasePath = "./JsonDatabase";

    public static async Task<List<T>> ReadAsync<T>(string fileName)
    {
        var filePath = Path.Combine(BasePath, fileName);
        if (!File.Exists(filePath))
        {
            return new List<T>();
        }

        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<T>>(json);
    }

    public static async Task WriteAsync<T>(string fileName, List<T> data)
    {
        var filePath = Path.Combine(BasePath, fileName);
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public static async Task DeleteAsync(string fileName)
    {
        var filePath = Path.Combine(BasePath, fileName);
        if (File.Exists(filePath))
        {
            await Task.Run(() => File.Delete(filePath));
        }
    }
}