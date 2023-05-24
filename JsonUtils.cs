// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.IO;
using System.Json;
using System.Text;
using System.Threading.Tasks;

namespace SharpedUtilsCollection;

public static class JsonUtils
{
    /**
     * Parse JSON-file to dynamic System.Json.JsonValue
     */
    public static async Task<JsonValue> DynamicFileParse(string filePath)
    {
        string fileContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
        return JsonValue.Parse(fileContent);
    }
}