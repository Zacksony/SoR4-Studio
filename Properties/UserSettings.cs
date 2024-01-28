using SoR4_Studio.Modules.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Windows;

namespace SoR4_Studio.Properties;

// 通过此类可以处理设置类的特定事件: 
// 在更改某个设置的值之前将引发 SettingChanging 事件。
// 在更改某个设置的值之后将引发 PropertyChanged 事件。
// 在加载设置值之后将引发 SettingsLoaded 事件。
// 在保存设置值之前将引发 SettingsSaving 事件。
internal sealed partial class UserSettings
{
    public UserSettings()
    {
        SettingsSaving += SettingsSavingEventHandler;

        UpgradeSettingsIfNewVersion();

        InitModdingLanguageSetting();
        InitAppLanguageSetting();
    }

    public void UpgradeSettingsIfNewVersion()
    {
        var configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
        if (!File.Exists(configPath))
        {
            Upgrade();
            Reload();
            Save();
        }
    }

    #region ModdingLanguage

    public static SortedSet<string> AcceptableModdingLanguages { get; } =
    [
        "br",
        "de",
        "en",
        "es",
        "fr",
        "it",
        "ja",
        "ko",
        "uk",
        "zh",
        "zt",
        "ztpc"
    ];

    public static string DefaultModdingLanguage => Thread.CurrentThread.CurrentCulture.Name == "zh-CN" ? "zh" : "en";

    private void InitModdingLanguageSetting()
    {
        if (AcceptableModdingLanguages.Contains(ModdingLanguage))
        {
            return;
        }

        ModdingLanguage = DefaultModdingLanguage;
    }

    #endregion

    #region AppLanguage

    private const string AppLanguagesRootUri = "pack://application:,,,/SoR4-Studio;component/Modules/Windows/Dictionary/Languages/";

    private static Uri GetAppLanguageDictionaryUri(string languageName) => new($"{AppLanguagesRootUri}{languageName}.xaml", UriKind.Absolute);

    public static SortedDictionary<string, string> AcceptableAppLanguages { get; } = new()
    {
        ["en_us"] = "English",
        ["zh_cn"] = "简体中文",
    };

    public static string DefaultAppLanguage => Thread.CurrentThread.CurrentCulture.Name == "zh-CN" ? "zh_cn" : "en_us";

    private void InitAppLanguageSetting()
    {
        if (AcceptableAppLanguages.ContainsKey(AppLanguage))
        {
            return;
        }

        AppLanguage = DefaultAppLanguage;
    }

    #endregion

    #region EventHandlers

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(sender, e);

        switch (e.PropertyName)
        {
            case nameof(AppLanguage): OnAppLanguageChanged(); return;
            case nameof(ModdingLanguage): OnModdingLanguageChanged(); return;
        }
    }

    private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
    {
        SavePath = "";
    }

    private static void OnModdingLanguageChanged()
    {
        ModdingViewModelBase.ModChangedAction?.Invoke();
    }

    private void OnAppLanguageChanged()
    {
        if (!AcceptableAppLanguages.ContainsKey(AppLanguage))
        {
            AppLanguage = DefaultAppLanguage;
            return;
        }

        Application.Current.Resources.MergedDictionaries[0] = new()
        {
            Source = GetAppLanguageDictionaryUri(AppLanguage)
        };

        ModdingViewModelBase.ModChangedAction?.Invoke();
    }

    #endregion
}
