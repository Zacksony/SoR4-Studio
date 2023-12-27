using System.Windows.Data;

namespace SoR4_Studio.Modules.Windows.Helpers;

public class EditableBinding : Binding
{
    public EditableBinding()
    {
        Initialize();
    }
    public EditableBinding(string path) : base(path)
    {
        Initialize();
    }

    private void Initialize()
    {
        Mode = BindingMode.TwoWay;
        UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
    }
}
