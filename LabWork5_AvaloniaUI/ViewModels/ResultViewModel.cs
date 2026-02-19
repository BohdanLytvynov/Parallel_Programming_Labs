using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LabWork5_AvaloniaUI.Helpers;
using LabWork5_AvaloniaUI.Messages.Calculations;
using LabWork5_AvaloniaUI.Strings;
using System;
using System.Collections.ObjectModel;

namespace LabWork5_AvaloniaUI.ViewModels
{
    public partial class ResultViewModel : ObservableObject, IDisposable
    {
        #region Events
        public event EventHandler OnOkPressed;
        #endregion

        #region Observable Fields
        [ObservableProperty]
        private string title = UIStrings.AppName;

        [ObservableProperty]
        private ObservableCollection<double[]> result = new ObservableCollection<double[]>();

        [ObservableProperty]
        private string header = string.Empty;
        #endregion

        #region Fields
        double [,] m_Mat = null;
        #endregion

        #region Ctor
        public ResultViewModel()
        {
            WeakReferenceMessenger.Default?.Register<CalculationFinishedMessage>(this,async (o, e) =>
            {
                m_Mat = e.Value.Mat;
                Header = string.Format("Operation {0} finished successful. Elapsed time: {1} s",
                    e.Value.Name, e.Value.ElapsedTime);
                if (m_Mat == null) return;
                var r = await UIHelper.MatToMatPresenterAsync(m_Mat);

                Dispatcher.UIThread.Post(() =>
                {
                    Result = new ObservableCollection<double[]>(r);
                });
            });
        }

        public void Dispose()
        {
            WeakReferenceMessenger.Default?.Unregister<CalculationFinishedMessage>(this);
        }
        #endregion

        #region Methods
        private bool CanOkPressed() => true;

        [RelayCommand (CanExecute = nameof(CanOkPressed))]
        private void OkPressed()
        { 
            OnOkPressed?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
