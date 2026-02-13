namespace LabWorks.Common.Factories.Base
{
    public abstract class AbstractFactoryBase<TEntity> : IAbstractFactoryBase<TEntity>
    {
        public string? ProductName { get; init; }

        public abstract TEntity Create();
    }
}
