using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LabWork5_AvaloniaUI.Messages.Dialogs
{
    /// <summary>
    /// Type of Information Dialog
    /// </summary>
    public enum MessageType
    { 
        Info,
        Warning,
        Error,
    }

    public struct DialogMessageContent
    {
        /// <summary>
        /// Message to show
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// type of the Message
        /// </summary>
        public MessageType MessageType { get; set; }
    }

    /// <summary>
    /// Send in case of error warning information
    /// </summary>
    public class DialogMessage : ValueChangedMessage<DialogMessageContent>
    {
        public DialogMessage(DialogMessageContent value) : base(value)
        {
        }
    }
}
