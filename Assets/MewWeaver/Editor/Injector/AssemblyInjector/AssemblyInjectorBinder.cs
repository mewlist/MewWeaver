using System;
using System.Collections.Generic;
using System.Linq;

namespace Mewlist.Weaver
{
    public class AssemblyInjectorBinder
    {
        protected AssemblyInjector AssemblyInjector { get; }
        protected List<string> TargetAssemblies { get; set; }

        public AssemblyInjectorBinder(AssemblyInjector assemblyInjector)
        {
            AssemblyInjector = assemblyInjector;
            TargetAssemblies = new List<string>();
        }

        public AssemblyInjectorBinder(AssemblyInjectorBinder other)
        {
            AssemblyInjector = other.AssemblyInjector;
            TargetAssemblies = other.TargetAssemblies.ToList();
        }

        private AssemblyInjectorBinder Clone()
        {
            return new AssemblyInjectorBinder(this);
        }

        public AssemblyInjectorBinder OnAssembly(string name)
        {
            var binder = Clone();
            binder.TargetAssemblies.Add(name);
            return binder;
        }

        public virtual AssemblyInjectorMethodBinder OnAttribute<T>()
            where T:Attribute
        {
            var binder = new AssemblyInjectorMethodBinder(this);
            return binder.OnAttribute<T>();
        }
    }
}