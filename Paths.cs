using System.IO;

namespace SharpedUtilsCollection;

public static class Paths
{
    /**
     * Create temporary folder
     */
    public static string GetTemporaryDirectory()
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

    /**
     * Remove from string all chars that can not be in path
     */
    public static string SafePath(string input)
    {
        return string.Concat(
            input.Split(Path.GetInvalidFileNameChars())
        );
    }
}
