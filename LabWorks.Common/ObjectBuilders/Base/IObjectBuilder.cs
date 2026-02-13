using System.Reflection;

namespace LabWorks.Common.ObjectBuilders.Base
{
    public interface IObjectBuilder
    {
        object Create(Type type, Assembly assembly);
    }
}
