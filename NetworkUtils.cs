// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MihaZupan;

namespace FordToolbox
{
    public class NetworkUtils
    {
        public static async Task DownloadFileThruProxy(string url, string dlPath, string proxyAddress="127.0.0.1", int proxyPort=9050)
        {
            HttpToSocks5Proxy proxy = new(proxyAddress, proxyPort);
            HttpClientHandler handler = new() { Proxy = proxy };
            HttpClient httpClient = new(handler, true);

            var result = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
            var content = await result.Content.ReadAsStreamAsync();
            FileUtils.WriteStreamToFile(content, dlPath);
        }

        public static bool IsPortOpen(string host, int port, int timeout = 4444, int retryCount = 1)
        {
            while (retryCount > 0)
            {
                try
                {
                    using var client = new TcpClient();
                    var result = client.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(timeout);
                    if (success)
                    {
                        client.EndConnect(result);
                        return true;
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {}

                retryCount--;
                Thread.Sleep(timeout);
            }

            return false;
        }
    }
}
