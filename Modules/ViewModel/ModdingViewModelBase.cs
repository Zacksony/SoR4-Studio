using CommunityToolkit.Mvvm.ComponentModel;
using SoR4_Studio.Modules.DataModel.GameDataModel;
using System;
using System.IO;
using System.Reflection;

namespace SoR4_Studio.Modules.ViewModel;

internal abstract partial class ModdingViewModelBase : ObservableObject
{
    public ModdingViewModelBase()
    {
        ModChangedAction += OnModChanged;
    }

    public static GameDataIO ModIO { get; private set; } = new(GenV8Stream(), isCompressed: true);
    public static GameData Mod => ModIO.GameData;

    protected static readonly GameDataIO v8IO = new(GenV8Stream(), isCompressed: true);
    public static GameData V8 => v8IO.GameData;

    public static void ChangeMod(Stream stream, bool isCompressed)
    {
        ModIO.Dispose();
        ModIO = new(stream, isCompressed);
        ModChangedAction?.Invoke();
    }

    public static Action? ModChangedAction { get; private set; }

    protected virtual void OnModChanged() { }

    protected static Stream GenV8Stream()
        => Assembly.GetExecutingAssembly().GetManifestResourceStream("SoR4_Studio.BinaryResources.bin")!;
}
