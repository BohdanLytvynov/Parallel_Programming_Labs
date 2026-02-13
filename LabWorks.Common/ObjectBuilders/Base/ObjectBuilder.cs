using LabWorks.Common.Attributes;
using LabWorks.Common.ObjectBuilders.Exceptions;
using System.Reflection;

namespace LabWorks.Common.ObjectBuilders.Base
{
    public class ObjectBuilder : IObjectBuilder
    {
        public object Create(Type type, Assembly assembly)
        {
            var ctor = type.GetConstructors()
                .FirstOrDefault(x => x.GetCustomAttribute<Ctor>() != null)
                ?? type.GetConstructors().FirstOrDefault();

            if (ctor == null)
            {
                return Activator.CreateInstance(type);
            }

            var parametersInfo = ctor.GetParameters();
            //No parameters - just build using empty ctor
            if (parametersInfo.Length == 0)
            {
                return Activator.CreateInstance(type);
            }

            List<object> args = new List<object>();

            foreach (var parameter in parametersInfo)
            {
                Type paramType = parameter.ParameterType;
                object arg = null;

                if (paramType.IsInterface)//Param is an interface
                {
                    //Get the realization
                    var implementationType = GetRealization(ctor.CustomAttributes, paramType, assembly);
                    //create parameter using recurtion. 
                    arg = Create(implementationType, assembly);
                }
                else if (paramType.IsClass && paramType != typeof(string))//Parameter is a Realization and not string
                {
                    arg = Create(paramType, assembly);//Create parameter recursive
                }
                else
                {
                    //Case of Struct Constraction
                    arg = paramType.IsValueType ? Activator.CreateInstance(paramType) : null;
                }

                args.Add(arg);
            }
            //Build object with all the parameters build
            return Activator.CreateInstance(type, args.ToArray());
        }

        private Type? GetRealization(IEnumerable<CustomAttributeData> customAttributes, Type interfaceType, Assembly assembly)
        {
            var attr = customAttributes.Where(x => x.AttributeType.Name.Equals("CtorParam`2"));

            foreach (var attribute in attr)
            { 
                var genTypes= attribute.AttributeType.GetGenericArguments();
                //Get interface name
                var inter = genTypes[0].Name;
                //Get realization name
                var real = genTypes[1].Name;

                if (inter.Equals(interfaceType.Name))
                { 
                    return assembly.DefinedTypes
                        .Where(x => x.Name.Equals(real))
                        .FirstOrDefault()?.AsType();
                }
            }

            return null;
        }
    }
}
