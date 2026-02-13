using LabWorks.Common.Factories.Base;
using LabWorks.Common.FactoryWrappers.Exceptions;
using LabWorks.Common.ObjectBuilders.Base;
using System.Reflection;

namespace LabWorks.Common.FactoryWrappers.Base
{
    public class FactoryWrapper : IFactoryWrapper
    {
        private readonly Dictionary<string, IFactory> m_factoryMap;

        private readonly IObjectBuilder m_objectBuilder;

        public FactoryWrapper(Assembly assembly, IObjectBuilder objectBuilder)
        {
            m_objectBuilder = objectBuilder ?? throw new ArgumentNullException(nameof(objectBuilder));

            m_factoryMap = new Dictionary<string, IFactory>();

            var factories = assembly.DefinedTypes.Where(x => x.IsAssignableTo(typeof(IFactory)));

            foreach (var factory in factories)
            {
                var f = m_objectBuilder.Create(factory.AsType(), assembly) as IFactory;
                
                if(f == null) continue;

                m_factoryMap.Add(f.ProductName, f);
            }
        }

        public virtual TEntity Create<TEntity>()
        {
            string productName = typeof(TEntity).Name;

            if (!m_factoryMap.ContainsKey(productName))
            {
                throw new FactoryForProductWasNotFound(productName);
            }

            var concreteFactory = m_factoryMap[productName] as IAbstractFactoryBase<TEntity>;

            if (concreteFactory == null)
                throw new InvalidOperationException("Unable to cast to the concrete Factory!");

            return concreteFactory.Create();
        }
    }
}
