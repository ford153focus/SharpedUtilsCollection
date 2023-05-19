using System.IO;

namespace SharpedUtilsCollection;

public class Paths
{
    static string GetTemporaryDirectory()
    {
        var folder = Path.Combine(
            Path.GetTempPath(),
            Path.GetFileNameWithoutExtension(
                Path.GetRandomFileName()
            )
        );

        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        return folder;
    }
}
