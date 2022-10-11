using System.IO;
using System.Json;
using System.Text;
using System.Threading.Tasks;

namespace SharpedUtilsCollection
{
    public static class JsonUtils
    {
        public static async Task<JsonValue> DynamicFileParse(string filePath)
        {
            string fileContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            return JsonValue.Parse(fileContent);
        }
    }
}
