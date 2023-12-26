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

    protected static GameDataIO modIO = new(GenV8Stream(), isCompressed: false);
    public static GameData Mod => modIO.GameData;

    protected static readonly GameDataIO v8IO = new(GenV8Stream(), isCompressed: false);
    public static GameData V8 => v8IO.GameData;

    public static void ChangeMod(Stream stream, bool isCompressed)
    {
        modIO.Dispose();
        modIO = new(stream, isCompressed);
        ModChangedAction?.Invoke();
    }

    public static Action? ModChangedAction { get; private set; }

    protected virtual void OnModChanged() { }

    protected static Stream GenV8Stream()
        => Assembly.GetExecutingAssembly().GetManifestResourceStream("SoR4_Studio.BinaryResources.bin")!;
}
