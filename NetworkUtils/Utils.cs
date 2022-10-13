// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MihaZupan;

namespace SharpedUtilsCollection.NetworkUtils
{
    public class Utils
    {
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
