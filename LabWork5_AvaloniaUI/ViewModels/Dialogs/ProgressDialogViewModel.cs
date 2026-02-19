using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LabWork5_AvaloniaUI.Messages.Calculations;
using LabWork5_AvaloniaUI.Messages.Progress;
using LabWork5_AvaloniaUI.Strings;
using System;

namespace LabWork5_AvaloniaUI.ViewModels.Dialogs
{
    public partial class ProgressDialogViewModel : ObservableObject, IDisposable
    {
        public event EventHandler ProgressCanceled;

        [ObservableProperty]
        private double progress = 0;
        [ObservableProperty]
        private string header = string.Empty;
        [ObservableProperty]
        private string title = UIStrings.AppName;

        public ProgressDialogViewModel()
        {
            WeakReferenceMessenger.Default?.Register<CalculationStartedMessage>(this, (o, e) =>
            {
                Header = string.Format("Operation {0} in progress...", e.Value);
            });

            WeakReferenceMessenger.Default?.Register<ProgressChangedMessage>(this, (o, e) =>
            { 
                Progress = e.Value;
            });

            WeakReferenceMessenger.Default?.Register<ProgressCloseMessage>(this, (o, e) =>
            {
                ProgressCanceled?.Invoke(this, EventArgs.Empty);
            });
        }

        public void Dispose()
        {
            WeakReferenceMessenger.Default?.Unregister<ProgressChangedMessage>(this);
            WeakReferenceMessenger.Default?.Unregister<CalculationStartedMessage>(this);
            WeakReferenceMessenger.Default?.Unregister<ProgressCloseMessage>(this);
        }

        private bool CanCancel() => true;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
        { 
            WeakReferenceMessenger.Default?.Send(new CalculationCanceledMessage(true));
            ProgressCanceled?.Invoke(this, EventArgs.Empty);
        }
    }
}
