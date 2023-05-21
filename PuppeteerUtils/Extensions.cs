using System.Threading.Tasks;
using PuppeteerSharp;

namespace SharpedUtilsCollection.PuppeteerUtils
{
    public static class Extensions
    {
        public static async Task<string> GetInnerText(this IElementHandle el)
        {
            var property = await el.GetPropertyAsync("innerText");
            var value = await property.JsonValueAsync<string>();
            return value;
        }
    }
}