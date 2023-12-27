﻿using System.Reflection;

namespace SoR4_Studio.VersionManager;

internal class CurrentVersion
{
    public static CurrentVersion Instance { get; } = new();

    public string AppName { get; } = Assembly.GetExecutingAssembly().GetName().Name!;
    public string DisplayAppName => AppName.Replace("-", " ");
    public uint PreReleaseNumber { get; } = 0;
    public bool IsPreRelease => PreReleaseNumber != 0;
    public int MajorVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version!.Major;
    public int MinorVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version!.Minor;
    public int BuildVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version!.Build;
    public string AppVersion => $"{MajorVersion}.{MinorVersion}.{BuildVersion}";
    public string DisplayVersion => AppVersion + (IsPreRelease ? $" ( pre{PreReleaseNumber} )" : string.Empty);

}