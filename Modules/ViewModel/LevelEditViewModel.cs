using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using SoR4_Studio.Modules.DataModel.GameDataModel.BeatThemAll;
using SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;
using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;
using SoR4_Studio.Modules.Windows.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SoR4_Studio.Modules.ViewModel;

internal partial class LevelEditViewModel : ModdingViewModelBase
{
    private const int VIS_SCALE = 65536;

    public LevelEditViewModel()
    {
        InitialLevelList();
        InitialWaveList();
        InitialPickupList();
        InitialBreakableList();
        InitialEnemyList();
        InitialEnemySpawnerList();
    }

    protected override void OnModChanged()
    {
        InitialLevelList();
        InitialWaveList();
        InitialPickupList();
        InitialBreakableList();
        InitialEnemyList();
        InitialEnemySpawnerList();
    }

    [RelayCommand]
    private static async Task LevelOrderEdit()
    {
        LevelOrderEditDialog dialog = new();

        bool accepted = (bool)(await DialogHost.Show(dialog, "Root") ?? false);
        if (!accepted)
        {
            return;
        }

        ((LevelOrderEditDialogViewModel)dialog.DataContext).Save();
    }

    #region Level & Wave Selection

    [ObservableProperty]
    private SortedDictionary<string, LevelData.LevelDataClass> levelList = [];

    [ObservableProperty]
    private LevelData.LevelDataClass currentLevel;

    [ObservableProperty]
    private List<Tuple<string, LevelData.LevelDataClass.WaveClass>> waveList = [];

    [ObservableProperty]
    private LevelData.LevelDataClass.WaveClass currentWave;

#pragma warning disable MVVMTK0034 

    [MemberNotNull(nameof(currentLevel))]
    private void InitialLevelList()
    {
        SortedDictionary<string, LevelData.LevelDataClass> newLevelList = new(StringComparer.Ordinal);

        foreach (string id in Mod.LevelIDs)
        {
            if (Application.Current.Resources[id] is string displayName)
            {
                newLevelList.Add(displayName, Mod.LevelData[id]);
            }
        }

        LevelList = newLevelList;
        CurrentLevel = newLevelList.First().Value;
    }

    [MemberNotNull(nameof(currentWave))]
    private void InitialWaveList()
    {
        List<Tuple<string, LevelData.LevelDataClass.WaveClass>> newWaveList = [];

        foreach (var wave in CurrentLevel.Waves)
        {
            newWaveList.Add(Tuple.Create(wave.WaveName.Value, wave));
        }

        WaveList = newWaveList;
        CurrentWave = newWaveList[0].Item2;
    }

#pragma warning restore MVVMTK0034

    #endregion

    #region Level Properties

    partial void OnCurrentLevelChanged(LevelData.LevelDataClass value)
    {
        if (value is null)
        {
            return;
        }

        InitialWaveList();
        OnPropertyChanged(nameof(EnableEnemyInfighting));
        OnPropertyChanged(nameof(EnableRetroFilter));
        OnPropertyChanged(nameof(CurrentStageAreas));
    }

    public bool EnableRetroFilter
    {
        get => CurrentLevel.EnableRetroFilter.Value;

        set
        {
            CurrentLevel.EnableRetroFilter.Value = value;
            OnPropertyChanged();
        }
    }

    public bool EnableEnemyInfighting
    {
        get => CurrentLevel.EnableEnemyInfighting.Value;

        set
        {
            CurrentLevel.EnableEnemyInfighting.Value = value;
            OnPropertyChanged();
        }
    }

    public Dictionary<LevelData.LevelDataClass, HashSet<StageAreaViewModel>> LoadedStageAreas { get; private set; } = [];

    public HashSet<StageAreaViewModel> CurrentStageAreas
    {
        get
        {
            if (LoadedStageAreas.TryGetValue(CurrentLevel, out HashSet<StageAreaViewModel>? found))
            {
                return found;
            }

            HashSet<StageAreaViewModel> result = [];

            foreach (List<(int, int)> area in Mod.DecorData[CurrentLevel.DecorID].Areas)
            {
                PointCollection points = [];

                foreach ((int x, int y) in area)
                {
                    points.Add(new((double)x / VIS_SCALE, (double)-y / VIS_SCALE));
                }

                result.Add(new() { Points = points });
            }

            LoadedStageAreas.Add(CurrentLevel, result);

            return result;
        }
    }

    #endregion

    #region Wave Properties

    partial void OnCurrentWaveChanged(LevelData.LevelDataClass.WaveClass value)
    {
        if (value is null)
        {
            return;
        }

        InitialSpawners();
        OnPropertyChanged(nameof(CameraLockType));
        OnPropertyChanged(nameof(MaxEnemiesOnScreen));
        OnPropertyChanged(nameof(IsInfiniteEnemiesOnScreen));
    }

    public int CameraLockType
    {
        get => CurrentWave.CameraLockType.Value;

        set
        {
            CurrentWave.CameraLockType.Value = value;
            OnPropertyChanged();
        }
    }

    public int MaxEnemiesOnScreen
    {
        get => CurrentWave.MaxEnemiesOnScreen.Value;

        set
        {
            CurrentWave.MaxEnemiesOnScreen.Value = value < 0 ? -1 : value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsInfiniteEnemiesOnScreen));
        }
    }

    public bool IsInfiniteEnemiesOnScreen
    {
        get => MaxEnemiesOnScreen < 0;

        set
        {
            MaxEnemiesOnScreen = value ? -1 : 1;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Spawner Properties

    [ObservableProperty]
    private static SortedDictionary<string, string> pickupList = [];

    [ObservableProperty]
    static private SortedDictionary<string, string> breakableList = [];

    [ObservableProperty]
    private static SortedDictionary<string, string> enemyList = [];

    [ObservableProperty]
    private HashSet<SpawnerPointViewModel> currentSpawners = [];

    [ObservableProperty]
    private LevelData.LevelDataClass.WaveClass.SpawnerClass? currentSpawner;

    private void InitialPickupList()
    {
        SortedDictionary<string, string> newList = new(StringComparer.CurrentCulture);

        foreach (string id in Mod.PickupIDs)
        {
            if (Application.Current.Resources[id] is string displayName)
            {
                newList.Add(displayName, id);
            }
        }

        PickupList = newList;
    }

    private void InitialBreakableList()
    {
        SortedDictionary<string, string> newList = new(StringComparer.CurrentCulture);

        foreach (string id in Mod.DestroyableIDs)
        {
            if (Application.Current.Resources[id] is string displayName)
            {
                newList.Add(displayName, id);
            }
        }

        BreakableList = newList;
    }

    private void InitialEnemyList()
    {
        SortedDictionary<string, string> newList = new(StringComparer.Ordinal);

        foreach (string id in Mod.CharacterIDs)
        {
            if (Application.Current.Resources[id] is string displayName)
            {
                newList.Add(displayName, id);
            }
        }

        EnemyList = newList;
    }

    partial void OnCurrentSpawnerChanged(LevelData.LevelDataClass.WaveClass.SpawnerClass? value)
    {
        OnPropertyChanged(nameof(IsPickup));
        OnPropertyChanged(nameof(Pickup));
        OnPropertyChanged(nameof(IsEmptyPickup));

        OnPropertyChanged(nameof(IsBreakable));
        OnPropertyChanged(nameof(Breakable));
        OnPropertyChanged(nameof(InBreakablePickup));
        OnPropertyChanged(nameof(IsEmptyInBreakablePickup));
        OnPropertyChanged(nameof(IsBreakableMirrored));

        OnPropertyChanged(nameof(IsEnemy));
        OnPropertyChanged(nameof(EnemySpwanMultip));
        OnPropertyChanged(nameof(IsGlobalEnemySpawnMultip));
        OnPropertyChanged(nameof(CanSetEnemySpawnMultip));

        InitialEnemySpawnerList();
    }

    public bool IsPickup => GetSpawnerType(CurrentSpawner) == SpawnerType.Pickup;

    public string? Pickup
    {
        get => CurrentSpawner?.OnGroundObjectID?.Value;

        set
        {
            CurrentSpawner!.OnGroundObjectID!.Value = value!;
            OnPropertyChanged();
        }
    }

    public bool? IsEmptyPickup
    {
        get
        {
            return Pickup is null ? null : Pickup == "";
        }

        set
        {
            Pickup = (bool)value! ? "" : PickupList.First().Value;
            OnPropertyChanged();
        }
    }

    public bool IsBreakable => GetSpawnerType(CurrentSpawner) == SpawnerType.Breakable;

    public string? Breakable
    {
        get => CurrentSpawner?.BreakableID?.Value;

        set
        {
            CurrentSpawner!.BreakableID!.Value = value!;
            OnPropertyChanged();
        }
    }

    public string? InBreakablePickup
    {
        get => CurrentSpawner?.InBreakablePickupID?.Value;

        set
        {
            CurrentSpawner!.InBreakablePickupID!.Value = value!;
            OnPropertyChanged();
        }
    }

    public bool? IsEmptyInBreakablePickup
    {
        get
        {
            return InBreakablePickup is null ? null : InBreakablePickup == "";
        }

        set
        {
            InBreakablePickup = (bool)value! ? "" : PickupList.First().Value;
            OnPropertyChanged();
        }
    }
    public bool? IsBreakableMirrored
    {
        get => CurrentSpawner?.IsBreakableMirrored?.Value;

        set
        {
            CurrentSpawner!.IsBreakableMirrored!.Value = (bool)value!;
            OnPropertyChanged();
        }
    }

    public bool IsEnemy => GetSpawnerType(CurrentSpawner) == SpawnerType.Enemy;

    public int? EnemySpwanMultip
    {
        get => CurrentSpawner?.EnemySpwanMultip?.Value;

        set
        {
            CurrentSpawner!.EnemySpwanMultip!.Value = (int)value!;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsGlobalEnemySpawnMultip));
        }
    }

    public bool? IsGlobalEnemySpawnMultip
    {
        get => EnemySpwanMultip is null ? null : EnemySpwanMultip <= 0;

        set
        {
            EnemySpwanMultip = (bool)value! ? 0 : 1;
            OnPropertyChanged();
        }
    }

    public bool CanSetEnemySpawnMultip
    {
        get => CurrentSpawner?.EnemySpwanMultip is not null;
    }

    [ObservableProperty]
    private ObservableCollection<EnemySpawner>? currentEnemySpawnerList;

    private void InitialEnemySpawnerList()
    {
        if (CurrentSpawner is null)
        {
            CurrentEnemySpawnerList = null;
            return;
        }

        if (GetSpawnerType(CurrentSpawner) != SpawnerType.Enemy)
        {
            CurrentEnemySpawnerList = null;
            return;
        }

        ObservableCollection<EnemySpawner> newList = [];

        if (CurrentSpawner.Enemies is not null)
        {
            foreach (var enemyData in CurrentSpawner.Enemies)
            {
                if (enemyData.EnemyID is not null)
                {
                    newList.Add(new()
                    {
                        EnemyID = enemyData.EnemyID,
                        HoldWeaponID = enemyData.HoldWeaponID,
                        HP = enemyData.HP
                    });
                }
            }
        }

        foreach (var enemyID in CurrentSpawner.EnemiesUnnormal)
        {
            newList.Add(new()
            {
                EnemyID = enemyID
            });
        }

        CurrentEnemySpawnerList = newList;
    }

    private void InitialSpawners()
    {
        static void AddSpawner(LevelData.LevelDataClass.WaveClass.SpawnerClass source, HashSet<SpawnerPointViewModel> destination)
        {
            SpawnerType spawnerType = GetSpawnerType(source);

            double x = (double)source.PosX / VIS_SCALE;
            double y = (double)-source.PosY / VIS_SCALE;

            for (int repeatCount = 2; repeatCount > 0; repeatCount--)
            {
                foreach (var addedSpawner in destination)
                {
                    if (double.Abs(addedSpawner.X - x) < 0.3 && double.Abs(addedSpawner.Y - y) < 0.3)
                    {
                        x = addedSpawner.X + ((addedSpawner.X - x) <= 0 ? 0.3 : -0.3);
                        y = addedSpawner.Y + ((addedSpawner.Y - y) <= 0 ? 0.3 : -0.3);
                    }
                }
            }

            destination.Add(new()
            {
                Spawner = source,
                X = x,
                Y = y,
                SpawnerType = spawnerType == SpawnerType.Pickup ? (string)Application.Current.Resources["String.Level.Area.Pickup"]
                : spawnerType == SpawnerType.Breakable ? (string)Application.Current.Resources["String.Level.Area.Breakable"]
                : spawnerType == SpawnerType.Enemy ? (string)Application.Current.Resources["String.Level.Area.Enemy"]
                : "?"
            });
        }

        HashSet<SpawnerPointViewModel> newSpawners = [];

        if (CurrentWave.SpawnerList is null)
        {
            CurrentSpawners = newSpawners;
            CurrentSpawner = null;
            return;
        }

        foreach (var spawnerData in CurrentWave.SpawnerList)
        {
            SpawnerType spawnerType = GetSpawnerType(spawnerData);

            if (spawnerType == SpawnerType.Unknown)
            {
                continue;
            }

            if (spawnerType == SpawnerType.Enemy
                && spawnerData.EnemiesUnnormal.Count == 0
                && spawnerData.Enemies is not null
                && spawnerData.Enemies.All((e) => e.EnemyID is null))
            {
                continue;
            }

            AddSpawner(spawnerData, newSpawners);
        }

        CurrentSpawners = newSpawners;
        CurrentSpawner = null;
    }

    private static SpawnerType GetSpawnerType(LevelData.LevelDataClass.WaveClass.SpawnerClass? spawner)
    {
        if (spawner is null)
        {
            return SpawnerType.Unknown;
        }

        if (spawner.OnGroundObjectID is not null)
        {
            return SpawnerType.Pickup;
        }

        if (spawner.BreakableID is not null)
        {
            return SpawnerType.Breakable;
        }

        if ((spawner.Enemies is not null && spawner.Enemies.Count > 0) || spawner.EnemiesUnnormal.Count > 0)
        {
            return SpawnerType.Enemy;
        }

        return SpawnerType.Unknown;
    }

    #endregion

    [RelayCommand]
    private void RestoreLevel()
    {
        if (MessageBox.Show("Really wanna restore whole current level data to V8?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) is MessageBoxResult.No)
        {
            return;
        }

        string currentLevelID = CurrentLevel.LevelID;
        string currentWaveName = CurrentWave.WaveName;

        CurrentLevel.BaseField.Message.Clear();
        foreach ((int id, ProtoField field) in V8.LevelData[CurrentLevel.LevelID].BaseField.Message)
        {
            CurrentLevel.BaseField.Message.Add((id, field.DeepClone()));
        }

        OnModChanged();

        if (LevelList.TryGetValue(Application.Current.Resources[currentLevelID] as string ?? "", out var foundLevel))
        {
            CurrentLevel = foundLevel;
        }
        else
        {
            return;
        }

        if (WaveList.FirstOrDefault((w) => w.Item1 == currentWaveName)?.Item2 is LevelData.LevelDataClass.WaveClass foundWave)
        {
            CurrentWave = foundWave;
        }
    }

    [RelayCommand]
    private void RestoreWave()
    {
        if (MessageBox.Show("Really wanna restore current wave data to V8?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) is MessageBoxResult.No)
        {
            return;
        }

        string currentLevelID = CurrentLevel.LevelID;
        string currentWaveName = CurrentWave.WaveName;

        CurrentWave.BaseField.Message.Clear();
        foreach ((int id, ProtoField field) in V8.LevelData[CurrentLevel.LevelID].Waves.FirstOrDefault((w) => w.WaveName == currentWaveName)?.BaseField.Message ?? [])
        {
            CurrentWave.BaseField.Message.Add((id, field.DeepClone()));
        }

        OnModChanged();

        if (LevelList.TryGetValue(Application.Current.Resources[currentLevelID] as string ?? "", out var foundLevel))
        {
            CurrentLevel = foundLevel;
        }
        else
        {
            return;
        }

        if (WaveList.FirstOrDefault((w) => w.Item1 == currentWaveName)?.Item2 is LevelData.LevelDataClass.WaveClass foundWave)
        {
            CurrentWave = foundWave;
        }
    }

    internal partial class StageAreaViewModel : ObservableObject
    {
        [ObservableProperty]
        private PointCollection points = [];
    }

    internal partial class SpawnerPointViewModel : ObservableObject
    {
        [ObservableProperty]
        private LevelData.LevelDataClass.WaveClass.SpawnerClass? spawner;

        [ObservableProperty]
        private double x;

        [ObservableProperty]
        private double y;

        [ObservableProperty]
        private string spawnerType = "";
    }

    internal enum SpawnerType
    {
        Unknown,
        Pickup,
        Breakable,
        Enemy
    }

    internal partial class EnemySpawner : ObservableObject
    {
        [ObservableProperty]
        private SoR4_DirectString? enemyID;

        [ObservableProperty]
        private SoR4_DirectString? holdWeaponID;

        [ObservableProperty]
        private SoR4_Int32? hP;

        partial void OnHoldWeaponIDChanged(SoR4_DirectString? value)
        {
            OnPropertyChanged(nameof(NoHoldWeapon));
            OnPropertyChanged(nameof(IsHoldWeaponNotNull));
        }

        partial void OnHPChanged(SoR4_Int32? value)
        {
            OnPropertyChanged(nameof(IsGlobalHP));
            OnPropertyChanged(nameof(IsHPNotNull));
        }

        public bool IsHoldWeaponNotNull => HoldWeaponID is not null;

        public bool IsHPNotNull => HP is not null;

        public bool NoHoldWeapon
        {
            get
            {
                return !Mod.PickupIDs.Contains(HoldWeaponID?.Value ?? "");
            }

            set
            {
                if (HoldWeaponID is null)
                {
                    return;
                }
                HoldWeaponID.Value = value ? "" : "objects/pickup_knife";
                OnPropertyChanged();
            }
        }

        public bool? IsGlobalHP
        {
            get
            {
                return HP is null ? null : HP < 0;
            }

            set
            {
                if (HP is null)
                {
                    return;
                }
                HP.Value = (bool)value! ? -1 : 80;
                OnPropertyChanged();
            }
        }
    }
}
