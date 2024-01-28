using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoR4_Studio.Modules.DataModel.GameDataModel;
using SoR4_Studio.Modules.DataModel.GameDataModel.BeatThemAll;
using SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static SoR4_Studio.Modules.ViewModel.LevelOrderEditDialogViewModel.StageLevelOrderViewModel;

namespace SoR4_Studio.Modules.ViewModel;

internal partial class LevelOrderEditDialogViewModel : ModdingViewModelBase
{
    public LevelOrderEditDialogViewModel()
    {
        InitialLevelOrders(Mod);
    }

    private void InitialLevelOrders(GameData original)
    {
        List<StageLevelOrderViewModel> levelOrdersBaseList = [];

        foreach (MetaGameConfigData.LevelOrderClass levelOrderData in original.MetaGameConfigData.MainCampaignLevelOrders)
        {
            List<string> levelOrderStrings = [];
            foreach (var levelIDData in levelOrderData.LevelIDs)
            {
                levelOrderStrings.Add(levelIDData.LevelID);
            }

            levelOrdersBaseList.Add(new(levelOrderStrings, LevelOrders));
        }

        LevelOrders.SetBaseList(levelOrdersBaseList);
    }

    [ObservableProperty]
    private ObservableList<StageLevelOrderViewModel> levelOrders = [];

    [RelayCommand]
    private void AddStage()
    {
        LevelOrders.Add(new([], LevelOrders));
    }

    [RelayCommand]
    private void Reset()
    {
        InitialLevelOrders(V8);
    }
    
    public void Save()
    {
        ExtenderList<MetaGameConfigData.LevelOrderClass> v8LevelOrdersData = V8.MetaGameConfigData.MainCampaignLevelOrders;

        int v8StageCount = v8LevelOrdersData.Count;

        int editingStageCount = LevelOrders.Count;

        MetaGameConfigData.LevelOrderClass.LevelIDClass GenLevelIDData(string levelID)
        {
            MetaGameConfigData.LevelOrderClass.LevelIDClass dataClone = v8LevelOrdersData[0].LevelIDs[0].DeepClone<MetaGameConfigData.LevelOrderClass.LevelIDClass>();
            dataClone.LevelID.Value = levelID;
            return dataClone;
        } 

        Mod.MetaGameConfigData.MainCampaignLevelOrders.Clear();

        for (int iStage = 0; iStage < editingStageCount; iStage++)
        {
            MetaGameConfigData.LevelOrderClass levelOrderData;

            if (iStage < v8StageCount)
            {
                levelOrderData = v8LevelOrdersData[iStage].DeepClone<MetaGameConfigData.LevelOrderClass>();
            }
            else // i >= v8StageCount
            { 
                levelOrderData = v8LevelOrdersData[0].DeepClone<MetaGameConfigData.LevelOrderClass>();
            }

            levelOrderData.LevelIDs.Clear();
            
            foreach (LevelSelectionViewModel level in LevelOrders[iStage].LevelOrder.Cast<LevelSelectionViewModel>())
            {
                levelOrderData.LevelIDs.Add(GenLevelIDData(level.LevelID));
            }

            Mod.MetaGameConfigData.MainCampaignLevelOrders.Add(levelOrderData);
        }
    }

    internal partial class StageLevelOrderViewModel : ObservableObject
    {
        public StageLevelOrderViewModel(List<string> levelOrderStrings, ObservableList<StageLevelOrderViewModel> parent)
        {
            List<LevelSelectionViewModel> levelOrderBaseList = [];

            foreach (string levelID in levelOrderStrings)
            {
                levelOrderBaseList.Add(new(levelID, levelOrder));
            }

            levelOrder.SetBaseList(levelOrderBaseList);

            this.parent = parent;
        }

        private ObservableList<StageLevelOrderViewModel> parent;

        public int StageNumber => parent.IndexOf(this) + 1;

        [ObservableProperty]
        private ObservableList<LevelSelectionViewModel> levelOrder = [];

        [RelayCommand]
        private void AddLevel()
        {
            LevelOrder.Add(new(LevelSelectionViewModel.AllLevelList.Values.FirstOrDefault() ?? string.Empty, LevelOrder));
        }

        [RelayCommand]
        private void Delete()
        {
            parent.Remove(this);
        }

        [RelayCommand]
        private void Up()
        {
            int thisIndex = parent.IndexOf(this);
            parent.Swap(thisIndex, thisIndex - 1);
        }

        [RelayCommand]
        private void Down()
        {
            int thisIndex = parent.IndexOf(this);
            parent.Swap(thisIndex, thisIndex + 1);
        }

        internal partial class LevelSelectionViewModel(string levelID, ObservableList<LevelSelectionViewModel> parent) : ObservableObject
        {
            private ObservableList<LevelSelectionViewModel> parent = parent;

            [ObservableProperty]
            private string levelID = levelID;

            [RelayCommand]
            private void Delete()
            {
                parent.Remove(this);
            }

            [RelayCommand]
            private void Up()
            {
                int thisIndex = parent.IndexOf(this);
                parent.Swap(thisIndex, thisIndex - 1);
            }

            [RelayCommand]
            private void Down()
            {
                int thisIndex = parent.IndexOf(this);
                parent.Swap(thisIndex, thisIndex + 1);
            }

            private static SortedDictionary<string, string>? allLevelList;

            public static SortedDictionary<string, string> AllLevelList => allLevelList ??= InitialLevelList();

            private static SortedDictionary<string, string> InitialLevelList()
            {
                SortedDictionary<string, string> newLevelList = new(StringComparer.Ordinal);

                foreach (string id in V8.LevelIDs)
                {
                    if (Application.Current.Resources[id] is string displayName)
                    {
                        newLevelList.Add(displayName, V8.LevelData[id].LevelID);
                    }
                }

                return allLevelList = newLevelList;
            }
        }
    }
}
