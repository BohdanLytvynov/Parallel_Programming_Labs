namespace LabWorks.Common.FactoryWrappers.Exceptions
{
    public class FactoryForProductWasNotFound : Exception
    {
        public FactoryForProductWasNotFound(string productName)
            : base($"Factory for product <{productName}> was not found!")
        {
            
        }
    }
}
