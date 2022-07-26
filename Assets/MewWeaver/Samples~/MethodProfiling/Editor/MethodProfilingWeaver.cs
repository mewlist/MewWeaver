namespace Mewlist.Weaver.Sample
{
    public class MethodProfilingWeaver : IWeaver
    {
        public void Weave(AssemblyInjector assemblyInjector)
        {
            assemblyInjector
                .OnMainAssembly()
                .OnAttribute<MethodProfilingAttribute>()
                .Do(new MethodProfilingILInjector(MethodProfilingLogger.Log))
                .Inject();
        }
    }
}