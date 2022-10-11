// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;
using System.IO;

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
