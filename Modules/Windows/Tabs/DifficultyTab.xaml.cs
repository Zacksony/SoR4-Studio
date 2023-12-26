using SoR4_Studio.Modules.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace SoR4_Studio.Modules.Windows.Tabs;

/// <summary>
/// DifficultyTab.xaml 的交互逻辑
/// </summary>
public partial class DifficultyTab : UserControl
{
    public DifficultyTab()
    {
        InitializeComponent();
    }

    private int? GetSelectedRow()
    {
        if (DataGrid_Difficulty.SelectedCells.Count == 0)
        {
            return null;
        }

        return DataGrid_Difficulty.Items.IndexOf(DataGrid_Difficulty.SelectedCells[0].Item);
    }

    private void Button_DifficultyList_Delete_Click(object sender, RoutedEventArgs e)
    {
        if (GetSelectedRow() is not int row)
        {
            return;
        }

        ((DifficultyListViewModel)DataContext).DeleteCommand.Execute(row);
    }

    private void Button_DifficultyList_Up_Click(object sender, RoutedEventArgs e)
    {
        if (GetSelectedRow() is not int row)
        {
            return;
        }

        if (row == 0) // 最顶部
        {
            return;
        }

        ((DifficultyListViewModel)DataContext).UpCommand.Execute(row);
    }

    private void Button_DifficultyList_Down_Click(object sender, RoutedEventArgs e)
    {
        if (GetSelectedRow() is not int row)
        {
            return;
        }

        if (row == DataGrid_Difficulty.Items.Count - 1) // 最底部
        {
            return;
        }

        ((DifficultyListViewModel)DataContext).DownCommand.Execute(row);
    }

    private void Button_DifficultyList_DescEdit_Click(object sender, RoutedEventArgs e)
    {
        if (GetSelectedRow() is not int row)
        {
            return;
        }

        ((DifficultyListViewModel)DataContext).DescEditCommand.Execute(row);
    }
}
