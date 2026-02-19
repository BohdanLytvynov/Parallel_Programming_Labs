using LabWork5_AvaloniaUI.Enums;

namespace LabWork5_AvaloniaUI.Models
{
    /// <summary>
    /// Matrix operation
    /// </summary>
    public class MatrixOperation
    {
        /// <summary>
        /// operation Name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Operation Type
        /// </summary>
        public Operation Operation { get; set; } = Operation.Add;
    }
}
