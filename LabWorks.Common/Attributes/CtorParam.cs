namespace LabWorks.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class CtorParam<TInterface, TRealization> : Attribute
        where TRealization : class
    {
    }
}
