using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Mewlist.Weaver
{
    public abstract class AbstractMethodILInjector : IILInjector
    {
        protected MethodInfo MethodInfo { get; }

        protected AbstractMethodILInjector(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
        }

        public abstract void Inject(CustomAttribute customAttribute, ModuleDefinition moduleDefinition, MethodDefinition methodDefinition);

        public void Validate(ICustomAttribute customAttribute)
        {
            var attributeParameterTypes = customAttribute.ConstructorArguments
                .Select(x => x.Type)
                .ToArray();
            var parameterTypes = MethodInfo.GetParameters()
                .Take(attributeParameterTypes.Count())
                .Select(x => x.ParameterType)
                .ToArray();

            foreach (var x in parameterTypes)
            {
                var validParameterTypes = x.IsEnum || x == typeof(string) || x == typeof(int) || x == typeof(float);
                if (!validParameterTypes)
                    throw new Exception($"Argument type {x.Name} is not supported");
            }

            var parameterNames = parameterTypes.Select(x => x.FullName).ToArray(); 
            var attributeParameterNames = attributeParameterTypes.Take(parameterNames.Length).Select(x => x.FullName).ToArray();
            var validParameters = parameterNames.SequenceEqual(attributeParameterNames);
            if (!validParameters)
                throw new Exception($"Attribute and Method arguments not matched.");
        }
    }
}