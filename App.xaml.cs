using System;
using System.Windows;

namespace SoR4_Studio;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new Exception("The App only supports Windows OS.");
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SoR4_Studio.Properties.UserSettings.Default.Save();
    }
}
