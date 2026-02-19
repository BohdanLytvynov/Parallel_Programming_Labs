using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LabWork5_AvaloniaUI.Messages.Dialogs;
using LabWork5_AvaloniaUI.Strings;
using System;
using Avalonia.Controls;
using Avalonia;

namespace LabWork5_AvaloniaUI.ViewModels.Dialogs
{
    public partial class DialogViewModel : ObservableObject, IDisposable
    {
        public event EventHandler OnOkPressed;//Event, will trigger the closing of the dialog in App.axaml.cs

        public string Title { get; set; } = UIStrings.AppName;

        [ObservableProperty]
        private string header;

        [ObservableProperty]
        private string message;

        [ObservableProperty]
        private MessageType messageType = MessageType.Info;

        [ObservableProperty]  
        private Brush headerForeground;//Foreground for Header

        private Color[] m_headerColors;//List of available color

        public DialogViewModel()
        {
            header = string.Empty;
            message = string.Empty;

            m_headerColors = new Color[3];

            var app = Application.Current;
            //Get al colors that we store in Colors.axaml
            m_headerColors[0] = TryGetValue<Color>("InformationColor", app);
            m_headerColors[1] = TryGetValue<Color>("WarningColor", app);
            m_headerColors[2] = TryGetValue<Color>("ErrorColor", app);
            //register the handler to react when DialogMessage was sent.
            WeakReferenceMessenger.Default.Register<DialogMessage>(this, (o, m) =>
            {
                switch (m.Value.MessageType)
                {
                    case MessageType.Info:
                        Header = UIStrings.InfoDialogHeader;
                        HeaderForeground = new SolidColorBrush(m_headerColors[0]);
                        break;
                    case MessageType.Warning:
                        Header = UIStrings.WarningDialogHeader;
                        HeaderForeground = new SolidColorBrush(m_headerColors[1]);
                        break;
                    case MessageType.Error:
                        Header = UIStrings.ErrorDialogHeader;
                        HeaderForeground = new SolidColorBrush(m_headerColors[2]);
                        break;
                }

                Message = m.Value.Message;
            });
        }

        public void Dispose()
        {
            //Don't forget to clean up
            WeakReferenceMessenger.Default.Unregister<DialogMessage>(this);
        }

        private bool CanOk() => true;

        [RelayCommand(CanExecute = nameof(CanOk))]
        private void Ok()
        { 
            OnOkPressed?.Invoke(this, EventArgs.Empty);
        }

        private TType TryGetValue<TType>(object key, Application app)
        {
            object obj = null;
            app.TryGetResource(key, out obj);
            return (TType)obj;
        }
    }
}
