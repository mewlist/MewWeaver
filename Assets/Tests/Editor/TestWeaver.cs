namespace Mewlist.Weaver.Test
{
    public class TestWeaver : IWeaver
    {
        public void Weave(AssemblyInjector assemblyInjector)
        {
            assemblyInjector
                .OnAssembly("MewWeaver.Tests")
                .OnAttribute<WeaverTestAttribute>()
                .BeforeDo(CodeToInject.CallBeforeInjectionCode)
                .AfterDo(CodeToInject.CallAfterInjectionCode)
                .Inject();
        }
    }
}