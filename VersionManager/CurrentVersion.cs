﻿



// Generated by text template.
// Don't edit the .cs file, edit the .tt file instead.

using System.Reflection;

[assembly: AssemblyVersion("0.2.0.1")]
[assembly: AssemblyFileVersion("0.2.0.1")]

namespace SoR4_Studio.VersionManager;

internal class CurrentVersion
{
    public static CurrentVersion Instance { get; } = new();

    public string AppName { get; } = Assembly.GetExecutingAssembly().GetName().Name!;
    public string DisplayAppName => AppName.Replace("-", " ");
    public uint PreReleaseNumber { get; } = 1;
    public bool IsPreRelease => PreReleaseNumber != 0;
    public uint MajorVersion { get; } = 0;
    public uint MinorVersion { get; } = 2;
    public uint BuildVersion { get; } = 0;
    public string AppVersion => $"{MajorVersion}.{MinorVersion}.{BuildVersion}";
    public string DisplayVersion => AppVersion + (IsPreRelease ? $" ( pre{PreReleaseNumber} )" : string.Empty);

}

