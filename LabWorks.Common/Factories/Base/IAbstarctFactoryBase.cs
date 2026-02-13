namespace LabWorks.Common.Factories.Base
{
    public interface IAbstractFactoryBase<TEntity> : IFactory
    {
        TEntity Create();
    }
}
