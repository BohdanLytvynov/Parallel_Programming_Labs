using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LabWork5_AvaloniaUI.Messages.Progress
{
    /// <summary>
    /// Send to the Progress Dialog when progress of operation was changed
    /// </summary>
    public class ProgressChangedMessage : ValueChangedMessage<double>
    {
        public ProgressChangedMessage(double value) : base(value)
        {
        }
    }
}
