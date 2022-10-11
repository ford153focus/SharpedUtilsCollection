using System.IO;
using System.Json;
using System.Text;
using System.Threading.Tasks;

namespace FordToolbox
{
    public class JsonUtils
    {
        private static async Task<JsonValue> DynamicParse(string filePath)
        {
            var fileContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            return JsonValue.Parse(fileContent);
        }
    }
}
