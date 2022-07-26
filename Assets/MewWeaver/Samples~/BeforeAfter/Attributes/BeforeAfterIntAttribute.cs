using System;

namespace Mewlist.Weaver.Sample
{
    [AttributeUsage(AttributeTargets.Method)] 
    public class BeforeAfterIntAttribute : Attribute
    {
        public BeforeAfterIntAttribute(int value) { }
    }
}