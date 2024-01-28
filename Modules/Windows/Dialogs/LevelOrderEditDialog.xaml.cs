using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoR4_Studio.Modules.Windows.Dialogs;

/// <summary>
/// LevelOrderEdit.xaml 的交互逻辑
/// </summary>
public partial class LevelOrderEditDialog : UserControl
{
    public LevelOrderEditDialog()
    {
        InitializeComponent();

        ItemsControl_Outer.MaxWidth = MainWindow.Instance!.Width * 0.8;
        ScrollViewer_Outer.MaxHeight = MainWindow.Instance!.Height * 0.8;
    }
}
