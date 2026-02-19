using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LabWork5_AvaloniaUI.Messages.Calculations
{
    /// <summary>
    /// Details about the operation
    /// </summary>
    public class OperationDetails
    {
        /// <summary>
        /// Operation Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Result of the operation
        /// </summary>
        public double[,] Mat { get; set; }
        /// <summary>
        /// Time Elapsed
        /// </summary>
        public double ElapsedTime { get; set; }

        public OperationDetails(string name, double[,] mat, double elapsedtime)
        {
            ElapsedTime = elapsedtime;
            Name = name;
            Mat = mat;
        }

        public OperationDetails() : this(string.Empty, null, 0)
        {
            
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Send from ViewModel to the Result Window Dialog
    /// </summary>
    public class CalculationFinishedMessage : ValueChangedMessage<OperationDetails>
    {
        public CalculationFinishedMessage(OperationDetails value) : base(value)
        {
        }
    }
}
