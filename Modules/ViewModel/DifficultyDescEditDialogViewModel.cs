using CommunityToolkit.Mvvm.ComponentModel;

namespace SoR4_Studio.Modules.ViewModel;

internal partial class DifficultyDescEditDialogViewModel(string difficultyDesc, string selectedDifficultyName) : ObservableObject
{
    [ObservableProperty]
    private string difficultyDesc = difficultyDesc;

    [ObservableProperty]
    private string selectedDifficultyName = selectedDifficultyName;
}
