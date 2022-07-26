using System;
using System.Reflection;

namespace Mewlist.Weaver
{
    public class AssemblyInjectorMethodBinder : AssemblyInjectorBinder
    {
        private MethodInjectorBinder MethodInjectorBinder { get; set; }

        public AssemblyInjectorMethodBinder(AssemblyInjectorBinder other) : base(other)
        {
            MethodInjectorBinder = new MethodInjectorBinder();
        }

        private AssemblyInjectorMethodBinder(AssemblyInjectorMethodBinder other) : base(other)
        {
            MethodInjectorBinder = other.MethodInjectorBinder.Clone();
        }

        private AssemblyInjectorMethodBinder Clone()
        {
            return new AssemblyInjectorMethodBinder(this);
        }

        public override AssemblyInjectorMethodBinder OnAttribute<T>()
        {
            var binder = Clone();
            binder.MethodInjectorBinder = binder.MethodInjectorBinder.OnAttribute<T>();
            return binder;
        }

        public AssemblyInjectorMethodBinder Do(IILInjector ilInjector)
        {
            var binder = Clone();
            binder.MethodInjectorBinder = binder.MethodInjectorBinder.Do(ilInjector);
            return binder;
        }

        public AssemblyInjectorMethodBinder BeforeDo(Action method) => BeforeDo(method.Method);
        public AssemblyInjectorMethodBinder BeforeDo<TArg>(Action<TArg> method) => BeforeDo(method.Method);
        public AssemblyInjectorMethodBinder BeforeDo(MethodInfo methodInfo)
        {
            var binder = Clone();
            binder.MethodInjectorBinder = binder.MethodInjectorBinder.BeforeDo(methodInfo);
            return binder;
        }

        public AssemblyInjectorMethodBinder AfterDo(Action method) => AfterDo(method.Method);
        public AssemblyInjectorMethodBinder AfterDo<TArg>(Action<TArg> method) => AfterDo(method.Method);
        public AssemblyInjectorMethodBinder AfterDo(MethodInfo methodInfo)
        {
            var binder = Clone();
            binder.MethodInjectorBinder = binder.MethodInjectorBinder.AfterDo(methodInfo);
            return binder;
        }

        public void Inject()
        {
            foreach (var targetAssembly in TargetAssemblies)
            {
                WeaverLogger.Log($"Try inject into {targetAssembly}.dll");
                var assembly = AssemblyInjector.Find(targetAssembly);
                AssemblyInjector.Inject(assembly, MethodInjectorBinder);
            }
        }
    }
}