// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Force.Crc32;

namespace SharpedUtilsCollection.FileUtils
{
    public static class Utils
    {
        public static void WriteStreamToFile(Stream stream,
                                             string destinationFile,
                                             int bufferSize = 4096,
                                             FileMode mode = FileMode.OpenOrCreate,
                                             FileAccess access = FileAccess.ReadWrite,
                                             FileShare share = FileShare.ReadWrite)
        {
            var destinationFileStream = new FileStream(destinationFile, mode, access, share);
            while (stream.Position < stream.Length)
            {
                destinationFileStream.WriteByte((byte)stream.ReadByte());
            }
        }

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

        ///<summary>Get files list in specified folder and subfolders</summary>
        public static List<string> CollectLocalFilesList(string path)
        {
            List<string> folders = new() { path };

            List<string> localFilesList = new();

            while (folders.Count > 0)
            {
                var dirs = Directory.GetDirectories(folders[0]);
                var files = Directory.GetFiles(folders[0]);

                localFilesList.AddRange(files);
                folders.AddRange(dirs);

                folders.RemoveAt(0);
            }

            return localFilesList;
        }
    }
}
