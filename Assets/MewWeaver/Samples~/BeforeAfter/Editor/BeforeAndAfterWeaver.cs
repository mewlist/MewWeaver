using UnityEngine;

namespace Mewlist.Weaver.Sample
{
    public class BeforeAndAfterWeaver : IWeaver
    {
        public void Weave(AssemblyInjector assemblyInjector)
        {
            Debug.Log("Weaver.Inject");

            assemblyInjector
                .OnAssembly("Assembly-CSharp")
                .OnAttribute<BeforeAttribute>()
                .BeforeDo(BeforeAfter.Before)
                .Inject();

            assemblyInjector
                .OnAssembly("Assembly-CSharp")
                .OnAttribute<BeforeAfterAttribute>()
                .BeforeDo(BeforeAfter.Before)
                .AfterDo(BeforeAfter.After)
                .Inject();

            assemblyInjector
                .OnAssembly("Assembly-CSharp")
                .OnAttribute<BeforeAfterIntAttribute>()
                .BeforeDo<int>(BeforeAfter.BeforeInt)
                .AfterDo<int>(BeforeAfter.AfterInt)
                .Inject();

            assemblyInjector
                .OnAssembly("Assembly-CSharp")
                .OnAttribute<BeforeEnumAttribute>()
                .BeforeDo<BeforeAfterEnum>(BeforeAfter.BeforeEnum)
                .Inject();

            assemblyInjector
                .OnAssembly("Assembly-CSharp")
                .OnAttribute<BeforeAfterStringAttribute>()
                .BeforeDo<string>(BeforeAfter.BeforeString)
                .AfterDo<string>(BeforeAfter.AfterString)
                .Inject();
        }  
    }
} 