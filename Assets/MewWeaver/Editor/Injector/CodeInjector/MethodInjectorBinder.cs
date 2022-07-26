using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Mewlist.Weaver
{
    public class MethodInjectorBinder : IMethodInjector
    {
        private List<Func<CustomAttribute, bool>> attributeValidators;
        private List<IILInjector> injectors;

        public MethodInjectorBinder()
        {
            attributeValidators = new();
            injectors = new();
        }

        private MethodInjectorBinder(MethodInjectorBinder methodInjector)
        {
            attributeValidators = methodInjector.attributeValidators.ToList();
            injectors = methodInjector.injectors.ToList();
        }

        public MethodInjectorBinder Clone()
        {
            return new MethodInjectorBinder(this);
        }

        public MethodInjectorBinder OnAttribute<TAttribute>()
            where TAttribute: Attribute
        {
            var injector = Clone();
            injector.attributeValidators.Add(customAttribute =>
                customAttribute.AttributeType.FullName == typeof(TAttribute).FullName);
            return injector;
        }

        public MethodInjectorBinder Do(IILInjector ilInjector)
        {
            var injector = Clone();
            injector.injectors.Add(ilInjector);
            return injector;
        }

        public MethodInjectorBinder BeforeDo(Action method) => BeforeDo(method.Method);
        public MethodInjectorBinder BeforeDo<TArg>(Action<TArg> method) => BeforeDo(method.Method);
        public MethodInjectorBinder BeforeDo(MethodInfo methodInfo)
        {
            if (!methodInfo.IsStatic)
                throw new ArgumentException("Non static method given.");
            return Do(new BeforeMethodILInjector(methodInfo));
        }

        public MethodInjectorBinder AfterDo(Action method) => AfterDo(method.Method);
        public MethodInjectorBinder AfterDo<TArg>(Action<TArg> method) => AfterDo(method.Method);
        public MethodInjectorBinder AfterDo(MethodInfo methodInfo)
        {
            if (!methodInfo.IsStatic)
                throw new ArgumentException("Non static method given.");
            return Do(new AfterMethodILInjector(methodInfo));
        }

        public void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition)
        {
            var customAttributes = methodDefinition.CustomAttributes.ToArray();
            foreach (var customAttribute in customAttributes)
            {
                if (attributeValidators.Any(x => x(customAttribute)))
                { 
                    foreach (var injector in injectors)
                    {
                        WeaverLogger.Log($"[{customAttribute.AttributeType.Name}] found on {methodDefinition.DeclaringType.Namespace}.{methodDefinition.Name}()");
                        injector.Validate(customAttribute);
                        injector.Inject(customAttribute, moduleDefinition, methodDefinition);
                    }
                    methodDefinition.CustomAttributes.Remove(customAttribute);
                }
            }
        }
    }
}