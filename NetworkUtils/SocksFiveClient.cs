// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MihaZupan;

namespace SharpedUtilsCollection.NetworkUtils;

public class SocksFiveClient
{
    public static async Task<string> DownloadPageThruProxy(
        string url, 
        string proxyAddress="127.0.0.1", int proxyPort=9050, 
        Dictionary<string,string>? headers=null
    )
    {
        HttpToSocks5Proxy proxy = new(proxyAddress, proxyPort);
        HttpClientHandler handler = new() { Proxy = proxy };
        HttpClient httpClient = new(handler, true);

        if (headers is not null)
            foreach(KeyValuePair<string,string> header in headers)
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        
        var result = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
        var content = await result.Content.ReadAsStringAsync();
        return content;
    }

    public static async Task DownloadFileThruProxy(
        string url, string path, 
        string proxyAddress="127.0.0.1", int proxyPort=9050, 
        Dictionary<string,string>? headers=null
    )
    {
        HttpToSocks5Proxy proxy = new(proxyAddress, proxyPort);
        HttpClientHandler handler = new() { Proxy = proxy };
        HttpClient httpClient = new(handler, true);

        if (headers is not null)
            foreach(KeyValuePair<string,string> header in headers)
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);

        var result = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
        var content = await result.Content.ReadAsStreamAsync();
        FileUtils.Utils.WriteStreamToFile(content, path);
    }
}
