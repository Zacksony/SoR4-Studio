using System.Windows.Controls;
using System.Windows.Input;

namespace SoR4_Studio.Modules.Windows;

public class MyDataGrid : DataGrid
{
    // 单击触发改为双击
    protected override void OnCanExecuteBeginEdit(CanExecuteRoutedEventArgs e)
    {
        if (e.Parameter is MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.ClickCount <= 1)
            {
                e.CanExecute = false;
                return;
            }
        }

        base.OnCanExecuteBeginEdit(e);
    }

    // 实现列宽自动收缩
    protected override void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
    {
        e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

        base.OnCellEditEnding(e);

        e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
    }
}
