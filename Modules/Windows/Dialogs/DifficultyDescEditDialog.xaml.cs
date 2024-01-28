using SoR4_Studio.Modules.ViewModel;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace SoR4_Studio.Modules.Windows.Dialogs;

/// <summary>
/// DifficultyDescEditDialog.xaml 的交互逻辑
/// </summary>
public partial class DifficultyDescEditDialog : UserControl
{
    public DifficultyDescEditDialog(string difficultyDesc, string selectedDifficultyName)
    {
        InitializeComponent();
        DataContext = new DifficultyDescEditDialogViewModel(difficultyDesc, selectedDifficultyName);
    }

    [LibraryImport("User32.dll")]
    private static partial IntPtr SetFocus(IntPtr hWnd);

    private bool _isFirstTime = true;

    private void UserControl_GotFocus(object sender, RoutedEventArgs e)
    {
        HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
        if (source != null)
        {
            SetFocus(source.Handle);
            TextBox_DescEdit.Focus();
        }

        if (_isFirstTime)
        {
            TextBox_DescEdit.Select(TextBox_DescEdit.Text.Length, 0);
            _isFirstTime = false;
        }
    }
}
