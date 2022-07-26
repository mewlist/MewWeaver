using System;

namespace Mewlist.Weaver.Sample
{
    [AttributeUsage(AttributeTargets.Method)] 
    public class BeforeAttribute : Attribute
    {
        public BeforeAttribute() { } 
    }
}