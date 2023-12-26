using CommunityToolkit.Mvvm.ComponentModel;

namespace SoR4_Studio.Modules.ViewModel;

internal partial class NewDifficultyDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private int selectedIndex = 5;
}
