using System;
using System.IO;

namespace SoR4_Studio.Modules.Utils;

internal static class TempFileManager
{
    private static readonly string TempFilePath = $"{Path.GetTempPath()}{CurrentVersion.Instance.AppName}{Path.DirectorySeparatorChar}";
    private static string GenTempFileName() => $"{TempFilePath}.{Convert.ToString(DateTime.UtcNow.Ticks, 16)}";

    public static FileStream GenNewTempFile()
    {
        Directory.CreateDirectory(TempFilePath);
        return File.Open(GenTempFileName(), FileMode.Create, FileAccess.ReadWrite);
    }

    public static void CleanUp()
    {
        if (!Directory.Exists(TempFilePath))
        {
            return;
        }

        string[] fileNames = Directory.GetFiles(TempFilePath);

        foreach (string fileName in fileNames)
        {
            if (!IsFileInUse(fileName))
            {
                File.Delete(fileName);
            }
        }
    }

    private static bool IsFileInUse(string fileName)
    {
        FileStream? fs;

        try
        {
            fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);
        }
        catch
        {
            return true;
        }

        fs?.Close();

        return false;
    }
}
