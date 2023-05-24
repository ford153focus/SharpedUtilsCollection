using System.Threading.Tasks;
using PuppeteerSharp;

namespace SharpedUtilsCollection.PuppeteerUtils
{
    public static class Extensions
    {
        public static async Task<string> GetInnerText(this IElementHandle el)
        {
            IJSHandle? property = await el.GetPropertyAsync("innerText");
            string? value = await property.JsonValueAsync<string>();
            return value ?? "";
        }
    }
}