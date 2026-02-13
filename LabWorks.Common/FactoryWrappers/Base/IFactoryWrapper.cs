namespace LabWorks.Common.FactoryWrappers.Base
{
    public interface IFactoryWrapper
    {
        TEntity Create<TEntity>();
    }
}
