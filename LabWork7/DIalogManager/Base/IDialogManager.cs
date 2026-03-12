using Avalonia.Controls;

namespace LabWork7.DialogManagers.Base
{
    public interface IDialogManager
    {
        TDialog Create<TDialog>() where TDialog : Window;
    }
}
