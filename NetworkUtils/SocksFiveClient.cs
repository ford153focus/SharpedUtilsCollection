// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MihaZupan;

namespace SharpedUtilsCollection.NetworkUtils;

public static class SocksFiveClient
{
    public static async Task<string> DownloadPage(
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

    /**
     * @param {int} timeOut Time out in seconds
     */
    public static async Task DownloadFile(
        string url, string path, 
        string proxyAddress="127.0.0.1", int proxyPort=9050, 
        Dictionary<string,string>? headers=null,
        HttpMethod? method = null,
        int timeOut = 0
    )
    {
        HttpToSocks5Proxy proxy = new(proxyAddress, proxyPort);
        HttpClientHandler handler = new() { Proxy = proxy };
        HttpClient httpClient = new(handler, true);

        if (timeOut > 0) httpClient.Timeout = TimeSpan.FromSeconds(timeOut);
        
        if (headers is not null)
            foreach(KeyValuePair<string,string> header in headers)
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);

        HttpRequestMessage request = new HttpRequestMessage(method ?? HttpMethod.Get, url);
        HttpResponseMessage? result = await httpClient.SendAsync(request);
        Stream? content = await result.Content.ReadAsStreamAsync();
        FileUtils.Utils.WriteStreamToFile(content, path);
    }
}
