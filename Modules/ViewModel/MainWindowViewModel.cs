using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SoR4_Studio.Modules.Utils;
using SoR4_Studio.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SoR4_Studio.Modules.ViewModel;

// 这部分的MVVM设计非常乱，或者可以说压根就不是MVVM。找个时间重新设计一下

internal partial class MainWindowViewModel : ModdingViewModelBase
{
    private const string GITHUB_RELEASE_URL = "https://github.com/Zacksony/SoR4-Studio/releases";

    public static string SavePath
    {
        get => UserSettings.Default.SavePath;
        set => UserSettings.Default.SavePath = value;
    }

    public static string GamePath
    {
        get => UserSettings.Default.GamePath;
        set => UserSettings.Default.GamePath = value;
    }

    public static string GameExePath => GamePath + @"\x64\SOR4.exe";
    public static string GameBigfilePath => GamePath + @"\data\bigfile";

    public static bool SaveCanceled { get; set; } = false;

    [RelayCommand]
    private static void Github()
    {
        System.Diagnostics.Process.Start(new ProcessStartInfo()
        {
            FileName = GITHUB_RELEASE_URL,
            UseShellExecute = true
        });
    }

    [RelayCommand]
    private static void New()
    {
        SaveCanceled = false;
        var result = MessageBox.Show("Save before new?", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Cancel)
        {
            return;
        }
        if (result == MessageBoxResult.Yes)
        {
            Save();
        }
        if (SaveCanceled)
        {
            SaveCanceled = false;
            return;
        }

        ChangeMod(GenV8Stream(), isCompressed: false);
    }

    [RelayCommand]
    private static void Save()
    {
        if (File.Exists(SavePath))
        {
            DoBackup();
            modIO.Save(SavePath);
        }
        else
        {
            SaveAs();
        }
    }

    [RelayCommand]
    private static void SaveAs()
    {
        SaveFileDialog dialog = new()
        {
            Title = "Save the bigfile..",
            Filter = "所有文件|*.*|bigfile|bigfile"
        };

        if (dialog.ShowDialog() is true)
        {
            SaveCanceled = false;

            SavePath = dialog.FileName;

            if (File.Exists(SavePath))
            {
                DoBackup();
            }

            modIO.Save(SavePath);
        }
        else
        {
            SaveCanceled = true;
        }
    }

    private static void JustSave(string path)
    {
        modIO.Save(path);
    }

    [RelayCommand]
    private static void Load()
    {
        SaveCanceled = false;
        var result = MessageBox.Show("Save before open?", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Cancel)
        {
            return;
        }
        if (result == MessageBoxResult.Yes)
        {
            Save();
        }
        if (SaveCanceled)
        {
            SaveCanceled = false;
            return;
        }

        OpenFileDialog dialog = new()
        {
            Title = "Load a bigfile..",
            Filter = "所有文件|*.*|bigfile|bigfile",
            Multiselect = false
        };

        if (dialog.ShowDialog() is true)
        {
            using FileStream copy = TempFileManager.GenNewTempFile();
            modIO.Output(copy);

            try
            {
                ChangeMod(File.Open(dialog.FileName, FileMode.Open), true);
            }
            catch
            {
                MessageBox.Show("Cannot open as bigfile!", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                ChangeMod(copy, false);
                return;
            }

            SavePath = dialog.FileName;
        }
    }

    [RelayCommand]
    private static void ApplyAndRun()
    {
        if (!File.Exists(GameExePath))
        {
            OpenFolderDialog dialog = new()
            {
                Title = "Find \"Streets of Rage 4\\\"",
                Multiselect = false
            };

            if (dialog.ShowDialog() is true)
            {
                GamePath = dialog.FolderName;
            }
            else
            {
                return;
            }

            if (!(File.Exists(GameExePath) && File.Exists(GameBigfilePath)))
            {
                MessageBox.Show("Invalid game path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
        }

        JustSave(GameBigfilePath);

        System.Diagnostics.Process pExecuteEXE = new();
        pExecuteEXE.StartInfo.FileName = GameExePath;
        pExecuteEXE.Start();
    }

    private static void DoBackup()
    {
        using FileStream original = File.Open(SavePath, FileMode.Open);
        using FileStream backup = File.Create($"{SavePath}.backup.{DateTime.Now.ToString().Replace("/", "-").Replace(":", "-")}");
        original.CopyTo(backup);
    }
}
