using System;

namespace Mewlist.Weaver.Sample
{
    [AttributeUsage(AttributeTargets.Method)] 
    public class BeforeEnumAttribute : Attribute
    {
        public BeforeEnumAttribute(BeforeAfterEnum value) { }
    }
}