// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace SharpedUtilsCollection.NetworkUtils;

internal class MyWebClient : WebClient
{
    // Overrides the GetWebRequest method and sets keep alive to false
    protected override WebRequest GetWebRequest(Uri address)
    {
        HttpWebRequest? req = base.GetWebRequest(address) as HttpWebRequest;
        Debug.Assert(req != null, nameof(req) + " != null");
        req.KeepAlive = true;
        return req;
    }
}

public static class HttpProxyUtils
{
    public static async Task<string> DownloadPage(
        string url, 
        string address="127.0.0.1", int port=9080, 
        Dictionary<string,string>? headers=null
    )
    {
        WebClient client = new MyWebClient();
        
        WebProxy proxy = new WebProxy($"http://{address}:{port.ToString()}");
        // proxy.Address = new Uri(address);
        // proxy.Credentials = new NetworkCredential("usernameHere", "pa****rdHere");  //These can be replaced by user input
        // proxy.UseDefaultCredentials = false;
        proxy.BypassProxyOnLocal = true;
        client.Proxy = proxy;

        if (headers is not null)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                if (header.Key == "HeaderNames.Connection") continue;
                client.Headers.Add(header.Key, header.Value);
            }
        }

        string doc = await client.DownloadStringTaskAsync(url);
        
        return doc;
    }

    /**
     * @param {int} timeOut Time out in seconds
     */
    public static async Task DownloadFile(
        string url, string path, 
        string address="127.0.0.1", int port=9080, 
        Dictionary<string,string>? headers=null
    )
    {
        WebClient client = new MyWebClient();
        
        WebProxy proxy = new WebProxy($"http://{address}:{port.ToString()}");
        // proxy.Address = new Uri(address);
        // proxy.Credentials = new NetworkCredential("usernameHere", "pa****rdHere");  //These can be replaced by user input
        // proxy.UseDefaultCredentials = false;
        proxy.BypassProxyOnLocal = true;
        client.Proxy = proxy;
        
        if (headers is not null)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                if (header.Key == "HeaderNames.Connection") continue;
                client.Headers.Add(header.Key, header.Value);
            }
        }

        await client.DownloadFileTaskAsync(url, path);
    }
}
