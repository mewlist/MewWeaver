using System;

namespace Mewlist.Weaver
{
    public static class MethodInjector
    {
        public static MethodInjectorBinder OnAttribute<TAttribute>()
            where TAttribute: Attribute
        {
            return new MethodInjectorBinder().OnAttribute<TAttribute>();
        }
    }
}