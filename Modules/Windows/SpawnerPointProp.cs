using SoR4_Studio.Modules.DataModel.GameDataModel.BeatThemAll;
using System.Windows;

namespace SoR4_Studio.Modules.Windows;

internal class SpawnerPointProp : DependencyObject
{
    public static LevelData.LevelDataClass.WaveClass.SpawnerClass? GetSpawnerObject(DependencyObject obj)
    {
        return (LevelData.LevelDataClass.WaveClass.SpawnerClass?)obj.GetValue(SpawnerObjectProperty);
    }

    public static void SetSpawnerObject(DependencyObject obj, LevelData.LevelDataClass.WaveClass.SpawnerClass? value)
    {
        obj.SetValue(SpawnerObjectProperty, value);
    }

    public static readonly DependencyProperty? SpawnerObjectProperty =
        DependencyProperty.RegisterAttached(
            "SpawnerObject",
            typeof(LevelData.LevelDataClass.WaveClass.SpawnerClass),
            typeof(SpawnerPointProp),
            new PropertyMetadata(default(LevelData.LevelDataClass.WaveClass.SpawnerClass))
        );
}
