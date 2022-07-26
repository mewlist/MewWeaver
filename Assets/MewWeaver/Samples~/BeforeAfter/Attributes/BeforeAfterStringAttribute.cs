using System;

namespace Mewlist.Weaver.Sample
{
    [AttributeUsage(AttributeTargets.Method)] 
    public class BeforeAfterStringAttribute : Attribute
    {
        public BeforeAfterStringAttribute(string value) { }
    }
}