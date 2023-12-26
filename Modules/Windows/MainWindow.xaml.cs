using Octokit;
using SoR4_Studio.Modules.ViewModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace SoR4_Studio.Modules.Windows;

// TODO: 一些交互逻辑可以优化，通过xaml里使用触发器

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
        MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 8;

        CheckUpdate();
    }

    #region Window Interactions

    private bool is_drag_moving = false;

    private void Border_Top_MouseLeftButtonDown(object sender, MouseButtonEventArgs? e)
    {
        is_drag_moving = true;

        if (WindowState == WindowState.Normal)
        {
            DragMove();
        }
    }

    private void Border_Top_MouseMove(object sender, MouseEventArgs e)
    {
        if (is_drag_moving && WindowState == WindowState.Maximized)
        {
            Top = 0;
            WindowState = WindowState.Normal;

            Left = Mouse.GetPosition(this).X - Width / 2d;

            DragMove();
        }
    }

    private void Border_Top_MouseLeave(object sender, MouseEventArgs e)
    {
        is_drag_moving = false;
    }

    private void Border_Top_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        is_drag_moving = false;
    }

    private void Button_Min_Click(object sender, RoutedEventArgs e)
    {
        var story = (Storyboard)Resources["MinimizeWindow"];

        Play_Anime(story, delegate { WindowState = WindowState.Minimized; });

    }

    private void Button_Max_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
        }
        else if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
    }

    private void Button_Close_Click(object sender, RoutedEventArgs e)
    {
        MainWindowViewModel.SaveCanceled = false;
        var result = MessageBox.Show("Save before exit?", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Cancel)
        {
            return;
        }
        if (result == MessageBoxResult.Yes)
        {
            ((MainWindowViewModel)DataContext).SaveCommand.Execute(this);
        }
        if (MainWindowViewModel.SaveCanceled)
        {
            MainWindowViewModel.SaveCanceled = false;
            return;
        }

        var story = (Storyboard)Resources["CloseWindow"];

        Play_Anime(story, delegate { Close(); });
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            return;
        }

        if (WindowState == WindowState.Normal)
        {
            Button_Max.Content = FindResource("Image.ControlMax");

            WindowShadowBorder.Margin = new Thickness(16);

            Button_Max.ToolTip = FindResource("String.Title.Button.Maximize");
        }

        if (WindowState == WindowState.Maximized)
        {
            Button_Max.Content = FindResource("Image.ControlRestore");

            WindowShadowBorder.Margin = new Thickness(0);

            Button_Max.ToolTip = FindResource("String.Title.Button.Restore");
        }

        var story = (Storyboard)Resources["ShowWindow"];

        Play_Anime(story, null);
    }

    private void Window_Activated(object sender, EventArgs e)
    {
        Border_Top.Background = new SolidColorBrush(new Color() { A = 0xFF, R = 0x14, G = 0x14, B = 0x17 });
        Border_ToolBar.Background = new SolidColorBrush(new Color() { A = 0xFF, R = 0x14, G = 0x14, B = 0x17 });
        WindowShadowBorder.Background = new SolidColorBrush(new Color() { A = 0xFF, R = 0x17, G = 0x17, B = 0x1A });
        WindowShadowBorder.BorderBrush = new SolidColorBrush(new Color() { A = 0xFF, R = 0x55, G = 0x55, B = 0x60 });
        WindowShadowBorder.Effect = new DropShadowEffect() { BlurRadius = 16, ShadowDepth = 0, Opacity = 0.8, Color = new Color() { A = 0xFF, R = 0x00, G = 0x00, B = 0x00 } };
        Label_MainTitle.Opacity = 1;
        Label_Version.Opacity = 1;
        Button_Min.Opacity = 1;
        Button_Max.Opacity = 1;
        Button_Close.Opacity = 1;
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
        Border_Top.Background = new SolidColorBrush(new Color() { A = 0xF0, R = 0x1D, G = 0x1D, B = 0x1F });
        Border_ToolBar.Background = new SolidColorBrush(new Color() { A = 0xF0, R = 0x1D, G = 0x1D, B = 0x1F });
        WindowShadowBorder.Background = new SolidColorBrush(new Color() { A = 0xF0, R = 0x1F, G = 0x1F, B = 0x21 });
        WindowShadowBorder.BorderBrush = new SolidColorBrush(new Color() { A = 0x8F, R = 0x4F, G = 0x4E, B = 0x50 });
        WindowShadowBorder.Effect = new DropShadowEffect() { BlurRadius = 0, ShadowDepth = 0, Opacity = 0.8, Color = new Color() { A = 0xFF, R = 0x00, G = 0x00, B = 0x00 } };
        Label_MainTitle.Opacity = 0.5;
        Label_Version.Opacity = 0.5;
        Button_Min.Opacity = 0.5;
        Button_Max.Opacity = 0.5;
        Button_Close.Opacity = 0.5;
    }

    private void Play_Anime(Storyboard? story, EventHandler? eventHandler)
    {
        if (story != null)
        {
            if (eventHandler != null)
            {
                story.Completed += eventHandler;
            }
            story.Begin(this);
        }
    }

    #endregion

    #region Update Checking

    private async void CheckUpdate()
    {
        Label_UpdateFound.Visibility = Visibility.Hidden;

        try
        {
            //Get all releases from GitHub
            //Source: https://octokitnet.readthedocs.io/en/latest/getting-started/
            GitHubClient client = new(new ProductHeaderValue("Zacksony"));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("Zacksony", "SoR4-Studio");

            //Setup the versions
            Version latestGitHubVersion = new(PickoutVersionString(releases[0].TagName));
            Version localVersion = new(CurrentVersion.Instance.DisplayVersion);

            //Compare the Versions
            //Source: https://stackoverflow.com/questions/7568147/compare-version-numbers-without-using-split-function
            int versionComparison = localVersion.CompareTo(latestGitHubVersion);
            if (versionComparison < 0)
            {
                //The version on GitHub is more up to date than this local release.
                Console.WriteLine("Go update now!!!");
                Label_UpdateFound.Visibility = Visibility.Visible;
            }
            else if (versionComparison > 0)
            {
                //This local version is greater than the release version on GitHub.
                Console.WriteLine("..how?");
            }
            else
            {
                //This local Version and the Version on GitHub are equal.
                Console.WriteLine("Good boy");
            }
        }
        catch
        {
            Console.WriteLine("I DONOT really care why the fuck it failed but we need to keep running anyway");
        }
    }

    private static string PickoutVersionString(in string rawString)
    {
        string pattern = @"\d+\.\d+\.\d+";
        Regex regex = new(pattern);
        return regex.Match(rawString).Value;
    }

    #endregion
}
