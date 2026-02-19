using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LabWork5_AvaloniaUI.Messages.Calculations
{
    /// <summary>
    /// Send to the progress dialog window
    /// </summary>
    public class CalculationStartedMessage : ValueChangedMessage<string>
    {
        public CalculationStartedMessage(string value) : base(value)
        {
        }
    }
}
