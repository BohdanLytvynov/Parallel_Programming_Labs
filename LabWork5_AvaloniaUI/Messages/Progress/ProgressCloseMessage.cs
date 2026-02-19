using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LabWork5_AvaloniaUI.Messages.Progress
{
    /// <summary>
    /// Send to Progress Dialog, causes it to close
    /// </summary>
    public class ProgressCloseMessage : ValueChangedMessage<bool>
    {
        public ProgressCloseMessage(bool value) : base(value)
        {
        }
    }
}
