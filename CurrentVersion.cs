using System.Reflection;

namespace SoR4_Studio;

internal class CurrentVersion
{
    public static CurrentVersion Instance { get; } = new();

    public string AppName { get; } = Assembly.GetExecutingAssembly().GetName().Name!;
    public int MajorVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version!.Major;
    public int MinorVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version!.Minor;
    public int BuildVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version!.Build;
    public string DisplayVersion => $"{MajorVersion}.{MinorVersion}.{BuildVersion}";
}
