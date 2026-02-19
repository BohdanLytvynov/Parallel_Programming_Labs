using Avalonia.Controls;

namespace LabWork5_AvaloniaUI.Managers
{
    public interface IWindowManager
    {
        /// <summary>
        /// Builds window according to TWindow
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <returns></returns>
        Window? Build<TWindow>() where TWindow : Window;
    }
}
