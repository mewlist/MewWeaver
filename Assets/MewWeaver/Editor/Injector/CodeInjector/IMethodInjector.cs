using Mono.Cecil;

namespace Mewlist.Weaver
{
    public interface IMethodInjector
    {
        void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition);
    }
}