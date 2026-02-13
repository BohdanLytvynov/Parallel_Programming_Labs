namespace LabWorks.Common.ObjectBuilders.Exceptions
{
    public class CtorAttributeNotFoundException : Exception
    {
        public CtorAttributeNotFoundException(string typeName)
            : base($"Ctor Attribute was not found on the constructor of type: <{typeName}>")
        {
            
        }
    }
}
