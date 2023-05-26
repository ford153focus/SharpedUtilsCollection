// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
using System.Threading.Tasks;
using PuppeteerSharp;

namespace SharpedUtilsCollection.PuppeteerUtils;

public static class Extensions
{
    public static async Task<string> GetInnerText(this IElementHandle el)
    {
        return await GetPropertyValue(el, "innerText");
    }

    public static async Task<string> GetPropertyValue(this IElementHandle el, string propertyName)
    {
        IJSHandle? property = await el.GetPropertyAsync(propertyName);
        string? value = await property.JsonValueAsync<string>();
        return value ?? "";
    }
}