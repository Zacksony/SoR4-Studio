using Microsoft.Win32;
using SoR4_Studio.Modules.ViewModel;
using SoR4_Studio.Properties;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SoR4_Studio.Modules.Windows.Tabs;

/// <summary>
/// OptionsTab.xaml 的交互逻辑
/// </summary>
public partial class OptionsTab : UserControl
{
    public OptionsTab()
    {
        InitializeComponent();

        InitModdingLanguageComboBox();
    }

    private void InitModdingLanguageComboBox()
    {
        ComboBox_ModdingLanguage.ItemsSource = UserSettings.AcceptableModdingLanguages;
        ComboBox_ModdingLanguage.SelectedItem = UserSettings.Default.ModdingLanguage;

        ComboBox_AppLanguage.ItemsSource = UserSettings.AcceptableAppLanguages;
        ComboBox_AppLanguage.DisplayMemberPath = "Value";
        ComboBox_AppLanguage.SelectedValuePath = "Key";
        ComboBox_AppLanguage.SelectedValue = UserSettings.Default.AppLanguage;
    }

    private void ComboBox_AppLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UserSettings.Default.AppLanguage = (string)ComboBox_AppLanguage.SelectedValue;
    }

    private void Button_Options_OpenGamePath_Click(object sender, RoutedEventArgs e)
    {
        OpenFolderDialog dialog = new()
        {
            Title = "Find \"Streets of Rage 4\\\"",
            Multiselect = false
        };

        if (dialog.ShowDialog() is true)
        {
            MainWindowViewModel.GamePath = dialog.FolderName;
        }
        else
        {
            return;
        }

        if (!(File.Exists(MainWindowViewModel.GameExePath) && File.Exists(MainWindowViewModel.GameBigfilePath)))
        {
            MessageBox.Show("Invalid game path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            MainWindowViewModel.GamePath = "";
            return;
        }
    }

    private void ComboBox_ModdingLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UserSettings.Default.ModdingLanguage = (string)ComboBox_ModdingLanguage.SelectedItem;
    }

    private void Button_Options_UnifyGameLanguage_Click(object sender, RoutedEventArgs e)
    {
        ModdingViewModelBase.Mod.LocalizationData.UnifyLanguage();
    }
}
