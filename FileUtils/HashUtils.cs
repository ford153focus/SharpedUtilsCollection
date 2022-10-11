using System.IO;
using System.Security.Cryptography;
using System.Text;
using Force.Crc32;

namespace SharpedUtilsCollection.FileUtils
{
    public class HashUtils
    {
        public static uint GetCrc32(string filePath)
        {
            return Crc32Algorithm.Compute(File.ReadAllBytes(filePath));
        }

        public static string GetMd5(string filePath)
        {
            var md5 = MD5.Create();
            var stream = File.OpenRead(filePath);
            return Encoding.Default.GetString(md5.ComputeHash(stream));
        }
    }
}