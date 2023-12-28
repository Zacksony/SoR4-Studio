using SoR4_Studio.Modules.DataModel.GameDataModel.BeatThemAll;
using SoR4_Studio.Modules.ViewModel;
using SoR4_Studio.Modules.Windows.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SoR4_Studio.Modules.Windows.Tabs;

/// <summary>
/// LevelTab.xaml 的交互逻辑
/// </summary>
public partial class LevelTab : UserControl
{
    private const int INITIAL_DISPLAY_SCALE = 12;
    private const int MAX_DISPLAY_SCALE = INITIAL_DISPLAY_SCALE * 4;
    private const int MIN_DISPLAY_SCALE = (int)(INITIAL_DISPLAY_SCALE * 0.2);

    public LevelTab()
    {
        InitializeComponent();
    }

    private Point MiddlePos => new(StageAreaCanvas.ActualWidth / 2, StageAreaCanvas.ActualHeight / 2);

    private ItemsControl? StageArea => (ItemsControl?)StageAreaCanvas?.Children[0];

    private TransformGroup StageAreaTransformGroup => (TransformGroup)StageArea!.RenderTransform;

    private TranslateTransform StageAreaTranslateTransform => (TranslateTransform)StageAreaTransformGroup.Children[0];

    private ScaleTransform StageAreaScaleTransform => (ScaleTransform)StageAreaTransformGroup.Children[1];

    private ItemsControl? SpawnersView => (ItemsControl?)StageAreaCanvas?.Children[1];

    private TransformGroup SpawnersViewTransformGroup => (TransformGroup)SpawnersView!.RenderTransform;

    private TranslateTransform SpawnersViewTranslateTransform => (TranslateTransform)SpawnersViewTransformGroup.Children[0];

    private ScaleTransform SpawnersViewScaleTransform => (ScaleTransform)SpawnersViewTransformGroup.Children[1];

    private Point previousMousePosition = new(0, 0);

    private void StageAreaCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!isMouseOnSpawnPoint)
        {
            Cursor = Cursors.ScrollAll;
        }

        previousMousePosition = e.GetPosition(this);
    }

    private void StageAreaCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!isMouseOnSpawnPoint)
        {
            Cursor = Cursors.Arrow;
        }
    }

    private void StageAreaCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (!(e.LeftButton == MouseButtonState.Pressed))
        {
            return;
        }

        Vector movedDistance = e.GetPosition(this) - previousMousePosition;

        StageAreaTranslateTransform.X += movedDistance.X / StageAreaScaleTransform.ScaleX;
        StageAreaTranslateTransform.Y += movedDistance.Y / StageAreaScaleTransform.ScaleY;

        SpawnersViewTranslateTransform.X += movedDistance.X / SpawnersViewScaleTransform.ScaleX;
        SpawnersViewTranslateTransform.Y += movedDistance.Y / SpawnersViewScaleTransform.ScaleY;

        previousMousePosition = e.GetPosition(this);
    }

    private void StageAreaCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        int delta = e.Delta;

        if (delta < 0 && (StageAreaScaleTransform.ScaleX <= MIN_DISPLAY_SCALE || StageAreaScaleTransform.ScaleY <= MIN_DISPLAY_SCALE))
        {
            return;
        }

        if (delta > 0 && (StageAreaScaleTransform.ScaleX >= MAX_DISPLAY_SCALE || StageAreaScaleTransform.ScaleY >= MAX_DISPLAY_SCALE))
        {
            return;
        }

        AnimatedAreaScale(StageAreaScaleTransform.ScaleX, StageAreaScaleTransform.ScaleX * (delta > 0 ? 1.66 : 0.602));
    }

    private void StageAreaCanvas_MouseLeave(object sender, MouseEventArgs e)
    {
        Cursor = Cursors.Arrow;
    }

    private struct AreaTransformInfo
    {
        public double translateX;
        public double translateY;
        public double scaleX;
        public double scaleY;
        public double scaleCenterX;
        public double scaleCenterY;
    }

    private readonly Dictionary<LevelData.LevelDataClass, AreaTransformInfo> AreaTransformMemory = [];

    private void SaveLevelTransform(LevelData.LevelDataClass level)
    {
        AreaTransformMemory.Remove(level);
        AreaTransformMemory.Add(level, new()
        {
            translateX = StageAreaTranslateTransform.X,
            translateY = StageAreaTranslateTransform.Y,
            scaleX = StageAreaScaleTransform.ScaleX,
            scaleY = StageAreaScaleTransform.ScaleY,
            scaleCenterX = StageAreaScaleTransform.CenterX,
            scaleCenterY = StageAreaScaleTransform.CenterY,
        });
    }

    private void RestoreLevelTransform(LevelData.LevelDataClass level)
    {
        if (AreaTransformMemory.TryGetValue(level, out AreaTransformInfo transFormInfo))
        {
            // 选过，还原缩放信息

            StageAreaTranslateTransform.X = transFormInfo.translateX;
            StageAreaTranslateTransform.Y = transFormInfo.translateY;

            StageAreaScaleTransform.CenterX = transFormInfo.scaleCenterX;
            StageAreaScaleTransform.CenterY = transFormInfo.scaleCenterY;

            SpawnersViewTranslateTransform.X = transFormInfo.translateX;
            SpawnersViewTranslateTransform.Y = transFormInfo.translateY;

            SpawnersViewScaleTransform.CenterX = transFormInfo.scaleCenterX;
            SpawnersViewScaleTransform.CenterY = transFormInfo.scaleCenterY;

            AnimatedAreaScale(transFormInfo.scaleX * 1.2, transFormInfo.scaleX);

            return;
        }

        // 第一次选择, 初始化

        InitialAreaTransForm();
    }

    private void StageAreaCanvas_Unloaded(object sender, RoutedEventArgs e)
    {
        if (ComboBox_Level.SelectedItem is not KeyValuePair<string, LevelData.LevelDataClass> selectedItem)
        {
            return;
        }

        SaveLevelTransform(selectedItem.Value);
    }

    private void StageAreaCanvas_Loaded(object sender, RoutedEventArgs e)
    {
        if (ComboBox_Level.SelectedItem is not KeyValuePair<string, LevelData.LevelDataClass> selectedItem)
        {
            return;
        }

        RestoreLevelTransform(selectedItem.Value);
    }

    private void StageAreaCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        double changedX = (e.NewSize.Width - e.PreviousSize.Width);
        double changedY = (e.NewSize.Height - e.PreviousSize.Height);

        StageAreaScaleTransform.CenterX += changedX / 2;
        StageAreaScaleTransform.CenterY += changedY / 2;

        SpawnersViewScaleTransform.CenterX += changedX / 2;
        SpawnersViewScaleTransform.CenterY += changedY / 2;

        StageAreaTranslateTransform.X += changedX / 2 - changedX / 2 / StageAreaScaleTransform.ScaleX;
        StageAreaTranslateTransform.Y += changedY / 2 - changedY / 2 / StageAreaScaleTransform.ScaleY;

        SpawnersViewTranslateTransform.X += changedX / 2 - changedX / 2 / SpawnersViewScaleTransform.ScaleX;
        SpawnersViewTranslateTransform.Y += changedY / 2 - changedY / 2 / SpawnersViewScaleTransform.ScaleY;
    }

    private void LevelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // 保存当前的

        if (e.RemovedItems.Count == 0)
        {
            return;
        }

        SaveLevelTransform(((KeyValuePair<string, LevelData.LevelDataClass>)e.RemovedItems[0]!).Value);

        // 加载新的

        if (ComboBox_Level.SelectedItem is not KeyValuePair<string, LevelData.LevelDataClass> selectedItem)
        {
            return;
        }

        RestoreLevelTransform(selectedItem.Value);
    }

    private void Button_Restore_Click(object sender, RoutedEventArgs e)
    {
        InitialAreaTransForm();
    }

    private (Point minPoint, Point maxPoint) GetAreaBoard()
    {
        double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;

        foreach (var point in from StageAreaViewModel polygon in ((LevelEditViewModel)DataContext).CurrentStageAreas
                              from Point point in polygon.Points
                              select point)
        {
            minX = Math.Min(minX, point.X);
            minY = Math.Min(minY, point.Y);
            maxX = Math.Max(maxX, point.X);
            maxY = Math.Max(maxY, point.Y);
        }

        return (new(minX, minY), new(maxX, maxY));
    }

    private void InitialAreaTransForm()
    {
        (Point minPoint, Point maxPoint) = GetAreaBoard();

        double width = maxPoint.X - minPoint.X;
        double height = maxPoint.Y - minPoint.Y;

        StageAreaTranslateTransform.X = MiddlePos.X - width / 2 - minPoint.X;
        StageAreaTranslateTransform.Y = MiddlePos.Y - height / 2 - minPoint.Y;

        SpawnersViewTranslateTransform.X = MiddlePos.X - width / 2 - minPoint.X;
        SpawnersViewTranslateTransform.Y = MiddlePos.Y - height / 2 - minPoint.Y;

        StageAreaScaleTransform.CenterX = MiddlePos.X;
        StageAreaScaleTransform.CenterY = MiddlePos.Y;

        SpawnersViewScaleTransform.CenterX = MiddlePos.X;
        SpawnersViewScaleTransform.CenterY = MiddlePos.Y;

        AnimatedAreaScale(INITIAL_DISPLAY_SCALE * 1.5, INITIAL_DISPLAY_SCALE);
    }

    private void AnimatedAreaScale(double from, double to)
    {
        DoubleAnimation anime = new()
        {
            From = from,
            To = to,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        Storyboard.SetTarget(anime, StageArea);
        StageAreaScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anime);
        StageAreaScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anime);
        Storyboard.SetTarget(anime, SpawnersView);
        SpawnersViewScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anime);
        SpawnersViewScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anime);
    }

    private void ComboBox_Level_MouseEnter(object sender, MouseEventArgs e)
    {
        (sender as UIElement)!.Focus();
        Cursor = Cursors.ScrollNS;
    }

    private void ComboBox_Wave_MouseEnter(object sender, MouseEventArgs e)
    {
        (sender as UIElement)!.Focus();
        Cursor = Cursors.ScrollNS;
    }

    private void ComboBox_Level_MouseLeave(object sender, MouseEventArgs e)
    {
        Cursor = Cursors.Arrow;
    }

    private void ComboBox_Wave_MouseLeave(object sender, MouseEventArgs e)
    {
        Cursor = Cursors.Arrow;
    }

    private bool isMouseOnSpawnPoint = false;

    private Border? currentSpawnerPoint;

    private void SpawnPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Border spawnPoint)
        {
            return;
        }

        if (currentSpawnerPoint is not null)
        {
            currentSpawnerPoint.BorderBrush = Brushes.Red;
        }

        ((LevelEditViewModel)DataContext).CurrentSpawner = SpawnerPointProp.GetSpawnerObject(spawnPoint);

        spawnPoint.BorderBrush = Brushes.Blue;
        currentSpawnerPoint = spawnPoint;
    }

    private void SpawnPoint_MouseEnter(object sender, MouseEventArgs e)
    {
        isMouseOnSpawnPoint = true;
        Cursor = Cursors.Hand;

        if (sender is not Border spawnPoint)
        {
            return;
        }

        DoubleAnimation animeScale = new()
        {
            From = 1,
            To = 1.15,
            Duration = TimeSpan.FromMilliseconds(100),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        DoubleAnimation animeTrans = new()
        {
            From = -0.5,
            To = -0.575,
            Duration = TimeSpan.FromMilliseconds(100),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        Storyboard.SetTarget(animeScale, spawnPoint);
        spawnPoint.BeginAnimation(WidthProperty, animeScale);
        spawnPoint.BeginAnimation(HeightProperty, animeScale);

        Storyboard.SetTarget(animeTrans, spawnPoint);
        spawnPoint.BeginAnimation(Canvas.TopProperty, animeTrans);
        spawnPoint.BeginAnimation(Canvas.LeftProperty, animeTrans);
    }

    private void SpawnPoint_MouseLeave(object sender, MouseEventArgs e)
    {
        isMouseOnSpawnPoint = false;
        Cursor = Cursors.Arrow;

        if (sender is not Border spawnPoint)
        {
            return;
        }

        DoubleAnimation animeScale = new()
        {
            From = 1.15,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(100),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        DoubleAnimation animeTrans = new()
        {
            From = -0.575,
            To = -0.5,
            Duration = TimeSpan.FromMilliseconds(100),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        Storyboard.SetTarget(animeScale, spawnPoint);
        spawnPoint.BeginAnimation(WidthProperty, animeScale);
        spawnPoint.BeginAnimation(HeightProperty, animeScale);

        Storyboard.SetTarget(animeTrans, spawnPoint);
        spawnPoint.BeginAnimation(Canvas.TopProperty, animeTrans);
        spawnPoint.BeginAnimation(Canvas.LeftProperty, animeTrans);
    }
}
