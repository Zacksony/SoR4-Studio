using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using SoR4_Studio.Modules.DataModel.GameDataModel.BeatThemAll;
using SoR4_Studio.Modules.Windows.Tabs;
using System;
using System.Threading.Tasks;

namespace SoR4_Studio.Modules.ViewModel;

internal partial class DifficultyListViewModel : ModdingViewModelBase
{
    public DifficultyListViewModel()
    {
        difficultyList = new(Mod.MetaGameConfigData.Difficulties);
        isNotEmpty = difficultyList.Count > 0;

        difficultyList.CollectionChanged += OnDifficultyListChanged;
    }

    #region DifficultyList

    [ObservableProperty]
    private ObservableList<MetaGameConfigData.DifficultyClass> difficultyList;

    [ObservableProperty]
    private bool isNotEmpty;

    private void OnDifficultyListChanged(object? sender, EventArgs? e)
    {
        IsNotEmpty = DifficultyList.Count > 0;
    }

    protected override void OnModChanged()
    {
        DifficultyList.New(Mod.MetaGameConfigData.Difficulties);
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void Clear()
    {
        DifficultyList.Clear();
    }

    [RelayCommand]
    private async Task Add()
    {
        NewDifficultyDialog dialog = new();

        bool accepted = (bool)(await DialogHost.Show(dialog, "Root") ?? false);
        if (!accepted)
        {
            return;
        }

        int result = ((NewDifficultyDialogViewModel)dialog.DataContext).SelectedIndex;

        DifficultyList.Add(V8.MetaGameConfigData.Difficulties[result].DeepClone<MetaGameConfigData.DifficultyClass>());
    }

    [RelayCommand]
    private void Reverse()
    {
        DifficultyList.Reverse();
    }

    [RelayCommand]
    private void Delete(int index)
    {
        DifficultyList.RemoveAt(index);
    }

    [RelayCommand]
    private void Up(int index)
    {
        DifficultyList.Swap(index, index - 1);
    }

    [RelayCommand]
    private void Down(int index)
    {
        DifficultyList.Swap(index, index + 1);
    }

    [RelayCommand]
    private async Task DescEdit(int index)
    {
        string selectedDifficultyName = DifficultyList[index].DifficultyName;
        string oldDesc = DifficultyList[index].DifficultyDescription;

        DifficultyDescEditDialog dialog = new(oldDesc, selectedDifficultyName);

        bool accepted = (bool)(await DialogHost.Show(dialog, "Root") ?? false);

        if (!accepted)
        {
            return;
        }

        string result = ((DifficultyDescEditDialogViewModel)dialog.DataContext).DifficultyDesc;
        DifficultyList[index].DifficultyDescription = result.Replace("\r", null);
    }

    #endregion
}
