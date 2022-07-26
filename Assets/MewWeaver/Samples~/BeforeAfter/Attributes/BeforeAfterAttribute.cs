using System;

namespace Mewlist.Weaver.Sample
{
    [AttributeUsage(AttributeTargets.Method)] 
    public class BeforeAfterAttribute : Attribute
    {
        public BeforeAfterAttribute() { } 
    }
}