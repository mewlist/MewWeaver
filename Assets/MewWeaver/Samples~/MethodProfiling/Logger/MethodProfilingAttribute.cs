using System;

namespace Mewlist.Weaver.Sample
{
    [AttributeUsage(AttributeTargets.Method)] 
    public class MethodProfilingAttribute : Attribute
    {
        public MethodProfilingAttribute() { }
    }
}