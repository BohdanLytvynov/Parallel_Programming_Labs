using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LabWork5_AvaloniaUI.Messages.Calculations
{
    /// <summary>
    /// Message Send from Progress dialog to the ViewModel
    /// </summary>
    public class CalculationCanceledMessage : ValueChangedMessage<bool>
    {
        public CalculationCanceledMessage(bool value) : base(value)
        {
        }
    }
}
